using DuplicateFinder.Model;
using DuplicateFinder.Utils.Finder;
using System;
using System.Threading;
using System.Windows.Forms;

namespace DuplicateFinder.WinUI
{
    public partial class DupUI : Form
    {
        private readonly ThreadManager threadManager = new ThreadManager();
        public readonly UIManager uiManager = new UIManager();
        public StatusInfo statusInfo = new StatusInfo();
        public Thread tSearch = null;
        public Thread tFinder = null;
        public Thread tDelete = null;
        public int dupFileListCount = 0;
        public int stage = 0;//0:Empty; 1:Search; 2:Find; 3:Delete

        public DupUI()
        {
            InitializeComponent();
            Application.EnableVisualStyles();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            threadManager.Browse(this);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            threadManager.Delete(this);
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            threadManager.Start(this);
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            threadManager.Abort(this);
        }

        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            threadManager.Refresh(this);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            uiManager.StopProgressBar(this);
        }
    }
}
