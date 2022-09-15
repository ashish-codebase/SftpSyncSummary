using System;
using System.Collections.Generic;
using WinSCP;
using System.Threading;
using System.Threading.Tasks;

namespace SftpSyncSummary
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var Connections = new SFTP_Connection("");
            List<SFTP_Parameter> connections = Connections.ConnectionList;
            foreach (var connection in connections)
            {
                Thread t = new Thread(()=>RunSingleSync(connection));
                t.Start();
            }

            void RunSingleSync(SFTP_Parameter sFTP_Parameter)
            {
                try
                {
                    // Setup session options
                    SessionOptions sessionOptions = new SessionOptions
                    {
                        Protocol = Protocol.Sftp,
                        HostName = sFTP_Parameter.IpAddress,
                        UserName = sFTP_Parameter.UserName,
                        Password = sFTP_Parameter.Password,
                        PortNumber = sFTP_Parameter.Port,
                        RootPath = sFTP_Parameter.SummaryPath
                    };

                    using (Session session = new Session())
                    {
                        // Connect
                        session.Open(sessionOptions);

                        // Upload files
                        TransferOptions transferOptions = new TransferOptions();
                        transferOptions.TransferMode = TransferMode.Binary;

                        TransferOperationResult transferResult;
                        transferResult =
                            session.PutFiles(@"d:\toupload\*", "/home/user/", false, transferOptions);

                        // Throw on any error
                        transferResult.Check();

                        // Print results
                        foreach (TransferEventArgs transfer in transferResult.Transfers)
                        {
                            Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e);
                }
                return default;
            }
            List<SFTP_Connection> GetAllSFTPParameters(string SettingsPath)
            {
                return default;
            }
        }

        public struct SFTP_Parameter
        {
            public string IpAddress { get; set; }
            public int Port { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string SummaryPath { get; set; }
        }

        public class SFTP_Connection
        {
            public List<SFTP_Parameter> ConnectionList { get; set; }

            public SFTP_Connection(string CSV_Path)
            {
                ConnectionList = GetCSVFile(CSV_Path);
            }

            private List<SFTP_Parameter> GetCSVFile(string CSV_Path)
            {
                return default;
            }
        }
    }
}