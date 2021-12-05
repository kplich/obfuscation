using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Obfuscation.Core.Name;

namespace Obfuscation.Core.Rename
{
    public class RandomMethodRenamer : CodeRenamer
    {
        public RandomMethodRenamer(IImmutableList<IIdentifierGenerator> generators) : base(generators)
        {
        }

        protected override async Task<Solution> RewriteCode(Solution solution, SyntaxTree syntaxTree, SemanticModel semanticModel)
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
                        ChooseGenerator().GenerateMethodName(methodSymbol.Name), solution.Workspace.Options);
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
            methodSymbol.Arity == 0;

        internal static bool IsNotMain(this IMethodSymbol methodSymbol) => !methodSymbol.IsMain();
    }
}