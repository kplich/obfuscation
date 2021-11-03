﻿using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Win32;
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

        public void PerformCodeObfuscation()
        {
            var tree = CSharpSyntaxTree.ParseText(Code.Original);
            var classRenamer = new RandomClassRenamer();

            var result1 = classRenamer.Visit(tree.GetRoot());

            Code.Obfuscated = result1.ToFullString();
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