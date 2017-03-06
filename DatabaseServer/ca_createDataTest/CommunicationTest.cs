using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using ba_createData.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ca_createDataTest
{

    [TestClass]
    public class CommunicationTest
    {

        private SslStream _loggedInCredentials;
        private const string Login = "LOGIN\r\n";
        private const string Lookupsinglemalware = "LOOK UP SINGLE HASH\r\n";

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None) return true; Console.WriteLine(@"Certificate error: {0}", sslPolicyErrors); // Do not allow this client to communicate with unauthenticated servers. 
            return true;
        }


        /// <summary>
        /// Gets the message from server
        /// </summary>
        /// <param name="clientStream"></param>
        /// <returns></returns>
        private static string GetMessageFromServer(SslStream clientStream)
        {
            if (clientStream == null) throw new ArgumentNullException(nameof(clientStream));
            var messageData = new StringBuilder();
            var message = new byte[512];
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
            }

            while (bytes != -1);
            return messageData.ToString().Replace("\r\n", string.Empty);
        }


        /// <summary>
        /// Send message to server
        /// </summary>
        /// <param name="clientStream"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static void SendMessageToServer(SslStream clientStream, string message)
        {
            var EOF = "\r\n";
            var bytesToSend = Encoding.ASCII.GetBytes(message + EOF);
            clientStream.Write(bytesToSend);
        }

        [TestMethod]
        public void LoginToServer()
        {
            // Start server
            var serverControl = new MainServerControl();


            // ADD REMOTE ACCESS
            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
            var clientConnection = new TcpClient();
            clientConnection.Connect("192.168.1.217", 3000);

            var clientStream = new SslStream(clientConnection.GetStream(), false, ValidateServerCertificate, null);
            var serverCertificate = X509Certificate.CreateFromCertFile(Thread.GetDomain().BaseDirectory + "rolandio.cer");
            var certificates = new X509CertificateCollection(new[] { serverCertificate });
            clientStream.AuthenticateAsClient("192.168.1.217", certificates, SslProtocols.Default, false);
            var bytesToSend = Encoding.ASCII.GetBytes("LOGIN\r\n");
            clientStream.Write(bytesToSend, 0, bytesToSend.Length);


            // Get Response
            var loginResponse = GetMessageFromServer(clientStream);
            if (loginResponse.Trim() == "200 OK")
            {
                SendMessageToServer(clientStream, "CLIENT_1027\r\n");

                // Get Response
                var userMessage = GetMessageFromServer(clientStream);
                if (userMessage.Equals("PASSWORD"))
                {
                    SendMessageToServer(clientStream, "12345678\r\n");

                    // Get Response
                    var loginIsAcceptedResponse = GetMessageFromServer(clientStream);
                    if (loginIsAcceptedResponse.Equals("LOGIN 200 OK"))
                    {
                        _loggedInCredentials = clientStream;


                        // Simmple test
                        SendMessageToServer(clientStream, "LOOK UP SINGLE HASH\r\n");
                        var readyForHash = GetMessageFromServer(clientStream);
                        if (readyForHash.Equals("200 OK"))
                        {
                            SendMessageToServer(clientStream, "00f538c3d410822e241486ca061a57ee$\r\n");
                            var report = GetMessageFromServer(clientStream);
                        }
                    }
                }
            }
        }
    }
}
