using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace H3_Serverside___WebServeren.Webserver.Interfaces
{
    public interface IResponseHandler
    {
        /// <summary>
        /// Sends a response to the client that the requested ressource was not implemented
        /// </summary>
        /// <param name="clientSocket"></param>
        public void NotImplemented(Socket clientSocket);

        /// <summary>
        /// Sends a response to the client that the requested ressource was not found
        /// </summary>
        /// <param name="clientSocket"></param>
        public void NotFound(Socket clientSocket);

        /// <summary>
        /// Sends a response to the client that the requested was successful
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        public void SendOkResponse(Socket clientSocket, byte[] content, string contentType);
    }
}
