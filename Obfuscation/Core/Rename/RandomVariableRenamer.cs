using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace Obfuscation.Core
{
    public class RandomVariableRenamer : CodeRenamer
    {
        public override async Task<Solution> RewriteCode(Solution solution, SyntaxTree syntaxTree, SemanticModel semanticModel)
        {
            var variableDeclarations = (await syntaxTree.GetRootAsync()).DescendantNodes().OfType<VariableDeclaratorSyntax>();
            var variableSymbols
                = variableDeclarations
                    .Select(variableSymbol => semanticModel.GetDeclaredSymbol(variableSymbol))
                    .Where(symbol => symbol != null);
            
            foreach (var variableSymbol in variableSymbols)
            {
                solution = await Renamer.RenameSymbolAsync(solution, variableSymbol,
                    Guid.NewGuid().ToString().Replace("-", "").Insert(0, "_"), solution.Workspace.Options);
            }

            return solution;
        }
    }
}