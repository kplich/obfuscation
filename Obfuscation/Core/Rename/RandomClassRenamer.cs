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
    public class RandomClassRenamer : CodeRenamer
    {
        public RandomClassRenamer(IImmutableList<IIdentifierGenerator> generators) : base(generators)
        {
        }

        protected override async Task<Solution> RewriteCode(Solution solution, SyntaxTree syntaxTree, SemanticModel semanticModel)
        {
            var classDeclarations = (await syntaxTree.GetRootAsync()).DescendantNodes().OfType<ClassDeclarationSyntax>();
            var classSymbols
                = classDeclarations
                    .Select(classDeclaration => semanticModel.GetDeclaredSymbol(classDeclaration))
                    .Where(symbol => symbol != null);
            
            foreach (var classSymbol in classSymbols)
            {
                solution = await Renamer.RenameSymbolAsync(solution, classSymbol,
                    ChooseGenerator().TransformClassName(classSymbol.Name), solution.Workspace.Options);
            }

            return solution;
        }

        
    }
}