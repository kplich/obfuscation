using System;
using System.Windows.Input;
using Obfuscation.Core;

namespace Obfuscation.Controls.Obfuscation.Command
{
    internal class RunCodeCommand : ICommand
    {
        private readonly ObfuscationViewModel _viewModel;

        public RunCodeCommand(ObfuscationViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        
        
        #region ICommand Members
        
        public bool CanExecute(object parameter)
        {
            return !string.IsNullOrWhiteSpace(_viewModel.Code.Original);
        }

        public void Execute(object parameter)
        {
            CodeRunner.RunCode(_viewModel.Code.Original);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        
        #endregion
    }
}