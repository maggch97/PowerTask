using System;
using System.Threading.Tasks;
using Xunit;

namespace VtConnect.Tests
{
    public class ConnectionTests
    {
        [Fact]
        public void TestFactory()
        {
            var telnetConnection = Connection.CreateConnection(new Uri("telnet://localhost"));
            Assert.NotNull(telnetConnection);
            Assert.IsType<Telnet.TelnetConnection>(telnetConnection);

            var sshConnection = Connection.CreateConnection(new Uri("ssh://localhost"));
            Assert.NotNull(sshConnection);
            Assert.IsType<SSH.SshConnection>(sshConnection);

            var badConnection = Connection.CreateConnection(new Uri("bad://localhost"));
            Assert.Null(badConnection);
        }

        [Fact]
        public void ConnectSshUsernameAndPassword()
        {
            var credentials = new UsernamePasswordCredentials
            {
                Username = "admin",
                Password = "Minions12345"
            };

            var destination = new Uri("ssh://10.100.5.100");
            var connection = Connection.CreateConnection(destination);
            Assert.NotNull(connection);

            Assert.True(connection.Connect(destination, credentials));
        }
    }
}
