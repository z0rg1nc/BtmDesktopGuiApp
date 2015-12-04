using System;
using System.Reactive.Subjects;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;


namespace BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers.Proxy
{
    public interface IProxyRpcSettings : IMyNotifyPropertyChanged
    {
        ushort PortNumber { get; set; }
        string HostName { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        TimeSpan RequestSentUtcTimeLimit { get; set; }
    }

    public class ProxyRpcSettings : IProxyRpcSettings
    {
        public ProxyRpcSettings()
        {
            HostName = "localhost";
            PortNumber = 14301;
            RequestSentUtcTimeLimit = TimeSpan.FromMinutes(1.0d);
            var usernamebuffer = new byte[6];
            MiscFuncs.GetRandomBytes(usernamebuffer);
            Username = Convert.ToBase64String(usernamebuffer);
            var passwordBuffer = new byte[9];
            MiscFuncs.GetRandomBytes(passwordBuffer);
            Password = Convert.ToBase64String(passwordBuffer);
        }

        public Subject<MyNotifyPropertyChangedArgs> PropertyChangedSubject
        {
            get
            {
                throw new Exception(MyNotifyPropertyChangedArgs.DefaultNotProxyExceptionString);
            }
        }
        public ushort PortNumber { get; set; }
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public TimeSpan RequestSentUtcTimeLimit { get; set; }
    }
}
