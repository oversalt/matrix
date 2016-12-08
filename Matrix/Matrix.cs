using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_B
{
    class Matrix : AMatrix
    {
        #region Attributes
        //Set up a 2D array
        private double[,] dArray;
        #endregion

        //Constructor
        public Matrix (double [,] dArray)
        {
            //Should do a deep copy, but we don't have time
            this.dArray = dArray;
            //GetLength takes a dimension as a parameter
            this.Rows = dArray.GetLength(0);
            this.Cols = dArray.GetLength(1);
        }

        public override double GetElement(int iRow, int iCol)
        {
            //Offset by one since matrices start at position [1,1]
            return dArray[iRow - 1, iCol - 1];
        }

        public override void SetElement(int iRow, int iCol, double dValue)
        {
            dArray[iRow - 1, iCol - 1] = dValue;
        }

        /// <summary>
        /// Returns an instance of "this" child class
        /// </summary>
        /// <param name="iRows">Number of rows required</param>
        /// <param name="iCols">Number of columns required</param>
        /// <returns></returns>
        internal override AMatrix NewMatrix(int iRows, int iCols)
        {
            return new Matrix(new double[iRows, iCols]);
        }

        #region Operator Overloading
        
        /*
         * Note that not all languages support operator overloading - Java doesn't :(
         * Some languages restric the operators that can be overloaded.
         * Some languages require taht certain operators must be overloaded as pairs (+, -).
         * 
         */
        public static Matrix operator +(Matrix LeftOp, Matrix RightOp)
        {
            return (Matrix)LeftOp.Add(RightOp);
        }

        public static Matrix operator -(Matrix LeftOp, Matrix RightOp)
        {
            return (Matrix)LeftOp.Subtract(RightOp);
        }

        public static Matrix operator *(Matrix LeftOp, Matrix RightOp)
        {
            return (Matrix)LeftOp.Multiply(RightOp);
        }

        public static Matrix operator *(Matrix LeftOp, double dScalar)
        {
            return (Matrix)LeftOp.ScalarMultiplication(dScalar);
        }

        public static Matrix operator *(double dScalar, Matrix RightOp)
        {
            return (Matrix)RightOp.ScalarMultiplication(dScalar);
        }
        #endregion
    }
}
