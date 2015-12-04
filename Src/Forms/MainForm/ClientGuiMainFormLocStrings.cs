using System;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainFormLocStrings
    {
        public class CommonMessagesLocStrings
        {
            public string UnexpectedErrorMessage = "Unexpected error '{ErrorMessage}'";
            public string UnexpedterErrorIdMessage = "{Id} unexpected error: '{ErrorMessage}'";
            public string ProxyServerIsNotConnectedError = "Proxy server is not connected";
            public string UpdateClientFirstError = "Update client first";
            public string NotConnecterYetError = "Not connected yet";
            public string NotLoggedYetError = "Not logged yet";
            public string NoProfileFilesFoundError = "No profile files found";
            public string NoProfileFileSelectedError = "No profile file selected";
            public string EmptyPasswordError = "Empty password";
            public string WrongPasswordError = "Wrong password";
            public string PasswordChangedInfo = "Password changed";
            public string LogoutFirstError = "Logout first";
            public string NotEnoughFundsError = "Not enough funds";
            public string FolderNotFoundError = "Folder not found";
            public string FileNotFound = "File not found";
        }
        public CommonMessagesLocStrings CommonMessages
            = new CommonMessagesLocStrings();
        /**/

        public class CommonTextLocStrings
        {
            public string Profile = "Profile";
            public string Certificate = "Certificate";
            public string MasterCertificate = "Master certificate";
            public string EnterProfileFilePasswordRequest = "Enter profile file password";
            public string EnterCertPasswordRequest = "Enter certificate password";
            public string EnterMasterCertPasswordRequest = "Enter master certificate password";
            public string Start = "Start";
            public string Finish = "Finish";
            public string Logged = "Logged";
            public string Connecting = "Connecting";
            public string Disconnecting = "Disconnecting";
        }
        public CommonTextLocStrings CommonText
            = new CommonTextLocStrings();
        /**/
        public class MainFormMessagesLocStrings
        {
            public string InformationMessageCaption = "Information";
            public string ErrorMessageCaption = "Error";
            public string ReadingPublicCommonSettingsFromFileError 
                = "Error reading PublicCommonSettings from file";
            public string PublicCommonSettingsCheckError 
                = "Public common settings check eror: '{ErrorMessage}'";
            public string LoadingLanguagePackError
                = "Loading language pack error '{ErrorMessage}'";
            public string PrivateCommonSettingsPasswordRequestCaption
                = "Enter private common settings password";
            public string WrongPrivateCommonSettingsPasswordError
                = "Wrong private common settings password";
            public string PrivateSettingsFileParseError
                = "Private settings file parse error {ErrorMessage}";
            public string PrivateSettingsDecryptError
                = "Private settings decrypt error {ErrorMessage}";

            public string PrivateSettingsFileNotFoundNewCreatedInfo
                = "Private common settings file not found, new will be created." +
                  Environment.NewLine +
                  " Please enter the new password for the file.";
            public string NewPrivateSettingsFilePasswordRequestCaption
                = "Enter the new password for the private settings file";
            public string NewPrivateSettingsFilePasswordIsEmptyError
                = "New password is empty. Try again?";
            public string PrivateCommonSettingsCheckError
                = "Private common settings check error '{ErrorMessage}'";
            public string CreateProxySessionError
                = "Create proxy session error {ErrorMessage}";
            public string MainFormInitializationError
                = "Main form initialization unexpected error '{ErrorMessage}'";
            public string AreYouSureQuestion
                = "Are you sure?";
            public string QuitQuestionCaption 
                = "Quit";
            /**/
            public string OldPrivateCommonSettingsPasswordRequestCaption
                = "Enter old private common settings password";
            public string EmptyOldPasswordError
                = "Empty old password";
            public string WrongOldPasswordError
                = "Wrong old pass";
            public string PasswordChangedInfo
                = "Password changed";
            /**/

            public string UpdateClientLowProxyBalanceError
                = "For receiving client updates" +
                  " balance should be more than {BalaceLimit}";

        }
        public MainFormMessagesLocStrings Messages
            = new MainFormMessagesLocStrings();

        public class UpdateClientLocStrings
        {
            public string ProgressFormCaption
                = "Client update";
            public string ProgressFormReport1
                = "Getting current client version from server";

            public string YouHaveLastVersionAlreadyInfo
                = "You are already running the most recent version.";
            public string ForceUpdateQuestion
                = "You are already running the most recent version, force update?";
            public string NewVersionUpdateQuestion
                = "New version available, update now?";
            public string ProgressFormReport2
                = "Getting new version archive from server";

            public string DownloadProgressText = "Received {DownloadedKb}/{TotalKb}kb";

            public string ProgressFormReport3
                = "Local update";
        }
        public UpdateClientLocStrings UpdateClientLocStringsInstance
            = new UpdateClientLocStrings();
        /**/
        public string Text = "BitMoney GUI ({AppId}) v. {AppVersion}";
    }
}
