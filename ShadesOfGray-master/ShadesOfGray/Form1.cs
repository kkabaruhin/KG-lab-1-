using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShadesOfGray
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // кнопка Открыть
        private void openButton_Click(object sender, EventArgs e)
        {
            // диалог для выбора файла
            OpenFileDialog ofd = new OpenFileDialog();
            // фильтр форматов файлов
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            // если в диалоге была нажата кнопка ОК
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // загружаем изображение
                    pictureBox1.Image = new Bitmap(ofd.FileName);
                }
                catch // в случае ошибки выводим MessageBox
                {
                    MessageBox.Show("Невозможно открыть выбранный файл", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // кнопка Сохранить
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null) // если изображение в pictureBox2 имеется
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Сохранить картинку как...";
                sfd.OverwritePrompt = true; // показывать ли "Перезаписать файл" если пользователь указывает имя файла, который уже существует
                sfd.CheckPathExists = true; // отображает ли диалоговое окно предупреждение, если пользователь указывает путь, который не существует
                // фильтр форматов файлов
                sfd.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                sfd.ShowHelp = true; // отображается ли кнопка Справка в диалоговом окне
                // если в диалоге была нажата кнопка ОК
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // сохраняем изображение
                        pictureBox2.Image.Save(sfd.FileName);
                    }
                    catch // в случае ошибки выводим MessageBox
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // кнопка Ч/Б
        private void grayButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null) // если изображение в pictureBox1 имеется
            {
                // создаём Bitmap из изображения, находящегося в pictureBox1
                Bitmap input = new Bitmap(pictureBox1.Image);
                // создаём Bitmap для черно-белого изображения
                Bitmap output = new Bitmap(input.Width, input.Height);
                // перебираем в циклах все пиксели исходного изображения
                for (int j = 0; j < input.Height; j++)
                    for (int i = 0; i < input.Width; i++)
                    {
                        // получаем (i, j) пиксель
                        UInt32 pixel = (UInt32)(input.GetPixel(i, j).ToArgb());
                        // получаем компоненты цветов пикселя
                        float R = (float)((pixel & 0x00FF0000) >> 16); // красный
                        float G = (float)((pixel & 0x0000FF00) >> 8); // зеленый
                        float B = (float)(pixel & 0x000000FF); // синий
                                                               // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                        R = G = B = (R + G + B) / 3.0f;
                        // собираем новый пиксель по частям (по каналам)
                        UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        // добавляем его в Bitmap нового изображения
                        output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                    }
                // выводим черно-белый Bitmap в pictureBox2
                pictureBox2.Image = output;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null) // если изображение в pictureBox1 имеется
            {
                // создаём Bitmap из изображения, находящегося в pictureBox1
                Bitmap input = new Bitmap(pictureBox1.Image);
                // создаём Bitmap для черно-белого изображения
                Bitmap output = new Bitmap(input.Width, input.Height);
                // перебираем в циклах все пиксели исходного изображения
                for (int j = 0; j < input.Height; j++)
                    for (int i = 0; i < input.Width; i++)
                    {
                        // получаем (i, j) пиксель
                        UInt32 pixel = (UInt32)(input.GetPixel(i, j).ToArgb());
                        // получаем компоненты цветов пикселя
                        float R = (float)((pixel & 0x00FF0000) >> 16); // красный
                        float G = (float)((pixel & 0x0000FF00) >> 8); // зеленый
                        float B = (float)(pixel & 0x000000FF); // синий
                                                               // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                        R = G = B = (R * 0.299f + G * 0.587f + B * 0.114f);
                        // собираем новый пиксель по частям (по каналам)
                        UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        // добавляем его в Bitmap нового изображения
                        output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                    }
                // выводим черно-белый Bitmap в pictureBox2
                pictureBox3.Image = output;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null && pictureBox3.Image != null) // если изображение в pictureBox1 имеется
            {
                // создаём Bitmap из изображения, находящегося в pictureBox1
                Bitmap input1 = new Bitmap(pictureBox2.Image);
                Bitmap input2 = new Bitmap(pictureBox3.Image);
                // создаём Bitmap для черно-белого изображения
                Bitmap output = new Bitmap(input1.Width, input1.Height);
                // перебираем в циклах все пиксели исходного изображения
                float mindif = 1;
                for (int j = 0; j < input1.Height; j++)
                    for (int i = 0; i < input1.Width; i++)
                    {
                        // получаем (i, j) пиксель
                        UInt32 pixel1 = (UInt32)(input1.GetPixel(i, j).ToArgb());
                        UInt32 pixel2 = (UInt32)(input2.GetPixel(i, j).ToArgb());
                        // получаем компоненты цветов пикселя
                        float R1 = (float)((pixel1 & 0x00FF0000) >> 16);
                        float R2 = (float)((pixel2 & 0x00FF0000) >> 16);

                        if (R1 - R2 < mindif)
                            mindif = R1 - R2;
                    }
                for (int j = 0; j < input1.Height; j++)
                    for (int i = 0; i < input1.Width; i++)
                    {
                        // получаем (i, j) пиксель
                        UInt32 pixel1 = (UInt32)(input1.GetPixel(i, j).ToArgb());
                        UInt32 pixel2 = (UInt32)(input2.GetPixel(i, j).ToArgb());

                        float B1 = (float)((pixel1 & 0x00FF0000) >> 16); // синий
                                                                 // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                        float B2 = (float)((pixel2 & 0x00FF0000) >> 16);

                        UInt32 newPixel;

                        if (mindif < 0)
                        {
                            newPixel = 0xFF000000 |
                            (((UInt32)B1 - (UInt32)B2 - (UInt32)mindif) << 16  |
                            ((UInt32)B1 - (UInt32)B2 - (UInt32)mindif) << 8)  |
                            (UInt32)B1 - (UInt32)B2 - (UInt32)mindif;

                        }
                        else
                        {
                            newPixel = 0xFF000000 |
                            ((UInt32)B1 - (UInt32)B2) << 16 |
                            ((UInt32)B1 - (UInt32)B2) << 8 |
                            (UInt32)B1 - (UInt32)B2;
                        }

                        // добавляем его в Bitmap нового изображения
                        output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                    }
                // выводим черно-белый Bitmap в pictureBox2
                pictureBox4.Image = output;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap input = new Bitmap(pictureBox1.Image);

            Bitmap barChart = null;
            if (pictureBox1 != null)
            {
                // определяем размеры гистограммы. В идеале, ширина должны быть кратна 768 - 
                // по пикселю на каждый столбик каждого из каналов
                int width = 768, height = 600;
                // получаем битмап из изображения
                Bitmap bmp = new Bitmap(pictureBox1.Image);
                // создаем саму гистограмму
                barChart = new Bitmap(width, height);
                // создаем массивы, в котором будут содержаться количества повторений для каждого из значений каналов.
                // индекс соответствует значению канала
                int[] R = new int[256];
                int[] G = new int[256];
                int[] B = new int[256];
                int i, j;
                Color color;
                // собираем статистику для изображения
                for (i = 0; i < bmp.Width; ++i)
                    for (j = 0; j < bmp.Height; ++j)
                    {
                        color = bmp.GetPixel(i, j);
                        ++R[color.R];
                        ++G[color.G];
                        ++B[color.B];
                    }
                // находим самый высокий столбец, чтобы корректно масштабировать гистограмму по высоте
                int max = 0;
                for (i = 0; i < 256; ++i)
                {
                    if (R[i] > max)
                        max = R[i];
                    if (G[i] > max)
                        max = G[i];
                    if (B[i] > max)
                        max = B[i];
                }
                // определяем коэффициент масштабирования по высоте
                double point = (double)max / height;
                // отрисовываем столбец за столбцом нашу гистограмму с учетом масштаба
                for (i = 0; i < width - 3; ++i)
                {
                    for (j = height - 1; j > height - R[i / 3] / point; --j)
                    {
                        barChart.SetPixel(i, j, Color.Red);
                    }
                    ++i;
                    for (j = height - 1; j > height - G[i / 3] / point; --j)
                    {
                        barChart.SetPixel(i, j, Color.Green);
                    }
                    ++i;
                    for (j = height - 1; j > height - B[i / 3] / point; --j)
                    {
                        barChart.SetPixel(i, j, Color.Blue);
                    }
                }
            }
            else
                barChart = new Bitmap(1, 1);
            pictureBox5.Image = barChart;

        }
    }
}
