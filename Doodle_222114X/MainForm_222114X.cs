using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Net.WebRequestMethods;
//using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Reflection;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;
using System.Security.Cryptography;


namespace Doodle_222114X
{
    public partial class MainForm_222114X : Form
    {
        //-------------------------INIT Variables-----------------------------
        float[] dashValues = { 2, 1 };
        Stack<Point> pixels = new Stack<Point>();
        Bitmap bm; Graphics g; Pen pen = new Pen(Color.Black, 5);
        SolidBrush brush = new SolidBrush(Color.Black), shadow = new SolidBrush(Color.DarkGray);
        Point startP = new Point(0, 0), endP = new Point(0, 0), TruestartP = new Point(0, 0), TrueendP = new Point(0, 0);
        Point c1P = new Point(0, 0), c2P = new Point(0, 0);
        string strText, Temp, Pather;
        int Xcounter, Ycounter;
        int R = 0, G = 0, B = 0, A = 0, startAngle, sweepAngle;
        bool flagDraw = false, flagErase = false, flagText = false, Liner = false, Circular = false, Rectanglular = false, Arcs = false, Transparencys = false, Bezier = false, Eyedroppers = false, IsSelecting = false, Blending = false, Filler = false;

        private void FontDialog_Click(object sender, EventArgs e)
        {
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            FontDialog fontDlg = new FontDialog();
            fontDlg.ShowColor = true;
            fontDlg.ShowApply = true;
            fontDlg.ShowEffects = true;
            fontDlg.MaxSize = 15;
            fontDlg.MinSize = 3;

            if (fontDlg.ShowDialog() != DialogResult.Cancel)
            {
                txtbox.Font = fontDlg.Font;
            }
        }

        private void Brushes_Click(object sender, EventArgs e)
        {
            Brushes.Size = new Size(0, 0);
            Shapes.Size = new Size(0, 0);
            Rotations.Size = new Size(0, 0);
            Tools.Size = new Size(0, 0);

            Font.Show();
            Trans.Location = new Point(17, 60);
            txtbox.Location = new Point(17, 105);
            Text.Location = new Point(17, 115);
            BrushIcon.Location = new Point(17, 165);
        }
        private void Shapes_Click(object sender, EventArgs e)
        {
            Brushes.Size = new Size(0, 0);
            Shapes.Size = new Size(0, 0);
            Rotations.Size = new Size(0, 0);
            Tools.Size = new Size(0, 0);

            Color1.Show();
            Color2.Show();
            Dotted.Show();
            Gradient.Show();
            Line.Location = new Point(17, 60);
            Circle.Location = new Point(17, 110);
            Rectangle.Location = new Point(17, 160);
            Arc.Location = new Point(17, 210);
        }

        private void Rotations_Click(object sender, EventArgs e)
        {
            Brushes.Size = new Size(0, 0);
            Shapes.Size = new Size(0, 0);
            Rotations.Size = new Size(0, 0);
            Tools.Size = new Size(0, 0);

            rotateDegree.Location = new Point(17, 60);
            Rotate.Location = new Point(17, 80);
            flipV.Location = new Point(17, 130);
            FlipH.Location = new Point(17, 180);
            L2R.Location = new Point(8, 230);
            Symmetry.Location = new Point(17, 245);
        }

        private void Tools_Click(object sender, EventArgs e)
        {
            Brushes.Size = new Size(0, 0);
            Shapes.Size = new Size(0, 0);
            Rotations.Size = new Size(0, 0);
            Tools.Size = new Size(0, 0);

            Blend.Location = new Point(17, 60);
            Fill.Location = new Point(17, 110);
            Crop.Location = new Point(17, 160);
            GreyS.Location = new Point(17, 210);
        }

