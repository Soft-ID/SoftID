using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace SoftID.Data
{
    public static class DbCommandExtension
    {
        public static DbParameter CreateOrSetParameter(this DbCommand command, string name, object value)
        {
            bool isCreate = !command.Parameters.Contains(name);
            DbParameter param = isCreate ? command.CreateParameter() : command.Parameters[name];
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            if (isCreate)
                command.Parameters.Add(param);
            return param;
        }

        public static DbParameter CreateOrSetParameter(this DbCommand command, string name, DbType dbType, object value)
        {
            DbParameter param = CreateOrSetParameter(command, name, value);
            param.DbType = dbType;
            return param;
        }

        public static DbParameter CreateOrSetParameter(this DbCommand command, string name, DbType dbType,
            ParameterDirection direction, object value)
        {
            DbParameter param = CreateOrSetParameter(command, name, dbType, value);
            param.Direction = direction;
            return param;
        }

        public static DbParameter CreateOrSetParameter(this DbCommand command, string name, DbType dbType,
            int size, object value)
        {
            DbParameter param = CreateOrSetParameter(command, name, dbType, value);
            param.Size = size;
            return param;
        }

        public static DbParameter CreateOrSetParameter(this DbCommand command, string name, DbType dbType,
            ParameterDirection direction, int size, object value)
        {
            DbParameter param = CreateOrSetParameter(command, name, dbType, direction, value);
            param.Size = size;
            return param;
        }

        public static DbParameter CreateOrSetParameter(this DbCommand command, string name,
            string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter param = CreateOrSetParameter(command, name, value);
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            return param;
        }

        public static DbParameter CreateOrSetParameter(this DbCommand command, string name,
            string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
        {
            DbParameter param = CreateOrSetParameter(command, name, sourceColumn, sourceVersion, value);
            param.SourceColumnNullMapping = sourceColumnNullMapping;
            return param;
        }

        public static DbParameter CreateOrSetParameter(this DbCommand command, string name, DbType dbType,
            string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter param = CreateOrSetParameter(command, name, dbType, value);
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            return param;
        }

        public static DbParameter CreateOrSetParameter(this DbCommand command, string name, DbType dbType,
            string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
        {
            DbParameter param = CreateOrSetParameter(command, name, dbType, sourceColumn, sourceVersion, value);
            param.SourceColumnNullMapping = sourceColumnNullMapping;
            return param;
        }

        public static DbParameter CreateOrSetParameter(this DbCommand command, string name,
            DbType dbType, ParameterDirection direction,
            string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter param = CreateOrSetParameter(command, name, dbType, direction, value);
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            return param;
        }

        public static DbParameter CreateOrSetParameter(this DbCommand command, string name,
            DbType dbType, ParameterDirection direction,
            string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
        {
            DbParameter param = CreateOrSetParameter(command, name, dbType, direction, sourceColumn, sourceVersion, value);
            param.SourceColumnNullMapping = sourceColumnNullMapping;
            return param;
        }

        public static DbParameter CreateOrSetParameter(this DbCommand command, string name, DbType dbType, int size,
            string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter param = CreateOrSetParameter(command, name, dbType, size, value);
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            return param;
        }

        public static DbParameter CreateOrSetParameter(this DbCommand command, string name, DbType dbType, int size,
            string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
        {
            DbParameter param = CreateOrSetParameter(command, name, dbType, size, sourceColumn, sourceVersion, value);
            param.SourceColumnNullMapping = sourceColumnNullMapping;
            return param;
        }

        public static DbParameter CreateOrSetParameter(this DbCommand command, string name,
            DbType dbType, ParameterDirection direction, int size,
            string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter param = CreateOrSetParameter(command, name, dbType, direction, size, value);
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            return param;
        }

        public static DbParameter CreateOrSetParameter(this DbCommand command, string name,
            DbType dbType, ParameterDirection direction, int size,
            string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
        {
            DbParameter param = CreateOrSetParameter(command, name, dbType, direction, size, sourceColumn, sourceVersion, value);
            param.SourceColumnNullMapping = sourceColumnNullMapping;
            return param;
        }

        public static void SetValues(this DbCommand command, params object[] values)
        {
            for (int i = 0; i < command.Parameters.Count; i++)
                command.Parameters[i].Value = (values != null && i < values.Length ? values[i] : null) ?? DBNull.Value;
        }

        public static void GetValues(this DbCommand command, params object[] values)
        {
            for (int i = 0; i < command.Parameters.Count; i++)
            {
                DbParameter param = command.Parameters[i];
                if (param.Direction == ParameterDirection.Input) continue;
                if (values != null && i < values.Length)
                    values[i] = param.Value is DBNull ? null : param.Value;
            }
        }

        public static void SetValues(this DbCommand command, DataRow row)
        {
            for (int i = 0; i < command.Parameters.Count; i++)
            {
                DbParameter param = command.Parameters[i];
                param.Value = row[param.SourceColumn, param.SourceVersion];
            }
        }

        public static void GetValues(this DbCommand command, DataRow row)
        {
            for (int i = 0; i < command.Parameters.Count; i++)
            {
                DbParameter param = command.Parameters[i];
                if (param.Direction != ParameterDirection.Input)
                    row[param.SourceColumn] = param.Value ?? DBNull.Value;
            }
        }
    }
}
