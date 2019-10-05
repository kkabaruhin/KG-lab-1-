using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace personal_work_1
{
    public partial class Form1 : Form
    {
        List<Point> points;
        List<int> left_break_points; //левые точки разрыва полигона
        List<int> right_break_points; //правые точки разрыва полигона

        public Form1()
        {
            InitializeComponent();

            points = new List<Point>();
            left_break_points = new List<int>();
            right_break_points = new List<int>();

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Bitmap bmp = (Bitmap)pictureBox1.Image;
            Clear();
            pictureBox1.Image = bmp;
        }

        private void Clear()
        {
            var g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);
            pictureBox1.Image = pictureBox1.Image;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            points.Add(new Point(e.X, e.Y));
            ((Bitmap)pictureBox1.Image).SetPixel(e.X, e.Y, Color.Black);

            pictureBox1.Image = pictureBox1.Image;
        }

        //угол между тремя точками
        double angle_point(Point a, Point b, Point c)
        {

            double abx = b.X - a.X,
                aby = b.Y - a.Y,
                bcx = c.X - b.X,
                bcy = c.Y - b.Y;

            double cos = (double)(abx * bcx + aby * bcy) / (Math.Sqrt(abx * abx + aby * aby) * Math.Sqrt(bcx * bcx + bcy * bcy));

            return Math.Acos(cos);

        }

        //находится ли точка p3 левее отрезка p1-p2
        private bool left(Point p1, Point p2, Point p3)
        {
            Point a = new Point(p2.X - p1.X, p2.Y - p1.Y), b = new Point(p3.X - p1.X, p3.Y - p1.Y);

            return b.Y * a.X - b.X * a.Y < 0;
        }

        //является ли точка p2 левым изгибом
        private bool left_bend(Point p1, Point p2, Point p3)
        {
            if (p1.X < p2.X || p3.X < p2.X || !left(p1,p2,p3))
                return false;

            if (angle_point(p1, p2, p3) > Math.PI / 2)
                return true;

            return false;
        }

        //является ли точка p2 правым изгибом
        private bool right_bend(Point p1, Point p2, Point p3)
        {
            if (p1.X > p2.X || p3.X > p2.X || !left(p1, p2, p3))
                return false;

            if (angle_point(p1, p2, p3) > Math.PI / 2)
                return true;

            return false;
        }

        bool less1(double p1, double p2)
        {
            return (p1 < p2 && Math.Abs(p1 - p2) >= 0.00001);
        }

        bool less_or_equal(double x, double y)
        {
            return Math.Abs(x - y) < 0.0001 || less1(x, y);
        }

        private bool vert_intersect(Tuple<double, double> c1, Tuple<double, double> c2, Tuple<double, double> d1, Tuple<double, double> d2, ref Point p)
        {
            double x1 = c1.Item1;
            //Ax+b=y, A - tang
            double tan2 = (d1.Item2 - d2.Item2) / (d1.Item1 - d2.Item1);
            double b2 = d1.Item2 - tan2 * d1.Item1;
            double y1 = tan2 * x1 + b2;

            //x,y лежат в заданных диапазонах 
            if (less_or_equal(d1.Item1, x1) && less_or_equal(x1, d2.Item1) && less_or_equal(Math.Min(c1.Item2, c2.Item2), y1)
                        && less_or_equal(y1, Math.Max(c1.Item2, c2.Item2)))
            {
                p.X = Convert.ToInt32(Math.Round(x1));
                p.Y = Convert.ToInt32(Math.Round(y1));
                return true;
            }
            return false;
        }


        bool intersect(Tuple<double, double> r1, Tuple<double, double> r2, Tuple<double, double> d1, Tuple<double, double> d2, ref Point p)
        {
            if (r1.Item1 == d1.Item1 && r1.Item2 == d1.Item2 || r1.Item1 == d2.Item1 && r1.Item2 == d2.Item2)
            {
                p = new Point(Convert.ToInt32(Math.Round(r1.Item1)), Convert.ToInt32(Math.Round(r1.Item2)));
                return true;
            }

            if (r2.Item1 == d1.Item1 && d1.Item2 == r2.Item2 || d1.Item1 == r2.Item1 && d1.Item2 == r2.Item2)
            {
                p = new Point(Convert.ToInt32(Math.Round(d1.Item1)), Convert.ToInt32(Math.Round(d1.Item2)));
                return true;
            }

            if (Math.Abs(d1.Item1 - d2.Item1) < 0.0001)
                return vert_intersect(d1, d2, r1, r2, ref p);

            if (Math.Abs(r1.Item1 - r2.Item1) < 0.0001)
                return vert_intersect(r1, r2, d1, d2, ref p);

            //Ax+b = y
            double a1 = (r1.Item2 - r2.Item2) / (r1.Item1 - r2.Item1);
            double a2 = (d1.Item2 - d2.Item2) / (d1.Item1 - d2.Item1);
            double b1 = r1.Item2 - a1 * r1.Item1;
            double b2 = d1.Item2 - a2 * d1.Item1;

            if (a1 == a2)
                return false; //отрезки параллельны


            //x - абсцисса точки пересечения двух прямых
            double x = (b2 - b1) / (a1 - a2);
            double y = a1 * x + b1;

            if (less1(x, Math.Min(r1.Item1, r2.Item1)) || less1(Math.Max(r1.Item1, r2.Item1), x) ||
                less1(x, Math.Min(d1.Item1, d2.Item1)) || less1(Math.Max(d1.Item1, d2.Item1), x) ||
                less1(y, Math.Min(r1.Item2, r2.Item2)) || less1(Math.Max(r1.Item2, r2.Item2), y) ||
                less1(y, Math.Min(d1.Item2, d2.Item2)) || less1(Math.Max(d1.Item2, d2.Item2), y))
                return false; //точка x находится вне пересечения проекций отрезков на ось X 

            p.X = Convert.ToInt32(Math.Round(x));
            p.Y = Convert.ToInt32(Math.Round(y));
            return true;
        }

        //поиск точки пересечения
        private bool intersection_point_search(Point a, Point b, Point c, Point d, ref Point p)
        {
            return intersect(new Tuple<double, double>(a.X, a.Y),
                new Tuple<double, double>(b.X, b.Y),
                new Tuple<double, double>(c.X, c.Y),
                new Tuple<double, double>(d.X, d.Y), ref p);
        }

        // нахождение точки пересечения отрезка, заданного точками p1 p2 c вертикальным/горизонтальным отрезком, заданным точками p3 p4
        public Point Crossing(Point p1, Point p2, Point p3, Point p4)
        {
            if (p3.X == p4.X)   // вертикаль
            {
                if (p2.X == p1.X)
                    return new Point(0, 0);
                double y = p1.Y + ((p2.Y - p1.Y) * (p3.X - p1.X)) / (p2.X - p1.X);
                if (y > Math.Max(p3.Y, p4.Y) || y < Math.Min(p3.Y, p4.Y) || y > Math.Max(p1.Y, p2.Y) || y < Math.Min(p1.Y, p2.Y))   // если за пределами отрезков
                    return new Point(0, 0);
                else
                    return new Point(p3.X, (int)y);
            }
            else            // горизонталь
            {
                double x = p1.X + ((p2.X - p1.X) * (p3.Y - p1.Y)) / (p2.Y - p1.Y);
                if (x > Math.Max(p3.X, p4.X) || x < Math.Min(p3.X, p4.X) || x > Math.Max(p1.X, p2.X) || x < Math.Min(p1.X, p2.X))   // если за пределами отрезков
                    return new Point(0, 0);
                else
                    return new Point((int)x, p3.Y);
            }
        }

        //если l_r == 0, то ищем для левого изгиба, иначе для правого
        private int find_new_break_point(int index, bool l_r)
        {
            bool f1 = false, f2 = false;
            Point straight1, straight2;
            straight1 = new Point(points[index].X, 0);
            straight2 = new Point(points[index].X, pictureBox1.Image.Height);

            Point p = new Point(0,0);

            Point down_close = new Point(-1, 0);
            Point up_close = new Point(-1, pictureBox1.Image.Height);

            int ind_a_down = 0, ind_b_down = 0, ind_a_up = 0, ind_b_up = 0;

            for (int i = 0; i < points.Count() - 1; i++)
                if (i != index && i + 1 != index)
                {
                    p = Crossing(points[i], points[i + 1], straight1, straight2);

                    if (!(p.X == 0 && p.Y == 0))
                    {
                        if (p.Y > down_close.Y && p.Y < points[index].Y)
                        {
                            f1 = true;
                            down_close = p;
                            ind_a_down = i;
                            ind_b_down = i + 1;
                        }

                        if (p.Y < up_close.Y && p.Y > points[index].Y)
                        {
                            f2 = true;
                            up_close = p;
                            ind_a_up = i;
                            ind_b_up = i + 1;
                        }
                    }
                }

            int ind_best_up = -1, ind_best_down = -1;

            if (f1)
                if (!l_r)
                {
                    if (points[ind_a_down].X < points[ind_b_down].X)
                        ind_best_down = ind_a_down;
                    else
                        ind_best_down = ind_b_down;
                }
                else
                {
                    if (points[ind_a_down].X > points[ind_b_down].X)
                        ind_best_down = ind_a_down;
                    else
                        ind_best_down = ind_b_down;
                }

            if (f2)
                if (!l_r)
                {
                    if (points[ind_a_up].X < points[ind_b_up].X)
                        ind_best_up = ind_a_up;
                    else
                        ind_best_up = ind_b_up;
                }
                else
                {
                    if (points[ind_a_up].X > points[ind_b_up].X)
                        ind_best_up = ind_a_up;
                    else
                        ind_best_up = ind_b_up;
                }

            int best_ind = Math.Max(ind_best_down, ind_best_up);

            return best_ind;

        }

        //очистить
        private void button1_Click(object sender, EventArgs e)
        {
            Clear();

            points.Clear();
            left_break_points.Clear();
            right_break_points.Clear();
        }

        //нарисовать
        private void button2_Click(object sender, EventArgs e)
        {
            if (points.Count() < 2)
                return;

            var pen = new Pen(Color.Black, 1);
            var g = Graphics.FromImage(pictureBox1.Image);


            for (int i = 0; i < points.Count() - 1; i++)
            {
                g.DrawLine(pen, points[i], points[i + 1]);
            }
            g.DrawLine(pen, points[0], points[points.Count() - 1]);

            pen.Dispose();
            g.Dispose();
            pictureBox1.Image = pictureBox1.Image;

        }

        //разбить
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            if (points.Count() < 3)
                return;

            /*Point straight1, straight2;
            straight1 = new Point(points[1].X, 0);
            straight2 = new Point(points[1].X, pictureBox1.Image.Height);

            Point p = new Point(-1, -1);
            //intersection_point_search(straight1, straight2, points[2], points[3], ref p);

            p = Crossing(points[2], points[3], straight1, straight2);

            textBox1.Text = p.X + "; " + p.Y;*/

            if (left_bend(points[points.Count() - 2], points[points.Count() - 1], points[0]))
            {
                left_break_points.Add(points.Count() - 1);
                int new_b_p = find_new_break_point(points.Count() - 1, false);
                if (new_b_p != -1)
                    left_break_points.Add(new_b_p);
            }

            if (left_bend(points[points.Count() - 1], points[0], points[1]))
            {
                left_break_points.Add(0);
                int new_b_p = find_new_break_point(0, false);
                if (new_b_p != -1)
                    left_break_points.Add(new_b_p);
            }

            if (right_bend(points[points.Count() - 2], points[points.Count() - 1], points[0]))
            { 
                right_break_points.Add(points.Count() - 1);
                int new_b_p = find_new_break_point(points.Count() - 1, true);
                if (new_b_p != -1)
                    right_break_points.Add(new_b_p);
            }

            if (left_bend(points[points.Count() - 1], points[0], points[1]))
            {
                right_break_points.Add(0);
                int new_b_p = find_new_break_point(0, true);
                if (new_b_p != -1)
                    right_break_points.Add(new_b_p);
            }

            for (int i = 1; i < points.Count() - 1; i++)
            {
                if (left_bend(points[i - 1], points[i], points[i + 1]))
                {
                    left_break_points.Add(i);
                    int new_b_p = find_new_break_point(i, false);
                    if (new_b_p != -1)
                        left_break_points.Add(new_b_p);
                }

                if (right_bend(points[i - 1], points[i], points[i + 1]))
                {
                    right_break_points.Add(i);
                    int new_b_p = find_new_break_point(i, true);
                    if (new_b_p != -1)
                        right_break_points.Add(new_b_p);
                }

            }

            var pen = new Pen(Color.Black, 1);
            var g = Graphics.FromImage(pictureBox1.Image);

            textBox3.Text = left_break_points.Count() + "; " + right_break_points.Count();

            for (int i = 0; i < left_break_points.Count() - 1; i++)
            {
                textBox1.Text += left_break_points[i] + " " + left_break_points[i + 1] + "; ";
                g.DrawLine(pen, points[left_break_points[i]], points[left_break_points[i + 1]]);
                
            }

            for (int i = 0; i < right_break_points.Count() - 1; i++)
            {
                textBox2.Text += right_break_points[i] + " " + right_break_points[i + 1] + "; ";
                g.DrawLine(pen, points[right_break_points[i]], points[right_break_points[i + 1]]);

            }


            pictureBox1.Image = pictureBox1.Image;
            pen.Dispose();
            g.Dispose();

            left_break_points.Clear();
            right_break_points.Clear();

        }
    }
}

