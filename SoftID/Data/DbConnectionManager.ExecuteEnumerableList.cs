using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace SoftID.Data
{
    public partial class DbConnectionManager
    {
        public IEnumerable<T> ExecuteEnumerableList<T>(string commandText, Func<IDataRecord, T> readerConverter)
        {
            return ExecuteEnumerableList<T>(commandText, _DefaultCommandType, CommandBehavior.Default, readerConverter,
                (DbParameter[])null);
        }

        public IEnumerable<T> ExecuteEnumerableList<T>(string commandText, Func<IDataRecord, T> readerConverter,
            params DbParameter[] parameters)
        {
            return ExecuteEnumerableList<T>(commandText, _DefaultCommandType, CommandBehavior.Default,
                readerConverter, parameters);
        }

        public IEnumerable<T> ExecuteEnumerableList<T>(string commandText, CommandType commandType,
            Func<IDataRecord, T> readerConverter, params DbParameter[] parameters)
        {
            return ExecuteEnumerableList<T>(commandText, commandType, CommandBehavior.Default,
                readerConverter, parameters);
        }

        public IEnumerable<T> ExecuteEnumerableList<T>(string commandText, CommandBehavior behavior,
            Func<IDataRecord, T> readerConverter, params DbParameter[] parameters)
        {
            return ExecuteEnumerableList<T>(commandText, _DefaultCommandType, behavior, readerConverter, parameters);
        }

        public IEnumerable<T> ExecuteEnumerableList<T>(string commandText, CommandType commandType,
            CommandBehavior behavior, Func<IDataRecord, T> readerConverter, params DbParameter[] parameters)
        {
            using (DbCommand command = this.CreateCommand(commandText, commandType, parameters))
            {
                return new EnumerableReader<T>(this, command, behavior, false, readerConverter);
            }
        }

        public IEnumerable<T> ExecuteEnumerableList<T>(DbCommand command, Func<IDataRecord, T> readerConverter)
        {
            return new EnumerableReader<T>(this, command, CommandBehavior.Default, true, readerConverter);
        }

        public IEnumerable<T> ExecuteEnumerableList<T>(DbCommand command, CommandBehavior behavior,
            Func<IDataRecord, T> readerConverter)
        {
            return new EnumerableReader<T>(this, command, behavior, true, readerConverter);
        }

        private class EnumerableReader<Tnumerable> : IEnumerable<Tnumerable>
        {
            private DbConnectionManager dbConn;
            private DbCommand command;
            private CommandBehavior behavior;
            private bool cloneCommand;
            private Func<DbDataReader, Tnumerable> readerConverter;

            public EnumerableReader(DbConnectionManager dbConn, DbCommand command, CommandBehavior behavior,
                bool cloneCommand, Func<DbDataReader, Tnumerable> readerConverter)
            {
                this.dbConn = dbConn;
                this.command = command;
                this.behavior = behavior;
                this.cloneCommand = cloneCommand;
                this.readerConverter = readerConverter;
            }

            #region IEnumerable<T> Members
            public IEnumerator<Tnumerable> GetEnumerator()
            {
                return new EnumeratorReader<Tnumerable>(this);
            }

            IEnumerator<Tnumerable> IEnumerable<Tnumerable>.GetEnumerator()
            {
                return this.GetEnumerator();
            }
            #endregion

            #region IEnumerable Members
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
            #endregion

            private class EnumeratorReader<Tnumerator> : IEnumerator<Tnumerator>
            {
                EnumerableReader<Tnumerator> enumerable;
                DbDataReader reader;

                public EnumeratorReader(EnumerableReader<Tnumerator> enumerableReader)
                {
                    this.enumerable = enumerableReader;
                    reader = this.enumerable.dbConn.ExecuteReader(this.enumerable.command,
                        this.enumerable.behavior, this.enumerable.cloneCommand);
                }

                #region IEnumerator<T> Members
                public Tnumerator Current
                {
                    get { return enumerable.readerConverter(reader); }
                }

                Tnumerator IEnumerator<Tnumerator>.Current
                {
                    get { return this.Current; }
                }
                #endregion

                #region IEnumerator Members
                public bool MoveNext()
                {
                    return reader.Read();
                }

                bool System.Collections.IEnumerator.MoveNext()
                {
                    return this.MoveNext();
                }

                public void Reset()
                {
                    this.Dispose();
                    reader = this.enumerable.dbConn.ExecuteReader(this.enumerable.command,
                        this.enumerable.behavior, this.enumerable.cloneCommand);
                }

                void System.Collections.IEnumerator.Reset()
                {
                    this.Reset();
                }

                object System.Collections.IEnumerator.Current
                {
                    get { return this.Current; }
                }
                #endregion

                #region IDisposable Members
                public void Dispose()
                {
                    if (reader != null)
                    {
                        if (!reader.IsClosed)
                            reader.Close();
                        reader.Dispose();
                        reader = null;
                    }
                    if (enumerable.dbConn._IsClosed)
                        this.enumerable.dbConn.Close();
                }

                void IDisposable.Dispose()
                {
                    this.Dispose();
                }
                #endregion
            }
        }
    }
}