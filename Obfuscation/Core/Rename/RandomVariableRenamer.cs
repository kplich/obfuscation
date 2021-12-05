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
    public class RandomVariableRenamer : CodeRenamer
    {
        public RandomVariableRenamer(IImmutableList<IIdentifierGenerator> generators) : base(generators)
        {
        }

        protected override async Task<Solution> RewriteCode(Solution solution, SyntaxTree syntaxTree, SemanticModel semanticModel)
        {
            var variableDeclarations = (await syntaxTree.GetRootAsync()).DescendantNodes().OfType<VariableDeclaratorSyntax>();
            var variableSymbols
                = variableDeclarations
                    .Select(variableSymbol => semanticModel.GetDeclaredSymbol(variableSymbol))
                    .Where(symbol => symbol != null);
            
            foreach (var variableSymbol in variableSymbols)
            {
                solution = await Renamer.RenameSymbolAsync(solution, variableSymbol,
                    ChooseGenerator().GenerateVariableName(variableSymbol.Name), solution.Workspace.Options);
            }

            return solution;
        }
    }
}