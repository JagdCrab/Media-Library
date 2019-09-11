using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;

using Media_Library.Components;

namespace Media_Library.Data
{
    class MediaAccessor
    {
        #region Image Tools
        /*---------------------------------------------*
         *---------------- Image Tools ----------------*
         *---------------------------------------------*/

        public static BitmapSource ResizeImage(BitmapSource sourceImage, int width, int height, bool crop)
        {
            double sourceX = 0;
            double sourceY = 0;
            double sourceWidth = sourceImage.Width;
            double sourceHeight = sourceImage.Height;
            double sourceAspectRatio = Math.Round(sourceWidth / sourceHeight, 5);

            double destWidth = width;
            double destHeight = height;
            double destAspectRatio = Math.Round(destWidth / destHeight, 5);

            if (sourceAspectRatio != destAspectRatio)
            {
                double widthScaleFactor = destWidth / sourceWidth;
                double heightScaleFactor = destHeight / sourceHeight;

                if (crop)
                {
                    if (widthScaleFactor <= heightScaleFactor)
                        sourceX = Convert.ToInt32((sourceWidth - destWidth / heightScaleFactor) / 2);
                    else
                        sourceY = Convert.ToInt32((sourceHeight - destHeight / widthScaleFactor) / 2);
                }
                else
                {
                    if (widthScaleFactor <= heightScaleFactor)
                        destHeight = sourceHeight * widthScaleFactor;
                    else
                        destWidth = sourceWidth * heightScaleFactor;
                }
            }

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext context = drawingVisual.RenderOpen())
                context.DrawImage(sourceImage, new Rect(new Size(destWidth, destHeight)));

            RenderTargetBitmap result = new RenderTargetBitmap(Convert.ToInt32(destWidth), Convert.ToInt32(destHeight), 96, 96, PixelFormats.Default);
            result.Render(drawingVisual);

            return result;
        }

        public static BitmapSource CreateGridScreenlist(string file)
        {
            int slWidth = 1920;
            int slHeight = 1080;

            MediaFile sourceFile = GetMetaData(file);
            string resolution = sourceFile.Metadata.VideoData.FrameSize;

            int screenshotWidth = Convert.ToInt32(resolution.Substring(0, resolution.IndexOf('x')));
            int screenshotHeight = Convert.ToInt32(resolution.Substring(resolution.IndexOf('x') + 1));

            List<Rect> grid = CreateGrid(new Size(slWidth, slHeight), new Size(screenshotWidth, screenshotHeight), 46);
            List<string> screenshotLinks = SaveScreenshots(sourceFile, grid.Count());

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext context = drawingVisual.RenderOpen())
            {
                context.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect(new Size(1920d, 1080d)));

