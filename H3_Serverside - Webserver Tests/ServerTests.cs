using Autofac.Extras.Moq;
using H3_Serverside___WebServeren.Webserver;
using H3_Serverside___WebServeren.Webserver.Interfaces;
using System.Net;
using System.Security.Principal;

namespace H3_Serverside___Webserver_Tests
{
    public class ServerTests
    {
        [Fact]
        public void Start_InvalidIpAddressShouldThrow()
        {
            // Arrange
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IRequestHandler>();

                var server = mock.Create<WebServer>();
                try
                {
                    IPAddress iPAddress = new IPAddress(new byte[] {0, 2, 4});
                    int port = 443;
                    int maxCon = 2;
                    string path = "test/test";
                }
                catch (ArgumentException ex) 
                {
                    // Assert
                    Assert.Equal("address", ex.ParamName);
                }

            }
        }

        [Fact]
        public void Start_ServerIsRunningShouldNotStart()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange 
                IPAddress iPAddress = new IPAddress(new byte[] { 10, 108, 130, 163 });
                int port = 443;
                int maxCon = 2;
                string path = "test/test";

                mock.Mock<IRequestHandler>();

                var server = mock.Create<WebServer>();

                // Act
                server.Start(iPAddress, port, maxCon, path); // Starting server so its already running
                bool secondStart = server.Start(iPAddress, port, maxCon, path);

                // Assert
                Assert.False(secondStart);
                Assert.True(server.IsRunning);
            }
        }

        [Fact]
        public void Stop_ServerIsRunningShouldStop()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange 
                IPAddress iPAddress = new IPAddress(new byte[] { 10, 108, 130, 163 });
                int port = 443;
                int maxCon = 2;
                string path = "test/test";

                mock.Mock<IRequestHandler>();

                var server = mock.Create<WebServer>();

                // Act
                server.Start(iPAddress, port, maxCon, path); // Starting server so its already running
                Thread.Sleep(100);
                server.Stop();

                // Assert
                Assert.False(server.IsRunning);
            }
        }

    }
}