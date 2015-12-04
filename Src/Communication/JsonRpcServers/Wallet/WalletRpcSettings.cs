using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;

namespace BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers.Wallet
{
    public interface IWalletRpcSettings : IMyNotifyPropertyChanged
    {
        ushort PortNumber { get; set; }
        string HostName { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        List<Guid> RpcAllowedWalletGuids { get; set; }
        bool AllowSendingPaymentsViaRpc { get; set; }
        bool DontCheckHmac { get; set; }
        byte[] RpcSendingPaymentsHmacKeycode { get; set; }
        TimeSpan RequestSentUtcTimeLimit { get; set; }
    }

    public class WalletRpcSettings : IWalletRpcSettings
    {
        public WalletRpcSettings()
        {
            HostName = "localhost";
            PortNumber = 14300;
            RpcAllowedWalletGuids = new List<Guid>();
            AllowSendingPaymentsViaRpc = false;
            DontCheckHmac = false;
            RequestSentUtcTimeLimit = TimeSpan.FromMinutes(1.0d);
            var usernamebuffer = new byte[6];
            MiscFuncs.GetRandomBytes(usernamebuffer);
            Username = Convert.ToBase64String(usernamebuffer);
            var passwordBuffer = new byte[9];
            MiscFuncs.GetRandomBytes(passwordBuffer);
            Password = Convert.ToBase64String(passwordBuffer);
            RpcSendingPaymentsHmacKeycode = new byte[64];
            MiscFuncs.GetRandomBytes(RpcSendingPaymentsHmacKeycode);
        }

        public Subject<MyNotifyPropertyChangedArgs> PropertyChangedSubject {
            get
            {
                throw new Exception(MyNotifyPropertyChangedArgs.DefaultNotProxyExceptionString);
            }
        }
        public ushort PortNumber { get; set; }
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Guid> RpcAllowedWalletGuids { get; set; }
        public bool AllowSendingPaymentsViaRpc { get; set; }
        public bool DontCheckHmac { get; set; }
        public byte[] RpcSendingPaymentsHmacKeycode { get; set; }
        public TimeSpan RequestSentUtcTimeLimit { get; set; }
    }
}
