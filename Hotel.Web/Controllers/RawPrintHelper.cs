using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Hotel.Controllers
{
    public static class RawPrintHelper
    {
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int DocVersion, [In] DocInfo di);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBuf, int cdBuf, out int pWritten);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class DocInfo
        {
            public int DocVersion = 1;
            public string DocName;
            public string OutputFile;
            public string Datatype;
        }

        public static void SendFileToPrinter(string printerName, string filePath)
        {
            IntPtr printerHandle;
            OpenPrinter(printerName, out printerHandle, IntPtr.Zero);

            DocInfo docInfo = new DocInfo();
            docInfo.DocName = Path.GetFileName(filePath);
            docInfo.OutputFile = null;
            docInfo.Datatype = "RAW";

            StartDocPrinter(printerHandle, 1, docInfo);

            byte[] bytes = File.ReadAllBytes(filePath);
            IntPtr pBytes = Marshal.AllocCoTaskMem(bytes.Length);
            Marshal.Copy(bytes, 0, pBytes, bytes.Length);

            int written;
            WritePrinter(printerHandle, pBytes, bytes.Length, out written);

            Marshal.FreeCoTaskMem(pBytes);
            EndDocPrinter(printerHandle);
            ClosePrinter(printerHandle);
        }
    }
}