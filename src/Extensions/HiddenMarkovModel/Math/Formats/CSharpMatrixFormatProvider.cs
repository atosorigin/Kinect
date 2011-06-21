// Accord Math Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using System.Globalization;

namespace Accord.Math.Formats
{
    /// <summary>
    ///   Gets the matrix representation used in C# multi-dimensional arrays.
    /// </summary>
    /// 
    public sealed class CSharpMatrixFormatProvider : MatrixFormatProviderBase
    {
        /// <summary>
        ///   Gets the IMatrixFormatProvider which uses the CultureInfo used by the current thread.
        /// </summary>
        /// 
        public static readonly CSharpMatrixFormatProvider CurrentCulture =
            new CSharpMatrixFormatProvider(CultureInfo.CurrentCulture);

        /// <summary>
        ///   Gets the IMatrixFormatProvider which uses the invariant system culture.
        /// </summary>
        /// 
        public static readonly CSharpMatrixFormatProvider InvariantCulture =
            new CSharpMatrixFormatProvider(CultureInfo.InvariantCulture);

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpMatrixFormatProvider"/> class.
        /// </summary>
        public CSharpMatrixFormatProvider(CultureInfo culture)
            : base(culture)
        {
            FormatMatrixStart = "new double[,] {\n";
            FormatMatrixEnd = " \n};";
            FormatRowStart = "    { ";
            FormatRowEnd = " }";
            FormatColStart = ", ";
            FormatColEnd = ", ";
            FormatRowDelimiter = ",\n";
            FormatColDelimiter = ", ";

            ParseMatrixStart = "new double[,] {";
            ParseMatrixEnd = "};";
            ParseRowStart = "{";
            ParseRowEnd = "}";
            ParseColStart = ",";
            ParseColEnd = ",";
            ParseRowDelimiter = "},";
            ParseColDelimiter = ",";
        }
    }
}