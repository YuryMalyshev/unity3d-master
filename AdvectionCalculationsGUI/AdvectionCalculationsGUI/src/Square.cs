using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvectionCalculationsGUI.src
{
	class Square<T> where T : Point
	{
		public readonly T A;
		public readonly T B;
		public readonly T C;
		public readonly T D;

		public Square(T A, T B, T C, T D)
		{
			
			this.A = A;
			this.B = B;
			this.C = C;
			this.D = D;
		}

		public bool Equals(Square<T> another)
		{
			return ((A == another.A || A == another.B || A == another.C || A == another.D) &&
					  (B == another.A || B == another.B || B == another.C || B == another.D) &&
					  (C == another.A || C == another.B || C == another.C || C == another.D) &&
					  (D == another.A || D == another.B || D == another.C || D == another.D));
		}

		public bool IsComplete()
		{
			return !(A == null || B == null || C == null || D == null);
		}
	}
}
