using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ba_createData.Scanner
{
    public class ScannerCaching
    {
        public static HashSet<string> CleanCache { get; set; }

        public ScannerCaching()
        {
            // Loads the cache from file
            // This is done once, when the server starts up
            // At 1pm the server updates, and save the cache to file.
            LoadCashedValues();
        }


        private void LoadCashedValues()
        {
            if (!File.Exists(Thread.GetDomain().BaseDirectory + ScannerConst.Md5Cashing)) return;
            CleanCache = new HashSet<string>();
            var hashSet = GetCacheFromFile(Thread.GetDomain().BaseDirectory + ScannerConst.Md5Cashing);
            if (hashSet != null)
            {
                CleanCache = hashSet;
            }
        }


        /// <summary>
        /// Return the cache status
        /// </summary>
        /// <returns></returns>
        public static bool IsCacheSet()
        {
            return CleanCache != null;
        }


        /// <summary>
        /// Return the loaded  
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private HashSet<string> GetCacheFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var text = File.ReadAllText(filePath);
                string[] words = text.Split(';');
                var done = words.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                return new HashSet<string>(done);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Update the cache with some hash, if cache does not contain key already
        /// </summary>
        /// <param name="hash"></param>
        public static void UpdateCache(string hash)
        {
            if (!CleanCache.Contains(hash))
            {
                CleanCache.Add(hash);
            }
        }


        /// <summary>
        /// Check to see, if key is already present in cache
        /// </summary>

        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool IsFileInCache(string hash)
        {
            return CleanCache.Contains(hash);
        }


        /// <summary>
        /// Delete a specific file from cache
        /// This is nessesary due to the fact that ther database is updated
        /// regulary.
        /// </summary>
        /// <param name="hash"></param>
        public static void DeleteHashFromCache(string hash)
        {
            CleanCache.Remove(hash);
        }



        /// <summary>
        /// Save cache to file
        /// </summary>
        public static void SaveCache()
        {
            Directory.CreateDirectory(Thread.GetDomain().BaseDirectory + ScannerConst.CashingFolder);
            var output = string.Join(";", CleanCache);
            if (File.Exists(Thread.GetDomain().BaseDirectory + ScannerConst.Md5Cashing) && !output.Equals(string.Empty))
            {
                while (!IsFileReady(Thread.GetDomain().BaseDirectory + ScannerConst.Md5Cashing))
                {
                    TextWriter tsw = new StreamWriter(Thread.GetDomain().BaseDirectory + ScannerConst.Md5Cashing, true);
                    tsw.Write(output);
                }
            }
            else
            {
                if (output.Equals(string.Empty)) return;
                var file = File.Create(Thread.GetDomain().BaseDirectory + ScannerConst.Md5Cashing);
                file.Close();

                TextWriter tsw = new StreamWriter(Thread.GetDomain().BaseDirectory + ScannerConst.Md5Cashing, true);
                tsw.Write(output);
                tsw.Close();
            }
        }


        /// <summary>
        /// Simple check, to see if file is ready
        /// </summary>
        /// <param name="sFilename"></param>
        /// <returns></returns>
        public static bool IsFileReady(string sFilename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (var inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return inputStream.Length > 0;

                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
