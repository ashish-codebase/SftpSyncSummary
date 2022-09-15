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
            var Connections = new ConnectSFPT("");
            List<ConnectSFPT.SFTP_Parameter> connections = Connections.ConnectionList;
            foreach (var connection in connections)
            {
                Thread t = new Thread(()=>RunSingleSync(connection));
                t.Start();
            }

            void RunSingleSync(ConnectSFPT.SFTP_Parameter sFTP_Parameter)
            {
                try
                {
                    // Setup session options
                    SessionOptions sessionOptions = new SessionOptions
                    {
                        Protocol = Protocol.Sftp,
                        HostName = sFTP_Parameter.EC_IpAddress,
                        UserName = sFTP_Parameter.EC_UserName,
                        Password = sFTP_Parameter.EC_Password,
                        PortNumber = sFTP_Parameter.EC_Port,
                        RootPath = sFTP_Parameter.EC_SummaryPath
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
                    Console.WriteLine($"Error: {e.Message}");
                }
            }
        }



    }

}