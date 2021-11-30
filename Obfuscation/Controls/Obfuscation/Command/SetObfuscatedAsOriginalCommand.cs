using System;
using System.Windows.Input;

namespace Obfuscation.Controls.Obfuscation.Command
{
    internal class SetObfuscatedAsOriginalCommand : ICommand
    {
        private readonly ObfuscationViewModel _viewModel;
        
        public SetObfuscatedAsOriginalCommand(ObfuscationViewModel viewModel)
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
            return !string.IsNullOrWhiteSpace(_viewModel.Code.Obfuscated);
        }

        public void Execute(object parameter)
        {
            _viewModel.Code.Original = _viewModel.Code.Obfuscated;
            _viewModel.Code.Obfuscated = "";
        }

        #endregion
    }
}