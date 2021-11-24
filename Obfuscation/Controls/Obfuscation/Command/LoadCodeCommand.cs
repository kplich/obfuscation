using System;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;

namespace Obfuscation.Controls.Obfuscation.Command
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
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _viewModel.Code.Original = File.ReadAllText(openFileDialog.FileName);
            }
        }

        #endregion
    }
}