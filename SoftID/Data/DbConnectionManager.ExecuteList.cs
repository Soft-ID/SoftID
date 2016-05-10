using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace SoftID.Data
{
    public partial class DbConnectionManager
    {
        public virtual List<T> ExecuteList<T>(string commandText, Func<IDataRecord, T> readerConverter)
        {
            return this.ExecuteList<T>(commandText, _DefaultCommandType, CommandBehavior.Default, readerConverter,
                (DbParameter[])null);
        }

        public virtual List<T> ExecuteList<T>(string commandText, Func<IDataRecord, T> readerConverter,
            params DbParameter[] parameters)
        {
            return this.ExecuteList<T>(commandText, _DefaultCommandType, CommandBehavior.Default, readerConverter,
                parameters);
        }

        public virtual List<T> ExecuteList<T>(string commandText, CommandType commandType,
            Func<IDataRecord, T> readerConverter, params DbParameter[] parameters)
        {
            return this.ExecuteList<T>(commandText, commandType, CommandBehavior.Default, readerConverter, parameters);
        }

        public virtual List<T> ExecuteList<T>(string commandText, CommandBehavior behavior,
            Func<IDataRecord, T> readerConverter, params DbParameter[] parameters)
        {
            return this.ExecuteList<T>(commandText, _DefaultCommandType, CommandBehavior.Default,
                readerConverter, parameters);
        }

        public virtual List<T> ExecuteList<T>(string commandText, CommandType commandType, CommandBehavior behavior,
            Func<IDataRecord, T> readerConverter, params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreateCommand(commandText, commandType, parameters))
            {
                try { return this.ExecuteList<T>(command, behavior, readerConverter); }
                catch (Exception ex) { throw ex; }
                finally { command.Parameters.Clear(); }
            }
        }

        public virtual List<T> ExecuteList<T>(DbCommand command, Func<IDataRecord, T> readerConverter)
        {
            return this.ExecuteList<T>(command, CommandBehavior.Default, readerConverter);
        }

        public virtual List<T> ExecuteList<T>(DbCommand command, CommandBehavior behavior,
            Func<IDataRecord, T> readerConverter)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            command.Connection = this._Connection;
            command.Transaction = this._Transaction;
            command.CommandTimeout = this._Connection.ConnectionTimeout;
            try
            {
                List<T> list = new List<T>();
                if (_IsClosed)
                    this._Connection.Open();
                using (DbDataReader reader = command.ExecuteReader(behavior))
                {
                    while (reader.Read())
                    {
                        T entity = readerConverter(reader);
                        list.Add(entity);
                    }
                    reader.Close();
                }
                return list;
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
            }
        }
    }
}
