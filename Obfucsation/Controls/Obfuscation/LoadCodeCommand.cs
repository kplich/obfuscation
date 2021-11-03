using System;
using System.Windows.Input;

namespace Obfucsation.Controls.Obfuscation
{
    internal class LoadCodeCommand : ICommand
    {
        private readonly ObfuscationViewModel _viewModel;
        
        public LoadCodeCommand(ObfuscationViewModel viewModel)
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
            return true;
        }

        public void Execute(object parameter)
        {
            _viewModel.LoadCodeFromFile();
        }

        #endregion
    }
}