using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Threading;

namespace SoftID.Data
{
    public partial class DbConnectionManager
    {
        public virtual int ExecuteNonQuery(string commandText)
        {
            return this.ExecuteNonQuery(commandText, _DefaultCommandType, (DbParameter[])null);
        }

        public virtual int ExecuteNonQuery(string commandText, params DbParameter[] parameters)
        {
            return this.ExecuteNonQuery(commandText, _DefaultCommandType, parameters);
        }

        public virtual int ExecuteNonQuery(string commandText, CommandType commandType,
            params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreateCommand(commandText, commandType, parameters))
            {
                try { return this.ExecuteNonQuery(command, false); }
                catch (Exception ex) { throw ex; }
                finally { command.Parameters.Clear(); }
            }
        }

        public virtual int ExecuteNonQuery(DbCommand command)
        {
            return ExecuteNonQuery(command, true);
        }

        protected virtual int ExecuteNonQuery(DbCommand command, bool cloneCommand)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (cloneCommand)
                command = this.CloneCommand(command);
            command.Connection = this._Connection;
            command.Transaction = this._Transaction;
            command.CommandTimeout = this._Connection.ConnectionTimeout;
            try
            {
                if (_IsClosed)
                    this._Connection.Open();
                int aff = command.ExecuteNonQuery();
                return aff;
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToLower();
                throw msg.Contains("duplicate key") ? new DuplicateKeyException(ex.Message, ex) : ex;
            }
            finally
            {
                if (_IsClosed)
                    this._Connection.Close();
                command.Transaction = null;
                command.Connection = null;
                if (cloneCommand)
                    command.Dispose();
            }
        }

        public virtual Task<int> ExecuteNonQueryAsync(string commandText)
        {
            return this.ExecuteNonQueryAsync(commandText, _DefaultCommandType, (DbParameter[])null);
        }

        public virtual Task<int> ExecuteNonQueryAsync(string commandText, params DbParameter[] parameters)
        {
            return this.ExecuteNonQueryAsync(commandText, _DefaultCommandType, parameters);
        }

        public virtual Task<int> ExecuteNonQueryAsync(string commandText, CommandType commandType,
            params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreateCommand(commandText, commandType, parameters))
            {
                try { return this.ExecuteNonQueryAsync(command, false, new Nullable<CancellationToken>()); }
                catch (Exception ex) { throw ex; }
                finally { command.Parameters.Clear(); }
            }
        }

        public virtual Task<int> ExecuteNonQueryAsync(DbCommand command)
        {
            return ExecuteNonQueryAsync(command, true, new Nullable<CancellationToken>());
        }

        protected virtual async Task<int> ExecuteNonQueryAsync(DbCommand command, bool cloneCommand,
            Nullable<CancellationToken> token)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (cloneCommand)
                command = this.CloneCommand(command);
            command.Connection = this._Connection;
            command.Transaction = this._Transaction;
            command.CommandTimeout = this._Connection.ConnectionTimeout;
            try
            {
                if (_IsClosed)
                    this._Connection.Open();
                int aff = await (token.HasValue ? command.ExecuteNonQueryAsync(token.Value)
                    : command.ExecuteNonQueryAsync());
                return aff;
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToLower();
                throw msg.Contains("duplicate key") ? new DuplicateKeyException(ex.Message, ex) : ex;
            }
            finally
            {
                if (_IsClosed)
                    this._Connection.Close();
                command.Transaction = null;
                command.Connection = null;
                if (cloneCommand)
                    command.Dispose();
            }
        }

        public virtual Task<int> ExecuteNonQueryAsync(string commandText, CancellationToken token)
        {
            return this.ExecuteNonQueryAsync(commandText, _DefaultCommandType, token, (DbParameter[])null);
        }

        public virtual Task<int> ExecuteNonQueryAsync(string commandText, CancellationToken token, params DbParameter[] parameters)
        {
            return this.ExecuteNonQueryAsync(commandText, _DefaultCommandType, token, parameters);
        }

        public virtual Task<int> ExecuteNonQueryAsync(string commandText, CommandType commandType,
            CancellationToken token, params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreateCommand(commandText, commandType, parameters))
            {
                try { return this.ExecuteNonQueryAsync(command, false, token); }
                catch (Exception ex) { throw ex; }
                finally { command.Parameters.Clear(); }
            }
        }

        public virtual Task<int> ExecuteNonQueryAsync(DbCommand command, CancellationToken token)
        {
            return ExecuteNonQueryAsync(command, true, token);
        }
    }
}