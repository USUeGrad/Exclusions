using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

/*
 * Resizes images in directory
 * */

namespace SnapShotApp
{
    public class ImageResizer
    {
        public void ResizeImagesInDirectory(string folderPath)
        {
            DirectoryInfo d = new DirectoryInfo(folderPath);

            foreach (var file in d.GetFiles("*.png"))
            {
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file.FullName);

                string outputFile = folderPath + fileNameWithoutExt + ".jpeg";

                //  Resize(file.FullName, outputFile, .6);  //previous resize value. User complained that text was blurry.
                    Resize(file.FullName, outputFile, .8);

                File.Delete(file.FullName);
            }
        }

        public void Resize(string imageFile, string outputFile, double scaleFactor)
        {
            using (var srcImage = Image.FromFile(imageFile))
            {
                var newWidth = (int)(srcImage.Width * scaleFactor);
                var newHeight = (int)(srcImage.Height * scaleFactor);
                using (var newImage = new Bitmap(newWidth, newHeight))
                using (var graphics = Graphics.FromImage(newImage))
                {
                    graphics.SmoothingMode = SmoothingMode.Default;
                    graphics.InterpolationMode = InterpolationMode.Default;
                    graphics.PixelOffsetMode = PixelOffsetMode.Default;
                    graphics.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight));
                    newImage.Save(outputFile, ImageFormat.Jpeg);
                }
            }
        }
    }
}