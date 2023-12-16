namespace BlackHoleSim
{
    //In Hamiltonian mechanics all physical quantities are represented as functions of coordinates and momenta,
    //this class helps to work with functions as physical quantities and perform operations on them 
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
        //returns the approximate partial derivative of a physical quantity with respect to x-nth
        public Observable dx(int n)
        {
            return new Observable(new function(xp => (F(xp.xd(n, d)) - F(xp.xd(n, -d))) / (2 * d)));
        }
        //returns the approximate partial derivative of a physical quantity with respect to p-nth
        public Observable dp(int n)
        {
            return new Observable(new function(xp => (F(xp.pd(n, d)) - F(xp.pd(n, -d))) / (2 * d)));
        }
        public static Observable PartialCommutator(Observable A, Observable B, int n)
        {
            return A.dp(n) * B.dx(n) - A.dx(n) * B.dp(n);
        }
        //Evaluation of Poisson bracket of two physical quantities A and B
        public static Observable Commutator(Observable A, Observable B, int n)
        {
            Observable C = 0;
            for (int i = 0; i < n; i++)
                C += PartialCommutator(A, B, i);
            return C;
        }
    }
}
