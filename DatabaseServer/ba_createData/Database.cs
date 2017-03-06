/*
 Copyright (c) 2016 Mark Roland, University of Copenhagen, Department of Computer Science
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
 
 IMPORTANT NOTICE:

 ANY CODE FLAGED TO BE OWNED BY AUTHORS OR COPYRIGHT HOLDERS ARE NOT FREE OF
 CHARGE, AND SHOULD BE USED WITH ANY RESTRICTIONS ASSOCIATED THE FILES/CODE.
*/

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ba_createData
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>
    /// The malware database.
    /// </summary>
    [Serializable]
    public static class Database
    {
        private const string Pattern = "^[0-9a-fA-F]{32}$";

        private static SqlConnection SetupSqlConnection()
        {
            // Base directory connection
            SqlConnection databaseConnection = new SqlConnection(
                @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\GitHub bachelor\BachelorNew\CreateData\ba_createData\Database.mdf;Integrated Security=True");

            return databaseConnection;
        }


        private static SqlConnection SetupSqlSuffixArrayConnection()
        {
            // Base directory connection
            SqlConnection databaseConnection = new SqlConnection(
                @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\GitHub bachelor\BachelorNew\CreateData\ba_createData\SuffixArray.mdf;Integrated Security=True");

            return databaseConnection;
        }


        private static SqlConnection SetupTestDatabaseConnection()
        {
            // Base directory connection
            SqlConnection databaseConnection = new SqlConnection(
                @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\GitHub bachelor\BachelorNew\CreateData\ba_createData\TestDatabase.mdf;Integrated Security=True");

            return databaseConnection;
        }

        /// <summary>
        /// The create database, such that each row in the .csv files are concat with the termination character '$'
        /// Data is saved as a file/ files
        /// </summary>
        public static void CreateDatabase()
        {
            var splitOption = Properties.Settings.Default.SplitOption;
            var databaseDirectory = Thread.GetDomain().BaseDirectory;
            var path = Thread.GetDomain().BaseDirectory + "Hashes//";
            var files = Directory.GetFiles(path);
            if (files.Length == 0) return;
            foreach (var file in files)
            {
                using (var sr = new StreamReader(file))
                {
                    // currentLine will be null when the StreamReader reaches the end of file
                    string currentLine;
                    while ((currentLine = sr.ReadLine()) != null)
                    {

                        // Checking current line to insure that it is a MD5 hash, with appropriate length
                        if (Regex.Match(currentLine, Pattern, RegexOptions.None).Success)
                        {
                            var filePath = databaseDirectory + "\\Data\\" + currentLine.Substring(0, splitOption) +
                                           ".data";
                            using (
                                var fileStream = new FileStream(
                                    filePath,
                                    FileMode.Append,
                                    FileAccess.Write,
                                    FileShare.Write))
                            using (var bw = new BinaryWriter(fileStream))
                            {
                                bw.Write(currentLine + "$");
                            }
                        }
                    }

                    sr.Close();
                    sr.Dispose();
                }
            }
        }



        /// <summary>
        /// Build the SQL database with all distinct MD5 string contained in the filedatabase 
        /// </summary>
        public static void BuildSqlDatabase()
        {
            var databaseConnection = SetupSqlConnection();
            var splitOption = Properties.Settings.Default.SplitOption;
            var path = Thread.GetDomain().BaseDirectory + "Hashes//";
            var files = Directory.GetFiles(path);
            if (files.Length == 0) return;
            databaseConnection.Open();
            var filePath = Thread.GetDomain().BaseDirectory + "\\Data\\" + "whatTheF.txt";
            foreach (var file in files)
            {
                using (var sr = new StreamReader(file))
                {
                    string currentLine;
                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        if (!Regex.Match(currentLine, Pattern, RegexOptions.None).Success) continue;
                        try
                        {

                            // Insert into database into the propper table in dbo
                            var database = new SqlCommand
                            {
                                Connection = databaseConnection,
                                CommandText =
                                    $"INSERT INTO {"MALWARE_" + currentLine.Substring(0, 1)}(md5Value) VALUES (@MD5ID)"
                            };
                            database.Parameters.AddWithValue("@MD5ID", currentLine + "$");
                            database.ExecuteNonQuery();

                        }
                        catch (Exception ex)
                        {
                            using (
                                var fileStream = new FileStream(
                                    filePath,
                                    FileMode.Append,
                                    FileAccess.Write,
                                    FileShare.Write))
                            using (var bw = new BinaryWriter(fileStream))
                            {
                                bw.Write(ex.Message + Environment.NewLine);
                            }
                        }
                    }
                }
            }
            databaseConnection.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<string> GetTables(SqlConnection connectionString)
        {
            connectionString.Open();
            var schema = connectionString.GetSchema("Tables");
            connectionString.Close();
            return (from DataRow row in schema.Rows select row[2].ToString()).ToList();

        }


        /// <summary>
        /// Returns the tables containing lcp values
        /// - All suffix array databases are named LCPARRAY
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLcpArrayDataBaseNames()
        {
            var databaseConnection = SetupSqlConnection();
            databaseConnection.Open();
            var malwareTableNames = GetTables(databaseConnection).Where(x => x.Contains("LCPARRAY_"));
            databaseConnection.Close();
            return malwareTableNames.ToList();
        }


        /// <summary>
        /// Build the SQL database for LCP values
        /// </summary>
        public static void BuildSqlLcpDatabase(int[] lcpArray, string databaseName, bool test)
        {
            var extension = databaseName.Split('_');
            var extensionName = extension[1];
            var databaseConnection = test ? SetupTestDatabaseConnection() : SetupSqlSuffixArrayConnection();
            var filePath = Thread.GetDomain().BaseDirectory + "\\Error\\" + "lcpArrayError.txt";
            var i = 0;
            foreach (var entry in lcpArray)
            {
                try
                {
                    databaseConnection.Open();
                    var database = new SqlCommand
                    {
                        Connection = databaseConnection,
                        CommandText = $"INSERT INTO {"LCPARRAY_" + extensionName}(id, lcpvalue) VALUES (@id, @lcpvalue)"
                    };
                    database.Parameters.AddWithValue("@id", i);
                    database.Parameters.AddWithValue("@lcpvalue", entry);
                    database.ExecuteNonQuery();
                    i++;
                    databaseConnection.Close();
                }
                catch (Exception ex)
                {
                    databaseConnection.Close();
                    using (
                        var fileStream = new FileStream(
                            filePath,
                            FileMode.Append,
                            FileAccess.Write,
                            FileShare.Write))
                    using (var bw = new BinaryWriter(fileStream))
                    {
                        bw.Write(ex.Message + Environment.NewLine);
                    }
                }
            }
        }



        /// <summary>
        /// Returns the tables containing suffix array md5 strings
        /// - All suffix array databases are named SUFFIXARRAY
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSuffixArrayDataBaseNames()
        {
            var malwareTableNames = GetTables(SetupSqlConnection()).Where(x => x.Contains("SUFFIXARRAY_"));
            return malwareTableNames.ToList();
        }

        /// <summary>
        /// Build the SQL database for LCP values
        /// </summary>
        public static void BuildSqlSuffixArrayDatabase(int[] suffixArray, string databaseName, bool test)
        {
            var extension = databaseName.Split('_');
            var extensionName = extension[1];
            var databaseConnection = test ? SetupTestDatabaseConnection() : SetupSqlSuffixArrayConnection();
            var filePath = Thread.GetDomain().BaseDirectory + "\\Error\\" + "suffixArrayError.txt";
            var i = 0;
            foreach (var entry in suffixArray)
            {
                try
                {
                    databaseConnection.Open();

                    var database = new SqlCommand
                    {
                        Connection = databaseConnection,
                        CommandText = $"INSERT INTO {"SUFFIXARRAY_" + extensionName}(id, suffixvalue) VALUES (@id, @suffixvalue)"
                    };
                    database.Parameters.AddWithValue("@id", i);
                    database.Parameters.AddWithValue("@suffixvalue", entry);
                    database.ExecuteNonQuery();
                    i++;
                    databaseConnection.Close();
                }
                catch (Exception ex)
                {
                    databaseConnection.Close();
                    using (
                        var fileStream = new FileStream(
                            filePath,
                            FileMode.Append,
                            FileAccess.Write,
                            FileShare.Write))
                    using (var bw = new BinaryWriter(fileStream))
                    {
                        bw.Write(ex.Message + "  ____EXTENSION ____ " + extensionName + Environment.NewLine);
                    }
                }
            }
        }


        /// <summary>
        /// Insert a single md5 into SQL Database
        /// </summary>
        /// <param name="md5">
        ///     md5 string must have a termination symbol 
        /// </param>
        /// <returns></returns>
        public static bool InsertSingleMd5(string md5)
        {
            var databaseConnection = SetupSqlConnection();
            if (!Regex.Match(md5, Pattern, RegexOptions.None).Success) return false;
            // Removes duplicates
            var dataExists = new SqlCommand
            {
                Connection = databaseConnection,
                CommandText = "SELECT COUNT(*) FROM MD5 WHERE MD5ID LIKE (@MD5ID)"
            };
            dataExists.Parameters.AddWithValue("@MD5ID", md5);
            var count = (int)dataExists.ExecuteScalar();
            if (!count.Equals(0)) return false;
            var database = new SqlCommand
            {
                Connection = databaseConnection,
                CommandText = "INSERT INTO MD5(MD5ID) VALUES (@MD5ID)"
            };
            database.Parameters.AddWithValue("@MD5ID", md5);
            databaseConnection.Open();
            database.ExecuteNonQuery();
            databaseConnection.Close();

            return true;
        }


        /// <summary>
        /// Insert a single md5 into SQL Database
        /// </summary>
        /// <param name="md5">
        ///     md5 string must have a termination symbol
        /// </param>
        /// <returns></returns>
        public static bool DeleteSingleMd5(string md5)
        {
            var databaseConnection = SetupSqlConnection();
            if (!Regex.Match(md5, Pattern, RegexOptions.None).Success) return false;
            // Removes duplicates
            var dataExists = new SqlCommand
            {
                Connection = databaseConnection,
                CommandText = "SELECT COUNT(*) FROM MD5 WHERE MD5ID LIKE (@MD5ID)"
            };
            dataExists.Parameters.AddWithValue("@MD5ID", md5);
            var count = (int)dataExists.ExecuteScalar();
            if (!count.Equals(0)) return false;
            var database = new SqlCommand
            {
                Connection = databaseConnection,
                CommandText = "DELETE FROM MD5 WHERE MD5ID = (@MD5ID)"
            };
            database.Parameters.AddWithValue("@MD5ID", md5);
            databaseConnection.Open();
            database.ExecuteNonQuery();
            databaseConnection.Close();

            return true;
        }


        /// <summary>
        /// Return true if and only if the md5 string is contained in the SQL database
        /// </summary>
        public static bool IsMd5InSqlLDatabase(string md5)
        {
            var databaseConnection = SetupSqlConnection();
            var dataExists = new SqlCommand
            {
                Connection = databaseConnection,
                CommandText = "SELECT COUNT(*) FROM MD5 WHERE MD5ID LIKE (@MD5ID)"
            };
            dataExists.Parameters.AddWithValue("@MD5ID", md5 + "$");
            databaseConnection.Open();
            var count = (int)dataExists.ExecuteScalar();
            databaseConnection.Close();
            return count.Equals(0);
        }



        /// <summary>
        /// Returns the tables containing malare md5 strings
        /// - All malware databaes are named MALWARE
        /// </summary>
        /// <returns></returns>
        public static List<string> GetMalwareDataBaseNames()
        {
            var malwareTableNames = GetTables(SetupSqlConnection()).Where(x => x.Contains("MALWARE_"));
            return malwareTableNames.ToList();
        }

        /// <summary>
        /// Return Malware Database as int[]
        /// </summary>
        public static HashSet<string> GetMd5MalwareFromDatabase(string tableName)
        {
            var databaseConnection = SetupSqlConnection();
            databaseConnection.Open();
            var databaseHolder = new HashSet<string>();
            var filePath = Thread.GetDomain().BaseDirectory + "\\Error\\" + "getDatabaseError.txt";
            try
            {
                var database = new SqlCommand
                {
                    Connection = databaseConnection,
                    CommandText = $"SELECT DISTINCT md5Value FROM {tableName}"
                };
                var reader = database.ExecuteReader();
                while (reader.Read())
                {    //Every new row will create a new dictionary that holds the columns
                    databaseHolder.Add(reader["md5Value"].ToString());
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                using (
                    var fileStream = new FileStream(
                        filePath,
                        FileMode.Append,
                        FileAccess.Write,
                        FileShare.Write))
                using (var bw = new BinaryWriter(fileStream))
                {
                    bw.Write(ex.Message + Environment.NewLine);
                }
            }
            databaseConnection.Close();
            return databaseHolder;
        }


        /// <summary>
        /// Return the lcp values stored in the database
        /// </summary>
        public static HashSet<int> GetLcpValues()
        {
            var lcpArray = new HashSet<int>();
            var databaseConnection = SetupSqlConnection();
            var lcpArrayCommand = new SqlCommand
            {
                Connection = databaseConnection,
                CommandText = "SELECT lcpvalue FROM LCPARRAY"
            };
            databaseConnection.Open();
            var data = lcpArrayCommand.ExecuteReader();
            foreach (int row in data)
            {
                lcpArray.Add(row);
            }
            databaseConnection.Close();
            return lcpArray;
        }


        /// <summary>
        /// Return the suffix array values stored in the database
        /// </summary>
        public static HashSet<int> GetSuffixArrayValues()
        {
            var suffixArray = new HashSet<int>();
            var databaseConnection = SetupSqlConnection();
            var suffixArrayCommand = new SqlCommand
            {
                Connection = databaseConnection,
                CommandText = "SELECT suffixvalues FROM SUFFIXARRAY"
            };
            databaseConnection.Open();
            var data = suffixArrayCommand.ExecuteReader();
            foreach (int row in data)
            {
                suffixArray.Add(row);
            }
            databaseConnection.Close();
            return suffixArray;
        }
    }
}
