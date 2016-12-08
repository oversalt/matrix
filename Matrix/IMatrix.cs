using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_B
{
    public interface IMatrix
    {
        IMatrix Add(IMatrix mRight);
        IMatrix Subtract(IMatrix mRight);
        IMatrix Multiply(IMatrix mRight);
        IMatrix ScalarMultiplication(double dScalar);
    }
}
