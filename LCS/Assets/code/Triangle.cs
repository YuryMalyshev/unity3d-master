using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.code
{
	class Triangle : IEquatable<Triangle>
	{
		public Seed A { get; private set; }
		public Seed B { get; private set; }
		public Seed C { get; private set; }

		public List<Seed> Points { get => new List<Seed>() { A, B, C }; }
		//private GameObject triangle;

		public Triangle(Seed A, Seed B, Seed C)
		{
			//Debug.Log("Creted new triangle [" + A.pos.x + ", " + A.pos.y + "] " +
			//										"[" + B.pos.x + ", " + B.pos.y + "] " +
			//										"[" + C.pos.x + ", " + C.pos.y + "]");
			this.A = A;
			this.B = B;
			this.C = C;
		}

		public bool Adjacent(Triangle another)
		{
			return (Points.Contains(another.A) && Points.Contains(another.B)) ||
				 	 (Points.Contains(another.A) && Points.Contains(another.C)) ||
				 	 (Points.Contains(another.B) && Points.Contains(another.B));
		}

		public void Accept()
		{
			A.Use();
			B.Use();
			C.Use();
		}

		override public string ToString()
		{
			return "[" + A.pos.x + ", " + A.pos.y + "] " +
					 "[" + B.pos.x + ", " + B.pos.y + "] " +
					 "[" + C.pos.x + ", " + C.pos.y + "]";
		}

		public bool Equals(Triangle other)
		{
			return ScrambledEquals(this, other);
		}

		private static bool ScrambledEquals(Triangle one, Triangle other)
		{
			Dictionary<Seed,  int> cnt = new Dictionary<Seed, int>();
			foreach (Seed s in one.Points)
			{
				if (cnt.ContainsKey(s))
				{
					cnt[s]++;
				}
				else
				{
					cnt.Add(s, 1);
				}
			}
			foreach (Seed s in other.Points)
			{
				if (cnt.ContainsKey(s))
				{
					cnt[s]--;
				}
				else
				{
					return false;
				}
			}
			bool equal = cnt.Values.All(c => c == 0);
			//Debug.Log(one + " EqualTo " + other + " " + equal);
			return equal;
		}
	}
}
