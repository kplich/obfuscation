using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Obfuscation.Core.Name;

namespace Obfuscation.Core.Rename
{
    public static class CodeRewriterUtils
    {
        public static async Task<string> RewriteCodeAsync(this string code, CodeRenamer renamer)
        {
            return await renamer.ModifyCode(code).ConfigureAwait(false);
        }

        public static async Task<string> RewriteCodeAsync(this Task<string> codeAsync, CodeRenamer renamer)
        {
            return await renamer.ModifyCode(await codeAsync).ConfigureAwait(false);
        }
    }
    
    public abstract class CodeRenamer
    {
        private readonly IImmutableList<IIdentifierGenerator> _identifierGenerators;

        protected CodeRenamer(IImmutableList<IIdentifierGenerator> generators)
        {
            _identifierGenerators = generators;
        }

        protected abstract Task<Solution> RewriteCode(Solution solution, SyntaxTree syntaxTree, SemanticModel semanticModel);

        public async Task<string> ModifyCode(string code)
        {
            var workspace = new AdhocWorkspace();
            var references = new List<MetadataReference> { MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location) };
            var projectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Default, "MyProject",
                "MyAssembly", "C#", metadataReferences: references);
            var project = workspace.AddProject(projectInfo);
            var document = workspace.AddDocument(project.Id, "MyDocument.cs", SourceText.From(code));
            Compilation compilation = await document.Project.GetCompilationAsync();
            SyntaxTree syntaxTree = compilation.SyntaxTrees.First();
            SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
            Solution solution = workspace.CurrentSolution;

            if (IdentifierGeneratorsAreProvided)
            {
                solution = await RewriteCode(solution, syntaxTree, semanticModel).ConfigureAwait(false);
            }
            else
            {
                Console.WriteLine("No identifier generators have been provided!");
            }

            return (await solution.Projects.Single().Documents.Single().GetSyntaxRootAsync())?.ToFullString();
        }

        protected IIdentifierGenerator ChooseGenerator()
        {
            return _identifierGenerators[new Random().Next(_identifierGenerators.Count)];
        }

        private bool IdentifierGeneratorsAreProvided => _identifierGenerators.Count > 0;
    }
}