using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Exchange;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.AesHelper;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.BitMoneyClient.Lib.ExchangeServerSession;
using BtmI2p.CryptFile.Lib;
using BtmI2p.MiscUtils;
using NLog;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
    public partial class RegisterExchangeClientForm : Form
    {
        private readonly IProxyServerSession _proxySession;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public RegisterExchangeClientForm(
            IProxyServerSession proxySession)
        {
            _proxySession = proxySession;
            InitializeComponent();
            profileNameTextBox.Text = string.Format(
                "exchange_{0}",
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

        private async void registerButton_Click(object sender, EventArgs e)
        {
            var curMethodName = this.MyNameOfMethod(x => x.registerButton_Click(null, null));
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
                            var profileName = profileNameTextBox.Text;
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
                            var exchangeClientProfileFileName
                                = Path.Combine(
                                    DefaultFolders.ExchangeProfilesFolder,
                                    string.Format("{0}.aes256", profileName)
                                    );
                            if (
                                File.Exists(
                                    exchangeClientProfileFileName
                                    )
                                )
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.Messages.ProfileAlreadyExistError
                                    );
                                return;
                            }
                            /**/
                            var profileFilePassword = profilePasswordTextBox1.Text;
                            if (string.IsNullOrWhiteSpace(profileFilePassword))
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.Messages.EmptyFilePasswordError
                                    );
                                return;
                            }
                            if (profilePasswordTextBox1.Text
                                != profilePasswordTextBox2.Text)
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.Messages.ProfilePasswordsArentEquaslError
                                    );
                                return;
                            }
                            var exchangeClientCertPass = certPasswordTextbox1.Text;
                            if (string.IsNullOrWhiteSpace(exchangeClientCertPass))
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.Messages.EmptyCertPasswordError
                                    );
                                return;
                            }
                            if (certPasswordTextbox1.Text
                                != certPasswordTextbox2.Text)
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.Messages.CertPasswordArentEqualError
                                    );
                                return;
                            }
                            var exchangeClientCertPassBytes =
                                Encoding.UTF8.GetBytes(exchangeClientCertPass);
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
                            var newExchangeClientCert
                                = await ExchangeServerSession.RegisterExchangeClient(
                                    _proxySession,
                                    lookupServerSession,
                                    new AesProtectedByteArray(
                                        new TempByteArray(exchangeClientCertPassBytes)
                                        ),
                                    cts.Token
                                    );
                            /**/
                            progressForm.ReportProgress(
                                LocStrings.ProgressFormReport4,
                                80
                                );
                            /**/
                            var exchangeProfile = new ExchangeClientProfile()
                            {
                                ExchangeClientCert = newExchangeClientCert,
                                ProfileName = profileName
                            };
                            if (!File.Exists(exchangeClientProfileFileName))
                                File.Create(exchangeClientProfileFileName).Close();
                            ScryptPassEncryptedData.WriteToFile(
                                exchangeProfile,
                                exchangeClientProfileFileName,
                                Encoding.UTF8.GetBytes(profileFilePassword)
                                );
                            /**/
                            var exchangeClientSettings =
                                new ExchangeClientSettings();
                            DefaultFolders.CreateFoldersIfNotExist();
                            if (!File.Exists(exchangeProfile.GetSettingsFilePath()))
                                File.Create(exchangeProfile.GetSettingsFilePath()).Close();
                            ScryptPassEncryptedData.WriteToFile(
                                exchangeClientSettings,
                                exchangeProfile.GetSettingsFilePath(),
                                exchangeProfile.SettingsPass
                                );
                            /**/
                            progressForm.ReportProgress(
                                LocStrings.ProgressFormReport5,
                                100
                                );
                            ClientGuiMainForm.ShowInfoMessage(
                                this,
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
        public static RegisterExchangeClientFormLocStrings LocStrings
            = new RegisterExchangeClientFormLocStrings();
        public static RegisterExchangeClientFormDesignerLocStrings DesignerLocStrings = new RegisterExchangeClientFormDesignerLocStrings();
        private void InitCommonView()
        {
            this.confirmCertPasswordLabel.Text = DesignerLocStrings.ConfirmCertPasswordLabelText;
            this.confirmFilePasswordLabel.Text = DesignerLocStrings.ConfirmFilePasswordLabelText;
            this.registerButton.Text = DesignerLocStrings.RegisterButtonText;
            this.profileNameLabel.Text = DesignerLocStrings.ProfileNameLabelText;
            this.profileFilePasswordLabel.Text = DesignerLocStrings.ProfileFilePasswordLabelText;
            this.certPasswordLabel.Text = DesignerLocStrings.CertPasswordLabelText;
            this.Text = DesignerLocStrings.Text;
            ClientGuiMainForm.ChangeControlFont(this, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
        }

        private void RegisterExchangeClientForm_Shown(object sender, EventArgs e)
        {
            InitCommonView();
        }
    }

    public class RegisterExchangeClientFormLocStrings
    {
        public class LocMessages
        {
            public string EmptyProfileNameError = "Empty exchange client name";
            public string ForbiddenCharsError
                = "Exchange client profile name cannot contains next chars:'{ForbiddenChars}'";
            public string ProfileAlreadyExistError
                = "Exchange client with such name already exists";
            public string EmptyFilePasswordError
                = "Empty profile file password";
            public string ProfilePasswordsArentEquaslError
                = "Profile file passwords aren't equal";
            public string EmptyCertPasswordError
                = "Empty exchange client cert password";
            public string CertPasswordArentEqualError
                = "Exchange client cert passwords aren't equal";
            public string RegistrationSuccessInfo
                = "Registration successful";
        }
        public LocMessages Messages
            = new LocMessages();
        /**/
        /**/
        public string ProgressFormCaption = "Exchange client registration";
        public string ProgressFormReport1 = "Check input";
        public string ProgressFormReport2 = "Server communication start";
        public string ProgressFormReport3 = "Server communication";
        public string ProgressFormReport4 = "Save settings to file";
        public string ProgressFormReport5 = "Registration complete";
    }
    public class RegisterExchangeClientFormDesignerLocStrings
    {
        public string ConfirmCertPasswordLabelText = "Confirm:";
        public string ConfirmFilePasswordLabelText = "Confirm:";
        public string RegisterButtonText = "Register";
        public string ProfileNameLabelText = "Profile name";
        public string ProfileFilePasswordLabelText = "Profile password";
        public string CertPasswordLabelText = "Certificate password";
        public string Text = "Register exchange client";
    }
}
