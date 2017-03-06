using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace ba_createData.Scanner
{
    public class Scanner
    {


        private CancellationTokenSource _mCancelTokenSource;


        /// <summary>
        /// The scan file.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ScanFile(string hash)
        {

            // If caching is activated, look in cache before scanning
            if (Properties.Settings.Default.UseCashing)
            {
                // If hash is in the clean MD5 cache, then return false
                if (ScannerCaching.IsFileInCache(hash))
                {
                    return false;
                }
            }


            if (Properties.Settings.Default.UseRAMOnly)
            {
                // All files are cached in memory, and can be accessed
                // without loading any files from HHD
                if (ScannerCaching.IsFileInCache(hash))
                {
                    return false;
                }
            }

            // Else check the file against the database
            var databaseDirectory = Thread.GetDomain().BaseDirectory + "SuffixArrays\\";
            var suffixArray = this.DeserializeArray(databaseDirectory + hash.Substring(0, Properties.Settings.Default.SplitOption) + ".data");
            var completeString =
                this.DeserializeString(databaseDirectory + ScannerConst.StringIdentificationName + hash.Substring(0, Properties.Settings.Default.SplitOption) + ".data");
            if (suffixArray == null && completeString == string.Empty) return false;
            var result = !this.IndexOf(hash, suffixArray, completeString).Equals(-1);

            // If the result is false, then the file is clean
            // and can be added to the cache
            if (!result)
            {
                ScannerCaching.UpdateCache(hash);
            }
            // Report back with findings
            return result;
        }

        /// <summary>
        /// The scan folder.
        /// </summary>
        /// <param name="directory">
        /// The directory.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<string> ScanFolder(string directory)
        {
            _mCancelTokenSource = new CancellationTokenSource();
            // Get a reference to the cancellation token.
            CancellationToken readFileCancelToken = _mCancelTokenSource.Token;

            var fileList = new List<string>();
            var files = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories).ToList();
            foreach (var file in files)
            {
                Task.Factory.StartNew(() =>
                {
                    // If cancel has been chosen, throw an exception now before doing anything.
                    readFileCancelToken.ThrowIfCancellationRequested();
                    try
                    {
                        var md5 = this.GetMd5HashOfAFile(file);
                        if (!this.ScanFile(md5 + "$"))
                        {
                            var filenameWithoutExrtension = Path.GetFileNameWithoutExtension(file);

                            // Kill all programs that lock the current file
                            var lockedProcesses = ScannerFileUtil.WhoIsLocking(file).ToList();
                            foreach (var lockedProcess in lockedProcesses)
                            {
                                lockedProcess.Kill();
                            }

                            // Get all instances of Notepad running on the local computer.
                            Process[] localByName = Process.GetProcessesByName(filenameWithoutExrtension);
                            foreach (Process process in localByName)
                            {
                                process.Kill();
                            }
                            var databaseDirectory = Thread.GetDomain().BaseDirectory + "Quarantine\\";
                            if (File.Exists(databaseDirectory + filenameWithoutExrtension + ".qua"))
                            {
                                File.Delete(databaseDirectory + filenameWithoutExrtension + ".qua");
                            }
                            File.Move(file, databaseDirectory + filenameWithoutExrtension + ".qua");
                            fileList.Add(file);
                        }
                    }
                    catch (Exception)
                    {
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                        }
                    }
                    finally
                    {
                        GC.Collect();
                    }
                }, readFileCancelToken);
            }

            return fileList;
        }

        /// <summary>
        /// The get md 5.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetMd5(string file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    var md5Byte = md5.ComputeHash(stream);
                    return BitConverter.ToString(md5Byte).Replace("-", string.Empty).ToLower();
                }
            }
        }

        public string GetMd5HashOfAFile(string file)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 8192);
            md5.ComputeHash(stream);
            stream.Close();

            byte[] hash = md5.Hash;
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Binary Search
        /// </summary>
        /// <param name="substr">
        /// The substr.
        /// </param>
        /// <param name="mSa">
        /// The m sa.
        /// </param>
        /// <param name="mStr">
        /// The m str.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int IndexOf(string substr, int[] mSa, string mStr)
        {
            var l = 0;
            var r = mSa.Length;

            if ((substr == null) || (substr.Length == 0))
            {
                return -1;
            }

            // Binary search for substring
            while (r > l)
            {
                var m = (l + r) / 2;
                // ReSharper disable once StringCompareToIsCultureSpecific
                if (m < mSa.Length && mSa[m] < mStr.Length && mStr.Substring(mSa[m]).CompareTo(substr) < 0)
                {
                    l = m + 1;
                }
                else
                {
                    r = m;
                }
            }

            if ((l == r) && (l < mStr.Length) && l < mSa.Length && mSa[l] < mStr.Length && mStr.Substring(mSa[l]).StartsWith(substr))
            {
                return mSa[l];
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// The deserialize array.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>int[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        public int[] DeserializeArray(string filePath)
        {
            if (File.Exists(filePath))
            {
                var text = File.ReadAllText(filePath);
                string[] words = text.Split(';');
                var done = words.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                return Array.ConvertAll(done, int.Parse);
            }
            else
            {
                return null;
            }
        }

        public HashSet<string> DeserializeHashSet(string filePath)
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
        /// The deserialize string.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DeserializeString(string filePath)
        {
            if (File.Exists(filePath))
            {
                var bytes = File.ReadAllBytes(filePath);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string UnZip(string value)
        {
            //Transform string into byte[]
            var byteArray = new byte[value.Length];
            var indexBa = 0;
            foreach (var item in value)
            {
                byteArray[indexBa++] = (byte)item;
            }

            //Prepare for decompress
            var ms = new MemoryStream(byteArray);
            var sr = new System.IO.Compression.GZipStream(ms,
                System.IO.Compression.CompressionMode.Decompress);

            //Reset variable to collect uncompressed result
            byteArray = new byte[byteArray.Length];

            //Decompress
            var rByte = sr.Read(byteArray, 0, byteArray.Length);

            //Transform byte[] unzip data to string
            System.Text.StringBuilder sB = new System.Text.StringBuilder(rByte);
            //Read the number of bytes GZipStream red and do not a for each bytes in
            //resultByteArray;
            for (var i = 0; i < rByte; i++)
            {
                sB.Append((char)byteArray[i]);
            }
            sr.Close();
            ms.Close();
            sr.Dispose();
            ms.Dispose();
            return sB.ToString();
        }
    }
}



