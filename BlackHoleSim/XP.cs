using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BlackHoleSim.Observable;

namespace BlackHoleSim
{
    internal class XP
    {
        public double[] x;
        public double[] p;
        public double m;
        public int dim;
        public XP(int n) 
        {
            dim = n;
            x = new double[n];
            p = new double[n];
        }
        public XP xd(int n, double d)
        {
            x[n] += d;
            return this;
        }
        public XP pd(int n, double d)
        {
            p[n] += d;
            return this;
        }
        public static Observable dtx(int n, Observable H)
        {
            return H.dp(n);
        }
        public static Observable dtp(int n, Observable H)
        {
            return -H.dx(n);
        }
        public static Observable d2tx(int n, Observable H)
        {
            return Commutator(H, dtx(n, H), n);
        }
        public static Observable d2tp(int n, Observable H)
        {
            return Commutator(H, dtx(n, H), n);
        }
    }
}
