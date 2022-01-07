using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Obfuscation.Utils;

namespace Obfuscation.Core.Bloat.ReplaceLiteralWithProperty
{
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
    }
}