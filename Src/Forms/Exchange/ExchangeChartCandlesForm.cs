using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.BitMoneyClient.Lib.ExchangeServerSession;
using BtmI2p.GeneralClientInterfaces.ExchangeServer;
using BtmI2p.MiscClientForms;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;
using BtmI2p.MyNotifyPropertyChanged.MyObservableCollections;
using BtmI2p.ObjectStateLib;
using NLog;
using Xunit;
using ZedGraph;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
    public partial class ExchangeChartCandlesForm : Form
    {
        private readonly IExchangeServerSessionModelData _exchangeModelData;
        private readonly string _secCode;
        private MyHotSwapObservableCollectionImpl<ExchangeChartCandleClientInfo> _hotSwapObservableCollection;
        private MyNLastObservableCollectionImpl<ExchangeChartCandleClientInfo> _nLastObservableCollection;
        public ExchangeChartCandlesForm(
            IExchangeServerSessionModelData exchangeModelData,
            string secCode
        )
        {
            _exchangeModelData = exchangeModelData;
            _secCode = secCode;
            InitializeComponent();
        }

        private IEnumerable<Tuple<string, EExchangeChartTimeframe>> GetTimeframeDataSource()
        {
            return from EExchangeChartTimeframe timeframe 
                   in Enum.GetValues(typeof(EExchangeChartTimeframe))
                   select Tuple.Create($"{timeframe}",timeframe);
        }
        public static ExchangeChartCandlesFormDesignerLocStrings DesignerLocStrings = new ExchangeChartCandlesFormDesignerLocStrings();
        private void InitCommonView()
        {
            this.label1.Text = DesignerLocStrings.Label1Text;
            this.button1.Text = DesignerLocStrings.Button1Text;
            this.Text = DesignerLocStrings.Text;
            ClientGuiMainForm.ChangeControlFont(this, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
        }
        private async void ExchangeChartCandlesForm_Shown(object sender, EventArgs e)
        {
            _stateHelper.SetInitializedState();
            InitCommonView();
            /**/
            var firstCollection = _exchangeModelData.GetChartCandlesCollection(
                _secCode,
                _selectedTimeFrame
            );
            _hotSwapObservableCollection =
                await MyHotSwapObservableCollectionImpl<ExchangeChartCandleClientInfo>.CreateInstance(
                    firstCollection
                );
            _nLastObservableCollection =
                await MyNLastObservableCollectionImpl<ExchangeChartCandleClientInfo>.CreateInstance(
                    _hotSwapObservableCollection,
                    new MyObservableCollectionProxyN(200)
                );
            _zedGraphControlCollectionBinding = await ZedGraphCandlestickCollectionChangedOneWayBinding.CreateInstance(
                zedGraphControl1,
                _nLastObservableCollection,
                _secCode,
                _selectedTimeFrame
            );
            _asyncSubscriptions.Add(
                new CompositeMyAsyncDisposable(
                     _zedGraphControlCollectionBinding,
                    _nLastObservableCollection,
                    _hotSwapObservableCollection
                )
            );
            /**/
            Text += $" {_secCode}";
            var tfDs = GetTimeframeDataSource().ToList();
            comboBox1.DataSource = tfDs;
            comboBox1.DisplayMember = "Item1";
            comboBox1.ValueMember = "Item2";
            comboBox1.SelectedIndex = 0;
        }

        private Tuple<string, EExchangeChartTimeframe> _chartToDisplay = null;
        //private ChartCandlestickCollectionChangedOneWayBinding _chartCollectionBinding = null;
        private ZedGraphCandlestickCollectionChangedOneWayBinding _zedGraphControlCollectionBinding = null;
        private async void ExchangeChartCandlesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cts.Cancel();
            await _stateHelper.MyDisposeAsync();
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
            _subscriptions.Clear();
            foreach (var asyncSubscription in _asyncSubscriptions)
            {
                await asyncSubscription.MyDisposeAsync();
            }
            _asyncSubscriptions.Clear();
            /**/
            if (_chartToDisplay != null)
            {
                await _exchangeModelData.CandlesToUpdate.WithAsyncLockSem(
                    _ => _.Remove(_chartToDisplay)
                );
            }
            _cts.Dispose();
        }

        private EExchangeChartTimeframe _selectedTimeFrame = EExchangeChartTimeframe.M1;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClientGuiMainForm.BeginInvokeProper(
                this,
                _stateHelper,
                _log,
                async () =>
                {
                    _selectedTimeFrame = (EExchangeChartTimeframe) comboBox1.SelectedValue;
                    await _exchangeModelData.CandlesToUpdate.WithAsyncLockSem(
                        _ =>
                        {
                            if(_chartToDisplay != null)
                                _.Remove(_chartToDisplay);
                            _chartToDisplay = Tuple.Create(_secCode, _selectedTimeFrame);
                            _.Add(_chartToDisplay);
                        }
                    );
                    await _hotSwapObservableCollection.ReplaceOriginCollectionChanged(
                        _exchangeModelData.GetChartCandlesCollection(
                            _secCode,
                            _selectedTimeFrame
                        )
                    );
                    await _zedGraphControlCollectionBinding.ChangeTimeFrame(_selectedTimeFrame);
                    MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                        _exchangeModelData,
                        _ => _.CandlesToUpdate
                    );
                }
            );
        }
        
        private readonly DisposableObjectStateHelper _stateHelper
            = new DisposableObjectStateHelper("ExchangeChartCandlesForm");
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private readonly List<IMyAsyncDisposable> _asyncSubscriptions = new List<IMyAsyncDisposable>(); 
        /**/
        private class ZedGraphCandlestickCollectionChangedOneWayBinding : IMyAsyncDisposable
        {
            private ZedGraphCandlestickCollectionChangedOneWayBinding()
            {
            }

            private ZedGraphControl _zedGraphControl;
            private IMyNotifyCollectionChanged<ExchangeChartCandleClientInfo> _collection;

            private static void AlignPanes(ZedGraphControl z1)
            {
                Assert.NotNull(z1);
                var pane1 = z1.MasterPane[0];
                var pane2 = z1.MasterPane[1];
                using (var g = z1.CreateGraphics())
                {
                    pane2.Chart.Rect = pane2.CalcChartRect(g);
                }
                pane2.Chart.Rect = new RectangleF(
                    pane1.Chart.Rect.X,
                    pane2.Chart.Rect.Y,
                    pane1.Chart.Rect.Width,
                    pane2.Chart.Rect.Height
                );
            }

            public static void CustomAxisChange(ZedGraphControl z1)
            {
                Assert.NotNull(z1);
                z1.AxisChange();
                AlignPanes(z1);
                z1.Invalidate();
            }

            public async Task ChangeTimeFrame(EExchangeChartTimeframe newTimeframe)
            {
                await _zedGraphControl.InvokeAsync(
                    z1 =>
                    {
                        var myPane1 = z1.MasterPane.PaneList[0];
                        myPane1.Title.Text = $"{_secCode} {newTimeframe}";
                        var firstCurve = myPane1.CurveList[0];
                        firstCurve.Label.Text = $"{_secCode} {newTimeframe} {LocStrings.ChartLocStringsInstance.PriceSeriesName}";
                        /**/
                        var myPane2 = z1.MasterPane.PaneList[1];
                        var secondCurve = myPane2.CurveList[0];
                        secondCurve.Label.Text = $"{_secCode} {newTimeframe} {LocStrings.ChartLocStringsInstance.VolumeSeriesName}";
                        CustomAxisChange(z1);
                    }
                );
            }

            private string _secCode;
            public static async Task<ZedGraphCandlestickCollectionChangedOneWayBinding> CreateInstance(
                ZedGraphControl zedGraphControl,
                IMyNotifyCollectionChanged<ExchangeChartCandleClientInfo> collection,
                string secCode,
                EExchangeChartTimeframe timeframe
                )
            {
                Assert.NotNull(zedGraphControl);
                Assert.NotNull(collection);
                Assert.NotNull(secCode);
                var result = new ZedGraphCandlestickCollectionChangedOneWayBinding();
                result._secCode = secCode;
                result._zedGraphControl = zedGraphControl;
                result._collection = collection;
                await zedGraphControl.InvokeAsync(
                    z1 =>
                    {
                        var masterPane = z1.MasterPane;
                        masterPane.PaneList.Clear();
                        for (int i = 0; i < 2; i++)
                        {
                            var newGraphPane = new GraphPane();
                            newGraphPane.CurveList.Clear();
                            newGraphPane.XAxis.Type = ZedGraph.AxisType.Date;
                            masterPane.Add(newGraphPane);
                        }
                        using (var g = z1.CreateGraphics())
                        {
                            masterPane.SetLayout(
                                g,
                                true,
                                new [] {1, 1}, 
                                new [] {3f, 1f}
                            );
                        } 
                        z1.IsSynchronizeXAxes = true;
                        /**/
                        var myPane1 = masterPane.PaneList[0];
                        myPane1.IsFontsScaled = false;
                        myPane1.Title.Text = $"{secCode} {timeframe}";
                        myPane1.Title.IsVisible = true;
                        myPane1.XAxis.Title.IsVisible = true;
                        myPane1.XAxis.Title.Text = LocStrings.ChartLocStringsInstance.DateString;
                        myPane1.YAxis.Title.Text = LocStrings.ChartLocStringsInstance.PriceSeriesName;
                        myPane1.CurveList.Clear();
                        var myCurve = myPane1.AddJapaneseCandleStick(
                            $"{secCode} {timeframe} {LocStrings.ChartLocStringsInstance.PriceSeriesName}",
                            new StockPointList()
                        );
                        myCurve.Stick.IsAutoSize = true;
                        myCurve.Stick.Color = Color.Blue;
                        /**/
                        var myPane2 = masterPane.PaneList[1];
                        myPane2.IsFontsScaled = false;
                        myPane2.Title.IsVisible = false;
                        myPane2.XAxis.Title.IsVisible = false;
                        myPane2.YAxis.Title.Text = LocStrings.ChartLocStringsInstance.VolumeSeriesName;
                        //myPane2.YAxis.Scale.FontSpec.Size = myPane1.YAxis.Scale.FontSpec.Size*4;
                        var myVolumeCurve = myPane2.AddBar(
                            $"{secCode} {timeframe} {LocStrings.ChartLocStringsInstance.VolumeSeriesName}", 
                            new PointPairList(), 
                            Color.Green
                        );
                        foreach (var myPaneI in new [] {myPane1,myPane2})
                        {
                            myPaneI.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);
                            myPaneI.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45.0f);
                        }
                        CustomAxisChange(z1);
                    }
                );
                result._stateHelper.SetInitializedState();
                result._subscriptions.Add(
                    collection.CollectionChangedObservable.Subscribe(
                        _ =>
                        {
                            if (result._changedArgsDict.TryAdd(_.ChangesNum, _))
                                result.ProcessNewChangedCandles();
                        }
                    )
                );
                var deepCopyArgs = await collection.GetDeepCopyAsync().ConfigureAwait(false);
                result._changedArgsDict[deepCopyArgs.ChangesNum] = deepCopyArgs;
                Interlocked.Exchange(
                    ref result._prevChangesCounter,
                    deepCopyArgs.ChangesNum - 1
                );
                result.ProcessNewChangedCandles();
                return await Task.FromResult(result).ConfigureAwait(false);
            }
            private readonly SemaphoreSlim _processNewChangedCandlesLockSem = new SemaphoreSlim(1);

            private async void ProcessNewChangedCandles()
            {
                try
                {
                    using (_stateHelper.GetFuncWrapper())
                    {
                        var lockSemCalledWrapper = _processNewChangedCandlesLockSem.GetCalledWrapper();
                        lockSemCalledWrapper.Called = true;
                        using (await _processNewChangedCandlesLockSem.GetDisposable(true).ConfigureAwait(false))
                        {
                            while (!_cts.IsCancellationRequested && lockSemCalledWrapper.Called)
                            {
                                lockSemCalledWrapper.Called = false;
                                var prevChangesNum = Interlocked.Read(ref _prevChangesCounter);
                                var currentChangesNumInDictToRemove =
                                    _changedArgsDict.Keys.Where(_ => _ <= prevChangesNum).ToList();
                                foreach (long key in currentChangesNumInDictToRemove)
                                {
                                    MyNotifyCollectionChangedArgs<ExchangeChartCandleClientInfo> removedArgs;
                                    _changedArgsDict.TryRemove(key, out removedArgs);
                                }
                                MyNotifyCollectionChangedArgs<ExchangeChartCandleClientInfo> nextArgs;
                                if (
                                    _changedArgsDict.TryGetValue(
                                        prevChangesNum + 1,
                                        out nextArgs
                                        )
                                    )
                                {
                                    lockSemCalledWrapper.Called = true;
                                    if (nextArgs.ChangedAction == EMyCollectionChangedAction.Reset)
                                    {
                                        await _zedGraphControl.InvokeAsync(
                                            z1 =>
                                            {
                                                z1.SuspendLayout();
                                                try
                                                {
                                                    var myPane1 = z1.MasterPane[0];
                                                    myPane1.CurveList[0].Clear();
                                                    var myPane2 = z1.MasterPane[1];
                                                    myPane2.CurveList[0].Clear();
                                                }
                                                finally
                                                {
                                                    z1.ResumeLayout();
                                                }
                                            }
                                        ).ConfigureAwait(false);
                                        var currentDataCopy = await _collection.GetDeepCopyAsync(
                                            ).ConfigureAwait(false);
                                        _changedArgsDict[currentDataCopy.ChangesNum] = currentDataCopy;
                                        Interlocked.Exchange(
                                            ref _prevChangesCounter,
                                            currentDataCopy.ChangesNum - 1
                                        );
                                    }
                                    else if (nextArgs.ChangedAction == EMyCollectionChangedAction.NewItemsRangeInserted)
                                    {
                                        var insertIndex = nextArgs.NewItemsIndices[0];
                                        var insertCount = nextArgs.NewItems.Count;
                                        if (insertCount > 0)
                                        {
                                            await _zedGraphControl.InvokeAsync(
                                                () =>
                                                {
                                                    _zedGraphControl.SuspendLayout();
                                                    try
                                                    {
                                                        var myPane1 = _zedGraphControl.MasterPane[0];
                                                        var spl = (StockPointList)myPane1.CurveList[0].Points;
                                                        var myPane2 = _zedGraphControl.MasterPane[1];
                                                        var pairPl = (PointPairList) myPane2.CurveList[0].Points;
                                                        for (int i = 0; i < nextArgs.NewItems.Count; i++)
                                                        {
                                                            var candle = nextArgs.NewItems[i];
                                                            var xDate = new XDate(candle.StartTime);
                                                            var pt = new StockPt(
                                                                xDate,
                                                                (double) candle.HighValue,
                                                                (double) candle.LowValue,
                                                                (double) candle.OpenValue,
                                                                (double) candle.CloseValue,
                                                                candle.TotalVolumeLots
                                                                );
                                                            spl.Insert(
                                                                insertIndex + i,
                                                                pt
                                                                );
                                                            var dpt = new PointPair(
                                                                xDate,
                                                                candle.TotalVolumeLots
                                                            );
                                                            pairPl.Insert(
                                                                insertIndex + i,
                                                                dpt
                                                            );
                                                        }
                                                    }
                                                    finally
                                                    {
                                                        _zedGraphControl.ResumeLayout();
                                                    }
                                                }
                                            ).ConfigureAwait(false);
                                        }
                                        Interlocked.Exchange(ref _prevChangesCounter, prevChangesNum + 1);
                                    }
                                    else if (nextArgs.ChangedAction == EMyCollectionChangedAction.ItemsRangeRemoved)
                                    {
                                        var removeIndexStart = nextArgs.OldItemsIndices[0];
                                        var removeCount = nextArgs.OldItems.Count;
                                        if (removeCount > 0)
                                        {
                                            await _zedGraphControl.InvokeAsync(
                                                z1 =>
                                                {
                                                    z1.SuspendLayout();
                                                    try
                                                    {
                                                        var myPane1 = z1.MasterPane[0];
                                                        var curve = myPane1.CurveList[0];
                                                        var spl = (StockPointList)curve.Points;
                                                        spl.RemoveRange(removeIndexStart,removeCount);
                                                        /**/
                                                        var myPane2 = z1.MasterPane[1];
                                                        var ppl = (PointPairList) myPane2.CurveList[0].Points;
                                                        ppl.RemoveRange(removeIndexStart, removeCount);
                                                    }
                                                    finally
                                                    {
                                                        z1.ResumeLayout();
                                                    }
                                                }
                                            ).ConfigureAwait(false);
                                        }
                                        Interlocked.Exchange(ref _prevChangesCounter, prevChangesNum + 1);
                                    }
                                    else if (nextArgs.ChangedAction == EMyCollectionChangedAction.ItemsChanged)
                                    {
                                        var count = nextArgs.NewItems.Count;
                                        if (count > 0)
                                        {
                                            await _zedGraphControl.InvokeAsync(
                                                z1 =>
                                                {
                                                    z1.SuspendLayout();
                                                    try
                                                    {
                                                        var myPane1 = z1.MasterPane[0];
                                                        var curve = myPane1.CurveList[0];
                                                        var spl = (StockPointList) curve.Points;
                                                        var myPane2 = z1.MasterPane[1];
                                                        var ppl = (PointPairList)myPane2.CurveList[0].Points;
                                                        for (int i = 0; i < count; i++)
                                                        {
                                                            var newIndex = nextArgs.NewItemsIndices[i];
                                                            var candle = nextArgs.NewItems[i];
                                                            var xDate = new XDate(candle.StartTime);
                                                            var pt = new StockPt(
                                                                xDate,
                                                                (double) candle.HighValue,
                                                                (double) candle.LowValue,
                                                                (double) candle.OpenValue,
                                                                (double) candle.CloseValue,
                                                                candle.TotalVolumeLots
                                                            );
                                                            spl[newIndex] = pt;
                                                            ppl[newIndex] = new PointPair(
                                                                xDate,
                                                                candle.TotalVolumeLots
                                                            );
                                                        }
                                                    }
                                                    finally
                                                    {
                                                        z1.ResumeLayout();
                                                    }
                                                }
                                            ).ConfigureAwait(false);
                                        }
                                        Interlocked.Exchange(ref _prevChangesCounter, prevChangesNum + 1);
                                    }
                                    else
                                    {
                                        throw new NotImplementedException();
                                    }
                                    await _zedGraphControl.InvokeAsync(
                                        z1 =>
                                        {
                                            CustomAxisChange(z1);
                                        }
                                    );
                                }
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (WrongDisposableObjectStateException)
                {
                }
                catch (Exception exc)
                {
                    MiscFuncs.HandleUnexpectedError(exc, _log);
                }
            }

            private readonly Logger _log = LogManager.GetCurrentClassLogger();
            private long _prevChangesCounter = -2;
            private readonly ConcurrentDictionary<long, MyNotifyCollectionChangedArgs<ExchangeChartCandleClientInfo>> _changedArgsDict
                = new ConcurrentDictionary<long, MyNotifyCollectionChangedArgs<ExchangeChartCandleClientInfo>>();
            private readonly DisposableObjectStateHelper _stateHelper
                = new DisposableObjectStateHelper("ZedGraphCandlestickCollectionChangedOneWayBinding");
            private readonly List<IDisposable> _subscriptions
                = new List<IDisposable>();
            private readonly CancellationTokenSource _cts = new CancellationTokenSource();
            public async Task MyDisposeAsync()
            {
                _cts.Cancel();
                await _stateHelper.MyDisposeAsync().ConfigureAwait(false);
                foreach (var subscription in _subscriptions)
                {
                    subscription.Dispose();
                }
                _subscriptions.Clear();
                _cts.Dispose();
            }
        }
        public static ExchangeChartCandlesFormLocStrings LocStrings 
            = new ExchangeChartCandlesFormLocStrings();
        //CSV export
        private void button1_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                async () =>
                {
                    var initFileName = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        $"candles_{Guid.NewGuid().ToString().Substring(0, 6)}.csv"
                    );
                    var fileNameForm = new InputBoxForm(
                        LocStrings.CsvExportLocStringsInstance.InputFilenameCaption,
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

                    var tf = _selectedTimeFrame;
                    var data = (await _exchangeModelData.GetChartCandlesCollection(
                        _secCode,
                        tf
                    ).GetDeepCopyAsync()).NewItems;
                    using (var myStream = new FileStream(fileName, FileMode.Create))
                    {
                        using (var textWriter = new StreamWriter(myStream, Encoding.UTF8))
                        {
                            await textWriter.WriteLineAsync(
                                $"{_secCode} {tf} CANDLE_TIME(yyyyMMddHHmm);HIGH;LOW;OPEN;CLOSE;VOLUME_LOTS"
                            ).ConfigureAwait(false);
                            foreach (var candleInfo in data)
                            {
                                await textWriter.WriteLineAsync(
                                    $"{candleInfo.StartTime:yyyyMMddHHmm}" +
                                    $";{candleInfo.HighValue:G29}" +
                                    $";{candleInfo.LowValue:G29}" +
                                    $";{candleInfo.OpenValue:G29}" +
                                    $";{candleInfo.CloseValue:G29}" +
                                    $";{candleInfo.TotalVolumeLots}");
                            }
                        }
                    }
                    ClientGuiMainForm.ShowInfoMessage(
                        this,
                        LocStrings.CsvExportLocStringsInstance.CsvExportComplete
                    );
                },
                _stateHelper,
                _log
            );
        }

        private void zedGraphControl1_Resize(object sender, EventArgs e)
        {
            
        }

        private void ExchangeChartCandlesForm_ResizeEnd(object sender, EventArgs e)
        {
            ZedGraphCandlestickCollectionChangedOneWayBinding.CustomAxisChange(
                zedGraphControl1
            );
        }
    }

    public class ExchangeChartCandlesFormLocStrings
    {
        public class MessagesLocStrings
        {
        }
        public MessagesLocStrings Messages = new MessagesLocStrings();
        /**/
        public class ChartLocStrings
        {
            public string PriceSeriesName = "Price";
            public string VolumeSeriesName = "Volume";
            public string LegendCaption = "";
            public string DateString = "Date";
        }
        public ChartLocStrings ChartLocStringsInstance = new ChartLocStrings();
        /**/
        public class CsvExportLocStrings
        {
            public string InputFilenameCaption = "Input filename";
            public string CsvExportComplete = "Export complete";
        }
        public CsvExportLocStrings CsvExportLocStringsInstance = new CsvExportLocStrings();
        /**/
    }
    public class ExchangeChartCandlesFormDesignerLocStrings
    {
        public string Label1Text = "Timeframe";
        public string Button1Text = "Export candles data";
        public string Text = "Chart";
    }
}