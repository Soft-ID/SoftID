using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace SoftID.Data
{
    public partial class DbConnectionManager
    {
        public virtual DataSet ExecuteDataSet(string commandText)
        {
            return this.ExecuteDataSet(commandText, _DefaultCommandType, (DbParameter[])null);
        }

        public virtual DataSet ExecuteDataSet(string commandText, params DbParameter[] parameters)
        {
            return this.ExecuteDataSet(commandText, _DefaultCommandType, parameters);
        }

        public virtual DataSet ExecuteDataSet(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            DataSet ds = new DataSet();
            FillDataSet(ds, commandText, commandType, parameters);
            return ds;
        }

        public virtual DataSet ExecuteDataSet(DbCommand command)
        {
            DataSet ds = new DataSet();
            FillDataSet(ds, command);
            return ds;
        }

        public virtual void FillDataSet(DataSet dataSet, string commandText)
        {
            FillDataSet(dataSet, commandText, _DefaultCommandType, (DbParameter[])null);
        }

        public virtual void FillDataSet(DataSet dataSet, string commandText, params DbParameter[] parameters)
        {
            FillDataSet(dataSet, commandText, _DefaultCommandType, parameters);
        }

        public virtual void FillDataSet(DataSet dataSet, string commandText, CommandType commandType,
            params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreateCommand(commandText, commandType, parameters))
            {
                try
                {
                    FillDataSet(dataSet, command, new Nullable<LoadOption>(),
                        new Nullable<MissingSchemaAction>(), false);
                }
                catch (Exception ex) { throw ex; }
                finally { command.Parameters.Clear(); }
            }
        }

        public virtual void FillDataSet(DataSet dataSet, DbCommand command)
        {
            FillDataSet(dataSet, command, new Nullable<LoadOption>(),
                new Nullable<MissingSchemaAction>(), true);
        }

        public virtual void FillDataSet(DataSet dataSet, DbCommand command,
            LoadOption loadOption, MissingSchemaAction missingSchemaAction)
        {
            FillDataSet(dataSet, command, loadOption, missingSchemaAction, true);
        }

        protected virtual void FillDataSet(DataSet dataSet, DbCommand command,
            LoadOption? loadOption, MissingSchemaAction? missingSchemaAction, bool cloneCommand)
        {
            if (cloneCommand)
                command = this.CloneCommand(command);
            command.Connection = this._Connection;
            command.Transaction = this._Transaction;
            command.CommandTimeout = this._Connection.ConnectionTimeout;
            try
            {
                if (_IsClosed)
                    this._Connection.Open();
                using (DbDataAdapter adapter = this.CreateDataAdapter(command))
                {
                    if (loadOption.HasValue)
                        adapter.FillLoadOption = loadOption.Value;
                    if (missingSchemaAction.HasValue)
                        adapter.MissingSchemaAction = missingSchemaAction.Value;
                    adapter.Fill(dataSet);
                }
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
    }
}