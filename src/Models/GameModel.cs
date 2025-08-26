using System;
using System.Drawing;

namespace EyeLaserGame.Models
{
    public class GameModel
    {
        // 眼睛位置
        public Point LeftEyePosition { get; set; }
        public Point RightEyePosition { get; set; }
        
        // 玩家状态
        public int Lives { get; set; } = 3;
        public int Health { get; set; } = 100;
        public bool IsColliding { get; set; } = false;
        
        // 游戏状态
        public bool IsGameOver => Lives <= 0;
        
        // 碰撞状态
        public bool IsPlayerColliding { get; set; } = false;
        
        // 碰撞恢复计时器
        private double _collisionRecoveryTimer = 0;
        public bool IsRecoveringFromCollision { get; private set; } = false;
        
        // 激光计时器
        private double _laserTimer = 0;
        private double _laserDuration = 2.0; // 激光持续2秒
        
        // 游戏窗口大小
        public int GameWidth { get; set; } = 720;  // 与窗口宽度匹配
        public int GameHeight { get; set; } = 990; // 与窗口高度匹配
        
        // 玩家位置
        public Point PlayerPosition { get; set; }
        
        // 激光/光点状态
        public bool IsLaserActive { get; set; }
        public Point[] LightParticles { get; set; }
        
        // 激光方向
        public double LeftLaserAngle { get; set; }
        public double RightLaserAngle { get; set; }
        
        // 激光终点
        public Point LeftLaserEndPoint { get; set; }
        public Point RightLaserEndPoint { get; set; }
        
        public GameModel()
        {
            // 初始化玩家位置在屏幕底部中央
            PlayerPosition = new Point(GameWidth / 2, GameHeight - 50);
            
            // 初始化光点数组 - 增加光点数量
            LightParticles = new Point[30];
            for (int i = 0; i < LightParticles.Length; i++)
            {
                LightParticles[i] = new Point(-1, -1); // 初始位置在屏幕外
            }
            
            // 初始化激光终点
            LeftLaserEndPoint = new Point(0, 0);
            RightLaserEndPoint = new Point(0, 0);
        }
        
        /// <summary>
        /// 更新眼睛位置
        /// </summary>
        public void UpdateEyePositions(Point leftEye, Point rightEye)
        {
            LeftEyePosition = leftEye;
            RightEyePosition = rightEye;
        }
        
        /// <summary>
        /// 移动玩家
        /// </summary>
        public void MovePlayer(int deltaX, int deltaY)
        {
            int newX = PlayerPosition.X + deltaX;
            int newY = PlayerPosition.Y + deltaY;
            
            // 确保玩家不会移出屏幕左右边界
            if (newX < 0)
                newX = 0;
            else if (newX > GameWidth)
                newX = GameWidth;
            
            // 计算眼睛位置的Y坐标平均值
            int eyeY = (LeftEyePosition.Y + RightEyePosition.Y) / 2;
            
            // 确保玩家只能在眼睛下方100px的区域内移动
            int minY = eyeY + 100;
            int maxY = GameHeight - 30; // 留一些边距
            
            // 调整玩家Y坐标，确保不会超出上下限制
            newY = Math.Max(minY, Math.Min(maxY, newY));
                
            PlayerPosition = new Point(newX, newY);
        }
        
        // 类成员随机数生成器
        private readonly Random _random = new Random();
        
        // 激光移动相关变量
        private double _leftLaserMoveSpeed = 0;
        private double _rightLaserMoveSpeed = 0;
        private const double MAX_LASER_MOVE_SPEED = 5.0;
        
