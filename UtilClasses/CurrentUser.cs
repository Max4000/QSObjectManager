using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Management;

namespace UtilClasses
{
    [SuppressMessage("ReSharper", "CommentTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CurrentUser
    {
        public static string GetExplorerUser()
        {
            var query = new ObjectQuery("SELECT * FROM Win32_Process WHERE Name='explorer.exe' AND ProcessId=" +
                                        GetParentExplorerProcessId());
            var explorerProcesses = new ManagementObjectSearcher(query).Get();
            foreach (ManagementObject mo in explorerProcesses)
            {
                string[] ownerInfo = new string[2];
                mo.InvokeMethod("GetOwner", (object[]) ownerInfo);
                return ownerInfo[1] + "\\" + ownerInfo[0];
            }

            return "";
        }

        private static uint GetParentExplorerProcessId()
        {
            return GetParentExplorerProcessId(Process.GetCurrentProcess().Id);
        }

        private static uint GetParentExplorerProcessId(int processId)
        {
            const uint ERROR = 0;

            IntPtr hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
            if (hSnapshot == IntPtr.Zero) return ERROR;

            PROCESSENTRY32 procEntry = new PROCESSENTRY32();
            procEntry.dwSize = (uint) Marshal.SizeOf(typeof(PROCESSENTRY32));

            if (Process32First(hSnapshot, ref procEntry) == false) return ERROR;

            List<PROCESSENTRY32> ids = new List<PROCESSENTRY32>();
            do
            {
                ids.Add(procEntry);
            } while (Process32Next(hSnapshot, ref procEntry));

            CloseHandle(hSnapshot);

            uint parentId = ids.Where(pe => pe.th32ProcessID == processId).FirstOrDefault().th32ParentProcessID;
            for (; parentId != 0;)
            {
                string name = ids.Where(pe => pe.th32ProcessID == parentId).FirstOrDefault().szExeFile;
                if ("explorer.exe".Equals(name, StringComparison.OrdinalIgnoreCase)) return parentId;

                parentId = ids.Where(pe => pe.th32ProcessID == parentId).FirstOrDefault().th32ParentProcessID;
            }

            return ERROR;
        }

        const uint TH32CS_SNAPPROCESS = 2;

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        };

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll")]
        static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);
    }
}