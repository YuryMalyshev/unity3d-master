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
		public Seed_DEPRECATED A { get; private set; }
		public Seed_DEPRECATED B { get; private set; }
		public Seed_DEPRECATED C { get; private set; }

		public List<Seed_DEPRECATED> Points { get => new List<Seed_DEPRECATED>() { A, B, C }; }
		//private GameObject triangle;

		public Triangle(Seed_DEPRECATED A, Seed_DEPRECATED B, Seed_DEPRECATED C)
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
			Dictionary<Seed_DEPRECATED,  int> cnt = new Dictionary<Seed_DEPRECATED, int>();
			foreach (Seed_DEPRECATED s in one.Points)
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
			foreach (Seed_DEPRECATED s in other.Points)
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
