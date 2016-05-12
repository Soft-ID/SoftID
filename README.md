# SoftID
Lightweight Database App Library

1. Key Features
   1. Automatically open and close the connection when sql query execution.
   2. Can open and close the connection manually when using transaction or data reader.
   3. Transform data reader to type T by sending Func&lt;IDataRecord, T&gt; object to ExecuteEnumerableList or ExecutePaginateEnumerableList method.
   4. Database vendor independent by using ADO.NET Factory Framework (System.Data.Common namespace).
2. Getting Started
   1. Configuration
      &lt;configuration&gt;
        &lt;configSections&gt;
          &lt;section name="softID"
                   type="SoftID.Configuration.SoftIDConfigurationSection, SoftID" /&gt;
        &lt;/configSections&gt;
        &lt;softID defaultConnectionStringName="DefaultConnection"&gt;
          &lt;paginateQueries&gt;
            &lt;add providerName="System.Data.SqlClient"
                 queryFormat="{0} ORDER BY {1} OFFSET @RowIndex1 ROWS FETCH NEXT @PageSize ROWS ONLY" /&gt;
            &lt;add providerName="MySql.Data.MySqlClient"
                 queryFormat="{0} ORDER BY {1} LIMIT @RowIndex1, @PageSize" /&gt;
          &lt;/paginateQueries&gt;
        &lt;/softID&gt;
        &lt;connectionStrings&gt;
          &lt;add name="DefaultConnection" providerName="System.Data.SqlClient"
               connectionString="Data Source=localhost\SQLEXPRESS;Initial Catalog=dbname;User ID=sa;Password=sapassword" /&gt;
        &lt;/connectionStrings&gt;
      &lt;/configuration&gt;
   2. Using SoftID.Data.DbConnectionManager object
      using SoftID.Data;
      using SoftID.Utilities;
      using System;
      using System.Data;

      namespace Sample
      {
        class Program
        {
          static void Main(string[] args)
          {
            /* Default constructor uses connectionStringName that is set
               by defaultConnectionStringName attribute in softID configuration section */
            DbConnectionManager db = new DbConnectionManager();
            Func<IDataRecord, Personal> fMap = rec =>
            {
              if (rec == null)
                throw new ArgumentNullException(nameof(rec));
              Personal pa = new Personal()
              {
                ID = rec["ID"].ToNullable<int>(), /* Using SoftID.Utilities.Converter.ToNullable<T>(object value) method */
                PersonalName = rec["PersonalName"] as string,
                BirthDate = rec["BirthDate"].ToStruct<DateTime>(),
                IDCardNumber = rec["IDCardNumber"] as string
              };
              return pa;
            };
            int pageIndex = 0,
                pageSize = 2,
                totalRecords;
            Console.WriteLine("ConnectionState: {0}", db.State); /* ConnectionState close here */
            foreach (var pa in db.ExecutePaginateEnumerableList<Personal>(ref pageIndex, pageSize, out totalRecords,
                "ID", "SELECT * FROM Personal", fMap))
            {
              Console.WriteLine("{0} : {1}", nameof(pa.ID), pa.ID);
              Console.WriteLine("{0} : {1}", nameof(pa.PersonalName), pa.PersonalName);
              Console.WriteLine("{0} : {1}", nameof(pa.BirthDate), pa.BirthDate);
              Console.WriteLine("{0} : {1}", nameof(pa.IDCardNumber), pa.IDCardNumber);
              Console.WriteLine("{0}\r\n", db.State); /* ConnectionState and reader open here */
            }
            Console.WriteLine("ConnectionState: {0}", db.State); /* ConnectionState auto close here */
            Console.WriteLine("Page Index: {0}, PageSize: {1}, TotalRecords: {2},
                pageIndex, pageSize, totalRecords); /* Total record info obtained */
            Console.Read();
          }
        }
        
        public class Personal
        {
          public int? ID { get; set; }
          public string PersonalName { get; set; }
          public DateTime BirthDate { get; set; }
          public string IDCardNumber { get; set; }
        }
      }
