using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdvectionCalculationsGUI.src
{
	public class SeedPoint : Point
	{
		public double FTLE { get; set; }
		public int Step { get; }
		public SeedPoint(Point p, double FTLE) : base(p)
		{
			this.FTLE = FTLE;
		}

		public SeedPoint(Vector3 pos, Vector3 vel, double FTLE) : base(pos, vel)
		{
			this.FTLE = FTLE;
		}

		public override object Clone()
		{
			return new SeedPoint(this, FTLE);
		}

		public new byte[] Serialize()
		{
			byte[] temp = base.Serialize();
			temp = temp.Concat(BitConverter.GetBytes(FTLE)).ToArray();
			return temp;
		}
	}
}
