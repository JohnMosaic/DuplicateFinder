using DuplicateFinder.Model;
using DuplicateFinder.Utils.Common;
using System.Collections.Generic;
using System.IO;

namespace DuplicateFinder.Utils.Engine
{
    public class DupAlgorithm
    {
        private readonly HashWorker hashWorker = new HashWorker();
        private Dictionary<string, string> totalFileMD5Dict = new Dictionary<string, string>();
        private Dictionary<string, string> partFileMD5Dict = new Dictionary<string, string>();

        public void GetDupFileInfos(StatusInfo statusInfo, List<DupFileInfo> dupFileInfoList, Dictionary<string, Dictionary<long, List<FileInfo>>> fileDict)
        {
            TypeFormatter typeFormatter = new TypeFormatter();
            int sn = 0;

            foreach (string type in fileDict.Keys)
            {
                foreach (long size in fileDict[type].Keys)
                {
                    int fileCount = fileDict[type][size].Count;

                    if (fileCount > 1)
                    {
                        if (size == 0)
                        {
                            for (int i = 0; i < fileCount; i++)
                            {
                                DupFileInfo dupFileInfo = new DupFileInfo();

                                if (i == 0) dupFileInfo.IsDel = false;
                                else
                                {
                                    dupFileInfo.IsDel = true;
                                    statusInfo.DupCount++;
                                }

                                dupFileInfo.SN = ++sn;
                                dupFileInfo.MD5 = hashWorker.zeroSizeFileMD5;
                                dupFileInfo.Name = fileDict[type][size][i].Name;
                                dupFileInfo.Type = type;
                                dupFileInfo.Size = "0B";
                                dupFileInfo.Path = fileDict[type][size][i].FullName;
                                dupFileInfoList.Add(dupFileInfo);
                                statusInfo.ProgressValue++;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < fileCount; i++)
                            {
                                FileInfo file1 = fileDict[type][size][i];
                                bool isDupFind = false;

                                for (int j = i + 1; j < fileCount; j++)
                                {
                                    FileInfo file2 = fileDict[type][size][j];

                                    if (IsDupFile(file1, file2))
                                    {
                                        DupFileInfo dupFileInfo = new DupFileInfo
                                        {
                                            IsDel = true,
                                            SN = ++sn,
                                            MD5 = totalFileMD5Dict[file2.FullName],
                                            Name = file2.Name,
                                            Type = type,
                                            Size = typeFormatter.GetFileSize(size),
                                            Path = file2.FullName
                                        };

                                        dupFileInfoList.Add(dupFileInfo);
                                        statusInfo.DupCount++;
                                        isDupFind = true;

                                        fileDict[type][size].RemoveAt(j);
                                        j--;
                                        fileCount--;
                                        statusInfo.ProgressValue++;
                                    }
                                }

                                if (isDupFind)
                                {
                                    DupFileInfo dupFileInfo = new DupFileInfo
                                    {
                                        IsDel = false,
                                        SN = ++sn,
                                        MD5 = totalFileMD5Dict[file1.FullName],
                                        Name = file1.Name,
                                        Type = type,
                                        Size = typeFormatter.GetFileSize(size),
                                        Path = file1.FullName
                                    };

                                    dupFileInfoList.Add(dupFileInfo);
                                }

                                statusInfo.ProgressValue++;
                            }
                        }
                    }
                    else statusInfo.ProgressValue++;
                }
            }
        }

        private bool IsDupFile(FileInfo file1, FileInfo file2)
        {
            string partFile1MD5, partFile2MD5;

            if (partFileMD5Dict.ContainsKey(file1.FullName)) partFile1MD5 = partFileMD5Dict[file1.FullName];
            else
            {
                partFile1MD5 = hashWorker.CalcPartFileMD5(file1.FullName);

                if (partFile1MD5.Equals("")) return false;
                else partFileMD5Dict.Add(file1.FullName, partFile1MD5);
            }

            if (partFileMD5Dict.ContainsKey(file2.FullName)) partFile2MD5 = partFileMD5Dict[file2.FullName];
            else
            {
                partFile2MD5 = hashWorker.CalcPartFileMD5(file2.FullName);

                if (partFile2MD5.Equals("")) return false;
                else partFileMD5Dict.Add(file2.FullName, partFile2MD5);
            }

            if (partFile1MD5 != partFile2MD5) return false;
            else
            {
                string totalFile1MD5, totalFile2MD5;

                if (totalFileMD5Dict.ContainsKey(file1.FullName)) totalFile1MD5 = totalFileMD5Dict[file1.FullName];
                else
                {
                    if (file1.Length < 8) totalFile1MD5 = partFile1MD5;
                    else
                    {
                        totalFile1MD5 = hashWorker.CalcTotalFileMD5(file1.FullName);

                        if (totalFile1MD5.Equals("")) return false;
                    }

                    totalFileMD5Dict.Add(file1.FullName, totalFile1MD5);
                }

                if (totalFileMD5Dict.ContainsKey(file2.FullName)) totalFile2MD5 = totalFileMD5Dict[file2.FullName];
                else
                {
                    if (file2.Length < 8) totalFile2MD5 = partFile2MD5;
                    else
                    {
                        totalFile2MD5 = hashWorker.CalcTotalFileMD5(file2.FullName);

                        if (totalFile2MD5.Equals("")) return false;
                    }

                    totalFileMD5Dict.Add(file2.FullName, totalFile2MD5);
                }

                if (totalFile1MD5 != totalFile2MD5) return false;
                else return true;
            }
        }
    }
}
