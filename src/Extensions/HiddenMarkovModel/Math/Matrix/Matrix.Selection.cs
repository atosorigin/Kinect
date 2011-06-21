// Accord Math Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;
using System.Collections.Generic;
using AForge;

namespace Accord.Math
{
    /// <summary>
    /// Static class Matrix. Defines a set of extension methods
    /// that operates mainly on multidimensional arrays and vectors.
    /// </summary>
    public static partial class Matrix
    {
        #region Submatrix

        /// <summary>Returns a sub matrix extracted from the current matrix.</summary>
        /// <param name="data">The matrix to return the submatrix from.</param>
        /// <param name="startRow">Start row index</param>
        /// <param name="endRow">End row index</param>
        /// <param name="startColumn">Start column index</param>
        /// <param name="endColumn">End column index</param>
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        public static T[,] Submatrix<T>(this T[,] data, int startRow, int endRow, int startColumn, int endColumn)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            if ((startRow > endRow) || (startColumn > endColumn) || (startRow < 0) ||
                (startRow >= rows) || (endRow < 0) || (endRow >= rows) ||
                (startColumn < 0) || (startColumn >= cols) || (endColumn < 0) ||
                (endColumn >= cols))
            {
                throw new ArgumentException("Argument out of range.");
            }

            var X = new T[endRow - startRow + 1,endColumn - startColumn + 1];
            for (int i = startRow; i <= endRow; i++)
            {
                for (int j = startColumn; j <= endColumn; j++)
                {
                    X[i - startRow, j - startColumn] = data[i, j];
                }
            }

            return X;
        }

        /// <summary>Returns a sub matrix extracted from the current matrix.</summary>
        /// <param name="data">The matrix to return the submatrix from.</param>
        /// <param name="rowIndexes">Array of row indices. Pass null to select all indices.</param>
        /// <param name="columnIndexes">Array of column indices. Pass null to select all indices.</param>
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        public static T[,] Submatrix<T>(this T[,] data, int[] rowIndexes, int[] columnIndexes)
        {
            if (rowIndexes == null)
                rowIndexes = Indexes(0, data.GetLength(0));

            if (columnIndexes == null)
                columnIndexes = Indexes(0, data.GetLength(1));


            var X = new T[rowIndexes.Length,columnIndexes.Length];

            for (int i = 0; i < rowIndexes.Length; i++)
            {
                for (int j = 0; j < columnIndexes.Length; j++)
                {
                    if ((rowIndexes[i] < 0) || (rowIndexes[i] >= data.GetLength(0)) ||
                        (columnIndexes[j] < 0) || (columnIndexes[j] >= data.GetLength(1)))
                    {
                        throw new ArgumentException("Argument out of range.");
                    }

                    X[i, j] = data[rowIndexes[i], columnIndexes[j]];
                }
            }

            return X;
        }

        /// <summary>Returns a sub matrix extracted from the current matrix.</summary>
        /// <param name="data">The matrix to return the submatrix from.</param>
        /// <param name="rowIndexes">Array of row indices</param>
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        public static T[,] Submatrix<T>(this T[,] data, int[] rowIndexes)
        {
            var X = new T[rowIndexes.Length,data.GetLength(1)];

            for (int i = 0; i < rowIndexes.Length; i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if ((rowIndexes[i] < 0) || (rowIndexes[i] >= data.GetLength(0)))
                    {
                        throw new ArgumentException("Argument out of range.");
                    }

                    X[i, j] = data[rowIndexes[i], j];
                }
            }

