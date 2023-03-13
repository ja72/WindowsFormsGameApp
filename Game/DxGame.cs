using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.Drawing.Drawing2D;

namespace JA.Game
{
    using System.Diagnostics;
    using static Geometry;

    public class DxGame
    {
        public event EventHandler Update;

        public DxGame(Control target)
        {
            Ball = new Ball();
            Paddle = new Paddle();
            Bricks = new List<Brick>();
            Target = target;
            Timer = new Timer();
            Timer.Enabled = false;
            Gravity = new Vector2(0, 1);
            target.Resize += (s, ev) => OnResize();
            target.Paint += (s, ev) => OnDraw(ev.Graphics);
            target.MouseMove += (s, ev) => MouseMove(ev.Location, ev.Button);
            target.MouseClick += (s, ev) => MouseClick(ev.Location, ev.Button);
            target.FindForm().KeyDown += (s, ev) => KeyDown(ev.KeyCode);
            Timer.Interval = 15;
            Timer.Tick += (s, ev) => OnUpdate(Timer.Interval / 100f);
            Timer.Stop();
            OnResize();
            SetupLevel();
        }

        #region Game
        public void SetupLevel()
        {
            Score = 0f;
            Time = 0f;

            Paddle.Center = new Vector2(PlaySize.X / 2, PlaySize.Y - Paddle.Size.Y);
            PaddleTarget = Paddle.Center;
            Ball.Center = new Vector2(Paddle.Center.X, Paddle.Top - Ball.Radius);
            Ball.Velocity = PolarDegrees(16f, -110);
            Ball.Fixed = false;

            const int n_rows = 8;
            const int n_cols = 8;

            float dx = PlaySize.X / n_cols, dy = 0.5f * PlaySize.Y / n_rows;

            for (int i = 0; i < n_rows; i++)
            {
                float y = dy * (i + 0.5f);

                for (int j = 0; j < n_cols; j++)
                {
                    float x = dx * (j + 0.5f);

                    var brick = new Brick(Color.Magenta, Math.Max(1, 3 - i))
                    {
                        Center = new Vector2(x, y),
                    };

                    Bricks.Add(brick);
                }
            }
            Update?.Invoke(this, new EventArgs());
        }
        public void OnUpdate(float elapsed)
        {
            Time += elapsed;
            if (!Ball.Fixed)
            {
                Ball.Velocity += elapsed * Gravity;
                Ball.Center += elapsed * Ball.Velocity;

                Ball.ImpactPlane(Vector2.Zero, Vector2.UnitX, Vector2.Zero, 0.96f, 0);
                Ball.ImpactPlane(Vector2.Zero, Vector2.UnitY, Vector2.Zero, 1, 0);
                Ball.ImpactPlane(new Vector2(PlaySize.X, 0), -Vector2.UnitX, Vector2.Zero, 0.96f, 0);
                Ball.ImpactPlane(new Vector2(0, PlaySize.Y), -Vector2.UnitY, Vector2.Zero, 0.92f, 0);
            }


            float λ = .45f;
            //float vx = (1 - λ) * Paddle.Velocity.X + λ * (PaddleTarget - Paddle.Center.X) / elapsed;
            //Paddle.Velocity = new Vector2(vx, 0);
            Vector2 v = (1 - λ) * Paddle.Velocity + λ * (PaddleTarget - Paddle.Center) / elapsed;
            Paddle.Velocity = v;
            Paddle.Center += elapsed * Paddle.Velocity;

            if (Paddle.Left < 0)
            {
                Paddle.Center = new Vector2(Paddle.Center.X - Paddle.Left, Paddle.Center.Y);
            }
            if (Paddle.Right > PlaySize.X)
            {
                Paddle.Center = new Vector2(Paddle.Center.X - (Paddle.Right - PlaySize.X), Paddle.Center.Y);
            }

            foreach (var brick in Bricks)
            {
                if (!brick.Fixed && brick.Visible)
                {
                    brick.Velocity += elapsed * Gravity;
                    brick.Center += elapsed * brick.Velocity;

                    if (brick.Bottom > PlaySize.Y)
                    {
                        brick.Visible = false;
                        brick.Fixed = true;
                    }
                }
                if (brick.Visible && brick.Intersects(Ball, out var point2, out var normal2))
                {
                    if (brick.Strength >= 0)
                    {
                        Score += 100 / (1 + brick.Strength);
                    }

                    brick.Strength = Math.Max(0, brick.Strength - 1);
                    brick.Fixed = brick.Strength > 0;
                    var Dv = Ball.ImpactPlane(point2, normal2, Vector2.Zero, 1.2f, 0.1f);
                    brick.Velocity -= Dv / 16;
                }
            }
            for (int i = Bricks.Count - 1; i >= 0; i--)
            {
                if (Bricks[i].Visible == false)
                {
                    Bricks.RemoveAt(i);
                }
            }

            if (Bricks.Count == 0)
            {
                Timer.Enabled = false;
            }

            if (!Ball.Fixed && Paddle.Intersects(Ball, out var point, out var normal))
            {
                Ball.ImpactPlane(point, normal, Paddle.Velocity, 1.2f, 0.2f);
            }

            if (Ball.Velocity.Length() > 25)
            {
                Ball.Velocity -= 0.1f * Ball.Velocity;
            }

            Update?.Invoke(this, new EventArgs());

            Target.Invalidate();
        } 
        #endregion

