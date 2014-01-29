using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Bitmap img,imgTrim,imgTemp;
        int x, ix, y, iy, w, h;
        String fn;
        double p;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_gb2Sizechange(object sender, EventArgs e)
        {
            groupBox2.Size = new System.Drawing.Size(panel1.Width / 2, groupBox2.Size.Height);
            groupBox2.Refresh();
        }

        private void Form1_gb3Sizechange(object sender, EventArgs e)
        {
            groupBox3.Location = new System.Drawing.Point(panel1.Width / 2, 32);
            groupBox3.Size = new System.Drawing.Size(panel1.Width / 2, groupBox3.Size.Height);
            groupBox3.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            DialogResult result = openDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                fn = openDialog.SafeFileName;
                img = new Bitmap(openDialog.FileName);
                pictureBox1.Image = img;
                textBox5.Text = openDialog.FileName;
            }
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            if (img == null) return;
            x = int.Parse(textBox1.Text);
            y = int.Parse(textBox2.Text);
            w = int.Parse(textBox3.Text);
            h = int.Parse(textBox4.Text);
            if (w <= 0 || h <= 0) return;
            imgTrim = new Bitmap(w, h);
            imgTrim.SetResolution(img.HorizontalResolution, img.VerticalResolution);
            Graphics gd = Graphics.FromImage(imgTrim);
            Rectangle rc = new Rectangle(x, y, w, h);
            gd.DrawImage(img, 0, 0, rc, GraphicsUnit.Pixel);//画像を切り取り

            imgTemp = (Bitmap)img.Clone();
            Graphics g = Graphics.FromImage(imgTemp);
            rc = new Rectangle(x, y, w, h);
            g.DrawRectangle(Pens.Red, rc);

            pictureBox1.Image = imgTemp;
            pictureBox2.Image = imgTrim;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (img == null) return;
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.FileName = fn;
            DialogResult result = saveDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                imgTrim.Save(saveDialog.FileName);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (img == null) return;

            double pAs = (double)pictureBox1.Height / (double)pictureBox1.Width;
            double iAs = (double)img.Height / (double)img.Width;
            x = e.X;
            y = e.Y;
            if (pAs < iAs)
            {
                p = ((double)pictureBox1.Height / (double)img.Height);
                ix = (int)((x - (int)(((double)pictureBox1.Width - (double)img.Width * p) / 2.0)) * (1.0 / p));
                iy = (int)(y * (1 / p));
            }
            else
            {
                p = ((double)pictureBox1.Width / (double)img.Width);
                ix = (int)(x * (1 / p));
                iy = (int)((y - (int)(((double)pictureBox1.Height - (double)img.Height * p) / 2.0)) * (1.0 / p));
                //ピクセルを元の画像にもどさないといけない
            }
            textBox1.Text = ix.ToString();
            textBox2.Text = iy.ToString();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (img == null) return;
            w = (int)((e.X - x) * (1.0 / p));
            h = (int)((e.Y - y) * (1.0 / p));
            imgTemp = (Bitmap)img.Clone();
            Graphics g = Graphics.FromImage(imgTemp);
            Rectangle rc = new Rectangle(ix, iy, w, h);
            g.DrawRectangle(Pens.Red, rc);
            pictureBox1.Image = imgTemp;
            textBox3.Text = w.ToString();
            textBox4.Text = h.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (img == null) return;
            button5_Click(sender, e);
            int nr = int.Parse(comboBox1.Text);
            for (int i = 0; i < imgTrim.Width; i++)
            for (int j = 0; j < imgTrim.Height; j++)
            {
                int nn= 0;//近傍数
                int sum = 0;//平均値を求める合計
                for (int ii = i - nr; ii <= i + nr; ii++)
                {
                    if (ii < 0) continue;//はみ出し処理
                    if (ii >= imgTrim.Width) continue;//
                    for (int jj = j - nr; jj <= j + nr; jj++)
                    {
                        if (jj < 0) continue;//はみ出し処理
                        if (jj >= imgTrim.Height) continue;//
                        sum += (int)imgTrim.GetPixel(ii, jj).R;
                        nn++;
                    }
                }
                int ave= sum / nn;//平均値を求める
                Color col= Color.FromArgb(ave,ave,ave);//Colorに変換
                imgTrim.SetPixel(i, j, col);
            }
            this.pictureBox2.Image = imgTrim;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (img == null) return;
            int r, g, b, luminance;

            if (imgTrim == null)
            {
                MessageBox.Show("画像を切り取ってから行って下さい。", "使用上の注意", MessageBoxButtons.OK);
                return;
            }

            for (int i = 0; i < imgTrim.Width; i++)
                for (int j = 0; j < imgTrim.Height; j++)
                {
                    r = (int)imgTrim.GetPixel(i, j).R;
                    g = (int)imgTrim.GetPixel(i, j).G;
                    b = (int)imgTrim.GetPixel(i, j).B;
                    luminance = (int)(0.298912 * (double)r + 0.586611 * (double)g + 0.114478 * (double)b);
                    Color col = Color.FromArgb(luminance, luminance, luminance);
                    imgTrim.SetPixel(i, j, col);
                }
            this.pictureBox2.Image = imgTrim;
        }
    }
}
