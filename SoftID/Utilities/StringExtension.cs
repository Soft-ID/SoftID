using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftID.Utilities
{
    public static class StringExtension
    {
        public static string ToLikeParameterFormat(this string likeParameter)
        {
            return string.IsNullOrWhiteSpace(likeParameter) ? "%" : string.Format("%{0}%", likeParameter);
        }

        public static string Left(this string text, int length)
        {
            if (text == null) return text;
            return text.Substring(0, Math.Min(text.Length, length));
        }

        public static string Right(this string text, int length)
        {
            if (text == null) return text;
            return text.Substring(Math.Max(text.Length - length, 0));
        }

        public static string JavascriptEncode(string jsText)
        {
            if (string.IsNullOrEmpty(jsText))
                return jsText;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < jsText.Length; i++)
            {
                char c = jsText[i];
                switch (c)
                {
                    case '\b':
                        sb.Append('\\').Append('b');
                        break;
                    case '\f':
                        sb.Append('\\').Append('f');
                        break;
                    case '\n':
                        sb.Append('\\').Append('n');
                        break;
                    case '\r':
                        sb.Append('\\').Append('r');
                        break;
                    case '\t':
                        sb.Append('\\').Append('t');
                        break;
                    case '\v':
                        sb.Append('\\').Append('v');
                        break;
                    case '\'':
                        sb.Append('\\').Append('\'');
                        break;
                    case '"':
                        sb.Append('\\').Append('"');
                        break;
                    case '\\':
                        sb.Append('\\').Append('\\');
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            string jsTextEncoded = sb.ToString();
            sb.Remove(0, sb.Length);
            sb = null;
            return jsTextEncoded;
        }
    }
}