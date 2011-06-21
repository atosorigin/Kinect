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
    ///   Common interface for Matrix format providers.
    /// </summary>
    /// 
    public interface IMatrixFormatProvider : IFormatProvider
    {
        #region Formatting specification

        /// <summary>A string denoting the start of the matrix to be used in formatting.</summary>
        string FormatMatrixStart { get; }

        /// <summary>A string denoting the end of the matrix to be used in formatting.</summary>
        string FormatMatrixEnd { get; }

        /// <summary>A string denoting the start of a matrix row to be used in formatting.</summary>
        string FormatRowStart { get; }

        /// <summary>A string denoting the end of a matrix row to be used in formatting.</summary>
        string FormatRowEnd { get; }

        /// <summary>A string denoting the start of a matrix column to be used in formatting.</summary>
        string FormatColStart { get; }

        /// <summary>A string denoting the end of a matrix column to be used in formatting.</summary>
        string FormatColEnd { get; }

        /// <summary>A string containing the row delimiter for a matrix to be used in formatting.</summary>
        string FormatRowDelimiter { get; }

        /// <summary>A string containing the column delimiter for a matrix to be used in formatting.</summary>
        string FormatColDelimiter { get; }

        #endregion

        #region Parsing specification

        /// <summary>A string denoting the start of the matrix to be used in parsing.</summary>
        string ParseMatrixStart { get; }

        /// <summary>A string denoting the end of the matrix to be used in parsing.</summary>
        string ParseMatrixEnd { get; }

        /// <summary>A string denoting the start of a matrix row to be used in parsing.</summary>
        string ParseRowStart { get; }

        /// <summary>A string denoting the end of a matrix row to be used in parsing.</summary>
        string ParseRowEnd { get; }

        /// <summary>A string denoting the start of a matrix column to be used in parsing.</summary>
        string ParseColStart { get; }

        /// <summary>A string denoting the end of a matrix column to be used in parsing.</summary>
        string ParseColEnd { get; }

        /// <summary>A string containing the row delimiter for a matrix to be used in parsing.</summary>
        string ParseRowDelimiter { get; }

        /// <summary>A string containing the column delimiter for a matrix to be used in parsing.</summary>
        string ParseColDelimiter { get; }

        #endregion

        /// <summary>
        ///   Gets the culture specific formatting information
        ///   to be used during parsing or formatting.
        /// </summary>
        CultureInfo CultureInfo { get; }
    }
}