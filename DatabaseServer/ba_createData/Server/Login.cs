using System.Collections.Generic;
using System.Net.Security;

namespace ba_createData.Server
{
    public static class Login
    {
        // Test credential parameter
        public static Dictionary<string, string> Credentials = new Dictionary<string, string> { { "USER", "1234" }};

        public static bool LoginProcedure(SslStream clientStream)
        {
            Communication.SendMessageToClient(clientStream, "200 OK");
            // Retrieve user name from client
            var userNameResponse = Communication.GetMessageFromClient(clientStream);
            if (!Credentials.ContainsKey(userNameResponse.Trim())) return false;
            // Username is recieved, tell client to send password
            Communication.SendMessageToClient(clientStream, Const.Const.SendPassword);

            // Retrieve password from client
            var password = Communication.GetMessageFromClient(clientStream);
            var clientCredential = Credentials[userNameResponse];
            if (!clientCredential.Equals(password)) return false;
            // Password accepted, send message to client that success is accepted.
            Communication.SendMessageToClient(clientStream, "LOGIN 200 OK");
            return true;
        }
    }
}
