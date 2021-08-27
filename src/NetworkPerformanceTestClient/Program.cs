// <copyright file="Program.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace NetworkPerformanceTestClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using CommandLine;
    using CommandLine.Text;
    using NetworkPerformanceShared.Database;
    using NetworkPerformanceTestClient.ExternalTools;
    using NetworkPerformanceTestClient.ExternalTools.Owping;
    using NetworkPerformanceTestClient.ExternalTools.Sockperf;
    using NetworkPerformanceTestClient.Utils;
    using Serilog;
    using Serilog.Events;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    /// <summary>
    /// Main program.
    /// </summary>
    public class Program
    {
        /// <summary>Gets a value indicating wheter the program is executed on Linux platform.</summary>
        public static readonly bool IsOSLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        private static ManualResetEvent quitEvent = new ManualResetEvent(false);

        /// <summary>Gets the database.</summary>
        public static Database Database { get; private set; }

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">Arguments for program execution.</param>
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose)
                .CreateLogger();
            Log.Information("Network Performance Test Client");

            if (args.Length == 0)
            {
                Log.Error("Argument for test tool missing, available tools:-\n Sockperf\n-IPA");
                return;
            }
            else if (!File.Exists("config.yaml"))
            {
                Log.Error("Configuration file 'config.yaml' not found");
                return;
            }

            var configFile = File.ReadAllText("config.yaml");
            var deserializer = new DeserializerBuilder()
                                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                                    .Build();
            var databaseConfiguration = deserializer.Deserialize<DatabaseConfiguration>(configFile);
            Program.Database = new Database(databaseConfiguration);

            // DatabaseInitializer.Seed(Program.Database);
            var tool = args[0];
            Log.Information($"Tool: Sockperf");

            var parser = new Parser(with =>
            {
                with.IgnoreUnknownArguments = true;
            });
            var parserResult = parser.ParseArguments<CommandLineOptions>(args)
                .WithParsed((cmdOptions) =>
                {
                    Log.Information($"Transmission Technology: {cmdOptions.TransmissionTechnology}");
                    Log.Information($"Network Interface: {cmdOptions.NetworkInterfaceName} (IP: {NetworkUtil.GetIpOfNetworkInterface(cmdOptions.NetworkInterfaceName)})");
                    Log.Information($"Network Interface Details: {NetworkUtil.GetInterfaceDetailsForDocumentation(cmdOptions.NetworkInterfaceName)}");

                    switch (tool.ToLower())
                    {
                        case "sockperf":
                            if (!Program.IsOSLinux)
                            {
                                Log.Error("SockPerf only available on Linux!");
                                return;
                            }

                            Sockperf.RunWithArgs(new List<string>(args).GetRange(1, args.Length - 1).ToArray());

                            break;

                        case "owping":
                            if (!Program.IsOSLinux)
                            {
                                Log.Error("SockPerf only available on Linux!");
                                return;
                            }

                            OWPing.RunWithArgs(new List<string>(args).GetRange(1, args.Length - 1).ToArray());

                            break;

                        case "rperf":
                            Rperf.RunWithArgs(new List<string>(args).GetRange(1, args.Length - 1).ToArray());
                            break;

                        default:
                            Log.Error($"Unsupported test tool '{tool}'");
                            return;
                    }
                })
                .WithNotParsed((errs) =>
                {
                    Log.Error("Unable to parse command line options");
                });

            if (parserResult is CommandLine.NotParsed<CommandLineOptions>)
            {
                var helpText = HelpText.AutoBuild(parserResult);
                Log.Error(helpText);
            }

            Program.Quit();
        }

        /// <summary>
        /// Quit the program.
        /// </summary>
        public static void Quit()
        {
            Program.quitEvent.Set();
            Environment.Exit(0);
        }
    }
}
