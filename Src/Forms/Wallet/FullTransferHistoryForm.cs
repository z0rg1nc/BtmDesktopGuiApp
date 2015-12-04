using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Wallet;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.BitMoneyClient.Lib.WalletServerSession;
using BtmI2p.GeneralClientInterfaces.WalletServer;
using BtmI2p.MiscClientForms;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged.MyObservableCollections;
using BtmI2p.MyNotifyPropertyChanged.Winforms;
using BtmI2p.ObjectStateLib;
using NLog;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Wallet
{
    public partial class FullTransferHistoryForm : Form
    {
        private readonly MyObservableCollectionSafeAsyncImpl<WalletFormModelTransferInfo> _originData
            = new MyObservableCollectionSafeAsyncImpl<WalletFormModelTransferInfo>();
        private readonly ObservableCollectionProxyFilter<WalletFormModelTransferInfo> _proxyFilter
            = new ObservableCollectionProxyFilter<WalletFormModelTransferInfo>(async _ => await Task.FromResult(true));
        private readonly MyObservableCollectionProxyComparer<WalletFormModelTransferInfo> _proxyComparer
            = new MyObservableCollectionProxyComparer<WalletFormModelTransferInfo>(
                Comparer<WalletFormModelTransferInfo>.Create(
                    (ord1, ord2) => -ord1.SentTime.CompareTo(ord2.SentTime)
                )
            );
        private MyNFirstObservableCollectionImpl<WalletFormModelTransferInfo> _nFirstObservableCollection;
        private readonly MyObservableCollectionProxyN _proxyN
            = new MyObservableCollectionProxyN(Int32.MaxValue);
        /**/
        private enum ESortTransfers
        {
            Direction,
            Amount,
            WalletFrom,
            WalletTo,
            Time
        }
        private ESortTransfers _sortBy = ESortTransfers.Time;
        private bool _sortDesc = true;
        private readonly IWalletFormModel _walletFormModel;
        public FullTransferHistoryForm(
            IWalletFormModel walletFormModel
        )
        {
            _walletFormModel = walletFormModel;
            InitializeComponent();
        }
        
        private void listView4_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this,
                () =>
                {
                    if (
                        (_sortBy == ESortTransfers.Direction && e.Column == 0)
                        || (_sortBy == ESortTransfers.Amount && e.Column == 1)
                        || (_sortBy == ESortTransfers.WalletFrom && e.Column == 2)
                        || (_sortBy == ESortTransfers.WalletTo && e.Column == 3)
                        || (_sortBy == ESortTransfers.Time && e.Column == 5)
                    )
                    {
                        _sortDesc = !_sortDesc;
                    }
                    switch (e.Column)
                    {
                        case 0:
                            _sortBy = ESortTransfers.Direction;
                            break;
                        case 1:
                            _sortBy = ESortTransfers.Amount;
                            break;
                        case 2:
                            _sortBy = ESortTransfers.WalletFrom;
                            break;
                        case 3:
                            _sortBy = ESortTransfers.WalletTo;
                            break;
                        case 5:
                            _sortBy = ESortTransfers.Time;
                            break;
                        default:
                            break;
                    }
                    _proxyComparer.Comparer = Comparer<WalletFormModelTransferInfo>.Create(
                        (t1, t2) =>
                        {
                            int res = 0;
                            if (_sortBy == ESortTransfers.Amount)
                                res = t1.Amount.CompareTo(t2.Amount);
                            else if (_sortBy == ESortTransfers.Direction)
                                res = t1.Direction.CompareTo(t2.Direction);
                            else if (_sortBy == ESortTransfers.Time)
                                res = t1.SentTime.CompareTo(t2.SentTime);
                            else if (_sortBy == ESortTransfers.WalletFrom)
                                res = t1.WalletFrom.CompareTo(t2.WalletFrom);
                            else if (_sortBy == ESortTransfers.WalletTo)
                                res = t1.WalletTo.CompareTo(t2.WalletTo);
                            if (_sortDesc) res *= -1;
                            return res;
                        }
                    );
                },
                _stateHelper,
                _log
            );
        }
        /**/
        private readonly DisposableObjectStateHelper _stateHelper 
            = new DisposableObjectStateHelper("FullTransferHistoryForm");

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        
        private void FullTransferHistoryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void showTranferInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this,
                async () =>
                {
                    var focusedItem = transferListView.FocusedItem;
                    if (focusedItem == null)
                    {
                        return;
                    }
                    var transferInfo = focusedItem.Tag as WalletFormModelTransferInfo;
                    if (transferInfo == null)
                    {
                        return;
                    }
                    await (new ShowTransferInfoForm(transferInfo)).ShowFormAsync(this);
                },
                _stateHelper,
                _log
            );
        }

        private void copyWalletToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this,
                () =>
                {
                    var focusedItem = transferListView.FocusedItem;
                    if (focusedItem == null)
                    {
                        ClientGuiMainForm.ShowErrorMessage(this,
                            LocStrings.Messages.NoTransferSelectedError
                        );
                        return;
                    }
                    var transferInfo = focusedItem.Tag as WalletFormModelTransferInfo;
                    if (transferInfo == null)
                    {
                        ClientGuiMainForm.ShowErrorMessage(this,
                            LocStrings.Messages.NoDataAttachedError
                        );
                        return;
                    }
                    Clipboard.SetText(
                        $"{transferInfo.WalletTo}"
                        );
                },
                _stateHelper,
                _log
            );
        }

        private void copyWalletFromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this,
                () =>
                {
                    var focusedItem = transferListView.FocusedItem;
                    if (focusedItem == null)
                    {
                        ClientGuiMainForm.ShowErrorMessage(this,
                            LocStrings.Messages.NoTransferSelectedError
                        );
                        return;
                    }
                    var transferInfo = focusedItem.Tag as WalletFormModelTransferInfo;
                    if (transferInfo == null)
                    {
                        ClientGuiMainForm.ShowErrorMessage(this,
                            LocStrings.Messages.NoDataAttachedError
                        );
                        return;
                    }
                    Clipboard.SetText(
                        $"{transferInfo.WalletFrom}"
                        );
                },
                _stateHelper,
                _log
            );
        }

        private void copyCommentStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this, () =>
                {
                    var focusedItem = transferListView.FocusedItem;
                    if (focusedItem == null)
                    {
                        ClientGuiMainForm.ShowErrorMessage(this,
                            LocStrings.Messages.NoTransferSelectedError
                        );
                        return;
                    }
                    var transferInfo = focusedItem.Tag as WalletFormModelTransferInfo;
                    if (transferInfo == null)
                    {
                        ClientGuiMainForm.ShowErrorMessage(this,
                            LocStrings.Messages.NoDataAttachedError
                        );
                        return;
                    }
                    string commentString = null;
                    if (transferInfo.CommentBytes == null
                        || transferInfo.CommentBytes.Length == 0)
                    {
                        commentString = string.Empty;
                    }
                    else
                    {
                        try
                        {
                            commentString = Encoding.UTF8.GetString(transferInfo.CommentBytes);
                        }
                        catch
                        {
                            ClientGuiMainForm.ShowErrorMessage(this,
                                LocStrings.Messages.NotUtf8StringError
                            );
                            return;
                        }
                    }
                    Clipboard.SetText(commentString.With(
                        _ => _ == "" ? " " : _
                    ));
                },
                _stateHelper,
                _log
            );
        }

        private void repeatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this,
                () =>
                {
                    var focusedItem = transferListView.FocusedItem;
                    if (focusedItem == null)
                    {
                        return;
                    }
                    var transferInfo = focusedItem.Tag as WalletFormModelTransferInfo;
                    if (transferInfo == null)
                    {
                        return;
                    }
                    if (transferInfo.TransferStatus != WalletFormModelTransferStatus.Sent)
                    {
                        ClientGuiMainForm.ShowErrorMessage(this,
                            LocStrings.Messages.OnlyForSentTransfersError
                        );
                        return;
                    }
                    ((ClientGuiMainForm)Owner).ProcessWalletInvoice(
                        new BitmoneyInvoiceData()
                        {
                            CommentBytes = transferInfo.CommentBytes,
                            ForceAnonymousTransfer = transferInfo.AnonymousTransfer,
                            TransferAmount = transferInfo.Amount,
                            WalletTo = transferInfo.WalletTo
                        },
                        _walletFormModel.WalletProfileInstance.WalletCert.Id
                    );
                },
                _stateHelper,
                _log
            );
        }
        // Get data
        private void button1_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this, async () =>
                {
                    var fromDateTime = fromDtPickerDay.Value.Date 
                        + fromDtPickerTimeOfDay.Value.TimeOfDay;
                    var toDateTime = untilDtPickerDay.Value.Date 
                        + untilDtPickerTimeOfDay.Value.TimeOfDay;
                    if(fromDateTime >= toDateTime)
                        throw new ArgumentOutOfRangeException(
                            MyNameof.GetLocalVarName(() => fromDateTime)
                        );
                    /**/
                    using (var wrp = await ProgressCancelFormWraper.CreateInstance(LocStrings.ProgressFormCaption, this))
                    {
                        var lastSentTransferTime = fromDateTime;
                        var lastReceivedTransferTime = fromDateTime;
                        var fromToTimeDiffMs = (toDateTime - fromDateTime).TotalMilliseconds;
                        var updateProgressAction = (Action)(
                            () => wrp.ProgressInst.ReportProgress(
                                string.Empty,
                                (int)(49.0d * ((lastSentTransferTime - fromDateTime).TotalMilliseconds / fromToTimeDiffMs)
                                      + 49.0d * ((lastReceivedTransferTime - fromDateTime).TotalMilliseconds / fromToTimeDiffMs))
                            )
                        );
                        var getSentTransferTask = await Task.Factory.StartNew(
                            async () =>
                            {
                                if (!sentCheckBox.Checked)
                                    return new List<ClientTransferBase>();
                                var res = new List<ClientTransferBase>();
                                Guid lastKnownSentTransferGuid = Guid.Empty;
                                while (true)
                                {
                                    var outTransfers = await _walletFormModel.WalletSession.GetOutcomeTransfersChunk(
                                        fromDateTime,
                                        toDateTime,
                                        lastKnownSentTransferGuid,
                                        wrp.Token
                                        );
                                    if (outTransfers.Item2.Count == 0)
                                        break;
                                    res.AddRange(outTransfers.Item2);
                                    lastKnownSentTransferGuid = outTransfers.Item1;
                                    lastSentTransferTime = outTransfers.Item2.Last().SentTime;
                                    updateProgressAction();
                                }
                                return res;
                            });
                        var getReceivedTransferTask = await Task.Factory.StartNew(
                            async () =>
                            {
                                if (!receivedCheckBox.Checked)
                                    return new List<ClientTransferBase>();
                                var res = new List<ClientTransferBase>();
                                Guid lastKnownReceivedTransferGuid = Guid.Empty;
                                while (true)
                                {
                                    var inTransfers = await _walletFormModel.WalletSession.GetIncomeTransfersChunk(
                                        fromDateTime,
                                        toDateTime,
                                        lastKnownReceivedTransferGuid,
                                        wrp.Token
                                        );
                                    if (inTransfers.Item2.Count == 0)
                                        break;
                                    res.AddRange(inTransfers.Item2);
                                    lastKnownReceivedTransferGuid = inTransfers.Item1;
                                    lastReceivedTransferTime = inTransfers.Item2.Last().SentTime;
                                    updateProgressAction();
                                }
                                return res;
                            }
                        );
                        var sentTransfers = await getSentTransferTask;
                        var receivedTransfers = await getReceivedTransferTask;
                        await _originData.ClearAsync();
                        await _originData.AddRangeAsync(
                            sentTransfers.Select(
                                outcomeTransfer => WalletFormModelTransferInfo
                                    .FromOutcomeTransfer(
                                        outcomeTransfer,
                                        _walletFormModel.WalletProfileInstance.WalletCert.Id
                                    )
                            ).ToList()
                        ).ConfigureAwait(false);
                        await _originData.AddRangeAsync(
                            receivedTransfers.Select(
                                receivedTransfer => WalletFormModelTransferInfo
                                    .FromIncomeTransfer(
                                        receivedTransfer,
                                        _walletFormModel.WalletProfileInstance.WalletCert.Id
                                    )
                            ).ToList()
                        ).ConfigureAwait(false);
                    }
                },
                _stateHelper,
                _log
            );
        }
        
        // Update filters
        private void button2_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this,
                () =>
                {
                    var fromDateTime = fromDtPickerDay.Value.Date
                        + fromDtPickerTimeOfDay.Value.TimeOfDay;
                    var toDateTime = untilDtPickerDay.Value.Date
                        + untilDtPickerTimeOfDay.Value.TimeOfDay;
                    /**/
                    var showSent = sentCheckBox.Checked;
                    var showReceived = receivedCheckBox.Checked;
                    /**/
                    var walletFromFilter = walletFromCheckBox.Checked;
                    Guid walletFrom = Guid.Empty;
                    walletFromTextBox.BackColor = SystemColors.Window;
                    if (walletFromFilter)
                    {
                        if (!Guid.TryParse(walletFromTextBox.Text, out walletFrom))
                        {
                            walletFromFilter = false;
                            walletFromTextBox.BackColor = Color.LightPink;
                        }
                    }
                    /**/
                    var walletToFilter = walletToCheckBox.Checked;
                    Guid walletTo = Guid.Empty;
                    walletToTextBox.BackColor = SystemColors.Window;
                    if (walletToFilter)
                    {
                        if (!Guid.TryParse(
                            walletToTextBox.Text,
                            out walletTo))
                        {
                            walletToFilter = false;
                            walletToTextBox.BackColor = Color.LightPink;
                        }
                    }
                    /**/
                    var amountFilter = amountCheckBox.Checked;
                    long amount = 0;
                    amountFilterTextBox.BackColor = SystemColors.Window;
                    if (amountFilter)
                    {
                        if (!long.TryParse(amountFilterTextBox.Text, out amount))
                        {
                            amountFilter = false;
                            amountFilterTextBox.BackColor = Color.LightPink;
                        }
                    }
                    string combobox1Value = amountConditionComboBox.Text;
                    if (string.IsNullOrWhiteSpace(combobox1Value))
                        amountFilter = false;
                    /**/
                    var anonymousOnlyFilter = anonymousFilterCheckBox.Checked;
                    /**/
                    var commentStringContainsFilter = commentFilterCheckBox.Checked;
                    var commentPatternToSearch = commentFilterTextBox.Text;
                    _proxyFilter.Predicate = async x =>
                    {
                        if (x.SentTime < fromDateTime || x.SentTime > toDateTime)
                            return false;
                        if (x.TransferStatus == WalletFormModelTransferStatus.Sent && !showSent)
                            return false;
                        if (x.TransferStatus == WalletFormModelTransferStatus.Received && !showReceived)
                            return false;
                        if (walletFromFilter && x.WalletFrom != walletFrom)
                            return false;
                        if (walletToFilter && x.WalletTo != walletTo)
                            return false;
                        if (amountFilter)
                        {
                            if (combobox1Value == "<" && !(x.Amount < amount))
                                return false;
                            if (combobox1Value == "<=" && !(x.Amount <= amount))
                                return false;
                            if (combobox1Value == "==" && x.Amount != amount)
                                return false;
                            if (combobox1Value == ">=" && !(x.Amount >= amount))
                                return false;
                            if (combobox1Value == ">" && !(x.Amount > amount))
                                return false;
                        }
                        if (anonymousOnlyFilter && !x.AnonymousTransfer)
                            return false;
                        if (commentStringContainsFilter)
                        {
                            try
                            {
                                var commentUtf8 = Encoding.UTF8.GetString(x.CommentBytes);
                                if (!commentUtf8.Contains(commentPatternToSearch))
                                    return false;
                            }
                            catch
                            {
                                return false;
                            }
                        }
                        return await Task.FromResult(true).ConfigureAwait(false);
                    };
                },
                _stateHelper,
                _log
            );
        }
        
        private void listView4_MouseClick(object sender, MouseEventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this, () =>
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        var focusedItem = transferListView.FocusedItem;
                        if (
                            focusedItem != null
                            && focusedItem.Bounds.Contains(e.Location)
                        )
                        {
                            contextMenuStrip_ShowTransferInfo.Show(
                                Cursor.Position
                            );
                        }
                    }
                },
                _stateHelper,
                _log
            );
        }

        private void listView4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this, async () =>
                {
                    var focusedItem = transferListView.FocusedItem;
                    if (focusedItem == null)
                    {
                        return;
                    }
                    var transferInfo = focusedItem.Tag as WalletFormModelTransferInfo;
                    if (transferInfo == null)
                    {
                        return;
                    }
                    await (new ShowTransferInfoForm(transferInfo)).ShowFormAsync(this);
                },
                _stateHelper,
                _log
            );
        }

        public static void EditWalletTransferListViewItem(
            WalletFormModelTransferInfo transferInfo,
            ListViewItem item
            )
        {
            item.Tag = transferInfo;
            string tranferComment;
            try
            {
                tranferComment
                    = Encoding.UTF8.GetString(transferInfo.CommentBytes);
                if (tranferComment.Length > 50)
                    tranferComment = tranferComment.Substring(0, 50) + "...";
            }
            catch
            {
                tranferComment = "???";
            }
            item.SubItems[0].Text = $"{transferInfo.TransferStatus}";
            if (transferInfo.TransferStatus == WalletFormModelTransferStatus.SendError)
                item.SubItems[0].Text += $" ({transferInfo.ErrCode} {transferInfo.ErrMessage})";
            item.SubItems[1].Text = $"{transferInfo.Amount}";
            item.SubItems[2].Text = $"{transferInfo.Fee}";
            item.SubItems[3].Text = $"{transferInfo.WalletFrom}";
            item.SubItems[4].Text = $"{transferInfo.WalletTo}";
            item.SubItems[5].Text = tranferComment;
            item.SubItems[6].Text = $"{transferInfo.SentTime:G}";
            item.SubItems[7].Text = 
                $"{(transferInfo.AuthenticatedOtherWalletCert ? '+' : '-')}" +
                $"{(transferInfo.AuthenticatedCommentKey ? '+' : '-')}" +
                $"{(transferInfo.AuthenticatedTransferDetails ? '+' : '-')}";
            if (!transferInfo.AnonymousTransfer)
            {
                item.Font = new Font(
                    item.Font,
                    FontStyle.Bold
                );
            }
            else
            {
                item.Font = new Font(
                    item.Font,
                    FontStyle.Italic | FontStyle.Bold
                );
            }
            item.BackColor
                = transferInfo.WalletFrom == transferInfo.WalletTo ? Color.White
                    : transferInfo.Direction
                        ? Color.Orange
                        : Color.LightGreen;
        }

        private async void FullTransferHistoryForm_Shown(object sender, EventArgs e)
        {
            _stateHelper.SetInitializedState();
            InitCommonView();
            /**/
            var filteredCollectionChanged =
                await MyFilteredObservableCollectionImpl.CreateInstance(
                    _originData,
                    _proxyFilter
                );
            var orderedCollectionChanged =
                await MyOrderedObservableCollection.CreateInstance(
                    filteredCollectionChanged,
                    _proxyComparer
                );
            _nFirstObservableCollection =
                await MyNFirstObservableCollectionImpl.CreateInstance(
                    orderedCollectionChanged,
                    _proxyN
                );
            var binding = await ListViewCollectionChangedOneWayBinding.CreateInstance(
                transferListView,
                _nFirstObservableCollection,
                null,
                EditWalletTransferListViewItem
            );
            _asyncSubscriptions.Add(
                new CompositeMyAsyncDisposable(
                    binding,
                    _nFirstObservableCollection,
                    orderedCollectionChanged,
                    filteredCollectionChanged
                )
            );
            /**/
            baseWalletGuidTextBox.Text = $"{_walletFormModel.WalletProfileInstance.WalletCert.Id}";
            fromDtPickerDay.Value =
                untilDtPickerDay.Value =
                    fromDtPickerTimeOfDay.Value =
                        untilDtPickerTimeOfDay.Value = DateTime.UtcNow;
        }
        public static FullTransferHistoryFormLocStrings LocStrings
            = new FullTransferHistoryFormLocStrings();
        public static FullTransferHistoryFormDesignerLocStrings DesignerLocStrings = new FullTransferHistoryFormDesignerLocStrings();
        private void InitCommonView()
        {
            this.baseWalletGuidLabel.Text = DesignerLocStrings.BaseWalletGuidLabelText;
            this.getDataButton.Text = DesignerLocStrings.GetDataButtonText;
            this.sentCheckBox.Text = DesignerLocStrings.SentCheckBoxText;
            this.receivedCheckBox.Text = DesignerLocStrings.ReceivedCheckBoxText;
            this.statusHeader.Text = DesignerLocStrings.StatusHeaderText;
            this.amountHeader.Text = DesignerLocStrings.AmountHeaderText;
            this.feeHeader.Text = DesignerLocStrings.FeeHeaderText;
            this.walletFromHeader.Text = DesignerLocStrings.WalletFromHeaderText;
            this.walletToHeader.Text = DesignerLocStrings.WalletToHeaderText;
            this.commentHeader.Text = DesignerLocStrings.CommentHeaderText;
            this.sentTimeHeader.Text = DesignerLocStrings.SentTimeHeaderText;
            this.authenticationsHeader.Text = DesignerLocStrings.AuthenticationsHeaderText;
            this.showTranferInfoToolStripMenuItem.Text = DesignerLocStrings.ShowTranferInfoToolStripMenuItemText;
            this.copyWalletToToolStripMenuItem.Text = DesignerLocStrings.CopyWalletToToolStripMenuItemText;
            this.copyWalletFromToolStripMenuItem.Text = DesignerLocStrings.CopyWalletFromToolStripMenuItemText;
            this.copyCommentStringToolStripMenuItem.Text = DesignerLocStrings.CopyCommentStringToolStripMenuItemText;
            this.repeatToolStripMenuItem.Text = DesignerLocStrings.RepeatToolStripMenuItemText;
            this.transfersLabel.Text = DesignerLocStrings.TransfersLabelText;
            this.updateFiterButton.Text = DesignerLocStrings.UpdateFiterButtonText;
            this.anonymousFilterCheckBox.Text = DesignerLocStrings.AnonymousFilterCheckBoxText;
            this.commentFilterCheckBox.Text = DesignerLocStrings.CommentFilterCheckBoxText;
            this.amountCheckBox.Text = DesignerLocStrings.AmountCheckBoxText;
            this.walletToCheckBox.Text = DesignerLocStrings.WalletToCheckBoxText;
            this.walletFromCheckBox.Text = DesignerLocStrings.WalletFromCheckBoxText;
            this.untilDbLabel.Text = DesignerLocStrings.UntilDbLabelText;
            this.fromDtLabel.Text = DesignerLocStrings.FromDtLabelText;
            this.csvExportButton.Text = DesignerLocStrings.CsvExportButtonText;
            this.dataLabel.Text = DesignerLocStrings.DataLabelText;
            this.filtersLabel.Text = DesignerLocStrings.FiltersLabelText;
            this.columnAutowidthByHeaderToolStripMenuItem.Text = DesignerLocStrings.ColumnAutowidthByHeaderToolStripMenuItemText;
            this.columnAutowidthByContentToolStripMenuItem.Text = DesignerLocStrings.ColumnAutowidthByContentToolStripMenuItemText;
            this.Text = DesignerLocStrings.Text;
            ClientGuiMainForm.ChangeControlFont(this, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenuStrip_ShowTransferInfo, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
        }
        //Export to CSV
        private void button4_Click_1(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this,
                async () =>
                {
                    var initFileName = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        $"hist_{Guid.NewGuid().ToString().Substring(0, 6)}.csv"
                    );
                    var fileNameForm = new InputBoxForm(
                        LocStrings.CsvExportLocStringsInstance.ExportSelectFileInputCaption,
                        initFileName,
                        s =>
                        {
                            try
                            {
                                Assert.False(string.IsNullOrWhiteSpace(s));
                                var dir = Path.GetDirectoryName(s);
                                Assert.NotNull(dir);
                                return Directory.Exists(dir);
                            }
                            catch
                            {
                                return false;
                            }
                        }
                    );
                    await fileNameForm.ShowFormAsync(this);
                    var fileName = await fileNameForm.TaskValue;
                    if (string.IsNullOrWhiteSpace(fileName))
                        return;
                    var directoryName = Path.GetDirectoryName(fileName);
                    if (directoryName == null)
                        return;
                    if (!Directory.Exists(directoryName))
                        ClientGuiMainForm.ShowErrorMessage(
                            this,
                            ClientGuiMainForm.LocStrings.CommonMessages.FolderNotFoundError
                        );
                    var data = (await _nFirstObservableCollection.GetDeepCopyAsync()).NewItems;
                    using (var myStream = new FileStream(fileName, FileMode.Create))
                    {
                        using (var textWriter = new StreamWriter(myStream, Encoding.UTF8))
                        {
                            await textWriter.WriteLineAsync(
                                "DIRECTION(1-sent/0-received)" +
                                ";ANONYMOUS(1/0)" +
                                ";AMOUNT" +
                                ";FEE" +
                                ";WALLET_FROM" +
                                ";WALLET_TO" +
                                ";COMMENT_BASE64" +
                                ";SENT_TIME(yyyyMMddHHmmss)" +
                                ";REQUEST_GUID" +
                                ";TRANSFER_GUID" +
                                ";AUTHENTICATED_OTHER_WALLET_CERT(1/0)" +
                                ";AUTHENTICATED_KEY(1/0)" +
                                ";AUTHENTICATED_PAYMENT_DETAILS(1/0)"
                            );
                            foreach (var modelTransferInfo in data)
                            {
                                await textWriter.WriteLineAsync(
                                    $"{(modelTransferInfo.Direction ? 1 : 0)}" +
                                    $";{(modelTransferInfo.AnonymousTransfer ? 1 : 0)}" +
                                    $";{modelTransferInfo.Amount}" +
                                    $";{modelTransferInfo.Fee}" +
                                    $";{modelTransferInfo.WalletFrom}" +
                                    $";{modelTransferInfo.WalletTo}" +
                                    $";\"{Convert.ToBase64String(modelTransferInfo.CommentBytes)}\"" +
                                    $";{modelTransferInfo.SentTime:yyyyMMddHHmmss}" +
                                    $";{modelTransferInfo.RequestGuid}" +
                                    $";{modelTransferInfo.TransferGuid}" +
                                    $";{(modelTransferInfo.AuthenticatedOtherWalletCert ? 1 : 0)}" +
                                    $";{(modelTransferInfo.AuthenticatedCommentKey ? 1 : 0)}" +
                                    $";{(modelTransferInfo.AuthenticatedTransferDetails ? 1 : 0)}"
                                );
                            }
                        }
                    }
                    ClientGuiMainForm.ShowInfoMessage(this,
                        LocStrings.CsvExportLocStringsInstance.ExportComplete
                    );
                },
                _stateHelper,
                _log
            );
        }
        private readonly List<IMyAsyncDisposable> _asyncSubscriptions = new List<IMyAsyncDisposable>(); 
        private async void FullTransferHistoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            await _stateHelper.MyDisposeAsync();
            foreach (var asyncSubscription in _asyncSubscriptions)
            {
                await asyncSubscription.MyDisposeAsync();
            }
            _asyncSubscriptions.Clear();
        }

        private void columnAutowidthByHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transferListView.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void columnAutowidthByContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transferListView.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.ColumnContent);
        }
    }
    public class FullTransferHistoryFormDesignerLocStrings
    {
        public string BaseWalletGuidLabelText = "Wallet GUID:";
        public string GetDataButtonText = "Get data";
        public string SentCheckBoxText = "Sent";
        public string ReceivedCheckBoxText = "Received";
        public string StatusHeaderText = "Status";
        public string AmountHeaderText = "Amount";
        public string FeeHeaderText = "Fee";
        public string WalletFromHeaderText = "From";
        public string WalletToHeaderText = "To";
        public string CommentHeaderText = "Comment";
        public string SentTimeHeaderText = "Time";
        public string AuthenticationsHeaderText = "Authentications";
        public string ShowTranferInfoToolStripMenuItemText = "Show full transfer info";
        public string CopyWalletToToolStripMenuItemText = "Copy wallet to";
        public string CopyWalletFromToolStripMenuItemText = "Copy wallet from";
        public string CopyCommentStringToolStripMenuItemText = "Copy comment string";
        public string RepeatToolStripMenuItemText = "Repeat";
        public string TransfersLabelText = "Transfers";
        public string UpdateFiterButtonText = "Update";
        public string AnonymousFilterCheckBoxText = "Anonymous only";
        public string CommentFilterCheckBoxText = "Comment contains";
        public string AmountCheckBoxText = "Amount";
        public string WalletToCheckBoxText = "Wallet to";
        public string WalletFromCheckBoxText = "Wallet from";
        public string UntilDbLabelText = "Until";
        public string FromDtLabelText = "From (UTC time, not local)";
        public string CsvExportButtonText = "Export to CSV";
        public string DataLabelText = "Data";
        public string FiltersLabelText = "Filters";
        public string ColumnAutowidthByHeaderToolStripMenuItemText = "Column autowidth by header";
        public string ColumnAutowidthByContentToolStripMenuItemText = "Column autowidth by content";
        public string Text = "Transfers history";
    }
    public class FullTransferHistoryFormLocStrings
    {
        public class MessagesLocStrings
        {
            public string NoTransferSelectedError = "No transfer selected";
            public string NoDataAttachedError
                = "No data attached";
            public string NotUtf8StringError = "Comment bytes is not UTF8 string";
            public string OnlyForSentTransfersError = "Only for 'Sent' transfers";
        }
        public MessagesLocStrings Messages = new MessagesLocStrings();
        /**/
        public string ProgressFormCaption = "Getting transfer infos";
        /**/

        public class CsvExportLocStrings
        {
            public string ExportComplete = "Export complete";
            public string ExportSelectFileInputCaption = "Input filename";
        }
        public CsvExportLocStrings CsvExportLocStringsInstance = new CsvExportLocStrings();
    }
}
