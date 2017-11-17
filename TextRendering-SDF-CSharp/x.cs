using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;

using SharpFont;

using System.Timers;

using static System.Console;
using static System.Math;

public class __Main__
{
    // ====================================================================
    //                             Common Tools
    // ====================================================================
    
    static R[] Mapping<R, T>(Func<T, R> F, T[] source)
        where R : struct
        where T : struct
    {
        var ret = new R[source.Length];
        for(int i=0; i<source.Length; i++)
            ret[i] = F(source[i]);
        return ret;
    }
    
    static T[] Filter<T>(Func<T, bool> Pass, T[] source)
        where T : struct
    {
        int cnt = 0;
        foreach(var i in source) if(Pass(i)) cnt++;
        var res = new T[cnt];
        foreach(var i in source) if(Pass(i)) res[--cnt] = i;
        return res;
    }
    
    // ====================================================================
    //                         Common Interfaces
    // ====================================================================
    
    interface IDrawableOnBitmap
    {
        void DrawToBitmap(Bitmap g, Color color, double scale, bool directed = false);
    }
    
    interface ICurve : IDrawableOnBitmap
    {
        Point from { get; }
        Point to { get; }
        Point Interpolate(double x);
        double DistanceTo(Point p);
    }
    
    // ====================================================================
    //                          Common Mathematics
    // ====================================================================
    
    static double eps = 1e-10; // different from double.epsilon, this is the controller of loops.
    static bool eq(double a, double b) { return Abs(a - b) < eps; }
    
    struct Point : IDrawableOnBitmap
    {
        public double x;
        public double y;
        public Point(double _x, double _y) { x = _x; y = _y; }
        public Point(FTVector v) : this(v.X, v.Y) { }
        public static Point operator+(Point a, Point b) { return new Point(a.x + b.x, a.y + b.y); }
        public static Point operator-(Point a, Point b) { return new Point(a.x - b.x, a.y - b.y); }
        public static Point operator-(Point a) { return new Point(-a.x, -a.y); }
        public static Point operator*(Point a, Point b) { return new Point(a.x * b.x, a.y * b.y); }
        public static Point operator/(Point a, Point b) { return new Point(a.x / b.x, a.y / b.y); }
        public static Point operator*(Point a, double b) { return new Point(a.x * b, a.y * b); }
        public static Point operator*(double b, Point a) { return new Point(a.x * b, a.y * b); }
        public static Point operator/(Point a, double b) { return new Point(a.x / b, a.y / b); }
        public override bool Equals(object obj) { return obj is Point ? this == (Point)obj : false; }
        public override int GetHashCode() { throw new NotImplementedException(); }
        public static bool operator==(Point a, Point b) { return eq(a.x, b.x) && eq(a.y, b.y); }
        public static bool operator!=(Point a, Point b) { return !(a == b); }
        public static double Dot(Point a, Point b) { return a.x * b.x + a.y * b.y; }
        public double Cross(Point a) { return x * a.y - y * a.x; }
        public static double Distance(Point a, Point b) { return Sqrt((b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y)); }
        public static double DistanceSqr(Point a, Point b) { return (b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y); }
        public double length { get { return Distance(this, original); } }
        public static Point original { get{ return new Point(0, 0); } }
        public override string ToString() => ToString("0.000000");
        public string ToString(string format) { return "(" + x.ToString(format) + "," + y.ToString(format) + ")"; }
    
        public static double Angle(Point observer, Point from, Point to)
        {
            return Asin((from - observer).Cross(to - observer) / (from - observer).length / (to - observer).length); 
        }
        
