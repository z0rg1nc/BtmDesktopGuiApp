using System;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        public void InitMiningModel()
        {
            _miningModel.MiningServerConnected = false;
            _miningModel.MiningAccountBalance = 0;
            _miningModel.MiningClientGuid = Guid.Empty;
            _miningModel.MiningClientProfileName = string.Empty;
        }
    }
}
