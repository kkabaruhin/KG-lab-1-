using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab5_l_system_
{
    public partial class Form1 : Form
    {
        struct State
        {
            public double x, y, angle;
        }

        Point start_point = new Point();
        string fileText = "", drawing_string, axiom;
        char draw_symbol;
        double angle_change, start_angle = 0, length_line; //направление в нулевой момент времени указывает слева направо. Величина 
        Dictionary<char, string> rules = new Dictionary<char, string>();
        Stack<State> States = new Stack<State>();

        public Form1()
        {
            InitializeComponent();

            textBox1.Text = "";
            textBox2.Text = "";
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            pictureBox1.BackColor = Color.White;
        }

        private void initializeRules()
        {
            rules.Clear();
            rules['-'] = "-";
            rules['+'] = "+";
            rules['['] = "[";
            rules[']'] = "]";
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            clear();

            start_point = new Point(e.X, e.Y);

            ((Bitmap)pictureBox1.Image).SetPixel(e.X, e.Y, Color.Black);

            pictureBox1.Image = pictureBox1.Image;

            label2.Text = "Стартовая точка выбрана";
        }

        private void clear()
        {
            var g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Image = pictureBox1.Image;
        }

        //clear
        private void button3_Click(object sender, EventArgs e)
        {
            clear();
        }

        //Во входном файле сначала аксиома, потом символ, который рисует, угол поворота, начальный угол
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку
            fileText = System.IO.File.ReadAllText(filename);

            string[] parse_text = fileText.Split('\n', ' ');

            axiom = parse_text[0];
            draw_symbol = parse_text[1][0];

            angle_change = Convert.ToDouble(parse_text[2]);

            start_angle = Convert.ToDouble(parse_text[3]);

            initializeRules();

            for (int i = 4; i < parse_text.Length; i++)
            {
                char key = parse_text[i][0];
                string value = parse_text[i].Substring(3);

                if (!rules.ContainsKey(key))
                    rules.Add(key, value);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (fileText == "" || textBox1.Text == "" || textBox2.Text == "")
                return;

            length_line = Convert.ToDouble(textBox2.Text);

            drawing_string = axiom;
            string new_drawing_string = "";

            for (int i = 0; i < Convert.ToInt32(textBox1.Text); i++)
            {
                new_drawing_string = "";
                foreach (char ch in drawing_string)
                {
                    if (rules.ContainsKey(ch))
                        new_drawing_string += rules[ch];
                    
                }
                drawing_string = new_drawing_string;
            }

            Pen pen = new Pen(Color.Black, 3);
            var g = Graphics.FromImage(pictureBox1.Image);

            State current_state;

            current_state.x = Convert.ToDouble(start_point.X);
            current_state.y = Convert.ToDouble(start_point.Y);
            current_state.angle = start_angle;

            foreach (char ch in drawing_string)
            {
                switch (ch)
                {
                    case '+':
                        current_state.angle = (current_state.angle + angle_change) % 360 ;
                        break;
                    case '-':
                        current_state.angle = (current_state.angle - angle_change) % 360;
                        break;
                    case '[':
                        States.Push(current_state);
                        break;
                    case ']':
                        current_state = States.Pop();
                        break;
                    default:
                        break;
                }
                if (ch == draw_symbol)
                {
                    g.DrawLine(pen, 
                        Convert.ToInt32(current_state.x), 
                        Convert.ToInt32(current_state.y),
                        Convert.ToInt32(current_state.x + length_line * Math.Cos(current_state.angle*Math.PI/180)),
                        Convert.ToInt32(current_state.y + length_line * Math.Sin(current_state.angle*Math.PI/180)));

                    current_state.x = current_state.x + length_line * Math.Cos(current_state.angle * Math.PI / 180);
                    current_state.y = current_state.y + length_line * Math.Sin(current_state.angle * Math.PI / 180);
                }
            }

            pen.Dispose();
            g.Dispose();
            pictureBox1.Image = pictureBox1.Image;
        }
    }
}
