using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Obfuscation.Core.Bloat.Property
{
    public interface IPropertyGenerator
    {
        PropertyDeclarationSyntax GenerateProperty(LiteralExpressionSyntax literal, string newName);
    }
}