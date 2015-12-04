using System;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainFormLocStrings
    {
        
    }
    public partial class ClientGuiMainForm
    {
        private void InitMessageClientView()
        {
            var combobox3DataSource = new[]
            {
                new { 
                    Name = LocStrings.MessageServerLocStringsInstance.Combobox3DataSourceTexts[0], 
                    Value = TimeSpan.FromMinutes(5.0f) 
                },
                new
                {
                    Name = LocStrings.MessageServerLocStringsInstance.Combobox3DataSourceTexts[1], 
                    Value = TimeSpan.FromHours(1.0f)
                },
                new
                {
                    Name = LocStrings.MessageServerLocStringsInstance.Combobox3DataSourceTexts[2], 
                    Value = TimeSpan.FromDays(1.0f)
                },
                new
                {
                    Name = LocStrings.MessageServerLocStringsInstance.Combobox3DataSourceTexts[3], 
                    Value = TimeSpan.FromDays(7.0f)
                },
                new
                {
                    Name = LocStrings.MessageServerLocStringsInstance.Combobox3DataSourceTexts[4], 
                    Value = TimeSpan.FromDays(30.0)
                }
            };
            comboBox3.DataSource = combobox3DataSource;
            comboBox3.DisplayMember = "Name";
            comboBox3.ValueMember = "Value";
        }
    }
}
