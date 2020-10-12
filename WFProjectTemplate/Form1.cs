using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WFProjectTemplate
{
    public partial class Form1 : Form
    {
        private List<Point> points;
        private List<Point> hull;

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics graphics = Graphics.FromImage(pictureBox1.Image);
            graphics.Clear(Color.White);
            points = new List<Point>();
            hull = new List<Point>();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Graphics graphics = Graphics.FromImage(pictureBox1.Image);
            Pen pen = new Pen(Color.Blue);
            if (e.Button == MouseButtons.Left)
            {
                points.Add(new Point(e.X, e.Y));
                graphics.DrawRectangle(pen, e.X, e.Y, 1, 1);
                pictureBox1.Invalidate();
                if (points.Count >= 3)
                {
                    button1.Enabled = true;
                    button2.Enabled = true;
                }
            }
        }
        
        private int FindSide(Point p, Point A, Point B)
        {
            int val = (p.X - A.X) * (B.Y - A.Y) -
                      (p.Y - A.Y) * (B.X - A.X);
            return (val > 0) ? 1 : ((val < 0) ? -1 : 0);
            // 1  - слева
            // -1 - справа
            // 0  - на линии
        }

        private int Distance(Point p, Point A, Point B)
        {
            return Math.Abs((p.Y - A.Y) * (B.X - A.X) - (B.Y - A.Y) * (p.X - A.X));
        }
        
        private void QuickHull()
        {
            if (points.Count <= 3)
            {
                hull = points;
                return;
            }
            
            int minX = int.MaxValue;
            int indexOfMinX = -1;
            int maxX = int.MinValue;
            int indexOfMaxX = -1;
            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];
                if (p.X > maxX)
                {
                    maxX = p.X;
                    indexOfMaxX = i;
                } else if (p.X < minX)
                {
                    minX = p.X;
                    indexOfMinX = i;
                }
            }

            Point pointMinX = points[indexOfMinX];
            Point pointMaxX = points[indexOfMaxX];
            hull.Add(pointMinX);
            hull.Add(pointMaxX);

            List<Point> left  = new List<Point>();
            List<Point> right = new List<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];
                switch (FindSide(pointMaxX, pointMinX, p))
                {
                    case -1:
                        right.Add(p);
                        break;
                    case 1:
                        left.Add(p);
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
            }
            GenerateHull(pointMinX, pointMaxX, left);
            GenerateHull(pointMaxX, pointMinX, right);
        }

        private void GenerateHull(Point a, Point b, List<Point> points)
        {
            int pos = hull.IndexOf(b);

            if (points.Count == 0)
                return;

            if (points.Count == 1)
            {
                hull.Insert(pos, points[0]);
                return;
            }

            int minDist = int.MinValue;
            int indexOfFarPoint = 0;

            for (int i = 0; i < points.Count; i++)
            {
                int currDist = Distance(points[i], a, b);
                if (currDist > minDist)
                {
                    minDist = currDist;
                    indexOfFarPoint = i;
                }
            }

            Point p = points[indexOfFarPoint];
            hull.Insert(pos, p);
            List<Point> segmentAP = new List<Point>();
            List<Point> segpentPB = new List<Point>();
            
            for (int i = 0; i < points.Count; i++)
            {
                Point currPoint = points[i];
                if (FindSide(p, a, currPoint) == 1)
                {
                    segmentAP.Add(currPoint);
                }
                
                if (FindSide(b, p, currPoint) == 1)
                {
                    segpentPB.Add(currPoint);
                }
            }
            GenerateHull(a, p, segmentAP);
            GenerateHull(p, b, segpentPB);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            hull.Clear();
            if (points.Count != 0)
            {
                pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                Graphics graphics1 = Graphics.FromImage(pictureBox1.Image);
                graphics1.Clear(Color.White);
                Pen pen1 = new Pen(Color.Blue);
                foreach (var p in points)
                {
                    graphics1.DrawRectangle(pen1, p.X, p.Y, 1, 1);
                    pictureBox1.Invalidate();
                }
            }
            Graphics graphics = Graphics.FromImage(pictureBox1.Image);
            Pen pen = new Pen(Color.Blue);
            QuickHull();
            graphics.DrawPolygon(pen, hull.ToArray());
            pictureBox1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics graphics = Graphics.FromImage(pictureBox1.Image);
            graphics.Clear(Color.White);
            points.Clear();
            hull.Clear();
            button1.Enabled = false;
            button2.Enabled = false;
        }
    }
}
