using BtmI2p.BitMoneyClient.Gui.Communication.Exchange;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        public void InitExchangeModel()
        {
            ExchangeClientModel.Reset(_exchangeModel);
        }
    }
}
