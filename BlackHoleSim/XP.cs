using static BlackHoleSim.Observable;

namespace BlackHoleSim
{
    //This class contains information about particle's position, momentum and mass
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
        //time derivative of x-nth according to Hamilton's equations
        public static Observable dtx(int n, Observable H)
        {
            return H.dp(n);
        }
        //time derivative of p-nth according to Hamilton's equations
        public static Observable dtp(int n, Observable H)
        {
            return -H.dx(n);
        }
        //second time derivative of x-nth according to Hamilton's equations
        public static Observable d2tx(int n, Observable H)
        {
            return Commutator(H, dtx(n, H), n);
        }
        //second time derivative of p-nth according to Hamilton's equations
        public static Observable d2tp(int n, Observable H)
        {
            return Commutator(H, dtx(n, H), n);
        }
    }
}
