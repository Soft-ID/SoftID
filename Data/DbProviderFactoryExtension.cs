using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace SoftID.Data
{
    public static class DbProviderFactoryExtension
    {
        public static DbConnection CreateConnection(this DbProviderFactory factory, string connectionString)
        {
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }

        #region CreateCommand
        public static DbCommand CreateCommand(this DbProviderFactory factory, string commandText,
            params DbParameter[] parameters)
        {
            CommandType defaultCommandType = factory is DbConnectionManager
                ? ((DbConnectionManager)factory).DefaultCommandType
                : defaultCommandType = CommandType.Text;
            return DbProviderFactoryExtension.CreateCommand(factory, commandText, defaultCommandType, parameters);
        }

        public static DbCommand CreateCommand(this DbProviderFactory factory, string commandText,
            CommandType commandType, params DbParameter[] parameters)
        {
            DbCommand command = factory.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;
            if (parameters != null && parameters.Length > 0)
                command.Parameters.AddRange(parameters);
            return command;
        }

        public static DbCommand CloneCommand(this DbProviderFactory factory, DbCommand command)
        {
            DbCommand newCommand = factory.CreateCommand(command.CommandText, command.CommandType);
            for (int i = 0; i < command.Parameters.Count; i++)
            {
                DbParameter parameter = command.Parameters[i];
                DbParameter newParameter = factory.CreateParameter(parameter.ParameterName, parameter.DbType,
                    parameter.Direction, parameter.Size, parameter.SourceColumn, parameter.SourceVersion,
                    parameter.SourceColumnNullMapping, parameter.Value);
                newCommand.Parameters.Add(newParameter);
            }
            return newCommand;
        }
        #endregion

        #region CreateParameter
        public static DbParameter CreateParameter(this DbProviderFactory factory, string name, object value)
        {
            DbParameter param = factory.CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            return param;
        }

        public static DbParameter CreateParameter(this DbProviderFactory factory, string name, DbType dbType, object value)
        {
            DbParameter param = CreateParameter(factory, name, value);
            param.DbType = dbType;
            return param;
        }

        public static DbParameter CreateParameter(this DbProviderFactory factory, string name, DbType dbType, ParameterDirection direction, object value)
        {
            DbParameter param = CreateParameter(factory, name, dbType, value);
            param.Direction = direction;
            return param;
        }

        public static DbParameter CreateParameter(this DbProviderFactory factory, string name, DbType dbType, int size, object value)
        {
            DbParameter param = CreateParameter(factory, name, dbType, value);
            param.Size = size;
            return param;
        }

        public static DbParameter CreateParameter(this DbProviderFactory factory, string name, DbType dbType, ParameterDirection direction, int size, object value)
        {
            DbParameter param = CreateParameter(factory, name, dbType, direction, value);
            param.Size = size;
            return param;
        }

        public static DbParameter CreateParameter(this DbProviderFactory factory, string name,
           string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter param = CreateParameter(factory, name, value);
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            return param;
        }

        public static DbParameter CreateParameter(this DbProviderFactory factory, string name,
            string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
        {
            DbParameter param = CreateParameter(factory, name, sourceColumn, sourceVersion, value);
            param.SourceColumnNullMapping = sourceColumnNullMapping;
            return param;
        }

        public static DbParameter CreateParameter(this DbProviderFactory factory, string name, DbType dbType,
            string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter param = CreateParameter(factory, name, dbType, value);
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            return param;
        }

        public static DbParameter CreateParameter(this DbProviderFactory factory, string name, DbType dbType,
            string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
        {
            DbParameter param = CreateParameter(factory, name, dbType, sourceColumn, sourceVersion, value);
            param.SourceColumnNullMapping = sourceColumnNullMapping;
            return param;
        }

        public static DbParameter CreateParameter(this DbProviderFactory factory, string name, DbType dbType, ParameterDirection direction,
            string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter param = CreateParameter(factory, name, dbType, direction, value);
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            return param;
        }

        public static DbParameter CreateParameter(this DbProviderFactory factory, string name, DbType dbType, ParameterDirection direction,
            string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
        {
            DbParameter param = CreateParameter(factory, name, dbType, direction, sourceColumn, sourceVersion, value);
            param.SourceColumnNullMapping = sourceColumnNullMapping;
            return param;
        }

        public static DbParameter CreateParameter(this DbProviderFactory factory, string name, DbType dbType, int size,
           string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter param = CreateParameter(factory, name, dbType, size, value);
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            return param;
        }

        public static DbParameter CreateParameter(this DbProviderFactory factory, string name, DbType dbType, int size,
            string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
        {
            DbParameter param = CreateParameter(factory, name, dbType, size, sourceColumn, sourceVersion, value);
            param.SourceColumnNullMapping = sourceColumnNullMapping;
            return param;
        }

        public static DbParameter CreateParameter(this DbProviderFactory factory, string name, DbType dbType, ParameterDirection direction, int size,
           string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter param = CreateParameter(factory, name, dbType, direction, size, value);
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            return param;
        }

        public static DbParameter CreateParameter(this DbProviderFactory factory, string name, DbType dbType, ParameterDirection direction, int size,
            string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
        {
            DbParameter param = CreateParameter(factory, name, dbType, direction, size, sourceColumn, sourceVersion, value);
            param.SourceColumnNullMapping = sourceColumnNullMapping;
            return param;
        }
        #endregion

        public static DbDataAdapter CreateDataAdapter(this DbProviderFactory factory, DbCommand selectCommand)
        {
            DbDataAdapter da = factory.CreateDataAdapter();
            da.SelectCommand = selectCommand;
            return da;
        }

        public static DbDataAdapter CreateDataAdapter(this DbProviderFactory factory, DbCommand selectCommand,
            DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand)
        {
            DbDataAdapter da = CreateDataAdapter(factory, selectCommand);
            da.InsertCommand = insertCommand;
            da.UpdateCommand = updateCommand;
            da.DeleteCommand = deleteCommand;
            return da;
        }
    }
}