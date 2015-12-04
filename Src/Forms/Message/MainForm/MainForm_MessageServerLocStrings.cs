using System.Collections.Generic;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainFormLocStrings
    {
        public class MessageServerLocStrings
        {
            public class MessagesLocStrings
            {
                public string RegistrationLowBalanceError
                    = "For user registration server proxy" +
                      " balance should be more than {BalanceLimit}";
                public string MessageServerActionInProgressError
                    = "Some message server action in progress, retry later";
                public string MessageServerIsConnectedError
                    = "Message server is connected";
                public string MessageServerIsNotConnectedError
                    = "Message server is not connected";
                public string LoginLowBalanceError
                    = "For connecting to message server proxy" +
                      " balance should be more than {BalanceLimit}";
                public string ContactNotSelectedError
                    = "Contact not selected";
                public string EmptyMessageStringError
                    = "Empty message string";
                public string TooBigMessageError
                    = "Message is too big";

                public string SendMessageError
                    = "Send message error {ErrorMessage}";

                public string ContactAlreadyAddedError
                    = "Contact with such GUID already exist";
            }
            public MessagesLocStrings Messages 
                = new MessagesLocStrings();
            /**/

            public class DisconnectLocStrings
            {
                public string ProgressFormCaption = "User disconnection";
                public string ProgressFormReport1 = "Start";
                public string ProgressFormReport2 = "Server disconnecting";
                public string ProgressFormReport3 = "Finish";
            }
            public DisconnectLocStrings  DisconnectLocStringsInstance
                = new DisconnectLocStrings();
            /**/

            public class ConnectLocStrings
            {
                public string ProgressFormCaption = "Message server connection";
                public string ProgressFormReport1 = "User input";
                public string SelectUserProfileFormCaption = "Select user profile";
                public string UserProfileFilePasswordRequestText
                    = "Enter profile file password";
                public string ProgressFormReport2
                    = "Reading and checking user settings from file";
                public string LoadingUserProfileError
                    = "Reading user profile from file faulted";

                public string UserCertPasswordRequestText
                    = "Enter user certificate password";
                public string ProgressFormReport3
                    = "Message history loading";
                public string MessagesHistoryLoadingError
                    = "Message history loading from file error: '{ErrorMessage}'";
                public string ProgressFormReport4
                    = "Message server connecting";
                public string ProgressFormReport5
                    = "Successful user connection";
            }
            public ConnectLocStrings ConnectLocStringsInstance
                = new ConnectLocStrings();
            /**/

            public class AddContactLocStrings
            {
                public string AddUserThePermissionToWriteMeQuestion
                    = "Add user permission write to me?";

                public string GrantPermissionProgressFormCaption
                    = "Grant user permission write to me";
                public string GrantPermissionProgressFormReport1
                    = "Start";
                public string GrantPermissionProgressFormReport2
                    = "Finish";
            }
            public AddContactLocStrings AddContactLocStringsInstance
                = new AddContactLocStrings();
            /**/
            public class RemoveContactLocStrings
            {
                public string RevokePermissionTooQuestion
                    = "Revoke user permission write to me too?";

                public string ProgressFormCaption
                    = "Revoke user permission write to me";

                public string ProgressFormReport1
                    = "Start";

                public string ProgressFormReport2
                    = "Finish";
            }
            public RemoveContactLocStrings RemoveContactLocStringsInstance
                = new RemoveContactLocStrings();
            /**/
            public class ChangePasswordLocStrings
            {
                public string SelectPasswordToChangeFormCaption
                    = "User client password to change";
                public string OldProfileFilePasswordRequestText
                    = "Enter old profile file password";
                public string NewProfileFilePasswordRequestText
                    = "Enter new profile file password";
                public string OldCertPasswordRequestText
                    = "Enter old certificate password";
                public string NewCertPasswordRequestText
                    = "Enter new certificate password";
                public string OldMasterCertPasswordRequestText
                    = "Enter old master certificate password";
                public string NewMasterCertPasswordRequestText
                    = "Enter new master certificate password";
            }
            public ChangePasswordLocStrings ChangePasswordLocStringsInstance
                = new ChangePasswordLocStrings();
            /**/
            public class IssueInvoiceLocStrings
            {
                public string RefillAmount = "Refill amount";
                public string WrongTransferAmountError = "Wrong transfer amount";
            }
            public IssueInvoiceLocStrings IssueInvoiceLocStringsInstance
                = new IssueInvoiceLocStrings();
            /**/
            public class EditUserSettingsLocStrings
            {
                public string ProgressFormCaption
                    = "Getting my settings from message server";

                public string ProgressFormReport1 = "Start";
                public string ProgressFormReport2
                    = "Getting current settings on message server";
                public string ProgressFormReport3
                    = "Finish";
            }
            public EditUserSettingsLocStrings EditUserSettingsLocStringsInstance
                = new EditUserSettingsLocStrings();
            /**/

            public class UpdateContactInfosLocStrings
            {
                public string ProgressFormCaption = "Receiving contact infos";
                public string ProgressFormReport1 = "Start";
                public string ProgressFormReport2 = "Finish";
            }
            public UpdateContactInfosLocStrings UpdateContactInfosLocStringsInstance
                = new UpdateContactInfosLocStrings();
            /**/
            public class PermissionsLocStrings
            {
                public string PermissionAlreadyGrantedError
                    = "Permission already granted";
                public string GrantToWriteMeProgressFormCaption
                    = "Grant user permission to write me";
                public string PermissionIsNotGrantedError
                    = "Permission is not yet granted";
                public string RevokeProgressFormCaption  
                    = "Revoke user permission write to me";
                public string AuthorizeMePermissionGuidRequestFormCaption
                    = "Permission GUID";
                public string ParsePermissionGuidError
                    = "Permission GUID parse error";
                public string GetPermissionProgressFormCaption
                    = "Get permission write to user";
            }
            public PermissionsLocStrings PermissionsLocStringsInstance
                = new PermissionsLocStrings();
            /**/
            public List<string> Combobox3DataSourceTexts = new List<string>(
                new[]
                {
                    "5 Min",
                    "1 Hour",
                    "1 Day",
                    "1 Week",
                    "1 Month"
                }
            );

            public string IncomeMessageHeader = "{SentTime}(+{LifetimeMinutes}) <<< from {UserFrom} {AuthUser}{AuthKey}{AuthMessage}: ";
            public string OutcomeMessageHeader = "{SentTime}(+{LifetimeMinutes}) >>> to {UserTo} {AuthUser}{AuthKey}{AuthMessage}:";
            public string StatusNotLogged = "Not logged";
            public string StatusLogged = "{ConectionStatus} as {ProfileName}";
        }
        public MessageServerLocStrings MessageServerLocStringsInstance
            = new MessageServerLocStrings();
    }
}
