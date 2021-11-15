using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace Obfuscation.Core
{
    public class RandomClassRenamer : CodeRewriter
    {
        public override async Task<Solution> RewriteCode(Solution solution, SyntaxTree syntaxTree, SemanticModel semanticModel)
        {
            var classes = (await syntaxTree.GetRootAsync()).DescendantNodes().OfType<ClassDeclarationSyntax>();
            var classSymbols
                = classes
                    .Select(classSyntax => semanticModel.GetDeclaredSymbol(classSyntax))
                    .Where(symbol => symbol != null);
            
            foreach (var classSymbol in classSymbols)
            {
                solution = await Renamer.RenameSymbolAsync(solution, classSymbol,
                    Guid.NewGuid().ToString().Replace("-", "").Insert(0, "_"), solution.Workspace.Options);
            }

            return solution;
        }
    }
}