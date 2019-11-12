﻿//MIT, 2014-present, WinterDev

namespace PixelFarm.Drawing
{
    public struct Point
    {
        int _x, _y;
        public Point(int x, int y)
        {
            _x = x;
            _y = y;
        }
        public void Offset(int dx, int dy)
        {
            _x += dx;
            _y += dy;
        }
        public int X
        {
            get => _x;
            set => _x = value;
        }

        public int Y
        {
            get => _y;
            set => _y = value;
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1._x == p2._x &&
                   p1._y == p2._y;
        }
        public static bool operator !=(Point p1, Point p2)
        {
            return p1._x != p2._x ||
                   p1._y != p2._y;
        }
        public override bool Equals(object obj)
        {
            Point p2 = (Point)obj;
            return _x == p2._x &&
                   _y == p2._y;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public int x { get { return _x; } } //temp
        public int y { get { return _y; } } //temp
        public static readonly Point Empty = new Point();
#if DEBUG
        public override string ToString()
        {
            return "(" + this.X + "," + this.Y + ")";
        }
#endif
    }

    public struct PointF
    {
        float _x, _y;
        public PointF(float x, float y)
        {
            _x = x;
            _y = y;
        }
        public float X
        {
            get => _x;
            set => _x = value;
        }
        public float Y
        {
            get => _y;
            set => _y = value;
        }
        public static implicit operator PointF(Point p)
        {
            return new PointF(p.X, p.Y);
        }
        public bool IsEq(PointF p)
        {
            return _x == p._x && _y == p._y;
        }
        public void Offset(float dx, float dy)
        {
            _x += dx;
            _y += dy;
        }

#if DEBUG
        public override string ToString()
        {
            return "(" + this.X + "," + this.Y + ")";
        }
#endif
    }

    public struct Size
    {
        int _w, _h;
        public Size(int w, int h)
        {
            _w = w;
            _h = h;
        }
        public int Width
        {
            get => _w;
            set => _w = value;
        }
        public int Height
        {
            get => _h;
            set => _h = value;
        }
        public static bool operator ==(Size s1, Size s2)
        {
            return (s1._w == s2._w) &&
                  (s1._h == s2._h);
        }
        public static bool operator !=(Size s1, Size s2)
        {
            return (s1._w != s2._w) ||
                  (s1._h != s2._h);
        }
        public override bool Equals(object obj)
        {
            Size s2 = (Size)obj;
            return (_w == s2._w) &&
                   (_h == s2._h);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static readonly Size Empty = new Size();

#if DEBUG
        public override string ToString()
        {
            return "(" + _w + "," + _h + ")";
        }
#endif
    }

    public struct SizeF
    {
        float _w, _h;
        public SizeF(float w, float h)
        {
            _w = w;
            _h = h;
        }
        //
        public float Width => _w;
        public float Height => _h;
        //
        public static implicit operator SizeF(Size p)
        {
            return new SizeF(p.Width, p.Height);
        }

#if DEBUG
        public override string ToString()
        {
            return "(" + _w + "," + _h + ")";
        }
#endif
    }
}