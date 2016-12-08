using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_B
{
    public abstract class AMatrix : IMatrix, ICloneable
    {
        #region Attributes and Properties
        private int iRows;
        private int iCols;

        public int Rows
        {
            get
            {
                return iRows;
            }

            set
            {
                if(value < 1)
                {
                    throw new ApplicationException("Rows must be 1 or greater");
                }
                iRows = value;
            }
        }

        public int Cols
        {
            get
            {
                return iCols;
            }

            set
            {
                if (value < 1)
                {
                    throw new ApplicationException("Columns must be 1 or greater");
                }
                iCols = value;
            }
        }
        #endregion

        #region Abstract Methods
        public abstract double GetElement(int iRow, int iCol);
        public abstract void SetElement(int iRow, int iCol, double dValue);
        internal abstract AMatrix NewMatrix(int iRows, int iCols);
        #endregion

        #region Least Squares
        /// <summary>
        /// Returns a solved augmented matrix, last column being the coefficients
        /// of the trendline representing the data from the calling matrix
        /// </summary>
        /// <param name="m">Degree or order required</param>
        /// <returns>Solved augmented matrix</returns>
        public AMatrix LeastSquares(int m)
        {
            //Create the augmented matrix. We could use NewMatrix, however,
            //this is a good time to introduce the C# specific method of doing this
            //It is called reflection
            double[,] dArray = new double[m + 1, m + 2];
            //Wrap up any parameters into a single object array
            object[] parameters = { dArray };
            //Magic line of reflection
            AMatrix mAugmented = (AMatrix)Activator.CreateInstance(this.GetType(), parameters);

            //Create temporary matrix for the x-sums
            AMatrix xSums = NewMatrix(1, 2 * m + 1);
            //If the calling matrix is the correct size
            if (this.Rows > m && this.Cols == 2)
            {
                //Calculate the x - sums
                for (int power = 0; power < 2 * m + 1; power++)
                {
                    //Create a temp value for the sum
                    double dSum = 0;
                    //Loop through each data point
                    for (int currentPoint = 1; currentPoint <= this.Rows; currentPoint++)
                    {
                        dSum += Math.Pow(this.GetElement(currentPoint, 1), power);
                    }
                    //Temporarily store in xSums
                    xSums.SetElement(1, power + 1, dSum);
                }

                //Calculate the y - sums
                for (int power = 0; power < m + 1; power++)
                {
                    double dSum = 0;
                    for (int currentPoint = 1; currentPoint <= this.Rows; currentPoint++)
                    {
                        dSum += this.GetElement(currentPoint, 2) * Math.Pow(GetElement(currentPoint, 1), power);
                    }
                    mAugmented.SetElement(power + 1, mAugmented.Cols, dSum);
                }

                //Put the x-sums into the augmented matrix
                for (int x = 1; x <= mAugmented.Rows; x++)
                {
                    for (int y = 1; y < mAugmented.Cols; y++)
                    {
                        //Put the current x-sum into the current position of the matrix
                        mAugmented.SetElement(x , y, xSums.GetElement(1, x + y - 1));
                    }
                }
            }
            else
            {
                //Not the correct size
                throw new ApplicationException("Calling matrix incorrect size");
            }

            return mAugmented.GaussJordanElimination();
        }
        #endregion

        #region Gauss Jordan Elimination
        public AMatrix GaussJordanElimination()
        {
            //a reference to a copy of calling matrix
            AMatrix mSolution = null;
            //row multiplying factor
            double dFactor = 0;
            //current pivot element
            double dPivot = 0;

            //if this is not an augmented matrix, throw an exception
            if (this.Cols != this.Rows + 1)
            {
                throw new ApplicationException("incorrect dimensions");
            }

            //create a copy of the calling matrix
            mSolution = (AMatrix)this.Clone();

            for (int i = 1; i <= mSolution.Rows; i++)
            {
                mSolution.SystemSolveable(i);
                dPivot = mSolution.GetElement(i, i);
                //start from the pivot column for efficiency
                for (int j = i; j <= mSolution.Cols; j++)
                {
                    double dCurrent = mSolution.GetElement(i, j) / dPivot;
                    mSolution.SetElement(i, j, dCurrent);
                }

                for (int k = 1; k <= mSolution.Rows; k++)
                {
                    if (k != i)
                    {
                        dFactor = -1 * mSolution.GetElement(k, i);
                        //start from the pivot column for efficiency
                        for (int j = i; j <= mSolution.Cols; j++)
                        {
                            double dCurrent = mSolution.GetElement(k, j) + mSolution.GetElement(i, j) * dFactor;
                            mSolution.SetElement(k, j, dCurrent);
                        }

                    }
                }
            }
            return mSolution;
        }

        /// <summary>
        /// Check the current pivot element if it is 0. if so, check the rows below for a
        ///     non 0 pivot. if found, swap the rows.
        /// </summary>
        /// <param name="i"></param>
        private void SystemSolveable(int i)
        {
            /*
             * CurrentPivot <-- get the current pivot element 
             * NextRow <-- current row to check for a non 0 pivot
             * 
             * while the current pivot is still 0 and there are still rows to check
             *      CurrentPivot <-- possible pivot from this row
             *      if CurrentPivot is not 0 (we found a row to swap)
             *          swap the pivot row (i) with the current row (nextrow)
             *      else
             *          check the next row (Nextrow + 1)
             * if a non 0 pivot wasnt found
             *      indicate error (throw exception)
            */

            double dCurrentPivot = this.GetElement(i, i);
            int iNextRow = i + 1;
            while (dCurrentPivot == 0 && iNextRow <= this.Rows)
            {
                dCurrentPivot = this.GetElement(iNextRow, i);
                if (dCurrentPivot != 0)
                {
                    //swap 2 rows around
                    for (int j = 1; j <= this.Cols; j++)
                    {
                        double dTemp = this.GetElement(i, j);
                        this.SetElement(i, j, this.GetElement(iNextRow, j));
                        this.SetElement(iNextRow, j, dTemp);
                    }
                }
                else
                {
                    iNextRow++;
                }
            }
            if (dCurrentPivot == 0)
            {
                throw new ApplicationException("no unique solution found");
            }
        }
        #endregion

        #region Assignment 2, Inverse
        public AMatrix Inverse()
        {
            double[,] d1 = { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
            //a reference to a copy of calling matrix
            AMatrix mCopy = null;
            AMatrix mInverse = (AMatrix)new Matrix(d1);
            //row multiplying factor
            double dFactor = 0;
            //current pivot element
            double dPivot = 0;


            //if this is not an augmented matrix, throw an exception
            if (this.Cols != this.Rows)
            {
                throw new ApplicationException("Not a square matrix");
            }

            //create a copy of the calling matrix
            mCopy = (AMatrix)this.Clone();

            for (int i = 1; i <= mCopy.Rows; i++)
            {
                dPivot = mCopy.GetElement(i, i);
                //start from the pivot column for efficiency
                for (int j = i; j <= mCopy.Cols; j++)
                {
                    double dCurrent = mCopy.GetElement(i, j) / dPivot;
                    double dInverseCurrent = mInverse.GetElement(i, j) / dPivot;
                    mCopy.SetElement(i, j, dCurrent);
                    mInverse.SetElement(i, j, dInverseCurrent);
                }

                for (int k = 1; k <= mCopy.Rows; k++)
                {
                    if (k != i)
                    {
                        dFactor = -1 * mCopy.GetElement(k, i);
                        //start from the pivot column for efficiency
                        for (int j = 1; j <= mCopy.Cols; j++) //The secret to making this god forsaken thing to working properly
                        {
                            double dCurrent = mCopy.GetElement(k, j) + mCopy.GetElement(i, j) * dFactor;
                            double dInverseCurrent = mInverse.GetElement(k, j) + mInverse.GetElement(i, j) * dFactor;
                            mCopy.SetElement(k, j, dCurrent);
                            mInverse.SetElement(k, j, dInverseCurrent);
                        }

                    }
                }
            }
            return mInverse;
        }
        #endregion

        //Broken Code, look into later
        //public AMatrix GaussJordanElimination()
        //{
        //    //A reference to a copy of the calling matrix
        //    AMatrix mSolution = null;
        //    //Row multiplying factor
        //    double dFactor = 0;
        //    //The current pivot element
        //    double dPivot = 0;

        //    //If this is not an augmented matrix, throw an exception
        //    if(this.Cols != this.Rows + 1)
        //    {
        //        throw new ApplicationException("Incorrect dimensions for an augmented matrix");
        //    }

        //    //Create a copy of the calling matrix
        //    mSolution = (AMatrix)this.Clone();

        //    //For each pivot in the solution matrix (i)
        //    for(int i = 1; i <= Rows - 1; i++)
        //    {
        //        //Store a copy of the current pivot value
        //        dPivot = mSolution.GetElement(i, i);
        //        //For each element for the pivot row, divide by the original pivot value(j)
        //        for (int j = 1; j <= Cols - 1; j++)
        //        {
        //            mSolution.SetElement(i, j, (mSolution.GetElement(i, j) / dPivot));
        //        }

        //        //For each row other than the pivot row, reduce elements in the pivot column to 0 (k)
        //        for (int k = 1; k <= Rows - 1; k++)
        //        {
        //            //If current row is not the pivot
        //            if (i != k)
        //            {
        //                //Get the multiplying factor
        //                dFactor = mSolution.GetElement(k, i) * -1;

        //                //For each element in the current row (j)
        //                for (int j = 1; j <= Cols - 1; j++)
        //                {
        //                    //Current <-- Current element + multiplying factor * corresponding element in pivot row
        //                    //Current location in solution matrix <-- Current
        //                    mSolution.SetElement(k, j, (mSolution.GetElement(k, j) + (dFactor * dPivot)));
        //                }
        //            }
        //        }
        //    }
        //    //Return solution matrix
        //    return mSolution;

        //}

        public IMatrix Add(IMatrix mRight)
        {
            AMatrix LeftOp = this;
            AMatrix RightOp = (AMatrix) mRight;
            AMatrix Sum = null;

            //If operands are the same size
            if (LeftOp.Rows == RightOp.Rows && LeftOp.Cols == RightOp.Cols)
            {
                //Create the sum matrix based on the size of the operands
                Sum = NewMatrix(LeftOp.Rows, LeftOp.Cols);
                for (int r = 1; r <= LeftOp.Rows; r++)
                {
                    for (int c = 1; c <= LeftOp.Cols; c++)
                    {
                        //Calculate the sum for this position
                        double dVal = LeftOp.GetElement(r, c) + RightOp.GetElement(r, c);
                        //Put the result in the sum matrix
                        Sum.SetElement(r, c, dVal);
                    }
                }
            }
            else
            {
                throw new ApplicationException("Operands must be of equal dimensions");
            }
            return Sum;
        }

        public IMatrix Subtract(IMatrix mRight)
        {
            //Just add the opposite
            return this.Add(mRight.ScalarMultiplication(-1.0));
        }

        public IMatrix Multiply(IMatrix mRight)
        {
            AMatrix LeftOp = this;
            AMatrix RightOp = (AMatrix) mRight;
            AMatrix Product = null;

            double dSum = 0;

            if (LeftOp.iCols == RightOp.Rows)
            {
                Product = NewMatrix(LeftOp.Rows, RightOp.Cols);
                for (int r = 1; r <= Product.Rows; r++)
                {
                    for (int c = 1; c <= Product.Cols; c++)
                    {
                        dSum = 0;
                        for (int e = 1; e <= LeftOp.Cols; e++)
                        {
                            dSum += LeftOp.GetElement(r, e) * RightOp.GetElement(e, c);
                        }

                        Product.SetElement(r, c, dSum);
                    }
                }
            }
            else
            {
                throw new ApplicationException("Operands are of incorrect dimensions");
            }

            return Product;
        }

        public IMatrix ScalarMultiplication(double dScalar)
        {
            AMatrix Product = NewMatrix(this.Rows, this.Cols);

            for (int r = 1; r <= this.Rows; r++)
            {
                for (int c = 1; c <= this.Cols; c++)
                {
                    Product.SetElement(r, c, this.GetElement(r, c) * dScalar);
                }
            }

            return Product;
        }

        public object Clone()
        {
            return ScalarMultiplication(1);
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            char cUL = (char)0x250C;
            char cUR = (char)0x2510;
            char cLL = (char)0x2514;
            char cLR = (char)0x2518;
            char cVLine = (char)0x2502;

            //build the top row
            s.Append(cUL);
            for (int j = 1; j <= this.Cols; j++)
            {
                s.Append("\t\t");
            }
            s.Append(cUR + "\n");

            //build the data rows
            for (int i = 1; i <= this.Rows; i++)
            {
                s.Append(cVLine);
                for (int j = 1; j <= this.Cols; j++)
                {
                    if (this.GetElement(i, j) >= 0)
                    {
                        s.Append(" ");
                    }
                    s.Append(String.Format("{0:0.000 e+00}", this.GetElement(i, j)) + "\t");

                }
                s.Append(cVLine + "\n");
            }
            //Build the bottom row
            s.Append(cLL);
            for (int j = 1; j <= this.Cols; j++)
            {
                s.Append("\t\t");
            }
            s.Append(cLR + "\n");
            return s.ToString();
        }
    }
}
