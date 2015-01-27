using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Threading;
using System.IO;
using System.Drawing.Imaging;

namespace ColorJunction
{
    class Sprite
    {
        BitmapImage _spriteSheet;
        ImageBrush _currentFrame;
        List<BitmapImage> _spriteFrames;
        DispatcherTimer _timer;

        int _animIndex = 0;

        int _numFrames;
        int _frameWidth;
        int _frameHeight;


        /// <summary>
        /// Create new sprite
        /// </summary>
        /// <param name="spriteSheet">BitmapImage of spritesheet</param>
        /// <param name="frames">Number of frames in the animation</param>
        /// <param name="frameWidth">Width of each frame in pixels</param>
        /// <param name="frameHeight">Height of each frame in pixels</param>
        public Sprite(BitmapImage spriteSheet, int frames, int frameWidth, int frameHeight)
        {
            this._spriteSheet = spriteSheet;
            this._numFrames = frames;
            this._frameWidth = frameWidth;
            this._frameHeight = frameHeight;

            this._spriteFrames = cutSheet();

            this._currentFrame = new ImageBrush(this._spriteFrames.First());

            this._timer = new DispatcherTimer();
            this._timer.Tick += (sender, e) =>
            {
                incrementIndex();
                this._currentFrame.ImageSource = this._spriteFrames[this._animIndex];
            };
            this._timer.Interval = TimeSpan.FromMilliseconds(100);
            this._timer.Start();

        }

        public ImageBrush Brush
        {
            get { return this._currentFrame; }
        }

        private void incrementIndex()
        {
            if (this._animIndex == _spriteFrames.Count - 1)
            {
                this._animIndex = 0;
            }
            else
            {
                this._animIndex++;
            }
        }

        public List<BitmapImage> Frames
        {
            get { return _spriteFrames; }
        }

        private List<BitmapImage> cutSheet()
        {
            List<BitmapImage> frames = new List<BitmapImage>();

            Rectangle cropRect = new Rectangle(0, 0, _frameWidth, _frameHeight);
            Bitmap sheet = BitmapImage2Bitmap(this._spriteSheet);
            Bitmap frame;

            if ((sheet.Height % this._frameHeight) == 0 && (sheet.Width % this._frameWidth) == 0)
            {
                int rows = sheet.Height / this._frameHeight;
                int columns = sheet.Width / this._frameWidth;
                int frameCount = 0;
                for (int row = 0; row < rows; row++)
                {
                    if (frameCount < this._numFrames)
                    {
                        for (int col = 0; col < columns; col++)
                        {
                            int currentY = row * this._frameWidth;
                            int currentX = col * this._frameHeight;
                            cropRect.X = currentX;
                            cropRect.Y = currentY;

                            frame = new System.Drawing.Bitmap(cropRect.Width, cropRect.Height);
                            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(frame))
                            {
                                g.DrawImage(sheet, new System.Drawing.Rectangle(0, 0, frame.Width, frame.Height), cropRect, System.Drawing.GraphicsUnit.Pixel);
                            }

                            frames.Add(Bitmap2BitmapImage(frame));

                            frameCount++;

                            if (frameCount == this._numFrames) { break; }
                        }
                    }
                    else { break; }
                }
            }
            return frames;
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}
