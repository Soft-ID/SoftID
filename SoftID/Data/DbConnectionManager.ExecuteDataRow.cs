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
        public virtual DataRow ExecuteDataRow(string commandText)
        {
            return this.ExecuteDataRow(commandText, _DefaultCommandType, (DbParameter[])null);
        }

        public virtual DataRow ExecuteDataRow(string commandText, params DbParameter[] parameters)
        {
            return this.ExecuteDataRow(commandText, _DefaultCommandType, parameters);
        }

        public virtual DataRow ExecuteDataRow(string commandText, CommandType commandType,
            params DbParameter[] parameters)
        {
            DataTable table = ExecuteDataTable(commandText, commandType, parameters);
            return GetSingleDataRow(table);
        }

        public virtual DataRow ExecuteDataRow(DbCommand command)
        {
            DataTable table = this.ExecuteDataTable(command);
            return GetSingleDataRow(table);
        }

        public static DataRow GetSingleDataRow(DataTable table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            switch (table.Rows.Count)
            {
                case 0:
                    return null;
                case 1:
                    return table.Rows[0];
                default:
                    throw new ApplicationException("Multiple rows returned.");
            }
        }
    }
}