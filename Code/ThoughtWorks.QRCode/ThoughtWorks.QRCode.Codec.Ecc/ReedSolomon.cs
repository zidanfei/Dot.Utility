using System;
namespace ThoughtWorks.QRCode.Codec.Ecc
{
	public class ReedSolomon
	{
		internal int[] y;
		internal int[] gexp = new int[512];
		internal int[] glog = new int[256];
		internal int NPAR;
		internal int MAXDEG;
		internal int[] synBytes;
		internal int[] Lambda;
		internal int[] Omega;
		internal int[] ErrorLocs = new int[256];
		internal int NErrors;
		internal int[] ErasureLocs = new int[256];
		internal int NErasures = 0;
		internal bool correctionSucceeded = true;
		public virtual bool CorrectionSucceeded
		{
			get
			{
				return this.correctionSucceeded;
			}
		}
		public virtual int NumCorrectedErrors
		{
			get
			{
				return this.NErrors;
			}
		}
		public ReedSolomon(int[] source, int NPAR)
		{
			this.initializeGaloisTables();
			this.y = source;
			this.NPAR = NPAR;
			this.MAXDEG = NPAR * 2;
			this.synBytes = new int[this.MAXDEG];
			this.Lambda = new int[this.MAXDEG];
			this.Omega = new int[this.MAXDEG];
		}
		internal virtual void initializeGaloisTables()
		{
			int p9;
			int p8;
			int p7;
			int p6;
			int p5;
			int p4;
			int p3 = p4 = (p5 = (p6 = (p7 = (p8 = (p9 = 0)))));
			int p10 = 1;
			this.gexp[0] = 1;
			this.gexp[255] = this.gexp[0];
			this.glog[0] = 0;
			for (int i = 1; i < 256; i++)
			{
				int pinit = p9;
				p9 = p8;
				p8 = p7;
				p7 = p6;
				p6 = (p5 ^ pinit);
				p5 = (p3 ^ pinit);
				p3 = (p4 ^ pinit);
				p4 = p10;
				p10 = pinit;
				this.gexp[i] = p10 + p4 * 2 + p3 * 4 + p5 * 8 + p6 * 16 + p7 * 32 + p8 * 64 + p9 * 128;
				this.gexp[i + 255] = this.gexp[i];
			}
			for (int i = 1; i < 256; i++)
			{
				for (int z = 0; z < 256; z++)
				{
					if (this.gexp[z] == i)
					{
						this.glog[i] = z;
						break;
					}
				}
			}
		}
		internal virtual int gmult(int a, int b)
		{
			int result;
			if (a == 0 || b == 0)
			{
				result = 0;
			}
			else
			{
				int i = this.glog[a];
				int j = this.glog[b];
				result = this.gexp[i + j];
			}
			return result;
		}
		internal virtual int ginv(int elt)
		{
			return this.gexp[255 - this.glog[elt]];
		}
		internal virtual void decode_data(int[] data)
		{
			for (int i = 0; i < this.MAXDEG; i++)
			{
				int sum = 0;
				for (int j = 0; j < data.Length; j++)
				{
					sum = (data[j] ^ this.gmult(this.gexp[i + 1], sum));
				}
				this.synBytes[i] = sum;
			}
		}
		public virtual void correct()
		{
			this.decode_data(this.y);
			this.correctionSucceeded = true;
			bool hasError = false;
			for (int i = 0; i < this.synBytes.Length; i++)
			{
				if (this.synBytes[i] != 0)
				{
					hasError = true;
				}
			}
			if (hasError)
			{
				this.correctionSucceeded = this.correct_errors_erasures(this.y, this.y.Length, 0, new int[1]);
			}
		}
		internal virtual void Modified_Berlekamp_Massey()
		{
			int[] psi = new int[this.MAXDEG];
			int[] psi2 = new int[this.MAXDEG];
			int[] D = new int[this.MAXDEG];
			int[] gamma = new int[this.MAXDEG];
			this.init_gamma(gamma);
			this.copy_poly(D, gamma);
			this.mul_z_poly(D);
			this.copy_poly(psi, gamma);
			int i = -1;
			int L = this.NErasures;
			for (int j = this.NErasures; j < 8; j++)
			{
				int d = this.compute_discrepancy(psi, this.synBytes, L, j);
				if (d != 0)
				{
					for (int k = 0; k < this.MAXDEG; k++)
					{
						psi2[k] = (psi[k] ^ this.gmult(d, D[k]));
					}
					if (L < j - i)
					{
						int L2 = j - i;
						i = j - L;
						for (int k = 0; k < this.MAXDEG; k++)
						{
							D[k] = this.gmult(psi[k], this.ginv(d));
						}
						L = L2;
					}
					for (int k = 0; k < this.MAXDEG; k++)
					{
						psi[k] = psi2[k];
					}
				}
				this.mul_z_poly(D);
			}
			for (int k = 0; k < this.MAXDEG; k++)
			{
				this.Lambda[k] = psi[k];
			}
			this.compute_modified_omega();
		}
		internal virtual void compute_modified_omega()
		{
			int[] product = new int[this.MAXDEG * 2];
			this.mult_polys(product, this.Lambda, this.synBytes);
			this.zero_poly(this.Omega);
			for (int i = 0; i < this.NPAR; i++)
			{
				this.Omega[i] = product[i];
			}
		}
		internal virtual void mult_polys(int[] dst, int[] p1, int[] p2)
		{
			int[] tmp = new int[this.MAXDEG * 2];
			for (int i = 0; i < this.MAXDEG * 2; i++)
			{
				dst[i] = 0;
			}
			for (int i = 0; i < this.MAXDEG; i++)
			{
				for (int j = this.MAXDEG; j < this.MAXDEG * 2; j++)
				{
					tmp[j] = 0;
				}
				for (int j = 0; j < this.MAXDEG; j++)
				{
					tmp[j] = this.gmult(p2[j], p1[i]);
				}
				for (int j = this.MAXDEG * 2 - 1; j >= i; j--)
				{
					tmp[j] = tmp[j - i];
				}
				for (int j = 0; j < i; j++)
				{
					tmp[j] = 0;
				}
				for (int j = 0; j < this.MAXDEG * 2; j++)
				{
					dst[j] ^= tmp[j];
				}
			}
		}
		internal virtual void init_gamma(int[] gamma)
		{
			int[] tmp = new int[this.MAXDEG];
			this.zero_poly(gamma);
			this.zero_poly(tmp);
			gamma[0] = 1;
			for (int e = 0; e < this.NErasures; e++)
			{
				this.copy_poly(tmp, gamma);
				this.scale_poly(this.gexp[this.ErasureLocs[e]], tmp);
				this.mul_z_poly(tmp);
				this.add_polys(gamma, tmp);
			}
		}
		internal virtual void compute_next_omega(int d, int[] A, int[] dst, int[] src)
		{
			for (int i = 0; i < this.MAXDEG; i++)
			{
				dst[i] = (src[i] ^ this.gmult(d, A[i]));
			}
		}
		internal virtual int compute_discrepancy(int[] lambda, int[] S, int L, int n)
		{
			int sum = 0;
			for (int i = 0; i <= L; i++)
			{
				sum ^= this.gmult(lambda[i], S[n - i]);
			}
			return sum;
		}
		internal virtual void add_polys(int[] dst, int[] src)
		{
			for (int i = 0; i < this.MAXDEG; i++)
			{
				dst[i] ^= src[i];
			}
		}
		internal virtual void copy_poly(int[] dst, int[] src)
		{
			for (int i = 0; i < this.MAXDEG; i++)
			{
				dst[i] = src[i];
			}
		}
		internal virtual void scale_poly(int k, int[] poly)
		{
			for (int i = 0; i < this.MAXDEG; i++)
			{
				poly[i] = this.gmult(k, poly[i]);
			}
		}
		internal virtual void zero_poly(int[] poly)
		{
			for (int i = 0; i < this.MAXDEG; i++)
			{
				poly[i] = 0;
			}
		}
		internal virtual void mul_z_poly(int[] src)
		{
			for (int i = this.MAXDEG - 1; i > 0; i--)
			{
				src[i] = src[i - 1];
			}
			src[0] = 0;
		}
		internal virtual void Find_Roots()
		{
			this.NErrors = 0;
			for (int r = 1; r < 256; r++)
			{
				int sum = 0;
				for (int i = 0; i < this.NPAR + 1; i++)
				{
					sum ^= this.gmult(this.gexp[i * r % 255], this.Lambda[i]);
				}
				if (sum == 0)
				{
					this.ErrorLocs[this.NErrors] = 255 - r;
					this.NErrors++;
				}
			}
		}
		internal virtual bool correct_errors_erasures(int[] codeword, int csize, int nerasures, int[] erasures)
		{
			this.NErasures = nerasures;
			for (int i = 0; i < this.NErasures; i++)
			{
				this.ErasureLocs[i] = erasures[i];
			}
			this.Modified_Berlekamp_Massey();
			this.Find_Roots();
			bool result;
			if (this.NErrors <= this.NPAR || this.NErrors > 0)
			{
				for (int r = 0; r < this.NErrors; r++)
				{
					if (this.ErrorLocs[r] >= csize)
					{
						result = false;
						return result;
					}
				}
				for (int r = 0; r < this.NErrors; r++)
				{
					int i = this.ErrorLocs[r];
					int num = 0;
					for (int j = 0; j < this.MAXDEG; j++)
					{
						num ^= this.gmult(this.Omega[j], this.gexp[(255 - i) * j % 255]);
					}
					int denom = 0;
					for (int j = 1; j < this.MAXDEG; j += 2)
					{
						denom ^= this.gmult(this.Lambda[j], this.gexp[(255 - i) * (j - 1) % 255]);
					}
					int err = this.gmult(num, this.ginv(denom));
					codeword[csize - i - 1] ^= err;
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
