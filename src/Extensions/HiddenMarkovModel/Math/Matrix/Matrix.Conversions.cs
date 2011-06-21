// Accord Math Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;
using System.Data;
using System.Globalization;

namespace Accord.Math
{
    /// <summary>
    /// Static class Matrix. Defines a set of extension methods
    /// that operates mainly on multidimensional arrays and vectors.
    /// </summary>
    public static partial class Matrix
    {
        /// <summary>
        ///   Converts a jagged-array into a multidimensional array.
        /// </summary>
        public static T[,] ToMatrix<T>(this T[][] array)
        {
            return ToMatrix(array, false);
        }

        /// <summary>
        ///   Converts a jagged-array into a multidimensional array.
        /// </summary>
        public static T[,] ToMatrix<T>(this T[][] array, bool transpose)
        {
            int rows = array.Length;
            if (rows == 0) return new T[0,rows];
            int cols = array[0].Length;

            T[,] m;

            if (transpose)
            {
                m = new T[cols,rows];
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        m[j, i] = array[i][j];
            }
            else
            {
                m = new T[rows,cols];
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        m[i, j] = array[i][j];
            }

            return m;
        }

        /// <summary>
        ///   Converts an array into a multidimensional array.
        /// </summary>
        public static T[,] ToMatrix<T>(this T[] array)
        {
            var m = new T[1,array.Length];
            for (int i = 0; i < array.Length; i++)
                m[0, i] = array[i];

            return m;
        }

        /// <summary>
        ///   Converts a multidimensional array into a jagged-array.
        /// </summary>
        public static T[][] ToArray<T>(this T[,] matrix)
        {
            return ToArray(matrix, false);
        }

        /// <summary>
        ///   Converts a multidimensional array into a jagged-array.
        /// </summary>
        public static T[][] ToArray<T>(this T[,] matrix, bool transpose)
        {
            T[][] array;

            if (transpose)
            {
                int cols = matrix.GetLength(1);

                array = new T[cols][];
                for (int i = 0; i < cols; i++)
                    array[i] = matrix.GetColumn(i);
            }
            else
            {
                int rows = matrix.GetLength(0);

                array = new T[rows][];
                for (int i = 0; i < rows; i++)
                    array[i] = matrix.GetRow(i);
            }

            return array;
        }

        #region Type conversions

        /// <summary>
        ///   Converts a double-precision floating point multidimensional
        ///   array into a single-precision floating point multidimensional
        ///   array.
        /// </summary>
        public static unsafe double[,] ToDouble(this float[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int length = matrix.Length;
            var r = new double[rows,cols];

            fixed (float* srcPtr = matrix)
            fixed (double* dstPtr = r)
            {
                float* src = srcPtr;
                double* dst = dstPtr;

                for (int i = 0; i < length; i++, src++, dst++)
                    *dst = *src;
            }

            return r;
        }

        /// <summary>
        ///   Converts a single-precision floating point multidimensional
        ///   array into a double-precision floating point multidimensional
        ///   array.
        /// </summary>
        public static unsafe float[,] ToSingle(this double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int length = matrix.Length;
            var r = new float[rows,cols];

            fixed (double* srcPtr = matrix)
            fixed (float* dstPtr = r)
            {
                double* src = srcPtr;
                float* dst = dstPtr;

                for (int i = 0; i < length; i++, src++, dst++)
                    *dst = (float) *src;
            }

            return r;
        }

        #endregion

        #region DataTable Conversions

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        public static double[,] ToMatrix(this DataTable table)
        {
            String[] names;
            return ToMatrix(table, out names);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        public static double[,] ToMatrix(this DataTable table, out string[] columnNames)
        {
            var m = new double[table.Rows.Count,table.Columns.Count];
            columnNames = new string[table.Columns.Count];

            for (int j = 0; j < table.Columns.Count; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                    m[i, j] = convertToDouble(table.Rows[i][j]);

                columnNames[j] = table.Columns[j].Caption;
            }
            return m;
        }


        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        public static double[][] ToArray(this DataTable table)
        {
            String[] names;
            return ToArray(table, out names);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        public static double[][] ToArray(this DataTable table, out string[] columnNames)
        {
            var m = new double[table.Rows.Count][];
            columnNames = new string[table.Columns.Count];

            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = new double[table.Columns.Count];

            for (int j = 0; j < table.Columns.Count; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                    m[i][j] = convertToDouble(table.Rows[i][j]);

                columnNames[j] = table.Columns[j].Caption;
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataColumn to a double[] array.
        /// </summary>
        public static double[] ToArray(this DataColumn column)
        {
            var m = new double[column.Table.Rows.Count];

            for (int i = 0; i < m.Length; i++)
                m[i] = convertToDouble(column.Table.Rows[i][column]);

            return m;
        }

        #endregion

        #region private methods

        private static double convertToDouble(object obj)
        {
            double d;

            if (obj is String)
            {
                d = Double.Parse((String) obj, CultureInfo.InvariantCulture);
            }
            else if (obj is Boolean)
            {
                d = (Boolean) obj ? 1.0 : 0.0;
            }
            else
            {
                try
                {
                    d = Convert.ToDouble(obj);
                }
                catch (InvalidCastException)
                {
                    d = 0;
                }
            }

            return d;
        }

        #endregion
    }
}