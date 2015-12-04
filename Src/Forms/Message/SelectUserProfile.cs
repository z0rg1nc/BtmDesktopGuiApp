using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;

namespace BtmI2p.BitMoneyClient.Gui.Forms
{
    public partial class SelectProfileForm : Form
    {
        public static SelectUserProfileFormLocStrings LocStrings 
            = new SelectUserProfileFormLocStrings();
        private readonly TaskCompletionSource<FileInfo> _tcs;
        private readonly FileInfo[] _profileFilInfos;
        private readonly string _caption;
        public SelectProfileForm(
            TaskCompletionSource<FileInfo> tcs,
            FileInfo[] profileFileInfos,
            string caption
        )
        {
            _tcs = tcs;
            _profileFilInfos = profileFileInfos;
            InitializeComponent();
            _caption = caption;
            foreach (var fileInfo in profileFileInfos)
            {
                var fileName = fileInfo.Name;
                if (fileName.Length > 7)
                    fileName = fileName.Remove(fileName.Length - 7);
                listBox1.Items.Add(fileName);
            }
        }

        private void SelectUserProfile_FormClosing(
            object sender, 
            FormClosingEventArgs e
        )
        {
            _tcs.TrySetResult(null);
        }

        private void SelectResult()
        {
            int selectedProfileNum = listBox1.SelectedIndex;
            if (selectedProfileNum == -1)
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.Messages.ProfileFineNotSelectedError
                );
                return;
            }
            _tcs.TrySetResult(_profileFilInfos[selectedProfileNum]);
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectResult();
        }

        private void listBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SelectResult();
            }
        }

        private void SelectUserProfileForm_Shown(object sender, EventArgs e)
        {
            InitCommonView();
        }

        private void InitCommonView()
        {
            this.button1.Text = LocStrings.Button1Text;
            this.Text = _caption;
            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }
    }

    public class SelectUserProfileFormLocStrings
    {
        public class LocMessages
        {
            public string ProfileFineNotSelectedError = "Profile file not selected";
        }
        public LocMessages Messages
            = new LocMessages();
        /**/
        public string Button1Text = "OK";
        public string TextInit = "Select user profile";
    }
}
