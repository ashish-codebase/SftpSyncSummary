using System.Collections.Generic;

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

        public List<SFTP_Parameter> ConnectionList { get; set; }

        public ConnectSFPT(string CSV_Path)
        {
            ConnectionList = GetCSVFile(CSV_Path);
        }

        private List<SFTP_Parameter> GetCSVFile(string CSV_Path)
        {
            return default;
        }
    }

}