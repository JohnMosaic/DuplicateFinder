using DuplicateFinder.Model;
using System.Collections.Generic;
using System.IO;

namespace DuplicateFinder.Utils.Common
{
    public class FileSearcher
    {
        public void Search(string rootDir, StatusInfo statusInfo, Dictionary<string, Dictionary<long, List<FileInfo>>> fileDict)
        {
            DirectoryInfo dir = new DirectoryInfo(rootDir);
            FileInfo[] fiArr = dir.GetFiles();
            DirectoryInfo[] dirArr = dir.GetDirectories();

            foreach (FileInfo f in fiArr)
            {
                try
                {
                    string type = f.Extension;
                    long size = f.Length;

                    if (!fileDict.ContainsKey(type))
                    {
                        List<FileInfo> fList = new List<FileInfo> { f };
                        Dictionary<long, List<FileInfo>> kvDict = new Dictionary<long, List<FileInfo>> { { size, fList } };
                        fileDict.Add(type, kvDict);
                    }
                    else
                    {
                        if (!fileDict[type].ContainsKey(size))
                        {
                            List<FileInfo> fList = new List<FileInfo> { f };
                            fileDict[type].Add(size, fList);
                        }
                        else fileDict[type][size].Add(f);
                    }

                    statusInfo.TotalCount++;
                }
                catch
                {
                    //Skip files without access
                }
            }

            foreach (DirectoryInfo d in dirArr)
            {
                try
                {
                    Search(d.FullName, statusInfo, fileDict);
                }
                catch
                {
                    //Skip directories without access
                }
            }
        }
    }
}
