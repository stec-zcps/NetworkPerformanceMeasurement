// <copyright file="Database.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace NetworkPerformanceShared.Database
{
    using System;
    using System.IO;
    using System.Linq;
    using MySql.Data.MySqlClient;
    using NetworkPerformanceShared.Model;
    using Serilog;

    /// <summary>
    /// Database to store test results.
    /// </summary>
    public class Database
    {
        private MySqlConnection databaseConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="databaseConfiguration">Configuration to connect to the database.</param>
        public Database(DatabaseConfiguration databaseConfiguration)
        {
            this.DatabaseConfiguration = databaseConfiguration;

            this.databaseConnection = new MySqlConnection(this.ConnectionString);
            this.DatabaseContext = new DatabaseContext(this.ConnectionString);
        }

        /// <summary>Gets the <see cref="DatabaseContext"/>.</summary>
        public DatabaseContext DatabaseContext { get; private set; }

        private DatabaseConfiguration DatabaseConfiguration { get; }

        private string ConnectionString
        {
            get
            {
                return $"server={this.DatabaseConfiguration.DatabaseHost};" +
                    $"port={this.DatabaseConfiguration.DatabasePort};" +
                    $"user={this.DatabaseConfiguration.DatabaseUser};" +
                    $"password={this.DatabaseConfiguration.DatabasePassword};" +
                    $"database={this.DatabaseConfiguration.DatabaseName};" +
                    $"AllowLoadLocalInfile=true";
            }
        }

        /// <summary>
        /// Insert a <see cref="LatencyTestResult"/> into database.
        /// </summary>
        /// <param name="latencyTestResult">The <see cref="LatencyTestResult"/> to be inserted.</param>
        public void Insert(LatencyTestResult latencyTestResult)
        {
            this.DatabaseContext.Add(latencyTestResult);
            this.DatabaseContext.SaveChanges();

            // Converting Dictionary to .csv
            string csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"mysql_latency-test-measurments.csv");

            // Columns: Id, LatencyTestResultId, Index, SendTime, ReceiveTime, Latency, CapturedLatency, Jitter, CapturedJitter, CapturedOneWayLatencyClientToServer, CapturedOneWayLatencyServerToClient, CapturedProcessingTimeServer, CapturedTapPingClientSide, CapturedTapPingServerSide, CapturedTapPongClientSide, CapturedTapPongServerSide
            string csv = string.Join(Environment.NewLine, latencyTestResult.TestMeasurments.Select(
                m => $"0;{latencyTestResult.Id};{m.Index};{m.SendTime};{m.ReceiveTime};{m.Latency};{m.LatencyClientToServer};{m.LatencyServerToClient};{m.CapturedLatency};{m.Jitter};{m.CapturedJitter};" +
                       $"{m.CapturedOneWayLatencyClientToServer};{m.CapturedOneWayLatencyServerToClient};{m.CapturedProcessingTimeServer};" +
                       $"{(m.CapturedTapPingClientSide ? 1 : 0)};{(m.CapturedTapPingServerSide ? 1 : 0)};{(m.CapturedTapPongClientSide ? 1 : 0)};{(m.CapturedTapPongServerSide ? 1 : 0)};" +
                       $"{(m.IsOutlier ? 1 : 0)};{(m.IsPingOutlier ? 1 : 0)};{(m.IsPongOutlier ? 1 : 0)};{(m.IsProcessingServerOutlier ? 1 : 0)};{(m.IsProcessingClientOutlier ? 1 : 0)}"));
            File.WriteAllText(csvPath, csv);

            // 0;394;1;3.110960406;3113.175359;1.1074771;0.922555;0.11142725;-0.019169152;0.6874;0.54361;0.6141;True;True;True;True
            this.databaseConnection.Open();

            MySqlBulkLoader bulkLoader = new MySqlBulkLoader(this.databaseConnection);
            bulkLoader.TableName = "LatencyTestMeasurments";
            bulkLoader.FieldTerminator = ";";

            // bulkLoader.LineTerminator = "\r\n";
            bulkLoader.FileName = csvPath;
            bulkLoader.Local = true;
            bulkLoader.NumberOfLinesToSkip = 0;
            var insertedRowsCound = bulkLoader.Load();

            if (insertedRowsCound > 0)
            {
                Log.Information($"Successfuly stored Latency Test Result with id {latencyTestResult.Id} ({insertedRowsCound} measurment points)");
            }
            else
            {
                Log.Information($"Failed to store Latency Test Result with id {latencyTestResult.Id}. No measurment points have been saved.");
            }

            this.databaseConnection.Close();

            File.Delete(csvPath);
        }
    }
}
