using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab2_task_3_
{
    public partial class Form1 : Form
    {

        private Form2 form2 = new Form2();

        public Form1()
        {
            InitializeComponent();
            AddOwnedForm(form2);
        }

        // кнопка Открыть
        private void button1_Click(object sender, EventArgs e)
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

        private byte[] GetHSV(byte _r, byte _g, byte _b)
        {
            double r = (double)_r / 255;
            double g = (double)_g / 255;
            double b = (double)_b / 255;

            double mx = Math.Max(r, Math.Max(g, b));
            double mn = Math.Min(r, Math.Min(g, b));
            byte[] res = new byte[3];
            double H = 0;
            if (Math.Abs(mx - mn) < 0.00001)
                H = 0;
            else if (Math.Abs(mx - r) < 0.0001 && g >= b)
                H = 60 * (g - b) / (mx - mn);
            else if (Math.Abs(mx - r) < 0.00001 && g < b)
                H = 60 * (g - b) / (mx - mn) + 360;
            else if (Math.Abs(mx - g) < 0.00001)
                H = 60 * (b - r) / (mx - mn) + 120;
            else if (Math.Abs(mx - b) < 0.00001)
                H = 60 * (r - g) / (mx - mn) + 240;

            double S;
            if (mx < 0.000001)
                S = 0;
            else
                S = 1 - mn / mx;

            double V = mx;

            res[0] = (byte)(H / 360 * 255);
            res[1] = (byte)(S * 255);
            res[2] = (byte)(V * 255);
            return res;
        }

        //b g r
        private byte[] GetRGB(byte _H, byte _S, byte _V)
        {
            double H = (double)_H / 255 * 360;
            double S = (double)_S / 255 * 100;
            double V = (double)_V / 255 * 100;
            int Hi = (int)H / 60 % 6;
            double Vmin = (100 - S) * V / 100;
            double a = (V - Vmin) * ((int)H % 60) / 60;
            double Vinc = Vmin + a;
            double Vdec = V - a;

            byte[] res = new byte[3];
            switch (Hi)
            {
                // b g r
                default:
                case 0:
                    res[0] = (byte)(Vmin * 255 / 100);
                    res[1] = (byte)(Vinc * 255 / 100);
                    res[2] = (byte)(V * 255 / 100);
                    break;
                case 1:
                    res[0] = (byte)(Vmin * 255 / 100);
                    res[1] = (byte)(V * 255 / 100);
                    res[2] = (byte)(Vdec * 255 / 100);
                    break;
                case 2:
                    res[0] = (byte)(Vinc * 255 / 100);
                    res[1] = (byte)(V * 255 / 100);
                    res[2] = (byte)(Vmin * 255 / 100);
                    break;
                case 3:
                    res[0] = (byte)(V * 255 / 100);
                    res[1] = (byte)(Vdec * 255 / 100);
                    res[2] = (byte)(Vmin * 255 / 100);
                    break;
                case 4:
                    res[0] = (byte)(V * 255 / 100);
                    res[1] = (byte)(Vmin * 255 / 100);
                    res[2] = (byte)(Vinc * 255 / 100);
                    break;
                case 5:
                    res[0] = (byte)(Vdec * 255 / 100);
                    res[1] = (byte)(Vmin * 255 / 100);
                    res[2] = (byte)(V * 255 / 100);
                    break;

            }
            return res;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null) // если изображение в pictureBox1 имеется
            {
                form2.Show();

                // создаём Bitmap из изображения, находящегося в pictureBox1
                Bitmap input = new Bitmap(pictureBox1.Image);
                // создаём Bitmap для черно-белого изображения
                Bitmap output1 = new Bitmap(input.Width, input.Height);
                Bitmap output2 = new Bitmap(input.Width, input.Height);
                Bitmap output3 = new Bitmap(input.Width, input.Height);
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

                        byte[] HSV = GetHSV((byte)R, (byte)G, (byte)B);
                        byte H, S, V, x;
                        H = HSV[0];
                        S = HSV[1];
                        V = HSV[2];

                        byte[] RGB = GetRGB(255, S, V);

                        R = RGB[2];
                        G = RGB[1];
                        B = RGB[0];

                        // собираем новый пиксель по частям (по каналам)
                        UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        // добавляем его в Bitmap нового изображения
                        output1.SetPixel(i, j, Color.FromArgb((int)newPixel));

                        RGB = GetRGB(H, 255, V);

                        R = RGB[2];
                        G = RGB[1];
                        B = RGB[0];

                        // собираем новый пиксель по частям (по каналам)
                        newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        // добавляем его в Bitmap нового изображения
                        output2.SetPixel(i, j, Color.FromArgb((int)newPixel));

                        RGB = GetRGB(H, S, 255);

                        R = RGB[2];
                        G = RGB[1];
                        B = RGB[0];

                        // собираем новый пиксель по частям (по каналам)
                        newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        // добавляем его в Bitmap нового изображения
                        output3.SetPixel(i, j, Color.FromArgb((int)newPixel));
                    }
                // выводим черно-белый Bitmap в pictureBox2
                (form2.Controls["pictureBox1"] as PictureBox).Image = output1;
                (form2.Controls["pictureBox2"] as PictureBox).Image = output2;
                (form2.Controls["pictureBox3"] as PictureBox).Image = output3;
            }
        }

        private void button3_Click(object sender, EventArgs e)
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

                        byte[] HSV = GetHSV((byte)R, (byte)G, (byte)B);
                        byte H, S, V;
                        H = HSV[0];
                        S = HSV[1];
                        V = HSV[2];

                        if (textBox1.Text != "")
                            H += (byte)(byte.Parse(textBox1.Text) % 255);
                        if (textBox2.Text != "")
                            if (int.Parse(textBox2.Text) > 0)
                                 S = (byte)(Math.Min(S + int.Parse(textBox2.Text), 255));
                            else
                                S = (byte)(Math.Max(S + int.Parse(textBox2.Text), 0));
                        if (textBox3.Text != "")
                            if (int.Parse(textBox3.Text) > 0)
                                V = (byte)(Math.Min(V + int.Parse(textBox3.Text), 255));
                            else
                                V = (byte)(Math.Max(V + int.Parse(textBox3.Text), 0));

                        byte[] RGB = GetRGB(H, S, V);

                        R = RGB[2];
                        G = RGB[1];
                        B = RGB[0];

                        // собираем новый пиксель по частям (по каналам)
                        UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        // добавляем его в Bitmap нового изображения
                        output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                    }
                // выводим черно-белый Bitmap в pictureBox2
                pictureBox2.Image = output;
            }
        }
    }

}
/*

public partial class Form1 : Form
{
    private byte[] rgbValues;
    private byte[] disp_rgbValues;
    private byte[] hsvValues;
    private byte[] disp_hsvValues;
    private Bitmap bmp;
    private int avg_h = 0;
    private int avg_s = 0;
    private int avg_v = 0;

    private int cavg_h = 0;
    private int cavg_s = 0;
    private int cavg_v = 0;

    private bool activate_flag;

    public Form1()
    {
        InitializeComponent();
        activate_flag = false;
    }

    private void load_Click(object sender, EventArgs e)
    {
        openFileDialog1.ShowDialog();
    }

    private byte[] GetHSV(byte _r, byte _g, byte _b)
    {
        double r = (double)_r / 255;
        double g = (double)_g / 255;
        double b = (double)_b / 255;

        double mx = Math.Max(r, Math.Max(g, b));
        double mn = Math.Min(r, Math.Min(g, b));
        byte[] res = new byte[3];
        double H = 0;
        if (mx == mn)
            H = 0;
        else if (mx == r && g >= b)
            H = 60 * (g - b) / (mx - mn);
        else if (mx == r && g < b)
            H = 60 * (g - b) / (mx - mn) + 360;
        else if (mx == g)
            H = 60 * (b - r) / (mx - mn) + 120;
        else if (mx == b)
            H = 60 * (r - g) / (mx - mn) + 240;

        double S;
        if (mx == 0)
            S = 0;
        else
            S = 1 - mn / mx;

        double V = mx;

        res[0] = (byte)(H / 360 * 255);
        res[1] = (byte)(S * 255);
        res[2] = (byte)(V * 255);
        return res;
    }

    private void GetDefValues()
    {
        // var g = Graphics.FromImage(pictureBox1.Image);
        bmp = pictureBox1.Image as Bitmap;
        Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        System.Drawing.Imaging.BitmapData bmpData =
        bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
        bmp.PixelFormat);

        // Get the address of the first line.
        IntPtr ptr = bmpData.Scan0;

        // Declare an array to hold the bytes of the bitmap.
        int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
        rgbValues = new byte[bytes];
        hsvValues = new byte[bytes];
        disp_hsvValues = new byte[bytes];
        disp_rgbValues = new byte[bytes];

        // Copy the RGB values into the array.
        System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
        System.Runtime.InteropServices.Marshal.Copy(ptr, disp_rgbValues, 0, bytes);
        bmp.UnlockBits(bmpData);
        avg_h = 0;
        avg_s = 0;
        avg_v = 0;

        // Set every third value to 255. A 24bpp bitmap will look red. 
        for (int counter = 0; counter < rgbValues.Length; counter += 3)
        {
            // rgbValues[counter] = 255; // blue
            // rgbValues[counter+1] = 255; // green
            // rgbValues[counter+2] = 255; // red

            byte[] hsvpixel;
            hsvpixel = GetHSV(rgbValues[counter + 2], rgbValues[counter + 1], rgbValues[counter]);
            hsvValues[counter] = hsvpixel[0]; // hue
            hsvValues[counter + 1] = hsvpixel[1]; // sat
            hsvValues[counter + 2] = hsvpixel[2]; // val

            disp_hsvValues[counter] = hsvpixel[0]; // hue
            disp_hsvValues[counter + 1] = hsvpixel[1]; // sat
            disp_hsvValues[counter + 2] = hsvpixel[2]; // val

            avg_h += hsvpixel[0];
            avg_s += hsvpixel[1];
            avg_v += hsvpixel[2];
            // Copy the RGB values back to the bitmap
            //System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.




            avg_h /= bytes;
            avg_s /= bytes;
            avg_v /= bytes;



            trackBarHUE.Value = avg_h;
            trackBarSat.Value = avg_s;
            trackBarBr.Value = avg_v;

            activate_flag = true;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            activate_flag = false;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = System.Drawing.Image.FromFile(openFileDialog1.FileName);
            GetDefValues();



        }

        private byte[] GetRGB(byte _H, byte _S, byte _V)
        {
            double H = (double)_H / 255 * 360;
            double S = (double)_S / 255 * 100;
            double V = (double)_V / 255 * 100;
            int Hi = (int)H / 60 % 6;
            double Vmin = (100 - S) * V / 100;
            double a = (V - Vmin) * ((int)H % 60) / 60;
            double Vinc = Vmin + a;
            double Vdec = V - a;

            byte[] res = new byte[3];
            switch (Hi)
            {
                // b g r
                default:
                case 0:
                    res[0] = (byte)(Vmin * 255 / 100);
                    res[1] = (byte)(Vinc * 255 / 100);
                    res[2] = (byte)(V * 255 / 100);
                    break;
                case 1:
                    res[0] = (byte)(Vmin * 255 / 100);
                    res[1] = (byte)(V * 255 / 100);
                    res[2] = (byte)(Vdec * 255 / 100);
                    break;
                case 2:
                    res[0] = (byte)(Vinc * 255 / 100);
                    res[1] = (byte)(V * 255 / 100);
                    res[2] = (byte)(Vmin * 255 / 100);
                    break;
                case 3:
                    res[0] = (byte)(V * 255 / 100);
                    res[1] = (byte)(Vdec * 255 / 100);
                    res[2] = (byte)(Vmin * 255 / 100);
                    break;
                case 4:
                    res[0] = (byte)(V * 255 / 100);
                    res[1] = (byte)(Vmin * 255 / 100);
                    res[2] = (byte)(Vinc * 255 / 100);
                    break;
                case 5:
                    res[0] = (byte)(Vdec * 255 / 100);
                    res[1] = (byte)(Vmin * 255 / 100);
                    res[2] = (byte)(V * 255 / 100);
                    break;

            }
            return res;

        }

        private void trackBarHUE_ValueChanged(object sender, EventArgs e)
        {
            if (!activate_flag)
                return;
            double scale_hue = (double)trackBarHUE.Value / avg_h;
            double scale_sat = (double)trackBarSat.Value / avg_s;
            double scale_val = (double)trackBarBr.Value / avg_v;

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
            bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
            bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            System.Runtime.InteropServices.Marshal.Copy(ptr, disp_rgbValues, 0, bytes);



            for (int counter = 0; counter < rgbValues.Length; counter += 3)
            {
                // rgbValues[counter] = 255; // blue
                // rgbValues[counter+1] = 255; // green
                // rgbValues[counter+2] = 255; // red
                if ((int)hsvValues[counter] * scale_hue > 255)
                    disp_hsvValues[counter] = (byte)((int)(((int)hsvValues[counter]) * scale_hue) % 255);
                else
                    disp_hsvValues[counter] = (byte)(((int)hsvValues[counter]) * scale_hue);

                if ((int)hsvValues[counter + 1] * scale_sat > 255)
                    disp_hsvValues[counter + 1] = (byte)255;
                else
                    disp_hsvValues[counter + 1] = (byte)(((int)hsvValues[counter + 1]) * scale_sat);

                if ((int)hsvValues[counter + 2] * scale_val > 255)
                    disp_hsvValues[counter + 2] = (byte)255;
                else
                    disp_hsvValues[counter + 2] = (byte)(((int)hsvValues[counter + 2]) * scale_val);

                byte[] rgbpixel = GetRGB(disp_hsvValues[counter], disp_hsvValues[counter + 1], disp_hsvValues[counter + 2]);


                for (int i = 0; i < 3; ++i)
                    disp_rgbValues[counter + i] = rgbpixel[i];
            }
            System.Runtime.InteropServices.Marshal.Copy(disp_rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);

            Refresh();

        }

public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
        share improve this answer*/