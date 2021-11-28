using System;
using System.Diagnostics;
using System.IO;

namespace Obfuscation.Core
{
    public static class CodeRunner
    {
        public static async void RunCode(string code)
        {
            var projectDirectory = GetTemporaryDirectory();
            
            using var createNewProjectProcess = new Process();
            createNewProjectProcess.StartInfo.WorkingDirectory = projectDirectory;
            createNewProjectProcess.StartInfo.FileName = "dotnet";
            createNewProjectProcess.StartInfo.Arguments = "new console";
            createNewProjectProcess.StartInfo.CreateNoWindow = true;
            createNewProjectProcess.StartInfo.UseShellExecute = false;
            createNewProjectProcess.Start();
            await createNewProjectProcess.WaitForExitAsync();

            await File.WriteAllTextAsync(Path.Combine(projectDirectory, "Program.cs"), code);

            using var buildProjectProcess = new Process();
            buildProjectProcess.StartInfo.WorkingDirectory = projectDirectory;
            buildProjectProcess.StartInfo.FileName = "dotnet";
            buildProjectProcess.StartInfo.Arguments = "build";
            buildProjectProcess.StartInfo.CreateNoWindow = true;
            buildProjectProcess.StartInfo.UseShellExecute = false;
            buildProjectProcess.Start();
            await buildProjectProcess.WaitForExitAsync();
            
            using var runProjectProcess = new Process();
            runProjectProcess.StartInfo.WorkingDirectory = projectDirectory;
            runProjectProcess.StartInfo.FileName = "dotnet";
            runProjectProcess.StartInfo.Arguments = "run";
            runProjectProcess.StartInfo.CreateNoWindow = false;
            runProjectProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            runProjectProcess.Start();
        }
        
        private static string GetTemporaryDirectory()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
        
    }
}