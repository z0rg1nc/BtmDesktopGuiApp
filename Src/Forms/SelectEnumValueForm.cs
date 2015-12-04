using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.MiscUtils;

namespace BtmI2p.BitMoneyClient.Gui.Forms
{
    public partial class SelectEnumValueForm<T1> : Form 
        where T1 : struct
    {
        private readonly TaskCompletionSource<T1?> _tcs
            = new TaskCompletionSource<T1?>();
        public Task<T1?> Result { get { return _tcs.Task; } }
        private readonly List<Tuple<T1, string>> _initValues;
        private readonly string _caption;
        /**/
        private SelectEnumValueFormLocStrings LocStrings 
            = SelectEnumValueFormStatic.LocStrings;
        public SelectEnumValueForm(
            List<Tuple<T1,string>> initValues,
            string caption
        )
        {
            if(initValues == null || initValues.Count == 0)
                throw new ArgumentNullException(
                    MyNameof.GetLocalVarName(() => initValues)
                );
            _caption = caption;
            _initValues = initValues;
            InitializeComponent();
        }

        private void SetNullResult()
        {
            _tcs.TrySetResult(null);
            Close();
        }

        private void SetResult()
        {
            _tcs.TrySetResult(
                (T1)comboBox1.SelectedValue
            );
            Close();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            SetResult();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetNullResult();
        }

        private void SelectEnumValueForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetNullResult();
        }

        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
                SetResult();
        }

        private void SelectEnumValueForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SetResult();
        }

        private void SelectEnumValueForm_Shown(object sender, EventArgs e)
        {
            InitCommonView();
            comboBox1.DataSource = _initValues;
            comboBox1.ValueMember
                = MyNameof<Tuple<T1, string>>.Property(
                    x => x.Item1
                );
            comboBox1.DisplayMember
                = MyNameof<Tuple<T1, string>>.Property(
                    x => x.Item2
                );
            Text = _caption;
        }

        private void InitCommonView()
        {
            this.button1.Text = LocStrings.Button1Text;
            this.button2.Text = LocStrings.Button2Text;
            this.Text = LocStrings.TextInit;
            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }
    }

    public static class SelectEnumValueFormStatic
    {
        public static SelectEnumValueFormLocStrings LocStrings
            = new SelectEnumValueFormLocStrings();
    }

    public class SelectEnumValueFormLocStrings
    {
        public string Button1Text = "OK";
        public string Button2Text = "Cancel";
        public string TextInit = "Select enum value";
    }
}