        private void Back_Click(object sender, EventArgs e)
        {
            Brushes.Size = new Size(56, 20);
            Shapes.Size = new Size(56, 20);
            Rotations.Size = new Size(56, 20);
            Tools.Size = new Size(56, 20);

            Color1.Hide();
            Color2.Hide();
            Font.Hide();
            Dotted.Hide();
            Gradient.Hide();

            //Brushes
            Trans.Location = new Point(17, 500);
            txtbox.Location = new Point(17, 500);
            Text.Location = new Point(17, 500);
            BrushIcon.Location = new Point(17, 500);

            //Shapes
            Line.Location = new Point(17, 500);
            Circle.Location = new Point(17, 500);
            Rectangle.Location = new Point(17, 500);
            Arc.Location = new Point(17, 500);

            //Rotate
            rotateDegree.Location = new Point(17, 500);
            Rotate.Location = new Point(17, 500);
            flipV.Location = new Point(17, 500);
            FlipH.Location = new Point(17, 500);
            L2R.Location = new Point(8, 500);
            Symmetry.Location = new Point(17, 500);

            //Tools
            Blend.Location = new Point(17, 500);
            Fill.Location = new Point(17, 500);
            Crop.Location = new Point(17, 500);
            GreyS.Location = new Point(17, 500);

        }

