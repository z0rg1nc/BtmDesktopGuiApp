namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainFormLocStrings
    {
        public class WalletServerLocStrings
        {
            public class MessagesLocStrings
            {
                public string WalletAlreadyConnectedError = "Wallet already connected";

                public string WalletServerActionInProgressError
                    = "Wallet action in progress, retry later";
                public string NoTransferSelectedError
                    = "No transfer selected";
                public string NoDataAttachedError
                    = "No data attached";
                public string CommentIsNotUtf8StringError
                    = "Comment bytes is not UTF8 string";

                public string NotEnoughFundsError
                    = "Not enough funds";

                public string SendTransferError 
                    = "Send transfer error: '{ErrorMessage}'";
                public string RepeatTransfersTypeError
                    = "Only for 'Sent' and 'ErrorSent' transfers";

                public string SendTransferExtendedError
                    = "Send transfer '{RequestGuid}' error '{ErrorCode}' '{ErrorMessage}'";
            }
            public MessagesLocStrings Messages = new MessagesLocStrings();
            /**/
            public class RegistrationLocStrings
            {
                public string RegistrationLowBalanceError
                    = "For wallet registration proxy" +
                      " balance should be more than {BalanceLimit}";
            }
            public RegistrationLocStrings RegistrationLocStringsInstance
                = new RegistrationLocStrings();
            /**/

            public class ConnectingLocStrings
            {
                public string ConnectLowBalanceError
                    = "For connecting to wallet server proxy" +
                      " balance should be more than {BalanceLimit}";

                public string ProgressFormCaption = "Wallet server connection";
                public string ProgressFormReport1 = "User input";
                public string SelectWalletProfile = "Select wallet profile";
                public string ProgressFormReport2 = "Wallet settings loading";
                public string ParseProfileError 
                    = "Parse encrypted wallet profile err: '{ErrorMessage}'";
                public string DecryptOrParseWalletSettingsError 
                    = "Decrypt or parse wallet settings err: '{ErrorMessage}'";
                public string WalletAlreadyConnectedError
                    = "The wallet profile already connected";

                public string ProgressFormReport3 = "Init subscriptions";
                public string ProgressFormReport4 
                    = "Load transfer history from file";

                public string LoadTransferHistoryError
                    = "Loading transfer history from file error '{ErrorMessage}'";

                public string ProgressFormReport5
                    = "Wallet server connecting";

                public string CreateWalletServerSessionError
                    = "Create wallet server session error '{ErrorMessage}'";

                public string ProgressFormReport6
                    = "Get initial balance";
            }
            public ConnectingLocStrings ConnectingLocStringsInstance
                = new ConnectingLocStrings();
            /**/
            public class DisconnectingLocStrings
            {
                public string ProgressFormCaption = "Wallet disconnection";
                public string ProgressFormReport1 = "Wallet server disconnecting";
            }
            public DisconnectingLocStrings DisconnectingLocStringsInstance
                = new DisconnectingLocStrings();
            /**/
            public class ChangePasswordsLocStrings
            {
                public string SelectWalletPasswordToChange
                    = "Wallet password to change";
                public string EnterOldProfileFilePasswordText
                    = "Enter old profile file password";
                public string EnterNewProfileFilePasswordText
                    = "Enter new profile file password";
                public string EnterOldCertificatePasswordText
                    = "Enter old certificate password";
                public string EnterNewCertificatePasswordText
                    = "Enter new certificate password";
                public string EnterOldMasterCertificatePasswordText
                    = "Enter old master certificate password";
                public string EnterNewMasterCertificatePasswordText
                    = "Enter new master certificate password";
            }
            public ChangePasswordsLocStrings ChangePasswordsLocStringsInstance
                = new ChangePasswordsLocStrings();
        }
        public WalletServerLocStrings WalletServerLocStringsInstance
            = new WalletServerLocStrings();
    }
}
