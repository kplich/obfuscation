using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Obfuscation.Utils.SyntaxGenerationUtils;

namespace Obfuscation.Core
{
    public static class DoNotObfuscate
    {
        public static MemberDeclarationSyntax GenerateDoNotObfuscateAttribute(string attributeName)
        {
            return ParseMemberDeclaration($"public class {attributeName} : Attribute " + "{}")
                .WithAttributeLists(AttributeListWithSingleAttribute(attributeName))
                .WithTrailingTrivia(CarriageReturn, CarriageReturn);
        }

        public static ClassDeclarationSyntax AddDoNotObfuscateAttribute(this ClassDeclarationSyntax classDeclarationSyntax, string attributeName)
        {
            return classDeclarationSyntax.AddMembers(
                GenerateDoNotObfuscateAttribute(attributeName)
            );
        }
    }
}