        private void Controls_Click(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            Tag = pb.Tag.ToString();

            g = Graphics.FromImage(bm);
            if (Tag == "Crop")
            {
                //Crop
                picBoxBrushColor.Image = Properties.Resources.crop;
                IsSelecting = true;
                Filler = false; Transparencys = false; Circular = false; Liner = false; Arcs = false; Rectanglular = false; Bezier = false; Eyedroppers = false; Blending = false;
            }
            else if (Tag == "GreyS")
            {
                Color C;
                int GA;
                for (int i = 0; i < bm.Width; i++)
                {
                    for (int j = 0; j < bm.Height; j++)
                    {
                        C = bm.GetPixel(i, j);
                        R = C.R;
                        G = C.G;
                        B = C.B;
                        GA = (R + G + B) / 3;
                        bm.SetPixel(i, j, Color.FromArgb(GA, GA, GA));
                    }
                }
                picBoxMain.Refresh();
            }
            else if (Tag == "BrushesIcons")
            {
                picBoxBrushColor.Image = Properties.Resources.brush;
                flagText = false; Filler = false; Blending = false; Circular = false; Liner = false; Arcs = false; Rectanglular = false; Bezier = false; IsSelecting = false; Eyedroppers = false; Transparencys = false;
            }
            else if (Tag == "Loadss")
            {
                //OPENFILEDIALOG cldnt find any pngs only empty folders
                picBoxBrushColor.Image = Properties.Resources.loadfile;

                if (BMPPath.Visible == false)
                {
                    BMPPath.Show();
                    Loads.Show();
                }
                else
                {
                    BMPPath.Hide();
                    Loads.Hide();
                }
            }
            else if (Tag == "flipV")
            {
                picBoxBrushColor.Image = Properties.Resources.FlipV;
                bm.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }
            else if (Tag == "FlipH")
            {
                picBoxBrushColor.Image = Properties.Resources.flipH;
                bm.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            else if (Tag == "Symmetry")
            {
                picBoxBrushColor.Image = Properties.Resources.Symmetry;
                if (L2R.Text == "left -> right")
                {
                    for (int Xcount = 0; Xcount < bm.Width / 2; Xcount++)
                    {
                        for (int Ycount = 0; Ycount < bm.Height; Ycount++)
                        {
                            bm.SetPixel(bm.Width - 1 - Xcount, Ycount, bm.GetPixel(Xcount, Ycount));
                        }
                    }
                    L2R.Text = "right -> left";
                }
                else
                {
                    for (int Xcount = 0; Xcount < bm.Width / 2; Xcount++)
                    {
                        for (int Ycount = 0; Ycount < bm.Height; Ycount++)
                        {
                            bm.SetPixel(Xcount, Ycount, bm.GetPixel(bm.Width - 1 - Xcount, Ycount));
                        }
                    }
                    L2R.Text = "left -> right";
                }
            }
            else if (Tag == "Rotate")
            {
                Bitmap rotatedImage = bm;
                rotatedImage.SetResolution(bm.HorizontalResolution, bm.VerticalResolution);

                using (Graphics g = Graphics.FromImage(rotatedImage))
                {
                    // Set the rotation point to the center in the matrix
                    g.TranslateTransform(bm.Width / 2, bm.Height / 2);
                    // Rotate
                    g.RotateTransform(Convert.ToInt32(Math.Round(rotateDegree.Value, 0)));
                    // Restore rotation point in the matrix
                    g.TranslateTransform(-bm.Width / 2, -bm.Height / 2);
                    // Draw the image on the bitmap
                    g.DrawImage(bm, new Point(0, 0));
                }
            }
            else if (Tag == "Trans")
            {
                //Transparency Brush
                picBoxBrushColor.Image = Properties.Resources.transparency;
                Transparencys = true;
                Filler = false; Circular = false; Liner = false; Arcs = false; Rectanglular = false; Bezier = false; IsSelecting = false; Eyedroppers = false; Blending = false;
            }
            else if (Tag == "Fill")
            {
                //FloodFill
                picBoxBrushColor.Image = Properties.Resources.Fill;
                Filler = true;
                IsSelecting = false; Transparencys = false; Circular = false; Liner = false; Arcs = false; Rectanglular = false; Bezier = false; Eyedroppers = false; Blending = false;
            }
            else if (Tag == "Save")
            {
                using (SaveFileDialog sfdlg = new SaveFileDialog())
                {
                    sfdlg.Title = "Save Dialog";
                    sfdlg.Filter = "Image Files(.PNG)|.PNG|All files (.)|.";
                    picBoxBrushColor.Image = Properties.Resources.save;
                    if (sfdlg.ShowDialog(this) == DialogResult.OK)
                    {
                        using (Bitmap bmp = new Bitmap(picBoxMain.Width, picBoxMain.Height))
                        {
                            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                            picBoxMain.DrawToBitmap(bmp, rect);
                            bmp.Save(sfdlg.FileName, ImageFormat.Png);
                            MessageBox.Show("File Saved Successfully");
                        }
                    }
                }
            }
            else if (Tag == "Clear")
            {
                Rectangle rect = picBoxMain.ClientRectangle;
                picBoxBrushColor.BackColor = black.BackColor;
                picBoxBrushColor.Image = Properties.Resources.Clear;
                g.FillRectangle(new SolidBrush(Color.WhiteSmoke), rect);
                g.Dispose(); picBoxMain.Invalidate();
                Temp = txtbox.Text; txtbox.Text = ""; flagErase = false;
                Filler = false; Blending = false; Circular = false; Liner = false; Arcs = false; Rectanglular = false; Bezier = false; IsSelecting = false; Eyedroppers = false; Transparencys = false;
                Color1.Image = Properties.Resources.RGB2;
                Color2.Image = Properties.Resources.RGB2;
            }
            else if (Tag == "Erase")
            {
                brush = new SolidBrush(picBoxMain.BackColor);
                picBoxBrushColor.Image = Properties.Resources.eraser;
                flagErase = true; txtbox.Text = ""; Temp = txtbox.Text;
                flagText = false; Filler = false; Blending = false; Circular = false; Liner = false; Arcs = false; Rectanglular = false; Bezier = false; IsSelecting = false; Eyedroppers = false; Transparencys = false;
            }
            else if (Tag == "Text")
            {
                picBoxBrushColor.Image = Properties.Resources.text;
                Filler = false; flagDraw = false; flagErase = false; flagText = true; Circular = false; Liner = false; Arcs = false; Rectanglular = false; Bezier = false; IsSelecting = false; Eyedroppers = false; Transparencys = false; Bezier = false; Blending = false;
            }
            else if (Tag == "Line")
            {
                if (Liner != true)
                {
                    Filler = false; Blending = false; Circular = false; Liner = true; Arcs = false; Rectanglular = false; Bezier = false; IsSelecting = false; Eyedroppers = false; Transparencys = false;
                    picBoxBrushColor.Image = Properties.Resources.line;
                }
                else
                {
                    Liner = false;
                    picBoxBrushColor.Image = null;
                }
            }
            else if (Tag == "Circle")
            {
                if (Circular != true)
                {
                    Filler = false; Blending = false; Circular = true; Liner = false; Arcs = false; Rectanglular = false; Bezier = false; IsSelecting = false; Eyedroppers = false; Transparencys = false;
                    picBoxBrushColor.Image = Properties.Resources.Circle;
                }
                else
                {
                    Circular = false;
                    picBoxBrushColor.Image = null;
                }
            }
            else if (Tag == "Rectangle")
            {
                if (Rectanglular != true)
                {
                    Filler = false; Blending = false; Circular = false; Liner = false; Arcs = false; Rectanglular = true; Bezier = false; IsSelecting = false; Eyedroppers = false; Transparencys = false;
                    picBoxBrushColor.Image = Properties.Resources.red_rectangle;
                }
                else
                {
                    Rectanglular = false;
                    picBoxBrushColor.Image = null;
                }
            }
            else if (Tag == "Arc")
            {
                if (Arcs != true)
                {
                    Filler = false; Blending = false; Circular = false; Liner = false; Arcs = true; Rectanglular = false; Bezier = false; IsSelecting = false; Eyedroppers = false; Transparencys = false;
                    picBoxBrushColor.Image = Properties.Resources.cardinal;
                }
                else
                {
                    Arcs = false;
                    picBoxBrushColor.Image = null;
                }
            }
            else if (Tag == "Colorwheel")
            {
                ColorDialog colorPicker = new ColorDialog();
                if (colorPicker.ShowDialog() == DialogResult.OK)
                {
                    Colorwheel.BackColor = colorPicker.Color;
                    pen.Color = Colorwheel.BackColor;
                    picBoxBrushColor.BackColor = pen.Color;
                    pink.BackColor = purple.BackColor;
                    purple.BackColor = cyan.BackColor;
                    cyan.BackColor = orange.BackColor;
                    orange.BackColor = black.BackColor;
                    black.BackColor = yellow.BackColor;
                    yellow.BackColor = blue.BackColor;
                    blue.BackColor = green.BackColor;
                    green.BackColor = red.BackColor;
                    red.BackColor = picBoxBrushColor.BackColor;
                }
            }
            else if (Tag == "Color1")
            {
                ColorDialog colorPicker = new ColorDialog();
                if (colorPicker.ShowDialog() == DialogResult.OK)
                {
                    Color1.BackColor = colorPicker.Color;
                    Color1.Image = null;
                }
            }
            else if (Tag == "Color2")
            {
                ColorDialog colorPicker = new ColorDialog();
                if (colorPicker.ShowDialog() == DialogResult.OK)
                {
                    Color2.BackColor = colorPicker.Color;
                    Color2.Image = null;
                }
            }
            else if (Tag == "Eye")
            {
                Eyedroppers = true;
            }
            else if (Tag == "Blend")
            {
                Blending = true;
                picBoxBrushColor.Image = Properties.Resources.blending;
            }
        }

        private void picBoxMain_MouseUp(object sender, MouseEventArgs e)
        {
            TrueendP = e.Location;
            flagDraw = false;
            g = Graphics.FromImage(bm);
            if (IsSelecting == true)
            {

                g = Graphics.FromImage(bm);
                //set background button highlight
                Width = TruestartP.X - TrueendP.X;
                Height = TruestartP.Y - TrueendP.Y;
                if (Width <= 0)
                {
                    Width *= -1;
                }
                if (Height <= 0)
                {
                    Height *= -1;
                }
                Rectangle sourceRectangle = new Rectangle(TruestartP.X, TruestartP.Y, Width, Height);
                // Compressed hand
                Rectangle destRectangle = new Rectangle(TruestartP.X, TruestartP.Y, Width * 6 / 5, Height * 6 / 5);

                // Draw the resize image
                g.DrawImage(bm, destRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }
            if (Liner == true)
            {
                flagDraw = false;
                Pen blackPen = new Pen(Color.Black, Convert.ToInt32(Math.Round(BrushSize.Value, 0)));
                if (Dotted.Checked == true)
                {
                    blackPen.DashPattern = dashValues;
                }
                if (Gradient.Checked == true)
                {
                    LinearGradientBrush linGrBrush = new LinearGradientBrush(
                    TruestartP, TrueendP, Color1.BackColor, Color2.BackColor);
                    Pen pen = new Pen(linGrBrush, Convert.ToInt32(Math.Round(BrushSize.Value, 0)));
                    linGrBrush.GammaCorrection = true;
                    g.DrawLine(pen, TruestartP.X, TruestartP.Y, TrueendP.X, TrueendP.Y);
                    g.Dispose(); //Prevent out of memory
                    picBoxMain.Invalidate();
                }
                else
                {
                    g.DrawLine(blackPen, TruestartP.X, TruestartP.Y, TrueendP.X, TrueendP.Y);
                    g.Dispose(); //Prevent out of memory
                    picBoxMain.Invalidate();
                }
            }
            if (Circular == true)
            {
                flagDraw = false;
                Pen blackPen = new Pen(Color.Black, Convert.ToInt32(Math.Round(BrushSize.Value, 0)));

                // Create start and sweep angles on ellipse.
                int startAngle = 90;
                int sweepAngle = 360;
                int x, y;

                // Draw arc to screen.
                x = TrueendP.X - TruestartP.X;
                y = TrueendP.Y - TruestartP.Y;
                if (x <= 0)
                {
                    TruestartP.X -= (TruestartP.X - TrueendP.X); //Needs to be double negative cos they dont accept negative numbers
                    x *= -1;
                }
                if (y <= 0)
                {
                    TruestartP.Y -= (TruestartP.Y - TrueendP.Y); //Needs to be double negative cos they dont accept negative numbers
                    y *= -1;
                }
                if (Dotted.Checked == true)
                {
                    blackPen.DashPattern = dashValues;
                }
                if (Gradient.Checked == true)
                {
                    LinearGradientBrush linGrBrush = new LinearGradientBrush(
                    TruestartP, TrueendP, Color1.BackColor, Color2.BackColor);
                    Pen pen = new Pen(linGrBrush, Convert.ToInt32(Math.Round(BrushSize.Value, 0)));
                    linGrBrush.GammaCorrection = true;
                    g.FillEllipse(linGrBrush, TruestartP.X, TruestartP.Y, x, y);
                    g.Dispose(); //Prevent out of memory
                    picBoxMain.Invalidate();
                }
                else
                {
                    g.DrawArc(blackPen, TruestartP.X, TruestartP.Y, x, y, startAngle, sweepAngle);
                    g.Dispose(); //Prevent out of memory
                    picBoxMain.Invalidate();
                }
            }
            if (Rectanglular == true)
            {
                Pen blackPen = new Pen(Color.Black, Convert.ToInt32(Math.Round(BrushSize.Value, 0)));
                int x, y;
                // Draw arc to screen.
                x = TrueendP.X - TruestartP.X;
                y = TrueendP.Y - TruestartP.Y;
                if (x <= 0)
                {
                    TruestartP.X -= (TruestartP.X - TrueendP.X); //Only x needs to be inversed since Y is counting from 0 from the top like a maniac
                    x *= -1;
                }
                if (Dotted.Checked == true)
                {
                    blackPen.DashPattern = dashValues;
                }
                if (Gradient.Checked == true)
                {
                    LinearGradientBrush linGrBrush = new LinearGradientBrush(
                    TruestartP, TrueendP, Color1.BackColor, Color2.BackColor);
                    Pen pen = new Pen(linGrBrush);
                    linGrBrush.GammaCorrection = true;
                    g.FillRectangle(linGrBrush, TruestartP.X, TruestartP.Y, x, y);
                    g.Dispose(); //Prevent out of memory
                    picBoxMain.Invalidate();
                }
                else
                {
                    g.DrawRectangle(blackPen, TruestartP.X, TruestartP.Y, x, y);
                    g.Dispose(); //Prevent out of memory
                    picBoxMain.Invalidate();
                }
            }
            if (Arcs == true)
            {
                Pen blackPen = new Pen(Color.Black, Convert.ToInt32(Math.Round(BrushSize.Value, 0)));
                int x, y;
                x = (TrueendP.X - TruestartP.X) * 2;
                y = (TrueendP.Y - TruestartP.Y) * 2;
                Console.WriteLine(x.ToString());
                Console.WriteLine(y.ToString());
                if (x >= 0 && y >= 0)
                {
                    TruestartP.X += (TruestartP.X - TrueendP.X);
                    TruestartP.Y += (TrueendP.Y - TruestartP.Y) / 2;
                    TruestartP.Y += (TruestartP.Y - TrueendP.Y);
                    startAngle = 270; //Start angle from top left
                    sweepAngle = 90;
                }
                else if (x <= 0 && y >= 0)
                {
                    x *= -1;
                    TruestartP.X -= (TruestartP.X - TrueendP.X);
                    TruestartP.Y += (TrueendP.Y - TruestartP.Y) / 2;
                    TruestartP.Y += (TruestartP.Y - TrueendP.Y);
                    startAngle = 270; //Start angle from top right
                    sweepAngle = -90;
                }
                else if (x >= 0 && y <= 0)
                {
                    y *= -1;
                    TruestartP.X += (TruestartP.X - TrueendP.X);
                    TruestartP.Y += (TrueendP.Y - TruestartP.Y) * 2;
                    startAngle = 90; //Start angle from bottom left
                    sweepAngle = -90;
                }
                else if (x <= 0 && y <= 0)
                {
                    x *= -1;
                    y *= -1;
                    TruestartP.X -= (TruestartP.X - TrueendP.X);
                    TruestartP.Y += (TrueendP.Y - TruestartP.Y) * 2;

                    startAngle = 90; //Start angle from bottom right
                    sweepAngle = 90;
                }
                else //this should never happen
                {
                    startAngle = 270; //Start angle from top mid CW
                    sweepAngle = 90;
                }
                if (Dotted.Checked == true)
                {
                    blackPen.DashPattern = dashValues;
                }
                g.DrawArc(blackPen, TruestartP.X, TruestartP.Y, x, y, startAngle, sweepAngle);
                g.Dispose(); //Prevent out of memory
                picBoxMain.Invalidate();
            }
        }

        private void MainForm_222114X_Load_1(object sender, EventArgs e)
        {
            bm = new Bitmap(picBoxMain.Width, picBoxMain.Height);
            picBoxMain.Image = bm;
        }

        private void picBoxMain_MouseMove(object sender, MouseEventArgs e)
        {
            g = Graphics.FromImage(bm);
            if (flagText!= true && Filler != true && flagDraw == true && Blending != true && Circular != true && Liner != true && Rectanglular != true && Arcs != true && Bezier != true && IsSelecting != true && Eyedroppers != true && Bezier != true)
            {
                endP = e.Location;
                if (Transparencys == true)
                {
                    Pen semiTransPen = new Pen(Color.FromArgb(128, 0, 0, 255), Convert.ToInt32(Math.Round(BrushSize.Value, 0))); //Using backcolor of picboxBrush makes it solid again somehow
                    g.CompositingQuality = CompositingQuality.GammaCorrected;
                    g.DrawLine(semiTransPen, startP.X, startP.Y, endP.X, endP.Y);
                }
                else
                {
                    if (flagErase == false)
                    {
                        Pen pen = new Pen(picBoxBrushColor.BackColor, Convert.ToInt32(Math.Round(BrushSize.Value, 0)));
                        g.DrawLine(pen, startP.X, startP.Y, endP.X, endP.Y);
                    }
                    else
                    {
                        g.FillEllipse(brush, endP.X, endP.Y, Convert.ToInt32(Math.Round(BrushSize.Value, 0)), Convert.ToInt32(Math.Round(BrushSize.Value, 0)));
                    }
                }
            }
            startP = endP;
            g.Dispose();
            picBoxMain.Invalidate();
        }

        private void picBoxMain_MouseDown_1(object sender, MouseEventArgs e)
        {
            startP = e.Location;
            TruestartP = e.Location;
            if (flagText == false)
            {
                if (e.Button == MouseButtons.Left)
                    flagDraw = true;
            }
            else if (txtbox.Text.Length >= 1 && Filler == false && Blending == false && Circular == false && Liner == false && Rectanglular == false && Arcs == false && Eyedroppers == false && Transparencys == false && Bezier == false && IsSelecting == false)
            {
                strText = txtbox.Text;
                g = Graphics.FromImage(bm);
                Font font = txtbox.Font;
                brush = new SolidBrush(picBoxBrushColor.BackColor); 
                shadow = new SolidBrush(Color.DarkGray);
                g.DrawString(strText, font, shadow, startP.X + 1, startP.Y + 1);
                g.DrawString(strText, font, brush, startP.X, startP.Y);
                g.Dispose();
                picBoxMain.Invalidate();
                flagErase = false;
            }
            else if (Filler == true || Blending == true || Liner == true || Circular == true || Rectanglular == true || Arcs == true || Bezier == true || Eyedroppers == true || Bezier == true || IsSelecting == true)
            {
                flagText = false; flagDraw = false;
            }
            else
            {
                flagText = true; flagDraw = true;
            }
        }

        private void picBoxMain_Click(object sender, MouseEventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;
            if (Filler == true)
            {
                Bitmap myBitmap = bm;
                ColorDialog colorPicker = new ColorDialog();
                if (colorPicker.ShowDialog() == DialogResult.OK)
                {
                    Fill.BackColor = colorPicker.Color;
                }
                FloodFill((Bitmap)myBitmap, (Point)coordinates, (Color)myBitmap.GetPixel(coordinates.X, coordinates.Y), (Color)Fill.BackColor);
                Filler = false; Blending = false; Circular = false; Liner = false; Arcs = false; Rectanglular = false; Transparencys = false; Bezier = false; Eyedroppers = false; IsSelecting = false;
            }
            if (Blending == true && coordinates.X > 26 && coordinates.Y > 26 && coordinates.X + 26 < bm.Width && coordinates.Y + 26 < bm.Height)
            {
                R = 0; G = 0; B = 0; A = 0;
                for (int Xcount = 0; Xcount < 50; Xcount++)
                {
                    for (int Ycount = 0; Ycount < 50; Ycount++)
                    {
                        Xcounter = Xcount + (int)TruestartP.X - 25;
                        Ycounter = Ycount + (int)TruestartP.Y - 25;
                        bm.GetPixel(Xcounter, Ycounter);
                        string[] colorsRGBs = bm.GetPixel(Xcounter, Ycounter).ToString().Split(',');
                        foreach (string colors in colorsRGBs)
                        {
                            if (colors.Contains("R="))
                            {
                                R += Int32.Parse(colors.Remove(0, 3));
                            }
                            else if (colors.Contains("G="))
                            {
                                G += Int32.Parse(colors.Remove(0, 3));
                            }
                            else if (colors.Contains("B="))
                            {
                                B += Int32.Parse(colors.Remove(colors.Length - 1, 1).Remove(0, 3));
                            }
                            else if (colors.Contains("A="))
                            {
                                A += Int32.Parse(colors.Remove(0, 9));
                            }
                        }
                    }
                }
                picBoxBrushColor.BackColor = Color.FromArgb(A / 2500, R / 2500, G / 2500, B / 2500);
                Filler = false; Blending = false; Circular = false; Liner = false; Arcs = false; Rectanglular = false; Transparencys = false; Bezier = false; Eyedroppers = false; IsSelecting = false;
            }
            if (Eyedroppers == true)
            {
                Point c1P = coordinates;
                picBoxBrushColor.BackColor = bm.GetPixel(c1P.X, c1P.Y);
                pen.Color = picBoxBrushColor.BackColor;
                picBoxBrushColor.Image = null;
                Temp = txtbox.Text;
                txtbox.Text = "";
                Filler = false; Blending = false; flagErase = false; Circular = false; Liner = false; Arcs = false; Rectanglular = false; Transparencys = false; Bezier = false; Eyedroppers = false; IsSelecting = false;
                flagDraw = true;
            }
        }

        private void picBoxclr_Click(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            Tag = pb.Tag.ToString();
            if (Tag == "green")
            {
                pen.Color = green.BackColor;
            }
            else if (Tag == "blue")
            {
                pen.Color = blue.BackColor;
            }
            else if (Tag == "red")
            {
                pen.Color = red.BackColor;
            }
            else if (Tag == "yellow")
            {
                pen.Color = yellow.BackColor;
            }
            else if (Tag == "orange")
            {
                pen.Color = orange.BackColor;
            }
            else if (Tag == "cyan")
            {
                pen.Color = cyan.BackColor;
            }
            else if (Tag == "purple")
            {
                pen.Color = purple.BackColor;
            }
            else if (Tag == "pink")
            {
                pen.Color = pink.BackColor;
            }
            else
            {
                pen.Color = black.BackColor;
            }
            picBoxBrushColor.BackColor = pen.Color;
            picBoxBrushColor.Image = null;
            Temp = txtbox.Text;
            txtbox.Text = "";
            Filler = false; flagErase = false; Circular = false; Liner = false; Arcs = false; Rectanglular = false; Bezier = false; IsSelecting = false; Eyedroppers = false; Transparencys = false; Bezier = false; Blending = false;
        }

        private void LoadBMP_Click(object sender, EventArgs e)
        {
            int x, y;
            Pather = BMPPath.Text;
            if (Pather != "Type_BMP/PNG/JPG_path_here")
            {
                try
                {
                    bm = new Bitmap(@"" + Pather);
                    Graphics.FromImage(bm).DrawImage(bm, 0, 0, 600, 300);
                }
                catch
                { }
            }
            else
            {
                bm = new Bitmap(picBoxMain.Width, picBoxMain.Height);
            }
            // Loop through the images pixels to reset color.
            for (x = 0; x < bm.Width; x++)
            {
                for (y = 0; y < bm.Height; y++)
                {
                    Color pixelColor = bm.GetPixel(x, y);
                    bm.SetPixel(x, y, pixelColor = bm.GetPixel(x, y));
                }
            }
            // Set the PictureBox to display the image.
            picBoxMain.Image = bm;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            picBoxBrushColor.Image = Properties.Resources.text;
            Filler = false; flagDraw = false; flagErase = false; flagText = true; Circular = false; Liner = false; Arcs = false; Rectanglular = false; Bezier = false; IsSelecting = false; Eyedroppers = false; Transparencys = false; Bezier = false; Blending = false;
        }
        public MainForm_222114X()
        {
            InitializeComponent();

            Color1.Hide();
            Color2.Hide();
            Font.Hide();
            Dotted.Hide();
            Gradient.Hide();
            BMPPath.Hide();
            Loads.Hide();
        }

        private void cheungXinYanJason222114XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            Clipboard.SetText(attribute.Value.ToString());
        }
        private void FloodFill(Bitmap bm, Point coordinates, Color targetColor, Color replacementColor)
        {
            targetColor = bm.GetPixel(coordinates.X, coordinates.Y); pixels.Push(coordinates);
            while (pixels.Count > 0)
            {
                Point pixelss = pixels.Pop();//Reiterate through the stack(List)
                if (pixelss.X < bm.Width && pixelss.X > 0 && pixelss.Y < bm.Height && pixelss.Y > 0)
                {
                    if (bm.GetPixel(pixelss.X, pixelss.Y) == targetColor)
                    {
                        bm.SetPixel(pixelss.X, pixelss.Y, replacementColor);
                        //Fill top bottom left right
                        pixels.Push(new Point(pixelss.X - 1, pixelss.Y));
                        pixels.Push(new Point(pixelss.X + 1, pixelss.Y));
                        pixels.Push(new Point(pixelss.X, pixelss.Y - 1));
                        pixels.Push(new Point(pixelss.X, pixelss.Y + 1));
                    }
                }
            }
            picBoxMain.Refresh(); return; //Refresh to reiterate
        }

    }
}
