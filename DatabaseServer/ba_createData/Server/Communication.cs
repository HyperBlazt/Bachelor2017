using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace ba_createData.Server
{
    public static class Communication
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientStream"></param>
        /// <returns></returns>
        public static string GetMessageFromClient(SslStream clientStream)
        {
            if (clientStream == null) throw new ArgumentNullException(nameof(clientStream));
            var messageData = new StringBuilder();
            var message = new byte[4096];
            int bytes;
            do
            {

                bytes = clientStream.Read(message, 0, message.Length);
                var decoder = Encoding.ASCII.GetDecoder();
                var chars = new char[decoder.GetCharCount(message, 0, bytes)];
                decoder.GetChars(message, 0, bytes, chars, 0);
                messageData.Append(chars);
                // Check for end of file.
                if (messageData.ToString().IndexOf("\r\n", StringComparison.Ordinal) != -1)
                {
                    break;
                }
            } while (bytes != -1);
            return messageData.ToString().Replace("\r\n", string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientStream"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void SendMessageToClient(SslStream clientStream, string message)
        {
            var EOF = "\r\n";
            var bytesToSend = Encoding.ASCII.GetBytes(message + EOF);
            clientStream.Write(bytesToSend);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientStream"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static void SendFileToClient(SslStream clientStream, FileInfo file)
        {
            var EOF = "\r\n";
            //var bytesToSend = Encoding.ASCII.GetBytes(message + EOF);
            //FileStream inputStream = File.OpenRead(filePath);
            //FileInfo f = new FileInfo(file);
            var size = unchecked((int)file.Length);
            byte[] byteSize = Encoding.ASCII.GetBytes(size.ToString());
            clientStream.Write(byteSize);
        }


        public static void ClientTalk(SslStream clientStream)
        {
            while (true)
            {
                try
                {
                    //blocks until a client sends a message
                    var result = GetMessageFromClient(clientStream);
                    switch (result)
                    {
                        case "LOOK UP SINGLE HASH":
                            // Send go ahead 
                            SendMessageToClient(clientStream, "200 OK");
                            var hash = GetMessageFromClient(clientStream);
                            if (hash.Length >= 32)
                            {
                                // create a new process so the client can send a new request
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        var hashToCheck = GetMessageFromClient(clientStream);
                                        var newScanner = new Scanner.Scanner();
                                        var isFileClear = newScanner.ScanFile(hashToCheck);

                                    }
                                    catch (Exception)
                                    {

                                        SendMessageToClient(clientStream, "901 FAIL");
                                    }
                                });
                            }
                            break;
                        case "LOOK UP STARTUPFILES":
                            break;
                        case "IS CLIENT ALIVE":
                            // check Connection
                            break;
                        default:
                            SendMessageToClient(clientStream, "900 FAIL");
                            break;
                    }
                }
                catch
                {
                    //a socket error has occured
                    break;
                }
            }
        }
    }
}
