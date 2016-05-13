using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftID.Configuration
{
    public class SoftIDConfigurationSection : ConfigurationSection
    {
        public SoftIDConfigurationSection() { }

        [ConfigurationProperty("paginateQueries")]
        public PaginateQueryConfigurationElementCollection PaginateQueries
        {
            get { return base["paginateQueries"] as PaginateQueryConfigurationElementCollection; }
            set { base["paginateQueries"] = value; }
        }

        [ConfigurationProperty("defaultConnectionStringName")]
        public string DefaultConnectionStringName
        {
            get { return base["defaultConnectionStringName"] as string; }
            set { base["defaultConnectionStringName"] = value; }
        }
    }
}
