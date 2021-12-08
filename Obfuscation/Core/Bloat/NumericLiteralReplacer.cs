using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Obfuscation.Core.Name;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Obfuscation.Core.Bloat.SyntaxTriviaUtils;

namespace Obfuscation.Core.Bloat
{
    public class NumericLiteralReplacer: CSharpSyntaxRewriter
    {
        private class LiteralExpressionInfo
        {
            public LiteralExpressionSyntax Value { get; set; }
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
                    Value = literalExpressionSyntax,
                    NewName = newPropertyName,
                    Member = literalExpressionSyntax.AsPlainProperty(newPropertyName)
                });
            }

            return base.VisitClassDeclaration(node.AddMembers(_untransformedLiterals.Select(literalInfo => literalInfo.Member).ToArray()));
        }

        public override SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            var literalInfo = _untransformedLiterals.FirstOrDefault(literalInfo => literalInfo.Value.Token.Text.Equals(node.Token.Text));
            if (literalInfo != null)
            {
                _untransformedLiterals.Remove(literalInfo);
                return IdentifierName(literalInfo.NewName);
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
    }
}