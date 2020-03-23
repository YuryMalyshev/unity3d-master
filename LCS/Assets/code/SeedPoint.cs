using System;
using System.Linq;
using System.Numerics;

namespace Assets.code
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

		public new static SeedPoint DeSerialize(byte[] subset)
		{
			byte[] _subset = new byte[sizeof(float) * 6];
			Array.Copy(subset, 0, _subset, 0, sizeof(float) * 6);
			Point p = Point.DeSerialize(_subset);
			double FTLE = BitConverter.ToDouble(subset, sizeof(float) * 6);
			return new SeedPoint(p, FTLE);
		}
	}
}
