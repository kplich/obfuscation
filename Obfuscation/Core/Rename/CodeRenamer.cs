﻿using System;
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
            var compilation = await document.Project.GetCompilationAsync();
            var syntaxTree = compilation.SyntaxTrees.First();
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var solution = workspace.CurrentSolution;

            if (IdentifierGeneratorsAreProvided)
            {
                foreach (var generator in _identifierGenerators)
                {
                    generator.ClearCache();
                }
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