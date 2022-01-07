using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Obfuscation.Core.Name;
using Obfuscation.Utils;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Obfuscation.Core.Bloat.SyntaxTriviaUtils;
using static Obfuscation.Core.DoNotObfuscate;

namespace Obfuscation.Core.Bloat.ReplaceLiteralWithProperty.Plain
{
    public class ReplaceLiteralWithPlainProperty : CSharpSyntaxRewriter
    {
        private class LiteralExpressionInfo
        {
            public LiteralExpressionSyntax Literal { get; init; }
            public string NewName { get; init; }
        }

        private readonly IImmutableList<IIdentifierGenerator> _identifierGenerators;
        private readonly string _doNotObfuscateAttributeName;

        private readonly IDictionary<string, LiteralExpressionInfo> _mapOfLiterals =
            new Dictionary<string, LiteralExpressionInfo>();

        public ReplaceLiteralWithPlainProperty(IImmutableList<IIdentifierGenerator> generators)
        {
            _identifierGenerators = generators;
            _doNotObfuscateAttributeName = ChooseGenerator().TransformClassName(string.Empty);
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node.HasAnAttributeWithName(_doNotObfuscateAttributeName)) return base.VisitClassDeclaration(node);

            var numericLiterals = node.DescendantNodes().OfType<LiteralExpressionSyntax>()
                .Where(syntaxNode => syntaxNode.Kind() == SyntaxKind.NumericLiteralExpression);

            foreach (var literalExpressionSyntax in numericLiterals)
            {
                if (!literalExpressionSyntax.IsWithin<ParameterSyntax>())
                {
                    var newPropertyName = ChooseGenerator().TransformName(string.Empty);
                    _mapOfLiterals[literalExpressionSyntax.Token.Text] = new LiteralExpressionInfo
                    {
                        Literal = literalExpressionSyntax,
                        NewName = newPropertyName
                    };
                }
            }

            var propertyGenerator = new PlainPropertyGenerator();

            var doNotObfuscateAttribute = GenerateDoNotObfuscateAttribute(_doNotObfuscateAttributeName);

            var literalsAsProperties =
                _mapOfLiterals
                    .Values
                    .Select(literalInfo =>
                        propertyGenerator.GenerateProperty(literalInfo.Literal, literalInfo.NewName))
                    .ToArray() as MemberDeclarationSyntax[];
            return base.VisitClassDeclaration(
                node.AddMembers(literalsAsProperties)
                    .AddMembers(doNotObfuscateAttribute));
        }

        public override SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            if (!_mapOfLiterals.ContainsKey(node.Token.Text) || node.HasAParentWithAttributeName(_doNotObfuscateAttributeName))
                return base.VisitLiteralExpression(node);

            var literalInfo = _mapOfLiterals[node.Token.Text];
            return IdentifierName(literalInfo.NewName).WithTrailingTrivia(SpaceTrivia());
        }

        private IIdentifierGenerator ChooseGenerator()
        {
            return _identifierGenerators[new Random().Next(_identifierGenerators.Count)];
        }
    }
}