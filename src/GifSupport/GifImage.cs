using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace EyeLaserGame
{
    public class GifImage : Image
    {
        private bool _isInitialized;
        private GifBitmapDecoder _gifDecoder;
        private Int32Animation _animation;
        private bool _isAnimationWorking;

        public static readonly DependencyProperty FrameIndexProperty =
            DependencyProperty.Register("FrameIndex", typeof(int), typeof(GifImage), 
                new UIPropertyMetadata(0, new PropertyChangedCallback(ChangingFrameIndex)));

        public static readonly DependencyProperty AutoStartProperty =
            DependencyProperty.Register("AutoStart", typeof(bool), typeof(GifImage), 
                new UIPropertyMetadata(false, new PropertyChangedCallback(AutoStartPropertyChanged)));

        public static readonly DependencyProperty GifSourceProperty =
            DependencyProperty.Register("GifSource", typeof(string), typeof(GifImage), 
                new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(GifSourcePropertyChanged)));

        public int FrameIndex
        {
            get { return (int)GetValue(FrameIndexProperty); }
            set { SetValue(FrameIndexProperty, value); }
        }

        public bool AutoStart
        {
            get { return (bool)GetValue(AutoStartProperty); }
            set { SetValue(AutoStartProperty, value); }
        }

        public string GifSource
        {
            get { return (string)GetValue(GifSourceProperty); }
            set { SetValue(GifSourceProperty, value); }
        }

        private static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            GifImage gifImage = obj as GifImage;
            gifImage.OnChangingFrameIndex((int)ev.OldValue, (int)ev.NewValue);
        }

        private void OnChangingFrameIndex(int oldValue, int newValue)
        {
            if (_gifDecoder != null && newValue >= 0 && newValue < _gifDecoder.Frames.Count)
                Source = _gifDecoder.Frames[newValue];
        }

        private static void AutoStartPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            GifImage gifImage = obj as GifImage;
            gifImage.OnAutoStartPropertyChanged((bool)ev.OldValue, (bool)ev.NewValue);
        }

        private void OnAutoStartPropertyChanged(bool oldValue, bool newValue)
        {
            if (newValue && _isInitialized)
                StartAnimation();
        }

        private static void GifSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            GifImage gifImage = obj as GifImage;
            gifImage.OnGifSourcePropertyChanged((string)ev.OldValue, (string)ev.NewValue);
        }

        private void OnGifSourcePropertyChanged(string oldValue, string newValue)
        {
            if (!string.IsNullOrEmpty(newValue))
                Initialize();
        }

        private void Initialize()
        {
            try
            {
                _gifDecoder = new GifBitmapDecoder(new Uri(GifSource), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                _isInitialized = true;
            }
            catch (Exception)
            {
                _isInitialized = false;
                throw;
            }

            if (AutoStart)
                StartAnimation();
        }

        public void StartAnimation()
        {
            if (!_isInitialized)
                Initialize();

            if (!_isAnimationWorking)
            {
                BeginAnimation();
                _isAnimationWorking = true;
            }
        }

        public void StopAnimation()
        {
            if (_isAnimationWorking)
            {
                StopAnimation();
                _isAnimationWorking = false;
            }
        }

        private void BeginAnimation()
        {
            _animation = new Int32Animation(0, _gifDecoder.Frames.Count - 1, 
                new Duration(TimeSpan.FromSeconds((double)_gifDecoder.Frames.Count / 10)))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            
            this.BeginAnimation(FrameIndexProperty, _animation);
        }

        private void EndAnimation()
        {
            this.BeginAnimation(FrameIndexProperty, null);
        }
    }
}