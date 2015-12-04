using System.Linq;
using BtmI2p.ComputableTaskInterfaces.Client;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        public void InitMiningView()
        {
            var locStrings = LocStrings.MiningServerLocStringsInstance;
            var taskTypeList = new[]
            {
                new
                {
                    TaskType = ETaskTypes.Scrypt, 
                    Name = locStrings.TaskTypes[0]
                }
            }.ToList();
            comboBox2.DataSource = taskTypeList;
            comboBox2.DisplayMember = "Name";
            comboBox2.ValueMember = "TaskType";
        }
    }
}
