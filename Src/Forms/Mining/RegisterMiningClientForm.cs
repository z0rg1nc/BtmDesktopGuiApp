using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Mining;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.AesHelper;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.CryptFile.Lib;
using BtmI2p.MiscUtils;
using NLog;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Mining
{
    public partial class RegisterMiningClientForm : Form
    {
        private readonly ProxyServerSessionOverI2P _proxySession;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        public RegisterMiningClientForm(
            ProxyServerSessionOverI2P proxySession
            )
        {
            _proxySession = proxySession;
            InitializeComponent();
            textBox1.Text = string.Format(
                "mining_{0}",
                Guid.NewGuid().ToString().Substring(0, 5)
            );
        }

        private ActionDisposable GetEnabledActionDisposable()
        {
            return new ActionDisposable(
                () =>
                {
                    Cursor = Cursors.WaitCursor;
                },
                () =>
                {
                    Cursor = Cursors.Default;
                }
            );
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var curMethodName = this.MyNameOfMethod(x => x.button1_Click(null, null));
            try
            {
                using (GetEnabledActionDisposable())
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        var onProgressLoadTcs = new TaskCompletionSource<object>();
                        var progressForm = new ProgressCancelForm(
                            cts,
                            onProgressLoadTcs,
                            LocStrings.ProgressFormCaption
                            );
                        progressForm.Show(this);
                        await onProgressLoadTcs.Task;
                        try
                        {
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                LocStrings.ProgressFormReport1,
                                0
                                );
                            var profileName = textBox1.Text;
                            if (string.IsNullOrWhiteSpace(profileName))
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.Messages.EmptyProfileNameError
                                    );
                                return;
                            }
                            var forbiddenSymbols = Path.GetInvalidFileNameChars();
                            if (profileName.Any(forbiddenSymbols.Contains))
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.Messages.ForbiddenCharsError.Inject(
                                        new
                                        {
                                            ForbiddenChars = new string(forbiddenSymbols)
                                        }
                                        )
                                    );
                                return;
                            }
                            DefaultFolders.CreateFoldersIfNotExist();
                            var miningClientProfileFileName
                                = Path.Combine(
                                    DefaultFolders.MiningProfilesFolder,
                                    string.Format("{0}.aes256", profileName)
                                    );
                            if (
                                File.Exists(
                                    miningClientProfileFileName
                                    )
                                )
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.Messages.ProfileAlreadyExistError
                                    );
                                return;
                            }
                            /**/
                            var profileFilePassword = textBox2.Text;
                            if (string.IsNullOrWhiteSpace(profileFilePassword))
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.Messages.EmptyFilePasswordError
                                    );
                                return;
                            }
                            if (textBox2.Text != textBox3.Text)
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.Messages.ProfilePasswordsArentEquaslError
                                    );
                                return;
                            }
                            var miningClientCertPass = textBox5.Text;
                            if (string.IsNullOrWhiteSpace(miningClientCertPass))
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.Messages.EmptyCertPasswordError
                                    );
                                return;
                            }
                            if (textBox5.Text != textBox4.Text)
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.Messages.CertPasswordArentEqualError
                                    );
                                return;
                            }
                            var miningClientCertPassBytes =
                                Encoding.UTF8.GetBytes(miningClientCertPass);
                            /**/
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                LocStrings.ProgressFormReport2,
                                50
                                );
                            var lookupServerSession
                                = await _proxySession.GetLookupServerSession(
                                    cts.Token
                                    );
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                LocStrings.ProgressFormReport3,
                                55
                                );
                            var newMiningClientCert
                                = await MiningServerSession.RegisterMiningClient(
                                    _proxySession,
                                    lookupServerSession,
                                    new AesProtectedByteArray(
                                        new TempByteArray(miningClientCertPassBytes)
                                        ),
                                    cts.Token
                                    );
                            /**/
                            progressForm.ReportProgress(
                                LocStrings.ProgressFormReport4,
                                80
                                );
                            /**/
                            var miningProfile = new MiningClientProfile()
                            {
                                MiningClientCert = newMiningClientCert,
                                ProfileName = profileName
                            };
                            if (!File.Exists(miningClientProfileFileName))
                                File.Create(miningClientProfileFileName).Close();
                            ScryptPassEncryptedData.WriteToFile(
                                miningProfile,
                                miningClientProfileFileName,
                                Encoding.UTF8.GetBytes(profileFilePassword)
                                );
                            /**/
                            var miningClientSettings =
                                new MiningClientSettings();
                            DefaultFolders.CreateFoldersIfNotExist();
                            if (!File.Exists(miningProfile.GetSettingsFilePath()))
                                File.Create(miningProfile.GetSettingsFilePath()).Close();
                            ScryptPassEncryptedData.WriteToFile(
                                miningClientSettings,
                                miningProfile.GetSettingsFilePath(),
                                miningProfile.SettingsPass
                                );
                            /**/
                            progressForm.ReportProgress(
                                LocStrings.ProgressFormReport5,
                                100
                                );
                            ClientGuiMainForm.ShowInfoMessage(this,
                                LocStrings.Messages.RegistrationSuccessInfo
                                );
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        finally
                        {
                            progressForm.SetProgressComplete();
                        }
                    }
                }
                Close();
            }
            catch (Exception exc)
            {
                _log.Error(
                    "{0} unexpected error '{1}'", 
                    curMethodName, 
                    exc.ToString()
                );
                ClientGuiMainForm.ShowErrorMessage(this,
                    ClientGuiMainForm.LocStrings.CommonMessages.UnexpectedErrorMessage.Inject(
                        new
                        {
                            ErrorMessage = exc.Message
                        }
                    )
                );
            }
        }

        private void RegisterMiningClientForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void InitCommonView()
        {
            this.label1.Text = LocStrings.Label1Text;
            this.label3.Text = LocStrings.Label3Text;
            this.label2.Text = LocStrings.Label2Text;
            this.label4.Text = LocStrings.Label4Text;
            this.label5.Text = LocStrings.Label5Text;
            this.button1.Text = LocStrings.Button1Text;
            this.Text = LocStrings.TextInit;
            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }
        public static RegisterMiningClientFormLocStrings LocStrings
            = new RegisterMiningClientFormLocStrings();

        private void RegisterMiningClientForm_Shown(object sender, EventArgs e)
        {
            InitCommonView();
        }
    }

    public class RegisterMiningClientFormLocStrings
    {
        public class LocMessages
        {
            public string EmptyProfileNameError = "Empty mining client name";
            public string ForbiddenCharsError 
                = "Mining client profile name cannot contains next chars:'{ForbiddenChars}'";
            public string ProfileAlreadyExistError
                = "Mining client with such name already exists";
            public string EmptyFilePasswordError
                = "Empty profile file password";
            public string ProfilePasswordsArentEquaslError
                = "Profile file passwords aren't equal";
            public string EmptyCertPasswordError 
                = "Empty mining client cert password";
            public string CertPasswordArentEqualError
                = "Mining client cert passwords aren't equal";
            public string RegistrationSuccessInfo
                = "Registration successful";
        }
        public LocMessages Messages 
            = new LocMessages();
        /**/
        public string ProgressFormCaption = "Mining client registration";
        public string ProgressFormReport1 = "Check input";
        public string ProgressFormReport2 = "Server communication start";
        public string ProgressFormReport3 = "Server communication";
        public string ProgressFormReport4 = "Save settings to file";
        public string ProgressFormReport5 = "Registration complete";
        /**/
        public string Label1Text = "Profile name:";
        public string Label3Text = "Confirm:";
        public string Label2Text = "Profile file password:";
        public string Label4Text = "Confirm:";
        public string Label5Text = "Mining client certificate password:";
        public string Button1Text = "Register";
        public string TextInit = "Register mining client";

    }
}
