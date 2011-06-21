// Accord Math Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;
using Accord.Math.Formats;

namespace Accord.Math
{
    /// <summary>
    /// Static class Matrix. Defines a set of extension methods
    /// that operates mainly on multidimensional arrays and vectors.
    /// </summary>
    public static partial class Matrix
    {
        #region ToString

        /// <summary>
        ///   Returns a <see cref="System.String"/> representing a matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public static string ToString(this double[,] matrix)
        {
            return ToString(matrix, DefaultMatrixFormatProvider.CurrentCulture);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that representing a matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="multiline">
        ///   If set to <c>true</c>, the matrix will be written using multiple
        ///   lines. If set to <c>false</c>, the matrix will be written in a 
        ///   single line.</param>
        /// <param name="provider">
        ///   The <see cref="IMatrixFormatProvider"/> to be used
        ///   when creating the resulting string. Default is to use
        ///   <see cref="DefaultMatrixFormatProvider.CurrentCulture"/>.
        /// </param>
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public static string ToString(this double[,] matrix, bool multiline, IMatrixFormatProvider provider)
        {
            if (multiline)
            {
                return ToString(matrix, Environment.NewLine, provider);
            }
            else
            {
                return ToString(matrix, null, provider);
            }
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that representing a matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="provider">
        ///   The <see cref="IMatrixFormatProvider"/> to be used
        ///   when creating the resulting string. Default is to use
        ///   <see cref="DefaultMatrixFormatProvider.CurrentCulture"/>.
        /// </param>
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public static string ToString(this double[,] matrix, IMatrixFormatProvider provider)
        {
            return ToString(matrix, null, provider);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that representing a matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="format">
        ///   The format to use when creating the resulting string.
        /// </param>
        /// <param name="provider">
        ///   The <see cref="IMatrixFormatProvider"/> to be used
        ///   when creating the resulting string. Default is to use
        ///   <see cref="DefaultMatrixFormatProvider.CurrentCulture"/>.
        /// </param>
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public static string ToString(this double[,] matrix, string format, IMatrixFormatProvider provider)
        {
            return MatrixFormatter.Format(format, matrix, provider);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that representing a matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public static string ToString(this double[][] matrix)
        {
            return ToString(matrix, DefaultMatrixFormatProvider.CurrentCulture);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that representing a matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="provider">
        ///   The <see cref="IMatrixFormatProvider"/> to be used
        ///   when creating the resulting string. Default is to use
        ///   <see cref="DefaultMatrixFormatProvider.CurrentCulture"/>.
        /// </param>
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public static string ToString(this double[][] matrix, IMatrixFormatProvider provider)
        {
            return ToString(matrix, null, provider);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that representing a matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="format">
        ///   The format to use when creating the resulting string.
        /// </param>
        /// <param name="provider">
        ///   The <see cref="IMatrixFormatProvider"/> to be used
        ///   when creating the resulting string. Default is to use
        ///   <see cref="DefaultMatrixFormatProvider.CurrentCulture"/>.
        /// </param>
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public static string ToString(this double[][] matrix, string format, IMatrixFormatProvider provider)
        {
            return MatrixFormatter.Format(format, matrix, provider);
        }

        #endregion

        #region Parse

        /// <summary>
        ///   Converts the string representation of a matrix to its
        ///   double-precision floating-point number matrix equivalent.
        /// </summary>
        /// <param name="str">The string representation of the matrix.</param>
        /// <returns>A double-precision floating-point number matrix parsed
        /// from the given string using the given format provider.</returns>
        /// 
        public static double[,] Parse(string str)
        {
            return MatrixFormatter.ParseMultidimensional(str, DefaultMatrixFormatProvider.CurrentCulture);
        }

        /// <summary>
        ///   Converts the string representation of a matrix to its
        ///   double-precision floating-point number matrix equivalent.
        /// </summary>
        /// <param name="str">The string representation of the matrix.</param>
        /// <param name="provider">
        ///   The format provider to use in the conversion. Default is to use
        ///   <see cref="DefaultMatrixFormatProvider.CurrentCulture"/>.
        /// </param>
        /// <returns>A double-precision floating-point number matrix parsed
        /// from the given string using the given format provider.</returns>
        /// 
        public static double[,] Parse(string str, IMatrixFormatProvider provider)
        {
            return MatrixFormatter.ParseMultidimensional(str, provider);
        }

        /// <summary>
        ///   Converts the string representation of a matrix to its
        ///   double-precision floating-point number matrix equivalent.
        /// </summary>
        /// <param name="s">The string representation of the matrix.</param>
        /// <param name="provider">
        ///   The format provider to use in the conversion. Default is to use
        ///   <see cref="DefaultMatrixFormatProvider.CurrentCulture"/>.
        /// </param>
        /// <returns>A double-precision floating-point number matrix parsed
        /// from the given string using the given format provider.</returns>
        /// 
        public static double[][] ParseJagged(string s, IMatrixFormatProvider provider)
        {
            return MatrixFormatter.ParseJagged(s, provider);
        }

        /// <summary>
        ///   Converts the string representation of a matrix to its
        ///   double-precision floating-point number matrix equivalent.
        ///   A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="s">The string representation of the matrix.</param>
        /// <param name="provider">
        ///   The format provider to use in the conversion. Default is to use
        ///   <see cref="DefaultMatrixFormatProvider.CurrentCulture"/>.
        /// </param>
        /// <param name="matrix">A double-precision floating-point number matrix parsed
        /// from the given string using the given format provider.</param>
        /// <result>When this method returns, contains the double-precision floating-point
        /// number matrix equivalent to the <see param="s"/> parameter, if the conversion succeeded, 
        /// or null if the conversion failed. The conversion fails if the <see param="s"/> parameter
        /// is null, is not a matrix in a valid format, or contains elements which represent
        /// a number less than MinValue or greater than MaxValue. This parameter is passed
        /// uninitialized. </result>
        /// 
        public static bool TryParse(string s, IMatrixFormatProvider provider, out double[,] matrix)
        {
            // TODO: Create a proper TryParse method without
            //       resorting to a underlying try-catch block.
            try
            {
                matrix = Parse(s, provider);
            }
            catch (FormatException)
            {
                matrix = null;
            }
            catch (ArgumentNullException)
            {
                matrix = null;
            }

            return matrix != null;
        }

        /// <summary>
        ///   Converts the string representation of a matrix to its
        ///   double-precision floating-point number matrix equivalent.
        ///   A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="s">The string representation of the matrix.</param>
        /// <param name="provider">
        ///   The format provider to use in the conversion. Default is to use
        ///   <see cref="DefaultMatrixFormatProvider.CurrentCulture"/>.
        /// </param>
        /// <param name="matrix">A double-precision floating-point number matrix parsed
        /// from the given string using the given format provider.</param>
        /// <result>When this method returns, contains the double-precision floating-point
        /// number matrix equivalent to the <see param="s"/> parameter, if the conversion succeeded, 
        /// or null if the conversion failed. The conversion fails if the <see param="s"/> parameter
        /// is null, is not a matrix in a valid format, or contains elements which represent
        /// a number less than MinValue or greater than MaxValue. This parameter is passed
        /// uninitialized. </result>
        /// 
        public static bool TryParse(string s, IMatrixFormatProvider provider, out double[][] matrix)
        {
            // TODO: Create a proper TryParse method without
            //       resorting to a underlying try-catch block.
            try
            {
                matrix = ParseJagged(s, provider);
            }
            catch (FormatException)
            {
                matrix = null;
            }
            catch (ArgumentNullException)
            {
                matrix = null;
            }

            return matrix != null;
        }

        #endregion
    }
}