                for (int i = 0; i < grid.Count; i++)
                {
                    BitmapImage screenshot = new BitmapImage();
                    using (var stream = new FileStream(screenshotLinks[i], FileMode.Open, FileAccess.Read))
                    {
                        screenshot.BeginInit();
                        screenshot.StreamSource = stream;
                        screenshot.CacheOption = BitmapCacheOption.OnLoad;
                        screenshot.EndInit();
                        screenshot.Freeze();
                    }

                    context.DrawImage(screenshot, grid[i]);

                    if (File.Exists(screenshotLinks[i]))
                        File.Delete(screenshotLinks[i]);
                }
            }

            RenderTargetBitmap screenlist = new RenderTargetBitmap(slWidth, slHeight, 96, 96, PixelFormats.Default);
            screenlist.Render(drawingVisual);
            screenlist.Freeze();

            return screenlist;
        }

        public static BitmapSource CreateGridScreenlist(string _file, Observable<int> _step, Observable<int> _max)
        {
            int slWidth = 1920;
            int slHeight = 1080;

            MediaFile sourceFile = GetMetaData(_file);
            string resolution = sourceFile.Metadata.VideoData.FrameSize;

            int screenshotWidth = Convert.ToInt32(resolution.Substring(0, resolution.IndexOf('x')));
            int screenshotHeight = Convert.ToInt32(resolution.Substring(resolution.IndexOf('x') + 1));

            List<Rect> grid = CreateGrid(new Size(slWidth, slHeight), new Size(screenshotWidth, screenshotHeight), 46);
            int screenshotsCount = grid.Count;
            int currentStep = 0;

            Application.Current.Dispatcher.BeginInvoke(new Action(() => { _max.Value = screenshotsCount * 2; }));
            Application.Current.Dispatcher.BeginInvoke(new Action(() => { _step.Value = currentStep * 2; }));

            List<string> screenshotLinks = new List<string>();

            if (!Directory.Exists(Path.Combine(Path.GetTempPath(), "MediaLib")))
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "MediaLib"));

            using (var engine = new Engine())
            {
                double screenshotWindow = sourceFile.Metadata.Duration.TotalSeconds / (screenshotsCount + 1);

                for (int n = 0; n < screenshotsCount; n++)
                {
                    string pathOut = Path.Combine(Path.GetTempPath(), "MediaLib", Path.GetFileNameWithoutExtension(sourceFile.Filename) + "_" + n.ToString() + ".jpg");
                    MediaFile screenshotFile = new MediaFile() { Filename = pathOut };
                    ConversionOptions options = new ConversionOptions() { Seek = TimeSpan.FromSeconds(screenshotWindow * (n + 1)) };
                    engine.GetThumbnail(sourceFile, screenshotFile, options);
                    screenshotLinks.Add(pathOut);
                    currentStep++;

                    Application.Current.Dispatcher.BeginInvoke(new Action(() => { _step.Value = currentStep * 2; }));
                }
            }

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext context = drawingVisual.RenderOpen())
            {
                context.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect(new Size(1920d, 1080d)));

                for (int i = 0; i < screenshotsCount; i++)
                {
                    BitmapImage screenshot = new BitmapImage();
                    using (var stream = new FileStream(screenshotLinks[i], FileMode.Open, FileAccess.Read))
                    {
                        screenshot.BeginInit();
                        screenshot.StreamSource = stream;
                        screenshot.CacheOption = BitmapCacheOption.OnLoad;
                        screenshot.EndInit();
                        screenshot.Freeze();
                    }

                    context.DrawImage(screenshot, grid[i]);

                    if (File.Exists(screenshotLinks[i]))
                        File.Delete(screenshotLinks[i]);

                    currentStep++;

                    Application.Current.Dispatcher.BeginInvoke(new Action(() => { _step.Value = currentStep * 2; }));
                }
            }

            RenderTargetBitmap screenlist = new RenderTargetBitmap(slWidth, slHeight, 96, 96, PixelFormats.Default);
            screenlist.Render(drawingVisual);
            screenlist.Freeze();

            return screenlist;
        }

        public static List<Rect> CreateGrid(Size baseSize, Size elementSize, int elements)
        {
            //Establish Initial Dimensions, Aspect Ratios and first guess on element sizes
            int baseWidth = Convert.ToInt32(baseSize.Width);
            int baseHeight = Convert.ToInt32(baseSize.Height);
            double baseAR = (double)baseWidth / baseHeight;
            Fraction baseARF = new Fraction(baseAR, 0.001);

            int elementWidth = Convert.ToInt32(elementSize.Width);
            int elementHeight = Convert.ToInt32(elementSize.Height);
            double elementAR = (double)elementWidth / elementHeight;
            Fraction elementARF = new Fraction(elementAR, 0.001);

            long baseArea = baseWidth * baseHeight;
            long elementArea = baseArea / elements;

            double elementSide = Math.Sqrt(elementArea / (elementARF.D * elementARF.N));
            elementWidth = Convert.ToInt32(elementSide * elementARF.N);
            elementHeight = Convert.ToInt32(elementSide * elementARF.D);

            //Check if minimum of elements requested fit into grid, and scale if neccessary

            int columns = baseWidth / elementWidth;
            int rows = baseHeight / elementHeight;

            int leftoversWidth = baseWidth % (elementWidth * columns);
            int leftoverHeight = baseHeight % (elementHeight * rows);

            while (columns * rows < elements)
            {
                elementWidth = Convert.ToInt32(elementWidth * 0.975);
                elementHeight = Convert.ToInt32(elementHeight * 0.975);

                columns = baseWidth / (elementWidth + 8);
                rows = baseHeight / (elementHeight + 8);

                leftoversWidth = baseWidth % ((elementWidth * columns) + (8 * (columns - 1)));
                leftoverHeight = baseHeight % ((elementHeight * rows) + (8 * (rows - 1)));
            }

            //Create margins
            int widthMargin = leftoversWidth / 2;
            int heightMargin = leftoverHeight / 2;

            int fittingElements = columns * rows;

            List<Rect> grid = new List<Rect>();

            int x = 0;
            int y = 0;

            for (int row = 0; row < rows; row++)
            {
                y = elementHeight * row + 8 * row + heightMargin;
                //y = elementHeight * row + heightMargin;
                for (int column = 0; column < columns; column++)
                {
                    x = elementWidth * column + 8 * column + widthMargin;
                    //x = elementWidth * column + widthMargin;
                    grid.Add(new Rect(new Point(x, y), new Size(elementWidth, elementHeight)));
                }
            }

            return grid;
        }

        #endregion

        #region Video Tools
        /*---------------------------------------------*
         *---------------- Video Tools ----------------*
         *---------------------------------------------*/

        public static MediaFile GetMetaData(string file)
        {
            var mediaFile = new MediaFile() { Filename = file };
            using (var engine = new Engine())
            {
                engine.GetMetadata(mediaFile);
            }
            return mediaFile;
        }

        public static MediaFile GetMetaData(MediaFile file)
        {
            var mediaFile = file;
            using (var engine = new Engine())
            {
                engine.GetMetadata(mediaFile);
            }
            return mediaFile;
        }

        public static List<string> SaveScreenshots(MediaFile videoFile, int screenshotCount)
        {
            List<string> results = new List<string>();

            if (!Directory.Exists(Path.Combine(Path.GetTempPath(), "MediaLib")))
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "MediaLib"));

            using (var engine = new Engine())
            {
                if (videoFile.Metadata == null)
                    engine.GetMetadata(videoFile);

                double screenshotWindow = videoFile.Metadata.Duration.TotalSeconds / (screenshotCount + 1);

                for (int n = 0; n < screenshotCount; n++)
                {
                    string pathOut = Path.Combine(Path.GetTempPath(), "MediaLib", Path.GetFileNameWithoutExtension(videoFile.Filename) + "_" + n.ToString() + ".jpg");
                    MediaFile screenshotFile = new MediaFile() { Filename = pathOut };
                    ConversionOptions options = new ConversionOptions() { Seek = TimeSpan.FromSeconds(screenshotWindow * (n + 1)) };
                    engine.GetThumbnail(videoFile, screenshotFile, options);
                    results.Add(pathOut);
                }
            }

            return results;
        }

        public static string SaveScreenshot(MediaFile videoFile, TimeSpan time)
        {
            if (!Directory.Exists(Path.Combine(Path.GetTempPath(), "MediaLib")))
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "MediaLib"));

            string pathOut = Path.Combine(Path.GetTempPath(), "MediaLib", Path.GetFileNameWithoutExtension(videoFile.Filename) + "_" + time.Seconds.ToString() + ".jpg");
            MediaFile screenshotFile = new MediaFile() { Filename = pathOut };

            using (var engine = new Engine())
            {
                if (videoFile.Metadata == null)
                    engine.GetMetadata(videoFile);

                ConversionOptions options = new ConversionOptions() { Seek = time };
                engine.GetThumbnail(videoFile, screenshotFile, options);
            }

            return screenshotFile.Filename;
        }

        #endregion
    }
}