        #region User Interactions
        private void KeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.Space:
                    Timer.Enabled = !Timer.Enabled;                    
                    Update?.Invoke(this, new EventArgs());
                    break;
                case Keys.Escape:
                    Timer.Enabled = false;
                    this.Target.FindForm().Close();
                    break;
            }
        }
        private void MouseMove(Point location, MouseButtons button)
        {
            Vector2 point = GetPoint(location);
            PaddleTarget = new Vector2(point.X, 
                Math.Max(PlaySize.Y - Paddle.Size.Y - 6,
                Math.Min(PlaySize.Y - Paddle.Size.Y, point.Y)));
            if (IsPaused)
            {
                Paddle.Center = PaddleTarget; 
                Paddle.Velocity = Vector2.Zero;

                if (Time == 0)
                {
                    Ball.Center = new Vector2(Paddle.Center.X, Paddle.Top - Ball.Radius);
                }

                Target.Invalidate();
            }
        }
        private void MouseClick(Point location, MouseButtons button)
        {
            if (Time == 0 && button == MouseButtons.Left)
            {
                Timer.Enabled = true;
                Update?.Invoke(this, new EventArgs());
            }
            else if (button == MouseButtons.Left)
            {
                // do something on mouse click, like a
                // ball boost 
                Ball.Radius = 4f;
            }
            else
            {
                Ball.Radius = 3;
            }
        } 
        #endregion

        #region Properties
        public bool IsPaused { get => !Timer.Enabled; }
        public float Time { get; private set; }
        public float Score { get; private set; }
        public Timer Timer { get; }
        public Control Target { get; }
        public Ball Ball { get; }
        public Paddle Paddle { get; }
        public Vector2 PaddleTarget { get; set; }
        public List<Brick> Bricks { get; }
        public Vector2 PlaySize { get; private set; }
        public float Scale { get; private set; }
        public Vector2 Gravity { get; set; }
        #endregion

        #region Drawing
        public PointF GetPixel(Vector2 point)
            => new PointF(Scale * point.X, Scale * point.Y);
        public SizeF GetSpan(Vector2 span)
            => new SizeF(Scale * span.X, Scale * span.Y);
        public Vector2 GetPoint(Point pixel)
            => new Vector2(pixel.X / Scale, pixel.Y / Scale);
        public void OnDraw(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            if (!Timer.Enabled)
            {
                const string text = "Press SPACE to Play.";
                SizeF sz = graphics.MeasureString(text, Target.Font);
                graphics.DrawString(text, Target.Font, Brushes.Red, Target.Width/2-sz.Width/2, Target.Height/2-sz.Height/2);
            }


            if (Paddle.Visible)
            {
                Paddle.Draw(graphics, this);
            }
            if (Ball.Visible)
            {
                Ball.Draw(graphics, this);
            }
            foreach (var brick in Bricks)
            {
                if (brick.Visible)
                {
                    brick.Draw(graphics, this);
                }
            }
        }
        public void OnResize()
        {
            Scale = Math.Min((Target.ClientSize.Width - 1) / 100, (Target.ClientSize.Height - 1) / 100);
            PlaySize = new Vector2(Target.ClientSize.Width / Scale, Target.ClientSize.Height / Scale);
            Target.Invalidate();
        }
        #endregion

    }
}
