using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdvectionCalculationsGUI
{
	public partial class GUI : Form
	{
		// graphics
		private readonly Pen dotPen;
		private readonly Pen linePen;
		private readonly Pen pointerPen;

		private Bitmap completeBackground;
		private Bitmap lastFrame;
		private Bitmap newFrame;
		private List<Point> pointsToDraw;
		private List<List<Point>> lines = new List<List<Point>> ();

		double xMin = 0;
		double xMax = 0;
		double yMin = 0;
		double yMax = 0;

		double ratio = 0;
		double offX = 0;
		double offY = 0;

		private int snapDist;
		private bool snap = false;

		// Data
		private FTLEDataSet fds;
		private InputDataSet ids;
		private bool dataLoaded = false;

		public GUI()
		{
			// Initialize
			InitializeComponent();
			ResetFields();
			fieldNormilizerWorker.WorkerReportsProgress = true;
			this.progressBar.Maximum = 1000;

			// Create tooltips
			ToolTip tooltip_avDistance = new ToolTip();
			tooltip_avDistance.ShowAlways = true;
			tooltip_avDistance.SetToolTip(label_avDistance, "Estimated average distance between points");

			// Add keyboard handlers to the canvas
			(this.canvas as Control).KeyDown += new KeyEventHandler(CanvasKeyDown);
			(this.canvas as Control).KeyUp += new KeyEventHandler(CanvasKeyUp);

			// Create pens
			dotPen = new Pen(Color.Black, 2);
			linePen = new Pen(Color.Green, 2);
			pointerPen = new Pen(Color.Red, 5);

			// Set snapping distance
			snapDist = (int)(pointerPen.Width * 4);

			// List of points which will be actually drawn
			pointsToDraw = new List<Point>();
		}

		Stopwatch sw = new Stopwatch();

		private void GUI_Load(object sender, EventArgs e)
		{
			lines.Add(new List<Point>());
		}

		/// <summary>
		/// Action Perfromed on "Select Data" click. Opens a dialog for file selection; resets state of the GUI
		/// </summary>
		private void SelectInputFile_Click(object sender, EventArgs e)
		{
			if (selectDataDalog.ShowDialog() == DialogResult.OK)
			{
				// Update/Reset fields
				this.label_filename.Text = selectDataDalog.SafeFileName;
				ResetFields();

				// Enable/Disable components
				this.label_avDistance.Enabled = true;
				this.label_voxelSize.Enabled = true;
				this.avDistance.Enabled = true;
				this.voxelSize.Enabled = true;
				this.loadDataBtn.Enabled = true;

				this.outputSettingsPanel.Enabled = false;
				this.startBtn.Enabled = false;
			}
		}

		/// <summary>
		/// Action Perfromed on "Load Data" click. Loads the data from text file and then draws it on canvas.
		/// </summary>
		private void LoadData_Click(object sender, EventArgs e)
		{
			this.outputSettingsPanel.Enabled = true; //TODO: reset values
			ids = new InputDataSet(selectDataDalog.FileName, double.Parse(voxelSize.Text));
			List<Point> points = ids.GetPoints();
			//TODO: based on amount
			double maxDist = double.Parse(this.avDistance.Text) * (15000f / points.Count * 10);
			pointsToDraw.Clear();
			foreach (Point p in points)
			{
				
				double localMin = double.PositiveInfinity;
				foreach(Point pd in pointsToDraw)
				{
					double dist = pd.DistanceTo(p);
					if(localMin > dist)
					{
						localMin = dist;
					}
					if(dist < maxDist)
					{
						break;
					}
				}
				if(localMin > maxDist)
				{
					pointsToDraw.Add(p);
				}
			}
			dataLoaded = true;
			DrawData();	
		}

		/// <summary>
		/// Draws the data into a bitmap. Bitmap is then used as a background on canvas
		/// </summary>
		private void DrawData()
		{
			completeBackground = new Bitmap(canvas.Width, canvas.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics background = Graphics.FromImage(completeBackground);
			background.Clear(Color.White);
			if (dataLoaded)
			{
				//TODO: dynamic
				xMin = 0;
				xMax = 2;
				yMin = 0;
				yMax = 1;

				ratio = Math.Min((completeBackground.Width * 0.75) / (xMax - xMin), (completeBackground.Height * 0.75) / (yMax - yMin));
				offX = completeBackground.Width / 2 - (xMax - xMin) / 2 * ratio;
				offY = completeBackground.Height / 2 - (yMax - yMin) / 2 * ratio;
				foreach (Point p in pointsToDraw)
				{
					background.DrawEllipse(dotPen, (float)(p.Pos.X * ratio + offX - dotPen.Width),
												(float)(p.Pos.Y * ratio + offY - dotPen.Width),
												dotPen.Width, dotPen.Width);
				}
			}
			background.Dispose();
		}

		/// <summary>
		/// Action Perfromed on "Select Folder" click. Opens folder selection dialog, unlocks "Start" button
		/// </summary>
		private void SelectFolder_Click(object sender, EventArgs e)
		{
			if (selectOutputFolderDialog.ShowDialog() == DialogResult.OK)
			{
				string path = selectOutputFolderDialog.SelectedPath;
				Debug.WriteLine("Selected " + path);
				int first = path.IndexOf('\\');
				int last = path.LastIndexOf('\\');
				if (first == last)
				{
					this.label_selectFolder.Text = path;
				}
				else
				{
					this.label_selectFolder.Text = path.Substring(0, first + 1) + "..." + path.Substring(last);
				}
				fds = new FTLEDataSet(path);
				this.startBtn.Enabled = true;
			}
		}

		private void Start_Click(object sender, EventArgs e)
		{
			fieldNormilizerWorker.RunWorkerAsync();
		}

		

		/// <summary>
		/// Called when mouse has moved over the canvas. Updates
		/// </summary>
		private void Canvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (lastFrame != null)
				lastFrame.Dispose();
			lastFrame = newFrame;
			newFrame = new Bitmap(canvas_holder.Width, canvas_holder.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics frame = Graphics.FromImage(newFrame);
			frame.DrawImageUnscaled(completeBackground,0,0);

			Point last = null;
			foreach (List<Point> line in lines)
			{
				last = null;
				foreach (Point p in line)
				{
					if (last != null)
					{
						int pX = (int)((float)(p.Pos.X * ratio + offX - dotPen.Width));
						int pY = (int)((float)(p.Pos.Y * ratio + offY - dotPen.Width));
						int lastX = (int)((float)(last.Pos.X * ratio + offX - dotPen.Width));
						int lastY = (int)((float)(last.Pos.Y * ratio + offY - dotPen.Width));
						frame.DrawLine(linePen, lastX, lastY, pX, pY);
					}
					last = p;
				}
			}

			int mX = e.X;
			int mY = e.Y;
			if (snap)
			{
				//Debug.WriteLine("Snapping!");
				foreach (Point p in pointsToDraw)
				{
					int pX = (int)((float)(p.Pos.X * ratio + offX - dotPen.Width));
					int pY = (int)((float)(p.Pos.Y * ratio + offY - dotPen.Width));
					if (Math.Sqrt(Math.Pow(pX - mX, 2) + Math.Pow(pY - mY, 2)) < snapDist)
					{
						mX = pX;
						mY = pY;
						break;
					}
				}
			}

			if (last != null)
			{
				int lastX = (int)((float)(last.Pos.X * ratio + offX - dotPen.Width));
				int lastY = (int)((float)(last.Pos.Y * ratio + offY - dotPen.Width));
				frame.DrawLine(linePen, lastX, lastY, mX, mY);
			}

			frame.DrawEllipse(new Pen(Color.Red, 10), mX - 5, mY - 5, 10, 10);
			frame.Dispose();
			Canvas_Refresh();
		}

		private void Canvas_Resize(object sender, EventArgs e)
		{
			DrawData();
			if(lastFrame != null)
				lastFrame.Dispose();
			lastFrame = newFrame;
			newFrame = new Bitmap(canvas_holder.Width, canvas_holder.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			//Debug.WriteLine("New size of canvas " + canvas.Width + " by " + canvas.Height);
			//Debug.WriteLine("New size of frame" + newFrame.Width + " by " + newFrame.Height);
			Graphics frame = Graphics.FromImage(newFrame);
			frame.DrawImageUnscaled(completeBackground, 0, 0);
			frame.Dispose();
			Canvas_Refresh();
		}

		private void Canvas_Refresh()
		{
			canvas.Image = newFrame;
		}

		private void CanvasKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Menu)
			{
				snap = true;
			}
			if(e.KeyCode == Keys.Enter)
			{
				lines.Add(new List<Point>());
			}
			e.Handled = true;
		}

		private void CanvasKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Menu)
			{
				snap = false;
			}
			e.Handled = true;
		}

		private void ResetFields()
		{
			this.avDistance.Text = "0.01";
			this.voxelSize.Text = "0.25";
			this.resolution.Text = "0.05";
			this.dt.Text = "0.15";
			this.steps.Text = "200";
			this.seconds.Text = "unused";
			this.direction.SelectedIndex = 0;
		}

		private void Canvas_GetFocus(object sender, EventArgs e)
		{
			canvas.Focus();
		}

		private void Canvas_MouseClick(object sender, MouseEventArgs e)
		{
			//Debug.WriteLine("Click in mouseClick");
			List<Point> line = lines[lines.Count - 1];
			if (e.Button == MouseButtons.Left)
			{
				Point p = SnapPoint(e);
				if(p != null)
				{
					line.Add(p);
				}
			}
			if (e.Button == MouseButtons.Right)
			{
				if (line.Count > 0)
				{
					line.RemoveAt(line.Count - 1);
				}
				else
				{
					if(lines.Count > 1)
					{
						lines.RemoveAt(lines.Count - 1);
					}
				}
			}
		}

		private Point SnapPoint(MouseEventArgs e)
		{
			
			int mX = e.X;
			int mY = e.Y;
			if (snap)
			{
				//Debug.WriteLine("Snapping!");
				foreach (Point p in pointsToDraw)
				{
					int pX = (int)((float)(p.Pos.X * ratio + offX - dotPen.Width));
					int pY = (int)((float)(p.Pos.Y * ratio + offY - dotPen.Width));
					if (Math.Sqrt(Math.Pow(pX - mX, 2) + Math.Pow(pY - mY, 2)) < snapDist)
					{
						return p;
					}
				}
			}
			try
			{
				Vector3 pos = new Vector3(
					(float)((double)(mX - offX) / ratio),
					(float)((double)(mY - offY) / ratio),
					0
				);
				return ids.GetPoint(pos);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.StackTrace);
				return null;
			}
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			List<Thread> threads;
			ManualResetEvent notifier = new ManualResetEvent(false);
			BackgroundWorker worker = sender as BackgroundWorker;

			Debug.WriteLine("Start doing work!");
			Advection adv = new Advection(ids, fds);
			// get advection parameter

			// start advection routine
			//Thread t = new Thread(adv.Start);
			//t.Start(new List<object> { radius, steps, dt, points, 50 }); //TODO
			float radius = float.Parse(this.avDistance.Text)/2;
			int steps = int.Parse(this.steps.Text);
			double dt = double.Parse(this.dt.Text);
			List<Point> entryPoints = new List<Point>();
			//TODO
			int segments = 20; //segments => points = segments - 1 + end_point
			foreach (List<Point> line in lines)
			{
				Point last = null;
				foreach(Point p in line)
				{
					if(last != null)
					{
						//interpolate a line
						for(int i = 1; i < segments; i++)
						{
							float ratio = (float)i / segments;
							Vector3 pos = new Vector3(
								((p.Pos.X - last.Pos.X) * ratio) + last.Pos.X,
								((p.Pos.Y - last.Pos.Y) * ratio) + last.Pos.Y,
								((p.Pos.Z - last.Pos.Z) * ratio) + last.Pos.Z
							);
							//Debug.WriteLine("Trying to add " + pos + " between " + p.Pos + " and " + last.Pos);
							entryPoints.Add(ids.GetPoint(pos));
						}
					}
					entryPoints.Add(p);
					last = p;
				}
			}
			Debug.WriteLine("N-points: " + entryPoints.Count);
			threads = adv.Start(radius, steps, dt, entryPoints, 40, selectOutputFolderDialog.SelectedPath, notifier);

			// wait for it to finish & update progress bar somehow
			//t.Join();
			Debug.WriteLine("Threads are done, waiting for their end");
			int threadMax = threads.Count;
			while (threads.Count > 0)
			{
				notifier.WaitOne(2000);
				notifier.Reset();
				foreach (Thread t in threads.ToList())
				{
					if (!t.IsAlive)
					{
						threads.Remove(t);
					}
				}
				worker.ReportProgress((threadMax - threads.Count) * 1000 / threadMax);
				Debug.WriteLine("threads.Count " + threads.Count + " Max " + threadMax + 
				" Fraction " + ((threadMax - threads.Count) * 1000 / threadMax) + "/" + 1000);
			}

			Debug.WriteLine("Start creating uniform FTLE field");

			double resolution = double.Parse(this.resolution.Text); 
			FTLEField field = new FTLEField(resolution, 2, 1, fds, selectOutputFolderDialog.SelectedPath);
			threads = field.Start(notifier);
			Debug.WriteLine("Threads are done, waiting for their end");
			threadMax = threads.Count;
			while (threads.Count > 0)
			{
				notifier.WaitOne(2000);
				notifier.Reset();
				foreach (Thread t in threads.ToList())
				{
					if (!t.IsAlive)
					{
						threads.Remove(t);
					}
				}
				worker.ReportProgress((threadMax - threads.Count)*1000/ threadMax);
				Debug.WriteLine("threads.Count " + threads.Count + " Max " + threadMax + 
				" Fraction " + ((threadMax - threads.Count) * 1000 / threadMax) + "/" + 1000);
			}
			field.Serialize();
			Debug.WriteLine("All done!");
		}

		private void ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			this.progressBar.Value = e.ProgressPercentage;
		}


	}
}
