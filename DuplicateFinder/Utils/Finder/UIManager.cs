using DuplicateFinder.Model;
using DuplicateFinder.WinUI;
using System.Windows.Forms;

namespace DuplicateFinder.Utils.Finder
{
    public class UIManager
    {
        public void StartProgressBar(DupUI ui)
        {
            ui.timer.Start();
            ui.progressBar.MarqueeAnimationSpeed = 50;
            ui.progressBar.Style = ProgressBarStyle.Marquee;
        }

        public void StopProgressBar(DupUI ui)
        {
            if (ui.stage != 1)
            {
                ui.timer.Stop();
                ui.progressBar.Style = ProgressBarStyle.Blocks;
            }
        }

        public void InitSearchProgress(DupUI ui)
        {
            ui.lblStatus.Text = "Search files.";
            ui.lblTotalFiles.Text = "0";
            ui.lblDuplicateFiles.Text = "0";
            ui.statusInfo = new StatusInfo();
            ui.dgvFiles.DataSource = null;
            ui.stage = 1;
        }

        public void UpdateSearchStatus(DupUI ui)
        {
            ui.lblTotalFiles.Text = ui.statusInfo.TotalCount.ToString();
        }

        public void InitFindProgress(DupUI ui)
        {
            ui.lblStatus.Text = "Get duplicate files.";
            ui.progressBar.Style = ProgressBarStyle.Blocks;
            ui.progressBar.Minimum = 0;
            ui.progressBar.Maximum = ui.statusInfo.TotalCount;
            ui.progressBar.Value = 0;
            ui.stage = 2;
        }

        public void UpdateFindStatus(DupUI ui)
        {
            ui.progressBar.Value = ui.statusInfo.ProgressValue;
            ui.lblDuplicateFiles.Text = ui.statusInfo.DupCount.ToString();
        }

        public void InitDeleteProgress(DupUI ui)
        {
            ui.lblStatus.Text = "Delete duplicate files.";
            ui.progressBar.Style = ProgressBarStyle.Blocks;
            ui.progressBar.Minimum = 0;
            ui.progressBar.Maximum = ui.dupFileListCount;
            ui.progressBar.Value = 0;
            ui.statusInfo.ProgressValue = 0;
            ui.stage = 3;
        }

        public void UpdateDeleteStatus(DupUI ui)
        {
            ui.progressBar.Value = ui.statusInfo.ProgressValue;
            ui.lblDuplicateFiles.Text = ui.statusInfo.DupCount.ToString();
        }

        public void UpdateAbortStatus(DupUI ui)
        {
            ui.progressBar.Style = ProgressBarStyle.Blocks;
            ui.progressBar.Value = ui.progressBar.Maximum;
            ui.lblStatus.Text = "Task abort.";
            ui.lblTotalFiles.Text = ui.statusInfo.TotalCount.ToString();
            ui.lblDuplicateFiles.Text = ui.statusInfo.DupCount.ToString();
        }

        public void DisabledDupUI(DupUI ui)
        {
            ui.btnBrowse.Enabled = false;
            ui.btnDelete.Enabled = false;
            ui.btnStart.Enabled = false;
        }

        public void EnabledDupUI(DupUI ui)
        {
            ui.btnBrowse.Enabled = true;
            ui.btnDelete.Enabled = true;
            ui.btnStart.Enabled = true;
        }
    }
}
