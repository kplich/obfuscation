using System.Windows.Input;
using Obfuscation.Controls.Obfuscation.Command;
using Obfuscation.Controls.Obfuscation.Model;
using Obfuscation.Core;

namespace Obfuscation.Controls.Obfuscation
{
    public class ObfuscationViewModel : TabViewModel
    {
        public ObfuscatedCode Code { get; }
        public ObfuscationOptions Options { get; }
        
        public ICommand ObfuscateCode { get; }
        public ICommand SaveObfuscatedCode { get; }
        public ICommand LoadCode { get; }
        public ICommand RunCode { get; }

        public ObfuscationViewModel() : base("Obfuscation")
        {
            Code = new ObfuscatedCode();
            Options = new ObfuscationOptions();
            
            ObfuscateCode = new ObfuscateCodeCommand(this);
            LoadCode = new LoadCodeCommand(this);
            SaveObfuscatedCode = new SaveObfuscatedCodeCommand(this);
            RunCode = new RunCodeCommand(this);
        }
    }
}