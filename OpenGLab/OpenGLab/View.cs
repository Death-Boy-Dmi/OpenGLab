
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace Smertin_tomogram_vizualizer
{
    class View
    {
        public int min=-1000, max=2000;
        private Bitmap textureImage;
        private int VBOtexture; //хранит номер текстуры в памяти видеокарты
        public void SetupView(int width, int height)
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Bin.X, 0, Bin.Z, -1, 1);
            GL.Viewport(0, 0, width, height);
        }
        public void SetMinMaxTransferFunction(int _min, int _max)
        {
            min = _min;
            max = _max;
        }
        public Color TransferFunction(short Value)
        {
            if ((max - min) == 0) max++;
            int newVal = Clamp((Value - min) * 255 / (max - min), 0, 255);
            return Color.FromArgb(255, newVal, newVal, newVal);
        }
        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        public void DrawQuads(int layerNumber)
        {
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			for (int x_coord = 0; x_coord < Bin.X - 1; x_coord++)
			{
				GL.Begin(BeginMode.QuadStrip);
				short value;
				//1 вершина
				value = Bin.array[x_coord + layerNumber * Bin.X];
				GL.Color3(TransferFunction(value));
				GL.Vertex2(x_coord, 0);
				//2 вершина
				value = Bin.array[(x_coord + 1) + layerNumber * Bin.X];
				GL.Color3(TransferFunction(value));
				GL.Vertex2(x_coord + 1, 0);
				for (int z_coord = 0; z_coord < Bin.Z - 1; z_coord++)
				{
					//3 вершина
					value = Bin.array[x_coord + layerNumber * Bin.X
						+ (z_coord + 1) * Bin.X * Bin.Y];
					GL.Color3(TransferFunction(value));
					GL.Vertex2(x_coord, z_coord + 1);
					//4 вершина
					value = Bin.array[(x_coord + 1) + layerNumber * Bin.X
						+ (z_coord + 1) * Bin.X * Bin.Y];
					GL.Color3(TransferFunction(value));
					GL.Vertex2(x_coord + 1, z_coord + 1);
				}
				GL.End();
			}
		}

        public void Load2DTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);
            BitmapData data = textureImage.LockBits(new System.Drawing.Rectangle(0, 0, textureImage.Width, textureImage.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0);


            textureImage.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            ErrorCode Er = GL.GetError();
            string str = Er.ToString();
        }

        public void generateTextureImage(int layerNumber)
        {
            textureImage = new Bitmap(Bin.X, Bin.Z);
            for (int i = 0; i < Bin.X; ++i)
                for (int j = 0; j < Bin.Z; ++j)
                {
                    int pixelNumber = i + layerNumber * Bin.X + j * Bin.X * Bin.Y;
                    textureImage.SetPixel(i, j, TransferFunction(Bin.array[pixelNumber]));
                }
        }

        public void DrawTexture()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);

            GL.Begin(BeginMode.Quads);
            GL.Color3(Color.White);

            GL.TexCoord2(0f, 0f);
            GL.Vertex2(0, 0);

            GL.TexCoord2(0f, 1f);
            GL.Vertex2(0, Bin.Z);

            GL.TexCoord2(1f, 1f);
            GL.Vertex2(Bin.X, Bin.Z);

            GL.TexCoord2(1f, 0f);
            GL.Vertex2(Bin.X, 0);
            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }

    }
   

}