        public void DrawToBitmap(Bitmap output, Color color, double scale, bool directed = false) 
            => DrawToBitmap(output, color, scale, 1.0);
        public void DrawToBitmap(Bitmap output, Color color, double scale, double radius)
        {
            Point src = NearestIntegerPoint(this * scale);
            for(int x = (int)Floor(src.x - radius); x <= (int)Ceiling(src.x + radius); x++)
                for(int y = (int)Floor(src.y - radius); y <= (int)Ceiling(src.y + radius); y++)
                {
                    if(0 <= x && x < output.Width && 0 <= y && y < output.Height)
                        if(Point.Distance(new Point(x, y), this * scale) <= radius)
                        {
                            output.SetPixel(x, y, color);
                        }
                }
        }
    }
    
    /// <summary>
    /// Input equation in a syntax of x^2 + b x + c.
    /// </summary>
    static double[] SolveSquare(double a, double b, double c)
    {
        double delta = b * b - 4 * a * c;
        if(delta < 0) return new double[]{};
        if(eq(delta, 0)) return new double[]{-b / (2*a)};
        double q = Sqrt(delta);
        return new double[]{(-b+q)/(2*a), (-b-q)/(2*a)};
    }
    
    /// <summary>
    /// To get a zero point bewtween L to R. 
    /// Assume F(L) F(R) < 0, so we have a zero point according to *the existence theorem of zero point*.
    /// </summary>
    static double GetZeroPointInMonotoneInterval(double L, double R, Func<double, double> F) // auto capture a, b, c.
    {
        double f1 = F(L);
        double f2 = F(R);
        if(eq(f1, 0)) return L;
        if(eq(f2, 0)) return R;
        if(Sign(f1) == Sign(f2)) return double.NaN; // It should be an imaginary root.
        
        // Make f1 < f2, then we can assume f1 < 0, f2 > 0.
        double l = L, r = R;
        if(f1 > f2) { l = R; r = L; }
        while(Abs(r - l) >= eps)
        {
            double mid = (r + l) * 0.5;
            double res = F(mid);
            if(eq(res, 0)) return mid;
            if(res > 0)
                r = mid;
            else
                l = mid;
        }
        return (l + r) * 0.5;
    }
    
    static double[] SolveCubic(double a, double b, double c, double d)
    {
        // The cubic function calculation.
        double F(double x) { return a * x * x * x + b * x * x + c * x + d; }
        
        
        double delta = b*b - 3*a*c;
        if(delta > 0) // The cubic function has two extreme point.
        {
            double exL = -1e10; // Should have not exceeded...
            double ex1 = (-b - Sqrt(b * b - 3 * a * c)) / (3 * a);
            double ex2 = (-b + Sqrt(b * b - 3 * a * c)) / (3 * a);
            double exR = 1e10; // Should have not exceeded...
            double s1 = GetZeroPointInMonotoneInterval(exL, ex1, F);
            double s2 = GetZeroPointInMonotoneInterval(ex1, ex2, F);
            double s3 = GetZeroPointInMonotoneInterval(ex2, exR, F);
            int g = (s1 != double.NaN ? 1 : 0) + (s2 != double.NaN ? 1 : 0) + (s3 != double.NaN ? 1 : 0);
            var p = new double[g];
            int t = 0;
            if(s1 != double.NaN) p[t++] = s1;
            if(s2 != double.NaN) p[t++] = s2;
            if(s3 != double.NaN) p[t++] = s3;
            return p;
        }
        else // The cubic function has at most one extreme point, indicates the function is Monotony.
        {
            double L = -1e10;
            double R = 1e10;
            return new double[]{GetZeroPointInMonotoneInterval(L, R, F)};
        }
    }
    
    
    // ====================================================================
    //                     Tool Functions for Shapes
    // ====================================================================
    
