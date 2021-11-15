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
                    .Select(methodDeclaration => semanticModel.GetDeclaredSymbol(methodDeclaration)!)
                    .Where(symbol => symbol != null);
            
            foreach (var methodSymbol in methodSymbols)
            {
                solution = await Renamer.RenameSymbolAsync(solution, methodSymbol,
                    Guid.NewGuid().ToString().Replace("-", "").Insert(0, "_"), solution.Workspace.Options);
            }

            return solution;
        }
    }
}