using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BtmI2p.CryptFile.Lib;
using BtmI2p.GeneralClientInterfaces;
using BtmI2p.LightCertificates.Lib;
using BtmI2p.MiscUtils;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Communication.Exchange
{
    public class ExchangeClientSettings : ICheckable
    {
        [NonSerialized]
        public SemaphoreSlim LockSem = new SemaphoreSlim(1);

        public void CheckMe()
        {
        }
        public async Task SaveToFile(string path, byte[] pass)
        {
            using (await LockSem.GetDisposable().ConfigureAwait(false))
            {
                if (!File.Exists(path))
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

    public class ExchangeClientProfile : ICheckable
    {
        public ExchangeClientProfile()
        {
            SettingsPass = new byte[32];
            MiscFuncs.GetRandomBytes(SettingsPass);
            SettingsFileName 
                = $"{MiscFuncs.GenGuidWithFirstBytes(0)}.aes256";
        }
        public string ProfileName = string.Empty;
        public LightCertificate ExchangeClientCert = null;
        public string SettingsFileName;

        public string GetSettingsFilePath()
        {
            return Path.Combine(
                DefaultFolders.ExchangeSettingsFolder, 
                SettingsFileName
            );
        }
        public byte[] SettingsPass;
        public void CheckMe()
        {
            Assert.NotNull(ExchangeClientCert);
            if(string.IsNullOrWhiteSpace(SettingsFileName))
                throw new ArgumentNullException(
                    this.MyNameOfProperty(e => e.SettingsFileName)
                );
            ExchangeClientCert.CheckMe();
            Assert.True(
                LightCertificateRestrictions.IsValid(
                    ExchangeClientCert
                )
            );
        }
    }
}
