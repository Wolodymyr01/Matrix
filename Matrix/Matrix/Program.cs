using System;
using static System.Console;
using static System.Convert;

namespace Matrix
{
    public enum CreationType : byte
    {
        Empty, Random, Own
    }
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Keywords: stop, random, empty\nMatrix A: enter size");
            Matrix A, B;
            string s = null;
            while (s != "stop")
            {
                try
                {
                    args = ReadLine().Split();
                    int n = ToInt32(args[0]), m = ToInt32(args[1]);
                    WriteLine("Random, empty or your own?");
                    switch (ReadLine())
                    {
                        case "empty":
                            A = new Matrix(n, m, CreationType.Empty);
                            break;
                        case "random":
                            A = new Matrix(n, m, CreationType.Random);
                            break;
                        default:
                            args = new string[n];
                            for (int i = 0; i < n; i++)
                            {
                                args[i] = ReadLine();
                            }
                            A = new Matrix(n, m, CreationType.Own, args);
                            break;
                    }
                    WriteLine($"A = \n{A.ToString()}");
                    WriteLine("Matrix B: enter size");
                    args = ReadLine().Split();
                    n = ToInt32(args[0]); m = ToInt32(args[1]);
                    WriteLine("Random, empty or your own?");
                    switch (ReadLine())
                    {
                        case "empty":
                            B = new Matrix(n, m, CreationType.Empty);
                            break;
                        case "random":
                            B = new Matrix(n, m, CreationType.Random);
                            break;
                        default:
                            args = new string[n];
                            for (int i = 0; i < n; i++)
                            {
                                args[i] = ReadLine();
                            }
                            B = new Matrix(n, m, CreationType.Own, args);
                            break;
                    }
                    WriteLine($"B = \n{B.ToString()}");
                    WriteLine("Enter operator");
                    switch (s = ReadLine())
                    {
                        case "+":
                            WriteLine($"A + B =\n{(A + B).ToString()}");
                            break;
                        case "-":
                            WriteLine($"A - B =\n{(A - B).ToString()}");
                            break;
                        case "*":
                            WriteLine($"A * B =\n{(A * B).ToString()}");
                            break;
                        default:
                            throw new ApplicationException("Wrong operator");
                    }
                    WriteLine("Enter a number");
                    s = ReadLine();
                    double b;
                    WriteLine($"{b = ToDouble(s)}*A =\n{A * b}");
                }
                catch (FormatException)
                {
                    WriteLine("Format exception. Try again");
                    continue;
                }
                catch (IndexOutOfRangeException)
                {
                    WriteLine("IndexOutOfRangeException. Try again");
                    continue;
                }
                catch (InvalidMatrixOperationException e)
                {
                    WriteLine(e.Message);
                    continue;
                }
            }
        }
    }
    public class InvalidMatrixOperationException : ApplicationException
    {
        public InvalidMatrixOperationException() : base()
        {

        }
        public InvalidMatrixOperationException(string message) : base(message)
        {

        }
        public InvalidMatrixOperationException(Matrix A, Matrix B, string message) : base(message)
        {
            n = A.n; m = A.m;
            k = B.n; l = B.m;
        }
        public readonly int n, m, k, l;
    }
    public class Randomizer
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                return random.Next(min, max);
            }
        }
    }
    public class Matrix : ICloneable
    {
        public Matrix(int n, int m, CreationType ct, params string[] args)
        {
            this.n = n; this.m = m;
            Elem = new double[n][];
            for (int i = 0; i < n; i++)
            {
                Elem[i] = new double[m];
            }
            if (ct == CreationType.Own)
            {
                if (args != null) Init(args);
                else throw new ApplicationException();
            }
            else Init(ct);
        }
        Matrix(Matrix instance)
        {
            n = instance.n; m = instance.m;
            Elem = (double[][])instance.Elem.Clone();
        }
        public readonly int n, m;
        void Init(CreationType ct)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    switch (ct)
                    {
                        case CreationType.Empty:
                            Elem[i][j] = 0;
                            break;
                        case CreationType.Random:
                            Elem[i][j] = Randomizer.RandomNumber(0, 1000);
                            break;
                        default:
                            throw new ApplicationException();
                    }
                }
            }
        }
        void Init(string[] args)
        {
            string[] pars; double[] arr;
            for (int i = 0; i < n; i++)
            {
                pars = args[i].Split();
                arr = new double[pars.Length];
                for (int j = 0; j < arr.Length; j++)
                {
                    double.TryParse(pars[j], out arr[j]);
                }
                Elem[i] = arr;
            }
        }
        public double[][] Elem { get; private set; }
        public object Clone()
        {
            return new Matrix(this) as object;
        }
        public static Matrix operator +(Matrix A, Matrix B)
        {
            if (A.n != B.n || A.m != B.m)
            {
                var e = new InvalidMatrixOperationException(A, B, $"Matrix must have equal number of columns and raws." +
                    $"A({A.n}, {A.m}), B({B.n},{B.m})");
                throw e;
            }
            Matrix C = A.Clone() as Matrix;
            for (int i = 0; i < C.n; i++)
            {
                for (int j = 0; j < C.m; j++)
                {
                    C.Elem[i][j] += B.Elem[i][j];
                }
            }
            return C;
        }
        public static Matrix operator -(Matrix A, Matrix B)
        {
            if (A.n != B.n || A.m != B.m)
            {
                var e = new InvalidMatrixOperationException(A, B, $"Matrix must have equal number of columns and raws." +
                    $"A({A.n}, {A.m}), B({B.n},{B.m})");
                throw e;
            }
            Matrix C = A.Clone() as Matrix;
            for (int i = 0; i < C.n; i++)
            {
                for (int j = 0; j < C.m; j++)
                {
                    C.Elem[i][j] -= B.Elem[i][j];
                }
            }
            return C;
        }
        public static Matrix operator *(Matrix A, double l)
        {
            Matrix B = A.Clone() as Matrix;
            for (int i = 0; i < B.n; i++)
            {
                for (int j = 0; j < B.m; j++)
                {
                    B.Elem[i][j] *= B.Elem[i][j];
                }
            }
            return B;
        }
        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A.m != B.n)
            {
                var e = new InvalidMatrixOperationException($"{A.m} (columns of A) differs from {B.n} (raws of B) when they must be equal");
                throw e;
            }
            Matrix C = new Matrix(A.n, B.m, CreationType.Empty);
            double temp = 0;
            for (int i = 0; i < A.n; i++)
            {
                for (int j = 0; j < B.m; j++)
                {
                    for (int k = 0; k < B.n; k++)
                    {
                        temp += A.Elem[i][k] * B.Elem[k][j];
                    }
                    C.Elem[i][j] = temp;
                    temp = 0;
                }
            }
            return C;
        }
        public override string ToString()
        {
            string temp = null;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    temp += $"{Elem[i][j]} ";
                }
                temp += "\n";
            }
            return temp;
        }
    }
}
