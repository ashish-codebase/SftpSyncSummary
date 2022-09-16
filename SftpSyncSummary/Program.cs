using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SftpSyncSummary
{
    internal class Program
    {
private static void Main(string[] args)
        {
            if(!File.Exists("EC_Summary_Sync_Settings.csv"))
            {
                CreateCSV();
            }
            

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

        private static void CreateCSV()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var resourceName = asm.GetManifestResourceNames().Single(str => str.EndsWith("EC_Summary_Sync_Settings.csv"));
            using (Stream InStream = asm.GetManifestResourceStream(resourceName))
            {
                StreamReader Reader = new StreamReader(InStream);
                var fstream = new FileStream("EC_Summary_Sync_Settings.csv", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                InStream.CopyTo(fstream);
                fstream.Close();
            }
        }
    }
}