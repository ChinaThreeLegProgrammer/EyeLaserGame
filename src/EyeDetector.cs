using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace EyeLaserGame
{
    public class EyeDetector
    {
        // 存储检测到的眼睛位置
        public Point LeftEyePosition { get; private set; }
        public Point RightEyePosition { get; private set; }
        
        // 固定的眼球位置坐标
        // 第一个眼球（左眼）：左上(178,282)，右下(226,330)
        // 第二个眼球（右眼）：左上(484,300)，右下(523,343)
        private readonly int _leftEyeX = (178 + 226) / 2;  // 左眼中心点X坐标
        private readonly int _leftEyeY = (282 + 330) / 2;  // 左眼中心点Y坐标
        private readonly int _rightEyeX = (484 + 523) / 2; // 右眼中心点X坐标
        private readonly int _rightEyeY = (300 + 343) / 2; // 右眼中心点Y坐标
        
        /// <summary>
        /// 从图像中获取眼睛位置（使用固定坐标）
        /// </summary>
        /// <param name="imagePath">图像文件路径</param>
        /// <returns>总是返回true，因为使用固定坐标</returns>
        public bool DetectEyesFromImage(string imagePath)
        {
            // 设置固定的眼睛位置
            SetFixedEyePositions();
            return true;
        }
        
        /// <summary>
        /// 从BitmapImage中获取眼睛位置（使用固定坐标）
        /// </summary>
        /// <param name="bitmapImage">BitmapImage对象</param>
        /// <returns>总是返回true，因为使用固定坐标</returns>
        public bool DetectEyesFromBitmapImage(BitmapImage bitmapImage)
        {
            // 设置固定的眼睛位置
            SetFixedEyePositions();
            return true;
        }
        
        /// <summary>
        /// 设置固定的眼睛位置
        /// </summary>
        private void SetFixedEyePositions()
        {
            // 使用预定义的固定坐标
            LeftEyePosition = new Point(_leftEyeX, _leftEyeY);
            RightEyePosition = new Point(_rightEyeX, _rightEyeY);
        }
    }
}
