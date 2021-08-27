// <copyright file="DatabaseConfiguration.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    /// <summary>
    /// Configuration to connect to database.
    /// </summary>
    public class DatabaseConfiguration
    {
        /// <summary>Gets or sets the name of the database.</summary>
        public string DatabaseName { get; set; } = "NetworkPerformanceTest";

        /// <summary>Gets or sets the database host.</summary>
        public string DatabaseHost { get; set; } = "localhost";

        /// <summary>Gets or sets database port.</summary>
        public int DatabasePort { get; set; } = 3306;

        /// <summary>Gets or sets database user.</summary>
        public string DatabaseUser { get; set; } = "root";

        /// <summary>Gets or sets database password.</summary>
        public string DatabasePassword { get; set; } = "password";
    }
}
