using DuplicateFinder.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DuplicateFinder.Utils.Common
{
    public class LogManager
    {
        private readonly string logDir = Environment.CurrentDirectory + "\\log\\";

        private void CreateLogDirectory()
        {
            if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);
        }

        public void RecordLogInfo(string logType, string logInfo, string logMark)
        {
            CreateLogDirectory();
            DateTime dt = DateTime.Now;
            string logFileName = logDir + dt.ToString("yyyy_MM") + "_DuplicateFinder.log";
            FileStream fs = new FileStream(logFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            StringBuilder sb = new StringBuilder();
            sb.Append(dt.ToString("yyyy-MM-dd HH:mm:ss")).Append("\r\n");
            sb.Append("[").Append(logType).Append("] ").Append(logInfo);
            sb.Append(" --> ").Append(logMark).Append("\r\n\r\n");
            sw.Write(sb.ToString());
            sw.Close();
            fs.Close();
        }

        public void RecordDuplicateFiles(List<DupFileInfo> dupFileInfoList)
        {
            CreateLogDirectory();
            FileStream fso = null;
            StreamWriter sw = null;
            string saveFile = logDir + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_DuplicateFiles.log";

            try
            {
                fso = new FileStream(saveFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                sw = new StreamWriter(fso, Encoding.UTF8);
                sw.WriteLine("SN,MD5,NAME,TYPE,SIZE,PATH");
                StringBuilder sb = new StringBuilder();
                int line = 0;

                foreach (DupFileInfo dfi in dupFileInfoList)
                {
                    try
                    {
                        sb.Append(dfi.SN.ToString()).Append(",");
                        sb.Append(dfi.MD5).Append(",");
                        sb.Append(dfi.Name).Append(",");
                        sb.Append(dfi.Type).Append(",");
                        sb.Append(dfi.Size).Append(",");
                        sb.Append(dfi.Path).Append("\r\n");
                        line++;

                        if (line == 5000)
                        {
                            sw.Write(sb.ToString());
                            line = 0;
                            sb = new StringBuilder();
                        }
                    }
                    catch (Exception e)
                    {
                        sw.WriteLine("[ERROR]," + e.Message);
                    }
                }

                if (line > 0) sw.Write(sb.ToString());
            }
            catch (Exception ex)
            {
                sw.WriteLine("[ERROR]," + ex.Message);
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fso != null) fso.Close();
            }
        }
    }
}
