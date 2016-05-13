using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SoftID.Data
{
    public class DuplicateKeyException : ApplicationException
    {
        public DuplicateKeyException() : this(string.Empty, null) { }

        public DuplicateKeyException(string message) : this(message, null) { }

        public DuplicateKeyException(string message, Exception innerException)
            : base(string.IsNullOrEmpty(message) ?
            "Violation of PRIMARY KEY constraint. Cannot insert duplicate key." : message,
            innerException) { }

        public DuplicateKeyException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