        /// <summary>
        /// 激活激光攻击
        /// </summary>
        public void ActivateLaser()
        {
            IsLaserActive = true;
            
            // 左眼激光角度：-30度到30度之间的随机角度
            LeftLaserAngle = _random.Next(-30, 31);
            
            // 右眼激光角度：-30度到30度之间的随机角度
            RightLaserAngle = _random.Next(-30, 31);
            
            // 设置激光移动速度（每帧移动的角度）
            _leftLaserMoveSpeed = (_random.NextDouble() * 2 - 1) * MAX_LASER_MOVE_SPEED;
            _rightLaserMoveSpeed = (_random.NextDouble() * 2 - 1) * MAX_LASER_MOVE_SPEED;
            
            // 计算激光终点
            CalculateLaserEndPoints();
            
            // 确保激光是可见的
            Console.WriteLine($"激活激光: 左眼({LeftEyePosition.X},{LeftEyePosition.Y}) -> ({LeftLaserEndPoint.X},{LeftLaserEndPoint.Y})");
            Console.WriteLine($"激活激光: 右眼({RightEyePosition.X},{RightEyePosition.Y}) -> ({RightLaserEndPoint.X},{RightLaserEndPoint.Y})");
            
            // 强制激光可见
            IsLaserActive = true;
        }
        
        /// <summary>
        /// 更新激光位置（使激光左右移动）
        /// </summary>
        public void UpdateLaserPosition(double deltaTime = 1.0)
        {
            if (!IsLaserActive) return;
            
            // 更新激光角度
            LeftLaserAngle += _leftLaserMoveSpeed * deltaTime;
            RightLaserAngle += _rightLaserMoveSpeed * deltaTime;
            
            // 限制激光角度范围
            if (LeftLaserAngle > 45)
            {
                LeftLaserAngle = 45;
                _leftLaserMoveSpeed *= -1; // 反向移动
            }
            else if (LeftLaserAngle < -45)
            {
                LeftLaserAngle = -45;
                _leftLaserMoveSpeed *= -1; // 反向移动
            }
            
            if (RightLaserAngle > 45)
            {
                RightLaserAngle = 45;
                _rightLaserMoveSpeed *= -1; // 反向移动
            }
            else if (RightLaserAngle < -45)
            {
                RightLaserAngle = -45;
                _rightLaserMoveSpeed *= -1; // 反向移动
            }
            
            // 重新计算激光终点
            CalculateLaserEndPoints();
        }
        
        /// <summary>
        /// 计算激光终点坐标
        /// </summary>
        private void CalculateLaserEndPoints()
        {
            // 延长激光的距离，使其更明显
            double distance = GameHeight * 2; // 使用更长的距离
            
            // 计算左眼激光终点 - 修正角度计算
            double leftRadians = LeftLaserAngle * Math.PI / 180;
            // 使用正确的三角函数：sin用于X方向，cos用于Y方向
            double leftEndX = LeftEyePosition.X + Math.Sin(leftRadians) * distance;
            // 注意：在屏幕坐标系中，Y轴向下为正，所以这里用加号
            double leftEndY = LeftEyePosition.Y + Math.Cos(leftRadians) * distance;
            
            // 确保激光终点在屏幕范围内
            leftEndY = Math.Min(leftEndY, GameHeight * 2); // 允许更长的激光
            
            LeftLaserEndPoint = new Point((int)leftEndX, (int)leftEndY);
            
            // 计算右眼激光终点 - 修正角度计算
            double rightRadians = RightLaserAngle * Math.PI / 180;
            double rightEndX = RightEyePosition.X + Math.Sin(rightRadians) * distance;
            double rightEndY = RightEyePosition.Y + Math.Cos(rightRadians) * distance;
            
            // 确保激光终点在屏幕范围内
            rightEndY = Math.Min(rightEndY, GameHeight * 2); // 允许更长的激光
            
            RightLaserEndPoint = new Point((int)rightEndX, (int)rightEndY);
            
            // 调试输出
            Console.WriteLine($"计算激光终点: 左眼({LeftEyePosition.X},{LeftEyePosition.Y}) -> ({LeftLaserEndPoint.X},{LeftLaserEndPoint.Y})");
            Console.WriteLine($"计算激光终点: 右眼({RightEyePosition.X},{RightEyePosition.Y}) -> ({RightLaserEndPoint.X},{RightLaserEndPoint.Y})");
        }
        