    /// <summary>
    /// Intersections includes *from* points, exclude *to* points.
    /// </summary>
    static Point[] GetIntersections(ConicBezier F, Segment S) => GetIntersections(S, F);
    static Point[] GetIntersections(Segment S, ConicBezier F)
    {
        // Segment To y = kx + b format...
        double k = (S.to.y - S.from.y) / (S.to.x - S.from.x);
        if(eq(1.0 / k, eps)) // slope is infinity, or near infinity.
        {
            return Mapping<Point, double>(
                F.Interpolate,
                Filter<double>(
                    x => (0.0 <= x && x < 1.0 && S.CoveringPoint(F.Interpolate(x))),
                    SolveSquare(F.from.x - S.from.x, 2.0 * (F.control.x - F.from.x), F.from.x + F.to.x - 2.0 * F.control.x)));
        }
        else
        {
            double b = S.to.y - k * S.to.x;
            Point N = F.from + F.to - 2.0 * F.control;
            Point M = 2.0 * (F.control - F.from);
            Point Q = F.from;
            double A = k * N.x - N.y;
            double B = k * M.x - M.y;
            double C = k * Q.x - Q.y + b;
            return Mapping<Point, double>(
                F.Interpolate,
                Filter<double>(
                    x => (0.0 <= x && x <= 1.0 && S.CoveringPoint(F.Interpolate(x))),
                    SolveSquare(A, B, C)));
        }
    }
    static Point[] GetIntersections(Segment S, Segment T)
    {
        Point ds = S.to - S.from;
        Point dt = T.to - T.from;
        if(eq(ds.Cross(dt), 0)) return new Point[]{};
        Point bs = S.from;
        Point bt = T.from;
        Point b = bt - bs;
        double t1 = b.Cross(-dt) / ds.Cross(-dt);
        double t2 = ds.Cross(b) / ds.Cross(-dt);
        if(0.0 <= t2 && t2 <= 1.0 && 0.0 <= t1 && t1 <= 1.0) return new Point[]{T.Interpolate(t2)};
        else return new Point[]{};
    }
    
    /// <summary>
    /// For convenience, replacing specific methods.
    /// </summary>
    static Point[] Intersections(ICurve A, ICurve B)
    {
        if(A is Segment && B is Segment) return GetIntersections((Segment)A, (Segment)B);
        if(A is Segment && B is ConicBezier) return GetIntersections((Segment)A, (ConicBezier)B);
        if(A is ConicBezier && B is Segment) return GetIntersections((ConicBezier)A, (Segment)B);
        throw new MissingMethodException();
    }
    
    static Point NearestIntegerPoint(Point cur)
    {
        Point[] p = new Point[4]{
            new Point(Ceiling(cur.x), Ceiling(cur.y)),
            new Point(Ceiling(cur.x), Floor(cur.x)),
            new Point(Floor(cur.x), Ceiling(cur.y)),
            new Point(Floor(cur.x), Floor(cur.y))};
    
        int k = 0;
        for(int s=1; s<4; s++)
            if((cur - p[s]).length < (cur - p[k]).length)
                k = s;
        
        return p[k];
    }
    
    // ====================================================================
    //                            Shape support
    // ====================================================================
    
    struct Segment : ICurve
    {
        public Point from { get; set; }
        public Point to { get; set; }
        public Segment(Point f, Point t) { from = f; to = t; }
        public Segment(FTVector f, FTVector t) : this(new Point(f), new Point(t)) { }
        
        public Point Interpolate(double x)
        {
            return from + (to - from) * x;
        }
        
        public double DistanceTo(Point p)
        {
            double d1 = Point.DistanceSqr(from, p);
            double d2 = Point.DistanceSqr(to, p);
            double d3 = double.PositiveInfinity;
            double f1 = Point.Dot(p - from, to - from);
            double f2 = Point.Dot(p - to, from - to);
            if(f1 > 0 && f2 > 0)
                d3 = Abs((p - from).Cross(to - from)) / (to - from).length;
            return Min(Sqrt(Min(d1, d2)), d3);
        }
        
        public bool CoveringPoint(Point p)
        { 
            return eq((from - p).length + (to - p).length, (to - from).length);
        }
        
