using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using H3_Serverside___WebServeren.Webserver.Interfaces;
using static System.Net.WebRequestMethods;

namespace H3_Serverside___WebServeren.Webserver
{
    public class ResponseHandler : IResponseHandler
    {
        private Encoding _encoder { get; set; } = Encoding.UTF8;

        private string _errorResponseTitle = "Simple Webserver";

        public void NotFound(Socket clientSocket)
        {
            SendResponse(clientSocket,
                "<html><head><meta http - equiv =\"Content-Type\" content=\"text/html; charset = utf - 8\">" +
                "</head><body><h2>" + _errorResponseTitle + "</ h2 >< div > 404 - NotFound </ div ></ body ></ html > ",
                "404 Not Found", "text/html");
        }

        public void NotImplemented(Socket clientSocket)
        {
            SendResponse(clientSocket,
                "<html><head><meta http - equiv =\"Content-Type\" content=\"text/html; charset = utf - 8\">" +
                "</head><body><h2>" + _errorResponseTitle + "</h2><div> 501 - Method NotImplemented </div></body></html>",
                "501 Not Implemented", "text/html");
        }

        public void SendOkResponse(Socket clientSocket, byte[] content, string contentType)
        {
            SendResponse(clientSocket, content, "200 OK", contentType);
        }


        private void SendResponse(Socket clientSocket, string strContent, string responseCode,
                                  string contentType)
        {
            byte[] bContent = _encoder.GetBytes(strContent);
            SendResponse(clientSocket, bContent, responseCode, contentType);
        }

        private void SendResponse(Socket clientSocket, byte[] bContent, string responseCode,
                                  string contentType)
        {
            try
            {
                byte[] bHeader = _encoder.GetBytes(
                                    "HTTP/1.1 " + responseCode + "\r\n"
                                  + "Server: Atasoy Simple Web Server\r\n"
                                  + "Content-Length: " + bContent.Length.ToString() + "\r\n"
                                  + "Connection: close\r\n"
                                  + "Content-Type: " + contentType + "\r\n\r\n");
                clientSocket.Send(bHeader);
                clientSocket.Send(bContent);
                clientSocket.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
