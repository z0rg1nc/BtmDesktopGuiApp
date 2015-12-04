using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.GeneralClientInterfaces.ExchangeServer;
using BtmI2p.MiscUtils;
using BtmI2p.ObjectStateLib;
using NLog;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
    public partial class ExchangePaymentDetailsEditOrShowForm : Form
    {
        public ExchangePaymentDetails OriginDetails { get; }
        private readonly EExchangePaymentDetailsEditOrShowFormMode _mode;
        private readonly string _caption;
        private readonly bool _showAttentionLabel;
        public ExchangePaymentDetailsEditOrShowForm(
            ExchangePaymentDetails originDetails,
            EExchangePaymentDetailsEditOrShowFormMode mode,
            string caption,
            bool showAttentionLabel = false
        )
        {
            Assert.NotNull(originDetails);
            originDetails.CheckMe();
            Assert.NotNull(caption);
            OriginDetails = originDetails;
            _mode = mode;
            _caption = caption;
            _showAttentionLabel = showAttentionLabel;
            InitializeComponent();
        }
        private readonly TaskCompletionSource<DialogResult> _closingTaskCompletionSource
            = new TaskCompletionSource<DialogResult>();
        public Task<DialogResult> ClosingTask => _closingTaskCompletionSource.Task;
        //Ok
        private void button1_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
                {
                    if (_mode == EExchangePaymentDetailsEditOrShowFormMode.Edit)
                    {
                        foreach (var entryKey in _entriesControlDict.Keys)
                        {
                            var entry = OriginDetails.PaymentDetailsEntries.First(
                                _ => _.EntryKey == entryKey
                            );
                            var tBox = (TextBox) _entriesControlDict[entryKey];
                            switch (entry.EntryType)
                            {
                                case EExchangePaymentDetailsEntryType.IntType:
                                {
                                    entry.IntValue = int.Parse(tBox.Text);
                                }
                                    break;
                                case EExchangePaymentDetailsEntryType.LongType:
                                {
                                    entry.LongValue = long.Parse(tBox.Text);
                                }
                                    break;
                                case EExchangePaymentDetailsEntryType.DecimalType:
                                {
                                    entry.DecimalValue = decimal.Parse(tBox.Text);
                                }
                                    break;
                                case EExchangePaymentDetailsEntryType.GuidType:
                                {
                                    entry.GuidValue = Guid.Parse(tBox.Text);
                                }
                                    break;
                                case EExchangePaymentDetailsEntryType.StringType:
                                {
                                    entry.StringValue = tBox.Text;
                                }
                                    break;
                                case EExchangePaymentDetailsEntryType.ByteArrayType:
                                {
                                    entry.ByteArrayValue = Convert.FromBase64String(tBox.Text);
                                }
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                    }
                    _closingTaskCompletionSource.TrySetResult(DialogResult.OK);
                    Close();
                },
                _stateHelper,
                _log
            );
        }
        //Cancel
        private void button2_Click(object sender, EventArgs e)
        {
            _closingTaskCompletionSource.TrySetResult(DialogResult.Cancel);
            Close();
        }
        //Closing
        private async void ExchangePaymentDetailsEditOrShowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _closingTaskCompletionSource.TrySetResult(DialogResult.Cancel);
            await _stateHelper.MyDisposeAsync();
        }

        private TextBox CreateCheckableTextBox(Func<string, bool> checkFunc)
        {
            Assert.NotNull(checkFunc);
            var tBox = new TextBox();
            tBox.TextChanged += (sender, args) =>
            {
                var newText = tBox.Text;
                if (checkFunc(newText))
                    tBox.BackColor = Color.LightGreen;
                else
                    tBox.BackColor = Color.LightPink;
            };
            return tBox;
        }
        private readonly Dictionary<string,Control> _entriesControlDict
            = new Dictionary<string, Control>();
        public static ExchangePaymentDetailsEditOrShowFormDesignerLocStrings DesignerLocStrings = new ExchangePaymentDetailsEditOrShowFormDesignerLocStrings();
        private void InitCommonView()
        {
            this.label2.Text = DesignerLocStrings.Label2Text;
            this.label1.Text = DesignerLocStrings.Label1Text;
            this.button1.Text = DesignerLocStrings.Button1Text;
            this.button2.Text = DesignerLocStrings.Button2Text;
            this.label3.Text = DesignerLocStrings.Label3Text;
            this.takeAttentionLabel.Text = DesignerLocStrings.TakeAttentionLabelText;
            this.Text = DesignerLocStrings.Text;
            ClientGuiMainForm.ChangeControlFont(this, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
        }
        private void ExchangePaymentDetailsEditOrShowForm_Shown(object sender, EventArgs e)
        {
            _stateHelper.SetInitializedState();
            InitCommonView();
            if (!_showAttentionLabel)
                takeAttentionLabel.Visible = false;
            this.Text = _caption;
            /*Autoheight description textbox*/
            descriptionTextBox.Text = OriginDetails.Description;
            AutoHeightTextBox(descriptionTextBox);
            /* Entries */
            entriesTableLayoutPanel.Controls.Clear();
            entriesTableLayoutPanel.RowStyles.Clear();
            entriesTableLayoutPanel.RowCount = 0;
            var insertRowNum = 0;
            foreach (var entry in OriginDetails.PaymentDetailsEntries.Where(_ => _.Enabled))
            {
                var entryLabel = new Label{
                    Font = this.Font,
                    Text = entry.Description,
                    AutoSize = true,
                    Dock = DockStyle.Top
                };
                entriesTableLayoutPanel.RowStyles.Insert(insertRowNum, new RowStyle(SizeType.AutoSize));
                entriesTableLayoutPanel.Controls.Add(entryLabel,0,insertRowNum);
                insertRowNum++;
                /**/
                Control entryControl;
                if (_mode == EExchangePaymentDetailsEditOrShowFormMode.Edit)
                {
                    TextBox tBox;
                    switch (entry.EntryType)
                    {
                        case EExchangePaymentDetailsEntryType.IntType:
                        {
                            int a;
                            tBox = CreateCheckableTextBox(s => int.TryParse(s, out a));
                            tBox.Text = $"{entry.IntValue}";
                        }
                            break;
                        case EExchangePaymentDetailsEntryType.LongType:
                        {
                            long a;
                            tBox = CreateCheckableTextBox(s => long.TryParse(s, out a));
                            tBox.Text = $"{entry.LongValue}";
                        }
                            break;
                        case EExchangePaymentDetailsEntryType.DecimalType:
                        {
                            decimal a;
                            tBox = CreateCheckableTextBox(s => decimal.TryParse(s, out a));
                            tBox.Text = $"{entry.DecimalValue:G29}";
                        }
                            break;
                        case EExchangePaymentDetailsEntryType.GuidType:
                        {
                            Guid a;
                            tBox = CreateCheckableTextBox(s => Guid.TryParse(s, out a));
                            tBox.Text = $"{entry.GuidValue}";
                        }
                            break;
                        case EExchangePaymentDetailsEntryType.StringType:
                        {
                            tBox = new TextBox();
                            tBox.Text = entry.StringValue;
                            tBox.Multiline = true;
                            tBox.Resize += (o, args) =>
                            {
                                AutoHeightTextBox(tBox);
                            };
                            tBox.TextChanged += (o, args) =>
                            {
                                AutoHeightTextBox(tBox);
                            };
                        }
                            break;
                        case EExchangePaymentDetailsEntryType.ByteArrayType:
                        {
                            tBox = CreateCheckableTextBox(s =>
                            {
                                try
                                {
                                    var bAR = Convert.FromBase64String(s);
                                    return true;
                                }
                                catch
                                {
                                    return false;
                                }
                            });
                            tBox.Text = Convert.ToBase64String(entry.ByteArrayValue);
                        }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    tBox.Font = this.Font;
                    tBox.Dock = DockStyle.Top;
                    entryControl = tBox;
                }
                else
                {
                    var tBox = new TextBox()
                    {
                        Font = this.Font,
                        Dock = DockStyle.Top,
                        ReadOnly = true
                    };
                    switch (entry.EntryType)
                    {
                        case EExchangePaymentDetailsEntryType.IntType: {
                            tBox.Text = $"{entry.IntValue}";
                        }
                            break;
                        case EExchangePaymentDetailsEntryType.LongType:
                        {
                            tBox.Text = $"{entry.LongValue}";
                        }
                            break;
                        case EExchangePaymentDetailsEntryType.DecimalType:
                        {
                            tBox.Text = $"{entry.DecimalValue:G29}";
                        }
                            break;
                        case EExchangePaymentDetailsEntryType.GuidType:
                        {
                            tBox.Text = $"{entry.GuidValue}";
                        }
                            break;
                        case EExchangePaymentDetailsEntryType.StringType:
                        {
                            tBox.Text = entry.StringValue;
                            tBox.Multiline = true;
                            tBox.Resize += (o, args) =>
                            {
                                AutoHeightTextBox(tBox);
                            };
                        }
                            break;
                        case EExchangePaymentDetailsEntryType.ByteArrayType:
                        {
                            tBox.Text = Convert.ToBase64String(entry.ByteArrayValue);
                        }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    entryControl = tBox;
                }
                /**/
                _entriesControlDict.Add(entry.EntryKey,entryControl);
                /**/
                entriesTableLayoutPanel.RowStyles.Insert(insertRowNum, new RowStyle(SizeType.AutoSize));
                entriesTableLayoutPanel.Controls.Add(entryControl, 0, insertRowNum);
                insertRowNum++;
            }
            /*Autoheight form*/
            this.ClientSize = new Size(
                this.ClientSize.Width,
                tableLayoutPanel1.Size.Height + 1
            );
        }
        private static void AutoHeightTextBox(TextBox tBox)
        {
            var sz = new Size(tBox.ClientSize.Width, int.MaxValue);
            var flags = TextFormatFlags.TextBoxControl 
                | TextFormatFlags.WordBreak 
                | TextFormatFlags.NoClipping;
            var padding = 3;
            var borders = tBox.Height - tBox.ClientSize.Height;
            sz = TextRenderer.MeasureText(
                tBox.Text.With(_ => string.IsNullOrWhiteSpace(_) ? "a" : _), 
                tBox.Font, 
                sz, 
                flags
            );
            var h = sz.Height + borders + padding;
            /*
            if (descriptionTextBox.Top + h > this.ClientSize.Height - 10)
            {
                h = this.ClientSize.Height - 10 - descriptionTextBox.Top;
            }
            */
            tBox.Height = h;
        }

        private void descriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            AutoHeightTextBox(descriptionTextBox);
        }
        private void ExchangePaymentDetailsEditOrShowForm_ResizeEnd(object sender, EventArgs e)
        {
            AutoHeightTextBox(descriptionTextBox);
        }

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly DisposableObjectStateHelper _stateHelper 
            = new DisposableObjectStateHelper("ExchangePaymentDetailsEditOrShowForm");

        private void tableLayoutPanel1_Resize(object sender, EventArgs e)
        {
            
        }
    }

    public enum EExchangePaymentDetailsEditOrShowFormMode
    {
        Show,
        Edit
    }
    public class ExchangePaymentDetailsEditOrShowFormDesignerLocStrings
    {
        public string Label2Text = "Payment details entries";
        public string Label1Text = "Description";
        public string Button1Text = "OK";
        public string Button2Text = "Cancel";
        public string Label3Text = "StringEntry1 description";
        public string Text = "Payment details";
        public string TakeAttentionLabelText = "Don't use same payment details again!!! Always create new deposit.";
    }
}
