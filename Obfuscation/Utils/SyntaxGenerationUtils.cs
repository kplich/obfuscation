using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Obfuscation.Utils
{
    public static class SyntaxGenerationUtils
    {
        
        public static SyntaxList<AttributeListSyntax> AttributeListWithSingleAttribute(string attributeName)
        {
            return new SyntaxList<AttributeListSyntax>().Add(
                AttributeList(
                    new SeparatedSyntaxList<AttributeSyntax>().Add(Attribute(IdentifierName(attributeName)))
                ).WithTrailingTrivia(CarriageReturn));
        }
        
        public static ForStatementSyntax ArrayBasedForLoop(string loopIndexName, string arrayName, BlockSyntax block)
        {
            return ForStatement(
                IntVariableValueWithValue(loopIndexName, "0"),
                new SeparatedSyntaxList<ExpressionSyntax>(),
                ParseExpression($"{loopIndexName} < {arrayName}.Length").WithLeadingTrivia(Space),
                IndexIncrementor(loopIndexName),
                block
            ).WithTrailingTrivia(CarriageReturn, CarriageReturn);
        }
        
        private static SeparatedSyntaxList<ExpressionSyntax> IndexIncrementor(string indexName)
        {
            return new SeparatedSyntaxList<ExpressionSyntax>()
                .Add(ParseExpression($"{indexName}++").WithLeadingTrivia(Space));
        }

        private static VariableDeclarationSyntax IntVariableValueWithValue(string name, string expression)
        {
            return VariableDeclaration(
                IntWithSpace(),
                new SeparatedSyntaxList<VariableDeclaratorSyntax>()
                    .Add(VariableWithValue(name, expression))
            );
        }

        private static PredefinedTypeSyntax IntWithSpace()
        {
            return PredefinedType(Token(SyntaxKind.IntKeyword).WithTrailingTrivia(Space));
        }

        private static VariableDeclaratorSyntax VariableWithValue(string name, string expression)
        {
            return VariableDeclarator(
                Identifier(name),
                null,
                EqualsValueClause(ParseExpression(expression))
            );
        }
    }
}