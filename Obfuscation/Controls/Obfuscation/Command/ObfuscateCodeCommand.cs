using System;
using System.Windows.Input;
using Microsoft.CodeAnalysis.CSharp;
using Obfuscation.Core.Bloat;
using Obfuscation.Core.Bloat.ReplaceLiteralWithProperty.Plain;
using Obfuscation.Core.Rename;

namespace Obfuscation.Controls.Obfuscation.Command
{
    internal class ObfuscateCodeCommand : ICommand
    {
        private readonly ObfuscationViewModel _viewModel;
        
        public ObfuscateCodeCommand(ObfuscationViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        
        public bool CanExecute(object parameter)
        {
            return !string.IsNullOrWhiteSpace(_viewModel.Code.Original);
        }

        public async void Execute(object parameter)
        {
            var obfuscatedCode = _viewModel.Code.Original;
            var identifierGenerators = _viewModel.Options.ChosenIdentifierGenerationStrategies;
            
            if (_viewModel.Options.RenameClasses)
            {
                var renamer = new RandomClassRenamer(identifierGenerators);
                obfuscatedCode = await obfuscatedCode.RewriteCodeAsync(renamer);
            }

            if (_viewModel.Options.RenameMethods)
            {
                var renamer = new RandomMethodRenamer(identifierGenerators);
                obfuscatedCode = await obfuscatedCode.RewriteCodeAsync(renamer);
            }

            if (_viewModel.Options.RenameVariables)
            {
                var renamer = new RandomVariableRenamer(identifierGenerators);
                obfuscatedCode = await obfuscatedCode.RewriteCodeAsync(renamer);
            }

            // bloat the obfuscated code with extra classes
            /*var syntaxTree = CSharpSyntaxTree.ParseText(obfuscatedCode);
            var newRoot = new ClassBloater().Visit(await syntaxTree.GetRootAsync());
            obfuscatedCode = newRoot.ToFullString();*/

            var syntaxTree2 = CSharpSyntaxTree.ParseText(obfuscatedCode);
            var newRoot2 = new ReplaceLiteralWithPlainProperty(identifierGenerators).Visit(await syntaxTree2.GetRootAsync());
            obfuscatedCode = newRoot2.ToFullString();

            _viewModel.Code.Obfuscated = obfuscatedCode;
        }

        #endregion
    }
}