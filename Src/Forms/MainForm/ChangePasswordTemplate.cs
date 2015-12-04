using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BtmI2p.CryptFile.Lib;
using BtmI2p.LightCertificates.Lib;
using BtmI2p.MiscClientForms;
using BtmI2p.MiscUtils;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public class ChangePasswordTemplateLocStrings
    {
        /**/
        public string SelectProfileFormCaption;
        public string SelectPasswordToChangeFormCaption;
        public string OldProfileFilePasswordRequestText;
        public string NewProfileFilePasswordRequestText;
        /**/
        public string OldCertPasswordRequestText;
        public string NewCertPasswordRequestText;
        /**/
        public string OldMasterCertPasswordRequestText;
        public string NewMasterCertPasswordRequestText;
        /**/
        public void CheckMe(List<EProfilePasswordKinds> passwordKindList)
        {
            Assert.NotNull(passwordKindList);
            Assert.False(string.IsNullOrWhiteSpace(SelectProfileFormCaption));
            Assert.False(string.IsNullOrWhiteSpace(SelectPasswordToChangeFormCaption));
            Assert.False(string.IsNullOrWhiteSpace(OldProfileFilePasswordRequestText));
            Assert.False(string.IsNullOrWhiteSpace(NewProfileFilePasswordRequestText));
            if (passwordKindList.Contains(EProfilePasswordKinds.Cert))
            {
                Assert.False(string.IsNullOrWhiteSpace(OldCertPasswordRequestText));
                Assert.False(string.IsNullOrWhiteSpace(NewCertPasswordRequestText));
            }
            if (passwordKindList.Contains(EProfilePasswordKinds.MasterCert))
            {
                Assert.False(string.IsNullOrWhiteSpace(OldMasterCertPasswordRequestText));
                Assert.False(string.IsNullOrWhiteSpace(NewMasterCertPasswordRequestText));
            }
        }
    }
    public enum EProfilePasswordKinds
    {
        Profile,
        Cert,
        MasterCert
    }
    public partial class ClientGuiMainForm
    {
        private void ChangePasswordTemplate<TProfile>(
            ChangePasswordTemplateLocStrings locStrings,
            SemaphoreSlim actionLockSem,
            Func<IDisposable> actionInProgressDisposableFunc,
            string profilesFolder,
            Func<string,Task<bool>> isProfileConnectedAsyncFunc,
            List<EProfilePasswordKinds> passwordKindList,
            Func<TProfile,LightCertificate> certSelectorFunc,
            Func<TProfile,LightCertificate> masterCertSelectorFunc
        )
        {
            Assert.NotNull(actionLockSem);
            Assert.NotNull(actionInProgressDisposableFunc);
            Assert.NotNull(profilesFolder);
            Assert.False(string.IsNullOrWhiteSpace(profilesFolder));
            Assert.NotNull(isProfileConnectedAsyncFunc);
            Assert.NotNull(passwordKindList);
            Assert.NotEmpty(passwordKindList);
            Assert.Equal(passwordKindList,passwordKindList.Distinct());
            /**/
            Assert.NotNull(locStrings);
            locStrings.CheckMe(passwordKindList);
            /**/
            HandleControlActionProper(async () =>
            {
                using (await actionLockSem.GetDisposable())
                {
                    using (actionInProgressDisposableFunc())
                    {
                        DefaultFolders.CreateFoldersIfNotExist();
                        var dirInfo = new DirectoryInfo(profilesFolder);
                        var profileFileList = dirInfo.GetFiles(
                            "*.aes256",
                            SearchOption.TopDirectoryOnly
                        );
                        if (profileFileList.Length == 0)
                        {
                            ShowErrorMessage(
                                LocStrings.CommonMessages.NoProfileFilesFoundError
                            );
                            return;
                        }
                        var profileTcs = new TaskCompletionSource<FileInfo>();
                        await (new SelectProfileForm(
                            profileTcs,
                            profileFileList,
                            locStrings.SelectProfileFormCaption
                        )).ShowFormAsync(this);
                        var profileFileInfo = await profileTcs.Task;
                        if (profileFileInfo == null)
                        {
                            return;
                        }
                        var profileName = profileFileInfo.Name.Substring(
                            0, profileFileInfo.Name.Length - 7
                        );
                        if (await isProfileConnectedAsyncFunc(profileName))
                        {
                            ShowErrorMessage(
                                LocStrings.CommonMessages.LogoutFirstError
                            );
                            return;
                        }
                        var passwordKindsDict = new Dictionary<EProfilePasswordKinds, string>()
                        {
                            {EProfilePasswordKinds.Profile, LocStrings.CommonText.Profile},
                            {EProfilePasswordKinds.Cert, LocStrings.CommonText.Certificate},
                            {EProfilePasswordKinds.MasterCert, LocStrings.CommonText.MasterCertificate}
                        };
                        var selectPassKindForm = new SelectEnumValueForm<EProfilePasswordKinds>(
                            passwordKindList.Select(_ => Tuple.Create(_, passwordKindsDict[_])).ToList(),
                            locStrings.SelectPasswordToChangeFormCaption
                        );
                        await selectPassKindForm.ShowFormAsync(this);
                        var selectPassKind = await selectPassKindForm.Result;
                        if (selectPassKind == null)
                            return;
                        var encryptedProfile = File.ReadAllText(profileFileInfo.FullName, Encoding.UTF8)
                            .ParseJsonToType<ScryptPassEncryptedData>();
                        if (selectPassKind == EProfilePasswordKinds.Profile)
                        {
                            var oldProfileFilenamePassword
                                = await EnterPasswordForm.CreateAndReturnResult(
                                    locStrings.OldProfileFilePasswordRequestText,
                                    this
                                );
                            /**/
                            if (
                                oldProfileFilenamePassword
                                    == null
                            )
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                );
                                return;
                            }
                            using (var tempPass = oldProfileFilenamePassword.TempData)
                            {
                                if (
                                    !encryptedProfile.CheckPass(
                                        tempPass.Data
                                    )
                                )
                                {
                                    ShowErrorMessage(
                                        LocStrings.CommonMessages.WrongPasswordError
                                    );
                                    return;
                                }
                            }
                            /**/
                            var newProfileFilenamePassword
                                = await EnterPasswordForm.CreateAndReturnResult(
                                    locStrings.NewProfileFilePasswordRequestText,
                                    this
                                );
                            /**/
                            if (
                                newProfileFilenamePassword
                                    == null
                            )
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                );
                                return;
                            }
                            /**/
                            using (var oldTempPass = oldProfileFilenamePassword.TempData)
                            {
                                using (var newTempPass = newProfileFilenamePassword.TempData)
                                {
                                    await encryptedProfile.ChangePass(
                                        oldTempPass.Data,
                                        newTempPass.Data
                                    ).ConfigureAwait(false);
                                }
                            }

                            File.WriteAllText(
                                profileFileInfo.FullName,
                                encryptedProfile.WriteObjectToJson(),
                                Encoding.UTF8
                            );
                            ShowInfoMessage(
                                LocStrings.CommonMessages.PasswordChangedInfo
                            );
                            return;
                        }
                        else if (selectPassKind == EProfilePasswordKinds.Cert)
                        {
                            /**/
                            var oldProfileFilenamePassword
                                = await EnterPasswordForm.CreateAndReturnResult(
                                    LocStrings.CommonText.EnterProfileFilePasswordRequest,
                                    this
                                );
                            /**/
                            if (
                                oldProfileFilenamePassword
                                     == null
                            )
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                );
                                return;
                            }
                            using (var tempPass = oldProfileFilenamePassword.TempData)
                            {
                                if (
                                    !encryptedProfile.CheckPass(
                                        tempPass.Data
                                    )
                                )
                                {
                                    ShowErrorMessage(
                                        LocStrings.CommonMessages.WrongPasswordError
                                    );
                                    return;
                                }
                            }
                            TProfile profile;
                            using (var tempPass = oldProfileFilenamePassword.TempData)
                            {
                                profile = encryptedProfile
                                    .GetValue<TProfile>(
                                        tempPass.Data
                                    );
                            }
                            /**/
                            var certPassword
                                = await EnterPasswordForm.CreateAndReturnResult(
                                    locStrings.OldCertPasswordRequestText,
                                    this
                                );
                            /**/
                            if (
                                certPassword == null
                            )
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                );
                                return;
                            }
                            using (var tempPass = certPassword.TempData)
                            {
                                if (
                                    !certSelectorFunc(profile).IsPassValid(
                                        tempPass.Data
                                    )
                                )
                                {
                                    ShowErrorMessage(
                                        LocStrings.CommonMessages.WrongPasswordError
                                    );
                                    return;
                                }
                            }
                            /**/
                            var newCertPassword
                                = await EnterPasswordForm.CreateAndReturnResult(
                                    locStrings.NewCertPasswordRequestText,
                                    this
                                );
                            /**/
                            if (
                                newCertPassword == null
                            )
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                );
                                return;
                            }
                            /**/
                            using (var oldTempPass = certPassword.TempData)
                            {
                                using (var newTempPass = newCertPassword.TempData)
                                {
                                    certSelectorFunc(profile).ChangePass(
                                        oldTempPass.Data,
                                        newTempPass.Data
                                    );
                                }
                            }
                            using (var tempPass = oldProfileFilenamePassword.TempData)
                            {
                                ScryptPassEncryptedData.WriteToFile(
                                    profile,
                                    profileFileInfo.FullName,
                                    tempPass.Data
                                );
                            }
                            ShowInfoMessage(
                                LocStrings.CommonMessages.PasswordChangedInfo
                            );
                            return;
                        }
                        else if (selectPassKind == EProfilePasswordKinds.MasterCert)
                        {
                            /**/
                            var oldProfileFilenamePassword
                                = await EnterPasswordForm.CreateAndReturnResult(
                                    LocStrings.CommonText.EnterProfileFilePasswordRequest,
                                    this
                                );
                            /**/
                            if (
                                oldProfileFilenamePassword
                                == null
                                )
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                );
                                return;
                            }
                            using (var tempPass = oldProfileFilenamePassword.TempData)
                            {
                                if (
                                    !encryptedProfile.CheckPass(
                                        tempPass.Data
                                        )
                                    )
                                {
                                    ShowErrorMessage(
                                        LocStrings.CommonMessages.WrongPasswordError
                                    );
                                    return;
                                }
                            }
                            /**/
                            TProfile profile;
                            using (var tempPass = oldProfileFilenamePassword.TempData)
                            {
                                profile = encryptedProfile
                                    .GetValue<TProfile>(
                                        tempPass.Data
                                    );
                            }
                            /**/
                            var masterCertPassword
                                = await EnterPasswordForm.CreateAndReturnResult(
                                    locStrings.OldMasterCertPasswordRequestText,
                                    this
                                );
                            /**/
                            if (
                                masterCertPassword == null
                                )
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                );
                                return;
                            }
                            using (var tempPass = masterCertPassword.TempData)
                            {
                                if (
                                    !masterCertSelectorFunc(profile).IsPassValid(
                                        tempPass.Data
                                        )
                                    )
                                {
                                    ShowErrorMessage(
                                        LocStrings.CommonMessages.WrongPasswordError
                                    );
                                    return;
                                }
                            }
                            /**/
                            var newMasterCertPassword
                                = await EnterPasswordForm.CreateAndReturnResult(
                                    locStrings.NewMasterCertPasswordRequestText,
                                    this
                                );
                            /**/
                            if (
                                newMasterCertPassword == null
                                )
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                );
                                return;
                            }
                            /**/
                            using (var oldTempPass = masterCertPassword.TempData)
                            {
                                using (var newTempPass = newMasterCertPassword.TempData)
                                {
                                    masterCertSelectorFunc(profile).ChangePass(
                                        oldTempPass.Data,
                                        newTempPass.Data
                                    );
                                }
                            }
                            using (var tempPass = oldProfileFilenamePassword.TempData)
                            {
                                ScryptPassEncryptedData.WriteToFile(
                                    profile,
                                    profileFileInfo.FullName,
                                    tempPass.Data
                                );
                            }
                            ShowInfoMessage(
                                LocStrings.CommonMessages.PasswordChangedInfo
                            );
                            return;
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(
                                MyNameof.GetLocalVarName(() => selectPassKind)
                            );
                        }
                    }
                }
            });
        }
    }
}
