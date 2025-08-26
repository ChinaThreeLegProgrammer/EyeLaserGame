using System;
using System.IO;
using System.Windows;

namespace EyeLaserGame
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // 如果有命令行参数，则运行测试
            if (args.Length > 0 && args[0] == "--test")
            {
                RunTests();
                return;
            }
            
            // 否则启动WPF应用程序
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
        
        private static void RunTests()
        {
            Console.WriteLine("运行眼睛检测算法测试...");
            
            // 获取测试图像路径
            string testImagePath = GetTestImagePath();
            if (string.IsNullOrEmpty(testImagePath))
            {
                Console.WriteLine("未找到测试图像，请确保在程序目录下有一个名为'test.jpg'或'test.png'的图像文件。");
                return;
            }
            
            // 运行眼睛检测测试
            EyeDetectionTest.TestEyeDetection(testImagePath);
        }
        
        private static string GetTestImagePath()
        {
            // 尝试在程序目录下查找测试图像
            string[] possibleExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            foreach (var ext in possibleExtensions)
            {
                string path = Path.Combine(baseDir, "test" + ext);
                if (File.Exists(path))
                {
                    return path;
                }
            }
            
            return null;
        }
    }
}