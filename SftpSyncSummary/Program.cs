using System;
using System.Collections.Generic;
using System.Threading;

namespace SftpSyncSummary
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int daysToDownload = 1;
            if (args.Length > 0)
            {
                daysToDownload = Convert.ToInt16(args[0]);
            }
            ConnectSFPT connectSFPT = new ConnectSFPT(daysToDownload);

            List<ConnectSFPT.SFTP_Parameter> ConnectionList = connectSFPT.GetParameters("EC_Summary_Sync_Settings.csv");

            foreach (var connection in ConnectionList)
            {
                Thread thread = new Thread(() =>
                {
                    connectSFPT.RunSingleSync(connection);
                });
                thread.Start();

            }
        }
    }
}