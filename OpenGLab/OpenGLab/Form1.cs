using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Smertin_tomogram_vizualizer
{
    public partial class Form1 : Form
    {
        Bin binaryFile;
        View view;
        bool loaded;
        bool needReload = false;
        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        void DispayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps = {0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }
        int currentLayer;
        public Form1()
        {
            InitializeComponent();
            view = new View();
            binaryFile = new Bin();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                binaryFile.ReadBin(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                trackBar1.Maximum = Bin.Z-1;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {   if (radioButton2.Checked)
                {
                  //  view.DrawQuads(currentLayer);
                    view.DrawQuads(currentLayer);
                }
            if (radioButton1.Checked)
            
                {
                    //   view.DrawQuads(currentLayer);
                    if (needReload)
                    {
                        view.generateTextureImage(currentLayer);
                        view.Load2DTexture();
                        needReload = false;

                    }
                    view.DrawTexture();
                    
                }
                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
            //glControl1.Refresh();
            
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                DispayFPS();
                glControl1.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            view.SetMinMaxTransferFunction(trackBar2.Value, trackBar2.Value + trackBar3.Value);
            label4.Text = "Текущий минимум = " + trackBar2.Value;
            label5.Text = "Текущий максимум = " + (trackBar2.Value + trackBar3.Value);
            needReload = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            view.SetMinMaxTransferFunction(trackBar2.Value, trackBar2.Value + trackBar3.Value);
            label6.Text = "Текущая ширина = " + trackBar3.Value;
            label5.Text = "Текущий максимум = " + (trackBar2.Value + trackBar3.Value);
            needReload = true;
        }
    }
}
