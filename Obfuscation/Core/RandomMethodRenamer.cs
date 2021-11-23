using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace Obfuscation.Core
{
    public class RandomMethodRenamer : CodeRenamer
    {
        public override async Task<Solution> RewriteCode(Solution solution, SyntaxTree syntaxTree, SemanticModel semanticModel)
        {
            var methodDeclarations = (await syntaxTree.GetRootAsync()).DescendantNodes().OfType<MethodDeclarationSyntax>();
            var methodSymbols
                = methodDeclarations
                    .Select(methodDeclaration => semanticModel.GetDeclaredSymbol(methodDeclaration))
                    .Where(symbol => symbol != null)
                    .OfType<IMethodSymbol>();
            
            foreach (var methodSymbol in methodSymbols)
            {
                if (methodSymbol.IsNotMain())
                {
                    solution = await Renamer.RenameSymbolAsync(solution, methodSymbol,
                        Guid.NewGuid().ToString().Replace("-", "").Insert(0, "_"), solution.Workspace.Options);
                }
            }

            return solution;
        }
    }

    internal static class RandomMethodRenamerUtils
    {
        private static bool IsMain(this IMethodSymbol methodSymbol) =>
            methodSymbol.IsStatic &&
            methodSymbol.Name == "Main" &&
            methodSymbol.ReturnsVoid &&
            methodSymbol.Arity == 0 &&
            methodSymbol.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Private;

        internal static bool IsNotMain(this IMethodSymbol methodSymbol) => !methodSymbol.IsMain();
    }
}