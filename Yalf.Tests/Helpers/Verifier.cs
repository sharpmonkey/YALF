using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Utilities;

namespace Yalf.Tests.Helpers
{
    public static class Verifier
    {
        #region Public Methods and Operators

        public static string Verify(string assemblyPath2)
        {
            var workingDirectory = Path.GetDirectoryName(assemblyPath2);
            string exePath = GetPathToPEVerify();
            var process =
				Process.Start(new ProcessStartInfo(exePath, "\"" + assemblyPath2 + "\"")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                });

            process.WaitForExit(10000);

            return process.StandardOutput.ReadToEnd().Trim();
        }

        #endregion

        #region Methods

        private static string GetPathToPEVerify()
        {
            const string subPath = @"bin\NETFX 4.0 Tools\peverify.exe";

            foreach (var dotNetVersion in Enum.GetValues(typeof(TargetDotNetFrameworkVersion)).OfType < TargetDotNetFrameworkVersion>().Reverse())
            {
                var path = Path.Combine(ToolLocationHelper.GetPathToDotNetFrameworkSdk(dotNetVersion), subPath);
                if (File.Exists(path))
                    return path;
            }

            throw new FileNotFoundException("Couldn't find path to peverify in any of .Net SDK folders", subPath);
        }

        #endregion
    }
}
