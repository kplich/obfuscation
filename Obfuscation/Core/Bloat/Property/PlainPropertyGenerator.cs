using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Obfuscation.Core.Bloat.SyntaxTriviaUtils;

namespace Obfuscation.Core.Bloat.Property
{
    public class PlainPropertyGenerator : IPropertyGenerator
    {
        public PropertyDeclarationSyntax GenerateProperty(LiteralExpressionSyntax literal, string newName)
        {
            if (!literal.IsOfNumericType()) return null;
            
            var attributeLists = new SyntaxList<AttributeListSyntax>();
            var modifiers = new SyntaxTokenList(
                Token(SyntaxKind.PublicKeyword)
                    .WithLeadingTrivia(TabulatorTrivia(2))
                    .WithTrailingTrivia(SpaceTrivia()));

            // sometimes, the property needs to be static!
            if (literal.NeedsToBeStatic())
            {
                modifiers = modifiers.Add(Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(SpaceTrivia()));
            }

            var nodeText = literal.Token.Text;
            var type = PredefinedType(nodeText.Contains(".")
                ? Token(SyntaxKind.DoubleKeyword)
                : Token(SyntaxKind.IntKeyword)).WithTrailingTrivia(Space);

            var identifierToken = Identifier(SyntaxTriviaList.Empty, newName, SpaceTrivia());

            var arrowExpressionClauseSyntax = ArrowExpressionClause(
                Token(SyntaxKind.EqualsGreaterThanToken).WithTrailingTrivia(Space),
                LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(int.Parse(literal.Token.ValueText))));

            var semicolon = Token(SyntaxKind.SemicolonToken);

            return PropertyDeclaration(attributeLists, modifiers, type, null, identifierToken, null,
                    arrowExpressionClauseSyntax, null, semicolon)
                .WithTrailingTrivia(CarriageReturn, CarriageReturn);
        }
    }
}