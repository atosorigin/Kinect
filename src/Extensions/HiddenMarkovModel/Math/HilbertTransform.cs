// Accord Math Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
// http://www.crsouza.com
//

using AForge.Math;

namespace Accord.Math
{
    /// <summary>
    ///   Hilbert Transformation.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The Fast Hilbert transform is a time-domain to time-domain transformation which
    ///   shifts the phase of a signal by 90 degrees. Positive frequency components are
    ///   shifted by +90 degrees, and negative frequency components are shifted by –90
    ///   degrees. Applying a Hilbert transform to a signal twice in succession shifts
    ///   the phases of all of the components by 180 degrees, and so produces the negative
    ///   of the original signal.</para>
    /// <para>
    ///   The hilbert transform may be implemented efficiently using the fast Fourier
    ///   transform. Following Fourier transformation, the negative frequencies are
    ///   zeroed. An inverse Fourier transform will then yield a 90-degree-phase-shifted
    ///   version of the original waveform. Each corresponding pair of samples from these
    ///   two waveforms are interpreted as Cartesian coordinates for Cartesian-to-polar
    ///   coordinate conversion. The resulting angular and magnitude values are the
    ///   instantaneous phase and amplitude values.</para>
    ///   
    ///   <para>
    ///     References:
    ///     <list type="bullet">
    ///       <item><description>
    ///         <a href="http://www.scholarpedia.org/article/Hilbert_transform_for_brain_waves">
    ///         http://www.scholarpedia.org/article/Hilbert_transform_for_brain_waves</a>
    ///       </description></item>
    ///     </list>
    ///   </para>
    /// </remarks>
    /// 
    public static class HilbertTransform
    {
        /// <summary>
        ///   Performs the transformation over a double[] array.
        /// </summary>
        public static void FHT(double[] data, FourierTransform.Direction direction)
        {
            int N = data.Length;


            // Forward operation
            if (direction == FourierTransform.Direction.Forward)
            {
                // Copy the input to a complex array which can be processed
                //  in the complex domain by the FFT
                var cdata = new Complex[N];
                for (int i = 0; i < N; i++)
                    cdata[i].Re = data[i];

                // Perform FFT
                FourierTransform.FFT(cdata, FourierTransform.Direction.Forward);

                //double positive frequencies
                for (int i = 1; i < (N/2); i++)
                {
                    cdata[i].Re *= 2.0;
                    cdata[i].Im *= 2.0;
                }

                // zero out negative frequencies
                //  (leaving out the dc component)
                for (int i = (N/2) + 1; i < N; i++)
                {
                    cdata[i].Re = 0.0;
                    cdata[i].Im = 0.0;
                }

                // Reverse the FFT
                FourierTransform.FFT(cdata, FourierTransform.Direction.Backward);

                // Convert back to our initial double array
                for (int i = 0; i < N; i++)
                    data[i] = cdata[i].Im;
            }

            else // Backward operation
            {
                // The inverse Hilbert can be calculated by
                //  negating the transform and reapplying the
                //  transformation.
                //
                // H^–1{h(t)} = –H{h(t)}

                FHT(data, FourierTransform.Direction.Forward);

                for (int i = 0; i < data.Length; i++)
                    data[i] = -data[i];
            }
        }


        /// <summary>
        ///   Performs the transformation over a complex[] array.
        /// </summary>
        public static void FHT(Complex[] data, FourierTransform.Direction direction)
        {
            int N = data.Length;

            // Forward operation
            if (direction == FourierTransform.Direction.Forward)
            {
                // Makes a copy of the data so we don't lose the
                //  original information to build our final signal
                var shift = (Complex[]) data.Clone();

                // Perform FFT
                FourierTransform.FFT(shift, FourierTransform.Direction.Backward);

                //double positive frequencies
                for (int i = 1; i < (N/2); i++)
                {
                    shift[i].Re *= 2.0;
                    shift[i].Im *= 2.0;
                }
                // zero out negative frequencies
                //  (leaving out the dc component)
                for (int i = (N/2) + 1; i < N; i++)
                {
                    shift[i].Re = 0.0;
                    shift[i].Im = 0.0;
                }

                // Reverse the FFT
                FourierTransform.FFT(shift, FourierTransform.Direction.Forward);

                // Put the Hilbert transform in the Imaginary part
                //  of the input signal, creating a Analytic Signal
                for (int i = 0; i < N; i++)
                    data[i].Im = shift[i].Im;
            }

            else // Backward operation
            {
                // Just discard the imaginary part
                for (int i = 0; i < data.Length; i++)
                    data[i].Im = 0.0;
            }
        }
    }
}