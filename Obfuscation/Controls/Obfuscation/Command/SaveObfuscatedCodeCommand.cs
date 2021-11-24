﻿using System;
using System.Windows;
using System.Windows.Input;

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
            MessageBox.Show(_viewModel.Code.Obfuscated, "Saved obfuscated code!");
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        
        #endregion
    }
}