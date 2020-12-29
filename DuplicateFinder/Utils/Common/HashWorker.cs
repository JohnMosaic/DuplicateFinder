using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DuplicateFinder.Utils.Common
{
    public class HashWorker
    {
        private readonly LogManager logManager = new LogManager();

        public string zeroSizeFileMD5 = "d41d8cd98f00b204e9800998ecf8427e";

        public string CalcPartFileMD5(string inputFile)
        {
            StringBuilder sb = new StringBuilder();
            FileStream fs = null;

            try
            {
                fs = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                MD5CryptoServiceProvider md5CSP = new MD5CryptoServiceProvider();
                long len = fs.Length;

                if (len < 8) sb.Append(CalcTotalFileMD5(inputFile));
                else
                {
                    byte[] buffer = new byte[3];
                    long offset = len / 2;
                    fs.Seek(offset - 4, SeekOrigin.Begin);
                    buffer[0] = (byte)fs.ReadByte();
                    fs.Seek(offset, SeekOrigin.Begin);
                    buffer[1] = (byte)fs.ReadByte();
                    fs.Seek(offset + 3, SeekOrigin.Begin);
                    buffer[2] = (byte)fs.ReadByte();
                    byte[] byteHash = md5CSP.ComputeHash(buffer);
                    md5CSP.Clear();

                    foreach (byte b in byteHash) sb.Append(b.ToString("x2"));
                }
            }
            catch (Exception ex)
            {
                logManager.RecordLogInfo("CALC_PART_MD5_ERROR", ex.Message, inputFile);
            }
            finally
            {
                if (fs != null) fs.Close();
            }

            return sb.ToString();
        }

        public string CalcTotalFileMD5(string inputFile)
        {
            StringBuilder sb = new StringBuilder();
            FileStream fs = null;

            try
            {
                fs = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                MD5CryptoServiceProvider md5CSP = new MD5CryptoServiceProvider();
                byte[] byteHash = md5CSP.ComputeHash(fs);
                md5CSP.Clear();

                foreach (byte b in byteHash) sb.Append(b.ToString("x2"));
            }
            catch (Exception ex)
            {
                logManager.RecordLogInfo("CALC_TOTAL_MD5_ERROR", ex.Message, inputFile);
            }
            finally
            {
                if (fs != null) fs.Close();
            }

            return sb.ToString();
        }
    }
}
