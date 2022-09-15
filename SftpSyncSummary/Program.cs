using System.Collections.Generic;
using System.Threading;

namespace SftpSyncSummary
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ConnectSFPT connectSFPT = new ConnectSFPT();

            List<ConnectSFPT.SFTP_Parameter> ConnectionList = connectSFPT.GetParameters("csv.csv");
            foreach (var connection in ConnectionList)
            {
                Thread t = new Thread(() => connectSFPT.RunSingleSync(connection));
                t.Start();
            }
        }
    }
}