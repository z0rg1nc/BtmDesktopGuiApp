using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Forms
{
    public partial class ProgressCancelForm : Form, IProgressCancelForm
    {
        /**/
        private readonly CancellationTokenSource _cts;
        private readonly TaskCompletionSource<object> _onLoadTaskSource;
        private readonly string _caption;
        public ProgressCancelForm(
            CancellationTokenSource cts,
            TaskCompletionSource<object> onLoadTaskSource,
            string caption
        )
        {
            _cts = cts;
            _onLoadTaskSource = onLoadTaskSource;
            _caption = caption;
            InitializeComponent();
        }

        private bool _progressComplete;

        public void SetProgressComplete()
        {
            _progressComplete = true;
            /*Invoke((Action) (() =>
            {
                progressBar1.Value = 100;
            }));*/
            if(!_cts.IsCancellationRequested)
                _cts.Cancel();
            Close();
        }

        private bool _firstReportProgress = false;

        public void ReportProgress(
            string progressText,
            int progressValue
        )
        {
            ReportProgress(progressValue,progressText);
        }

        public void ReportProgress(
            int progressValue,
            string progressText
        )
        {
            if(_progressComplete)
                return;
            BeginInvoke((Action) (() =>
            {
                textBox1.AppendText(
                    string.Format(
                        "{2}{0:T}: {1}",
                        DateTime.UtcNow,
                        progressText,
                        _firstReportProgress ? Environment.NewLine : ""
                        )
                    );
                if (!_firstReportProgress)
                    _firstReportProgress = true;
                if (progressValue != -1)
                    progressBar1.Value = progressValue;
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _cts.Cancel();
        }

        private void ProgressCancelForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_progressComplete)
                e.Cancel = true;
        }

        private void ProgressCancelForm_Shown(object sender, EventArgs e)
        {
            InitCommonView();
            Text = _caption;
            _onLoadTaskSource.TrySetResult(null);
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void InitCommonView()
        {
            this.button1.Text = LocStrings.Button1Text;
            this.Text = LocStrings.TextInit;
            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }
        public static ProgressCancelFormLocStrings LocStrings
            = new ProgressCancelFormLocStrings();
    }

    public interface IProgressCancelForm
    {
        void SetProgressComplete();
        void ReportProgress(
            int progressValue,
            string progressText
        );
        void ReportProgress(
            string progressText,
            int progressValue
        );
    }

    public class ProgressCancelFormLocStrings
    {
        public string Button1Text = "Cancel";
        public string TextInit = "Caption";
    }
    public class ProgressCancelFormWraper : IDisposable
    {
        private ProgressCancelFormWraper()
        {
        }
        private readonly CancellationTokenSource _cts 
            = new CancellationTokenSource();
        public CancellationToken Token => _cts.Token;
        private ProgressCancelForm _progressForm;
        public IProgressCancelForm ProgressInst => _progressForm;

        public static async Task<ProgressCancelFormWraper> CreateInstance(
            string caption,
            Form owner
        )
        {
            Assert.NotNull(caption);
            var result = new ProgressCancelFormWraper();
            var tcs = new TaskCompletionSource<object>();
			result._progressForm = new ProgressCancelForm(
                result._cts,
                tcs,
                caption
            );
            result._progressForm.Show(owner);
            await tcs.Task;
            return result;
        }

        public void Dispose()
        {
            _progressForm.SetProgressComplete();
            _cts.Dispose();
        }
    }
        
}
