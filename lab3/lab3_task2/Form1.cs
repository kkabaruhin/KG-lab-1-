// Выделение границы связной области

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace lab3_task2
{
    public partial class Form1 : Form
    {
        Bitmap bmp = null;
        Dictionary<int, SortedSet<int>> dict;
        Bitmap border = null;

        public Form1()
        {
            InitializeComponent();
        }

        // для добавления новой пары x, y в dict
        bool addOrUpdate(int key, int newValue)
        {
            bool res = false;
            SortedSet<int> val;
            if (dict.TryGetValue(key, out val))
            {
                res = dict[key].Add(newValue);
            }
            else
            {
                dict[key] = new SortedSet<int>();
                res = dict[key].Add(newValue);
            }
            return res;
        }

        private void FindBorder(object sender, EventArgs e)
        {
            // x и y - координаты точки в области, границу которой ищем
            var x = Convert.ToInt32(textBox1.Text); 
            var y = Convert.ToInt32(textBox2.Text);
            var areaColor = bmp.GetPixel(x, y); // цвет области, границу которой ищем
            dict = new Dictionary<int, SortedSet<int>>();  // y - ключ, иксы упорядочены от меньшего к большему
            border = new Bitmap(bmp);

            while (bmp.GetPixel(x + 1, y) == areaColor) // ищем первую точку границы
                x += 1;

            addOrUpdate(y, x + 1);
            char currentDirection = 'd';

            // стартовые значения x, y и направления 
            var x_start = x;
            var y_start = y;
            var startDirection = 'd';

            if (bmp.GetPixel(x, y + 1) != areaColor)
                currentDirection = 'l';
            else
                y += 1;

            // идем по часовой стрелке, при этом держим границу слева относительно текущего пикселя
            while (x != x_start || y != y_start || currentDirection != startDirection)
            {
                switch (currentDirection){
                    case 'd':
                        if (bmp.GetPixel(x + 1, y) != areaColor)
                        {
                            addOrUpdate(y, x + 1);
                            if (bmp.GetPixel(x, y + 1) != areaColor)
                                currentDirection = 'l';
                            else
                                y += 1;
                        }
                        else
                        {
                            x += 1;
                            currentDirection = 'r';
                        }
                        break;

                    case 'r':
                        if (bmp.GetPixel(x, y - 1) != areaColor)
                        {
                            addOrUpdate(y - 1, x);
                            if (bmp.GetPixel(x + 1, y) != areaColor)
                                currentDirection = 'd';
                            else
                                x += 1;
                        }
                        else
                        {
                            y -= 1;
                            currentDirection = 'u';
                        }
                        break;

                    case 'u':
                        if (bmp.GetPixel(x - 1, y) != areaColor)
                        {
                            addOrUpdate(y, x - 1);
                            if (bmp.GetPixel(x, y - 1) != areaColor)
                                currentDirection = 'r';
                            else
                                y -= 1;
                        }
                        else
                        {
                            x -= 1;
                            currentDirection = 'l';
                        }
                        break;

                    case 'l':
                        if (bmp.GetPixel(x, y + 1) != areaColor)
                        {
                            addOrUpdate(y + 1, x);
                            if (bmp.GetPixel(x - 1, y) != areaColor)
                                currentDirection = 'u';
                            else
                                x -= 1;
                        }
                        else
                        {
                            y += 1;
                            currentDirection = 'd';
                        }
                        break;
                }

            }

            // строим границу по точкам
            foreach (KeyValuePair<int, SortedSet<int>> y_entry in dict)
            {
                foreach (var x_entry in y_entry.Value)
                {
                    border.SetPixel(x_entry, y_entry.Key, Color.Red);
                }
            }
            pictureBox1.Image = border;
            checkBox1.Checked = true;
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = new Bitmap(ofd.FileName);
                    bmp = new Bitmap(ofd.FileName);
                }
                catch
                {
                    MessageBox.Show("Невозможно открыть выбранный файл", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // координаты стартовой точки области
        private void PictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;
            textBox1.Text = coordinates.X.ToString();
            textBox2.Text = coordinates.Y.ToString();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                pictureBox1.Image = border;
            else
                pictureBox1.Image = bmp;
        }
    }
}
