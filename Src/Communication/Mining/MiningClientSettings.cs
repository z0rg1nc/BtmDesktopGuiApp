using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.ComputableTaskInterfaces.Client;
using BtmI2p.CryptFile.Lib;
using BtmI2p.LightCertificates.Lib;
using BtmI2p.MiscUtils;
using BtmI2p.GeneralClientInterfaces;

namespace BtmI2p.BitMoneyClient.Gui.Communication.Mining
{
    public class MiningClientProfile : ICheckable
    {
        public MiningClientProfile()
        {
            SettingsPass = new byte[32];
            MiscFuncs.GetRandomBytes(SettingsPass);
            SettingsFileName = $"{Guid.NewGuid()}.aes256";
        }
        public string ProfileName = string.Empty;
        public LightCertificate MiningClientCert = null;
        public string SettingsFileName;

        public string GetSettingsFilePath()
        {
            return Path.Combine(
                DefaultFolders.MiningSettingsFolder, 
                SettingsFileName
            );
        }
        public byte[] SettingsPass;
        public void CheckMe()
        {
            if(MiningClientCert == null)
                throw new ArgumentNullException(
                    this.MyNameOfProperty(e => e.MiningClientCert)
                );
            if(string.IsNullOrWhiteSpace(SettingsFileName))
                throw new ArgumentNullException(
                    this.MyNameOfProperty(e => e.SettingsFileName)
                );
            MiningClientCert.CheckMe();
            if (
                !LightCertificateRestrictions.IsValid(
                    MiningClientCert
                )
            )
                throw new ArgumentOutOfRangeException(
                    this.MyNameOfProperty(e => e.MiningClientCert)
                );
        }
    }
    public class MiningClientSettings : ICheckable
    {
        [NonSerialized]
        public SemaphoreSlim LockSem = new SemaphoreSlim(1);
        /**/
        public List<MiningJobInfo> MiningJobInfos
            = new List<MiningJobInfo>();
        public List<ComputableTaskSerializedSolution> TaskSolutionsToPass
            = new List<ComputableTaskSerializedSolution>();
        public List<MiningTransferToWalletInfo> TransferToInfos
            = new List<MiningTransferToWalletInfo>(); 
        /**/
        public void CheckMe()
        {
            if(
                MiningJobInfos == null
                || TaskSolutionsToPass == null
                || TransferToInfos == null
            )
                throw new ArgumentNullException();
        }

        public async Task SaveToFile(string path, byte[] pass)
        {
            using (await LockSem.GetDisposable().ConfigureAwait(false))
            {
                if(!File.Exists(path))
                    File.Create(path).Close();
                var encryptedThis = 
                    ScryptPassEncryptedData.FromValue(
                        this,
                        pass
                    );
                File.WriteAllText(
                    path,
                    encryptedThis.WriteObjectToJson()
                );
            }
        }
    }
}