        /// <summary>
        /// 停止激光攻击
        /// </summary>
        public void DeactivateLaser()
        {
            IsLaserActive = false;
            Console.WriteLine("激光已停用");
        }
        
        // 存储光点的速度向量
        private Point[] _lightParticleVelocities;
        
        /// <summary>
        /// 生成光点攻击
        /// </summary>
        public void GenerateLightParticles()
        {
            // 如果速度数组未初始化，则初始化它
            if (_lightParticleVelocities == null)
            {
                _lightParticleVelocities = new Point[LightParticles.Length];
            }
            
            // 使用类成员随机数生成器
            int particlesToGenerate = _random.Next(2, 5); // 每次生成2-4个光点
            int particlesGenerated = 0;
            
            // 从眼睛位置生成光点
            for (int i = 0; i < LightParticles.Length && particlesGenerated < particlesToGenerate; i++)
            {
                // 如果光点不在屏幕上（-1, -1），则激活它
                if (LightParticles[i].X == -1 && LightParticles[i].Y == -1)
                {
                    // 随机选择左眼或右眼作为起点
                    Point startPoint = _random.Next(2) == 0 ? LeftEyePosition : RightEyePosition;
                    
                    // 设置光点初始位置为眼睛位置
                    LightParticles[i] = startPoint;
                    
                    // 设置光点的随机速度向量（向下扩散）
                    int velocityX = _random.Next(-5, 6); // -5到5的随机X速度（增加水平扩散）
                    int velocityY = _random.Next(4, 9);  // 4到8的随机Y速度（确保向下移动且速度更快）
                    _lightParticleVelocities[i] = new Point(velocityX, velocityY);
                    
                    particlesGenerated++;
                }
            }
        }
        
        /// <summary>
        /// 更新光点位置
        /// </summary>
        public void UpdateLightParticles()
        {
            // 如果速度数组未初始化，则初始化它
            if (_lightParticleVelocities == null)
            {
                _lightParticleVelocities = new Point[LightParticles.Length];
            }
            
            for (int i = 0; i < LightParticles.Length; i++)
            {
                // 如果光点在屏幕上
                if (LightParticles[i].X != -1 && LightParticles[i].Y != -1)
                {
                    // 根据速度向量移动光点
                    int newX = LightParticles[i].X + _lightParticleVelocities[i].X;
                    int newY = LightParticles[i].Y + _lightParticleVelocities[i].Y;
                    
                    // 如果光点移出屏幕，重置它
                    if (newY > GameHeight || newX < 0 || newX > GameWidth)
                    {
                        LightParticles[i] = new Point(-1, -1);
                        _lightParticleVelocities[i] = new Point(0, 0);
                    }
                    else
                    {
                        LightParticles[i] = new Point(newX, newY);
                    }
                }
            }
        }
        
        /// <summary>
        /// 检测碰撞
        /// </summary>
        public bool CheckCollisions()
        {
            bool collision = false;
            bool wasColliding = IsPlayerColliding;
            
            // 如果正在从碰撞中恢复，不检测新的碰撞
            if (IsRecoveringFromCollision)
            {
                return false;
            }
            
            // 检查激光碰撞
            if (IsLaserActive)
            {
                // 左眼激光碰撞检测
                if (IsPointNearLine(PlayerPosition, LeftEyePosition, LeftLaserEndPoint, 15))
                {
                    TakeDamage(10);
                    collision = true;
                    IsColliding = true;
                }
                
                // 右眼激光碰撞检测
                if (IsPointNearLine(PlayerPosition, RightEyePosition, RightLaserEndPoint, 15))
                {
                    TakeDamage(10);
                    collision = true;
                    IsColliding = true;
                }
            }
            
            // 检查光点碰撞
            foreach (var particle in LightParticles)
            {
                if (particle.X != -1 && particle.Y != -1)
                {
                    // 简单的碰撞检测：如果光点和玩家距离很近
                    double distance = Math.Sqrt(Math.Pow(particle.X - PlayerPosition.X, 2) + 
                                               Math.Pow(particle.Y - PlayerPosition.Y, 2));
                    if (distance < 20)
                    {
                        TakeDamage(5);
                        collision = true;
                        IsColliding = true;
                    }
                }
            }
            
            // 更新碰撞状态
            IsPlayerColliding = collision;
            
            // 如果发生碰撞，启动恢复计时器
            if (collision)
            {
                IsRecoveringFromCollision = true;
                _collisionRecoveryTimer = 0;
            }
            
            return collision;
        }
        
