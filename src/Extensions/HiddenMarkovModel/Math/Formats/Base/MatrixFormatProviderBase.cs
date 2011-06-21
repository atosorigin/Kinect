// Accord Math Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System;
using System.Globalization;

namespace Accord.Math.Formats
{
    /// <summary>
    ///   Base class for IMatrixFormatProvider implementors.
    /// </summary>
    /// 
    public abstract class MatrixFormatProviderBase : IMatrixFormatProvider
    {
        #region Formatting specification

        /// <summary>
        /// A string denoting the start of the matrix to be used in formatting.
        /// </summary>
        public string FormatMatrixStart { get; protected set; }

        /// <summary>
        /// A string denoting the end of the matrix to be used in formatting.
        /// </summary>
        public string FormatMatrixEnd { get; protected set; }

        /// <summary>
        /// A string denoting the start of a matrix row to be used in formatting.
        /// </summary>
        public string FormatRowStart { get; protected set; }

        /// <summary>
        /// A string denoting the end of a matrix row to be used in formatting.
        /// </summary>
        public string FormatRowEnd { get; protected set; }

        /// <summary>
        /// A string denoting the start of a matrix column to be used in formatting.
        /// </summary>
        public string FormatColStart { get; protected set; }

        /// <summary>
        /// A string denoting the end of a matrix column to be used in formatting.
        /// </summary>
        public string FormatColEnd { get; protected set; }

        /// <summary>
        /// A string containing the row delimiter for a matrix to be used in formatting.
        /// </summary>
        public string FormatRowDelimiter { get; protected set; }

        /// <summary>
        /// A string containing the column delimiter for a matrix to be used in formatting.
        /// </summary>
        public string FormatColDelimiter { get; protected set; }

        #endregion

        #region Parsing specification

        /// <summary>
        /// A string denoting the start of the matrix to be used in parsing.
        /// </summary>
        public string ParseMatrixStart { get; protected set; }

        /// <summary>
        /// A string denoting the end of the matrix to be used in parsing.
        /// </summary>
        public string ParseMatrixEnd { get; protected set; }

        /// <summary>
        /// A string denoting the start of a matrix row to be used in parsing.
        /// </summary>
        public string ParseRowStart { get; protected set; }

        /// <summary>
        /// A string denoting the end of a matrix row to be used in parsing.
        /// </summary>
        public string ParseRowEnd { get; protected set; }

        /// <summary>
        /// A string denoting the start of a matrix column to be used in parsing.
        /// </summary>
        public string ParseColStart { get; protected set; }

        /// <summary>
        /// A string denoting the end of a matrix column to be used in parsing.
        /// </summary>
        public string ParseColEnd { get; protected set; }

        /// <summary>
        /// A string containing the row delimiter for a matrix to be used in parsing.
        /// </summary>
        public string ParseRowDelimiter { get; protected set; }

        /// <summary>
        /// A string containing the column delimiter for a matrix to be used in parsing.
        /// </summary>
        public string ParseColDelimiter { get; protected set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixFormatProviderBase"/> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        protected MatrixFormatProviderBase(CultureInfo culture)
        {
            CultureInfo = culture;
        }

        #region IMatrixFormatProvider Members

        /// <summary>
        /// Gets the culture specific formatting information
        /// to be used during parsing or formatting.
        /// </summary>
        public CultureInfo CultureInfo { get; protected set; }

        /// <summary>
        ///   Returns an object that provides formatting services for the specified
        ///   type. Currently, only <see cref="IMatrixFormatProvider"/> is supported.
        /// </summary>
        /// <param name="formatType">
        ///   An object that specifies the type of format
        ///   object to return. </param>
        /// <returns>
        ///   An instance of the object specified by formatType, if the
        ///   <see cref="IFormatProvider">IFormatProvider</see> implementation
        ///   can supply that type of object; otherwise, null.</returns>
        ///   
        public object GetFormat(Type formatType)
        {
            // Determine whether custom formatting object is requested.

            if (formatType == typeof (ICustomFormatter))
            {
                return new MatrixFormatter();
            }

            return null;
        }

        #endregion
    }
}