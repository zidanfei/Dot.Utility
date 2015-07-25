namespace Dot.Utility
{
    using Microsoft.Win32;
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public static class WindowsHelper
    {
        public static void DoWindowsEvents()
        {
            Application.DoEvents();
        }

        public static Point GetCursorPosition()
        {
            return Cursor.Position;
        }

        private static Icon GetIcon(string path, FILE_ATTRIBUTE dwAttr, SHGFI dwFlag)
        {
            SHFILEINFO sfi = new SHFILEINFO();
            int num1 = (int)SHGetFileInfo(path, dwAttr, ref sfi, 0, dwFlag);
            return Icon.FromHandle(sfi.hIcon);
        }

        public static Form GetMdiChildForm(Form parentForm, System.Type childFormType)
        {
            foreach (Form form in parentForm.MdiChildren)
            {
                if (form.GetType() == childFormType)
                {
                    return form;
                }
            }
            return null;
        }

        public static string GetStartupDirectoryPath()
        {
            return Application.StartupPath;
        }

        public static Icon GetSystemIconByFileType(string fileType, bool isLarge)
        {
            if (isLarge)
            {
                return GetIcon(fileType, FILE_ATTRIBUTE.NORMAL, SHGFI.ICON | SHGFI.USEFILEATTRIBUTES);
            }
            return GetIcon(fileType, FILE_ATTRIBUTE.NORMAL, SHGFI.SMALLICON | SHGFI.ICON | SHGFI.USEFILEATTRIBUTES);
        }

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte key, byte bScan, KeybdEventFlag flags, uint dwExtraInfo);
        public static bool MdiChildIsExist(Form parentForm, System.Type childFormType)
        {
            return (GetMdiChildForm(parentForm, childFormType) != null);
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);
        public static void RunWhenStart_usingReg(bool started, string name, string path)
        {
            RegistryKey localMachine = Registry.LocalMachine;
            try
            {
                RegistryKey key2 = localMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\");
                if (started)
                {
                    key2.SetValue(name, path);
                }
                else
                {
                    key2.DeleteValue(name);
                }
            }
            finally
            {
                localMachine.Close();
            }
        }

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);
        [DllImport("shell32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SHGetFileInfo(string pszPath, FILE_ATTRIBUTE dwFileAttributes, ref SHFILEINFO sfi, int cbFileInfo, SHGFI uFlags);
        public static bool ShowQuery(string query)
        {
            if (DialogResult.Yes != MessageBox.Show(query, "提示", MessageBoxButtons.YesNo))
            {
                return false;
            }
            return true;
        }

        [Flags]
        public enum FILE_ATTRIBUTE
        {
            ARCHIVE = 0x20,
            COMPRESSED = 0x800,
            DEVICE = 0x40,
            DIRECTORY = 0x10,
            ENCRYPTED = 0x4000,
            HIDDEN = 2,
            NORMAL = 0x80,
            NOT_CONTENT_INDEXED = 0x2000,
            OFFLINE = 0x1000,
            READONLY = 1,
            REPARSE_POINT = 0x400,
            SPARSE_FILE = 0x200,
            SYSTEM = 4,
            TEMPORARY = 0x100
        }

        [Flags]
        public enum KeybdEventFlag : uint
        {
            Down = 0,
            Up = 2
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [Flags]
        public enum SHGFI : uint
        {
            ADDOVERLAYS = 0x20,
            ATTR_SPECIFIED = 0x20000,
            ATTRIBUTES = 0x800,
            DISPLAYNAME = 0x200,
            EXETYPE = 0x2000,
            ICON = 0x100,
            ICONLOCATION = 0x1000,
            LARGEICON = 0,
            LINKOVERLAY = 0x8000,
            OPENICON = 2,
            OVERLAYINDEX = 0x40,
            PIDL = 8,
            SELECTED = 0x10000,
            SHELLICONSIZE = 4,
            SMALLICON = 1,
            SYSICONINDEX = 0x4000,
            TYPENAME = 0x400,
            USEFILEATTRIBUTES = 0x10
        }

        [Flags]
        public enum MouseEventFlag : uint
        {
            Absolute = 0x8000,
            LeftDown = 2,
            LeftUp = 4,
            MiddleDown = 0x20,
            MiddleUp = 0x40,
            Move = 1,
            RightDown = 8,
            RightUp = 0x10,
            VirtualDesk = 0x4000,
            Wheel = 0x800,
            XDown = 0x80,
            XUp = 0x100
        }
    }
}
