using System;
namespace ThoughtWorks.QRCode.Codec.Ecc
{
	public class BCH15_5
	{
		internal int[][] gf16;
		internal bool[] recieveData;
		internal int numCorrectedError;
		internal static string[] bitName = new string[]
		{
			"c0",
			"c1",
			"c2",
			"c3",
			"c4",
			"c5",
			"c6",
			"c7",
			"c8",
			"c9",
			"d0",
			"d1",
			"d2",
			"d3",
			"d4"
		};
		public virtual int NumCorrectedError
		{
			get
			{
				return this.numCorrectedError;
			}
		}
		public BCH15_5(bool[] source)
		{
			this.gf16 = this.createGF16();
			this.recieveData = source;
		}
		public virtual bool[] correct()
		{
			int[] s = this.calcSyndrome(this.recieveData);
			int[] errorPos = this.detectErrorBitPosition(s);
			return this.correctErrorBit(this.recieveData, errorPos);
		}
		internal virtual int[][] createGF16()
		{
			this.gf16 = new int[16][];
			for (int i = 0; i < 16; i++)
			{
				this.gf16[i] = new int[4];
			}
			int[] array = new int[4];
			array[0] = 1;
			array[1] = 1;
			int[] seed = array;
			for (int i = 0; i < 4; i++)
			{
				this.gf16[i][i] = 1;
			}
			for (int i = 0; i < 4; i++)
			{
				this.gf16[4][i] = seed[i];
			}
			for (int i = 5; i < 16; i++)
			{
				for (int j = 1; j < 4; j++)
				{
					this.gf16[i][j] = this.gf16[i - 1][j - 1];
				}
				if (this.gf16[i - 1][3] == 1)
				{
					for (int j = 0; j < 4; j++)
					{
						this.gf16[i][j] = (this.gf16[i][j] + seed[j]) % 2;
					}
				}
			}
			return this.gf16;
		}
		internal virtual int searchElement(int[] x)
		{
			int i;
			for (i = 0; i < 15; i++)
			{
				if (x[0] == this.gf16[i][0] && x[1] == this.gf16[i][1] && x[2] == this.gf16[i][2] && x[3] == this.gf16[i][3])
				{
					break;
				}
			}
			return i;
		}
		internal virtual int[] getCode(int input)
		{
			int[] f = new int[15];
			int[] r = new int[8];
			for (int i = 0; i < 15; i++)
			{
				int w = r[7];
				int yin;
				int w2;
				if (i < 7)
				{
					yin = (input >> 6 - i) % 2;
					w2 = (yin + w) % 2;
				}
				else
				{
					yin = w;
					w2 = 0;
				}
				r[7] = (r[6] + w2) % 2;
				r[6] = (r[5] + w2) % 2;
				r[5] = r[4];
				r[4] = (r[3] + w2) % 2;
				r[3] = r[2];
				r[2] = r[1];
				r[1] = r[0];
				r[0] = w2;
				f[14 - i] = yin;
			}
			return f;
		}
		internal virtual int addGF(int arg1, int arg2)
		{
			int[] p = new int[4];
			for (int i = 0; i < 4; i++)
			{
				int w = (arg1 < 0 || arg1 >= 15) ? 0 : this.gf16[arg1][i];
				int w2 = (arg2 < 0 || arg2 >= 15) ? 0 : this.gf16[arg2][i];
				p[i] = (w + w2) % 2;
			}
			return this.searchElement(p);
		}
		internal virtual int[] calcSyndrome(bool[] y)
		{
			int[] s = new int[5];
			int[] p = new int[4];
			int i;
			for (i = 0; i < 15; i++)
			{
				if (y[i])
				{
					for (int j = 0; j < 4; j++)
					{
						p[j] = (p[j] + this.gf16[i][j]) % 2;
					}
				}
			}
			i = this.searchElement(p);
			s[0] = ((i >= 15) ? -1 : i);
			p = new int[4];
			for (i = 0; i < 15; i++)
			{
				if (y[i])
				{
					for (int j = 0; j < 4; j++)
					{
						p[j] = (p[j] + this.gf16[i * 3 % 15][j]) % 2;
					}
				}
			}
			i = this.searchElement(p);
			s[2] = ((i >= 15) ? -1 : i);
			p = new int[4];
			for (i = 0; i < 15; i++)
			{
				if (y[i])
				{
					for (int j = 0; j < 4; j++)
					{
						p[j] = (p[j] + this.gf16[i * 5 % 15][j]) % 2;
					}
				}
			}
			i = this.searchElement(p);
			s[4] = ((i >= 15) ? -1 : i);
			return s;
		}
		internal virtual int[] calcErrorPositionVariable(int[] s)
		{
			int[] e = new int[4];
			e[0] = s[0];
			int t = (s[0] + s[1]) % 15;
			int mother = this.addGF(s[2], t);
			mother = ((mother >= 15) ? -1 : mother);
			t = (s[2] + s[1]) % 15;
			int child = this.addGF(s[4], t);
			child = ((child >= 15) ? -1 : child);
			e[1] = ((child < 0 && mother < 0) ? -1 : ((child - mother + 15) % 15));
			t = (s[1] + e[0]) % 15;
			int t2 = this.addGF(s[2], t);
			t = (s[0] + e[1]) % 15;
			e[2] = this.addGF(t2, t);
			return e;
		}
		internal virtual int[] detectErrorBitPosition(int[] s)
		{
			int[] e = this.calcErrorPositionVariable(s);
			int[] errorPos = new int[4];
			int[] result;
			if (e[0] == -1)
			{
				result = errorPos;
			}
			else
			{
				if (e[1] == -1)
				{
					errorPos[0] = 1;
					errorPos[1] = e[0];
					result = errorPos;
				}
				else
				{
					for (int i = 0; i < 15; i++)
					{
						int x3 = i * 3 % 15;
						int x4 = i * 2 % 15;
						int x5 = i;
						int t = (e[0] + x4) % 15;
						int t2 = this.addGF(x3, t);
						t = (e[1] + x5) % 15;
						int t3 = this.addGF(t, e[2]);
						int anError = this.addGF(t2, t3);
						if (anError >= 15)
						{
							errorPos[0]++;
							errorPos[errorPos[0]] = i;
						}
					}
					result = errorPos;
				}
			}
			return result;
		}
		internal virtual bool[] correctErrorBit(bool[] y, int[] errorPos)
		{
			for (int i = 1; i <= errorPos[0]; i++)
			{
				y[errorPos[i]] = !y[errorPos[i]];
			}
			this.numCorrectedError = errorPos[0];
			return y;
		}
	}
}
