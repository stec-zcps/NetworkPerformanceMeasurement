// <copyright file="ShellHelper.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
// Copyright 2021 Fraunhofer Institute for Manufacturing Engineering and Automation IPA
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
namespace NetworkPerformanceShared
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Helper class to execute shell commands.
    /// </summary>
    public static class ShellHelper
    {
        /// <summary>
        /// Execute command via Bash.
        /// </summary>
        /// <param name="cmd">Command to be executed.</param>
        /// <param name="args">Arguments for command execution.</param>
        /// <returns>Output of the command execution.</returns>
        public static string Bash(this string cmd, string args = "")
        {
            Process process;
            var escapedArgs = args.Replace("\"", "\\\"");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{cmd} {escapedArgs}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    },
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var escapedCmd = cmd.Replace("\\\\", "\\");
                process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = escapedCmd,
                        Arguments = escapedArgs,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    },
                };
            }
            else
            {
                throw new PlatformNotSupportedException($"Unsupported platform '{RuntimeInformation.OSDescription}' to execute command via shell");
            }

            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return result;
        }
    }
}
