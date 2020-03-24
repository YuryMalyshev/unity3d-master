using AdvectionCalculationsGUI.src;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
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
		private List<List<Point>> lines = new List<List<Point>>();

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
		private InputDataSet ids;
		private StreamLines sls;
		//private FTLEField ffield;
		private bool dataLoaded = false;

		// Timing check
		private Stopwatch sw = new Stopwatch();

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
			xMin = double.PositiveInfinity;
			xMax = double.NegativeInfinity;
			yMin = double.PositiveInfinity;
			yMax = double.NegativeInfinity;

			this.outputSettingsPanel.Enabled = true; //TODO: reset values
			ids = new InputDataSet(selectDataDalog.FileName, double.Parse(voxelSize.Text));
			List<Point> points = ids.GetPoints();
			//TODO: based on amount
			double maxDist = double.Parse(this.avDistance.Text) * (15000f / points.Count * 10);

			pointsToDraw.Clear();
			foreach (Point p in points)
			{
				// Update limits
				if(p.Pos.X < xMin)
				{
					xMin = p.Pos.X;
				}
				if(p.Pos.Y < yMin)
				{
					yMin = p.Pos.Y;
				}

				if (p.Pos.X > xMax)
				{
					xMax = p.Pos.X;
				}
				if (p.Pos.Y > yMax)
				{
					yMax = p.Pos.Y;
				}
				// Take points only outside of a certain radius from existing
				double localMin = double.PositiveInfinity;
				foreach (Point pd in pointsToDraw)
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
					pointsToDraw.Add(p);
				}
			}
			dataLoaded = true;
			DrawData();
		}

		/// <summary>
		/// Draws the data into a bitmap. The picture is then reused on redraw
		/// </summary>
		private void DrawData()
		{
			completeBackground = new Bitmap(canvas.Width, canvas.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics g = Graphics.FromImage(completeBackground);
			g.Clear(Color.White);
			if (dataLoaded)
			{
				// Scaling ratio
				ratio = Math.Min((completeBackground.Width * 0.75) / (xMax - xMin), (completeBackground.Height * 0.75) / (yMax - yMin));
				// Constant offset from a corner, such that data apppears to be in the center
				offX = completeBackground.Width / 2 - (xMax - xMin) / 2 * ratio;
				offY = completeBackground.Height / 2 - (yMax - yMin) / 2 * ratio;
				foreach (Point p in pointsToDraw)
				{
					g.DrawEllipse(dotPen, (float)(p.Pos.X * ratio + offX - dotPen.Width),
												(float)(p.Pos.Y * ratio + offY - dotPen.Width),
												dotPen.Width, dotPen.Width);
				}
			}
			g.Dispose();
			canvas.Image = completeBackground;
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
				sls = new StreamLines();
				this.startBtn.Enabled = true;
			}
		}

		private void Start_Click(object sender, EventArgs e)
		{
			SetAllEnabled(false);
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
			frame.DrawImageUnscaled(completeBackground, 0, 0);

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
			if (lastFrame != null)
				lastFrame.Dispose();
			lastFrame = newFrame;
			newFrame = new Bitmap(canvas_holder.Width, canvas_holder.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
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
			if (e.KeyCode == Keys.Enter)
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
			this.resolution.Text = "5";
			this.dt.Text = "0.1";
			this.steps.Text = "200";
			this.seconds.Text = "20";
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
				if (p != null)
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
					if (lines.Count > 1)
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
			catch (Exception ex)
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
			worker.ReportProgress(0);

			Debug.WriteLine("Start doing work!");
			Advection adv = new Advection(ids, sls);
			// start advection routine
			float radius = float.Parse(this.avDistance.Text) / 2;
			int steps = int.Parse(this.steps.Text);
			double dt = double.Parse(this.dt.Text);
			List<Point> entryPoints = new List<Point>();
			int segments = 20; //segments => points = segments - 1 + end_point //TODO
			foreach (List<Point> line in lines)
			{
				Point last = null;
				foreach (Point p in line)
				{
					if (last != null)
					{
						//interpolate a line
						for (int i = 1; i < segments; i++)
						{
							float ratio = (float)i / segments;
							Vector3 pos = new Vector3(
								((p.Pos.X - last.Pos.X) * ratio) + last.Pos.X,
								((p.Pos.Y - last.Pos.Y) * ratio) + last.Pos.Y,
								((p.Pos.Z - last.Pos.Z) * ratio) + last.Pos.Z
							);
							entryPoints.Add(ids.GetPoint(pos, true));
						}
					}
					entryPoints.Add(p);
					last = p;
				}
			}
			Debug.WriteLine("N-points: " + entryPoints.Count);
			threads = adv.Start(radius, steps, dt, entryPoints, 40, notifier); //TODO

			Debug.WriteLine("Threads are creared, waiting untilthey all die");
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
				//Debug.WriteLine("threads.Count " + threads.Count + " Max " + threadMax +
				//" Fraction " + ((threadMax - threads.Count) * 1000 / threadMax) + "/" + 1000);
			}

			worker.ReportProgress(0);
			Debug.WriteLine("StreamLines were calculated");
			Debug.WriteLine("Start creating uniform FTLE field");
			int resolution = int.Parse(this.resolution.Text);
			FTLEField field = new FTLEField(ids, resolution);

			threads = field.PrepareField(notifier, sls);
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
				worker.ReportProgress((threadMax - threads.Count) * 1000 / threadMax);
				Debug.WriteLine("threads.Count " + threads.Count + " Max " + threadMax +
				" Fraction " + ((threadMax - threads.Count) * 1000 / threadMax) + "/" + 1000);
			}

			worker.ReportProgress(0);
			threads = field.Start(notifier, resolution);
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
				worker.ReportProgress((threadMax - threads.Count) * 1000 / threadMax);
				Debug.WriteLine("threads.Count " + threads.Count + " Max " + threadMax +
				" Fraction " + ((threadMax - threads.Count) * 1000 / threadMax) + "/" + 1000);
			}

			worker.ReportProgress(0);
			field.CreateSquares(resolution, worker);

			field.Serialize(selectOutputFolderDialog.SelectedPath, worker);
			Debug.WriteLine("All done!");
		}

		private void ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			this.progressBar.Value = e.ProgressPercentage;
		}

		private void GenericTextBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			if(!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.')
			{
				// NOK
				e.Handled = true;
			}

			// only allow one decimal point
			if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
			{
				// NOK
				e.Handled = true;
			}
		}

		private void IntegerTextBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			GenericTextBoxKeyPress(sender, e);
			if(e.KeyChar == '.')
			{
				// actually, NOK
				e.Handled = true;
			}
		}

		private void GenericTextChange(object sender, EventArgs e)
		{
			Debug.WriteLine(sender);
			System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)sender;
			if (tb.Text.Length == 0)
			{
				tb.BackColor = Color.PaleVioletRed;
			}
			else
			{
				if(double.TryParse(tb.Text, out double result))
				{
					if(result != 0)
						tb.BackColor = System.Drawing.SystemColors.Window;
					else
						tb.BackColor = Color.PaleVioletRed;
				}
				else
				{
					tb.Text = "1";
				}
			}
		}

		private void StepsParamsTextChange(object sender, EventArgs e)
		{
			GenericTextChange(sender, e);
			//double dt = double.Parse(this.dt.Text);
			if(!double.TryParse(this.dt.Text, out double dt))
			{
				dt = 1;
			}
			if (!double.TryParse(this.seconds.Text, out double seconds))
			{
				seconds = 1;
			}
			if (!int.TryParse(this.steps.Text, out int steps))
			{
				if(this.steps.Text.Length > 0)
				{
					this.steps.Text = "1";
				}
				steps = 1;
			}
			if(sender.Equals(this.dt))
			{
				this.seconds.Text = "" + steps * dt;
			}
			else if(sender.Equals(this.seconds))
			{
				this.steps.Text = "" + (int)(Math.Ceiling(seconds / dt));
			}
			else if(sender.Equals(this.steps))
			{
				this.seconds.Text = "" + steps * dt;
			}
		}

		private void SecondsTextChange(object sender, EventArgs e)
		{
			Debug.WriteLine(",");
		}

		private void GenericTextBoxLeave(object sender, EventArgs e)
		{
			System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)sender;
			if (tb.TextLength == 0)
			{
				tb.Text = "1";
			}
		}

		private void SetAllEnabled(bool enable)
		{
			inputSettingsPanel.Enabled = enable;
			outputSettingsPanel.Enabled = enable;
			startBtn.Enabled = enable;
		}

		private void WorkerDone(object sender, RunWorkerCompletedEventArgs e)
		{
			SetAllEnabled(true);
		}
	}
}
