using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ColorJunction
{
    class Sprite
    {

        BitmapImage[] _spriteAnimations1;//Animations related to the sprite, for example idle, active, selected etc
        ImageBrush _currentFrame;
        DispatcherTimer _frameTimer;

        int _imageIndex = 0;

        public Sprite(BitmapImage[] bitmpImage) 
        { 
            _spriteAnimations1 = bitmpImage;
            ImageBrush tempImage = new ImageBrush();
            tempImage.ImageSource = _spriteAnimations1[0];
            _currentFrame = tempImage;
            _frameTimer = new DispatcherTimer();
            _frameTimer.Tick += (sender, e) =>
            {
                frameCount();
                _currentFrame.ImageSource = _spriteAnimations1[_imageIndex];
            };
            _frameTimer.Interval = TimeSpan.FromMilliseconds(30);
            _frameTimer.Start();
        }

        public ImageBrush Animation
        {
            get
            {
                return _currentFrame;
            }
        }

        private void frameCount() 
        {
            if (_imageIndex == _spriteAnimations1.Length-2)
            {
                _imageIndex = 0;
            }
            else 
            {
                _imageIndex++;
            }
        }

        public BitmapImage[] Frames
        {
            get 
            {
                return _spriteAnimations1;
            }
        }
    }
}
