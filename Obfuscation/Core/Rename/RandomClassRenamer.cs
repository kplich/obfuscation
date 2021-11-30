using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace Obfuscation.Core
{
    public class RandomClassRenamer : CodeRenamer
    {
        public override async Task<Solution> RewriteCode(Solution solution, SyntaxTree syntaxTree, SemanticModel semanticModel)
        {
            var classDeclarations = (await syntaxTree.GetRootAsync()).DescendantNodes().OfType<ClassDeclarationSyntax>();
            var classSymbols
                = classDeclarations
                    .Select(classDeclaration => semanticModel.GetDeclaredSymbol(classDeclaration))
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