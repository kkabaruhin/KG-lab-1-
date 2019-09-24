using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab3__task_1a___1b_
{
    public partial class Form1 : Form
    {
        bool color_selected = false;
        Color start_point_color;
        Color fill_color;
        int start_x = -1, start_y = -1;

        public Form1()
        {
            InitializeComponent();
        }

        private void button_open_area(object sender, EventArgs e)
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

        private void button_open_picture(object sender, EventArgs e)
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
                    pictureBox2.Image = new Bitmap(ofd.FileName);
                }
                catch // в случае ошибки выводим MessageBox
                {
                    MessageBox.Show("Невозможно открыть выбранный файл", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button_choose_color(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            // Keeps the user from selecting a custom color.
            MyDialog.AllowFullOpen = true;
            // Allows the user to get help. (The default is false.)
            MyDialog.ShowHelp = true;

            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox4.BackColor = MyDialog.Color;
                fill_color = MyDialog.Color;
                color_selected = true;
            }
        }

        private void fill_line(ref Bitmap bm, int x_input, int y_input)
        {
            if (y_input < 0 || y_input >= bm.Height || x_input < 0 || x_input >= bm.Width)
                return;

            if (bm.GetPixel(x_input, y_input) != start_point_color)
                return;

            int x = x_input, left, right;

            while (bm.GetPixel(x, y_input) == start_point_color)
            {
                bm.SetPixel(x, y_input, fill_color);
                x = x - 1;
                if (x < 0)
                    break;
            }

            left = x + 1;

            x = x_input + 1;

            if (x < bm.Width)
                while (bm.GetPixel(x, y_input) == start_point_color)
                {
                    bm.SetPixel(x, y_input, fill_color);
                    x = x + 1;
                    if (x >= bm.Width)
                        break;
                }

            right = x;

            for (int i = left; i < right; i++)
                fill_line(ref bm, i, y_input + 1);

            for (int i = left; i < right; i++)
                fill_line(ref bm, i, y_input - 1);
        }

        private void button_fill_color(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null && color_selected && start_x != -1 && start_y != -1)
            {
                
                Bitmap image = new Bitmap(pictureBox1.Image);

                if (fill_color.R != start_point_color.R || fill_color.G != start_point_color.G || fill_color.B != start_point_color.B)
                    fill_line(ref image, start_x, start_y);

                pictureBox3.Image = image;
            }
        }

        private void fill_line_picture(ref Bitmap bm, Bitmap bm_picture, int x_input, int y_input)
        {
            if (y_input < 0 ||
                y_input >= bm.Height || 
                y_input >= bm_picture.Height || 
                x_input < 0 || 
                x_input >= bm.Width)
                return;

            if (bm.GetPixel(x_input, y_input) != start_point_color)
                return;

            int x = x_input, left, right;

            while (bm.GetPixel(x, y_input) == start_point_color)
            {
                bm.SetPixel(x, y_input, bm_picture.GetPixel(x, y_input));
                x = x - 1;
                if (x < 0)
                    break;
            }

            left = x + 1;

            x = x_input + 1;

            if (x < bm.Width)
                while (bm.GetPixel(x, y_input) == start_point_color)
                {
                    if (x < bm_picture.Width)
                        bm.SetPixel(x, y_input, bm_picture.GetPixel(x, y_input));
                    x = x + 1;
                    if (x >= bm.Width)
                        break;
                }

            right = x;

            for (int i = left; i < right; i++)
                fill_line_picture(ref bm, bm_picture, i, y_input + 1);

            for (int i = left; i < right; i++)
                fill_line_picture(ref bm, bm_picture, i, y_input - 1);
        }

        private void button_fill_picture(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null && pictureBox2.Image != null && start_x != -1 && start_y != -1)
            {
                Bitmap image = new Bitmap(pictureBox1.Image);
                Bitmap picture = new Bitmap(pictureBox2.Image);

                fill_line_picture(ref image, picture, start_x, start_y);
                pictureBox5.Image = image;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                start_x = Cursor.Position.X - pictureBox1.Location.X - this.Location.X - SystemInformation.BorderSize.Width - 7;
                start_y = Cursor.Position.Y - pictureBox1.Location.Y - this.Location.Y - SystemInformation.CaptionHeight - 8;

                //start_x = Control.MousePosition.X - pictureBox1.Location.X - SystemInformation.BorderSize.Width;
                //start_y = Control.MousePosition.Y - pictureBox1.Location.Y - SystemInformation.CaptionHeight;
                //если курсор находится за пределами изображения
                /*if (start_x < ((pictureBox1.Width - pictureBox1.Image.Width) / 2)
                    || start_x > ((pictureBox1.Width + pictureBox1.Image.Width) / 2)
                    || start_y < ((pictureBox1.Height - pictureBox1.Image.Height) / 2)
                    || start_y > ((pictureBox1.Height + pictureBox1.Image.Height) / 2))
                {
                    label2.Text = "Выберите точку в пределах изображения.";
                    //start_x = start_y = -1;
                }
                else*/
                label2.Text = "Начальная точка выбрана. Можете щелкнуть на изображение еще раз, чтобы выбрать другую.";

                Bitmap bm = new Bitmap(pictureBox1.Image);

                start_point_color = bm.GetPixel(start_x, start_y);
            }
        }
    }
}
