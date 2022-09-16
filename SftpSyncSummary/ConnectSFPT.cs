using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WinSCP;

namespace SftpSyncSummary
{
    public class ConnectSFPT
    {
        public int DaysToDownload { get; set; }

        public struct SFTP_Parameter
        {
            public string Site_Name { get; set; }
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

        public ConnectSFPT(int daysToDownload)
        {
            DaysToDownload = daysToDownload;
            // Blank new class.
        }

        public List<SFTP_Parameter> GetParameters(string CSV_Path)
        {
            List<SFTP_Parameter> parameters = new List<SFTP_Parameter>();
            var all_lines = File.ReadAllLines(CSV_Path);
            SFTP_Parameter sFTP_Parameter = new SFTP_Parameter();
            for (int i = 1; i < all_lines.Length; i++)
            {
                var parameter_list = all_lines[i].Split(',');
                sFTP_Parameter.Site_Name = parameter_list[0];
                sFTP_Parameter.EC_IpAddress = parameter_list[1];
                sFTP_Parameter.EC_Port = Convert.ToInt32(parameter_list[2]);
                sFTP_Parameter.EC_UserName = parameter_list[3];
                sFTP_Parameter.EC_Password = parameter_list[4];
                sFTP_Parameter.EC_SummaryPath = parameter_list[5];
                sFTP_Parameter.HPRCC_IpAddress = parameter_list[6];
                sFTP_Parameter.HPRCC_Port = Convert.ToInt32(parameter_list[7]);
                sFTP_Parameter.HPRCC_UserName = parameter_list[8];
                sFTP_Parameter.HPRCC_Password = parameter_list[9];
                sFTP_Parameter.HPRCC_SummaryPath = parameter_list[10];
                parameters.Add(sFTP_Parameter);
            }
            return parameters;
        }

        public void RunSingleSync(SFTP_Parameter sFTP_Parameter)
        {
            List<string> SummaryFiles = DownloadSummaryFiles(sFTP_Parameter);
            if (SummaryFiles.Count > 0)
            {
                UploadSummaryFiles(sFTP_Parameter, SummaryFiles);
            }
            else
            {
                Console.WriteLine("There was some error when downloading summary files.");
            }
        }

        private List<string> DownloadSummaryFiles(SFTP_Parameter sFTP_Parameter)
        {
            List<string> local_summary_files = new List<string>();
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
                    SshHostKeyPolicy = SshHostKeyPolicy.GiveUpSecurityAndAcceptAny
                };

                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;
                    List<RemoteFileInfo> remoteFileInfo = new List<RemoteFileInfo>();
                    var directoryInfo = session.ListDirectory("/home/licor/data/summaries/");
                    foreach (RemoteFileInfo file in directoryInfo.Files)
                    {
                        if (file.Length > 0 && file.LastWriteTime.Date < DateTime.Today)
                        {
                            remoteFileInfo.Add(file);
                        }
                    }
                    remoteFileInfo = remoteFileInfo.OrderByDescending(s => s.Name).ToList();

                    for (int i = 0; i < DaysToDownload; i++)
                    {
                        session.GetFiles(remoteFileInfo[i].FullName, ".\\", false, null);
                        local_summary_files.Add(remoteFileInfo[i].Name);
                    }
                }
                return local_summary_files;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return local_summary_files;
            }
        }

        private void UploadSummaryFiles(SFTP_Parameter sFTP_Parameter, List<string> SummaryFiles)
        {
            try
            {
                // Setup session options
                var sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = sFTP_Parameter.HPRCC_IpAddress,
                    UserName = sFTP_Parameter.HPRCC_UserName,
                    Password = sFTP_Parameter.HPRCC_Password,
                    PortNumber = sFTP_Parameter.HPRCC_Port,
                    SshHostKeyPolicy = SshHostKeyPolicy.GiveUpSecurityAndAcceptAny
                };

                using (var session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    // Upload files
                    var transferOptions = new TransferOptions
                    {
                        TransferMode = TransferMode.Binary,
                        PreserveTimestamp = true
                    };
                    foreach (var summaryFile in SummaryFiles)
                    {
                        var hccFilePath = sFTP_Parameter.HPRCC_SummaryPath + summaryFile;
                        var localFileInfo = new FileInfo(summaryFile);
                        if (!session.FileExists(hccFilePath) || session.GetFileInfo(hccFilePath).Length < localFileInfo.Length)
                        {
                            session.PutFiles(summaryFile, hccFilePath, true, transferOptions);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            foreach (var file in SummaryFiles)
            {
                File.Delete(file);
            }
        }
    }
}