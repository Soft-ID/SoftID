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
        public virtual DbDataReader ExecutePaginateReader(ref int pageIndex, int pageSize, out int totalRecords,
            string orderBy, string selectCommandText, params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreatePaginateCommand(ref pageIndex, pageSize, out totalRecords,
                selectCommandText, orderBy, parameters))
            {
                DbDataReader reader = null;
                try
                {
                    reader = this.ExecuteReader(command);
                    return reader;
                }
                catch (Exception ex)
                {
                    if (reader != null)
                    {
                        if (!reader.IsClosed)
                            reader.Close();
                        reader.Dispose();
                    }
                    string msg = ex.Message.ToLower();
                    throw msg.Contains("duplicate key") ? new DuplicateKeyException(ex.Message, ex) : ex;
                }
                finally
                {
                    command.Parameters.Clear();
                }
            }
        }

        public virtual DataTable ExecutePaginateDataTable(ref int pageIndex, int pageSize, out int totalRecords,
            string orderBy, string selectCommandText, params DbParameter[] parameters)
        {
            DataTable table = new DataTable();
            this.FillPaginateDataTable(table, ref pageIndex, pageSize, out totalRecords,
                orderBy, selectCommandText, parameters);
            return table;
        }

        public virtual void FillPaginateDataTable(DataTable table, ref int pageIndex, int pageSize, out int totalRecords,
            string orderBy, string selectCommandText, params DbParameter[] parameters)
        {
            table.Rows.Clear();
            table.Columns.Clear();
            try
            {
                using (DbDataReader reader = this.ExecutePaginateReader(ref pageIndex, pageSize, out totalRecords, orderBy,
                    selectCommandText, parameters))
                {
                    table.Load(reader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_IsClosed)
                    this._Connection.Close();
            }
        }

        public virtual IEnumerable<IDataRecord> ExecutePaginateEnumerableReader(
            ref int pageIndex, int pageSize, out int totalRecords,
            string orderBy, string selectCommandText, params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreatePaginateCommand(ref pageIndex, pageSize, out totalRecords,
                selectCommandText, orderBy, parameters))
            {
                return new EnumerableReader<IDataRecord>(this, command, CommandBehavior.Default, false, r => r);
            }
        }

        public virtual IEnumerable<T> ExecutePaginateEnumerableList<T>(
            ref int pageIndex, int pageSize, out int totalRecords, string orderBy, string selectCommandText,
            Func<IDataRecord, T> readerConverter, params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreatePaginateCommand(ref pageIndex, pageSize, out totalRecords,
                selectCommandText, orderBy, parameters))
            {
                return new EnumerableReader<T>(this, command, CommandBehavior.Default, false, readerConverter);
            }
        }
    }
}