using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Obfuscation.Core.Name;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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

        private readonly INameGenerator _nameGenerator = new GuidNameGenerator();

        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            return node.WithMembers(node.Members.Add(GenerateClass()));
        }

        private ClassDeclarationSyntax GenerateClass()
        {
            var randomName = _nameGenerator.GenerateName();
            var trailingWhitespaces = BloaterUtils.EndOfLineTrivia();

            var identifierToken = Identifier(SyntaxTriviaList.Empty, randomName, trailingWhitespaces);

            return ClassDeclaration(identifierToken)
                .WithTrailingTrivia(BloaterUtils.EndOfLineTrivia())
                .WithKeyword(Token(SyntaxKind.ClassKeyword)
                    .WithLeadingTrivia(BloaterUtils.TabulatorTrivia(1))
                    .WithTrailingTrivia(BloaterUtils.SpaceTrivia()))
                .WithOpenBraceToken(
                    Token(SyntaxKind.OpenBraceToken)
                        .WithLeadingTrivia(BloaterUtils.TabulatorTrivia(1))
                        .WithTrailingTrivia(BloaterUtils.EndOfLineTrivia()))
                .WithCloseBraceToken(
                    Token(SyntaxKind.CloseBraceToken)
                        .WithLeadingTrivia(BloaterUtils.TabulatorTrivia(1))
                        .WithTrailingTrivia(BloaterUtils.EndOfLineTrivia())
                )
                .WithTrailingTrivia(BloaterUtils.EndOfLineTrivia())
                .AddMembers(GeneratePrimitiveProperty());
        }

        private MemberDeclarationSyntax GeneratePrimitiveProperty()
        {
            var type = PrimitiveTypes.GetRandomElement()
                .WithTrailingTrivia(BloaterUtils.SpaceTrivia());
            
            var randomName = _nameGenerator.GenerateName();
            var trailingWhitespaces = BloaterUtils.SpaceTrivia();

            var identifierToken = Identifier(SyntaxTriviaList.Empty, randomName, trailingWhitespaces);

            return PropertyDeclaration(type, identifierToken)
                .AddModifiers(
                    Token(SyntaxKind.PublicKeyword)
                        .WithLeadingTrivia(BloaterUtils.TabulatorTrivia(2))
                        .WithTrailingTrivia(BloaterUtils.SpaceTrivia()))
                .AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithLeadingTrivia(BloaterUtils.SpaceTrivia())
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                        .WithTrailingTrivia(BloaterUtils.SpaceTrivia()))
                .AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                        .WithTrailingTrivia(BloaterUtils.SpaceTrivia()))
                .WithTrailingTrivia(BloaterUtils.EndOfLineTrivia());
        }
    }

    internal static class ListUtil
    {
        public static T GetRandomElement<T>(this List<T> list)
        {
            return list[new Random().Next(list.Count)];
        }
    }

    internal static class BloaterUtils
    {
        public static SyntaxTriviaList SpaceTrivia()
        {
            return new SyntaxTriviaList(SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " "));
        }
        
        public static SyntaxTriviaList TabulatorTrivia(int numberOfTabs, int spacesInTab = 4)
        {
            var whitespaceString = " ".Repeat(numberOfTabs * spacesInTab);
            return new SyntaxTriviaList(SyntaxTrivia(SyntaxKind.WhitespaceTrivia, whitespaceString));
        }

        public static SyntaxTriviaList EndOfLineTrivia()
        {
            return new SyntaxTriviaList(SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "\n"));
        }
    }

    internal static class StringUtils
    {
        public static string Repeat(this string s, int numberOfTimes)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < numberOfTimes; i++)
            {
                builder.Append(s);
            }

            return builder.ToString();
        }
    }
}