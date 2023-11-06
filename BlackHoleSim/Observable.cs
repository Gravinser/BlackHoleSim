using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlackHoleSim
{
    internal class Observable
    {
        public delegate double function(XP xp);
        function F;
        const double d = 0.000006;
        public Observable(function f)
        {
            F = f;
        }
        public static implicit operator Observable(double a)
        {
            return new Observable(new function(xp => a));
        }
        public double Value(XP xp)
        {
            return F(xp);
        }
        public static Observable operator +(Observable A, Observable B)
        {
            return new Observable(new function(xp => A.F(xp) + B.F(xp)));
        }
        public static Observable operator -(Observable A, Observable B)
        {
            return new Observable(new function(xp => A.F(xp) - B.F(xp)));
        }
        public static Observable operator -(Observable A)
        {
            return new Observable(new function(xp => -A.F(xp)));
        }
        public static Observable operator *(Observable A, Observable B)
        {
            return new Observable(new function(xp => A.F(xp) * B.F(xp)));
        }
        public static Observable operator /(Observable A, Observable B)
        {
            return new Observable(new function(xp => A.F(xp) / B.F(xp)));
        }
        public Observable dx(int n)
        {
            return new Observable(new function(xp => (F(xp.xd(n, d)) - F(xp.xd(n, -d))) / (2 * d)));
        }
        public Observable dp(int n)
        {
            return new Observable(new function(xp => (F(xp.pd(n, d)) - F(xp.pd(n, -d))) / (2 * d)));
        }
        public static Observable PartialCommutator(Observable A, Observable B, int n)
        {
            return A.dp(n) * B.dx(n) - A.dx(n) * B.dp(n);
        }
        public static Observable Commutator(Observable A, Observable B, int n)
        {
            Observable C = 0;
            for (int i = 0; i < n; i++)
                C += PartialCommutator(A, B, i);
            return C;
        }
    }
}
