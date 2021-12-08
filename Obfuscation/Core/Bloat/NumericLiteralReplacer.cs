using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Obfuscation.Annotations;
using Obfuscation.Core.Name;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Obfuscation.Core.Bloat.SyntaxTriviaUtils;

namespace Obfuscation.Core.Bloat
{
    public class NumericLiteralReplacer: CSharpSyntaxRewriter
    {
        private class LiteralExpressionInfo
        {
            public LiteralExpressionSyntax Literal { get; set; }
            public string NewName { get; set; }
            public MemberDeclarationSyntax Member { get; set; }
        }

        private readonly IImmutableList<IIdentifierGenerator> _identifierGenerators;
        private readonly IList<LiteralExpressionInfo> _untransformedLiterals = new List<LiteralExpressionInfo>();

        public NumericLiteralReplacer(IImmutableList<IIdentifierGenerator> generators)
        {
            _identifierGenerators = generators;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var numericLiterals = node.DescendantNodes().OfType<LiteralExpressionSyntax>()
                .Where(syntaxNode => syntaxNode.Kind() == SyntaxKind.NumericLiteralExpression);
            
            foreach (var literalExpressionSyntax in numericLiterals)
            {
                var newPropertyName = ChooseGenerator().TransformName(string.Empty);
                _untransformedLiterals.Add(new LiteralExpressionInfo
                {
                    Literal = literalExpressionSyntax,
                    NewName = newPropertyName,
                    Member = literalExpressionSyntax.AsPlainProperty(newPropertyName)
                });
            }

            return base.VisitClassDeclaration(node.AddMembers(_untransformedLiterals.Select(literalInfo => literalInfo.Member).ToArray()));
        }

        public override SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            var literalInfo = _untransformedLiterals.FirstOrDefault(literalInfo => literalInfo.Literal.Token.Text.Equals(node.Token.Text));
            if (literalInfo != null)
            {
                var literal = literalInfo.Literal;
                var literalIsWithinArgument = literal.GetParent<ParameterSyntax>() != null;

                if (!literalIsWithinArgument)
                {
                    _untransformedLiterals.Remove(literalInfo);
                    return IdentifierName(literalInfo.NewName);
                }
                else
                {
                    return base.VisitLiteralExpression(node);
                }
            }
            else
            {
                return base.VisitLiteralExpression(node);
            }
        }

        public IIdentifierGenerator ChooseGenerator()
        {
            return _identifierGenerators[new Random().Next(_identifierGenerators.Count)];
        }
    }

    internal static class NumericLiteralReplacerUtils
    {
        internal static bool IsOfNumericType(this LiteralExpressionSyntax node)
        {
            return node.Kind() == SyntaxKind.NumericLiteralExpression;
        }

        internal static PropertyDeclarationSyntax AsPlainProperty(this LiteralExpressionSyntax node, string name)
        {
            if (node.IsOfNumericType())
            {
                var attributeLists = new SyntaxList<AttributeListSyntax>();
                var modifiers = new SyntaxTokenList(
                    Token(SyntaxKind.PublicKeyword)
                        .WithLeadingTrivia(TabulatorTrivia(2))
                        .WithTrailingTrivia(SpaceTrivia()));

                // sometimes, the property needs to be static!
                var parentMethod = node.GetParentMethod();
                var parentMethodIsStatic =
                    parentMethod?.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.StaticKeyword)) ?? false;
                var fieldDeclaration = node.GetParentClass();
                var literalBelongsToAClass = fieldDeclaration != null;

                if (parentMethodIsStatic || literalBelongsToAClass)
                {
                    modifiers = modifiers.Add(Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(SpaceTrivia()));
                }

                var nodeText = node.Token.Text;
                var type = PredefinedType(nodeText.Contains(".")
                    ? Token(SyntaxKind.DoubleKeyword)
                    : Token(SyntaxKind.IntKeyword)).WithTrailingTrivia(Space);

                var identifierToken = Identifier(SyntaxTriviaList.Empty, name, SpaceTrivia());

                var arrowExpressionClauseSyntax = ArrowExpressionClause(
                    Token(SyntaxKind.EqualsGreaterThanToken)
                        .WithTrailingTrivia(Space),
                    LiteralExpression(SyntaxKind.NumericLiteralExpression, node.Token)
                );

                var semicolon = Token(SyntaxKind.SemicolonToken);

                return PropertyDeclaration(attributeLists, modifiers, type, null, identifierToken, null,
                        arrowExpressionClauseSyntax, null, semicolon)
                    .WithLeadingTrivia(DoubleEndOfLineTrivia());
            }
            else return null;
        }

        [CanBeNull]
        internal static MethodDeclarationSyntax GetParentMethod(this SyntaxNode node)
        {
            var parentNode = node.Parent;
            while (parentNode is not MethodDeclarationSyntax && parentNode != null)
            {
                parentNode = parentNode.Parent;
            }
            return parentNode as MethodDeclarationSyntax;
        }
        
        [CanBeNull]
        internal static ClassDeclarationSyntax GetParentClass(this SyntaxNode node)
        {
            var parentNode = node.Parent;
            while (parentNode is not ClassDeclarationSyntax && parentNode != null)
            {
                parentNode = parentNode.Parent;
            }
            return parentNode as ClassDeclarationSyntax;
        }

        internal static ParameterSyntax GetParentParameter(this SyntaxNode node)
        {
            var parentNode = node.Parent;
            while (parentNode is not ParameterSyntax && parentNode != null)
            {
                parentNode = parentNode.Parent;
            }
            return parentNode as ParameterSyntax;
        }

        internal static T GetParent<T>(this SyntaxNode node) where T : SyntaxNode
        {
            var parentNode = node.Parent;
            while (parentNode is not T && parentNode != null)
            {
                parentNode = parentNode.Parent;
            }

            return parentNode as T;
        }
    }
}