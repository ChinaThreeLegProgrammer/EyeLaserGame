using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using EyeLaserGame.Audio;
using EyeLaserGame.Models;
using EyeLaserGame.MVVM;
using Point = System.Drawing.Point;

namespace EyeLaserGame.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly GameModel _gameModel;
        private readonly EyeDetector _eyeDetector;
        private readonly DispatcherTimer _gameTimer;
        private readonly Random _random = new Random();
        
        private bool _isGameRunning;
        private int _score;
        
        // 属性
        public bool IsGameRunning
        {
            get => _isGameRunning;
            set => SetProperty(ref _isGameRunning, value);
        }
        
        public int Lives
        {
            get => _gameModel.Lives;
            set
            {
                if (_gameModel.Lives != value)
                {
                    _gameModel.Lives = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private int _previousHealth;
        public int Health
        {
            get => _gameModel.Health;
            set
            {
                if (_gameModel.Health != value)
                {
                    _previousHealth = _gameModel.Health;
                    _gameModel.Health = value;
                    OnPropertyChanged();
                    
                    // 触发血条动画
                    AnimateHealthChange(_previousHealth, value);
                }
            }
        }
        
        // 用于触发血条动画的方法
        private void AnimateHealthChange(int oldValue, int newValue)
        {
            // 这个方法会被调用，但实际的动画是在XAML中通过绑定实现的
            // 我们可以在这里添加额外的逻辑，比如当血量低于某个阈值时播放警告音效
            if (newValue <= 30 && oldValue > 30)
            {
                // 血量低于30%时播放警告音效
                SoundManager.Instance.PlayCollisionSound();
            }
        }
        
        public int Score
        {
            get => _score;
            set => SetProperty(ref _score, value);
        }
        
        public Point PlayerPosition
        {
            get => _gameModel.PlayerPosition;
            set
            {
                if (_gameModel.PlayerPosition != value)
                {
                    _gameModel.PlayerPosition = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public Point LeftEyePosition
        {
            get => _gameModel.LeftEyePosition;
            set
            {
                if (_gameModel.LeftEyePosition != value)
                {
                    _gameModel.LeftEyePosition = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public Point RightEyePosition
        {
            get => _gameModel.RightEyePosition;
            set
            {
                if (_gameModel.RightEyePosition != value)
                {
                    _gameModel.RightEyePosition = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public bool IsLaserActive
        {
            get => _gameModel.IsLaserActive;
            set
            {
                if (_gameModel.IsLaserActive != value)
                {
                    _gameModel.IsLaserActive = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public Point[] LightParticles => _gameModel.LightParticles;
        
        public bool IsPlayerColliding
        {
            get => _gameModel.IsColliding;
            set
            {
                if (_gameModel.IsColliding != value)
                {
                    _gameModel.IsColliding = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public double PlayerOpacity
        {
            get => IsPlayerColliding ? 0.5 : 1.0;
        }
        
        public Point LeftLaserEndPoint
        {
            get => _gameModel.LeftLaserEndPoint;
            set
            {
                if (_gameModel.LeftLaserEndPoint != value)
                {
                    _gameModel.LeftLaserEndPoint = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public Point RightLaserEndPoint
        {
            get => _gameModel.RightLaserEndPoint;
            set
            {
                if (_gameModel.RightLaserEndPoint != value)
                {
                    _gameModel.RightLaserEndPoint = value;
                    OnPropertyChanged();
                }
            }
        }
        
        // 命令
        public ICommand StartGameCommand { get; }
        public ICommand MoveLeftCommand { get; }
        public ICommand MoveRightCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        
        public MainViewModel()
        {
            _gameModel = new GameModel();
            _eyeDetector = new EyeDetector();
            
            // 初始化命令
            StartGameCommand = new RelayCommand(StartGame);
            MoveLeftCommand = new RelayCommand(_ => MovePlayer(-20, 0));
            MoveRightCommand = new RelayCommand(_ => MovePlayer(20, 0));
            MoveUpCommand = new RelayCommand(_ => MovePlayer(0, -20));
            MoveDownCommand = new RelayCommand(_ => MovePlayer(0, 20));
            
            // 设置游戏计时器
            _gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50) // 20 FPS
            };
            _gameTimer.Tick += GameLoop;
            
            // 直接初始化眼睛位置（使用EyeDetector中的固定坐标）
            _eyeDetector.DetectEyesFromImage("dummy"); // 这会设置固定的眼睛位置
            LeftEyePosition = _eyeDetector.LeftEyePosition;
            RightEyePosition = _eyeDetector.RightEyePosition;
        }
        
        private void StartGame(object parameter)
        {
            // 重置游戏状态（包括设置玩家位置）
            _gameModel.ResetGame();
            Score = 0;
            
            // 更新UI绑定
            OnPropertyChanged(nameof(Lives));
            OnPropertyChanged(nameof(Health));
            OnPropertyChanged(nameof(PlayerPosition));
            OnPropertyChanged(nameof(LightParticles));
            OnPropertyChanged(nameof(LeftLaserEndPoint));
            OnPropertyChanged(nameof(RightLaserEndPoint));
            
            // 启动游戏循环
            IsGameRunning = true;
            _gameTimer.Start();
            
            // 立即生成一次激光，确保激光系统正常工作
            _gameModel.ActivateLaser();
            IsLaserActive = true;
            
            // 强制更新所有相关属性
            OnPropertyChanged(nameof(IsLaserActive));
            OnPropertyChanged(nameof(LeftLaserEndPoint));
            OnPropertyChanged(nameof(RightLaserEndPoint));
            
            // 输出调试信息
            Console.WriteLine("游戏开始时激活激光:");
            Console.WriteLine($"左眼位置: {LeftEyePosition}, 终点: {LeftLaserEndPoint}");
            Console.WriteLine($"右眼位置: {RightEyePosition}, 终点: {RightLaserEndPoint}");
            Console.WriteLine($"激光状态: {IsLaserActive}");
            
            // 3秒后关闭初始激光
            System.Threading.Tasks.Task.Delay(3000).ContinueWith(_ => 
            {
                if (IsGameRunning)
                {
                    IsLaserActive = false;
                    OnPropertyChanged(nameof(IsLaserActive));
                    Console.WriteLine("初始激光已关闭");
                }
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }
        
        // 激光计时器
        private double _laserTimer = 0;
        private double _laserCooldown = 0;
        private const double LASER_DURATION = 2.0; // 激光持续2秒
        private const double LASER_COOLDOWN = 3.0; // 激光冷却3秒
        
        private void GameLoop(object sender, EventArgs e)
        {
            if (_gameModel.IsGameOver)
            {
                EndGame();
                return;
            }
            
            // 计算时间增量（假设每帧16.67毫秒，即60FPS）
            double deltaTime = 0.01667;
            
            // 更新游戏模型
            _gameModel.Update(deltaTime);
            
            // 更新激光计时器
            if (_gameModel.IsLaserActive)
            {
                _laserTimer += 0.05; // 每帧增加50毫秒
                
                // 更新激光位置（使激光左右移动）
                _gameModel.UpdateLaserPosition(deltaTime);
                
                // 更新UI绑定
                OnPropertyChanged(nameof(LeftLaserEndPoint));
                OnPropertyChanged(nameof(RightLaserEndPoint));
                
                // 如果激光持续时间已到，停止激光
                if (_laserTimer >= LASER_DURATION)
                {
                    _gameModel.IsLaserActive = false;
                    _laserTimer = 0;
                    _laserCooldown = 0;
                    
                    // 通知UI更新激光状态
                    OnPropertyChanged(nameof(IsLaserActive));
                    Console.WriteLine("激光持续时间已到，停止激光");
                }
            }
            else
            {
                // 更新冷却时间
                _laserCooldown += 0.05;
                
                // 冷却时间已到，有机会生成新激光
                if (_laserCooldown >= LASER_COOLDOWN && _random.Next(100) < 30) // 30%的几率激活激光
                {
                    _gameModel.ActivateLaser();
                    _laserTimer = 0;
                    
                    // 更新激光终点属性，触发UI更新
                    OnPropertyChanged(nameof(IsLaserActive));
                    OnPropertyChanged(nameof(LeftLaserEndPoint));
                    OnPropertyChanged(nameof(RightLaserEndPoint));
                    
                    // 调试输出
                    Console.WriteLine("激光已在GameLoop中激活! 左眼终点: " + LeftLaserEndPoint + ", 右眼终点: " + RightLaserEndPoint);
                }
            }
            
            // 当激光不活跃时，生成更多光点
            if (!_gameModel.IsLaserActive)
            {
                // 大幅增加光点生成几率
                if (_random.Next(100) < 25) // 25%的几率
                {
                    _gameModel.GenerateLightParticles();
                }
            }
            else
            {
                // 激光活跃时，减少光点生成
                if (_random.Next(100) < 5) // 5%的几率
                {
                    _gameModel.GenerateLightParticles();
                }
            }
            
            // 更新光点位置
            _gameModel.UpdateLightParticles();
            OnPropertyChanged(nameof(LightParticles));
            
            // 检测碰撞
            if (_gameModel.CheckCollisions())
            {
                // 更新UI绑定
                OnPropertyChanged(nameof(Lives));
                OnPropertyChanged(nameof(Health));
                OnPropertyChanged(nameof(IsPlayerColliding));
                
                // 更新玩家透明度
                OnPropertyChanged(nameof(PlayerOpacity));
                
                // 播放碰撞音效
                SoundManager.Instance.PlayCollisionSound();
            }
            else if (_gameModel.IsColliding)
            {
                // 如果之前在碰撞但现在不再碰撞，更新状态
                _gameModel.IsColliding = false;
                OnPropertyChanged(nameof(IsPlayerColliding));
            }
            
            // 增加分数
            Score++;
        }
        
        private void EndGame()
        {
            _gameTimer.Stop();
            IsGameRunning = false;
            MessageBox.Show($"游戏结束！你的得分：{Score}");
        }
        
        private void MovePlayer(int deltaX, int deltaY)
        {
            if (!IsGameRunning) return;
            
            _gameModel.MovePlayer(deltaX, deltaY);
            OnPropertyChanged(nameof(PlayerPosition));
        }
    }
}