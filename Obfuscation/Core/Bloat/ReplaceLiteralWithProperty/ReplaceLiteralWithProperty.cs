using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Obfuscation.Core.Bloat.Property;
using Obfuscation.Core.Name;
using Obfuscation.Utils;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Obfuscation.Core.Bloat.SyntaxTriviaUtils;
using static Obfuscation.Core.DoNotObfuscate;

namespace Obfuscation.Core.Bloat.ReplaceLiteralWithProperty
{
    public class ReplaceLiteralWithProperty : CSharpSyntaxRewriter
    {
        private readonly IImmutableList<IIdentifierGenerator> _identifierGenerators;
        // private readonly string _doNotObfuscateAttributeName; // TODO: how should this be used???

        private readonly IImmutableList<PropertyGenerator> _propertyGenerators;

        private readonly IDictionary<string, PropertyDeclarationSyntax> _literalsAndProperties =
            new Dictionary<string, PropertyDeclarationSyntax>();

        private readonly IList<PropertyGenerator> _generatorsUsed = new List<PropertyGenerator>();

        public ReplaceLiteralWithProperty(IImmutableList<IIdentifierGenerator> identifierGenerators, IImmutableList<PropertyGenerator> propertyGenerators)
        {
            _identifierGenerators = identifierGenerators;
            _propertyGenerators = propertyGenerators;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // if (node.HasAnAttributeWithName(_doNotObfuscateAttributeName)) return base.VisitClassDeclaration(node);

            var literals = node.DescendantNodes().OfType<LiteralExpressionSyntax>();

            foreach (var literal in literals)
            {
                if (literal.CanBeReplacedByProperty()) continue;
                
                var suitableGenerators =
                    _propertyGenerators
                        .Where(generator => generator.Supports(literal))
                        .ToList();

                if (suitableGenerators.Count <= 0) continue;
                
                var generator = suitableGenerators.GetRandomElement();
                _literalsAndProperties[literal.Token.Text] = generator.GenerateProperty(literal);
                _generatorsUsed.Add(generator);
            }
            
            var properties = _literalsAndProperties.Values.ToArray() as MemberDeclarationSyntax[];
            
            var preparedClass = _generatorsUsed
                .Aggregate(node, (current, generator) => generator.PrepareClass(current));

            return base.VisitClassDeclaration(
                preparedClass.AddMembers(properties)
                    );

        }

        public override SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            /*if (node.HasAParentWithAttributeName(_doNotObfuscateAttributeName))
            {
                return base.VisitLiteralExpression(node);
            }*/

            if (!_literalsAndProperties.ContainsKey(node.Token.Text))
            {
                return base.VisitLiteralExpression(node);
            }

            var property = _literalsAndProperties[node.Token.Text];
            return IdentifierName(property.Identifier).WithTrailingTrivia(SpaceTrivia());
        }

        public IIdentifierGenerator ChooseGenerator()
        {
            return _identifierGenerators[new Random().Next(_identifierGenerators.Count)];
        }
    }
    
    internal static class ReplaceLiteralWithPropertyUtils
    {
        internal static bool IsOfNumericType(this LiteralExpressionSyntax node)
        {
            return node.Kind() == SyntaxKind.NumericLiteralExpression;
        }

        internal static bool NeedsToBeStatic(this LiteralExpressionSyntax node)
        {
            var parentMethod = node.GetParent<MethodDeclarationSyntax>();
            var parentMethodIsStatic = parentMethod?.IsStatic() ?? false;

            return parentMethodIsStatic || node.IsWithin<ClassDeclarationSyntax>();
        }

        internal static bool CanBeReplacedByProperty(this LiteralExpressionSyntax node)
        {
            return node.IsWithin<ParameterSyntax>();
        }
    }
}