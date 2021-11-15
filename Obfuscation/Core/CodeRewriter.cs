using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Obfuscation.Core
{
    public static class CodeRewriterUtils
    {
        public static async Task<string> RewriteCodeAsync<T>(this string code) where T : CodeRewriter, new()
        {
            return await new T().ModifyCode(code).ConfigureAwait(false);
        }

        public static async Task<string> RewriteCodeAsync<T>(this Task<string> codeAsync) where T : CodeRewriter, new()
        {
            return await new T().ModifyCode(await codeAsync).ConfigureAwait(false);
        }

        public static async Task<string> RewriteCodeAsync(this string code, CodeRewriter rewriter)
        {
            return await rewriter.ModifyCode(code).ConfigureAwait(false);
        }

        public static async Task<string> RewriteCodeAsync(this Task<string> codeAsync, CodeRewriter rewriter)
        {
            return await rewriter.ModifyCode(await codeAsync).ConfigureAwait(false);
        }
    }
    
    public abstract class CodeRewriter
    {
        public abstract Task<Solution> RewriteCode(Solution solution, SyntaxTree syntaxTree, SemanticModel semanticModel);

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

            solution = await RewriteCode(solution, syntaxTree, semanticModel).ConfigureAwait(false);

            return (await solution.Projects.Single().Documents.Single().GetSyntaxRootAsync())?.ToFullString();
        }
    }
}