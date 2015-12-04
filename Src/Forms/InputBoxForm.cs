using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Forms
{
    public partial class InputBoxForm : Form
    {
        private readonly string _caption;
        private readonly string _defaultValue = "";
        private readonly TaskCompletionSource<string> _taskValueSource 
            = new TaskCompletionSource<string>();

        public Task<string> TaskValue => _taskValueSource.Task;
		
	    private readonly Func<string, bool> _valueCheckerFunc;
        private readonly bool _multiline;
        public InputBoxForm(
		    string caption,
		    string defaultValue,
		    Func<string, bool> valueCheckerFunc,
            bool multiline = false
		    )
	    {
			Assert.NotNull(caption);
			Assert.NotNull(defaultValue);
			Assert.NotNull(valueCheckerFunc);
		    _caption = caption;
		    _defaultValue = defaultValue;
		    _valueCheckerFunc = valueCheckerFunc;
            _multiline = multiline;
			InitializeComponent();
        }

	    public InputBoxForm(
            string caption = "", 
            string defaultValue = "",
            string filterRegexValue = "^.*$",
            bool multiline = false
        ) : this(
			caption,
			defaultValue, 
			s => Regex.IsMatch(s,filterRegexValue),
            multiline
		)
        {
        }
        
        private void InputBoxForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _taskValueSource.TrySetResult("");
        }

        private void SetResult()
        {
            if (!_valueCheckerFunc(textBox1.Text))
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.Messages.WrongStringError
                );
                return;
            }
            _taskValueSource.TrySetResult(textBox1.Text);
            Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetResult();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _taskValueSource.TrySetResult("");
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!_valueCheckerFunc(textBox1.Text))
            {
                BackColor = Color.LightPink;
            }
            else
            {
                BackColor = Color.LightGreen;
            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetResult();
            }
        }

        private void InputBoxForm_Shown(object sender, EventArgs e)
        {
            InitCommonView();
            Text = _caption;
            textBox1.Multiline = _multiline;
            textBox1.Text = _defaultValue;
            var defaultValueLineNum = _defaultValue.Count(_ => _ == '\n');
            if (_multiline && defaultValueLineNum > 1)
            {
                Height += textBox1.Height * (defaultValueLineNum - 1);
            }
        }
        public static InputBoxFormLocStrings LocStrings
            = new InputBoxFormLocStrings();

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

    public class InputBoxFormLocStrings
    {
        public class LocMessages
        {
            public string WrongStringError = "Wrong string";
        }

        public LocMessages Messages = new LocMessages();
        /**/
        public string Button1Text = "OK";
        public string Button2Text = "Cancel";
        public string TextInit = "Caption";
    }
}
