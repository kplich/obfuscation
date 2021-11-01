using System.Windows;
using System.Windows.Input;
using Obfucsation.Core;

namespace Obfucsation.Controls.Obfuscation
{
    public class ObfuscationViewModel : TabViewModel
    {
        public ObfuscatedCode Code { get; }

        public ObfuscationViewModel() : base("Obfuscation")
        {
            Code = new ObfuscatedCode();
            Obfuscate = new ObfuscateCodeCommand(this);
            SaveObfuscatedCode = new SaveObfuscatedCodeCommand(this);
        }
        
        #region 'Obfuscate code' button

        public ICommand Obfuscate { get; }
        public bool CodeCanBeObfuscated => !string.IsNullOrWhiteSpace(Code.Original);

        public void PerformCodeObfuscation()
        {
            Code.Obfuscated = Code.Original + " ;)";
        }

        #endregion
        
        #region 'Save obfuscated code' button
        public ICommand SaveObfuscatedCode { get; }
        public bool ObfuscatedCodeCanBeSaved => !string.IsNullOrWhiteSpace(Code.Obfuscated);
        public void SaveObfuscatedCodeToFile()
        { 
            MessageBox.Show(Code.Obfuscated, "Saved obfuscated code!");
        }
        
        #endregion
    }
}