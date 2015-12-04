using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BtmI2p.BitMoneyClient.Lib.MessageServerSession;
using BtmI2p.Community.CsharpSqlite.SQLiteClient;
using BtmI2p.Dapper.SqlCrudExtensions.Base;
using BtmI2p.Dapper.SqlCrudExtensions.Sqlite;
using BtmI2p.MiscUtils;
using BtmI2p.ObjectStateLib;
using Dapper;
using NLog;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Communication.Messages
{
    public class MessageHistory : IMyAsyncDisposable
    {
        private MessageHistory()
        {
        }
        [TableHint("messages")]
        private class ClientTextMessageDbInfo : ClientTextMessage, ISqlitePOCO
        {
            public static ClientTextMessageDbInfo From(ClientTextMessage copyFrom)
            {
                Assert.NotNull(copyFrom);
                return new ClientTextMessageDbInfo()
                {
                    MessageGuid = copyFrom.MessageGuid,
                    MessageAuthenticated = copyFrom.MessageAuthenticated,
                    OtherUserCertAuthenticated = copyFrom.OtherUserCertAuthenticated,
                    MessageKeyAuthenticated = copyFrom.MessageKeyAuthenticated,
                    SaveUntil = copyFrom.SaveUntil,
                    UserTo = copyFrom.UserTo,
                    UserFrom = copyFrom.UserFrom,
                    SentTime = copyFrom.SentTime,
                    MessageNum = copyFrom.MessageNum,
                    MessageText = copyFrom.MessageText,
                    OutcomeMessage = copyFrom.OutcomeMessage
                };
            }
            [ColumnHint("message_guid")]
            public override Guid MessageGuid { get; set; }
            [ColumnHint("message_num")]
            public override int MessageNum { get; set; }
            [ColumnHint("message_cached_utf8_text")]
            public override string MessageText { get; set; }
            [ColumnHint("sent_time")]
            public override DateTime SentTime { get; set; }
            [ColumnHint("save_until")]
            public override DateTime SaveUntil { get; set; }
            [ColumnHint("user_from")]
            public override Guid UserFrom { get; set; }
            [ColumnHint("user_to")]
            public override Guid UserTo { get; set; }
            [ColumnHint("outcome_message")]
            public override bool OutcomeMessage { get; set; } = false;
            [ColumnHint("auth_other_user_cert")]
            public override bool OtherUserCertAuthenticated { get; set; } = false;
            [ColumnHint("auth_message_key")]
            public override bool MessageKeyAuthenticated { get; set; } = false;
            [ColumnHint("auth_message")]
            public override bool MessageAuthenticated { get; set; } = false;
        }
        private void CreateTables()
        {
            var mDb = SqliteCrudExtensions.GetDbInfo<ClientTextMessageDbInfo>();
            _conn.Execute(
                $"create table {mDb.TableName} (" +
                    $"{mDb.ColumnNameWithoutTable(_ => _.MessageNum)} int primary key not null" +
                    $", {mDb.ColumnNameWithoutTable(_ => _.MessageGuid)} guid" +
                    $", {mDb.ColumnNameWithoutTable(_ => _.SentTime)} datetime" +
                    $", {mDb.ColumnNameWithoutTable(_ => _.SaveUntil)} datetime" +
                    $", {mDb.ColumnNameWithoutTable(_ => _.OtherUserCertAuthenticated)} boolean" +
                    $", {mDb.ColumnNameWithoutTable(_ => _.MessageKeyAuthenticated)} boolean" +
                    $", {mDb.ColumnNameWithoutTable(_ => _.MessageAuthenticated)} boolean" +
                    $", {mDb.ColumnNameWithoutTable(_ => _.UserFrom)} guid" +
                    $", {mDb.ColumnNameWithoutTable(_ => _.UserTo)} guid" +
                    $", {mDb.ColumnNameWithoutTable(_ => _.OutcomeMessage)} boolean" +
                    $", {mDb.ColumnNameWithoutTable(_ => _.MessageText)} text" +
                    $", UNIQUE({mDb.ColumnNameWithoutTable(_ => _.MessageGuid)}, {mDb.ColumnNameWithoutTable(_ => _.OutcomeMessage)})" +
                ");"
            );
            _conn.Execute(
                $"create index messages_user_from_index on {mDb.TableName} ({mDb.ColumnNameWithoutTable(_ => _.UserFrom)});"
            );
            _conn.Execute(
                $"create index messages_user_to_index on {mDb.TableName} ({mDb.ColumnNameWithoutTable(_ => _.UserTo)});"
            );
            _conn.Execute(
                $"create index messages_sent_time_index on {mDb.TableName} ({mDb.ColumnNameWithoutTable(_ => _.SentTime)});"
            );
        }

        public async Task<List<ClientTextMessage>> GetLastTextMessages(
            Guid userGuid,
            Guid myGuid,
            int count = 100
        )
        {
            using (_stateHelper.GetFuncWrapper())
            {
                using (await _dbLockSem.GetDisposable().ConfigureAwait(false))
                {
                    var messageDbTypeInfo = SqliteCrudExtensions.GetDbInfo<ClientTextMessageDbInfo>();
                    return (await _conn.QueryAsync<ClientTextMessageDbInfo>(
                        $"select {messageDbTypeInfo.SelectString}" +
                        $" from {messageDbTypeInfo.TableName}" +
                        " where" +
                            $" (" +
                                $"{messageDbTypeInfo[_ => _.UserFrom]}=:UserGuid" +
                                $" and {messageDbTypeInfo[_ => _.UserTo]}=:UserMyGuid" +
                            $") or (" +
                                $"{messageDbTypeInfo[_ => _.UserTo]}=:UserGuid" +
                                $" and {messageDbTypeInfo[_ => _.UserFrom]}=:UserMyGuid" +
                            $")" +
                        $" order by" +
                            $" {messageDbTypeInfo[_ => _.SentTime]} desc" +
                            $", {messageDbTypeInfo[_ => _.MessageNum]} desc" +
                        " limit :MessageCount;",
                        new
                        {
                            UserGuid = userGuid,
                            UserMyGuid = myGuid,
                            MessageCount = count
                        }
                    ).ConfigureAwait(false)).Select(_ => (ClientTextMessage)_).ToList();
                }
            }
        }
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        public async Task AddTextMessages(
            List<ClientTextMessage> textMessages
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
                            foreach (var message in textMessages)
                            {
                                await _conn.InsertAsync(
                                    ClientTextMessageDbInfo.From(message),
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

        public async Task<Guid> GetLastKnownMessageGuid(bool outcomeMessage = true)
        {
            using (_stateHelper.GetFuncWrapper())
            {
                using (await _dbLockSem.GetDisposable().ConfigureAwait(false))
                {
                    var mDb = SqliteCrudExtensions.GetDbInfo<ClientTextMessageDbInfo>();
                    return (
                        await _conn.QueryAsync<Guid>(
                            $"select {mDb[_ => _.MessageGuid]} from {mDb.TableName}" +
                            $" where {mDb[_ => _.OutcomeMessage]}= :OutcomeMessage" +
                            $" order by {mDb[_ => _.SentTime]} desc, {mDb[_ => _.MessageNum]} desc" +
                            " limit 1;",
                            new
                            {
                                OutcomeMessage = outcomeMessage
                            }
                        ).ConfigureAwait(false)
                    ).FirstOrDefault();
                }
            }
        }

        public async Task<int> GetInitCounter()
        {
            using (_stateHelper.GetFuncWrapper())
            {
                using (await _dbLockSem.GetDisposable().ConfigureAwait(false))
                {
                    var mDb = SqliteCrudExtensions.GetDbInfo<ClientTextMessageDbInfo>();
                    return (await _conn.ExecuteScalarAsync<int?>(
                        $"select max({mDb[_ => _.MessageNum]}) from {mDb.TableName};"
                    ).ConfigureAwait(false) ?? 0) + 1;
                }
            }
        }
        
        //Db in memory
        public static MessageHistory CreateInstance()
        {
            var result = new MessageHistory();
            const string connectionString = "Data Source=file://:memory:,Version=3";
            result._conn = new SqliteConnection(connectionString);
            result._conn.Open();
            result.CreateTables();
            result._stateHelper.SetInitializedState();
            return result;
        }

        public static async Task<MessageHistory> CreateInstance(
            string filename, byte[] password = null
        )
        {
            var result = new MessageHistory();
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
            return await Task.FromResult(result);
        }
        private readonly SemaphoreSlim _dbLockSem = new SemaphoreSlim(1);
        private SqliteConnection _conn;
        private readonly DisposableObjectStateHelper _stateHelper
            = new DisposableObjectStateHelper("MessageHistory");
        public async Task MyDisposeAsync()
        {
            await _stateHelper.MyDisposeAsync().ConfigureAwait(false);
            _conn.Close();
        }
    }
}
