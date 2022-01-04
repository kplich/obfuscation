using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Obfuscation.Core.Name;
using Obfuscation.Utils;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Obfuscation.Core.Bloat.SyntaxTriviaUtils;

namespace Obfuscation.Core.Bloat
{
    public class ClassBloater : CSharpSyntaxRewriter
    {
        private static readonly List<TypeSyntax> PrimitiveTypes =
            new List<string>
            {
                "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "ufloat", "double",
                "char", "string", "decimal", "bool"

            }.Select(typeString => ParseTypeName(typeString)).ToList();

        private readonly IIdentifierGenerator _identifierGenerator = new GuidIdentifierGenerator();

        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            return node.WithMembers(node.Members.Add(GenerateClass()));
        }

        private ClassDeclarationSyntax GenerateClass()
        {
            var randomName = _identifierGenerator.TransformName(string.Empty);
            var trailingWhitespaces = SyntaxTriviaList.Create(CarriageReturn);

            var identifierToken = Identifier(SyntaxTriviaList.Empty, randomName, trailingWhitespaces);

            return ClassDeclaration(identifierToken)
                .WithTrailingTrivia(CarriageReturn)
                .WithKeyword(Token(SyntaxKind.ClassKeyword)
                    .WithLeadingTrivia(TabulatorTrivia(1))
                    .WithTrailingTrivia(SpaceTrivia()))
                .WithOpenBraceToken(
                    Token(SyntaxKind.OpenBraceToken)
                        .WithLeadingTrivia(TabulatorTrivia(1))
                        .WithTrailingTrivia(CarriageReturn))
                .WithCloseBraceToken(
                    Token(SyntaxKind.CloseBraceToken)
                        .WithLeadingTrivia(TabulatorTrivia(1))
                        .WithTrailingTrivia(CarriageReturn)
                )
                .WithTrailingTrivia(CarriageReturn)
                .AddMembers(GeneratePrimitiveProperty());
        }

        private MemberDeclarationSyntax GeneratePrimitiveProperty()
        {
            var type = PrimitiveTypes.GetRandomElement()
                .WithTrailingTrivia(SpaceTrivia());
            
            var randomName = _identifierGenerator.TransformName(string.Empty);
            var trailingWhitespaces = SpaceTrivia();

            var identifierToken = Identifier(SyntaxTriviaList.Empty, randomName, trailingWhitespaces);

            return PropertyDeclaration(type, identifierToken)
                .AddModifiers(
                    Token(SyntaxKind.PublicKeyword)
                        .WithLeadingTrivia(TabulatorTrivia(2))
                        .WithTrailingTrivia(SpaceTrivia()))
                .AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithLeadingTrivia(SpaceTrivia())
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                        .WithTrailingTrivia(SpaceTrivia()))
                .AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                        .WithTrailingTrivia(SpaceTrivia()))
                .WithTrailingTrivia(CarriageReturn);
        }
    }
}