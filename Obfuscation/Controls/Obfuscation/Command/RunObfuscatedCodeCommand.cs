using System;
using System.Windows.Input;
using Obfuscation.Core;

namespace Obfuscation.Controls.Obfuscation.Command
{
    internal class RunObfuscatedCodeCommand : ICommand
    {
        private readonly ObfuscationViewModel _viewModel;

        public RunObfuscatedCodeCommand(ObfuscationViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        
        
        #region ICommand Members
        
        public bool CanExecute(object parameter)
        {
            return !string.IsNullOrWhiteSpace(_viewModel.Code.Obfuscated);
        }

        public void Execute(object parameter)
        {
            CodeRunner.RunCode(_viewModel.Code.Obfuscated);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        
        #endregion
    }
}