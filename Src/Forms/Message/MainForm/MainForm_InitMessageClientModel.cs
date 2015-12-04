using BtmI2p.BitMoneyClient.Gui.Communication.Message;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        private void InitMessageClientModel()
        {
            _messageClientModel.ConnectionStatus 
                = MessageSesionConnectionStatus.Disconnected;
        }
    }
}
