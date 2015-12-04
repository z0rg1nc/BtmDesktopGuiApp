using System.Collections.Generic;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainFormLocStrings
    {
        public class MiningServerLocStrings
        {
            public class MessagesLocStrings
            {
                public string MiningServerIsNotConnectedError
                    = "Mining server is not connected";
                public string MiningServerConnectedError
                    = "Mining server already connected";
                public string MiningServerActionInProgressError
                    = "Some mining server action in progress, retry later";
            }
            public MessagesLocStrings Messages = new MessagesLocStrings();
            /**/

            public class AddJobLocStrings
            {
                public string EmptyJobNameError = "Empty job name";
                public string ParseAmountError = "Parse amount error";
                public string AmountLessThanZeroError = "amount <= 0";
                public string TaskTypeNotSelectedError = "Task type is not selected";
            }
            public AddJobLocStrings AddJobLocStringsInstance
                = new AddJobLocStrings();
            /**/
            public class SendTransferLocStrings
            {
                public string ParseTransferAmountError = "Parse transfer amount error";
                public string AmountLessThanZeroError = "amount <= 0";
                public string ParseWalletToError = "Parse wallet To error";
                public string EnterMiningCertPasswordRequestText
                    = "Enter mining cert password";
            }
            public SendTransferLocStrings SendTransferLocStringsInstance
                = new SendTransferLocStrings();
            /**/
            public class RegistrationLocStrings
            {
                public string RegistrationLowBalanceError
                    = "For mining client registration server proxy" +
                      " balance should be more than {BalanceLimit}";
            }
            public RegistrationLocStrings RegistrationLocStringsInstance
                = new RegistrationLocStrings();
            /**/
            public class ConnectingLocStrings
            {
                public string ConnectingLowBalanceError
                    = "For connecting to mining server proxy" +
                                " balance should be more than {BalanceLimit}";

                public string ProgressFormCaption = "Mining client connection";
                public string ProgressFormReport1 = "User input";
                public string SelectMiningProfileFormCaption = "Select mining profile";
                public string ProgressFormReport2 = "Mining client settings loading";
                public string ProfileNotValid = "Mining profile not valid {ErrorMessage}";
                public string SettingsNotValid = "Mining settings not valid {ErrorMessage}";
                public string ProgressFormReport3 = "Mining client session initializing";
                public string ProgressFormReport4 = "Pass out transfer infos";
                public string ProgressFormReport5 = "Pass out task solutions";
                public string ProgressFormReport6 = "Pass saved job infos";
                public string ProgressFormReport7 = "Get initial balance";
                public string ProgressFormReport8 = "Connection complete";
            }
            public ConnectingLocStrings ConnectingLocStringsInstance
                = new ConnectingLocStrings();
            /**/
            public class DisconnectingLocStrings
            {
                public string ProgressFormCaption = "Mining server disconnection";
            }
            public DisconnectingLocStrings DisconnectingLocStringsInstance
                = new DisconnectingLocStrings();
            /**/
            public class ChangePasswordLocStrings
            {
                public string SelectPasswordKindText = "Mining client password to change";
                public string EnterOldProfileFilePaswordText
                    = "Enter old profile file password";
                public string EnterNewProfileFilePaswordText
                    = "Enter new profile file password";
                public string EnterOldCertPaswordText
                    = "Enter old certificate password";
                public string EnterNewCertPaswordText
                    = "Enter new certificate password";
            }
            public ChangePasswordLocStrings ChangePasswordLocStringsInstance
                = new ChangePasswordLocStrings();
            /**/
            public List<string> TaskTypes = new List<string>(new[]
            {
                "Scrypt"
            }); 
        }
        public MiningServerLocStrings MiningServerLocStringsInstance
            = new MiningServerLocStrings();
    }
}
