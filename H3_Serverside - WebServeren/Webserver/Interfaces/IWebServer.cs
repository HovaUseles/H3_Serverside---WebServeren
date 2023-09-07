using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace H3_Serverside___WebServeren.Webserver.Interfaces
{
    public interface IWebServer
    {
        /// <summary>
        /// Starts the server if its not allready running
        /// </summary>
        /// <param name="ipAddress">IP Address the server should run</param>
        /// <param name="port">Port for the server</param>
        /// <param name="maxConnections">Maximum number of connections</param>
        /// <param name="contentPath">The path to the content</param>
        /// <returns>If the server was started. Returns false if its was already running.</returns>
        public bool Start(IPAddress ipAddress, int port, int maxConnections, string contentPath);

        /// <summary>
        /// Stop the server and closes connections
        /// </summary>
        public void Stop();
    }
}
