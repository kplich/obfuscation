using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace Obfuscation.Controls.Obfuscation.Command
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
            return !string.IsNullOrWhiteSpace(_viewModel.Code.Obfuscated);
        }

        public void Execute(object parameter)
        {
            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, _viewModel.Code.Obfuscated);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        
        #endregion
    }
}