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
        public virtual DataTable ExecuteDataTable(string commandText)
        {
            return this.ExecuteDataTable(commandText, _DefaultCommandType, (DbParameter[])null);
        }

        public virtual DataTable ExecuteDataTable(string commandText, params DbParameter[] parameters)
        {
            return this.ExecuteDataTable(commandText, _DefaultCommandType, parameters);
        }

        public virtual DataTable ExecuteDataTable(string commandText, CommandType commandType,
            params DbParameter[] parameters)
        {
            DataTable table = new DataTable();
            FillDataTable(table, commandText, commandType, parameters);
            return table;
        }

        public virtual DataTable ExecuteDataTable(DbCommand command)
        {
            DataTable table = new DataTable();
            FillDataTable(table, command);
            return table;
        }

        public virtual void FillDataTable(DataTable table, string commandText)
        {
            FillDataTable(table, commandText, _DefaultCommandType, (DbParameter[])null);
        }

        public virtual void FillDataTable(DataTable table, string commandText, params DbParameter[] parameters)
        {
            FillDataTable(table, commandText, _DefaultCommandType, parameters);
        }

        public virtual void FillDataTable(DataTable table, string commandText, CommandType commandType,
            params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreateCommand(commandText, commandType, parameters))
            {
                try
                {
                    FillDataTable(table, command, new Nullable<LoadOption>(),
                        new Nullable<MissingSchemaAction>(), false);
                }
                catch (Exception ex) { throw ex; }
                finally { command.Parameters.Clear(); }
            }
        }

        public virtual void FillDataTable(DataTable table, DbCommand command)
        {
            this.FillDataTable(table, command, new Nullable<LoadOption>(),
                new Nullable<MissingSchemaAction>(), true);
        }

        public virtual void FillDataTable(DataTable table, DbCommand command,
            LoadOption loadOption, MissingSchemaAction missingSchemaAction)
        {
            FillDataTable(table, command, loadOption, missingSchemaAction, true);
        }

        protected virtual void FillDataTable(DataTable table, DbCommand command,
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
                    adapter.Fill(table);
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