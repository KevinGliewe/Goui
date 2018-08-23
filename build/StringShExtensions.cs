using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace GCore.Extensions.StringShEx {
    public static class StringShExtensions {
        public static string Sh(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var fileName = "/bin/bash";
            var arguments = $"-c \"{escapedArgs}\"";

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                fileName = "cmd.exe";
                arguments = $"/C \"{escapedArgs}\"";
            }
            
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }

        public static Version ExtractVersion(this string self) {
            var match = Regex.Match(self, @"^.*(\d+)\.(\d+)\.(\d+).*$");
            if(match.Captures.Count > 0)
                return new Version(
                    int.Parse(match.Groups[1].Value), 
                    int.Parse(match.Groups[2].Value), 
                    int.Parse(match.Groups[3].Value));
            throw new Exception($"String '{self}' does not contain a version number");
        }
    }
}
