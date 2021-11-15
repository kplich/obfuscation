using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Obfuscation.Core;

namespace Obfuscation.Controls.Obfuscation
{
    public class ObfuscationViewModel : TabViewModel
    {
        public ObfuscatedCode Code { get; }

        public ObfuscationViewModel() : base("Obfuscation")
        {
            Code = new ObfuscatedCode();
            Obfuscate = new ObfuscateCodeCommand(this);
            LoadCode = new LoadCodeCommand(this);
            SaveObfuscatedCode = new SaveObfuscatedCodeCommand(this);
        }

        #region 'Load code' button
        public ICommand LoadCode { get; }
        public void LoadCodeFromFile()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Code.Original = File.ReadAllText(openFileDialog.FileName);
            }
        }

        #endregion

        #region 'Obfuscate code' button

        public ICommand Obfuscate { get; }
        public bool CodeCanBeObfuscated => !string.IsNullOrWhiteSpace(Code.Original);

        public async Task PerformCodeObfuscation()
        {
            Code.Obfuscated = await Code.Original
                .RewriteCodeAsync<RandomClassRenamer>()
                .RewriteCodeAsync<RandomMethodRenamer>()
                .RewriteCodeAsync<RandomVariableRenamer>();
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