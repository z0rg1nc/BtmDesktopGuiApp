using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.GeneralClientInterfaces.ExchangeServer;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;
using BtmI2p.MyNotifyPropertyChanged.MyObservableCollections;
using BtmI2p.MyNotifyPropertyChanged.Winforms;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        private MyHotSwapObservableCollectionImpl<ExchangeAccountTranferClientInfo> _hotSwapObservableTransferCollection;
        private Tuple<string, bool> _exchangeTransfersSortedArgs; //True - asc
        private IMyObservableCollectionProxyComparer<ExchangeAccountTranferClientInfo> _proxyTransferComparer;
        private IMyObservableCollectionProxyN _proxyTransferN;
        /**/
        private MyHotSwapObservableCollectionImpl<ExchangeAccountLockedFundsClientInfo> _hotSwapObservableLockedFundsCollection;
        private Tuple<string, bool> _exchangeLockedFundsSortedArgs; //True - asc
        private IMyObservableCollectionProxyComparer<ExchangeAccountLockedFundsClientInfo> _proxyLockedFundsComparer;
        private IMyObservableCollectionProxyN _proxyLockedFundsN;
        private async Task InitExchangeSubscriptions()
        {
            _hotSwapObservableTransferCollection =
                await MyHotSwapObservableCollectionImpl<ExchangeAccountTranferClientInfo>.CreateInstance(
                    new MyObservableCollectionSafeAsyncImpl<ExchangeAccountTranferClientInfo>()
                ).ConfigureAwait(false);
            _exchangeTransfersSortedArgs = Tuple.Create(
                nameof(ExchangeAccountTranferClientInfo.SentDateTime),
                false
            );
            _proxyTransferComparer = new MyObservableCollectionProxyComparer<ExchangeAccountTranferClientInfo>(
                Comparer<ExchangeAccountTranferClientInfo>.Create(
                    (a, b) => -Comparer<DateTime>.Default.Compare(a.SentDateTime, b.SentDateTime)
                    )
                );
            var orderedTransferCollectionChanged =
                await MyOrderedObservableCollection<ExchangeAccountTranferClientInfo>.CreateInstance(
                    _hotSwapObservableTransferCollection,
                    _proxyTransferComparer
                    ).ConfigureAwait(false);
            _proxyTransferN = new MyObservableCollectionProxyN(100);
            var firstNTransferCollectionChanged =
                await MyNFirstObservableCollectionImpl<ExchangeAccountTranferClientInfo>.CreateInstance(
                    orderedTransferCollectionChanged,
                    _proxyTransferN
                ).ConfigureAwait(false);
            var transferListViewBinding =
                await ListViewCollectionChangedOneWayBinding<ExchangeAccountTranferClientInfo>.CreateInstance(
                    exchangeTransferListView,
                    firstNTransferCollectionChanged,
                    null,
                    (transfer, item) =>
                    {
                        item.Tag = transfer.TransferGuid;
                        item.SubItems[0].Text = $"{transfer.SentDateTime}";
                        item.SubItems[1].Text =
                            $"{LocStrings.ExchangeServerLocStringsInstance.TransferTypeLocStrings[transfer.TransferType]}";
                        item.SubItems[2].Text = $"{transfer.Value}";
                        item.SubItems[3].Text = $"{transfer.Note}";
                        item.SubItems[4].Text = $"{transfer.TransferGuid}";
                    }
                ).ConfigureAwait(false);
            /**/
            _hotSwapObservableLockedFundsCollection =
                await MyHotSwapObservableCollectionImpl<ExchangeAccountLockedFundsClientInfo>.CreateInstance(
                    new MyObservableCollectionSafeAsyncImpl<ExchangeAccountLockedFundsClientInfo>()
                ).ConfigureAwait(false);
            var filteredLockedFundsCollectionChanged =
                await MyFilteredObservableCollectionImpl.CreateInstance(
                    _hotSwapObservableLockedFundsCollection,
                    new ObservableCollectionProxyFilter<ExchangeAccountLockedFundsClientInfo>(
                        async _ => await Task.FromResult(_.IsActive)
                        )
                    ).ConfigureAwait(false);
            _exchangeLockedFundsSortedArgs = Tuple.Create(
                nameof(ExchangeAccountLockedFundsClientInfo.LockDate),
                false
            );
            _proxyLockedFundsComparer = new MyObservableCollectionProxyComparer<ExchangeAccountLockedFundsClientInfo>(
                Comparer<ExchangeAccountLockedFundsClientInfo>.Create(
                    (a, b) => -Comparer<DateTime>.Default.Compare(a.LockDate, b.LockDate)
                    )
                );
            var orderedLockedFundsCollectionChanged =
                await MyOrderedObservableCollection.CreateInstance(
                    filteredLockedFundsCollectionChanged,
                    _proxyLockedFundsComparer
                    ).ConfigureAwait(false);
            _proxyLockedFundsN = new MyObservableCollectionProxyN(100);
            var firstNLockedFundsCollectionChanged =
                await MyNFirstObservableCollectionImpl.CreateInstance(
                    orderedLockedFundsCollectionChanged,
                    _proxyLockedFundsN
                ).ConfigureAwait(false);
            var lockedFundsListViewBinding =
                await ListViewCollectionChangedOneWayBinding.CreateInstance(
                    exchangeLockedFundsListView,
                    firstNLockedFundsCollectionChanged,
                    null,
                    (lockedFund, item) =>
                    {
                        item.Tag = lockedFund.LockedFundsGuid;
                        item.SubItems[0].Text = $"{lockedFund.LockDate}";
                        item.SubItems[1].Text = $"{lockedFund.Value}";
                        item.SubItems[2].Text = $"{lockedFund.Note}";
                        item.SubItems[3].Text = $"{lockedFund.LockedFundsGuid}";
                    }
                ).ConfigureAwait(false);
            /**/
            var accountWithBalanceJoinedCollection = await MyJoinedObservableCollectionImpl.CreateInstance(
                _exchangeModel.SessionModel.Data.AccountCollection,
                _exchangeModel.SessionModel.Data.AccountBalanceCollection,
                (acc1, balance1) => acc1.AccountGuid == balance1.AccountGuid,
                JoinType.LeftJoin
            ).ConfigureAwait(false);
            var accountWithBalanceBinding = await ListViewCollectionChangedOneWayBinding.CreateInstance(
                exchangeAccountsListView,
                accountWithBalanceJoinedCollection,
                null,
                (tuple, item) =>
                {
                    item.Tag = tuple.Item1.AccountGuid;
                    item.SubItems[0].Text = $"{tuple.Item1.AccountGuid}";
                    item.SubItems[1].Text = $"{tuple.Item1.CurrencyCode}";
                    if (tuple.Item2 != null)
                    {
                        item.SubItems[2].Text = $"{tuple.Item2.CommittedBalance:G29}";
                        item.SubItems[3].Text = $"{tuple.Item2.LockedFundsBalance:G29}";
                        item.SubItems[4].Text = $"{tuple.Item2.TotalAvailableBalance:G29}";
                    }
                    else
                    {
                        item.SubItems[2].Text = "0.0";
                        item.SubItems[3].Text = "0.0";
                        item.SubItems[4].Text = "0.0";
                    }
                    item.Font = new Font(
                        item.Font,
                        tuple.Item1.IsDefaultForTheCurrency
                            ? FontStyle.Bold
                            : FontStyle.Regular
                    );
                }
            ).ConfigureAwait(false);
            /**/
            _formSubscriptions.AddRange(
                new[]
                {
                    _exchangeModel.PropertyChangedSubject
                        .Subscribe(ExchangeModelPropertyChangedAction),
                    _exchangeModel.SessionModel.Data.PropertyChangedSubject
                        .Subscribe(ExchangeSessionModelDataPropertyChangedAction)
                }
            );
            _formAsyncSubscriptions.AddRange(
                new[] {
                    new CompositeMyAsyncDisposable(
                        transferListViewBinding,
                        firstNTransferCollectionChanged,
                        orderedTransferCollectionChanged,
                        _hotSwapObservableTransferCollection
                    ),
                    new CompositeMyAsyncDisposable(
                        lockedFundsListViewBinding,
                        firstNLockedFundsCollectionChanged,
                        orderedLockedFundsCollectionChanged,
                        filteredLockedFundsCollectionChanged,
                        _hotSwapObservableLockedFundsCollection
                    ),
                    new CompositeMyAsyncDisposable(
                        accountWithBalanceBinding,
                        accountWithBalanceJoinedCollection
                    )
                }
            );
        }
        private void ExchangeModelPropertyChangedAction(
            MyNotifyPropertyChangedArgs args
        )
        {
            BeginInvokeProper(async () =>
            {
                if (
                    args.PropertyName
                    == _exchangeModel.MyNameOfProperty(
                        _ => _.ExchangeServerConnected
                    )
                )
                {
                    var exchangeClientConnected 
                        = _exchangeModel.ExchangeServerConnected;
                    exchangeProfileNameLabel.ForeColor = 
                        exchangeClientConnected 
                            ? Color.Green 
                            : Color.Black;
                }
                else if (
                    args.PropertyName
                    == _exchangeModel.MyNameOfProperty(
                        _ => _.ExchangeClientProfileName
                    )
                )
                {
                    exchangeProfileNameLabel.Text
                        = _exchangeModel.ExchangeClientProfileName;
                }
                else if (
                    args.PropertyName
                    == _exchangeModel.MyNameOfProperty(
                        _ => _.SelectedAccountGuid
                    )
                )
                {
                    var selectedGuid = _exchangeModel.SelectedAccountGuid;
                    if (
                        selectedGuid == Guid.Empty
                        || !await _exchangeModel.SessionModel.Data.AccountTransferCollectionChangedDict
                            .WithAsyncLockSem(_ => _.ContainsKey(selectedGuid))
                        )
                    {
                        await _hotSwapObservableTransferCollection.ReplaceOriginCollectionChanged(
                            new MyObservableCollectionSafeAsyncImpl<ExchangeAccountTranferClientInfo>()
                            ).ConfigureAwait(false);
                    }
                    else
                    {
                        await _hotSwapObservableTransferCollection.ReplaceOriginCollectionChanged(
                            _exchangeModel.SessionModel.Data.AccountTransferCollectionChangedDict[selectedGuid]
                        ).ConfigureAwait(false);
                    }
                    /**/
                    if (
                        selectedGuid == Guid.Empty
                        || !await _exchangeModel.SessionModel.Data.AccountLockedFundsCollectionChangedDict
                            .WithAsyncLockSem(_ => _.ContainsKey(selectedGuid))
                        )
                    {
                        await _hotSwapObservableLockedFundsCollection.ReplaceOriginCollectionChanged(
                            new MyObservableCollectionSafeAsyncImpl<ExchangeAccountLockedFundsClientInfo>()
                        ).ConfigureAwait(false);
                    }
                    else
                    {
                        await _hotSwapObservableLockedFundsCollection.ReplaceOriginCollectionChanged(
                            _exchangeModel.SessionModel.Data.AccountLockedFundsCollectionChangedDict[selectedGuid]
                        ).ConfigureAwait(false);
                    }
                }
            });
        }
        /**/
        public static void FlattenListViewItems(
            ListView.ListViewItemCollection itemCollection,
            int totalCount,
            Func<ListViewItem> newItemConstructor
        )
        {
            Assert.NotNull(itemCollection);
            Assert.True(totalCount >= 0);
            Assert.NotNull(newItemConstructor);
            var initItemCollectionCount = itemCollection.Count;
            for (int i = 0, initCount = initItemCollectionCount - totalCount; i < initCount; i++)
            {
                itemCollection.RemoveAt(itemCollection.Count - 1);
            }
            for (int i = 0, initCount = totalCount - initItemCollectionCount; i < initCount; i++)
            {
                var newItem = newItemConstructor();
                Assert.NotNull(newItem);
                itemCollection.Add(newItem);
            }
        }
        
        private void ExchangeSessionModelDataPropertyChangedAction(
            MyNotifyPropertyChangedArgs args
        )
        {
            var sessionModelData = _exchangeModel.SessionModel.Data;
            BeginInvokeProper(() => {});
        }
    }
}
