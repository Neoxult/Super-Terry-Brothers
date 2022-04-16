namespace TerryBros.Utils
{
    public class Matrix
    {
        public int Columns { get; }
        public int Rows { get; }
        private readonly float[,] _content;

        public Matrix(int columnNumber, int rowNumber)
        {
            Columns = columnNumber;
            Rows = rowNumber;
            _content = new float[Columns, Rows];
        }

        public Matrix(Vector3 vector1, Vector3 vector2, Vector3 vector3, bool rowPerVector = true) : this(3, 3)
        {
            float value;

            for (int j = 0; j < 3; j++)
            {
                Vector3 vector = j == 0 ? vector1 : j == 1 ? vector2 : vector3;

                for (int i = 0; i < 3; i++)
                {
                    value = vector[i];

                    if (rowPerVector)
                    {
                        _content[i, j] = value;
                    }
                    else
                    {
                        _content[j, i] = value;
                    }
                }
            }
        }

        public Matrix(Matrix A) : this(A.Rows, A.Columns)
        {
            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    _content[i, j] = A[i, j];
                }
            }
        }

        public float this[int index1, int index2]
        {
            get => _content[index1, index2];
            set => _content[index1, index2] = value;
        }

        public void SetVector3(Vector3 vector, int column, int row = 1, bool standing = true)
        {
            for (int i = 0; i < 3; i++)
            {
                if (column >= Columns || row >= Rows)
                {
                    throw new System.IndexOutOfRangeException($"Matrix can't hold more elements, index is out of range with [{column},{row}] and a size of [{Columns},{Rows}]");
                }

                _content[column, row] = vector[i];
                column += standing ? 1 : 0;
                row += standing ? 0 : 1;
            }
        }

        /// <summary>
        /// Simply Inverts a 3 by 3 matrix. Calculation taken from here:
        /// https://en.wikipedia.org/wiki/Invertible_matrix#Inversion_of_3_%C3%97_3_matrices
        /// </summary>
        public void Invert3x3()
        {
            Sandbox.Assert.True(Columns == 3 && Rows == 3);

            float a, b, c, d, e, f, g, h, i;
            float A, B, C, D, E, F, G, H, I;

            a = _content[0, 0]; b = _content[0, 1]; c = _content[0, 2];
            d = _content[1, 0]; e = _content[1, 1]; f = _content[1, 2];
            g = _content[2, 0]; h = _content[2, 1]; i = _content[2, 2];
            A = e * i - f * h; D = -(b * i - c * h); G = b * f - c * e;
            B = -(d * i - f * g); E = a * i - c * g; H = -(a * f - c * d);
            C = d * h - e * g; F = -(a * h - b * g); I = a * e - b * d;

            float det = a * A + b * B + c * C;

            _content[0, 0] = A / det; _content[0, 1] = D / det; _content[0, 2] = G / det;
            _content[1, 0] = B / det; _content[1, 1] = E / det; _content[1, 2] = H / det;
            _content[2, 0] = C / det; _content[2, 1] = F / det; _content[2, 2] = I / det;
        }
        public static Matrix operator +(Matrix A) => A;
        public static Matrix operator -(Matrix A) => A * -1f;
        public static Matrix operator *(Matrix A, float f)
        {
            Matrix B = new(A.Columns, A.Rows);

            for (int i = 0; i < A.Columns; i++)
            {
                for (int j = 0; j < A.Rows; j++)
                {
                    B[i, j] = A[i, j] * f;
                }
            }

            return B;
        }

        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A.Rows != B.Columns)
            {
                throw new System.InvalidOperationException($"Can't multiply a Matrix with {A.Rows} rows with a Matrix with {B.Columns} columns!");
            }
            Matrix C = new(A.Columns, B.Rows);

            for (int i = 0; i < A.Columns; i++)
            {
                for (int j = 0; j < B.Rows; j++)
                {
                    for (int k = 0; k < A.Rows; k++)
                    {

                        C[i, j] = A[i, k] * B[k, j];
                    }
                }
            }

            return C;
        }

        public static Vector3 operator *(Matrix A, Vector3 v)
        {
            if (A.Columns != 3 || A.Rows != 3)
            {
                throw new System.InvalidOperationException($"Can't multiply a Matrix with size [{A.Columns},{A.Rows}] with a Vector3 datatype and return a Vector3!");
            }

            Vector3 resultVec = Vector3.Zero;

            for (int i = 0; i < A.Columns; i++)
            {
                for (int j = 0; j < A.Rows; j++)
                {
                    resultVec[i] += A[i, j] * v[j];
                }
            }

            return resultVec;
        }

        public override string ToString()
        {
            System.Text.StringBuilder builder = new();

            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    builder.Append(_content[i, j]);

                    if (j < Rows - 1)
                    {
                        builder.Append(", ");
                    }
                    else
                    {
                        builder.Append(";\n");
                    }
                }
            }

            return builder.ToString();
        }
    }
}
