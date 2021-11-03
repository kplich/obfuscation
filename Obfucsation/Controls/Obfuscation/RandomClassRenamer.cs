using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Obfucsation.Controls.Obfuscation
{
    internal class RandomClassRenamer : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            return node.WithIdentifier(SyntaxFactory.Identifier(Guid.NewGuid().ToString().Replace("-", "")));
        }
    }
}