        public void DrawToBitmap(Bitmap g, Color color, double scale, bool directed = false)
        {
            int count = (int)Ceiling(2.0 * (from - to).length * scale);
            for(int i=0; i<=count; i++)
            {
                Point t = NearestIntegerPoint(Interpolate((double)i / count) * scale);
                if(0.0 <= t.x && t.x < g.Width && 0.0 <= t.y && t.y < g.Height)
                    g.SetPixel((int)t.x, (int)t.y,
                        directed ?
                            Color.FromArgb(
                                (int)(((double)i / count) * color.A),
                                (int)(((double)i / count) * color.R),
                                (int)(((double)i / count) * color.G),
                                (int)(((double)i / count) * color.B)
                            ) :
                            color);
            }
        }
        
        public override string ToString() { return String.Format("Segment[from:{0} to:{1}]", from, to); }
    }
    
    struct ConicBezier : ICurve
    {
        public Point from { get; set; }
        public Point control { get; set; }
        public Point to { get; set; }
        
        public ConicBezier(Point _from, Point _contorol, Point _to)
        {
            from = _from;
            control = _contorol;
            to = _to;
        }
        
        public ConicBezier(FTVector f, FTVector c, FTVector t) : this(new Point(f), new Point(c), new Point(t)) { }
        
        public Point Interpolate(double t)
        {
            double r = 1 - t;
            return from * r * r + 2 * control * r * t + to * t * t;
        }
        
        public double DistanceTo(Point p)
        {
            // The square of disatnce can be represented by this expression, using vector's self-dot-multiply:
            // > (from * (1-t)^2 + 2 * control * t * (1-t) + to * t^2 - p)^2,
            // where "from", "control", "to" and "p" are vectors representing points of this curve.
            // Expend the derivative of this expression into:
            // > (from - p + 2 * (control - from) * t + (from + to - 2 * control) * t^2)
            // > DotMultiply
            // > (control - from + (from + to - 2 * control) * t)
            // Solve this cubic equation to get all extreme points.
            // Points "from" and "to" plus extreme points are the only possible solutions.
            Point N = from - p;
            Point M = control - from;
            Point K = from + to - 2.0 * control;
            double a = Point.Dot(K, K);
            double b = 3.0 * Point.Dot(K, M);
            double c = Point.Dot(N, K) + 2.0 * Point.Dot(M, M);
            double d = Point.Dot(N, M);
            var sol = SolveCubic(a, b, c, d);
            double res = double.PositiveInfinity;
            foreach(var i in sol)
                if(0.0 <= i && i <= 1.0)
                    res = Min(res, Point.Distance(Interpolate(i), p));
            res = Min(res, Point.Distance(Interpolate(0), p));
            res = Min(res, Point.Distance(Interpolate(1), p));
            return res;
        }
        
        public void DrawToBitmap(Bitmap g, Color color, double scale, bool directed = false)
        {
            int count = (int)Ceiling(scale * (2.0 * ((control - from).length + (to - control).length + (from - to).length)));
            for(int i=0; i<=count; i++)
            {
                Point t = NearestIntegerPoint(Interpolate((double)i / count) * scale);
                if(0.0 <= t.x && t.x < g.Width && 0.0 <= t.y && t.y < g.Height)
                    g.SetPixel((int)t.x, (int)t.y,
                        directed ?
                            Color.FromArgb(
                                (int)(((double)i / count) * color.A),
                                (int)(((double)i / count) * color.R),
                                (int)(((double)i / count) * color.G),
                                (int)(((double)i / count) * color.B)
                            ) :
                            color);
            }
        }
        
        public override string ToString() { return String.Format("ConicBezier[from:{0} control:{1} to:{2}]", from, control, to); }
    }
    
    /// <summary>
    /// Should be nammed Outline...
    /// Duplicated with SharpFont...
    /// </summary>
    class ClosedCurve
    {
        readonly LinkedList<ICurve> _curves = new LinkedList<ICurve>();
        public LinkedList<ICurve> curves { get{ wingingStored = false; return _curves; } }
        