        /// <summary>
        /// 检查点是否靠近线段
        /// </summary>
        private bool IsPointNearLine(Point point, Point lineStart, Point lineEnd, double maxDistance)
        {
            // 计算线段长度的平方
            double lineLength2 = Math.Pow(lineEnd.X - lineStart.X, 2) + Math.Pow(lineEnd.Y - lineStart.Y, 2);
            
            if (lineLength2 == 0) // 线段长度为0
                return false;
                
            // 计算点到线段的投影比例
            double t = ((point.X - lineStart.X) * (lineEnd.X - lineStart.X) + 
                       (point.Y - lineStart.Y) * (lineEnd.Y - lineStart.Y)) / lineLength2;
            
            // 限制t在[0,1]范围内，确保我们只考虑线段上的点
            t = Math.Max(0, Math.Min(1, t));
            
            // 计算投影点
            double projX = lineStart.X + t * (lineEnd.X - lineStart.X);
            double projY = lineStart.Y + t * (lineEnd.Y - lineStart.Y);
            
            // 计算点到投影点的距离
            double distance = Math.Sqrt(Math.Pow(point.X - projX, 2) + Math.Pow(point.Y - projY, 2));
            
            // 如果距离小于阈值，则认为点靠近线段
            return distance < maxDistance;
        }
        
        /// <summary>
        /// 玩家受到伤害
        /// </summary>
        public void TakeDamage(int amount)
        {
            Health -= amount;
            
            // 如果血量归零，减少一条命
            if (Health <= 0)
            {
                Lives--;
                
                // 如果还有命，重置血量
                if (Lives > 0)
                {
                    Health = 100;
                }
            }
        }
        
        /// <summary>
        /// 重置游戏
        /// </summary>
        public void ResetGame()
        {
            Lives = 3;
            Health = 100;
            
            // 计算眼睛位置的Y坐标平均值
            int eyeY = (LeftEyePosition.Y + RightEyePosition.Y) / 2;
            
            // 设置玩家初始位置在眼睛下方100px处
            int playerY = eyeY + 100;
            
            // 确保玩家不会超出屏幕底部
            playerY = Math.Min(playerY, GameHeight - 50);
            
            PlayerPosition = new Point(GameWidth / 2, playerY);
            IsLaserActive = false;
            
            // 重置所有光点
            for (int i = 0; i < LightParticles.Length; i++)
            {
                LightParticles[i] = new Point(-1, -1);
            }
            
            // 确保激光终点初始化
            LeftLaserEndPoint = new Point(LeftEyePosition.X, GameHeight);
            RightLaserEndPoint = new Point(RightEyePosition.X, GameHeight);
        }
        
        /// <summary>
        /// 更新游戏状态
        /// </summary>
        public void Update(double deltaTime)
        {
            // 更新激光计时器
            if (IsLaserActive)
            {
                _laserTimer += deltaTime;
                if (_laserTimer >= _laserDuration)
                {
                    DeactivateLaser();
                }
                else
                {
                    // 更新激光位置
                    UpdateLaserPosition();
                }
            }
            
            // 更新碰撞恢复计时器
            if (IsRecoveringFromCollision)
            {
                _collisionRecoveryTimer += deltaTime;
                if (_collisionRecoveryTimer >= 0.5) // 500毫秒
                {
                    IsRecoveringFromCollision = false;
                    IsPlayerColliding = false;
                    _collisionRecoveryTimer = 0;
                }
            }
        }
    }
}
