using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace ba_createData.Server
{
    public class MainServerControl : Form1
    {


        public static TcpListener _clientListener;

        public static List<TcpClient> ActiveTcpClients;

        public bool ServerIsActive;

        public MainServerControl()
        {
            ActiveTcpClients = new List<TcpClient>();
            _clientListener = new TcpListener(IPAddress.Any, 3000);
            var listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Start();
        }

        private void ListenForClients()
        {
            _clientListener.Start();
            ServerIsActive = true;
            while (ServerIsActive)
            {
                try
                {
                    //blocks until a client has connected to the server
                    var client = _clientListener.AcceptTcpClient();
                    SetMainTime(client.Client.RemoteEndPoint + " Enters login phase..." + Environment.NewLine);
                    // Accept multiple clients
                    ThreadPool.QueueUserWorkItem(ThreadProc, client);

                }
                catch (Exception e)
                {                  
                    continue;
                }
            }
        }

        public bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None) return true;
            Console.WriteLine(@"Certificate error: {0}", sslPolicyErrors);
            // Do not allow this client to communicate with unauthenticated servers. 
            return true;
        }



        private void ThreadProc(object obj)
        {
            var client = (TcpClient)obj;
            var tcpClient = client;

            // SSL Stream to secure communication
            var sslStream = (new SslStream(client.GetStream(), false, ValidateServerCertificate, null));
            var serverCertificate = X509Certificate.CreateFromCertFile(Thread.GetDomain().BaseDirectory + "rolandio.cer");
            sslStream.AuthenticateAsServer(serverCertificate,
                false, SslProtocols.Default, false);

            // Getting Message From Client
            var message = Communication.GetMessageFromClient(sslStream);
            switch (message)
            {
                case "LOGIN":
                    // Send client to login procedure
                    if (Login.LoginProcedure(sslStream))
                    {
                        ActiveTcpClients.Add(tcpClient);
                        Communication.ClientTalk(sslStream);
                    }
                    else
                    {
                        Communication.SendMessageToClient(sslStream, "910 NOT SUCCESS");
                    }
                    break;
                default:
                    Communication.SendMessageToClient(sslStream, "911 NOT SUCCESS");
                    break;
            }

            ActiveTcpClients.Remove(tcpClient);
            tcpClient.Close();
        }
    }
}
