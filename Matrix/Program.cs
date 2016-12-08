using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_B
{
    class Program
    {
        static void TestCreateMatrix()
        {
            double[,] dArray = { { 12, 3, 52 }, { -10, 45, 0.98 } };
            Matrix m = new Matrix(dArray);
            Console.WriteLine(m.ToString());
        }

        static void TestAdd()
        {
            double[,] d1 = { { 1, 0 }, { 2, 1 } };
            double[,] d2 = { { 1, -1 }, { 0, -1 } };
            Matrix m1 = new Matrix(d1);
            Matrix m2 = new Matrix(d2);
            //Matrix sum = (Matrix) m1.Add(m2);
            //Can now use + as it was overloaded
            Matrix sum = m1 + m2;
            Console.WriteLine(sum.ToString());
        }

        static void TestScalar()
        {
            double[,] d1 = { { 1, 0 }, { 2, 1 } };
            Matrix m1 = new Matrix(d1);
            Matrix product = (Matrix) m1.ScalarMultiplication(3.0);
            Console.WriteLine(product.ToString());
        }

        static void TestSubtract()
        {
            double[,] d1 = { { 1, 0 }, { 2, 1 } };
            double[,] d2 = { { 1, -1 }, { 0, -1 } };
            Matrix m1 = new Matrix(d1);
            Matrix m2 = new Matrix(d2);
            Matrix difference = (Matrix)m1.Subtract(m2);
            Console.WriteLine(difference.ToString());
        }

        static void TestMultiply()
        {
            double[,] d1 = { { 1, 2, 0 }, { -1, 0, 1 } };
            double[,] d2 = { { 1, 0, }, { -1, 2 }, { 0, 1 } };
            Matrix m1 = new Matrix(d1);
            Matrix m2 = new Matrix(d2);
            Matrix product = m1 * m2;
            Console.WriteLine(product.ToString());
        }

        static void TestGaussJordan()
        {
            double[,] d1 = { { 5,5,73,61 }, { 5,73,209,61 }, { 73,209,1825,655 } };
            Matrix m1 = new Matrix(d1);
            Matrix solved = (Matrix)m1.GaussJordanElimination();
            Console.WriteLine(solved.ToString());
        }
        
        static void TestSquare()
        {
            double[,] d1 = { { -2, 4 }, { -1, 2 }, { 0, 1 }, { 1, 2 }, { 2, 4 } };
            Matrix m1 = new Matrix(d1);
            Matrix solved = (Matrix)m1.LeastSquares(2);
            Console.WriteLine(solved.ToString());
        }

        static void TestInverse()
        {
            double[,] d1 = { { 1, -1, -1 }, { -1, 2, 3 }, { 1, 1, 4 } };
            Matrix m1 = new Matrix(d1);
            Matrix inverse = (Matrix)m1.Inverse();
            Console.WriteLine(inverse.ToString());
        }

        static void Main(string[] args)
        {
            //TestAdd();
            //TestScalar();
            //TestSubtract();
            //TestMultiply();
            TestGaussJordan();
            //TestSquare();
            //TestInverse();
        }
    }
}
