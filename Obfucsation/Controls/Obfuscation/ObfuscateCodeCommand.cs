using System;
using System.Windows.Input;

namespace Obfucsation.Controls.Obfuscation
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
            return _viewModel.CodeCanBeObfuscated;
        }

        public void Execute(object parameter)
        {
            _viewModel.PerformCodeObfuscation();
        }

        #endregion
    }
}