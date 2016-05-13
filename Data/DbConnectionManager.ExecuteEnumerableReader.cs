using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftID.Data
{
    public partial class DbConnectionManager
    {
        public IEnumerable<IDataRecord> ExecuteEnumerableReader(string commandText)
        {
            return ExecuteEnumerableReader(commandText, _DefaultCommandType, CommandBehavior.Default, null);
        }

        public IEnumerable<IDataRecord> ExecuteEnumerableReader(string commandText, params DbParameter[] parameters)
        {
            return ExecuteEnumerableReader(commandText, _DefaultCommandType, CommandBehavior.Default, parameters);
        }

        public IEnumerable<IDataRecord> ExecuteEnumerableReader(string commandText,
            CommandType commandType, params DbParameter[] parameters)
        {
            return ExecuteEnumerableReader(commandText, commandType, CommandBehavior.Default, parameters);
        }

        public IEnumerable<IDataRecord> ExecuteEnumerableReader(string commandText,
            CommandBehavior behavior, params DbParameter[] parameters)
        {
            return ExecuteEnumerableReader(commandText, _DefaultCommandType, behavior, parameters);
        }

        public IEnumerable<IDataRecord> ExecuteEnumerableReader(string commandText, CommandType commandType,
            CommandBehavior behavior, params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreateCommand(commandText, commandType, parameters))
            {
                return new EnumerableReader<IDataRecord>(this, command, behavior, false, r => r);
            }
        }

        public IEnumerable<IDataRecord> ExecuteEnumerableReader(DbCommand command)
        {
            return new EnumerableReader<IDataRecord>(this, command, CommandBehavior.Default, true, r => r);
        }

        public IEnumerable<IDataRecord> ExecuteEnumerableReader(DbCommand command,
            CommandBehavior behavior)
        {
            return new EnumerableReader<IDataRecord>(this, command, behavior, true, r => r);
        }
    }
}