// <copyright file="DatabaseContext.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
    using NetworkPerformanceShared.Model;

    /// <summary>
    /// Context of the database.
    /// </summary>
    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="connectionstring">String with connection details.</param>
        public DatabaseContext(string connectionstring)
        {
            this.ConnectionString = connectionstring;
            this.Database.EnsureCreated();
        }

        /// <summary>Gets or sets latency test results.</summary>
        public DbSet<LatencyTestResult> LatencyTestResults { get; set; }

        /// <summary>Gets or sets latency test measurments.</summary>
        public DbSet<LatencyTestMeasurment> LatencyTestMeasurments { get; set; }

        /// <summary>Gets or sets latency test parameters.</summary>
        public DbSet<LatencyTestParameters> LatencyTestParameters { get; set; }

        /// <summary>Gets or sets throughput test results.</summary>
        public DbSet<ThroughputTestResult> ThroughputTestResults { get; set; }

        /// <summary>Gets or sets througput test parameters.</summary>
        public DbSet<ThroughputTestParameters> ThroughputTestParameters { get; set; }

        private string ConnectionString { get; set; }

        /// <summary>
        /// Insert a <see cref="LatencyTestResult"/>.
        /// </summary>
        /// <param name="latencyTestResult"><see cref="LatencyTestResult"/> to be inserted.</param>
        public void InsertLatencyTestResult(LatencyTestResult latencyTestResult)
        {
            this.Add(latencyTestResult);
            this.LatencyTestMeasurments.AddRange(latencyTestResult.TestMeasurments);
            this.SaveChanges();
        }

        /// <summary>
        /// Called when database context is configured.
        /// </summary>
        /// <param name="optionsBuilder">Options builder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var serverVersion = new MySqlServerVersion(new Version(10, 5, 0));
                optionsBuilder.UseMySql(this.ConnectionString, serverVersion);
            }
        }

        /// <summary>
        /// Called when database model is created.
        /// </summary>
        /// <param name="modelBuilder">Model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                                            v => v.ToUniversalTime(),
                                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? v.Value.ToUniversalTime() : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.IsKeyless)
                {
                    continue;
                }

                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                }
            }
        }
    }
}