/*private int find_new_break_point(int index, bool l_r)
        {
            bool f1 = false, f2 = false;
            Point straight1, straight2;
            straight1 = new Point(points[index].X, 0);
            straight2 = new Point(points[index].X, pictureBox1.Image.Height);

            Point p = new Point(0,0);

            Point down_close = new Point(-1, 0);
            Point up_close = new Point(-1, pictureBox1.Image.Height);

            int ind_a_down = 0, ind_b_down = 0, ind_a_up = 0, ind_b_up = 0;

            for (int i = 0; i < points.Count() - 1; i++)
                if (i != index && i + 1 != index)
                    if (intersection_point_search(straight1, straight2, points[i], points[i + 1], ref p))
                    {
                        if (p.Y > down_close.Y && p.Y < points[index].Y)
                        {
                            f1 = true;
                            down_close = p;
                            ind_a_down = i;
                            ind_b_down = i + 1;
                        }

                        if (p.Y < up_close.Y && p.Y > points[index].Y)
                        {
                            f2 = true;
                            up_close = p;
                            ind_a_up = i;
                            ind_b_up = i + 1;
                        }
                    }

            int ind_best_up = -1, ind_best_down = -1;

            if (f1)
                if (l_r)
                {
                    if (points[ind_a_down].X < points[ind_b_down].X)
                        ind_best_down = ind_a_down;
                    else
                        ind_best_down = ind_b_down;
                }
                else
                {
                    if (points[ind_a_down].X > points[ind_b_down].X)
                        ind_best_down = ind_a_down;
                    else
                        ind_best_down = ind_b_down;
                }

            if (f2)
                if (l_r)
                {
                    if (points[ind_a_up].X < points[ind_b_up].X)
                        ind_best_up = ind_a_up;
                    else
                        ind_best_up = ind_b_up;
                }
                else
                {
                    if (points[ind_a_up].X > points[ind_b_up].X)
                        ind_best_up = ind_a_up;
                    else
                        ind_best_up = ind_b_up;
                }

            int best_ind = Math.Max(ind_best_down, ind_best_up);

            return best_ind;

        }*/
