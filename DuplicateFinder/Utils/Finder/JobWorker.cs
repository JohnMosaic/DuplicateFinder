using DuplicateFinder.Model;
using DuplicateFinder.Utils.Common;
using DuplicateFinder.Utils.Engine;
using DuplicateFinder.WinUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace DuplicateFinder.Utils.Finder
{
    public class JobWorker
    {
        public readonly LogManager logManager = new LogManager();
        private Dictionary<string, Dictionary<long, List<FileInfo>>> fileDict = null;
        private List<DupFileInfo> dupFileInfoList = null;
        private delegate void InvokeHandler();

        public void DoSearchWork(DupUI ui, string rootDirectory)
        {
            try
            {
                fileDict = new Dictionary<string, Dictionary<long, List<FileInfo>>>();
                new FileSearcher().Search(rootDirectory, ui.statusInfo, fileDict);
                ui.uiManager.UpdateSearchStatus(ui);
            }
            catch (Exception ex)
            {
                logManager.RecordLogInfo("SEARCH_WORK_ERROR", ex.Message, "Searching files.");
            }
            finally
            {
                ui.timerStatus.Stop();
                ui.stage = 0;
                ui.uiManager.EnabledDupUI(ui);
            }
        }

        public void DoFindWork(DupUI ui)
        {
            string procedure = string.Empty;

            try
            {
                procedure = "Getting duplicate files.";
                dupFileInfoList = new List<DupFileInfo>();
                new DupAlgorithm().GetDupFileInfos(ui.statusInfo, dupFileInfoList, fileDict);
                ui.uiManager.UpdateFindStatus(ui);

                int dupFileListCount = dupFileInfoList.Count;

                if (dupFileListCount > 0)
                {
                    ui.dupFileListCount = dupFileListCount;
                    procedure = "Setting <DataGridView> datasource.";
                    SetDataSource(ui);

                    procedure = "Recording duplicate files.";
                    new Thread(() => logManager.RecordDuplicateFiles(dupFileInfoList)) { IsBackground = true }.Start();
                }
                else MessageBox.Show("No duplicate files found.", "Tips", MessageBoxButtons.OK, MessageBoxIcon.Information);

                procedure = "Clearing file dictionary.";
                fileDict.Clear();
            }
            catch (Exception ex)
            {
                logManager.RecordLogInfo("FIND_WORK_ERROR", ex.Message, procedure);
            }
            finally
            {
                ui.timerStatus.Stop();
                ui.stage = 0;
                ui.uiManager.EnabledDupUI(ui);
            }
        }

        private void SetDataSource(DupUI ui)
        {
            ui.Invoke(new InvokeHandler(delegate ()
            {
                ui.dgvFiles.DataSource = null;
                ui.dgvFiles.DataSource = dupFileInfoList;
                new DgvLayout().SetDgvColumnsWidth(ui.dgvFiles);
            }));
        }

        public void DoDeleteWork(DupUI ui)
        {
            string procedure = string.Empty;

            try
            {
                procedure = "Deleting duplicate files.";
                List<DupFileInfo> remainFileInfoList = new List<DupFileInfo>();
                DeleteDupFiles(ui, remainFileInfoList);
                ui.uiManager.UpdateDeleteStatus(ui);

                procedure = "Updating <DataGridView> datasource.";
                UpdateDataSource(ui, remainFileInfoList);

                procedure = "Clearing duplicate files list.";
                dupFileInfoList.Clear();
                dupFileInfoList.TrimExcess();
            }
            catch (Exception ex)
            {
                logManager.RecordLogInfo("DEL_WORK_ERROR", ex.Message, procedure);
            }
            finally
            {
                ui.timerStatus.Stop();
                ui.stage = 0;
                ui.uiManager.EnabledDupUI(ui);
            }
        }

        private void DeleteDupFiles(DupUI ui, List<DupFileInfo> remainFileInfoList)
        {
            int index = 0;

            foreach (DupFileInfo dupFileInfo in dupFileInfoList)
            {
                if (dupFileInfo.IsDel)
                {
                    try
                    {
                        File.Delete(dupFileInfo.Path);
                    }
                    catch (Exception ex)
                    {
                        logManager.RecordLogInfo("DEL_FILE_ERROR", ex.Message, dupFileInfo.Path);
                    }

                    ui.statusInfo.DupCount--;
                }
                else
                {
                    dupFileInfo.SN = ++index;
                    remainFileInfoList.Add(dupFileInfo);
                }

                ui.statusInfo.ProgressValue++;
            }
        }

        private void UpdateDataSource(DupUI ui, List<DupFileInfo> remainFileInfoList)
        {
            ui.Invoke(new InvokeHandler(delegate ()
            {
                ui.dgvFiles.DataSource = null;

                if (remainFileInfoList.Count > 0)
                {
                    ui.dgvFiles.DataSource = remainFileInfoList;
                    new DgvLayout().SetDgvColumnsWidth(ui.dgvFiles);
                }
            }));
        }
    }
}
