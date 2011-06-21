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
    ///   Format provider for the matrix format used by Octave (and MATLAB).
    /// </summary>
    /// 
    public sealed class OctaveMatrixFormatProvider : MatrixFormatProviderBase
    {
        /// <summary>
        ///   Gets the IMatrixFormatProvider which uses the CultureInfo used by the current thread.
        /// </summary>
        /// 
        public static readonly OctaveMatrixFormatProvider CurrentCulture =
            new OctaveMatrixFormatProvider(CultureInfo.CurrentCulture);

        /// <summary>
        ///   Gets the IMatrixFormatProvider which uses the invariant system culture.
        /// </summary>
        /// 
        public static readonly OctaveMatrixFormatProvider InvariantCulture =
            new OctaveMatrixFormatProvider(CultureInfo.InvariantCulture);

        /// <summary>
        /// Initializes a new instance of the <see cref="OctaveMatrixFormatProvider"/> class.
        /// </summary>
        public OctaveMatrixFormatProvider(CultureInfo culture)
            : base(culture)
        {
            FormatMatrixStart = "[";
            FormatMatrixEnd = "]";
            FormatRowStart = String.Empty;
            FormatRowEnd = String.Empty;
            FormatColStart = String.Empty;
            FormatColEnd = String.Empty;
            FormatRowDelimiter = "; ";
            FormatColDelimiter = " ";

            ParseMatrixStart = "[";
            ParseMatrixEnd = "]";
            ParseRowStart = String.Empty;
            ParseRowEnd = String.Empty;
            ParseColStart = String.Empty;
            ParseColEnd = String.Empty;
            ParseRowDelimiter = "; ";
            ParseColDelimiter = " ";
        }
    }
}