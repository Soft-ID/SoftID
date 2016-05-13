using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.Data.Common;
using SoftID.Configuration;

namespace SoftID.Data
{
    public partial class DbConnectionManager : DbProviderFactory, IDisposable, ICloneable
    {
        public static readonly string DefaultConnectionStringName;
        private static readonly PaginateQueryConfigurationElementCollection _PaginateQueryList = new PaginateQueryConfigurationElementCollection();
        private readonly string _PaginateCommandTextFormat = "{0}";

        static DbConnectionManager()
        {
            SoftIDConfigurationSection softIDConfig = ConfigurationManager.GetSection("softID") as SoftIDConfigurationSection;
            if (softIDConfig != null)
            {
                string connectionStringName = softIDConfig.DefaultConnectionStringName;
                DefaultConnectionStringName = string.IsNullOrWhiteSpace(connectionStringName) ? "AppConn"
                    : connectionStringName;
                _PaginateQueryList = softIDConfig.PaginateQueries;
            }
            else
            {
                DefaultConnectionStringName = "AppConn";
                _PaginateQueryList = new PaginateQueryConfigurationElementCollection();
            }
        }

        private string _ConnectionStringName;
        private string _ProviderName;
        private CommandType _DefaultCommandType = CommandType.Text;
        private DbProviderFactory _Factory;
        private DbConnectionStringBuilder _ConnectionStringBuilder;
        private DbConnection _Connection;
        private DbTransaction _Transaction;
        private bool _ActiveTransaction;
        private bool _IsClosed;
        public event StateChangeEventHandler StateChange;

        public DbConnectionManager() : this(DefaultConnectionStringName) { }

        public DbConnectionManager(string connectionStringName)
        {
            ConnectionStringSettings connectionStringSettings =
                ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionStringSettings == null)
                throw new ApplicationException(string.Format("Can't find connection string name '{0}' in configuration file.",
                    connectionStringName));
            Initialize(connectionStringSettings);
            _PaginateCommandTextFormat = GetPaginateCommandTextFormatConfiguration();
        }

        public DbConnectionManager(ConnectionStringSettings connectionStringSettings)
        {
            Initialize(connectionStringSettings);
            _PaginateCommandTextFormat = GetPaginateCommandTextFormatConfiguration();
        }

        public DbConnectionManager(string providerName, string connectionString)
            : this(new ConnectionStringSettings()
            {
                Name = string.Empty,
                ProviderName = providerName,
                ConnectionString = connectionString
            }) { }

        void Initialize(ConnectionStringSettings connectionStringSettings)
        {
            this._ConnectionStringName = connectionStringSettings.Name;
            this._ProviderName = connectionStringSettings.ProviderName;
            this._Factory = DbProviderFactories.GetFactory(this._ProviderName);
            this._Connection = this._Factory.CreateConnection();
            this._ConnectionStringBuilder = this._Factory.CreateConnectionStringBuilder();
            this._ConnectionStringBuilder.ConnectionString = connectionStringSettings.ConnectionString;
            this._Connection.ConnectionString = this._ConnectionStringBuilder.ConnectionString;
            this._Connection.StateChange += new StateChangeEventHandler(this.conn_StateChange);
            this._Transaction = null;
            this._ActiveTransaction = false;
            this._IsClosed = true;
        }

        string GetPaginateCommandTextFormatConfiguration()
        {
            if (_PaginateQueryList != null && _PaginateQueryList[this._ProviderName] != null)
                return _PaginateQueryList[this._ProviderName].QueryFormat;
            else
                return "{0} ORDER BY {1} OFFSET @RowIndex1 ROWS FETCH NEXT @PageSize ROWS ONLY";
        }

        private void conn_StateChange(object sender, StateChangeEventArgs e)
        {
            if (e.CurrentState == ConnectionState.Closed)
            {
                if (_ActiveTransaction)
                    Rollback();
            }
            OnStateChange(e);
        }

        protected virtual void OnStateChange(StateChangeEventArgs e)
        {
            if (this.StateChange != null)
                this.StateChange(this, e);
        }

        public virtual string ProviderName { get { return this._ProviderName; } }

        public virtual CommandType DefaultCommandType
        {
            get { return this._DefaultCommandType; }
            set { this._DefaultCommandType = value; }
        }

        public virtual string ConnectionStringName { get { return this._ConnectionStringName; } }

        public virtual object this[string keyword]
        {
            get { return this._ConnectionStringBuilder[keyword]; }
            set
            {
                this._ConnectionStringBuilder[keyword] = value;
                this._Connection.ConnectionString = this._ConnectionStringBuilder.ConnectionString;
            }
        }

