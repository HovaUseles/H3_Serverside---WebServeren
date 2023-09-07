using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using H3_Serverside___WebServeren.Webserver.Interfaces;

namespace H3_Serverside___WebServeren.Webserver
{
    public class WebServer : IWebServer
    {

        public bool IsRunning { get; set; }

        private int _timeout { get; set; }
        private Socket _socket { get; set; }

        private IRequestHandler _requestHandler;

        // Constructors
        public WebServer(IRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        // Methods
        public bool Start(IPAddress ipAddress, int port, int maxConnections, string contentPath)
        {
            // If server is already running, return false, Server was not started.
            if (IsRunning)
            {
                return false;
            }

            try
            {
                // A tcp/ip socket (ipv4)
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                               ProtocolType.Tcp);
                _socket.Bind(new IPEndPoint(ipAddress, port));
                _socket.Listen(maxConnections);
                _socket.ReceiveTimeout = _timeout;
                _socket.SendTimeout = _timeout;
                IsRunning = true;
            }
            catch (Exception ex)
            {
                return false; // An exception occured, return false
            }

            // Set up threads that will start new threads when requests are received
            Thread requestListenerT = new Thread(() =>
            {
                while (IsRunning)
                {
                    Socket clientSocket;
                    try
                    {
                        clientSocket = _socket.Accept();
                        // When new client is connected, start up new thread to handle client requests
                        Thread requestHandler = new Thread(() =>
                        {
                            clientSocket.ReceiveTimeout = _timeout;
                            clientSocket.SendTimeout = _timeout;
                            try
                            {
                                _requestHandler.HandleRequest(clientSocket, contentPath);
                            }
                            catch
                            {
                                try
                                {
                                    clientSocket.Close();
                                }
                                catch
                                {

                                }
                            }
                        });
                        requestHandler.Start();
                    }
                    catch
                    {

                    }
                }
            });
            requestListenerT.Start(); // Begin the thread

            return true; // If everythings is set up, server was started and we return true
        }

        public void Stop()
        {
            // If Server is running try to close the socket
            if (IsRunning)
            {
                IsRunning = false;
                try
                {
                    _socket.Close();
                }
                catch
                {

                }
                _socket = null;
            }
        }

    }
}
