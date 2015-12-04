using System;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        private void InitProxyModel()
        {
            _proxyModel.ProxySessionModel.ClientServerTimeDifference = TimeSpan.Zero;
            /**/
            _proxyModel.ProxyServerConnected = false;
        }
    }
}
