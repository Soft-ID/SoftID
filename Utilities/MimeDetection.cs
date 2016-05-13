using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace SoftID.Utilities
{
    public static class MimeDetection
    {
        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static System.UInt32 FindMimeFromData(
            System.UInt32 pBC,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            System.UInt32 cbSize,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzMimeProposed,
            System.UInt32 dwMimeFlags,
            out System.UInt32 ppwzMimeOut,
            System.UInt32 dwReserverd
        );

        public static string ToMime(this byte[] bytes)
        {
            uint length = (uint)Math.Min(256, (int)bytes.LongLength);
            try
            {
                System.UInt32 mimetype;
                FindMimeFromData(0, null, bytes, length, null, 0, out mimetype, 0);
                System.IntPtr mimeTypePtr = new IntPtr(mimetype);
                string mime = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
                return mime;
            }
            catch (Exception)
            {
                return "application/octet-stream";
            }
        }

        public static string ToMime(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            stream.Position = 0;
            byte[] buffer = new byte[256];
            int length = Math.Min(256, (int)stream.Length);
            stream.Read(buffer, 0, length);
            stream.Position = 0;
            return ToMime(buffer);
        }

        public static string MimeToExtension(string mime)
        {
            RegistryKey regKey = Registry.ClassesRoot;
            regKey = regKey.OpenSubKey("MIME").OpenSubKey("Database").OpenSubKey("Content Type");
            regKey = regKey.OpenSubKey(mime);
            string ext = regKey == null ? string.Empty : regKey.GetValue("Extension") as string;
            return ext;
        }
    }
}