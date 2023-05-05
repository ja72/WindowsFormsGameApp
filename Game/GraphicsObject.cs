using System;
using System.Drawing;
using System.Numerics;

namespace JA.Game
{

    public abstract class GraphicsObject : IDisposable
    {
        protected GraphicsObject(Color color)
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

        /// <summary>
        /// Gets the closest point on the object to the target point.
        /// </summary>
        /// <param name="target">The target point.</param>
        /// <param name="normal">The normal vector at the resulting point.</param>
        /// <returns>A point.</returns>
        public abstract Vector2 GetClosestPointTo(Vector2 target, out Vector2 normal);

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
