using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace EyeLaserGame.Audio
{
    /// <summary>
    /// 音效管理类，负责加载和播放游戏音效
    /// </summary>
    public class SoundManager
    {
        private MediaPlayer _collisionSound;
        private bool _isPlayingCollisionSound = false;
        private static SoundManager _instance;
        
        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SoundManager();
                }
                return _instance;
            }
        }
        
        private SoundManager()
        {
            try
            {
                // 初始化音频播放器
                _collisionSound = new MediaPlayer();
                _collisionSound.MediaEnded += (s, e) => 
                {
                    _isPlayingCollisionSound = false;
                    _collisionSound.Stop();
                    Console.WriteLine("音效播放结束");
                };
                
                // 获取音频文件的完整路径
                string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "warning.mp3");
                
                if (File.Exists(soundPath))
                {
                    // 使用文件路径创建 Uri
                    Uri soundUri = new Uri(soundPath);
                    _collisionSound.Open(soundUri);
                    Console.WriteLine($"音效文件已加载: {soundPath}");
                }
                else
                {
                    Console.WriteLine($"音效文件不存在: {soundPath}");
                }
                
                Console.WriteLine("音效管理器初始化成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载音效时出错: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 播放碰撞音效
        /// </summary>
        public void PlayCollisionSound()
        {
            try
            {
                if (_collisionSound != null && !_isPlayingCollisionSound)
                {
                    _isPlayingCollisionSound = true;
                    _collisionSound.Position = TimeSpan.Zero; // 重置播放位置
                    _collisionSound.Play();
                    Console.WriteLine("播放碰撞音效");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"播放碰撞音效时出错: {ex.Message}");
                _isPlayingCollisionSound = false; // 重置状态，以便下次尝试播放
            }
        }
    }
}