        /// <summary>
        /// Winging is a number indicates how the curve is rotated.
        /// Put a point P *inside* the curve, and a point T walk through the curve.
        /// As T walking, the polar angle of segment P->T is changing.
        /// As T comlete the walking, total changing of polar angle of P->T is a multiply of 2*Pi.
        /// The winging number is that angle divided by 2*Pi.
        /// So a counter-clockwise circle's winging is 1.
        /// A clockwisw circle's winging is -1.
        /// As a curve that does not self-intersect, the winging is always -1 or 1.
        /// </summary>
        int _winging = 0;
        bool wingingStored = false;
        public int winging
        {
            get
            {
                if(wingingStored) return _winging;
                
                // We assume that each sub-curve will not self-intersect and will not intersect with each other.
                // So we can simply count the winging using the end points.
                // Then, we divide all sub-curves into at least 2 parts,
                // so that angle of each pairs of consistant points will not exeeded Pi.
                // Here we make it 3 parts...
                double p = 0.0;
                Point t = curves.First.Value.from;
                foreach(var i in curves)
                {
                    Point cf = i.Interpolate(1 / 3.0);
                    Point ct = i.Interpolate(2 / 3.0);
                    if(t != i.from && t != cf) p += Point.Angle(t, i.from, cf);
                    if(t != cf && t != ct) p += Point.Angle(t, cf, ct);
                    if(t != ct && t != i.to) p += Point.Angle(t, ct, i.to);
                }
                if(p == 0.0) _winging = 0; // p is not changed; not a valid curve...
                else _winging = p > 0 ? 1 : -1;
                wingingStored = true;
                return _winging;
            }
        }
        
        /// <summary>
        /// true if p is inside the *Positive Area*,
        /// where *Positive Area* is defined *outside* the curve when connected conter-clockwise (winging > 0),
        /// and is defined inside the curve when connected clockwise (winging < 0).
        /// Using ray testing.
        /// This method is valid as long as curves are not self-intersecting.
        /// </summary>
        public bool PositiveAreaContains(Point p)
        {
            LinkedList<Point> g = new LinkedList<Point>(); 
            Segment s = new Segment(p, p + new Point(1e2, 1e2));
            foreach(var i in curves)
            {
                foreach(var t in Intersections(s, i))
                    g.AddLast(t);
            }
            
            if(g.Count != 0)
            {
                // remove duplicated points.
                LinkedListNode<Point> h = g.First.Next;
                while(h != null)
                {
                    if(h.Previous.Value == h.Value) g.Remove(h.Previous); // O(1) operation.
                    h = h.Next;
                }
            }
            
            if(g.Count >= 2 && g.First.Value == g.Last.Value) g.RemoveFirst();
            int w = winging;
            if(w == 0) return false; // not a valid curve...
            return (w < 0) ^ ((g.Count & 1) == 0);
        }
    }
    
    // ====================================================================
    //                             Main Process
    // ====================================================================

    static string Vec2Str(FTVector v) { return "(" + v.X.ToString("0.0000") + "," + v.Y.ToString("0.0000") + ")"; }
    
