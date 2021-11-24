using System.ComponentModel;
using System.Runtime.CompilerServices;
using Obfuscation.Annotations;

namespace Obfuscation.Controls.Obfuscation.Model
{
    public sealed class ObfuscatedCode : INotifyPropertyChanged
    {
        private string _original;
        public string Original
        {
            get => _original;
            set
            {
                _original = value;
                OnPropertyChanged(nameof(Original));
            }
        }

        private string _obfuscated;
        public string Obfuscated
        {
            get => _obfuscated;
            set
            {
                _obfuscated = value;
                OnPropertyChanged(nameof(Obfuscated));
            }
        }

        public ObfuscatedCode(string original = "", string obfuscated = "")
        {
            _original = original;
            Obfuscated = obfuscated;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}