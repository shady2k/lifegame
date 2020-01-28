using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lifegame
{
    public partial class Form1 : Form
    {
        bool[,] world;

        public Form1()
        {
            InitializeComponent();
        }
        private void GenerateWorld()
        {
            GenerateWorld(Int32.Parse(udHeight.Value.ToString()), Int32.Parse(udWidth.Value.ToString()));
        }
        private void GenerateWorld(int worldHeight, int worldWidth)
        {
            world = new bool[worldWidth, worldHeight];

            int xStep = (pictureBox1.Width - 1) / worldWidth;
            int yStep = (pictureBox1.Height - 1) / worldHeight;

            int frameHeight = yStep * worldHeight;
            int frameWidth = xStep * worldWidth;

            Bitmap img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            using (Graphics graphics = Graphics.FromImage(img))
            {
                graphics.FillRectangle(Brushes.White, 0, 0, frameWidth, frameHeight);

                for (int i = 0; i <= worldWidth; i++)
                {
                    graphics.DrawLine(Pens.Black, i * xStep, 0, i * xStep, frameHeight);
                }

                for (int i = 0; i <= worldHeight; i++)
                {
                    graphics.DrawLine(Pens.Black, 0, i * yStep, frameWidth, i * yStep);
                }
            }
            pictureBox1.Image = img;
        }
        private void UpdateWorld()
        {
            int worldHeight = world.GetLength(1);
            int worldWidth = world.GetLength(0);

            int xStep = (pictureBox1.Width - 1) / worldWidth;
            int yStep = (pictureBox1.Height - 1) / worldHeight;

            int frameHeight = yStep * worldHeight;
            int frameWidth = xStep * worldWidth;

            int rectWidth = (frameWidth / worldWidth) - 3;
            int rectHeight = (frameHeight / worldHeight) - 3;

            Image img = pictureBox1.Image;
            using (Graphics graph = Graphics.FromImage(img))
            {
                for (int i = 0; i < worldWidth; i++)
                {
                    for (int j = 0; j < worldHeight; j++)
                    {
                        if (world[i, j])
                        {
                            graph.FillRectangle(Brushes.Black, i * xStep + 2, j * yStep + 2, rectWidth, rectHeight);
                        }
                        else
                        {
                            graph.FillRectangle(Brushes.White, i * xStep + 1, j * yStep + 1, rectWidth, rectHeight);
                        }
                    }
                }
            }
            pictureBox1.Image = img;
        }
        private void GenerateRandomPopulation()
        {
            Random r = new Random();
            int x = world.GetLength(0);
            int y = world.GetLength(1);

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    world[i, j] = (r.NextDouble() >= 0.5);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            GenerateRandomPopulation();
            UpdateWorld();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GenerateWorld();
        }

        private void udHeight_ValueChanged(object sender, EventArgs e)
        {
            GenerateWorld();
        }

        private void udWidth_ValueChanged(object sender, EventArgs e)
        {
            GenerateWorld();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            GenerateWorld();
        }
    }
}
