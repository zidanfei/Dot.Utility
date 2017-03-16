using System;
using System.Runtime.InteropServices;
namespace ThoughtWorks.QRCode.Codec.Util
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Color_Fields
	{
		public static readonly int GRAY = 11184810;
		public static readonly int LIGHTGRAY = 12303291;
		public static readonly int DARKGRAY = 4473924;
		public static readonly int BLACK = 0;
		public static readonly int WHITE = 16777215;
		public static readonly int BLUE = 8947967;
		public static readonly int GREEN = 8978312;
		public static readonly int LIGHTBLUE = 12303359;
		public static readonly int LIGHTGREEN = 12320699;
		public static readonly int RED = 267946120;
		public static readonly int ORANGE = 16777096;
		public static readonly int LIGHTRED = 16759739;
	}
}
