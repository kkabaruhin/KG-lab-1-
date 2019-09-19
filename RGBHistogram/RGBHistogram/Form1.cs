using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RGBHistogram
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = new Bitmap(ofd.FileName);
                    pictureBox2.Image = null;
                    pictureBox3.Image = null;
                    pictureBox4.Image = null;
                    pictureBox5.Image = null;
                    pictureBox6.Image = null;
                    pictureBox7.Image = null;
                    label7.Text = "";
                    label8.Text = "";
                    label9.Text = "";
                }
                catch
                {
                    MessageBox.Show("Невозможно открыть выбранный файл", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RgbButton_Click(object sender, EventArgs e)
        {
            Bitmap input = new Bitmap(pictureBox1.Image);
            var width = input.Width;
            var height = input.Height;
            Bitmap[] result = new Bitmap[3] { new Bitmap(width, height), new Bitmap(width, height), new Bitmap(width, height) };
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color color = input.GetPixel(i, j);
                    result[0].SetPixel(i, j, Color.FromArgb(color.A, color.R, 0, 0));
                    result[1].SetPixel(i, j, Color.FromArgb(color.A, 0, color.G, 0));
                    result[2].SetPixel(i, j, Color.FromArgb(color.A, 0, 0, color.B));
                }
            }
            pictureBox2.Image = result[0];
            pictureBox3.Image = result[1];
            pictureBox4.Image = result[2];
        }

        private void HistogramButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                int width = pictureBox5.Width, height = pictureBox5.Height;
                Bitmap bmp = new Bitmap(pictureBox1.Image);
                Bitmap histR = null;
                Bitmap histG = null;
                Bitmap histB = null;
                
                //массивы, для хранения количества повторений каждого из значений каналов
                int[] R = new int[256];
                int[] G = new int[256];
                int[] B = new int[256];

                Color color;
                for (int i = 0; i < bmp.Width; ++i)
                    for (int j = 0; j < bmp.Height; ++j)
                    {
                        color = bmp.GetPixel(i, j);
                        ++R[color.R];
                        ++G[color.G];
                        ++B[color.B];
                    }

                // определяем коэффициенты масштабирования по высоте
                int maxR = 0;
                int maxG = 0;
                int maxB = 0;
                for (int i = 0; i < 256; ++i)
                {
                    if (R[i] > maxR)
                        maxR = R[i];
                    if (G[i] > maxG)
                        maxG = G[i];
                    if (B[i] > maxB)
                        maxB = B[i];
                }
                double pointR = (double)maxR / height;
                double pointG = (double)maxG / height;
                double pointB = (double)maxB / height;
                label7.Text = maxR.ToString(); // выводим максимальные значения для гистограммы
                label8.Text = maxG.ToString();
                label9.Text = maxB.ToString();

                // рисуем гистограмму
                histR = new Bitmap(width, height);
                histG = new Bitmap(width, height);
                histB = new Bitmap(width, height);
                for (int i = 0; i < 256; ++i)
                {
                    for (var j = height - 1; j > height - R[i] / pointR; --j)
                    {
                        histR.SetPixel(i, j, Color.Red);
                    }
                    for (var j = height - 1; j > height - G[i] / pointG; --j)
                    {
                        histG.SetPixel(i, j, Color.Green);
                    }
                    for (var j = height - 1; j > height - B[i] / pointB; --j)
                    {
                        histB.SetPixel(i, j, Color.Blue);
                    }
                }
                pictureBox5.Image = histR;
                pictureBox6.Image = histG;
                pictureBox7.Image = histB;
            }
            else
                return;

        }

    }
}
