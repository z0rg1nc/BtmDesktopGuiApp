using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Wallet;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.AesHelper;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.BitMoneyClient.Lib.WalletServerSession;
using BtmI2p.CryptFile.Lib;
using BtmI2p.GeneralClientInterfaces.WalletServer;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;
using BtmI2p.ObjectStateLib;
using NLog;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Wallet
{
    public partial class RegisterWalletForm : Form
    {
        private readonly ProxyServerSessionOverI2P _proxySession;
        public RegisterWalletForm(ProxyServerSessionOverI2P proxySession)
        {
            _proxySession = proxySession;
            InitializeComponent();
            _stateHelper.SetInitializedState();
            textBox7.Text = string.Format(
                "wallet_{0}", 
                Guid.NewGuid().ToString().Substring(0, 5)
            );
        }
        private ActionDisposable GetCursorWaitDisposable()
        {
            return new ActionDisposable(
                () => 
                    BeginInvoke(
                        new Action(
                            () =>
                            {
                                Cursor = Cursors.WaitCursor; 
                            }                 
                        )
                    ), 
                () => 
                    BeginInvoke(
                        new Action(
                            () =>
                            {
                                Cursor = Cursors.Default;
                            }
                        )
                    )
            );
        }

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly DisposableObjectStateHelper _stateHelper
            = new DisposableObjectStateHelper("RegisterWalletForm");

        private async void RegisterWalletForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            await _stateHelper.MyDisposeAsync();
        }

        private void RegisterWalletForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (_stateHelper.GetFuncWrapper())
                {
                    bool close = false;
                    using (GetCursorWaitDisposable())
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
                                var profileName = textBox7.Text;
                                if (string.IsNullOrWhiteSpace(profileName))
                                {
                                    ClientGuiMainForm.ShowErrorMessage(this,
                                        LocStrings.Messages.EmptyWalletNameError
                                        );
                                    return;
                                }
                                var restrictedSymbols = Path.GetInvalidFileNameChars();
                                if (profileName.Any(restrictedSymbols.Contains))
                                {
                                    ClientGuiMainForm.ShowErrorMessage(this,
                                        LocStrings.Messages.WalletNameForbidderCharsError.Inject(
                                            new
                                            {
                                                ForbiddenChars = new string(restrictedSymbols)
                                            }
                                            )
                                        );
                                    return;
                                }
                                DefaultFolders.CreateFoldersIfNotExist();
                                var walletProfileFileName =
                                    Path.Combine(
                                        DefaultFolders.WalletProfilesFolder,
                                        string.Format("{0}.aes256", profileName)
                                        );
                                if (
                                    File.Exists(
                                        walletProfileFileName
                                        )
                                    )
                                {
                                    ClientGuiMainForm.ShowErrorMessage(this,
                                        LocStrings.Messages.WalletNameAlreadyExistError
                                        );
                                    return;
                                }
                                /**/
                                if (string.IsNullOrWhiteSpace(textBox1.Text))
                                {
                                    ClientGuiMainForm.ShowErrorMessage(this,
                                        LocStrings.Messages.EmptyProfileFilePasswordError
                                        );
                                    return;
                                }
                                if (textBox1.Text != textBox2.Text)
                                {
                                    ClientGuiMainForm.ShowErrorMessage(this,
                                        LocStrings.Messages.ProfileFilePasswordsArentEqualError
                                        );
                                    return;
                                }
                                /**/
                                if (string.IsNullOrWhiteSpace(textBox4.Text))
                                {
                                    ClientGuiMainForm.ShowErrorMessage(this,
                                        LocStrings.Messages.EmptyCertPasswordError
                                        );
                                    return;
                                }
                                if (textBox4.Text != textBox3.Text)
                                {
                                    ClientGuiMainForm.ShowErrorMessage(this,
                                        LocStrings.Messages.CertPasswordsArentEqualError
                                        );
                                    return;
                                }
                                /**/
                                if (string.IsNullOrWhiteSpace(textBox6.Text))
                                {
                                    ClientGuiMainForm.ShowErrorMessage(this,
                                        LocStrings.Messages.EmptyMasterWalletCertPasswordError
                                        );
                                    return;
                                }
                                if (textBox6.Text != textBox5.Text)
                                {
                                    ClientGuiMainForm.ShowErrorMessage(this,
                                        LocStrings.Messages.MasterWalletCertPasswordsArentEqualError
                                        );
                                    return;
                                }
                                var walletProfileFilePasswordBytes =
                                    Encoding.UTF8.GetBytes(textBox1.Text);
                                var walletCertPasswordBytes =
                                    Encoding.UTF8.GetBytes(textBox4.Text);
                                var masterWalletCertPasswordBytes =
                                    Encoding.UTF8.GetBytes(textBox6.Text);
                                /**/
                                cts.Token.ThrowIfCancellationRequested();
                                progressForm.ReportProgress(
                                    LocStrings.ProgressFormReport2,
                                    50
                                    );
                                var lookupServerSession =
                                    await _proxySession.GetLookupServerSession(
                                        cts.Token
                                        );
                                var registerResult = await WalletServerSession.RegisterWallet(
                                    _proxySession,
                                    lookupServerSession,
                                    new AesProtectedByteArray(
                                        new TempByteArray(
                                            walletCertPasswordBytes
                                            )
                                        ),
                                    new AesProtectedByteArray(
                                        new TempByteArray(
                                            masterWalletCertPasswordBytes
                                            )
                                        ),
                                    new WalletClientSettingsOnServer(),
                                    cts.Token
                                    );
                                progressForm.ReportProgress(
                                    LocStrings.ProgressFormReport3,
                                    90
                                    );
                                var walletProfile = new WalletProfile()
                                {
                                    ProfileName = profileName,
                                    WalletCert = registerResult.WalletCert,
                                    MasterWalletCert = registerResult.MasterWalletCert
                                };
                                ScryptPassEncryptedData.WriteToFile(
                                    walletProfile,
                                    walletProfileFileName,
                                    walletProfileFilePasswordBytes
                                    );
                                var walletSettings = MyNotifyPropertyChangedImpl.GetProxy(
                                    (IWalletSettings) new WalletSettings()
                                    );
                                ScryptPassEncryptedData.WriteToFile(
                                    walletSettings,
                                    walletProfile.GetSettingsFilePath(),
                                    walletProfile.SettingsPass
                                    );
                                progressForm.ReportProgress(
                                    LocStrings.ProgressFormReportFinish,
                                    100
                                    );
                                ClientGuiMainForm.ShowInfoMessage(
                                    this,
                                    LocStrings.Messages.WalletRegisteredInfo,
                                    LocStrings.Messages.WalletRegisteredInfoCaption
                                    );
                                close = true;
                            }
                            finally
                            {
                                progressForm.SetProgressComplete();
                            }
                        }
                    }
                    if(close)
                        Close();
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (Exception  exc)
            {
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
        public static RegisterWalletFormLocStrings LocStrings
            = new RegisterWalletFormLocStrings();

        private void InitCommonView()
        {
            this.label8.Text = LocStrings.Label8Text;
            this.label5.Text = LocStrings.Label5Text;
            this.label6.Text = LocStrings.Label6Text;
            this.label3.Text = LocStrings.Label3Text;
            this.label4.Text = LocStrings.Label4Text;
            this.label2.Text = LocStrings.Label2Text;
            this.label1.Text = LocStrings.Label1Text;
            this.button1.Text = LocStrings.Button1Text;
            this.Text = LocStrings.TextInit;

            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }

        private void RegisterWalletForm_Shown(object sender, EventArgs e)
        {
            InitCommonView();
        }
    }

    public class RegisterWalletFormLocStrings
    {
        public class MessagesLocStrings
        {
            public string EmptyWalletNameError = "Empty wallet name";
            public string WalletNameForbidderCharsError
                = "Wallet profile name cannot contains next chars:'{ForbiddenChars}'";
            public string WalletNameAlreadyExistError
                = "Wallet with such name already exists";
            public string EmptyProfileFilePasswordError 
                = "Empty profile file password";
            public string ProfileFilePasswordsArentEqualError
                = "Profile file passwords aren't equal";
            public string EmptyCertPasswordError
                = "Empty wallet cert password";
            public string CertPasswordsArentEqualError
                = "Wallet cert passwords aren't equal";
            public string EmptyMasterWalletCertPasswordError
                = "Empty master wallet cert password";
            public string MasterWalletCertPasswordsArentEqualError
                = "Master wallet cert passwords aren't equal";
            public string WalletRegisteredInfo
                = "Wallet registered";
            public string WalletRegisteredInfoCaption
                = "Success";
        }
        public MessagesLocStrings Messages = new MessagesLocStrings();
        /**/
        public string ProgressFormCaption = "Wallet registration";
        public string ProgressFormReport1 = "User input";
        public string ProgressFormReport2 = "Wallet server communication";
        public string ProgressFormReport3 = "Saving profile settings to file";
        public string ProgressFormReportFinish = "Finish";
        /**/
        public string Label8Text = "Wallet name:";
        public string Label5Text = "Confirm:";
        public string Label6Text = "Master wallet cert password:";
        public string Label3Text = "Confirm:";
        public string Label4Text = "Wallet cert password:";
        public string Label2Text = "Confirm:";
        public string Label1Text = "Profile file password:";
        public string Button1Text = "Register";
        public string TextInit = "Register Wallet";
    }
}
