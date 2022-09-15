using System;
using System.Collections.Generic;
using WinSCP;

namespace SftpSyncSummary
{
    public class ConnectSFPT
    {
        public struct SFTP_Parameter
        {
            public string EC_IpAddress { get; set; }
            public int EC_Port { get; set; }
            public string EC_UserName { get; set; }
            public string EC_Password { get; set; }
            public string EC_SummaryPath { get; set; }
            public string HPRCC_IpAddress { get; set; }
            public int HPRCC_Port { get; set; }
            public string HPRCC_UserName { get; set; }
            public string HPRCC_Password { get; set; }
            public string HPRCC_SummaryPath { get; set; }
        }

        public ConnectSFPT()
        {

        }
        public List<SFTP_Parameter> GetParameters(string CSV_Path)
        {
            return GetCSVFile(CSV_Path);
        }

        private List<SFTP_Parameter> GetCSVFile(string CSV_Path)
        {
            return default;
        }

        public void RunSingleSync(SFTP_Parameter sFTP_Parameter)
        {
            bool result = GetSummaryFile(sFTP_Parameter);
            if (result)
            {
                UploadSummaryFile(sFTP_Parameter);
            }
            else
            {
                Console.WriteLine("File not downloaded");
            }
        }

        private bool GetSummaryFile(SFTP_Parameter sFTP_Parameter)
        {
            string local_summary_text;
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
                };

                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult transferResult;
                    var directoryInfo = session.ListDirectory("/home/licor/data/summaries/");
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
            return true;
        }
        private void UploadSummaryFile(SFTP_Parameter sFTP_Parameter)
        {
            string local_summary_text;
            try
            {
                // Setup session options
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = sFTP_Parameter.HPRCC_IpAddress,
                    UserName = sFTP_Parameter.HPRCC_UserName,
                    Password = sFTP_Parameter.HPRCC_Password,
                    PortNumber = sFTP_Parameter.HPRCC_Port,
                };

                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult transferResult;
                    var directoryInfo = session.ListDirectory("/home/licor/data/summaries/");
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