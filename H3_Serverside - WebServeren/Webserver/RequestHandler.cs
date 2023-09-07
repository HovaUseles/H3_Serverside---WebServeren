using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using H3_Serverside___WebServeren.Webserver.Interfaces;

namespace H3_Serverside___WebServeren.Webserver
{
    public class RequestHandler : IRequestHandler
    {
        private Encoding _encoder { get; set; } = Encoding.UTF8;

        private Dictionary<string, string> _extensions = new Dictionary<string, string>()
        {
            //{ "extension", "content type" }
            { "htm", "text/html" },
            { "html", "text/html" },
            { "xml", "text/xml" },
            { "txt", "text/plain" },
            { "css", "text/css" },
            { "png", "image/png" },
            { "gif", "image/gif" },
            { "jpg", "image/jpg" },
            { "jpeg", "image/jpeg" },
            { "zip", "application/zip"}
        };
        private readonly IResponseHandler _responseHandler;

        // Constructors
        public RequestHandler(IResponseHandler responseHandler)
        {
            _responseHandler = responseHandler;
        }

        // Methods
        public void HandleRequest(Socket clientSocket, string contentPath)
        {
            byte[] buffer = new byte[10240]; // 10 kb, just in case
            int receivedBCount = clientSocket.Receive(buffer); // Receive the request
            string strReceived = _encoder.GetString(buffer, 0, receivedBCount);

            string requestedUrl;
            try
            {
                string httpMethod = ExtractHttpMethodFromRequest(strReceived);
                requestedUrl = ExtractUrlFromRequest(strReceived);
            }
            catch (ArgumentException ex)
            {
                throw new HttpRequestException("Request was invalid", ex);
            }
            catch (NotImplementedException ex)
            {
                // We only support GET and POST as of now
                _responseHandler.NotImplemented(clientSocket);
                return;
            }
            catch (InvalidOperationException ex)
            {
                throw new HttpRequestException("Request method was invalid", ex);
            }

            string requestedFile = requestedUrl.Split('?')[0];

            requestedFile = requestedFile.Replace("/", @"\").Replace("\\..", "");
            int fileStart = requestedFile.LastIndexOf('.') + 1; // Get start index for the file data
            if (fileStart > 0)
            {
                int length = requestedFile.Length - fileStart;
                string extension = requestedFile.Substring(fileStart, length);

                // Do we support this extension?
                if (_extensions.ContainsKey(extension) == false)
                {
                    _responseHandler.NotImplemented(clientSocket);
                    return;
                }

                if (File.Exists(contentPath + requestedFile) == false)
                {
                    // We don't support this extension.
                    // We are assuming that it doesn't exist.
                    _responseHandler.NotFound(clientSocket);
                    return;
                }
                // If yes check existence of the file
                // Everything is OK, send requested file with correct content type:
                _responseHandler.SendOkResponse(clientSocket,
                    File.ReadAllBytes(contentPath + requestedFile), _extensions[extension]);
            }
            else
            {
                // If file is not specified try to send index.htm or index.html
                // You can add more (default.htm, default.html)
                int length = requestedUrl.Length;
                if (requestedFile.Substring(length - 1, 1) != @"\")
                {
                    requestedFile += @"\"; // Add backslash if last char is not already backslash
                }

                if (File.Exists(contentPath + requestedFile + "index.htm"))
                {
                    _responseHandler.SendOkResponse(clientSocket,
                      File.ReadAllBytes(contentPath + requestedFile + "\\index.htm"), "text/html");
                }
                else if (File.Exists(contentPath + requestedFile + "index.html"))
                {
                    _responseHandler.SendOkResponse(clientSocket,
                      File.ReadAllBytes(contentPath + requestedFile + "\\index.html"), "text/html");
                }
                else
                {
                    _responseHandler.NotFound(clientSocket);
                }
            }
        }

        /// <summary>
        /// Extracts the HTTP method from the HTTP request
        /// </summary>
        /// <param name="receivedRequest">The HTTP request string</param>
        /// <returns>The HTTP method name</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="FormatException"></exception>
        private string ExtractHttpMethodFromRequest(string receivedRequest)
        {
            if (string.IsNullOrWhiteSpace(receivedRequest))
            {
                throw new ArgumentNullException(nameof(receivedRequest));
            }

            // Parse method of the request
            // request should look like this 
            // GET /somePath HTTP...
            //    ^
            // We can find the first whitespace and take any chars before that as the http method name
            string httpMethod = receivedRequest.Substring(0, receivedRequest.IndexOf(" "));
            try
            {
                if (ValidateHttpMethod(httpMethod))
                {
                    return httpMethod; // Request was valid
                }
                else
                {
                    throw new NotImplementedException($"HTTP method: {httpMethod} is not supported.");
                }
            }
            catch (ArgumentNullException)
            {
                throw new FormatException("Request was not valid format");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Validates that the HTTP method is supported
        /// </summary>
        /// <param name="httpMethod">The method string</param>
        /// <returns>Whether the method is supported or not</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private bool ValidateHttpMethod(string httpMethod)
        {
            if (string.IsNullOrWhiteSpace(httpMethod))
            {
                throw new ArgumentNullException(nameof(httpMethod));
            }

            switch (httpMethod)
            {
                case "GET":
                case "POST":
                    return true;
                case "PUT":
                case "DELETE":
                    return false;
                default:
                    throw new InvalidOperationException($"The method: {httpMethod} is not allowed");
            }
        }

        /// <summary>
        /// Extracts the requested Url from HTTP request
        /// </summary>
        /// <param name="receivedRequest">The HTTP request string</param>
        /// <returns>The URL from the request</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private string ExtractUrlFromRequest(string receivedRequest)
        {
            if (string.IsNullOrWhiteSpace(receivedRequest))
            {
                throw new ArgumentNullException(nameof(receivedRequest));
            }

            string httpMethod = ExtractHttpMethodFromRequest(receivedRequest); // Get Http method name
            try
            {
                int start = receivedRequest.IndexOf(httpMethod) + httpMethod.Length + 1;

                // Get the length of the Url
                // GET /index.html HTTP
                //     ^^^^^^^^^^^
                // To do this we get index of the "HTTP" and subtract length if method name trailing whitespace "GET "
                // Then subtract 1 from the leading whitespace of "HTTP"
                int length = receivedRequest.LastIndexOf("HTTP") - start - 1;

                // Find the start of the request without Http method
                // i.e.
                // GET /index.html HTTP...
                //     ^
                // Use the end of method and start of Http to get the requested Url part
                string requestedUrl = receivedRequest.Substring(start, length);
                return requestedUrl;
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException("Request was incomplete, it did not have an URL", nameof(receivedRequest));
            }
        }
    }
}
