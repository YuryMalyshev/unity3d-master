using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdvectionCalculationsGUI.src
{
	class Picture
	{
		private readonly Color dotColor;
		private readonly Pen dotPen;
		private readonly Pen pointerPen;
		private readonly Pen linePen;

		private Bitmap completeBackground;
		private Bitmap currentFrame;
		public List<Point> PointsToDraw { get; private set; }
		private List<Vector3> pointsToDrawLocation;

		double xMin = 0;
		double xMax = 0;
		double yMin = 0;
		double yMax = 0;
		double zMin = 0;
		double zMax = 0;

		double ratio = 0;
		double offX = 0;
		double offY = 0;

		double oOffX = 0;
		double oOffY = 0;

		float maxLayerFraction = 1;

		float[,] rmf; // rotation matrix
		public List<List<Point>> Lines { get; private set; }
		private List<List<Vector3>> linesLocation;

		public Picture()
		{
			// Create pens
			dotColor = Color.Black;
			dotPen = new Pen(dotColor, 2);
			pointerPen = new Pen(Color.Red, 5);
			linePen = new Pen(Color.Green, 1);

			// List of points which will be actually drawn
			Clear();
		}

		public void Clear()
		{
			PointsToDraw = new List<Point>();
			pointsToDrawLocation = new List<Vector3>();
			currentFrame = new Bitmap(1, 1, PixelFormat.Format24bppRgb);
			completeBackground = new Bitmap(1, 1, PixelFormat.Format24bppRgb);

			Lines = new List<List<Point>>
			{
				new List<Point>()
			};
			linesLocation = new List<List<Vector3>>
			{
				new List<Vector3>()
			};
		}

		public void MakeSelection(double avDistance, List<Point> points)
		{
			PointsToDraw.Clear();
			double maxDist = avDistance * (15000f / points.Count * 10); //TODO: based on amount
			foreach (Point p in points)
			{
				/*// Update limits
				*/
				// Point p is at least maxDist away from any other drawn point
				double localMin = double.PositiveInfinity;
				foreach (Point pd in PointsToDraw)
				{
					double dist = pd.DistanceTo(p);
					if (localMin > dist)
					{
						localMin = dist;
					}
					if (dist < maxDist)
					{
						break;
					}
				}
				if (localMin > maxDist)
				{
					PointsToDraw.Add(p);
					pointsToDrawLocation.Add(p.Pos);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x">Rotation arond x-axis, in radians</param>
		/// <param name="y">Rotation arond y-axis, in radians</param>
		/// <param name="z">Rotation arond z-axis, in radians</param>
		public void RotateObject(double x, double y, double z)
		{
			xMin = double.PositiveInfinity;
			xMax = double.NegativeInfinity;
			yMin = double.PositiveInfinity;
			yMax = double.NegativeInfinity;
			zMin = double.PositiveInfinity;
			zMax = double.NegativeInfinity;
			// Calculate rotation matrix
			double[,] rmx = new double[3, 3] { { 1, 0, 0 }, { 0, Math.Cos(x), -Math.Sin(x) }, { 0, Math.Sin(x), Math.Cos(x) } };
			double[,] rmy = new double[3, 3] { { Math.Cos(y), 0, Math.Sin(y) }, { 0, 1, 0 }, { -Math.Sin(y), 0, Math.Cos(y) } };
			double[,] rmz = new double[3, 3] { { Math.Cos(z), -Math.Sin(z), 0 }, { Math.Sin(z), Math.Cos(z), 0 }, { 0, 0, 1 } };
			double[,] rm = Accord.Math.Matrix.Dot(Accord.Math.Matrix.Dot(rmx, rmy), rmz);
			// cast it to float matrix
			rmf = new float[rm.GetLength(0), rm.GetLength(1)];
			for(int i = 0; i < rm.GetLength(0); i++)
			{
				for(int j = 0; j < rm.GetLength(1); j++)
				{
					rmf[i, j] = (float)Math.Round(rm[i, j]);
				}
			}
			//Array.Copy(rm, rmf, rm.Length);
			// Calculate positions of points (not scaled)
			float[] pos = new float[3];
			for (int i = 0; i < PointsToDraw.Count; i++)
			{
				PointsToDraw[i].Pos.CopyTo(pos);
				pos = Accord.Math.Matrix.Dot(pos, rmf);
				Vector3 Pos = new Vector3(pos[0], pos[1], pos[2]);
				// Update limits after rotation
				if (Pos.X < xMin)
				{
					xMin = Pos.X;
				}
				if (Pos.Y < yMin)
				{
					yMin = Pos.Y;
				}
				if (Pos.Z < zMin)
				{
					zMin = Pos.Z;
				}

				if (Pos.X > xMax)
				{
					xMax = Pos.X;
				}
				if (Pos.Y > yMax)
				{
					yMax = Pos.Y;
				}
				if (Pos.Z > zMax)
				{
					zMax = Pos.Z;
				}
				pointsToDrawLocation[i] = Pos;
			}
			for(int i = 0; i < linesLocation.Count; i++)
			{
				for(int j = 0; j < linesLocation[i].Count; j++)
				{
					Lines[i][j].Pos.CopyTo(pos);
					pos = Accord.Math.Matrix.Dot(pos, rmf);
					linesLocation[i][j] = new Vector3(pos[0], pos[1], pos[2]);
				}
			}
		}

		public void FinishLine()
		{
			if (Lines.Last().Count >= 2)
			{
				Lines.Add(new List<Point>());
				linesLocation.Add(new List<Vector3>());
			}
		}

		public void DrawScaledObjectOn(PictureBox canvas)
		{
			completeBackground.Dispose();
			completeBackground = new Bitmap(canvas.Width, canvas.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics g = Graphics.FromImage(completeBackground);
			g.Clear(Color.White);

			ratio = Math.Min((completeBackground.Width * 0.75) / (xMax - xMin), (completeBackground.Height * 0.75) / (yMax - yMin));
			oOffX = (xMin + xMax) / 2;
			oOffY = (yMin + yMax) / 2;
			offX = completeBackground.Width / 2 - oOffX * ratio;
			offY = completeBackground.Height / 2 - oOffY * ratio;
			double rangeZ = zMax - zMin;
			//offZ = completeBackground.Height / 2 - (zMax - zMin) / 2 * ratio;
			foreach (Vector3 p in pointsToDrawLocation)
			{
				float layerFraction;
				if (rangeZ != 0)
					layerFraction = (float)((p.Z - zMin) / rangeZ);
				else
					layerFraction = 1;
				if (layerFraction <= maxLayerFraction)
				{
					byte b = (byte)((1-layerFraction) * 255);
					dotPen.Color = Color.FromArgb(b, b, b);
					g.DrawEllipse(dotPen, (float)(p.X * ratio + offX - dotPen.Width / 2),
												(float)(p.Y * ratio + offY - dotPen.Width / 2),
												dotPen.Width, dotPen.Width);
				}
			}
			g.Dispose();
			canvas.Image = completeBackground;
			UpdateMousePointer(canvas, -20, -20);
		}

		public void UpdateMousePointer(PictureBox canvas, int mX, int mY)
		{
			Bitmap toBeDisposed = currentFrame;
			currentFrame = new Bitmap(canvas.Width, canvas.Height, PixelFormat.Format24bppRgb);

			Graphics g = Graphics.FromImage(currentFrame);
			g.Clear(Color.White);
			g.DrawImageUnscaled(completeBackground, 0, 0);
			Vector3 last = Vector3.Zero;
			int pX = 0, pY = 0, lastX, lastY;
			foreach (List<Vector3> line in linesLocation)
			{
				last = Vector3.Zero;
				foreach (Vector3 p in line)
				{
					pX = (int)((float)(p.X * ratio + offX - dotPen.Width));
					pY = (int)((float)(p.Y * ratio + offY - dotPen.Width));
					if (last != Vector3.Zero)
					{
						lastX = (int)((float)(last.X * ratio + offX - dotPen.Width));
						lastY = (int)((float)(last.Y * ratio + offY - dotPen.Width));
						g.DrawLine(linePen, lastX, lastY, pX, pY);
					}
					last = p;
				}
			}
			if(last != Vector3.Zero && mX > 0 && mY > 0)
			{
				g.DrawLine(linePen, pX, pY, mX, mY);
			}

			g.DrawEllipse(pointerPen, mX - pointerPen.Width/2, mY - pointerPen.Width / 2, pointerPen.Width, pointerPen.Width);
			g.Dispose();
			canvas.Image = currentFrame;
			if(toBeDisposed != null)
				toBeDisposed.Dispose();
		}

		public void AddPoint(PictureBox canvas, int mX, int mY, InputDataSet ids)
		{
			float[] pos = new float[3] { mX, mY, (float)(zMin + (zMax - zMin)*maxLayerFraction) };
			pos[0] = (float)((mX - offX + dotPen.Width / 2) / ratio);
			pos[1] = (float)((mY - offY + dotPen.Width / 2) / ratio);
			Vector3 pointPos = new Vector3(pos[0], pos[1], pos[2]);


			pos = Accord.Math.Matrix.Dot(pos, Accord.Math.Matrix.Inverse(rmf));
			
			Point p = ids.GetPoint(new Vector3(pos[0], pos[1], pos[2]));
			if(p != null)
			{
				Lines.Last().Add(p);
				linesLocation.Last().Add(pointPos);
			}
			UpdateMousePointer(canvas, mX, mY);
		}

		public void RemovePoint(PictureBox canvas, int mX, int mY)
		{
			if(Lines.Last().Count > 0)
			{
				Lines.Last().Remove(Lines.Last().Last());
				linesLocation.Last().Remove(linesLocation.Last().Last());
			}
			else
			{
				if(Lines.Count > 1)
				{
					Lines.Remove(Lines.Last());
					linesLocation.Remove(linesLocation.Last());
				}
			}
			UpdateMousePointer(canvas, mX, mY);
		}

		public void ChangeLevel(int value, int maximum)
		{
			maxLayerFraction = (float)value / maximum;
		}
	}
}
