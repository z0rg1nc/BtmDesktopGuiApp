using System;
using System.Reactive.Subjects;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;


namespace BtmI2p.BitMoneyClient.Gui.Proxy
{
    public interface IPublicProxySettings : IMyNotifyPropertyChanged, ICheckable
    {
        bool AutoFillup { get; set; }
        decimal AutoFillupMinBalance { get; set; }
        string SamServerAddress { get; set; }
        int SamServerPort { get; set; }
        //TimeSpan PingProxyInterval { get; set; }
    }

    public class PublicProxySettings : IPublicProxySettings
    {
        public PublicProxySettings()
        {
            AutoFillup = true;
            AutoFillupMinBalance = 10.0m;
            SamServerAddress = "127.0.0.1";
            SamServerPort = 7656;
            //PingProxyInterval = TimeSpan.FromSeconds(30.0);
        }

        public bool AutoFillup { get; set; }
        public decimal AutoFillupMinBalance { get; set; }
        public string SamServerAddress { get; set; }
        public int SamServerPort { get; set; }
        //public TimeSpan PingProxyInterval { get; set; }
        
        public void CheckMe()
        {
            if(string.IsNullOrWhiteSpace(SamServerAddress))
                throw new ArgumentNullException(
                    MyNameof.GetLocalVarName(() => SamServerAddress));
            if(SamServerPort <= 0 || SamServerPort > 65535)
                throw new ArgumentOutOfRangeException(
                    MyNameof.GetLocalVarName(() => SamServerPort));
            if (AutoFillupMinBalance <= 0)
                throw new ArgumentOutOfRangeException(
                    MyNameof.GetLocalVarName(() => AutoFillupMinBalance));
        }
        /**/
        public Subject<MyNotifyPropertyChangedArgs> PropertyChangedSubject
        {
            get
            {
                throw new Exception(MyNotifyPropertyChangedArgs.DefaultNotProxyExceptionString);
            }
        }
    }
    
    public interface IPrivateProxySettings : IMyNotifyPropertyChanged, ICheckable
    {
        string ClientPrivKeys { get; set; }
    }

    public class PrivateProxySettings : IPrivateProxySettings
    {
        public PrivateProxySettings()
        {
            ClientPrivKeys = null;
        }
        public string ClientPrivKeys { get; set; }
        /**/
        public void CheckMe()
        {
        }
        public Subject<MyNotifyPropertyChangedArgs> PropertyChangedSubject
        {
            get
            {
                throw new Exception(MyNotifyPropertyChangedArgs.DefaultNotProxyExceptionString);
            }
        }
    }
}
