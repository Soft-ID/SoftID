using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftID.Data
{
    public partial class DbConnectionManager
    {
        public override bool CanCreateDataSourceEnumerator
        {
            get { return this._Factory.CanCreateDataSourceEnumerator; }
        }

        public override DbCommand CreateCommand()
        {
            DbCommand command = this._Factory.CreateCommand();
            command.CommandType = _DefaultCommandType;
            return command;
        }

        public override DbCommandBuilder CreateCommandBuilder()
        {
            return this._Factory.CreateCommandBuilder();
        }

        public override DbConnection CreateConnection()
        {
            return this._Factory.CreateConnection();
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return this._Factory.CreateConnectionStringBuilder();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return this._Factory.CreateDataAdapter();
        }

        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            return this._Factory.CreateDataSourceEnumerator();
        }

        public override DbParameter CreateParameter()
        {
            return this._Factory.CreateParameter();
        }

        public override System.Security.CodeAccessPermission CreatePermission(System.Security.Permissions.PermissionState state)
        {
            return this._Factory.CreatePermission(state);
        }
    }
}