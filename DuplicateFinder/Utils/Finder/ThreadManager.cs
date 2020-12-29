using DuplicateFinder.WinUI;
using System.Threading;
using System.Windows.Forms;

namespace DuplicateFinder.Utils.Finder
{
    public class ThreadManager
    {
        private readonly JobWorker jobWorker = new JobWorker();
        private bool isFindReady = false;
        private bool isDeleteReady = false;

        public void Browse(DupUI ui)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "Select a folder."
            };

            string path = string.Empty;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                path = folderBrowserDialog.SelectedPath;
                ui.tbxPath.Text = path;
            }

            if (!string.IsNullOrEmpty(path))
            {
                isFindReady = true;
                ui.uiManager.InitSearchProgress(ui);
                ui.uiManager.DisabledDupUI(ui);
                ui.uiManager.StartProgressBar(ui);
                ui.timerStatus.Start();

                ui.tSearch = new Thread(() => jobWorker.DoSearchWork(ui, path))
                {
                    Priority = ThreadPriority.Highest,
                    IsBackground = true
                };

                ui.tSearch.Start();
            }
        }

        public void Start(DupUI ui)
        {
            if (isFindReady)
            {
                if (ui.statusInfo.TotalCount > 0)
                {
                    isFindReady = false;
                    isDeleteReady = true;
                    ui.uiManager.InitFindProgress(ui);
                    ui.uiManager.DisabledDupUI(ui);
                    ui.timerStatus.Start();

                    ui.tFinder = new Thread(() => jobWorker.DoFindWork(ui))
                    {
                        Priority = ThreadPriority.Highest,
                        IsBackground = true
                    };

                    ui.tFinder.Start();
                }
                else MessageBox.Show("No files in this directory and its subdirectory.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else MessageBox.Show("Please select/reselect a folder at first.", "Tips", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void Abort(DupUI ui)
        {
            if (ui.stage != 0)
            {
                try
                {
                    if (ui.stage == 1 && ui.tSearch != null) ui.tSearch.Abort();
                    else if (ui.stage == 2 && ui.tFinder != null) ui.tFinder.Abort();
                    else if (ui.stage == 3 && ui.tDelete != null) ui.tDelete.Abort();
                }
                catch
                {
                    //
                }
                finally
                {
                    ui.timerStatus.Stop();
                    ui.stage = 0;
                    ui.uiManager.UpdateAbortStatus(ui);
                    ui.uiManager.EnabledDupUI(ui);
                }
            }
        }

        public void Delete(DupUI ui)
        {
            if (isDeleteReady)
            {
                if (ui.statusInfo.DupCount > 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Unrecoverable, are you sure to delete?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        isDeleteReady = false;
                        ui.uiManager.InitDeleteProgress(ui);
                        ui.uiManager.DisabledDupUI(ui);
                        ui.timerStatus.Start();

                        ui.tDelete = new Thread(() => jobWorker.DoDeleteWork(ui))
                        {
                            Priority = ThreadPriority.Highest,
                            IsBackground = true
                        };

                        ui.tDelete.Start();
                    }
                }
                else MessageBox.Show("Please find duplicate files at first.", "Tips", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else MessageBox.Show("Please select/reselect a folder at first.", "Tips", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void Refresh(DupUI ui)
        {
            switch (ui.stage)
            {
                case 1:
                    ui.uiManager.UpdateSearchStatus(ui);
                    break;
                case 2:
                    ui.uiManager.UpdateFindStatus(ui);
                    break;
                case 3:
                    ui.uiManager.UpdateDeleteStatus(ui);
                    break;
                default:
                    break;
            }
        }
    }
}
