using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.Consoles
{

    public class ConsoleColorHelper
    {
        private int hConsoleHandle;
        private COORD ConsoleOutputLocation;
        private CONSOLE_SCREEN_BUFFER_INFO ConsoleInfo;
        private int OriginalColors;

        private const int STD_OUTPUT_HANDLE = -11;

        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true,
                        CharSet = CharSet.Auto,
                        CallingConvention = CallingConvention.StdCall)]
        private static extern int GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "GetConsoleScreenBufferInfo",
                        SetLastError = true, CharSet = CharSet.Auto,
                        CallingConvention = CallingConvention.StdCall)]
        private static extern int GetConsoleScreenBufferInfo(int hConsoleOutput,
                        ref CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

        [DllImport("kernel32.dll", EntryPoint = "SetConsoleTextAttribute",
                        SetLastError = true, CharSet = CharSet.Auto,
                        CallingConvention = CallingConvention.StdCall)]
        private static extern int SetConsoleTextAttribute(int hConsoleOutput,
                                int wAttributes);

        public enum Foreground
        {
            //
            // 摘要:
            //     The color black.
            Black = 0,
            //
            // 摘要:
            //     The color dark blue.
            DarkBlue = 1,
            //
            // 摘要:
            //     The color dark green.
            DarkGreen = 2,
            //
            // 摘要:
            //     The color dark cyan (dark blue-green).
            DarkCyan = 3,
            //
            // 摘要:
            //     The color dark red.
            DarkRed = 4,
            //
            // 摘要:
            //     The color dark magenta (dark purplish-red).
            DarkMagenta = 5,
            //
            // 摘要:
            //     The color dark yellow (ochre).
            DarkYellow = 6,
            //
            // 摘要:
            //     The color gray.
            Gray = 7,
            //
            // 摘要:
            //     The color dark gray.
            DarkGray = 8,
            //
            // 摘要:
            //     The color blue.
            Blue = 9,
            //
            // 摘要:
            //     The color green.
            Green = 10,
            //
            // 摘要:
            //     The color cyan (blue-green).
            Cyan = 11,
            //
            // 摘要:
            //     The color red.
            Red = 12,
            //
            // 摘要:
            //     The color magenta (purplish-red).
            Magenta = 13,
            //
            // 摘要:
            //     The color yellow.
            Yellow = 14,
            //
            // 摘要:
            //     The color white.
            White = 15
            //Blue = 0x00000001,
            //Green = 0x00000002,
            //Red = 0x00000004,
            //Yellow
            //Intensity = 0x00000008
        }

        public enum Background
        {
            Blue = 0x00000010,
            Green = 0x00000020,
            Red = 0x00000040,
            Intensity = 0x00000080
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            short X;
            short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SMALL_RECT
        {
            short Left;
            short Top;
            short Right;
            short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD dwSize;
            public COORD dwCursorPosition;
            public int wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
        }

        // Constructor.
        public ConsoleColorHelper()
        {
            ConsoleInfo = new CONSOLE_SCREEN_BUFFER_INFO();
            ConsoleOutputLocation = new COORD();
            hConsoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            GetConsoleScreenBufferInfo(hConsoleHandle, ref ConsoleInfo);
            OriginalColors = ConsoleInfo.wAttributes;
        }

        public void TextColor(int color)
        {
            SetConsoleTextAttribute(hConsoleHandle, color);
        }

        public void ResetColor()
        {
            SetConsoleTextAttribute(hConsoleHandle, OriginalColors);
        }
    }
}
