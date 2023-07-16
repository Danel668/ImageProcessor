using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ImageProcessor
{
    public partial class Form1 : Form
    {
        public string? imagepath { get; set; } = null;
        public Color color { get; set; } = Color.FromArgb(0, 177, 64);
        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Файлы PNG (*.png)|*.png";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    imagepath = ofd.FileName;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            DialogResult result = colorDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                color = colorDialog.Color;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (imagepath != null)
            {
                Bitmap image = new Bitmap(imagepath);

                Color targetColor = color;

                Bitmap result = new Bitmap(image.Width, image.Height);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                BitmapData imageData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData resultData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                int bytesPerPixel = 4; // 4 байта на пиксель (ARGB)
                int imageSize = imageData.Stride * imageData.Height;

                byte[] imagePixels = new byte[imageSize];
                //byte[] resultPixels = new byte[imageSize];

                Marshal.Copy(imageData.Scan0, imagePixels, 0, imageSize);

                for (int i = 0; i < imageSize; i += bytesPerPixel)
                {
                    byte blue = imagePixels[i];
                    byte green = imagePixels[i + 1];
                    byte red = imagePixels[i + 2];
                    byte alpha = imagePixels[i + 3];

                    Color pixelColor = Color.FromArgb(alpha, red, green, blue);

                    if (ChromaKeyHelper.IsChromaKeyColor(pixelColor, targetColor, 50))
                    {
                        imagePixels[i] = 0;
                        imagePixels[i + 1] = 0;
                        imagePixels[i + 2] = 0;
                        imagePixels[i + 3] = 0;
                    }
                    else
                    {
                        imagePixels[i] = blue;
                        imagePixels[i + 1] = green;
                        imagePixels[i + 2] = red;
                        imagePixels[i + 3] = alpha;
                    }
                }

                Marshal.Copy(imagePixels, 0, resultData.Scan0, imageSize);

                image.UnlockBits(imageData);
                result.UnlockBits(resultData);

                DateTime curDate = DateTime.Now;
                string imgname = curDate.ToString("yyyy.MM.dd.HH.mm.ss") + ".png";
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        string selectedDirectory = fbd.SelectedPath;
                        string img = Path.Combine(selectedDirectory, imgname);

                        try
                        {
                            result.Save(img, ImageFormat.Png);
                            MessageBox.Show("Изображение успешно сохранено!", "Уведомление");
                        }
                        catch
                        {
                            MessageBox.Show("Файл с таким названием уже существует!", "Уведомление");
                        }
                    }
                }
            }
            else
                MessageBox.Show("Неккоректный файл!", "Уведомление");
        }
    }
}