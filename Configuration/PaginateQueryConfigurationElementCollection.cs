using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftID.Configuration
{
    public class PaginateQueryConfigurationElementCollection : ConfigurationElementCollection
    {
        public PaginateQuery this[int index]
        {
            get { return base.BaseGet(index) as PaginateQuery; }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public PaginateQuery this[string providerName]
        {
            get { return base.BaseGet(providerName) as PaginateQuery; }
            set
            {
                if (base.BaseGet(providerName) != null)
                {
                    base.BaseRemove(providerName);
                }
                base.BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PaginateQuery();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PaginateQuery)element).ProviderName;
        }
    }
}