            return X;
        }

        /// <summary>Returns a subvector extracted from the current vector.</summary>
        /// <param name="data">The vector to return the subvector from.</param>
        /// <param name="indexes">Array of indices.</param>
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        public static T[] Submatrix<T>(this T[] data, int[] indexes)
        {
            var X = new T[indexes.Length];

            for (int i = 0; i < indexes.Length; i++)
            {
                X[i] = data[indexes[i]];
            }

            return X;
        }

        /// <summary>Returns a sub matrix extracted from the current matrix.</summary>
        /// <param name="data">The vector to return the subvector from.</param>
        /// <param name="i0">Starting index.</param>
        /// <param name="i1">End index.</param>
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        public static T[] Submatrix<T>(this T[] data, int i0, int i1)
        {
            var X = new T[i1 - i0 + 1];

            for (int i = i0; i <= i1; i++)
            {
                X[i - i0] = data[i];
            }

            return X;
        }

        /// <summary>Returns a sub matrix extracted from the current matrix.</summary>
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        public static T[] Submatrix<T>(this T[] data, int first)
        {
            if (first < 1 || first > data.Length)
                throw new ArgumentOutOfRangeException("first");

            return Submatrix(data, 0, first - 1);
        }

        /// <summary>Returns a sub matrix extracted from the current matrix.</summary>
        /// <param name="data">The matrix to return the submatrix from.</param>
        /// <param name="i0">Starting row index</param>
        /// <param name="i1">End row index</param>
        /// <param name="c">Array of column indices</param>
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        public static T[,] Submatrix<T>(this T[,] data, int i0, int i1, int[] c)
        {
            if ((i0 > i1) || (i0 < 0) || (i0 >= data.GetLength(0))
                || (i1 < 0) || (i1 >= data.GetLength(0)))
            {
                throw new ArgumentException("Argument out of range.");
            }

            var X = new T[i1 - i0 + 1,c.Length];

            for (int i = i0; i <= i1; i++)
            {
                for (int j = 0; j < c.Length; j++)
                {
                    if ((c[j] < 0) || (c[j] >= data.GetLength(1)))
                    {
                        throw new ArgumentException("Argument out of range.");
                    }

                    X[i - i0, j] = data[i, c[j]];
                }
            }

            return X;
        }

        /// <summary>Returns a sub matrix extracted from the current matrix.</summary>
        /// <param name="data">The matrix to return the submatrix from.</param>
        /// <param name="rowIndexes">Array of row indices</param>
        /// <param name="j0">Start column index</param>
        /// <param name="j1">End column index</param>
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        public static T[,] Submatrix<T>(this T[,] data, int[] rowIndexes, int j0, int j1)
        {
            if ((j0 > j1) || (j0 < 0) || (j0 >= data.GetLength(1)) || (j1 < 0)
                || (j1 >= data.GetLength(1)))
            {
                throw new ArgumentException("Argument out of range.");
            }

            if (rowIndexes == null)
                rowIndexes = Indexes(0, data.GetLength(0));

            var X = new T[rowIndexes.Length,j1 - j0 + 1];

            for (int i = 0; i < rowIndexes.Length; i++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    if ((rowIndexes[i] < 0) || (rowIndexes[i] >= data.GetLength(0)))
                    {
                        throw new ArgumentException("Argument out of range.");
                    }

                    X[i, j - j0] = data[rowIndexes[i], j];
                }
            }

            return X;
        }

        /// <summary>Returns a sub matrix extracted from the current matrix.</summary>
        /// <param name="data">The matrix to return the submatrix from.</param>
        /// <param name="rowIndexes">Array of row indices</param>
        /// <param name="j0">Start column index</param>
        /// <param name="j1">End column index</param>
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        public static T[][] Submatrix<T>(this T[][] data, int[] rowIndexes, int j0, int j1)
        {
            if ((j0 > j1) || (j0 < 0) || (j0 >= data[0].Length) ||
                (j1 < 0) || (j1 >= data[0].Length))
            {
                throw new ArgumentException("Argument out of range.");
            }

            if (rowIndexes == null)
                rowIndexes = Indexes(0, data.GetLength(0));

            var X = new T[rowIndexes.Length][];

            for (int i = 0; i < rowIndexes.Length; i++)
            {
                X[i] = new T[j1 - j0 + 1];

                for (int j = j0; j <= j1; j++)
                {
                    if ((rowIndexes[i] < 0) || (rowIndexes[i] >= data.GetLength(0)))
                    {
                        throw new ArgumentException("Argument out of range.");
                    }

                    X[i][j - j0] = data[rowIndexes[i]][j];
                }
            }

            return X;
        }

        /// <summary>Returns a sub matrix extracted from the current matrix.</summary>
        /// <param name="data">The matrix to return the submatrix from.</param>
        /// <param name="i0">Starting row index</param>
        /// <param name="i1">End row index</param>
        /// <param name="columnIndexes">Array of column indices</param>
        /// <remarks>
        ///   Routine adapted from Lutz Roeder's Mapack for .NET, September 2000.
        /// </remarks>
        public static T[][] Submatrix<T>(this T[][] data, int i0, int i1, int[] columnIndexes)
        {
            if ((i0 > i1) || (i0 < 0) || (i0 >= data.Length)
                || (i1 < 0) || (i1 >= data.Length))
            {
                throw new ArgumentException("Argument out of range");
            }

            if (columnIndexes == null)
                columnIndexes = Indexes(0, data[0].Length);

            var X = new T[i1 - i0 + 1][];

            for (int i = i0; i <= i1; i++)
            {
                X[i] = new T[columnIndexes.Length];

                for (int j = 0; j < columnIndexes.Length; j++)
                {
                    if ((columnIndexes[j] < 0) || (columnIndexes[j] >= data[i].Length))
                    {
                        throw new ArgumentException("Argument out of range.");
                    }

                    X[i - i0][j] = data[i][columnIndexes[j]];
                }
            }

            return X;
        }

        #endregion

        #region Row and column getters and setters

        /// <summary>
        ///   Gets a column vector from a matrix.
        /// </summary>
        public static T[] GetColumn<T>(this T[,] m, int index)
        {
            var column = new T[m.GetLength(0)];

            for (int i = 0; i < column.Length; i++)
                column[i] = m[i, index];

            return column;
        }

        /// <summary>
        ///   Gets a column vector from a matrix.
        /// </summary>
        public static T[] GetColumn<T>(this T[][] m, int index)
        {
            var column = new T[m.Length];

            for (int i = 0; i < column.Length; i++)
                column[i] = m[i][index];

            return column;
        }

        /// <summary>
        ///   Stores a column vector into the given column position of the matrix.
        /// </summary>
        public static T[,] SetColumn<T>(this T[,] m, int index, T[] column)
        {
            for (int i = 0; i < column.Length; i++)
                m[i, index] = column[i];

            return m;
        }

        /// <summary>
        ///   Gets a row vector from a matrix.
        /// </summary>
        public static T[] GetRow<T>(this T[,] m, int index)
        {
            var row = new T[m.GetLength(1)];

            for (int i = 0; i < row.Length; i++)
                row[i] = m[index, i];

            return row;
        }

        /// <summary>
        ///   Stores a row vector into the given row position of the matrix.
        /// </summary>
        public static T[,] SetRow<T>(this T[,] m, int index, T[] row)
        {
            for (int i = 0; i < row.Length; i++)
                m[index, i] = row[i];

            return m;
        }

        #endregion

        #region Row and column insertion and removal

        /// <summary>
        ///   Returns a new matrix without one of its columns.
        /// </summary>
        public static T[][] RemoveColumn<T>(this T[][] m, int index)
        {
            var X = new T[m.Length][];

            for (int i = 0; i < m.Length; i++)
            {
                X[i] = new T[m[i].Length - 1];
                for (int j = 0; j < index; j++)
                {
                    X[i][j] = m[i][j];
                }
                for (int j = index + 1; j < m[i].Length; j++)
                {
                    X[i][j - 1] = m[i][j];
                }
            }
            return X;
        }

        /// <summary>
        ///   Returns a new matrix with a given column vector inserted at a given index.
        /// </summary>
        public static T[,] InsertColumn<T>(this T[,] m, T[] column, int index)
        {
            int rows = m.GetLength(0);
            int cols = m.GetLength(1);

            var X = new T[rows,cols + 1];

            for (int i = 0; i < rows; i++)
            {
                // Copy original matrix
                for (int j = 0; j < index; j++)
                {
                    X[i, j] = m[i, j];
                }
                for (int j = index; j < cols; j++)
                {
                    X[i, j + 1] = m[i, j];
                }

                // Copy additional column
                X[i, index] = column[i];
            }

            return X;
        }

        /// <summary>
        ///   Returns a new matrix with a given row vector inserted at a given index.
        /// </summary>
        public static T[,] InsertRow<T>(this T[,] m, T[] row, int index)
        {
            if (m == null) throw new ArgumentNullException("m");
            if (row == null) throw new ArgumentNullException("row");

            if (row.Length != m.GetLength(1))
                throw new ArgumentException();

            int rows = m.GetLength(0);
            int cols = m.GetLength(1);

            var X = new T[rows + 1,cols];

            for (int i = 0; i < cols; i++)
            {
                // Copy original matrix
                for (int j = 0; j < rows; j++)
                {
                    X[j, i] = m[j, i];
                }
                for (int j = index; j < rows; j++)
                {
                    X[j + 1, i] = m[j, i];
                }

                // Copy additional column
                X[index, i] = row[i];
            }

            return X;
        }

        /// <summary>
        ///   Removes an element from a vector.
        /// </summary>
        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            var r = new T[array.Length - 1];
            for (int i = 0; i < index; i++)
                r[i] = array[i];
            for (int i = index; i < r.Length; i++)
                r[i] = array[i + 1];

            return r;
        }

        #endregion

        #region Element search

        /// <summary>
        ///   Gets the indices of all elements matching a certain criteria.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="data">The array to search inside.</param>
        /// <param name="func">The search criteria.</param>
        public static int[] Find<T>(this T[] data, Func<T, bool> func)
        {
            return Find(data, func, false);
        }

        /// <summary>
        ///   Gets the indices of all elements matching a certain criteria.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="data">The array to search inside.</param>
        /// <param name="func">The search criteria.</param>
        /// <param name="firstOnly">
        ///    Set to true to stop when the first element is
        ///    found, set to false to get all elements.
        /// </param>
        public static int[] Find<T>(this T[] data, Func<T, bool> func, bool firstOnly)
        {
            var idx = new List<int>();
            for (int i = 0; i < data.Length; i++)
            {
                if (func(data[i]))
                {
                    if (firstOnly)
                        return new[] {i};
                    else idx.Add(i);
                }
            }
            return idx.ToArray();
        }

        /// <summary>
        ///   Gets the indices of all elements matching a certain criteria.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="data">The array to search inside.</param>
        /// <param name="func">The search criteria.</param>
        public static int[][] Find<T>(this T[,] data, Func<T, bool> func)
        {
            return Find(data, func, false);
        }

        /// <summary>
        ///   Gets the indices of all elements matching a certain criteria.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="data">The array to search inside.</param>
        /// <param name="func">The search criteria.</param>
        /// <param name="firstOnly">
        ///    Set to true to stop when the first element is
        ///    found, set to false to get all elements.
        /// </param>
        public static int[][] Find<T>(this T[,] data, Func<T, bool> func, bool firstOnly)
        {
            var idx = new List<int[]>();
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (func(data[i, j]))
                    {
                        if (firstOnly)
                            return new[] {new[] {i, j}};
                        else idx.Add(new[] {i, j});
                    }
                }
            }
            return idx.ToArray();
        }

        #endregion

        #region Element ranges (maximum and minimum)

        /// <summary>
        ///   Gets the maximum element in a vector.
        /// </summary>
        public static T Max<T>(this T[] values, out int imax) where T : IComparable
        {
            imax = 0;
            T max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                    imax = i;
                }
            }
            return max;
        }

        /// <summary>
        ///   Gets the maximum element in a vector.
        /// </summary>
        public static T Max<T>(this T[] values) where T : IComparable
        {
            int imax;
            return Max(values, out imax);
        }

        /// <summary>
        ///   Gets the minimum element in a vector.
        /// </summary>
        public static T Min<T>(this T[] values, out int imax) where T : IComparable
        {
            imax = 0;
            T max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(max) < 0)
                {
                    max = values[i];
                    imax = i;
                }
            }
            return max;
        }

        /// <summary>
        ///   Gets the minimum element in a vector.
        /// </summary>
        public static T Min<T>(this T[] values) where T : IComparable
        {
            int imin = 0;
            return Min(values, out imin);
        }

        /// <summary>
        ///   Gets the maximum values accross one dimension of a matrix.
        /// </summary>
        public static T[] Max<T>(this T[,] matrix, int dimension, out int[] imax) where T : IComparable
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            T[] max;

            if (dimension == 1) // Search down columns
            {
                imax = new int[rows];
                max = matrix.GetColumn(0);

                for (int j = 0; j < rows; j++)
                {
                    for (int i = 1; i < cols; i++)
                    {
                        if (matrix[j, i].CompareTo(max[j]) > 0)
                        {
                            max[j] = matrix[j, i];
                            imax[j] = i;
                        }
                    }
                }
            }
            else
            {
                imax = new int[cols];
                max = matrix.GetRow(0);

                for (int j = 0; j < cols; j++)
                {
                    for (int i = 1; i < rows; i++)
                    {
                        if (matrix[i, j].CompareTo(max[j]) > 0)
                        {
                            max[j] = matrix[i, j];
                            imax[j] = i;
                        }
                    }
                }
            }

            return max;
        }

        /// <summary>
        ///   Gets the minimum values across one dimension of a matrix.
        /// </summary>
        public static T[] Min<T>(this T[,] matrix, int dimension, out int[] imin) where T : IComparable
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            T[] min;

            if (dimension == 1) // Search down columns
            {
                imin = new int[rows];
                min = matrix.GetColumn(0);

                for (int j = 0; j < rows; j++)
                {
                    for (int i = 1; i < cols; i++)
                    {
                        if (matrix[j, i].CompareTo(min[j]) < 0)
                        {
                            min[j] = matrix[j, i];
                            imin[j] = i;
                        }
                    }
                }
            }
            else
            {
                imin = new int[cols];
                min = matrix.GetRow(0);

                for (int j = 0; j < cols; j++)
                {
                    for (int i = 1; i < rows; i++)
                    {
                        if (matrix[i, j].CompareTo(min[j]) < 0)
                        {
                            min[j] = matrix[i, j];
                            imin[j] = i;
                        }
                    }
                }
            }

            return min;
        }

        /// <summary>
        ///   Gets the maximum values accross one dimension of a matrix.
        /// </summary>
        public static T[] Max<T>(this T[][] matrix) where T : IComparable
        {
            int[] imax;
            return Max(matrix, 0, out imax);
        }

        /// <summary>
        ///   Gets the maximum values accross one dimension of a matrix.
        /// </summary>
        public static T[] Max<T>(this T[][] matrix, int dimension, out int[] imax) where T : IComparable
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;

            T[] max;

            if (dimension == 1) // Search down columns
            {
                imax = new int[rows];
                max = matrix.GetColumn(0);

                for (int j = 0; j < rows; j++)
                {
                    for (int i = 1; i < cols; i++)
                    {
                        if (matrix[j][i].CompareTo(max[j]) > 0)
                        {
                            max[j] = matrix[j][i];
                            imax[j] = i;
                        }
                    }
                }
            }
            else
            {
                imax = new int[cols];
                max = (T[]) matrix[0].Clone();

                for (int j = 0; j < cols; j++)
                {
                    for (int i = 1; i < rows; i++)
                    {
                        if (matrix[i][j].CompareTo(max[j]) > 0)
                        {
                            max[j] = matrix[i][j];
                            imax[j] = i;
                        }
                    }
                }
            }

            return max;
        }

        /// <summary>
        ///   Gets the minimum values across one dimension of a matrix.
        /// </summary>
        public static T[] Min<T>(this T[][] matrix) where T : IComparable
        {
            int[] imin;
            return Min(matrix, 0, out imin);
        }

        /// <summary>
        ///   Gets the minimum values across one dimension of a matrix.
        /// </summary>
        public static T[] Min<T>(this T[][] matrix, int dimension, out int[] imin) where T : IComparable
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;

            T[] min;

            if (dimension == 1) // Search down columns
            {
                imin = new int[rows];
                min = matrix.GetColumn(0);

                for (int j = 0; j < rows; j++)
                {
                    for (int i = 1; i < cols; i++)
                    {
                        if (matrix[j][i].CompareTo(min[j]) < 0)
                        {
                            min[j] = matrix[j][i];
                            imin[j] = i;
                        }
                    }
                }
            }
            else
            {
                imin = new int[cols];
                min = (T[]) matrix[0].Clone();

                for (int j = 0; j < cols; j++)
                {
                    for (int i = 1; i < rows; i++)
                    {
                        if (matrix[i][j].CompareTo(min[j]) < 0)
                        {
                            min[j] = matrix[i][j];
                            imin[j] = i;
                        }
                    }
                }
            }

            return min;
        }

        /// <summary>
        ///   Gets the range of the values in a vector.
        /// </summary>
        public static DoubleRange Range(this double[] array)
        {
            if (array.Length == 0)
                return new DoubleRange(0, 0);

            double min = array[0];
            double max = array[0];

            for (int i = 1; i < array.Length; i++)
            {
                if (min > array[i])
                    min = array[i];
                if (max < array[i])
                    max = array[i];
            }
            return new DoubleRange(min, max);
        }

        /// <summary>
        ///   Gets the range of the values in a vector.
        /// </summary>
        public static IntRange Range(this int[] array)
        {
            if (array.Length == 0)
                return new IntRange(0, 0);

            int min = array[0];
            int max = array[0];

            for (int i = 1; i < array.Length; i++)
            {
                if (min > array[i])
                    min = array[i];
                if (max < array[i])
                    max = array[i];
            }
            return new IntRange(min, max);
        }

        /// <summary>
        ///   Gets the range of the values accross the columns of a matrix.
        /// </summary>
        public static DoubleRange[] Range(double[,] value)
        {
            var ranges = new DoubleRange[value.GetLength(1)];

            for (int j = 0; j < ranges.Length; j++)
            {
                double max = value[0, j];
                double min = value[0, j];

                for (int i = 0; i < value.GetLength(0); i++)
                {
                    if (value[i, j] > max)
                        max = value[i, j];

                    if (value[i, j] < min)
                        min = value[i, j];
                }

                ranges[j] = new DoubleRange(min, max);
            }

            return ranges;
        }

        #endregion

        /// <summary>
        ///   Sorts the columns of a matrix by sorting keys.
        /// </summary>
        /// <param name="keys">The key value for each column.</param>
        /// <param name="values">The matrix to be sorted.</param>
        /// <param name="comparer">The comparer to use.</param>
        public static TValue[,] Sort<TKey, TValue>(TKey[] keys, TValue[,] values, IComparer<TKey> comparer)
        {
            var indices = new int[keys.Length];
            for (int i = 0; i < keys.Length; i++) indices[i] = i;

            Array.Sort(keys, indices, comparer);

            return values.Submatrix(0, values.GetLength(0) - 1, indices);
        }
    }
}