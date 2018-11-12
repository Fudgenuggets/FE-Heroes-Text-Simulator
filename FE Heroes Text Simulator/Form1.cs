﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace FE_Heroes_Text_Simulator
{
    public partial class Form1 : Form
    {
        public Bitmap backgroundImage;
        public Bitmap conversationBackground;
        public Bitmap resizedBackground;
        public Bitmap unitImage;
        public Bitmap resizedUnit;


        Lists BGList = new Lists();

        public Form1()
        {
            InitializeComponent();

            List<string> backgrounds = BGList.GetBackgrounds();

            pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);

            comboBox_BG.Items.Add("");      //populates the combo box for backgrounds
            foreach (string entry in backgrounds)
            {
                comboBox_BG.Items.Add(entry);
            }

            backgrounds = BGList.GetConversationBackgrounds();
            comboBox_BG2.Items.Add("");
            foreach (string entry in backgrounds)
            {
                comboBox_BG2.Items.Add(entry);
            }

            refresh_Picturebox();
        }

        private void button_ImportUnit_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog     //prompts user to open file
            {
                Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                unitImage = new Bitmap(ofd.FileName);
                resizedUnit = resizeImage(unitImage, 1325, 1590);
            }

            refresh_Picturebox();
        }

        private void button_ImportBG_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                comboBox_BG.SelectedIndex = 0;
                backgroundImage = new Bitmap(ofd.FileName);
                resizedBackground = resizeImage(backgroundImage, (int)(backgroundImage.Width * (1280f / backgroundImage.Height)), 1280);
            }

            refresh_Picturebox();
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PNG(*.PNG)|*.png"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                pictureBox.Image.Save(sfd.FileName, ImageFormat.Png);
            }
        }

        private void refresh_Picturebox()
        {
            Bitmap bmp = new Bitmap(pictureBox.Image);
            Graphics g = Graphics.FromImage(bmp);
            StringFormat stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.Clear(Color.Transparent);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            if (backgroundImage != null)    //if comboBox for background isn't null
            {
                if (checkBox_ResizeBG.Checked && checkBox_Darken.Checked)      //draws resized image darkened
                {
                    g.DrawImage(SetLightness(resizedBackground, 0.5f), -(resizedBackground.Width - pictureBox.Width) / 2f + (float)numericUpDown_BGX.Value, (float)numericUpDown_BGY.Value, resizedBackground.Width, resizedBackground.Height);
                }
                else if (checkBox_Darken.Checked)
                {
                    g.DrawImage(SetLightness(backgroundImage, 0.5f), -(backgroundImage.Width - pictureBox.Width) / 2f + (float)numericUpDown_BGX.Value, (float)numericUpDown_BGY.Value, backgroundImage.Width, backgroundImage.Height);
                }
                else if (checkBox_ResizeBG.Checked)
                {
                    g.DrawImage(resizedBackground, -(resizedBackground.Width - pictureBox.Width) / 2f + (float)numericUpDown_BGX.Value, (float)numericUpDown_BGY.Value, resizedBackground.Width, resizedBackground.Height);
                }
                else    //draws image without resizing
                {
                    g.DrawImage(backgroundImage, -(backgroundImage.Width - pictureBox.Width) / 2f + (float)numericUpDown_BGX.Value, (float)numericUpDown_BGY.Value, backgroundImage.Width, backgroundImage.Height);
                }
            }

            if (conversationBackground != null)
            {
                g.DrawImage(conversationBackground, -(conversationBackground.Width - pictureBox.Width) / 2f, 157, conversationBackground.Width, conversationBackground.Height);

            }

            if (unitImage != null)
            {
                if (checkBox_ResizeUnit.Checked)
                {
                    g.DrawImage(resizedUnit, -(resizedUnit.Width - pictureBox.Width) / 2f + (float)numericUpDown_UnitX.Value, (float)numericUpDown_UnitY.Value, resizedUnit.Width, resizedUnit.Height);
                }
                else
                {
                    g.DrawImage(unitImage, -(unitImage.Width - pictureBox.Width) / 2f + (float)numericUpDown_UnitX.Value, (float)numericUpDown_UnitY.Value, unitImage.Width, unitImage.Height);
                }
            }

            g.DrawImage(Properties.Resources.Text_box, 0, 890);
            g.DrawImage(Properties.Resources.Name_box, 16, 766);

            GraphicsPath path = new GraphicsPath();     //draw text lines
            path.AddString(richTextBox_Line1.Text, new FontFamily("nintendoP_Skip-D_003"), (int)FontStyle.Regular, 31, new Point(34, 981), StringFormat.GenericDefault);
            path.AddString(richTextBox_Line2.Text, new FontFamily("nintendoP_Skip-D_003"), (int)FontStyle.Regular, 31, new Point(34, 1034), StringFormat.GenericDefault);
            path.AddString(richTextBox_Line3.Text, new FontFamily("nintendoP_Skip-D_003"), (int)FontStyle.Regular, 31, new Point(34, 1087), StringFormat.GenericDefault);

            GraphicsPath path2 = new GraphicsPath();    //draw name
            path2.AddString(richTextBox_Name.Text, new FontFamily("nintendoP_Skip-D_003"), (int)FontStyle.Regular, 34, new Point(206, 868), stringFormat);

            Pen pen = new Pen(Color.FromArgb(15, 30, 40), 5);
            Pen pen2 = new Pen(Color.FromArgb(50, 30, 10), 5);
            pen.LineJoin = LineJoin.Round;
            pen2.LineJoin = LineJoin.Round;
            g.DrawPath(pen, path);
            g.DrawPath(pen2, path2);
            SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 255));
            g.FillPath(brush, path);
            g.FillPath(brush, path2);

            pictureBox.Image = bmp;
        }

        private Bitmap resizeImage(Bitmap image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void richTextBox_Name_TextChanged(object sender, EventArgs e)
        {
            refresh_Picturebox();
        }

        private void richTextBox_Line1_TextChanged(object sender, EventArgs e)
        {
            refresh_Picturebox();
        }

        private void richTextBox_Line2_TextChanged(object sender, EventArgs e)
        {
            refresh_Picturebox();
        }

        private void richTextBox_Line3_TextChanged(object sender, EventArgs e)
        {
            refresh_Picturebox();
        }

        private void numericUpDown_UnitX_ValueChanged(object sender, EventArgs e)
        {
            refresh_Picturebox();
        }

        private void numericUpDown_UnitY_ValueChanged(object sender, EventArgs e)
        {
            refresh_Picturebox();
        }

        private void comboBox_BG_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox_BG.Text))
            {
                string resourceName = comboBox_BG.Text;
                if (resourceName.Contains(" "))
                    resourceName = resourceName.Replace(" ", "_");
                if (resourceName.Contains("("))
                    resourceName = resourceName.Replace("(", "_");
                if (resourceName.Contains(")"))
                    resourceName = resourceName.Replace(")", "_");
                if (resourceName.Contains("."))
                    resourceName = resourceName.Replace(".", "_");
                backgroundImage = (Bitmap)Properties.Resources.ResourceManager.GetObject(resourceName);
                resizedBackground = resizeImage(backgroundImage, (int)(backgroundImage.Width * (1280f / backgroundImage.Height)), 1280);
            }
            else
                backgroundImage = null;
            refresh_Picturebox();
        }

        private void numericUpDown_BGX_ValueChanged(object sender, EventArgs e)
        {
            refresh_Picturebox();
        }

        private void numericUpDown_BGY_ValueChanged(object sender, EventArgs e)
        {
            refresh_Picturebox();
        }

        private void button_ClearUnit_Click(object sender, EventArgs e)
        {
            unitImage = null;
            refresh_Picturebox();
        }

        private void checkBox_ResizeUnit_CheckedChanged(object sender, EventArgs e)
        {
            refresh_Picturebox();
        }

        private void checkBox_ResizeBG_CheckedChanged(object sender, EventArgs e)
        {
            refresh_Picturebox();
        }

        private void checkBox_Darken_CheckedChanged(object sender, EventArgs e)
        {
            refresh_Picturebox();
        }

        public Bitmap SetLightness(Bitmap bmp, float lightness)
        {
            Bitmap tempBmp = bmp;

            Bitmap newBmp = new Bitmap(tempBmp.Width, tempBmp.Height);

            Graphics g = Graphics.FromImage(newBmp);

            float[][] floatColorMatrix = {
                new float[] {lightness, 0, 0, 0, 50},
                new float[] {0, lightness, 0, 0, 50},
                new float[] {0, 0, lightness, 0, 50},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            };

            ColorMatrix newColorMatrix = new ColorMatrix(floatColorMatrix);
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(newColorMatrix);

            g.DrawImage(tempBmp, new Rectangle(0, 0, tempBmp.Width, tempBmp.Height), 0, 0, tempBmp.Width, tempBmp.Height, GraphicsUnit.Pixel, attributes);

            return newBmp;
        }

        private void comboBox_BG2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox_BG2.Text))
            {
                string resourceName = comboBox_BG2.Text;
                if (resourceName.Contains(" "))
                    resourceName = resourceName.Replace(" ", "_");
                if (resourceName.Contains("("))
                    resourceName = resourceName.Replace("(", "_");
                if (resourceName.Contains(")"))
                    resourceName = resourceName.Replace(")", "_");
                if (resourceName.Contains("."))
                    resourceName = resourceName.Replace(".", "_");
                conversationBackground = (Bitmap)Properties.Resources.ResourceManager.GetObject(resourceName);
                conversationBackground = resizeImage(conversationBackground, 1066, 453);
            }
            else
                conversationBackground = null;
            refresh_Picturebox();
        }
    }
}
