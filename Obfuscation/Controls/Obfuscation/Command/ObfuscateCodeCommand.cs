using System;
using System.Windows.Input;
using Obfuscation.Core;

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
            
            if (_viewModel.Options.RenameClasses)
            {
                obfuscatedCode = await obfuscatedCode.RewriteCodeAsync<RandomClassRenamer>();
            }

            if (_viewModel.Options.RenameMethods)
            {
                obfuscatedCode = await obfuscatedCode.RewriteCodeAsync<RandomMethodRenamer>();
            }

            if (_viewModel.Options.RenameVariables)
            {
                obfuscatedCode = await obfuscatedCode.RewriteCodeAsync<RandomVariableRenamer>();
            }

            _viewModel.Code.Obfuscated = obfuscatedCode;
        }

        #endregion
    }
}