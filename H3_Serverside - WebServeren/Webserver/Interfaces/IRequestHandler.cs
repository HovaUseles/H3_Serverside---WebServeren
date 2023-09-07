using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace H3_Serverside___WebServeren.Webserver.Interfaces
{
    public interface IRequestHandler
    {
        /// <summary>
        /// Method for handling request to the server
        /// </summary>
        /// <param name="clientSocket">The client socket sending the request</param>
        /// <param name="contentPath">The path to the content</param>
        public void HandleRequest(Socket clientSocket, string contentPath)
        {

        }
    }
}