    public static void Main(string[] args)
    {
        // ===========================================================
        LinkedList<ClosedCurve> GetOutlines(char ch)
        {
            Library lib = new Library();
            Face face = new Face(lib, "./LiberationMono-Regular.ttf", 0);
            face.LoadChar(ch, LoadFlags.NoScale, LoadTarget.Normal);
            LinkedList<ClosedCurve> outlines = new LinkedList<ClosedCurve>();
            LinkedListNode<ClosedCurve> curOutline = null;
            
            // Simply use lambda functions.
            // Use IntPtr.Zero to avoid using unsafe section.
            FTVector last = new FTVector(0, 0);
            face.Glyph.Outline.Decompose(new OutlineFuncs(
                // Move To...
                (ref FTVector v, IntPtr z) =>
                {
                    // WriteLine("Move to: " + Vec2Str(v));
                    curOutline = outlines.AddLast(new ClosedCurve());
                    last = v;
                    return 0;
                },
                // Line To...
                (ref FTVector v, IntPtr z) =>
                {
                    // WriteLine("Create: " + new Segment(last, v));
                    curOutline.Value.curves.AddLast(new Segment(last, v));
                    last = v;
                    return 0;
                },
                // Conic To...
                (ref FTVector c, ref FTVector v, IntPtr z) =>
                {
                    // WriteLine("Create: " + new ConicBezier(last, c, v));
                    curOutline.Value.curves.AddLast(new ConicBezier(last, c, v));
                    last = v;
                    return 0;
                },
                // Cubic To...
                (ref FTVector c, ref FTVector s, ref FTVector v, IntPtr z) =>
                {
                    // Cubic Bezier looks not supported by true type...
                    // Make it no sence.
                    throw new NotImplementedException();
                },
                0, 0), IntPtr.Zero);
            return outlines;
        }
        
        // ===========================================================
        void DrawDirectedOutlines(LinkedList<ClosedCurve> outlines, Bitmap output, double scale)
        {
            foreach(var l in outlines)
            {
                int winging = l.winging;
                Color color = winging > 0 ? Color.FromArgb(255, 255, 0, 0) : Color.FromArgb(255, 0, 255, 0);
                Parallel.ForEach(l.curves, x =>
                {
                    x.DrawToBitmap(output, color, scale, true);
                });
            }
        }
        
        // ===========================================================
        /// <param name="p">Point in *Glyph coordinates*. parameter scale is not affect this.</param>
        void DrawIntersections(LinkedList<ClosedCurve> outlines, Bitmap output, Segment s, double scale)
        {
            var ts = new Segment(s.from / scale, s.to / scale);
            ts.DrawToBitmap(output, Color.FromArgb(255, 100, 100, 255), scale);
            LinkedList<Point> intersections = new LinkedList<Point>();
            foreach(var l in outlines)
            {
                foreach(var ot in l.curves)
                {
                    foreach(var its in Intersections(ot, ts))
                    {
                        intersections.AddLast(its);
                        its.DrawToBitmap(output, Color.FromArgb(255, 255, 255, 0), scale, 5);
                    }
                }
            }
        }
        // ===========================================================
        void DrawInnerArea(LinkedList<ClosedCurve> outlines, Bitmap output, double scale)
        {
            Parallel.For(0, output.Width, x =>
            {
                for(int y=0; y<output.Height; y++)
                {
                    bool able = true;
                    foreach(var otl in outlines)
                    {
                        if(!otl.PositiveAreaContains(new Point(x / scale, y / scale)))
                        {
                            able = false;
                            break;
                        }
                    }
                    if(able) output.SetPixel(x, y, Color.FromArgb(255, 60, 60, 60));
                }
            });
        }
        
        // ===========================================================
        void DrawDistanceField(LinkedList<ClosedCurve> outlines, Bitmap output, Point origin, double scale, double lower, double upper)
        {
            Parallel.For(0, output.Width, x =>
            { 
                for(int y = 0 ; y < output.Height; y++)
                {
                    Point p = (new Point(x, y) - origin) / scale;
                    double dist = double.PositiveInfinity;
                    bool inside = true;
                    foreach(var otl in outlines)
                    {
                        foreach(var cv in otl.curves)
                        {
                            dist = Min(dist, cv.DistanceTo(p));
                        }
                        if(!otl.PositiveAreaContains(p)) inside = false;
                    }
                    if(inside) dist = -dist;
                    if(dist * scale > upper) dist = upper / scale;
                    if(dist * scale < lower) dist = lower / scale;
                    int val = 255 - (int)(255 * (dist * scale - lower) / (upper - lower));
                    output.SetPixel(x, y, Color.FromArgb(255, 0, 0, val));
                }
            });
        }
        // ===========================================================
        void DrawTextUsingDistanceField(Bitmap dst, Bitmap src)
        {
            int Get(Point k)
            {
                if(k.x < 0 || src.Width - 1 < k.x || k.y < 0 || src.Height - 1 < k.y) return 0;
                return src.GetPixel((int)k.x, (int)k.y).B;
            };
            
            Parallel.For(0, dst.Width, x =>
            {
                for(int y = 0; y < dst.Height; y++)
                {
                    Point d = new Point(x, y);
                    Point s = d / new Point(dst.Width, dst.Height) * new Point(src.Width, src.Height) - new Point(.5, .5);
                    Point lb = new Point(Floor(s.x), Floor(s.y));
                    Point lt = new Point(Floor(s.x), Floor(s.y) + 1);
                    Point rb = new Point(Floor(s.x) + 1, Floor(s.y));
                    Point rt = new Point(Floor(s.x) + 1, Floor(s.y) + 1);
                    double b = Get(lb) * (rb.x - s.x) + Get(rb) * (s.x - lb.x);
                    double t = Get(lt) * (rt.x - s.x) + Get(rt) * (s.x - lt.x);
                    double dep = b * (lt.y - s.y) + t * (s.y - lb.y);
                    if(dep > 127.5) dst.SetPixel(x, y, Color.FromArgb(255, 0, (int)dep, 0));
                }
            });
        }
        
        // ===========================================================
        // ===========================================================
        double timestep = 1e-3;
        double timerCount = 0.0;
        var timer = new System.Timers.Timer(timestep);
        timer.Elapsed += (object obj, ElapsedEventArgs e) => { timerCount += timestep; };
        timer.Start();
        
        // ===========================================================
        {
            Bitmap op = new Bitmap(512, 512, PixelFormat.Format32bppArgb);
            var g = GetOutlines('D');
            double sc = 20000;
            double t1 = timerCount;
            
            DrawInnerArea(g, op, sc);
            double t2 = timerCount;
            WriteLine("Time used by DrawInnerArea: {0}", (t2 - t1).ToString("0.000"));
            
            DrawIntersections(g, op, new Segment(new Point(97, 200), new Point(1100, 60)), sc);
            double t3 = timerCount;
            WriteLine("Time used by DrawIntersections: {0}", (t3 - t2).ToString("0.000"));
            
            DrawDirectedOutlines(g, op, sc);
            double t4 = timerCount;
            WriteLine("Time used by DrawDirectedOutlines: {0}", (t4 - t3).ToString("0.000"));
            
            op.RotateFlip(RotateFlipType.RotateNoneFlipY);
            op.Save("./test.bmp", ImageFormat.Bmp);
            op.Dispose();
            WriteLine("test.bmp done.");
        }
        // ===========================================================
        {
            Bitmap op = new Bitmap(64, 64, PixelFormat.Format32bppArgb);
            double sc = 1900;
            var g = GetOutlines('B');
            timerCount = 0.0;
            DrawDistanceField(g, op, new Point(12, 12), sc, -12, 12);
            WriteLine("Time used : {0}", timerCount.ToString("0.000"));
            op.RotateFlip(RotateFlipType.RotateNoneFlipY);
            op.Save("./x.bmp", ImageFormat.Bmp);
            op.Dispose();
            WriteLine("x.bmp done.");
        }
        // ===========================================================
        {
            Bitmap op = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);
            Bitmap sr = new Bitmap("./x.bmp");
            timerCount = 0.0;
            DrawTextUsingDistanceField(op, sr);
            WriteLine("Time used : {0}", timerCount.ToString("0.000"));
            op.Save("./output.bmp", ImageFormat.Bmp);
            op.Dispose();
            WriteLine("output.bmp done.");
        }
        timer.Stop();
    }
}
