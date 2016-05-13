using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftID.Configuration
{
    public class PaginateQuery : ConfigurationElement
    {
        [ConfigurationProperty("providerName", DefaultValue = "", IsKey = true)]
        public string ProviderName
        {
            get { return base["providerName"] as string; }
            set { base["providerName"] = value; }
        }

        [ConfigurationProperty("queryFormat", DefaultValue = "")]
        public string QueryFormat
        {
            get { return base["queryFormat"] as string; }
            set { base["queryFormat"] = value; }
        }

    }
}
