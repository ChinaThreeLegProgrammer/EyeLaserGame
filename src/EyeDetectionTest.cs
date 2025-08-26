using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace EyeLaserGame
{
    public class EyeDetectionTest
    {
        public static void TestEyeDetection(string imagePath)
        {
            Console.WriteLine($"测试眼睛检测算法，图像路径: {imagePath}");
            
            EyeDetector detector = new EyeDetector();
            bool success = detector.DetectEyesFromImage(imagePath);
            
            if (success)
            {
                Console.WriteLine("成功检测到眼睛位置:");
                Console.WriteLine($"左眼位置: X={detector.LeftEyePosition.X}, Y={detector.LeftEyePosition.Y}");
                Console.WriteLine($"右眼位置: X={detector.RightEyePosition.X}, Y={detector.RightEyePosition.Y}");
                
                // 在图像上标记眼睛位置并保存
                MarkEyesOnImage(imagePath, detector.LeftEyePosition, detector.RightEyePosition);
            }
            else
            {
                Console.WriteLine("未能成功检测到眼睛位置。");
            }
        }
        
        private static void MarkEyesOnImage(string imagePath, Point leftEye, Point rightEye)
        {
            string outputPath = Path.Combine(
                Path.GetDirectoryName(imagePath),
                Path.GetFileNameWithoutExtension(imagePath) + "_marked" + Path.GetExtension(imagePath));
                
            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    // 绘制左眼标记（红色圆圈）
                    g.DrawEllipse(new Pen(Color.Red, 3), 
                        leftEye.X - 5, leftEye.Y - 5, 10, 10);
                    
                    // 绘制右眼标记（红色圆圈）
                    g.DrawEllipse(new Pen(Color.Red, 3), 
                        rightEye.X - 5, rightEye.Y - 5, 10, 10);
                }
                
                bitmap.Save(outputPath);
                Console.WriteLine($"已保存标记后的图像: {outputPath}");
            }
        }
    }
}