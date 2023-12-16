using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlackHoleSim
{
    public partial class Form1 : Form
    {
        List<XP> particles = new List<XP>();
        Bitmap bmp;
        float windowWidth = 0;
        float windowHeight = 0;
        //rs - Schwarzschild radius of a black hole
        //dt - time step of numerical integration
        //n - amount of time steps in one iteration
        static double rs = 25;
        static double dt = 0.2;
        static int n = 20;
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(Canvas.Width, Canvas.Height);
            Random random = new Random();

            /*
            for (int _ = 0; _ < 2000; _++)
            {
                XP xp = new XP(2);
                xp.x[0] = 300;
                xp.x[1] = (float)i / 20;
                xp.p[0] = -1;
                xp.p[1] = 0;
                xp.m = 0;
                xps.Add(xp);
            }
            */
            for (var _ = 0; _ < 100; _++)
                {
                    XP xp = new XP(2);
                    xp.x[0] = gaussianRandom(random) * 10 + 150;
                    xp.x[1] = gaussianRandom(random) * 10;
                    xp.p[0] = gaussianRandom(random) * 0.02;
                    xp.p[1] = gaussianRandom(random) * 0.02 + 0.3;
                    xp.m = 1;
                    particles.Add(xp);
                }
            for (var _ = 0; _ < 100; _++)
                {
                    XP xp = new XP(2);
                    xp.x[0] = gaussianRandom(random) * 10 + 100;
                    xp.x[1] = gaussianRandom(random) * 10;
                    xp.p[0] = gaussianRandom(random) * 0.02;
                    xp.p[1] = gaussianRandom(random) * 0.02 + 0.3;
                    xp.m = 1;
                    particles.Add(xp);
                }
            for (var _ = 0; _ < 100; _++)
                {
                    XP xp = new XP(2);
                    xp.x[0] = gaussianRandom(random) * 10 + 400;
                    xp.x[1] = gaussianRandom(random) * 10 - 100;
                    xp.p[0] = gaussianRandom(random) * 0.04 - 0.5;
                    xp.p[1] = gaussianRandom(random) * 0.04;
                    xp.m = 1;
                    particles.Add(xp);
                }
            for (var _ = 0; _ < 100; _++)
                {
                    XP xp = new XP(2);
                    xp.x[0] = gaussianRandom(random) * 10 + 250;
                    xp.x[1] = gaussianRandom(random) * 10;
                    xp.p[0] = gaussianRandom(random) * 0.04;
                    xp.p[1] = gaussianRandom(random) * 0.04;
                    xp.m = 1;
                    particles.Add(xp);
                }
            }
        //rendering particles and event horizon in the window
        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (windowWidth != Canvas.Width || windowHeight != Canvas.Height)
            {
                bmp = new Bitmap(Canvas.Width, Canvas.Height);
                windowWidth = Canvas.Width;
                windowHeight = Canvas.Height;
                g = Graphics.FromImage(bmp);
                g.Clear(Color.Black);
            }
            g.FillRectangle(new SolidBrush(Color.FromArgb(20, Color.Black)),0,0,windowWidth,windowHeight);
            g.DrawArc(new Pen(Color.White, 2), (float)(windowWidth / 2 - rs), (float)(windowHeight / 2 - rs), (float)(2 * rs), (float)(2 * rs), 0, 360);
            float r = 2;
            foreach(XP xp in particles)
                g.FillEllipse(xp.m == 0 ? Brushes.Yellow: Brushes.White, (float)(windowWidth / 2 + xp.x[0] - r), (float)(windowHeight / 2 - xp.x[1] - r), 2 * r, 2 * r);
            e.Graphics.DrawImage(bmp, 0, 0);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            Parallel.ForEach(particles, xp =>
            {
                XP xp1 = new XP(2);
                //Positions and momenta of the particles are updated according to Hamilton's equations to the second order of accuracy.
                for (int _ = 0; _ < n; _++)
                {
                    xp1.x[0] = xp.x[0] + XP.dtx(0, H).Value(xp) * dt + XP.d2tx(0, H).Value(xp) * dt * dt / 2;
                    xp1.x[1] = xp.x[1] + XP.dtx(1, H).Value(xp) * dt + XP.d2tx(1, H).Value(xp) * dt * dt / 2;
                    xp.x[0] = xp1.x[0];
                    xp.x[1] = xp1.x[1];
                    xp1.p[0] = xp.p[0] + XP.dtp(0, H).Value(xp) * dt + XP.d2tp(0, H).Value(xp) * dt * dt / 2;
                    xp1.p[1] = xp.p[1] + XP.dtp(1, H).Value(xp) * dt + XP.d2tp(1, H).Value(xp) * dt * dt / 2;
                    xp.p[0] = xp1.p[0];
                    xp.p[1] = xp1.p[1];
                }
                //staying near the event horizon for too long causes the particle's momentum to increase too much,
                //leading to numerical instabilities and non-physical predictions,
                //so the particle is effectively removed from the simulation when it gets close enough to the event horizon
                if (r(xp.x[0], xp.x[1]) <= rs + 1)
                    xp.m = double.NaN;
            });
            Canvas.Invalidate();
        }

        Observable H = new Observable(new Observable.function(xp => Hf(xp)));
        //Hamiltonian function, which is equal to the time component of the 4-momentum of the particle,
        //derived from the length of the 4-momentum in a given metric
        static double Hf(XP xp1)
        {
            double x = xp1.x[0];
            double y = xp1.x[1];
            double xp = xp1.p[0];
            double yp = xp1.p[1];
            double m = xp1.m;
            var g = g_cartesian(x, y);
            var k1 = g.tt;
            var k2 = 2 * (g.tx * xp + g.ty * yp);
            var k3 = g.xx * xp * xp + g.yy * yp * yp + 2 * g.xy * xp * yp - m * m;
            return (-k2 + Math.Sqrt(k2 * k2 - 4 * k1 * k3)) / (2 * k1);
        }
        //distance from the coordinate origin
        static double r(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }
        //Schwarzschild metric tensor in cartesian coordinates (t,x,y), derived from polar coordinates
        static (double tt, double xx, double yy, double tx, double ty, double xy) g_cartesian(double x, double y)
        {
            double a = x * x;
            double b = y * y;
            double c = a + b;
            double r = Math.Sqrt(c);
            var g = g_polar(r);
            return (
                tt: g.tt,
                xx: (g.rr * a + g.ff * b) / c,
                yy: (g.rr * b + g.ff * a) / c,
                tx: g.tf * y / r,
                ty: -g.tf * x / r,
                xy: (g.rr - g.ff) * x * y / c
                );
        }
        //Schwarzschild metric tensor in polar coordinates (t,r,f), where f basis vector is normalized
        static (double tt, double rr, double ff, double tf) g_polar(double r)
        {
            return (
                tt: 1 / (1 - rs / r),
                rr: -(1 - rs / r),
                ff: -1d,
                tf: 0d
                );
        }
        static double gaussianRandom(Random r, double mean = 0, double stdev = 1)
        {
            double u = 1.0 - r.NextDouble();
            double v = r.NextDouble();
            double z = Math.Sqrt(-2.0 * Math.Log(u)) * Math.Cos(2.0 * Math.PI * v);
            return z * stdev + mean;
        }
    }
}
