using System;
namespace ThoughtWorks.QRCode.Codec.Reader.Pattern
{
	public class LogicalSeed
	{
		private static int[][] seed;
		public static int[] getSeed(int version)
		{
			return LogicalSeed.seed[version - 1];
		}
		public static int getSeed(int version, int patternNumber)
		{
			return LogicalSeed.seed[version - 1][patternNumber];
		}
		static LogicalSeed()
		{
			LogicalSeed.seed = new int[40][];
			LogicalSeed.seed[0] = new int[]
			{
				6,
				14
			};
			LogicalSeed.seed[1] = new int[]
			{
				6,
				18
			};
			LogicalSeed.seed[2] = new int[]
			{
				6,
				22
			};
			LogicalSeed.seed[3] = new int[]
			{
				6,
				26
			};
			LogicalSeed.seed[4] = new int[]
			{
				6,
				30
			};
			LogicalSeed.seed[5] = new int[]
			{
				6,
				34
			};
			LogicalSeed.seed[6] = new int[]
			{
				6,
				22,
				38
			};
			LogicalSeed.seed[7] = new int[]
			{
				6,
				24,
				42
			};
			LogicalSeed.seed[8] = new int[]
			{
				6,
				26,
				46
			};
			LogicalSeed.seed[9] = new int[]
			{
				6,
				28,
				50
			};
			LogicalSeed.seed[10] = new int[]
			{
				6,
				30,
				54
			};
			LogicalSeed.seed[11] = new int[]
			{
				6,
				32,
				58
			};
			LogicalSeed.seed[12] = new int[]
			{
				6,
				34,
				62
			};
			LogicalSeed.seed[13] = new int[]
			{
				6,
				26,
				46,
				66
			};
			LogicalSeed.seed[14] = new int[]
			{
				6,
				26,
				48,
				70
			};
			LogicalSeed.seed[15] = new int[]
			{
				6,
				26,
				50,
				74
			};
			LogicalSeed.seed[16] = new int[]
			{
				6,
				30,
				54,
				78
			};
			LogicalSeed.seed[17] = new int[]
			{
				6,
				30,
				56,
				82
			};
			LogicalSeed.seed[18] = new int[]
			{
				6,
				30,
				58,
				86
			};
			LogicalSeed.seed[19] = new int[]
			{
				6,
				34,
				62,
				90
			};
			LogicalSeed.seed[20] = new int[]
			{
				6,
				28,
				50,
				72,
				94
			};
			LogicalSeed.seed[21] = new int[]
			{
				6,
				26,
				50,
				74,
				98
			};
			LogicalSeed.seed[22] = new int[]
			{
				6,
				30,
				54,
				78,
				102
			};
			LogicalSeed.seed[23] = new int[]
			{
				6,
				28,
				54,
				80,
				106
			};
			LogicalSeed.seed[24] = new int[]
			{
				6,
				32,
				58,
				84,
				110
			};
			LogicalSeed.seed[25] = new int[]
			{
				6,
				30,
				58,
				86,
				114
			};
			LogicalSeed.seed[26] = new int[]
			{
				6,
				34,
				62,
				90,
				118
			};
			LogicalSeed.seed[27] = new int[]
			{
				6,
				26,
				50,
				74,
				98,
				122
			};
			LogicalSeed.seed[28] = new int[]
			{
				6,
				30,
				54,
				78,
				102,
				126
			};
			LogicalSeed.seed[29] = new int[]
			{
				6,
				26,
				52,
				78,
				104,
				130
			};
			LogicalSeed.seed[30] = new int[]
			{
				6,
				30,
				56,
				82,
				108,
				134
			};
			LogicalSeed.seed[31] = new int[]
			{
				6,
				34,
				60,
				86,
				112,
				138
			};
			LogicalSeed.seed[32] = new int[]
			{
				6,
				30,
				58,
				86,
				114,
				142
			};
			LogicalSeed.seed[33] = new int[]
			{
				6,
				34,
				62,
				90,
				118,
				146
			};
			LogicalSeed.seed[34] = new int[]
			{
				6,
				30,
				54,
				78,
				102,
				126,
				150
			};
			LogicalSeed.seed[35] = new int[]
			{
				6,
				24,
				50,
				76,
				102,
				128,
				154
			};
			LogicalSeed.seed[36] = new int[]
			{
				6,
				28,
				54,
				80,
				106,
				132,
				158
			};
			LogicalSeed.seed[37] = new int[]
			{
				6,
				32,
				58,
				84,
				110,
				136,
				162
			};
			LogicalSeed.seed[38] = new int[]
			{
				6,
				26,
				54,
				82,
				110,
				138,
				166
			};
			LogicalSeed.seed[39] = new int[]
			{
				6,
				30,
				58,
				86,
				114,
				142,
				170
			};
		}
	}
}
