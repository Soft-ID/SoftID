using System;
using System.Data.Common;
using System.Data;
using System.Threading.Tasks;
using System.Threading;

namespace SoftID.Data
{
    public partial class DbConnectionManager
    {
        public virtual object ExecuteScalar(string commandText)
        {
            return this.ExecuteScalar(commandText, _DefaultCommandType, (DbParameter[])null);
        }

        public virtual object ExecuteScalar(string commandText, params DbParameter[] parameters)
        {
            return this.ExecuteScalar(commandText, _DefaultCommandType, parameters);
        }

        public virtual object ExecuteScalar(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreateCommand(commandText, commandType, parameters))
            {
                try { return this.ExecuteScalar(command, false); }
                catch (Exception ex) { throw ex; }
                finally { command.Parameters.Clear(); }
            }
        }

        public virtual object ExecuteScalar(DbCommand command)
        {
            return ExecuteScalar(command, true);
        }

        protected virtual object ExecuteScalar(DbCommand command, bool cloneCommand)
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
                object value = command.ExecuteScalar();
                return value;
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

        public virtual Task<object> ExecuteScalarAsync(string commandText)
        {
            return this.ExecuteScalarAsync(commandText, _DefaultCommandType, (DbParameter[])null);
        }

        public virtual Task<object> ExecuteScalarAsync(string commandText, params DbParameter[] parameters)
        {
            return this.ExecuteScalarAsync(commandText, _DefaultCommandType, parameters);
        }

        public virtual Task<object> ExecuteScalarAsync(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreateCommand(commandText, commandType, parameters))
            {
                try { return this.ExecuteScalarAsync(command, false, new Nullable<CancellationToken>()); }
                catch (Exception ex) { throw ex; }
                finally { command.Parameters.Clear(); }
            }
        }

        public virtual Task<object> ExecuteScalarAsync(DbCommand command)
        {
            return ExecuteScalarAsync(command, true, new Nullable<CancellationToken>());
        }

        protected virtual async Task<object> ExecuteScalarAsync(DbCommand command, bool cloneCommand,
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
                object value = await (token.HasValue ? command.ExecuteScalarAsync(token.Value)
                    : command.ExecuteScalarAsync());
                return value;
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

        public virtual Task<object> ExecuteScalarAsync(string commandText, CancellationToken token)
        {
            return this.ExecuteScalarAsync(commandText, _DefaultCommandType, token, (DbParameter[])null);
        }

        public virtual Task<object> ExecuteScalarAsync(string commandText, CancellationToken token, params DbParameter[] parameters)
        {
            return this.ExecuteScalarAsync(commandText, _DefaultCommandType, token, parameters);
        }

        public virtual Task<object> ExecuteScalarAsync(string commandText, CommandType commandType,
            CancellationToken token, params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreateCommand(commandText, commandType, parameters))
            {
                try { return this.ExecuteScalarAsync(command, false, token); }
                catch (Exception ex) { throw ex; }
                finally { command.Parameters.Clear(); }
            }
        }

        public virtual Task<object> ExecuteScalarAsync(DbCommand command, CancellationToken token)
        {
            return ExecuteScalarAsync(command, true, token);
        }
    }
}