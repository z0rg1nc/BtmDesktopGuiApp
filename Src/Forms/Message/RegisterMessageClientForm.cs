using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Message;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.AesHelper;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.BitMoneyClient.Lib.MessageServerSession;
using BtmI2p.CryptFile.Lib;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;
using BtmI2p.ObjectStateLib;
using NLog;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Message
{
    public partial class RegisterMessageClientForm : Form
    {
        public static RegisterUserFormLocStrings LocStrings 
            = new RegisterUserFormLocStrings();
        private readonly ProxyServerSessionOverI2P _proxySession;
        private readonly DisposableObjectStateHelper _stateHelper
            = new DisposableObjectStateHelper("RegisterMessageClientForm");
        public RegisterMessageClientForm(ProxyServerSessionOverI2P proxySession)
        {
            _proxySession = proxySession;
            InitializeComponent();
            _stateHelper.SetInitializedState();
            textBox7.Text = $"message_{Guid.NewGuid().ToString().Substring(0, 5)}";
        }
        private ActionDisposable GetCursorWaitDisposable()
        {
            return new ActionDisposable(
                () =>
                    Invoke(
                        new Action(
                            () =>
                            {
                                Cursor = Cursors.WaitCursor;
                            }
                        )
                    ),
                () =>
                    Invoke(
                        new Action(
                            () =>
                            {
                                Cursor = Cursors.Default;
                            }
                        )
                    )
            );
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (_stateHelper.GetFuncWrapper())
                {
                    bool close = false;
                    using (GetCursorWaitDisposable())
                    {
                        using (var wrp = await ProgressCancelFormWraper.CreateInstance(LocStrings.ProgressFormCaption, this))
                        { 
                            wrp.Token.ThrowIfCancellationRequested();
                            wrp.ProgressInst.ReportProgress(
                                LocStrings.ProgressFormParseParameteres,
                                0
                            );
                            var profileName = textBox7.Text;
                            if (string.IsNullOrWhiteSpace(profileName))
                            {
                                ClientGuiMainForm.ShowErrorMessage(
                                    this, LocStrings.EmptyProfileNameErrorMessage
                                );
                                return;
                            }
                            var forbiddenNameChars = Path.GetInvalidFileNameChars();
                            if (profileName.Any(forbiddenNameChars.Contains))
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.ProfileNameForbiddenCharsErrorMessage.Inject(
                                        new
                                        {
                                            ForbiddenCharsString = new string(forbiddenNameChars)
                                        }
                                    )
                                );
                                return;
                            }
                            DefaultFolders.CreateFoldersIfNotExist();
                            string userProfilesDirName =
                                DefaultFolders.MessageProfilesFolder;
                            var userProfileFilename =
                                Path.Combine(
                                    userProfilesDirName, 
                                    string.Format("{0}.aes256", profileName)
                                );
                            if (
                                File.Exists(
                                    userProfileFilename
                                )
                            )
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.ProfileExistErrorMessage
                                );
                                return;
                            }
                            /**/
                            var userProfileFilePassword = textBox1.Text;
                            if (
                                string.IsNullOrWhiteSpace(
                                    userProfileFilePassword
                                )
                            )
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.EmptyFilePasswordErrorMessage
                                );
                                return;
                            }
                            if (textBox1.Text != textBox2.Text)
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.FilePasswordDontMatchErrorMessage
                                );
                                return;
                            }
                            var userProfileFilePasswordBytes
                                = Encoding.UTF8.GetBytes(
                                    userProfileFilePassword
                                );
                            /**/
                            var userCertPassword = textBox4.Text;
                            if (string.IsNullOrWhiteSpace(userCertPassword))
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.EmptyCertPasswordErrorMessage
                                );
                                return;
                            }
                            if (textBox4.Text != textBox3.Text)
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.CertPasswordDontMatchErrorMessage
                                );
                                return;
                            }
                            var userCertPasswordBytes =
                                Encoding.UTF8.GetBytes(userCertPassword);
                            /**/
                            var masterUserCertPassword = textBox6.Text;
                            if (string.IsNullOrWhiteSpace(masterUserCertPassword))
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.EmptyMasterCertPasswordErrorMessage
                                );
                                return;
                            }
                            if (textBox6.Text != textBox5.Text)
                            {
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.MasterCertPasswordDontMatchErrorMessage 
                                );
                                return;
                            }
                            var masterUserCertPasswordBytes =
                                Encoding.UTF8.GetBytes(masterUserCertPassword);
                            /**/
                            wrp.Token.ThrowIfCancellationRequested();
                            wrp.ProgressInst.ReportProgress(
                                LocStrings.ProgressFormServerCommunication,
                                50
                            );
                            MessageServerSession.RegisterUserResult registerResult;
                            try
                            {
                                var lookupServerSession =
                                    await _proxySession
                                        .GetLookupServerSession(
                                            wrp.Token
                                        );
                                registerResult = await MessageServerSession.RegisterUser(
                                    _proxySession,
                                    lookupServerSession,
                                    new AesProtectedByteArray(
                                        new TempByteArray(
                                            userCertPasswordBytes
                                        )
                                    ),
                                    new AesProtectedByteArray(
                                        new TempByteArray(
                                            masterUserCertPasswordBytes
                                        )
                                    ),
                                    wrp.Token
                                );
                            }
                            catch (Exception exc)
                            {
                                MiscFuncs.HandleUnexpectedError(exc,_log);
                                ClientGuiMainForm.ShowErrorMessage(this,
                                    LocStrings.RegistrationErrorMessage.Inject(
                                        new
                                        {
                                            ErrorMessage = exc.Message
                                        }
                                    )
                                );
                                return;
                            }
                            /**/
                            wrp.ProgressInst.ReportProgress(
                                LocStrings.ProgressFormSavingProfile,
                                90
                            );
                            var userProfile = new MessageClientProfile()
                            {
                                ProfileName = profileName,
                                UserCert = registerResult.UserCert,
                                MasterUserCert = registerResult.MasterUserCert
                            };
                            ScryptPassEncryptedData.WriteToFile(
                                userProfile,
                                userProfileFilename,
                                userProfileFilePasswordBytes
                            );
                            var userSettings = MyNotifyPropertyChangedImpl.GetProxy(
                                (IMessageClientSettings)new MessageClientSettings()
                            );
                            ScryptPassEncryptedData.WriteToFile(
                                userSettings,
                                userProfile.GetSettingsFilePath(),
                                userProfile.SettingsPass
                            );
                            wrp.ProgressInst.ReportProgress(LocStrings.ProgressFormFinish, 100);
                            ClientGuiMainForm.ShowInfoMessage(this,
                                LocStrings.RegistrationSuccessInfoMessage
                            );
                            close = true;
                        }
                    }
                    if (close)
                        Close();
                }
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (OperationCanceledException)
            {
                
            }
            catch (Exception exc)
            {
                MiscFuncs.HandleUnexpectedError(exc,_log);
                ClientGuiMainForm.ShowErrorMessage(this,
                    ClientGuiMainForm.LocStrings.CommonMessages.UnexpectedErrorMessage.Inject(
                        new{
                            ErrorMessage = exc.Message
                        }
                    )
                );
            }
        }

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private async void RegisterUserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            await _stateHelper.MyDisposeAsync();
        }

        private void RegisterUserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void RegisterUserForm_Shown(object sender, EventArgs e)
        {
            InitCommonView();
        }

        private void InitCommonView()
        {
            this.label8.Text = LocStrings.Label8Text;
            this.label1.Text = LocStrings.Label1Text;
            this.label3.Text = LocStrings.Label3Text;
            this.label2.Text = LocStrings.Label2Text;
            this.label4.Text = LocStrings.Label4Text;
            this.button1.Text = LocStrings.Button1Text;
            this.label5.Text = LocStrings.Label5Text;
            this.label6.Text = LocStrings.Label6Text;
            this.Text = LocStrings.TextInit;
            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }
    }

    public class RegisterUserFormLocStrings
    {
        public string ProgressFormCaption = "User registration";
        public string ProgressFormParseParameteres = "Parse parameters";
        public string EmptyProfileNameErrorMessage = "Empty message user profile name";
        public string ProfileNameForbiddenCharsErrorMessage 
            = "Message user profile name cannot contains next chars:'{ForbiddenCharsString}'";
        public string ProfileExistErrorMessage 
            = "Profile with such name already exists";
        public string EmptyFilePasswordErrorMessage = "Empty profile file password";
        public string FilePasswordDontMatchErrorMessage = "Profile file passwords don't match";
        public string EmptyCertPasswordErrorMessage = "Empty user cert password";
        public string CertPasswordDontMatchErrorMessage = "User cert passwords don't match";
        public string EmptyMasterCertPasswordErrorMessage = "Empty master user cert password";
        public string MasterCertPasswordDontMatchErrorMessage 
            = "Master user cert passwords don't match";
        public string ProgressFormServerCommunication = "Server communication";
        public string RegistrationErrorMessage = "Registration error '{ErrorMessage}'";
        public string ProgressFormSavingProfile = "Saving profile to file";
        public string ProgressFormFinish = "Finish";
        public string RegistrationSuccessInfoMessage = "Registration successful";
        /**/
        public string Label8Text = "Profile name:";
        public string Label1Text = "Profile file password:";
        public string Label3Text = "Confirm:";
        public string Label2Text = "Confirm:";
        public string Label4Text = "User cert password:";
        public string Button1Text = "Register";
        public string Label5Text = "Confirm:";
        public string Label6Text = "Master user cert password:";
        public string TextInit = "Register user";
    }
}
