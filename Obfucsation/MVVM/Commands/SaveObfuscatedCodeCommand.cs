using System;
using System.Windows.Input;
using Obfucsation.MVVM.ViewModel;

namespace Obfucsation.MVVM.Commands
{
    public class SaveObfuscatedCodeCommand: ICommand
    {
        private readonly ObfuscationViewModel _viewModel;
        
        public SaveObfuscatedCodeCommand(ObfuscationViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        
        #region ICommand Members
        
        public bool CanExecute(object parameter)
        {
            return _viewModel.ObfuscatedCodeCanBeSaved;
        }

        public void Execute(object parameter)
        {
            _viewModel.SaveObfuscatedCodeToFile();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        
        #endregion
    }
}