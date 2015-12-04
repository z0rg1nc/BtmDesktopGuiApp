using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BtmI2p.BitMoneyClient.Lib.WalletServerSession;
using BtmI2p.Community.CsharpSqlite.SQLiteClient;
using BtmI2p.Dapper.SqlCrudExtensions.Base;
using BtmI2p.Dapper.SqlCrudExtensions.Sqlite;
using BtmI2p.MiscUtils;
using BtmI2p.ObjectStateLib;
using Dapper;
using NLog;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Communication.Wallet
{
    public class WalletTransferHistory : IMyAsyncDisposable
    {
        private WalletTransferHistory()
        {
        }
        [TableHint("transfers")]
        private class TransferDbInfo : ClientTransferBase, ISqlitePOCO
        {
            [ColumnHint("transfer_num")]
            public override int TransferNum { get; set; }
            [ColumnHint("transfer_guid")]
            public override Guid TransferGuid { get; set; }
            [ColumnHint("request_guid")]
            public override Guid RequestGuid { get; set; } = Guid.Empty;
            [ColumnHint("amount")]
            public override long Amount { get; set; }
            [ColumnHint("fee")]
            public override long Fee { get; set; } = 0;
            [ColumnHint("sent_time")]
            public override DateTime SentTime { get; set; }
            [ColumnHint("comment_bytes")]
            public override byte[] CommentBytes { get; set; } = new byte[0];
            [ColumnHint("anonymous")]
            public override bool AnonymousTransfer { get; set; }
            [ColumnHint("wallet_from")]
            public override Guid WalletFrom { get; set; }
            [ColumnHint("wallet_to")]
            public override Guid WalletTo { get; set; }
            [ColumnHint("outcome_transfer")]
            public override bool OutcomeTransfer { get; set; } = false;
            [ColumnHint("auth_other_wallet_cert")]
            public override bool AuthenticatedOtherWalletCert { get; set; } = false;
            [ColumnHint("auth_key")]
            public override bool AuthenticatedCommentKey { get; set; } = false;
            [ColumnHint("auth_details")]
            public override bool AuthenticatedTransferDetails { get; set; } = false;
            /**/
            public static TransferDbInfo FromClientInfo(ClientTransferBase tr)
            {
                Assert.NotNull(tr);
                return new TransferDbInfo()
                {
                    WalletTo = tr.WalletTo,
                    AnonymousTransfer = tr.AnonymousTransfer,
                    TransferGuid = tr.TransferGuid,
                    WalletFrom = tr.WalletFrom,
                    RequestGuid = tr.RequestGuid,
                    AuthenticatedOtherWalletCert = tr.AuthenticatedOtherWalletCert,
                    AuthenticatedTransferDetails = tr.AuthenticatedTransferDetails,
                    AuthenticatedCommentKey = tr.AuthenticatedCommentKey,
                    SentTime = tr.SentTime,
                    Amount = tr.Amount,
                    TransferNum = tr.TransferNum,
                    OutcomeTransfer = tr.OutcomeTransfer,
                    Fee = tr.Fee,
                    CommentBytes = tr.CommentBytes  
                };
            }
        }

        private void CreateTables()
        {
            var tDb = SqliteCrudExtensions.GetDbInfo<TransferDbInfo>();
            _conn.Execute(
                $"create table {tDb.TableName} (" +
                    $"{tDb.ColumnNameWithoutTable(_ => _.TransferNum)} int primary key not null" +
                    $", {tDb.ColumnNameWithoutTable(_ => _.TransferGuid)} guid" +
                    $", {tDb.ColumnNameWithoutTable(_ => _.RequestGuid)} guid" +
                    $", {tDb.ColumnNameWithoutTable(_ => _.Amount)} bigint" +
                    $", {tDb.ColumnNameWithoutTable(_ => _.Fee)} bigint" +
                    $", {tDb.ColumnNameWithoutTable(_ => _.SentTime)} datetime" +
                    $", {tDb.ColumnNameWithoutTable(_ => _.CommentBytes)} blob" +
                    $", {tDb.ColumnNameWithoutTable(_ => _.AnonymousTransfer)} boolean" +
                    $", {tDb.ColumnNameWithoutTable(_ => _.WalletFrom)} guid" +
                    $", {tDb.ColumnNameWithoutTable(_ => _.WalletTo)} guid" +
                    $", {tDb.ColumnNameWithoutTable(_ => _.OutcomeTransfer)} boolean" +
                    $", {tDb.ColumnNameWithoutTable(_ => _.AuthenticatedOtherWalletCert)} boolean" +
                    $", {tDb.ColumnNameWithoutTable(_ => _.AuthenticatedCommentKey)} boolean" +
                    $", {tDb.ColumnNameWithoutTable(_ => _.AuthenticatedTransferDetails)} boolean" +
                    $", UNIQUE({tDb.ColumnNameWithoutTable(_ => _.TransferGuid)}, {tDb.ColumnNameWithoutTable(_ => _.OutcomeTransfer)})" +
                ");"
            );
            _conn.Execute(
                $"create index transfers_wallet_from_index on {tDb.TableName} ({tDb.ColumnNameWithoutTable(_ => _.WalletFrom)});"
            );
            _conn.Execute(
                $"create index transfers_wallet_to_index on {tDb.TableName} ({tDb.ColumnNameWithoutTable(_ => _.WalletTo)});"
            );
            _conn.Execute(
                $"create index transfers_sent_time_index on {tDb.TableName} ({tDb.ColumnNameWithoutTable(_ => _.SentTime)});"
            );
        }
        public async Task<List<ClientTransferBase>> GetLastTransfers(
            int count = 100
        )
        {
            using (_stateHelper.GetFuncWrapper())
            {
                using (await _dbLockSem.GetDisposable().ConfigureAwait(false))
                {
                    var tDb = SqliteCrudExtensions.GetDbInfo<TransferDbInfo>();
                    return (await _conn.QueryAsync<TransferDbInfo>(
                        $"select {tDb.SelectString} from {tDb.TableName}" +
                        $" order by {tDb[_ => _.SentTime]} desc, {tDb[_ => _.TransferNum]} desc" +
                        " limit :TransferCount;",
                        new
                        {
                            TransferCount = count
                        }
                    ).ConfigureAwait(false)).Select(_ => (ClientTransferBase)_).ToList();
                }
            }
        }

        public async Task AddTransfers(
            List<ClientTransferBase> transfers
        )
        {
            using (_stateHelper.GetFuncWrapper())
            {
                using (await _dbLockSem.GetDisposable().ConfigureAwait(false))
                {
                    using (var trans = _conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var transfer in transfers)
                            {
                                await _conn.InsertAsync(
                                    TransferDbInfo.FromClientInfo(transfer),
                                    trans,
                                    true
                                    ).ConfigureAwait(false);
                            }
                            trans.Commit();
                        }
                        catch (Exception exc)
                        {
                            MiscFuncs.HandleUnexpectedError(exc,_log);
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        public async Task<Guid> GetLastKnownTransferGuid(
            bool outcomeTransfer = true
        )
        {
            using (_stateHelper.GetFuncWrapper())
            {
                using (await _dbLockSem.GetDisposable().ConfigureAwait(false))
                {
                    var tDb = SqliteCrudExtensions.GetDbInfo<TransferDbInfo>();
                    return (await _conn.QueryAsync<Guid>(
                        $"select {tDb[_ => _.TransferGuid]} from {tDb.TableName}" +
                        $" where {tDb[_ => _.OutcomeTransfer]}= :OutcomeTransfer" +
                        $" order by {tDb[_ => _.SentTime]} desc, {tDb[_ => _.TransferNum]} desc" +
                        " limit 1;",
                        new
                        {
                            OutcomeTransfer = outcomeTransfer
                        }
                    ).ConfigureAwait(false)).FirstOrDefault();
                }
            }
        }

        public async Task<int> GetInitCounter()
        {
            using (_stateHelper.GetFuncWrapper())
            {
                using (await _dbLockSem.GetDisposable().ConfigureAwait(false))
                {
                    var tDb = SqliteCrudExtensions.GetDbInfo<TransferDbInfo>();
                    return await _conn.ExecuteScalarAsync<int?>(
                        $"select max({tDb[_ => _.TransferNum]})+1 from {tDb.TableName};"
                    ).ConfigureAwait(false) ?? 1;
                }
            }
        }
        /* Db in memory */
        public static WalletTransferHistory CreateInstance()
        {
            var result = new WalletTransferHistory();
            const string connectionString = "Data Source=file://:memory:,Version=3";
            result._conn = new SqliteConnection(connectionString);
            result._conn.Open();
            result.CreateTables();
            result._stateHelper.SetInitializedState();
            return result;
        }
        public static async Task<WalletTransferHistory> CreateInstance(
            string filename, byte[] password = null
        )
        {
            if (password != null)
            {
                if (password.Length != 16)
                    throw new ArgumentOutOfRangeException(
                        MyNameof.GetLocalVarName(() => password));
            }
            var result = new WalletTransferHistory();
            bool createTable = !File.Exists(filename);
            var connectionString
                = $"Data Source=file://{Path.GetFullPath(filename)},Version=3";
            if (password != null)
            {
                if (password.Length != 16)
                    throw new ArgumentOutOfRangeException(
                        MyNameof.GetLocalVarName(() => password));
                connectionString +=
                    $",Password=0x{BitConverter.ToString(password).Replace("-", string.Empty)}";
            }
            result._conn = new SqliteConnection(connectionString);
            result._conn.Open();
            if (createTable)
                result.CreateTables();
            result._stateHelper.SetInitializedState();
            return await Task.FromResult(result).ConfigureAwait(false);
        }
        private readonly SemaphoreSlim _dbLockSem = new SemaphoreSlim(1);
        private SqliteConnection _conn;
        private readonly DisposableObjectStateHelper _stateHelper
            = new DisposableObjectStateHelper("WalletTransferHistory");
        private static readonly Logger _log 
            = LogManager.GetCurrentClassLogger();
        public async Task MyDisposeAsync()
        {
            await _stateHelper.MyDisposeAsync().ConfigureAwait(false);
            _conn.Close();
        }
    }
}
