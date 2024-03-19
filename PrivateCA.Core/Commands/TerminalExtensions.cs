using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PrivateCA.Core.Commands; 

public static class TerminalExtensions {
    
    public static bool IsLinux() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    public static string Bash(this string cmd) {
        if (!IsLinux()) {
            throw new ApplicationException("Bro, why not Linux?");
        }
        
        var escapedArgs = cmd.Replace("\"", "\\\"");
        string result = Run("/bin/bash", $"-c \"{escapedArgs}\"");
        return result;
    }
    
    private static string Run (string filename, string arguments){
        var process = new Process() {
            StartInfo = new ProcessStartInfo {
                FileName = filename,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false,
            }
        };
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        if (!string.IsNullOrWhiteSpace(error)) {
            //Console.WriteLine("An error occured:\n" + error);
            //Console.WriteLine("Continuing with result " + result);
        }
        
        return result;
    }
}