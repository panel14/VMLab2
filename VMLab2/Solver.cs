using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMLab2
{
    internal class Solver
    {
        private static Func<double, double> equation1 = (x) => Math.Pow(x, 3) + 4 * x - 3;
        private static Func<double, double> equation2 = (x) => Math.Pow(x, 2) - 20 * Math.Sin(x);
        private static Func<double, double> equation3 = (x) => Math.Sqrt(x + 1) - 1 / x;

        private static Func<double, double>[] funcs = { equation1, equation2, equation3 };

        private static Func<double, double> fDer1 = (x) => 3 * Math.Pow(x, 2) + 4;
        private static Func<double, double> fDer2 = (x) => 2 * x - 20 * Math.Cos(x);
        private static Func<double, double> fDer3 = (x) => - 1/2 * Math.Sqrt(x + 1) + 1 / Math.Pow(x, 2);

        private static Func<double, double>[] fDers = { fDer1, fDer2, fDer3 };

        private static Func<double, double> sDer1 = (x) => 6 * x;
        private static Func<double, double> sDer2 = (x) => 2 + 20 * Math.Sin(x);
        private static Func<double, double> sDer3 = (x) => -1 / 4 * Math.Sqrt(x + 1) - 2 / Math.Pow(x, 3);

        private static Func<double, double>[] sDers = { sDer1, sDer2, sDer3 };

        private static Func<double, double, double, double> tanget = (x0, y0, fDerVal) => - y0 / fDerVal + x0;

        public static double? executeDivisionMethod(int eqNum, double[] borders, double eps, out int iter)
        {
            iter = 0;

            if (!DMCond(borders, eqNum))
                return null;

            double middle = 0;

            do
            {
                middle = (borders[0] + borders[1]) / 2;
                if (funcs[eqNum](middle) == 0)
                    return middle;
                var newRange = getRange(borders, middle, eqNum);
                if (newRange == null)
                    return null;
                else borders = newRange;
                iter++;
            } while (Math.Abs(borders[0] - borders[1]) > eps);

            return middle;
        }

        private static bool DMCond(double[] borders, int func)
        {
            double test = funcs[func](borders[0]) * funcs[func](borders[1]);
            return funcs[func](borders[0]) * funcs[func](borders[1]) < 0;
        }

        private static double[]? getRange(double[] borders, double middle, int func)
        {
            double[] left = {borders[0], middle};
            double[] right = {borders[1], middle};
            if (DMCond(left, func)) return left;
            if (DMCond(right, func)) return right;
            return null;
        }

        public static double? executeNewtonMethod(int eqNum, double[] borders, double eps, out int iter)
        {
            iter = 0;

            double curX;
            if (NMCond(borders[1], eqNum))
                curX = borders[1];
            else if (NMCond(borders[0], eqNum))
                curX = borders[0];
            else
                return null;

            if (curX == 0) return null;

            curX = tanget(curX, funcs[eqNum](curX), fDers[eqNum](curX));

            while (!NMStopCond(curX, eps, eqNum))
            {
                curX = curX - funcs[eqNum](curX) / fDers[eqNum](curX);
                if (curX == double.NaN)
                    return null;
                iter++;
            }
            return curX;
        }

        public static bool NMCond(double x, int func)
        {
            return funcs[func](x) * sDers[func](x) > 0;
        } 

        public static bool NMStopCond(double curX, double eps, int func)
        {
            return Math.Abs(funcs[func](curX) / fDers[func](curX)) < eps;
        }

        public static double[]? executeNewtonSystemMethod(double quality, out int iter, out double[] uncVal)
        {
            iter = 0;
            uncVal = new double[2];
            Random rand = new Random();
            double[] xVector = { 1.2 , 1.7 };
            for (int i = 0; i < xVector.Length; i++)
                uncVal[i] = xVector[i];

            double[] curDeltas = new double[] { Double.MaxValue, Double.MaxValue };

            while (iter < 2)
            {
                double[][] curW = getJacobiMatrix(xVector);
                double[] curSys = getSystem(xVector);

                curDeltas = makeAStep(curSys, curW);
                xVector = new double[] {
                    xVector[0] + curDeltas[0],
                    xVector[1] + curDeltas[1],
                };

                if (Double.IsNaN(xVector[0]) || Double.IsNaN(xVector[1]))
                    return null;
                iter++;
            }

            return xVector;
        }

        private static double[][] getJacobiMatrix(double[] xVector)
        {
            double[][] jacobiMatrix = new double[2][];

            jacobiMatrix[0] = new double[] {
                6 * Math.Pow(xVector[1], 2),
                -2 * xVector[1]
            };

            jacobiMatrix[1] = new double[] {
                Math.Pow(xVector[1], 3),
                3 * xVector[0] * Math.Pow(xVector[1], 2) - xVector[0]
            };

            return jacobiMatrix;
        }

        private static double[] getSystem(double[] xVector)
        {
            return new double[] {
                2 * Math.Pow(xVector[0], 3) + 3 - Math.Pow(xVector[1], 2) - 1,
                xVector[0] * Math.Pow(xVector[1], 3) - xVector[1] - 4,
            };
        }

        private static double[][] inverseMatrix(double[][] matrix)
        {

            double determinant = matrix[0][0] * matrix[1][1] - matrix[0][1] * matrix[1][0];

            int len = matrix.Length;

            double tmp = matrix[0][1];
            matrix[0][1] = matrix[1][0];
            matrix[1][0] = tmp;

            for (int j = 0; j < len; j++)
            {
                tmp = matrix[0][j];
                matrix[0][j] = matrix[len - 1][len - j - 1] * Math.Pow(-1, j);
                matrix[len - 1][len - j - 1] = tmp * Math.Pow(-1, len - j);
            }

            for (int i = 0; i < len; i++)
                for (int j = 0; j < len; j++)
                    matrix[i][j] /= determinant;

            return matrix;
            
        }

        private static double[][] matrixMulty(double[][] first, double[][] second)
        {
            double[][] multyResult = new double[first.Length][];
            Parallel.For(0, first.Length, i =>
            {
                multyResult[i] = new double[first.Length];
                for (int j = 0; j < second[0].Length; ++j)
                    for (int k = 0; k < first[0].Length; ++k)
                        multyResult[i][j] += first[i][k] * second[k][j];
            });

            return multyResult;
        }

        private static double[] makeAStep(double[] curSys, double[][] jacobi)
        {
            double[][] inverseJacobi = inverseMatrix(jacobi);

            for (int i = 0; i < curSys.Length; ++i)
                curSys[i] *= -1;

            double[][] sysAsMatrix = new double[curSys.Length][];
            for (int i = 0; i < curSys.Length; i++)
                sysAsMatrix[i] = new double[] { curSys[i] };

            double[][] result = matrixMulty(inverseJacobi, sysAsMatrix);

            return new double[] { result[0][0], result[1][0] };
        }
    }
}
