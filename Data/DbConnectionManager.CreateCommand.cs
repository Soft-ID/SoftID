using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using SoftID.Utilities;

namespace SoftID.Data
{
    public partial class DbConnectionManager
    {
        private DbCommand CreatePaginateCommand(ref int pageIndex, int pageSize,
            string selectCommandText, string orderBy, params DbParameter[] parameters)
        {
            if (pageSize < 0)
                pageSize = 1;
            if (pageIndex < 0)
                pageIndex = 0;
            int rowIndex1 = pageIndex * pageSize;
            int rowIndex2 = rowIndex1 + pageSize - 1;
            if (rowIndex2 < rowIndex1)
                rowIndex2 = rowIndex1;

            selectCommandText = string.Format(_PaginateCommandTextFormat,
                selectCommandText, orderBy);
            DbCommand command = this.CreateCommand(selectCommandText, CommandType.Text, parameters);
            command.CreateOrSetParameter("RowIndex1", DbType.Int32, rowIndex1);
            command.CreateOrSetParameter("RowIndex2", DbType.Int32, rowIndex2);
            command.CreateOrSetParameter("PageSize", DbType.Int32, pageSize);
            return command;
        }

        private DbCommand CreatePaginateCommand(ref int pageIndex, int pageSize, out int totalRecords,
            string selectCommandText, string orderBy, params DbParameter[] parameters)
        {
            DbCommand command = CreateCountCommand(selectCommandText, parameters);
            try { totalRecords = ExecuteScalar(command, false).ToStruct<int>(); }
            catch (Exception ex) { throw ex; }
            finally
            {
                command.Parameters.Clear();
                command.Dispose();
                command = null;
            }

            if (pageSize < 0)
                pageSize = 1;
            int totalPages = Convert.ToInt32(Math.Ceiling((double)totalRecords / (double)pageSize));
            if (pageIndex >= totalPages)
                pageIndex = totalPages - 1;
            if (pageIndex < 0)
                pageIndex = 0;
            int rowIndex1 = pageIndex * pageSize;
            int rowIndex2 = Math.Min(rowIndex1 + pageSize, totalRecords) - 1;
            if (rowIndex2 < rowIndex1)
                rowIndex2 = rowIndex1;

            selectCommandText = string.Format(_PaginateCommandTextFormat,
                selectCommandText, orderBy);
            command = this.CreateCommand(selectCommandText, CommandType.Text, parameters);
            command.CreateOrSetParameter("RowIndex1", DbType.Int32, rowIndex1);
            command.CreateOrSetParameter("RowIndex2", DbType.Int32, rowIndex2);
            command.CreateOrSetParameter("PageSize", DbType.Int32, pageSize);
            return command;
        }

        private DbCommand CreateCountCommand(string selectCommandText, params DbParameter[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT COUNT(*)");
            sb.AppendLine("FROM (");
            sb.AppendLine(selectCommandText);
            sb.AppendLine(") AS X");
            selectCommandText = sb.ToString();
            sb.Remove(0, sb.Length);
            sb = null;
            return this.CreateCommand(selectCommandText, CommandType.Text, parameters);
        }
    }
}