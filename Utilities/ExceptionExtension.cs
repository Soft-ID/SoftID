using System;
using System.IO;
using System.Text;
using System.Data;
using System.Data.Common;

namespace SoftID.Utilities
{
    public static class ExceptionExtension
    {
        public static string ConcatAllExceptionMessage(this Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            while (ex != null)
            {
                sb.Append(ex.Message);
                sb.AppendLine();
                ex = ex.InnerException;
            }
            if (sb.Length > 0)
                sb.Remove(sb.Length - 2, 2);
            string msg = sb.ToString();
            sb.Remove(0, sb.Length);
            sb = null;
            return msg;
        }
    }
}