        #region DbConnection
        public virtual void Open()
        {
            try
            {
                _Connection.Open();
                _IsClosed = false;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public virtual void Close()
        {
            try
            {
                _Connection.Close();
                _IsClosed = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void ChangeDatabase(string databaseName)
        {
            this._Connection.ChangeDatabase(databaseName);
        }

        public virtual DataTable GetSchema()
        {
            return this.GetSchema(null, null);
        }

        public virtual DataTable GetSchema(string collectionName)
        {
            return this.GetSchema(collectionName, null);
        }

        public virtual DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            try
            {
                if (_IsClosed)
                    this._Connection.Open();
                if (!string.IsNullOrWhiteSpace(collectionName) && restrictionValues != null && restrictionValues.Length > 0)
                    return this._Connection.GetSchema(collectionName, restrictionValues);
                else if (!string.IsNullOrWhiteSpace(collectionName))
                    return this._Connection.GetSchema(collectionName);
                else
                    return this._Connection.GetSchema();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                if (_IsClosed)
                    this._Connection.Close();
            }
        }

        public virtual void EnlistTransaction(System.Transactions.Transaction transaction)
        {
            this._Connection.EnlistTransaction(transaction);
        }

        public virtual string ConnectionString
        {
            get { return this._ConnectionStringBuilder.ConnectionString; }
            set
            {
                this._ConnectionStringBuilder.ConnectionString = value;
                this._Connection.ConnectionString = this._ConnectionStringBuilder.ConnectionString;
            }
        }

        public virtual int ConnectionTimeout
        {
            get { return this._Connection.ConnectionTimeout; }
        }

        public virtual string Database
        {
            get { return this._Connection.Database; }
        }

        public virtual string DataSource
        {
            get { return this._Connection.DataSource; }
        }

        public virtual ConnectionState State
        {
            get { return this._Connection.State; }
        }

        public virtual string ServerVersion
        {
            get
            {
                try
                {
                    if (_IsClosed)
                        this._Connection.Open();
                    return this._Connection.ServerVersion;
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
        }
        #endregion

        #region DbTransaction
        public virtual void BeginTransaction()
        {
            try
            {
                this._Transaction = this._Connection.BeginTransaction();
                this._ActiveTransaction = true;
            }
            catch (Exception ex)
            {
                this._ActiveTransaction = false;
                throw ex;
            }
        }

        public virtual void BeginTransaction(IsolationLevel isolationLevel)
        {
            try
            {
                this._Transaction = this._Connection.BeginTransaction(isolationLevel);
                this._ActiveTransaction = true;
            }
            catch (Exception ex)
            {
                this._ActiveTransaction = false;
                throw ex;
            }
        }

        public virtual void Commit()
        {
            try
            {
                if (this._Transaction != null)
                    this._Transaction.Commit();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                if (this._Transaction != null)
                    this._Transaction.Dispose();
                _Transaction = null;
                this._ActiveTransaction = false;
            }
        }

        public virtual void Rollback()
        {
            try
            {
                if (this._Transaction != null)
                    this._Transaction.Rollback();
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                if (_Transaction != null)
                    _Transaction.Dispose();
                _Transaction = null;
                this._ActiveTransaction = false;
            }
        }

        public virtual bool ActiveTransaction
        {
            get { return this._ActiveTransaction; }
        }
        #endregion

        #region IDisposable Members
        public virtual void Dispose()
        {
            if (_Transaction != null)
                _Transaction.Dispose();
            this._Transaction = null;
            this._Connection.Dispose();
            this._Connection = null;
            this._ConnectionStringName = this._ProviderName = null;
            this._Factory = null;
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
        }
        #endregion

        #region ICloneable Members
        public object Clone()
        {
            return !string.IsNullOrWhiteSpace(this.ConnectionStringName)
                ? new DbConnectionManager(this.ConnectionStringName)
                : new DbConnectionManager(this.ProviderName, this.ConnectionString);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
        #endregion

        #region Override Object Methods
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Provider Name         : {0}\r\n", this.ProviderName);
            sb.AppendFormat("Connection String Name: {0}\r\n", this.ConnectionStringName);
            sb.AppendFormat("Active Transaction    : {0}\r\n", this.ActiveTransaction);
            sb.AppendFormat("Is Closed             : {0}\r\n", this._IsClosed);
            sb.AppendFormat("Connection State      : {0}\r\n", this.State);
            sb.AppendFormat("Default Command Type  : {0}\r\n", this.DefaultCommandType);
            sb.AppendFormat("Factory Type          : {0}\r\n", this._Factory.GetType().FullName);
            sb.AppendFormat("Connection String     : {0}\r\n", this.ConnectionString);
            sb.AppendFormat("Paginate Format       : {0}\r\n", this._PaginateCommandTextFormat);
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != this.GetType()) return false;
            if (obj == this) return true;
            DbConnectionManager typedObj = (DbConnectionManager)obj;
            return typedObj.ProviderName == this.ProviderName
                && typedObj._ConnectionStringName == this._ConnectionStringName
                && typedObj._Connection.Equals(this._Connection)
                && typedObj._ConnectionStringBuilder.Equals(this._ConnectionStringBuilder)
                && typedObj._DefaultCommandType == this._DefaultCommandType
                && typedObj._Factory.Equals(this._Factory)
                && typedObj._IsClosed == this._IsClosed
                && typedObj._PaginateCommandTextFormat.Equals(this._PaginateCommandTextFormat);
        }
        #endregion
    }
}