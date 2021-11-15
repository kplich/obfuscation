using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Obfuscation.Controls.Obfuscation
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

        public async void Execute(object parameter)
        {
            await _viewModel.PerformCodeObfuscation();
        }

        #endregion
    }
}