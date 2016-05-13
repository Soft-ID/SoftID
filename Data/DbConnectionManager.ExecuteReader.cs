using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SoftID.Data
{
    public partial class DbConnectionManager
    {
        public virtual DbDataReader ExecuteReader(string commandText)
        {
            return this.ExecuteReader(commandText, _DefaultCommandType, CommandBehavior.Default,
                (DbParameter[])null);
        }

        public virtual DbDataReader ExecuteReader(string commandText, params DbParameter[] parameters)
        {
            return this.ExecuteReader(commandText, _DefaultCommandType, CommandBehavior.Default,
                parameters);
        }

        public virtual DbDataReader ExecuteReader(string commandText, CommandType commandType,
            params DbParameter[] parameters)
        {
            return this.ExecuteReader(commandText, commandType, CommandBehavior.Default, parameters);
        }

        public virtual DbDataReader ExecuteReader(string commandText, CommandBehavior behavior,
            params DbParameter[] parameters)
        {
            return this.ExecuteReader(commandText, _DefaultCommandType, CommandBehavior.Default,
                parameters);
        }

        public virtual DbDataReader ExecuteReader(string commandText, CommandType commandType,
            CommandBehavior behavior, params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreateCommand(commandText, commandType, parameters))
            {
                try { return this.ExecuteReader(command, behavior, false); }
                catch (Exception ex) { throw ex; }
                finally { command.Parameters.Clear(); }
            }
        }

        public virtual DbDataReader ExecuteReader(DbCommand command)
        {
            return this.ExecuteReader(command, CommandBehavior.Default, true);
        }

        public virtual DbDataReader ExecuteReader(DbCommand command, CommandBehavior behavior)
        {
            return ExecuteReader(command, behavior, true);
        }

        protected virtual DbDataReader ExecuteReader(DbCommand command, CommandBehavior behavior, bool cloneCommand)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (cloneCommand)
                command = this.CloneCommand(command);
            command.Connection = this._Connection;
            command.Transaction = this._Transaction;
            command.CommandTimeout = this._Connection.ConnectionTimeout;
            DbDataReader reader = null;
            try
            {
                if (_IsClosed)
                    this._Connection.Open();
                reader = command.ExecuteReader(behavior);
                return reader;
            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                        reader.Close();
                    reader.Dispose();
                    reader = null;
                }
                if (_IsClosed)
                    this._Connection.Close();
                string msg = ex.Message.ToLower();
                throw msg.Contains("duplicate key") ? new DuplicateKeyException(ex.Message, ex) : ex;
            }
            finally
            {
                command.Transaction = null;
                command.Connection = null;
            }
        }

        public virtual Task<DbDataReader> ExecuteReaderAsync(string commandText)
        {
            return this.ExecuteReaderAsync(commandText, _DefaultCommandType, CommandBehavior.Default,
                (DbParameter[])null);
        }

        public virtual Task<DbDataReader> ExecuteReaderAsync(string commandText, params DbParameter[] parameters)
        {
            return this.ExecuteReaderAsync(commandText, _DefaultCommandType, CommandBehavior.Default,
                parameters);
        }

        public virtual Task<DbDataReader> ExecuteReaderAsync(string commandText, CommandType commandType,
            params DbParameter[] parameters)
        {
            return this.ExecuteReaderAsync(commandText, commandType, CommandBehavior.Default, parameters);
        }

        public virtual Task<DbDataReader> ExecuteReaderAsync(string commandText, CommandBehavior behavior,
            params DbParameter[] parameters)
        {
            return this.ExecuteReaderAsync(commandText, _DefaultCommandType, CommandBehavior.Default,
                parameters);
        }

        public virtual Task<DbDataReader> ExecuteReaderAsync(string commandText, CommandType commandType,
            CommandBehavior behavior, params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreateCommand(commandText, commandType, parameters))
            {
                try { return this.ExecuteReaderAsync(command, behavior, false, new Nullable<CancellationToken>()); }
                catch (Exception ex) { throw ex; }
                finally { command.Parameters.Clear(); }
            }
        }

        public virtual Task<DbDataReader> ExecuteReaderAsync(DbCommand command)
        {
            return this.ExecuteReaderAsync(command, CommandBehavior.Default, true, new Nullable<CancellationToken>());
        }

        public virtual Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CommandBehavior behavior)
        {
            return ExecuteReaderAsync(command, behavior, true, new Nullable<CancellationToken>());
        }

        protected virtual async Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CommandBehavior behavior,
            bool cloneCommand, Nullable<CancellationToken> token)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (cloneCommand)
                command = this.CloneCommand(command);
            command.Connection = this._Connection;
            command.Transaction = this._Transaction;
            command.CommandTimeout = this._Connection.ConnectionTimeout;
            DbDataReader reader = null;
            try
            {
                if (_IsClosed)
                    this._Connection.Open();
                reader = await (token.HasValue ? command.ExecuteReaderAsync(behavior, token.Value) :
                    command.ExecuteReaderAsync(behavior));
                return reader;
            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                        reader.Close();
                    reader.Dispose();
                    reader = null;
                }
                if (_IsClosed)
                    this._Connection.Close();
                string msg = ex.Message.ToLower();
                throw msg.Contains("duplicate key") ? new DuplicateKeyException(ex.Message, ex) : ex;
            }
            finally
            {
                command.Transaction = null;
                command.Connection = null;
            }
        }

        public virtual Task<DbDataReader> ExecuteReaderAsync(string commandText, CancellationToken token)
        {
            return this.ExecuteReaderAsync(commandText, _DefaultCommandType, CommandBehavior.Default, token,
                (DbParameter[])null);
        }

        public virtual Task<DbDataReader> ExecuteReaderAsync(string commandText, CancellationToken token, params DbParameter[] parameters)
        {
            return this.ExecuteReaderAsync(commandText, _DefaultCommandType, CommandBehavior.Default, token, parameters);
        }

        public virtual Task<DbDataReader> ExecuteReaderAsync(string commandText, CommandType commandType,
            CancellationToken token, params DbParameter[] parameters)
        {
            return this.ExecuteReaderAsync(commandText, commandType, CommandBehavior.Default, token, parameters);
        }

        public virtual Task<DbDataReader> ExecuteReaderAsync(string commandText, CommandBehavior behavior,
            CancellationToken token, params DbParameter[] parameters)
        {
            return this.ExecuteReaderAsync(commandText, _DefaultCommandType, CommandBehavior.Default, token, parameters);
        }

        public virtual Task<DbDataReader> ExecuteReaderAsync(string commandText, CommandType commandType,
            CommandBehavior behavior, CancellationToken token, params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreateCommand(commandText, commandType, parameters))
            {
                try { return this.ExecuteReaderAsync(command, behavior, false, token); }
                catch (Exception ex) { throw ex; }
                finally { command.Parameters.Clear(); }
            }
        }

        public virtual Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CancellationToken token)
        {
            return this.ExecuteReaderAsync(command, CommandBehavior.Default, true, token);
        }

        public virtual Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CommandBehavior behavior,
            CancellationToken token)
        {
            return ExecuteReaderAsync(command, behavior, true, token);
        }
    }
}