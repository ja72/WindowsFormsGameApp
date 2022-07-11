using System;
using System.Drawing;
using System.Numerics;

namespace JA.Game
{
    public interface IBounce
    {
        Vector2 GetClosestPointTo(Vector2 target, out Vector2 normal);
        bool Intersects(Ball ball, out Vector2 point, out Vector2 normal);
    }

    public abstract class BaseSprite : IDisposable
    {
        protected BaseSprite(Color color)
        {
            Pen = new Pen(color, 0);
            Fill = new SolidBrush(color);
            Color = color;
            Fixed = true;
            Visible = true;
        }

        public Color Color
        {
            get => Pen.Color;
            set
            {
                Pen.Color = value;
                Fill.Color = Color.FromArgb(128, value);
            }
        }
        public SolidBrush Fill { get; }
        public Pen Pen { get; }
        public bool Visible { get; set; }
        public bool Fixed { get; set; }

        /// <summary>
        /// Draws the object.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="game">The game object.</param>
        public abstract void Draw(Graphics graphics, DxGame game);

        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Pen.Dispose();
                    Fill.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
