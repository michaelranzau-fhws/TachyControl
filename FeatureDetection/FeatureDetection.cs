using System;
using System.Drawing;
using System.Linq;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV;
using Emgu.CV.Util;
using System.Xml;
using System.Xml.Serialization;

namespace Features
{
    /// <summary>
    /// Finding features in images.
    /// </summary>
    public static class FeatureDetection
    {
        // ##### Static properties

        /// <summary>
        /// Threshold for distance from found point to approximated point, in [px]
        /// </summary>
        public static double approxThresh = 30;
        private static int _lineWeight = 1;
        /// <summary>
        /// Width of line for drawings, in [px]
        /// </summary>
        public static int LineWeight
        {
            get { return _lineWeight; }
            set { _lineWeight = value; }
        }

        private static MCvScalar _lineColor = new MCvScalar(255, 255, 255);

        /// <summary>
        /// Colour of line for drawings, in R[byte], G[byte], B[byte]
        /// </summary>
        public static MCvScalar LineColor
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        /// <summary>
        /// Find all spheres in an Mat image.
        /// </summary>
        /// <param name="reference">Image as Emgu.CV.Mat</param>
        /// <returns>Sphere2D-Array of found spheres.</returns>
        public static Sphere2D[] findSphere(Mat reference)
        {
            Mat src = toGray(reference); // works with gray images
            Mat dst = src.Clone();
            CvInvoke.GaussianBlur(src, dst, new Size(3, 3), 0); // Smooth image to reduce image noise

            // Mean and std deviation of the image
            MCvScalar mean = new MCvScalar();
            MCvScalar stdDev = new MCvScalar();
            CvInvoke.MeanStdDev(dst, ref mean, ref stdDev);

            // Background substraction by thresholding
            int CenterOfMass_Background_factor = 5;
            // Make binary image
            CvInvoke.Threshold(dst, dst, mean.V0 + stdDev.V0 * CenterOfMass_Background_factor, 255, ThresholdType.Binary);
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            // Find connected pixel areas
            CvInvoke.FindContours(dst, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxNone);

            Sphere2D[] s = new Sphere2D[contours.Size];
            double radius = 0;
            double area = 0;
            int count = 0;
            for (int i = 0; i < contours.Size; i++)
            {
                MCvMoments mu = CvInvoke.Moments(contours[i]);
                MCvPoint2D64f p = mu.GravityCenter;
                area = mu.M00;
                radius = Math.Sqrt(area / Math.PI);
                // If sphere is completly in the image and the area bigger than 99 px.
                if (area >= 100 &&  p.X > radius && p.X < src.Width - radius && p.Y > radius && p.Y < src.Height - radius)
                {
                    // Compute the center of gravity
                    double xc, yc, sigmaX, sigmaY, sigma;
                    if (fit2DGaussian(src, p.X, p.Y, area, out xc, out yc, out sigmaX, out sigmaY, out sigma)) 
                    {
                        s[count] = new Sphere2D((float)(xc), (float)(yc), (float)radius * 2, (float) sigma, (float)sigmaX, (float)sigmaY);
                        count++;
                    }
                }
            }
            Sphere2D[] s2 = new Sphere2D[count];
            for (int i = 0; i < count; i++)
            {
                s2[i] = s[i];
            }
            return s2;
        }
        /// <summary>
        /// Gauss2D: a*e^(-((x^2+y^2)/(2r^2)))
        /// </summary>
        /// <param name="srcMat">Image</param>
        /// <param name="xc_0">Center: approximated x</param>
        /// <param name="yc_0">Center: approximated y</param>
        /// <param name="area">Approximated area</param>
        /// <param name="xc">Compensated center: x</param>
        /// <param name="yc">Compensated center: y</param>
        /// <param name="sigmaX">Standard deviation: x</param>
        /// <param name="sigmaY">Standard deviation: y</param>
        /// <param name="sigma">Standard deviation</param>
        /// <returns>true: successful</returns>
        private static bool fit2DGaussian(Mat srcMat, double xc_0, double yc_0, double area, out double xc,
            out double yc, out double sigmaX, out double sigmaY, out double sigma)
        {
            sigmaX = -1;
            sigmaY = -1;
            sigma = -1;
            double a_0;
            double b_0;
            double r_0;

            Image<Gray, Byte> src = srcMat.ToImage < Gray, Byte> ();

            // Calculate the approximated unknowns
            a_0 = src.Data[(int)yc_0, (int)xc_0, 0];
            r_0 = Math.Sqrt(area / Math.PI / 2);

            int xc_0_int = (int)xc_0;
            int yc_0_int = (int)yc_0;
            int half_width = (int)r_0 * 5;
            int nbrObs = (2 * half_width + 1) * (2 * half_width + 1);
            int nbrUnknown = 5;
            int first_x = xc_0_int - half_width;
            int first_y = yc_0_int - half_width;
            int last_x = xc_0_int + half_width;
            int last_y = yc_0_int + half_width;
            b_0 = src.Data[first_y, first_x, 0];
            a_0 = a_0 - b_0;

            bool cont = true;
            bool ok = true;

            if (first_x <= 50 || first_y <= 50 || last_x >= src.Width - 50 || last_y >= src.Height - 50 || half_width == 0 || area < 100)
            {
                cont = false;
                ok = false;
            }
            Image<Gray, float> m_A = new Mat(nbrObs, nbrUnknown, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_AT = new Mat(nbrUnknown, nbrObs, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_ATA = new Mat(nbrUnknown, nbrUnknown, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_Qxx = new Mat(nbrUnknown, nbrUnknown, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_ATL = new Mat(nbrUnknown, 1, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_dX = new Mat(nbrUnknown, 1, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_L = new Mat(nbrObs, 1, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_AdX = new Mat(nbrObs, 1, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_V = new Mat(nbrObs, 1, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_VT = new Mat(1, nbrObs, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_VTV = new Mat(1, 1, DepthType.Cv32F, 1).ToImage<Gray, float>();

            int kk = 0;
            while (cont)
            {
                kk += 1;
                // A
                int k = 0;
                for (int i = first_x; i < last_x + 1; i++)
                {
                    for (int j = first_y; j < last_y + 1; j++)
                    {
                        // r_0 = 2 * sigma
                        float exp_0 = (float)Math.Exp(-((Math.Pow(i - xc_0, 2) + Math.Pow(j - yc_0, 2)) / (Math.Pow(r_0, 2))));
                        m_A.Data[k, 0, 0] = exp_0;
                        m_A.Data[k, 1, 0] = 1;
                        m_A.Data[k, 2, 0] = (float)(a_0 * exp_0 * 2 * (i - xc_0) / Math.Pow(r_0, 2));
                        m_A.Data[k, 3, 0] = (float)(a_0 * exp_0 * 2 * (j - yc_0) / Math.Pow(r_0, 2));
                        m_A.Data[k, 4, 0] = (float)(a_0 * exp_0 * 2 * (Math.Pow(i - xc_0, 2) + Math.Pow(j - yc_0, 2)) / Math.Pow(r_0, 3));
                        k += 1;
                    }
                }

                // L
                k = 0;
                for (int i = first_x; i < last_x + 1; i++)
                {
                    for (int j = first_y; j < last_y + 1; j++)
                    {
                        double l_0 = b_0 + a_0 * Math.Exp(-((Math.Pow(i - xc_0, 2) + Math.Pow(j - yc_0, 2)) / (Math.Pow(r_0, 2))));
                        m_L.Data[k, 0, 0] = (float)(src.Data[j, i, 0] - l_0);
                        k += 1;
                    }
                }

                // Calculate the corrections
                CvInvoke.Transpose(m_A, m_AT);
                CvInvoke.Gemm(m_AT, m_A, 1, null, 1, m_ATA);
                CvInvoke.Invert(m_ATA, m_Qxx, DecompMethod.LU);
                CvInvoke.Gemm(m_AT, m_L, 1, null, 1, m_ATL);
                CvInvoke.Gemm(m_Qxx, m_ATL, 1, null, 1, m_dX);

                a_0 += m_dX.Data[0, 0, 0];
                b_0 += m_dX.Data[1, 0, 0];
                xc_0 += m_dX.Data[2, 0, 0];
                yc_0 += m_dX.Data[3, 0, 0];
                r_0 += m_dX.Data[4, 0, 0];

                if (kk < 20 && (Math.Abs(m_dX.Data[0, 0, 0]) > 0.02 ||
                    Math.Abs(m_dX.Data[1, 0, 0]) > 0.02 || Math.Abs(m_dX.Data[2, 0, 0]) > 0.02 ||
                    Math.Abs(m_dX.Data[3, 0, 0]) > 0.02 || Math.Abs(m_dX.Data[4, 0, 0]) > 0.02))
                {
                    cont = true;
                }
                else
                {
                    cont = false;
                    if (kk < 19)
                    {
                        ok = true;
                    }
                    else
                    {
                        ok = false;
                    }
                }

            }

            if (ok)
            {
                // Standard deviation
                CvInvoke.Gemm(m_A, m_dX, 1, null, 1, m_AdX);
                CvInvoke.cvConvertScale(m_L, m_V, -1.0, 0);
                CvInvoke.Add(m_V, m_AdX, m_V);
                CvInvoke.Transpose(m_V, m_VT);
                CvInvoke.Gemm(m_VT, m_V, 1, null, 1, m_VTV);


                double s0 = Math.Sqrt(m_VTV.Data[0, 0, 0] / (nbrObs - nbrUnknown));
                double s_x = s0 * Math.Sqrt(m_Qxx.Data[2,2,0]);
                double s_y = s0 * Math.Sqrt(m_Qxx.Data[3, 3, 0]);
                sigmaX = s_x;
                sigmaY = s_y;
                sigma = Math.Sqrt(Math.Pow(s_x, 2) + Math.Pow(s_y, 2));
                // ###
                // Abortion criterion could be adjustable.
                // ###
                if (s_x >= 0.5 || s_y >= 0.5)
                {
                    ok = false;
                }
            }

            xc = xc_0;
            yc = yc_0;

            return ok;
        }

        /// <summary>
        /// Find all circles in an Mat image.
        /// </summary>
        /// <param name="reference">Image</param>
        /// <returns>Sphere2D-Array of found circles</returns>
        public static Sphere2D[] findCircle(Mat reference)
        {
            Mat reference2 = toGray(reference);
            Mat src = reference2.Clone();

            CvInvoke.Canny(reference2, reference2, 200, 230, 3, false);

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(reference2, contours, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxNone);
            Sphere2D[] s = new Sphere2D[contours.Size];
            double area = 0;
            double a = 0;
            double b = 0;
            double alpha = 0;
            int count = 0;
            RotatedRect rr;
            for (int i = 0; i < contours.Size; i++)
            {
                MCvMoments mu = CvInvoke.Moments(contours[i]);
                MCvPoint2D64f p = mu.GravityCenter;
                area = mu.M00;
                if (area > 100)
                {
                    rr = CvInvoke.FitEllipse(contours[i]);
                    double dif = Math.Sqrt(Math.Pow((p.X - rr.Center.X), 2) + Math.Pow((p.Y - rr.Center.Y), 2));
                    a = rr.Size.Height;
                    b = rr.Size.Width;
                    alpha = (rr.Angle) / 180 * Math.PI;
                    double xc, yc, sigmaX, sigmaY, sigma, radius;

                    if (isCircle(contours[i], p.X, p.Y, a/2, out xc, out yc, out radius, out sigmaX, out sigmaY, out sigma))
                    {
                        if (!double.IsNaN(xc))
                        {
                            s[count] = new Sphere2D((float)xc, (float)yc, (float)(radius * 2));

                            count++;
                        }
                    }
                }
            }
            Sphere2D[] s2 = new Sphere2D[count];
            for (int i = 0; i < count; i++)
            {
                s2[i] = s[i];
            }
            return s2;
        }
        /// <summary>
        /// Fit circle into contour.
        /// </summary>
        /// <param name="contour">Contour</param>
        /// <param name="xc_0">Approximated x</param>
        /// <param name="yc_0">Approximated y</param>
        /// <param name="r">Approximated radius</param>
        /// <param name="xc">Calculated x</param>
        /// <param name="yc">Calculated y</param>
        /// <param name="radius">Calculated radius</param>
        /// <param name="sigmaX">Standard deviation: x</param>
        /// <param name="sigmaY">Standard deviation: y</param>
        /// <param name="sigma">Standard deviation</param>
        /// <returns>true: successful</returns>
        private static bool isCircle(VectorOfPoint contour, double xc_0, double yc_0, double r, out double xc,
            out double yc, out double radius, out double sigmaX, out double sigmaY, out double sigma)
        {
            sigmaX = -1;
            sigmaY = -1;
            sigma = -1;

            int nbrObs = contour.Size;
            int nbrUnknown = 3;

            bool cont = true;
            bool ok = true;

            if (nbrObs < 20)
            {
                cont = false;
                ok = false;
            }
            Image<Gray, double> m_A = new Mat(nbrObs, nbrUnknown, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_AT = new Mat(nbrUnknown, nbrObs, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_ATA = new Mat(nbrUnknown, nbrUnknown, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_Qxx = new Mat(nbrUnknown, nbrUnknown, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_ATL = new Mat(nbrUnknown, 1, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_dX = new Mat(nbrUnknown, 1, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_L = new Mat(nbrObs, 1, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_AdX = new Mat(nbrObs, 1, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_V = new Mat(nbrObs, 1, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_VT = new Mat(1, nbrObs, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_VTV = new Mat(1, 1, DepthType.Cv32F, 1).ToImage<Gray, double>();

            int kk = 0;
            while (cont)
            {
                kk += 1;
                Console.WriteLine("kk = " + kk);
                int k = 0;
                for (int i = 0; i < contour.Size; i++)
                {
                    double exp_0 = (1.0 * (contour[i].X - xc_0));
                    double exp_1 = (1.0 * (contour[i].Y - yc_0));
                    double exp_2 = (1.0 * Math.Sqrt(exp_0 * exp_0 * exp_1 * exp_1));
                    // A
                    m_A.Data[k, 0, 0] = exp_0 / exp_2;
                    m_A.Data[k, 1, 0] = exp_1 / exp_2;
                    m_A.Data[k, 2, 0] = exp_2;

                    // L
                    double l_0 = (exp_2);
                    m_L.Data[k, 0, 0] = (exp_2 - l_0);
                    if (double.IsNaN(m_A.Data[k, 0, 0]) ||
                        double.IsNaN(m_A.Data[k, 1, 0]) ||
                        double.IsNaN(m_A.Data[k, 2, 0]))
                        Console.WriteLine("NaN");
                    k += 1;
                }


                // Calculate the corrections
                CvInvoke.Transpose(m_A, m_AT);
                CvInvoke.Gemm(m_AT, m_A, 1, null, 1, m_ATA);
                CvInvoke.Invert(m_ATA, m_Qxx, DecompMethod.LU);
                CvInvoke.Gemm(m_AT, m_L, 1, null, 1, m_ATL);
                CvInvoke.Gemm(m_Qxx, m_ATL, 1, null, 1, m_dX);

                xc_0 += m_dX.Data[0, 0, 0];
                yc_0 += m_dX.Data[1, 0, 0];
                r += m_dX.Data[2, 0, 0];
                if (kk < 20 && (Math.Abs(m_dX.Data[0, 0, 0]) > 0.02 ||
                    Math.Abs(m_dX.Data[1, 0, 0]) > 0.02 || Math.Abs(m_dX.Data[2, 0, 0]) > 0.02))
                {
                    cont = true;
                }
                else
                {
                    cont = false;
                    if (kk < 19)
                    {
                        ok = true;
                    }
                    else
                    {
                        ok = false;
                    }
                }

            }
            if (ok)
            {
                // Standard deviation
                CvInvoke.Gemm(m_A, m_dX, 1, null, 1, m_AdX);
                CvInvoke.cvConvertScale(m_L, m_V, -1.0, 0);
                CvInvoke.Add(m_V, m_AdX, m_V);
                CvInvoke.Transpose(m_V, m_VT);
                CvInvoke.Gemm(m_VT, m_V, 1, null, 1, m_VTV);


                double s0 = Math.Sqrt(m_VTV.Data[0, 0, 0] / (nbrObs - nbrUnknown));
                double s_x = s0 * Math.Sqrt(m_Qxx.Data[0, 0, 0]);
                double s_y = s0 * Math.Sqrt(m_Qxx.Data[1, 1, 0]);
                sigmaX = s_x;
                sigmaY = s_y;
                sigma = Math.Sqrt(Math.Pow(s_x, 2) + Math.Pow(s_y, 2));
                if (s_x >= 0.5 || s_y >= 0.5)
                {
                    ok = false;
                }
            }

            xc = xc_0;
            yc = yc_0;
            radius = r;

            return ok;
        }
        /// <summary>
        /// Find all ellipses in an Mat image.
        /// </summary>
        /// <param name="reference">Image</param>
        /// <param name="approxEllipse">Approximated Ellipse</param>
        /// <returns>Found ellipse: successful, new clean ellipse (-1; -1): failed</returns>
        public static Ellipse2D findEllipse(Mat reference, Ellipse2D approxEllipse)
        {
            double a = 0;
            double b = 0;
            double alpha = 0;

            double minArea = 1000000;
            Ellipse2D ellipse = new Ellipse2D(-1, -1);

            Mat reference2 = toGray(reference);
            Mat src = reference2.Clone();

            CvInvoke.BitwiseNot(reference2, reference2);
            CvInvoke.Threshold(reference2, reference2, minMaxAverage8Bit(reference2), 255, ThresholdType.Binary);
            Mat element = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(3, 3), new Point(-1, -1));
            CvInvoke.MorphologyEx(reference2, reference2, MorphOp.Open, element, new Point(-1, -1), 1, BorderType.Constant, new MCvScalar(0));
            CvInvoke.Canny(reference2, reference2, 255, 240, 5, false);


            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(reference2, contours, hierarchy, RetrType.Ccomp, ChainApproxMethod.ChainApproxNone);
            Image<Bgra, int> h = hierarchy.ToImage<Bgra, int>();

            Ellipse2D[] e = new Ellipse2D[contours.Size];
            double area = 0;
            RotatedRect rr;


            for (int i = 0; i < contours.Size; i++)
            {
                MCvMoments mu = CvInvoke.Moments(contours[i]);
                MCvPoint2D64f p = mu.GravityCenter;
                area = contours[i].Size;


                if (area >= 20)
                {
                    rr = CvInvoke.FitEllipse(contours[i]);
                    double dist = distBetweenPoints(new Point2D(rr.Center), approxEllipse);
                    if (dist <= approxThresh)
                    {
                        // Take the most inner ellipse for fitting
                        if(area < minArea)
                        {
                            a = rr.Size.Height;
                            b = rr.Size.Width;
                            alpha = (rr.Angle) / 180 * Math.PI;
                            double xc, yc, b_out, a_out, angle, sigmaX, sigmaY, sigma;

                            minArea = area;

                            if (isEllipse(contours[i], p.X, p.Y, a, b, alpha, out xc, out yc, out b_out, out a_out, out angle,
                                out sigmaX, out sigmaY, out sigma))
                            {
                                ellipse = new Ellipse2D((float)xc, (float)yc, (float)(b_out), (float)(a_out), (float)(angle), (float)(sigma));

                            }
                            else
                            // If self fitting didn't work take the OpenCV ellipse without sigma
                            {
                                ellipse = new Ellipse2D((float)p.X, (float)p.Y, (float)(b), (float)(a), (float)(-alpha));
                            }

                        }
                    }
                }
            }

            // Draw ellipse and center into image and show it.
            //drawEllipse2D(src, ellipse);
            //drawCrossHair(src, new Point2D(ellipse.XInt, ellipse.YInt));
            //Image<Gray, byte> img = src.ToImage<Gray, byte>();
            //ImageViewer.Show(img);

            return ellipse;
        }
        /// <summary>
        /// Fit ellipse
        /// </summary>
        /// <param name="contour">Contour</param>
        /// <param name="xc_0">Approximated x</param>
        /// <param name="yc_0">Approximated y</param>
        /// <param name="a">Approximated a</param>
        /// <param name="b">Approximated b</param>
        /// <param name="alpha">Approximated rotation [rad]</param>
        /// <param name="xc">Calculated x</param>
        /// <param name="yc">Calculated y</param>
        /// <param name="b_out">Calculated b</param>
        /// <param name="a_out">Calculated a</param>
        /// <param name="angle">Calculated rotation [rad]</param>
        /// <param name="sigmaX">Standard deviation x</param>
        /// <param name="sigmaY">Standard deviation y</param>
        /// <param name="sigma">Standard deviation</param>
        /// <returns>true: successful</returns>
        private static bool isEllipse(VectorOfPoint contour, double xc_0, double yc_0, double height, double width, double alpha, out double xc,
            out double yc, out double b_out, out double a_out, out double angle, out double sigmaX, out double sigmaY, out double sigma)
        {
            double max_s = 10;
            xc = -1;
            yc = -1;
            angle = -1;
            b_out = -1;
            a_out = -1;

            sigmaX = -1;
            sigmaY = -1;
            sigma = -1;

            double a = height / 2;
            double b = width / 2;
            double epsi_0 = Math.Sqrt(a * a - b * b) / a;

            Point[] pp = contour.ToArray();
            Point[] pDistinct = pp.Distinct().ToArray();
            int pDLength = pDistinct.Length;

            int nbrObs = pDistinct.Length;
            int nbrUnknown = 5;

            bool cont = true;
            bool ok = true;

            if (nbrObs < 40)
            {
                cont = false;
                ok = false;
            }
            Image<Gray, double> m_A = new Mat(nbrObs, nbrUnknown, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_AT = new Mat(nbrUnknown, nbrObs, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_ATA = new Mat(nbrUnknown, nbrUnknown, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_Qxx = new Mat(nbrUnknown, nbrUnknown, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_ATL = new Mat(nbrUnknown, 1, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_dX = new Mat(nbrUnknown, 1, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_L = new Mat(nbrObs, 1, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_AdX = new Mat(nbrObs, 1, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_V = new Mat(nbrObs, 1, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_VT = new Mat(1, nbrObs, DepthType.Cv32F, 1).ToImage<Gray, double>();
            Image<Gray, double> m_VTV = new Mat(1, 1, DepthType.Cv32F, 1).ToImage<Gray, double>();

            int kk = 0;
            while (cont)
            {
                kk += 1;
                int k = 0;
                for (int i = 0; i < pDistinct.Length; i++)
                {
                    double exp_0 = (1.0 * Math.Atan2((pDistinct[i].X - xc_0), (pDistinct[i].Y - yc_0))) + Math.PI / 2;
                    double exp_1 = (1.0 * exp_0 - alpha);
                    double exp_2 = (1.0 * Math.Pow(Math.Cos(exp_1), 2));
                    double exp_3 = (1.0 * Math.Pow((1.0 - epsi_0 * epsi_0 * exp_2), 1.5));
                    double exp_4 = (1.0 * Math.Pow(1.0 - epsi_0 * epsi_0 * exp_2, 0.5));
                    double exp_5 = (1.0 * Math.Pow(pDistinct[i].X - xc_0, 2) / Math.Pow(pDistinct[i].Y - yc_0, 2) + 1);
                    double exp_6 = (1.0 * Math.Sin(exp_1));
                    // A
                    m_A.Data[k, 0, 0] = 1.0 / exp_4;
                    m_A.Data[k, 1, 0] = (1.0 * (b * exp_2 * epsi_0) / (exp_3));
                    m_A.Data[k, 2, 0] = (-1.0 * (b * epsi_0 * epsi_0 * Math.Cos(exp_1) * exp_6) /
                        ((pDistinct[i].Y - yc_0) * exp_5 * exp_3));
                    m_A.Data[k, 3, 0] = (-1.0 * (b * epsi_0 * epsi_0 * Math.Cos(exp_1) * exp_6) /
                        ((pDistinct[i].X - xc_0) * exp_5 * exp_3));
                    m_A.Data[k, 4, 0] = (-1.0 * (b * epsi_0 * epsi_0 * Math.Cos(alpha - exp_0) * Math.Sin(alpha - exp_0)) /
                        (Math.Pow(1 - epsi_0 * epsi_0 * Math.Pow(Math.Cos(alpha - exp_0), 2), 1.5)));


                    // L
                    double l_0 = (1.0 * b / exp_4);
                    m_L.Data[k, 0, 0] = Math.Sqrt(Math.Pow(pDistinct[i].X - xc_0, 2) + Math.Pow(pDistinct[i].Y - yc_0, 2)) - l_0;

                    if (double.IsNaN(m_L.Data[k, 0, 0]) ||
                        double.IsNaN(m_A.Data[k, 0, 0]) ||
                        double.IsNaN(m_A.Data[k, 1, 0]) ||
                        double.IsNaN(m_A.Data[k, 2, 0]) ||
                        double.IsNaN(m_A.Data[k, 3, 0]) ||
                        double.IsNaN(m_A.Data[k, 4, 0]))
                    {
                        ok = false;
                        return ok;
                    }
                    k += 1;
                }

                CvInvoke.Transpose(m_A, m_AT);
                CvInvoke.Gemm(m_AT, m_A, 1.0, null, 1.0, m_ATA);
                CvInvoke.Invert(m_ATA, m_Qxx, DecompMethod.LU);
                CvInvoke.Gemm(m_AT, m_L, 1.0, null, 1.0, m_ATL);
                CvInvoke.Gemm(m_Qxx, m_ATL, 1.0, null, 1.0, m_dX);

                b += m_dX.Data[0, 0, 0];
                epsi_0 += m_dX.Data[1, 0, 0];
                xc_0 += m_dX.Data[2, 0, 0];
                yc_0 += m_dX.Data[3, 0, 0];
                alpha += m_dX.Data[4, 0, 0];

                double deltaB = 0.015;
                double deltaEpsi = 0.001;
                double deltaX = 0.015;
                double deltaY = 0.005;
                double deltaAlpha = 0.001;

                if (kk < 40 && (Math.Abs(m_dX.Data[0, 0, 0]) > deltaB ||
                    Math.Abs(m_dX.Data[1, 0, 0]) > deltaEpsi || Math.Abs(m_dX.Data[2, 0, 0]) > deltaX ||
                    Math.Abs(m_dX.Data[3, 0, 0]) > deltaY || Math.Abs(m_dX.Data[4, 0, 0]) > deltaAlpha))
                {
                    cont = true;
                }
                else
                {
                    cont = false;
                    if (kk < 39)
                    {
                        ok = true;
                    }
                    else
                    {
                        ok = false;
                    }
                }
            }
            if (ok)
            {
                // Standard deviation
                CvInvoke.Gemm(m_A, m_dX, 1.0, null, 1.0, m_AdX);
                CvInvoke.cvConvertScale(m_L, m_V, -1.0, 0);
                CvInvoke.Add(m_V, m_AdX, m_V);
                CvInvoke.Transpose(m_V, m_VT);
                CvInvoke.Gemm(m_VT, m_V, 1.0, null, 1.0, m_VTV);


                double s0 = Math.Sqrt(m_VTV.Data[0, 0, 0] / (nbrObs - nbrUnknown));
                double s_x = s0 * Math.Sqrt(m_Qxx.Data[2, 2, 0]);
                double s_y = s0 * Math.Sqrt(m_Qxx.Data[3, 3, 0]);
                sigmaX = s_x;
                sigmaY = s_y;
                sigma = s0;
                if (s_x >= max_s || s_y >= max_s || sigma == 0)
                {
                    ok = false;
                }
            }

            xc = xc_0;
            yc = yc_0;
            a_out = b / (Math.Sqrt(1 - epsi_0 * epsi_0)) * 2;
            b_out = b * 2;
            angle = alpha;
            if (double.IsNaN(xc) || b_out == 0)
                ok = false;

            //Console.WriteLine("ok=" + ok + "; xc=" + xc + "; yc=" + yc + "; a_out=" + a_out + "; b_out=" + b_out +
            //    "; angle=" + angle + "; sigma=" + sigma + "; kk=" + kk +
            //    "; length=" + contour.Size + "; length2=" + pDLength);
            return ok;
        }

        /// <summary>
        /// Find the corner in Mat image.
        /// </summary>
        /// <param name="reference">Image</param>
        /// <param name="approx">Approximated corner with directions of outgoing edges.</param>
        /// <returns>Corner coordinates: successful, sigma == -1: failed.</returns>
        public static Corner2D findCorner(Mat reference, Corner2D approx)
        {
            Corner2D c2 = new Corner2D();
            int[,] cannys = new int[,] {
                { 250, 250,  5 },
                { 250,  10,  5 },
                { 250, 250,  3 },
                { 130,  10,  3 },
                {  30,  10,  3 },
                {  80,  10,  3 }
            };
            Corner2D[] cannyCorner = new Corner2D[cannys.Length / 3];

            int threshHalfSize = 50;
            double approxThreshDistance = 25;
            double approxThreshAngle = 10.0 / 180 * Math.PI;
            int thresContourSize = threshHalfSize / 8;

            Mat reference2 = toGray(reference);
            Rectangle roi = new Rectangle(approx.XInt - threshHalfSize, approx.YInt - threshHalfSize,
                threshHalfSize * 2 + 1, threshHalfSize * 2 + 1);

            // ##### Try different Cannys, find Hough lines and stop if number of found lines == approx.countLines
            for (int l = 0; l < cannys.Length / 3; l++)
            {
                Mat src = reference2.Clone();

                Mat srcROI = new Mat(src, roi);
                LineSegment2D[] lines = new LineSegment2D[approx.CountLines];

                CvInvoke.MedianBlur(srcROI, srcROI, 9);
                CvInvoke.Canny(srcROI, srcROI, cannys[l, 0], cannys[l, 1], cannys[l, 2], false);

                // ##### Find contours
                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                Mat hierarchy = new Mat();
                CvInvoke.FindContours(srcROI, contours, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxNone);
                Image<Gray, byte> iOut = new Image<Gray, byte>(srcROI.Size);
                drawContoursAsLines(iOut, contours, new MCvScalar(255), 1);

                // ##### Split contours at knick
                src = reference2.Clone();
                srcROI = new Mat(src, roi);
                contours = splitContour2Lines(srcROI, contours);

                //iOut = new Image<Gray, byte>(srcROI.Size);
                //drawContoursAsLines(iOut, contours, new MCvScalar(255), 1);

                Corner2D[] c = new Corner2D[contours.Size];

                // ##### Delete contours with size <= thres
                int countDeleteBySize = 0;
                Point[][] contours2 = new Point[contours.Size][];

                for (int i = 0; i < contours.Size; i++)
                {
                    if (contours[i].Size >= thresContourSize)
                    {
                        contours2[countDeleteBySize] = contours[i].ToArray();
                        countDeleteBySize++;
                    }
                }
                Point[][] contours3 = new Point[countDeleteBySize][];
                for (int i = 0; i < countDeleteBySize; i++)
                {
                    contours3[i] = contours2[i];
                }
                contours = new VectorOfVectorOfPoint(contours3);

                iOut = new Image<Gray, byte>(srcROI.Size);
                drawContoursAsLines(iOut, contours, new MCvScalar(255), 1);

                // ##### Compensate line
                VectorOfFloat[] linePara = new VectorOfFloat[contours.Size];
                for (int i = 0; i < contours.Size; i++)
                {
                    linePara[i] = new VectorOfFloat(4);
                    double accRaduis = 0.2;
                    double accAngle = Math.PI / 180 * 0.2;
                    // ##### Fit a straight
                    CvInvoke.FitLine(contours[i], linePara[i], DistType.L2, 0, accRaduis, accAngle);

                    // ##### Compensate, find start and end point, get accuracy
                    double x0, y0, vX, vY;
                    x0 = linePara[i][2];
                    y0 = linePara[i][3];
                    vX = linePara[i][0];
                    vY = linePara[i][1];
                }

                // ##### Merge lines
                contours = mergeLines(srcROI, contours, linePara);
                iOut = new Image<Gray, byte>(srcROI.Size);
                //drawContoursAsLines(iOut, contours, new MCvScalar(255), 1);

                // ##### Fit lines
                linePara = new VectorOfFloat[contours.Size];

                for (int i = 0; i < contours.Size; i++)
                {
                    linePara[i] = new VectorOfFloat(4);
                    double accRaduis = 0.2;
                    double accAngle = Math.PI / 180 * 0.2;
                    // ##### Fit a straight
                    CvInvoke.FitLine(contours[i], linePara[i], DistType.L2, 0, accRaduis, accAngle);

                    // ##### Compensate, find start and end point, get accuracy
                    double x0, y0, vX, vY;
                    x0 = linePara[i][2];
                    y0 = linePara[i][3];
                    vX = linePara[i][0];
                    vY = linePara[i][1];
                }

                // ##### Select lines looking like approx lines
                double[] approxLines = { approx.Line1, approx.Line2, approx.Line3 };
                Straight[] s = new Straight[contours.Size];
                Straight[] cornerLines = new Straight[3];
                for (int i = 0; i < 3; i++)
                {
                    cornerLines[i] = new Straight(0, 0);
                }
                for (int i = 0; i < contours.Size; i++)
                {
                    s[i] = new Straight(linePara[i]);
                    s[i].CountPixels = contours[i].Size;
                    double distance = Math.Abs(pointFromLine(s[i], new Point(threshHalfSize + 1, threshHalfSize + 1)));
                    double alpha = s[i].Alpha - Math.PI / 2;
                    if (alpha < Math.PI * -1) alpha += 2 * Math.PI;
                    if (alpha > Math.PI) alpha -= 2 * Math.PI;
                    double alpha2 = s[i].Alpha + Math.PI / 2;
                    if (alpha2 < Math.PI * -1) alpha2 += 2 * Math.PI;
                    if (alpha2 > Math.PI) alpha2 -= 2 * Math.PI;
                    for (int k = 0; k < approx.CountLines; k++)
                    {
                        double alpha1distance = Math.Abs(alpha - approxLines[k]);
                        if (alpha1distance < Math.PI * -1) alpha1distance += Math.PI;
                        if (alpha1distance > Math.PI) alpha1distance -= Math.PI;
                        double alpha2distance = Math.Abs(alpha2 - approxLines[k]);
                        if (alpha2distance < Math.PI * -1) alpha2distance += Math.PI;
                        if (alpha2distance > Math.PI) alpha2distance -= Math.PI;
                        // lines like approx lines?
                        if (distance <= approxThreshDistance &&
                            (alpha1distance <= approxThreshAngle ||
                            alpha2distance <= approxThreshAngle))
                        {
                            if (s[i].CountPixels > cornerLines[k].CountPixels)
                                cornerLines[k] = s[i];
                        }
                    }
                }
                int isCorner = 0;
                iOut = new Image<Gray, byte>(srcROI.Size);
                for (int i = 0; i < approx.CountLines; i++)
                {
                    if (cornerLines[i].Alpha != 0 && cornerLines[i].Length != 0)
                    {
                        double pointOL = pointOnLine(cornerLines[i].Alpha, cornerLines[i].Length, new Point(threshHalfSize + 1, threshHalfSize + 1));
                        double halfLineLength = threshHalfSize / 2;
                        Straight.drawStraight(iOut, cornerLines[i], halfLineLength, pointOL);
                        isCorner++;
                    }
                    else
                    {
                    }
                }
                Point2D intersect;
                
                float sigma = -1;
                if (isCorner > 1)
                {
                    // ##### Find intersections
                    Point2D i1 = Straight.intersect(cornerLines[0], cornerLines[1]);
                    if (approx.CountLines > 2 && isCorner > 2)
                    {
                        Point2D i2 = Straight.intersect(cornerLines[1], cornerLines[2]);
                        Point2D i3 = Straight.intersect(cornerLines[2], cornerLines[0]);
                        intersect = new Point2D((i1.X + i2.X + i3.X) / 3, (i1.Y + i2.Y + i3.Y) / 3);
                        sigma = (float)Math.Sqrt(
                            (Math.Pow((i1.X - intersect.X), 2) + Math.Pow((i1.Y - intersect.Y), 2) +
                            Math.Pow((i2.X - intersect.X), 2) + Math.Pow((i2.Y - intersect.Y), 2) +
                            Math.Pow((i3.X - intersect.X), 2) + Math.Pow((i3.Y - intersect.Y), 2))
                            * 0.5
                            );
                    }
                    else
                    {
                        intersect = i1;
                        sigma = 0;
                    }

                    c2 = new Corner2D(intersect.X + approx.X - threshHalfSize, intersect.Y + approx.Y - threshHalfSize,
                        (float)cornerLines[0].Alpha,
                        (float)cornerLines[1].Alpha, (float)cornerLines[2].Alpha);
                }
                else
                {
                    c2 = new Corner2D();
                }
                c2.Sigma = sigma;
                cannyCorner[l] = c2;
            }
            float sigma2 = -1;
            for (int i = 0; i < cannyCorner.Length; i++)
            {
                if (cannyCorner[i].Sigma >= 0)
                {
                    if (sigma2 < 0 && i < 1)
                    {
                        sigma2 = cannyCorner[i].Sigma;
                        c2 = cannyCorner[i];
                    }
                    else
                    {
                        if (cannyCorner[i].Sigma <= sigma2 && cannyCorner[i].Sigma > 0)
                        {
                            sigma2 = cannyCorner[i].Sigma;
                            c2 = cannyCorner[i];
                        }
                    }
                }
            }
            c2.Sigma = sigma2;
            return c2;
        }
        /// <summary>
        /// Get shortest distance between a straight and a point
        /// </summary>
        /// <param name="st">Straight given as Features.Straight</param>
        /// <param name="point">Point</param>
        /// <returns>Distance</returns>
        private static double pointFromLine(Straight st, Point point)
        {
            PointF s = new PointF((float)(Math.Cos(st.Alpha) * st.Length), (float)(Math.Sin(st.Alpha) * st.Length));
            double sp = Math.Sqrt(Math.Pow(point.X - s.X, 2) + Math.Pow(point.Y - s.Y, 2));
            double rSP = Math.Atan2(point.X - s.X, point.Y - s.Y);
            double beta = Math.PI / 2 - rSP - (st.Alpha - Math.PI / 2);
            double distance = Math.Sin(beta) * sp;


            return distance;
        }
        /// <summary>
        /// Get shortest distance between a line and a point
        /// </summary>
        /// <param name="lineSegment2D">Line</param>
        /// <param name="point">Point</param>
        /// <returns>Distance</returns>
        private static double pointFromLine(LineSegment2D lineSegment2D, Point point)
        {
            Straight st = new Straight(lineSegment2D);
            PointF s = new PointF((float)(Math.Cos(st.Alpha) * st.Length), (float)(Math.Sin(st.Alpha) * st.Length));
            double sp = Math.Sqrt(Math.Pow(point.X - s.X, 2) + Math.Pow(point.Y - s.Y, 2));
            double rSP = Math.Atan2(point.X - s.X, point.Y - s.Y);
            double beta = Math.PI / 2 - rSP - (st.Alpha - Math.PI / 2);
            double distance = Math.Sin(beta) * sp;


            return distance;
        }
        /// <summary>
        /// Draw contours to grayscale image.
        /// </summary>
        /// <param name="cOut">Image</param>
        /// <param name="contours">Contours</param>
        /// <param name="mCvScalar">Drawing colour</param>
        /// <param name="thickness">Thickness of line</param>
        /// <returns>Image</returns>
        private static Image<Gray, byte> drawContoursAsLines(Image<Gray,byte> cOut, VectorOfVectorOfPoint contours, MCvScalar mCvScalar, int thickness)
        {
            for (int i = 0; i < contours.Size; i++)
                for (int j = 1; j < contours[i].Size; j++)
                    CvInvoke.Line(cOut,
                        new Point(contours[i][j - 1].X, contours[i][j - 1].Y),
                        new Point(contours[i][j].X, contours[i][j].Y),
                        mCvScalar, thickness);

            return cOut;
        }
        /// <summary>
        /// Merge lines, which appear to be the same.
        /// </summary>
        /// <param name="src">Image</param>
        /// <param name="contours">Contours</param>
        /// <param name="linePara">Calculated line parameters</param>
        /// <returns>Merged lines</returns>
        private static VectorOfVectorOfPoint mergeLines(Mat src, VectorOfVectorOfPoint contours, VectorOfFloat[] linePara)
        {
            double tAlpha = Math.PI / 8;
            double tLength = 5;
            double tGap = 100;

            Point[,] mergedContourPoints = new Point[contours.Size, 30000];
            double[,] lineParamater = new double[contours.Size, 10];
            for (int i = 0; i < contours.Size; i++)
            {
                lineParamater[i, 0] = linePara[i][2]; // X0
                lineParamater[i, 1] = linePara[i][3]; // Y0
                lineParamater[i, 2] = linePara[i][0]; // vX
                lineParamater[i, 3] = linePara[i][1]; // vY
                // alpha
                lineParamater[i, 4] = Math.PI / 2 + Math.Atan2(linePara[i][1], linePara[i][0]);
                // length
                lineParamater[i, 5] = Math.Cos(lineParamater[i, 4]) * linePara[i][2] +
                    Math.Sin(lineParamater[i, 4]) * linePara[i][3];

                // startLength
                lineParamater[i, 6] = pointOnLine(lineParamater[i, 4], lineParamater[i, 5], contours[i][0]);
                // endLength
                lineParamater[i, 7] = pointOnLine(lineParamater[i, 4], lineParamater[i, 5], contours[i][contours[i].Size-1]);
                // contour id
                lineParamater[i, 8] = i;
                // line length
                lineParamater[i, 9] = (int)Math.Abs(lineParamater[i, 7] - lineParamater[i, 6]);
            }

            // ##### Group contours by alpha and length
            int[,] kitContours = new int[contours.Size, 1000];
            int[] kitContoursLength = new int[contours.Size];
            for (int i = 0; i < contours.Size; i++)
            {
                kitContoursLength[i] = 0;
                kitContours[i, kitContoursLength[i]++] = i;
                for (int j = 0; j < contours.Size; j++)
                    if (i != j)
                    {
                        if (Math.Abs(lineParamater[i, 4] - lineParamater[j, 4]) <= tAlpha &&
                            Math.Abs(lineParamater[i, 5] - lineParamater[j, 5]) <= tLength
                            )
                        {
                            kitContours[i, kitContoursLength[i]++] = j;
                        }
                    }
            }

            // ##### Ungroup contour, which gap is bigger then tGap
            for (int i = 0; i < contours.Size; i++)
            {
                // sort lines by start / endpoint
                for (int j = 0; j < kitContoursLength[i]-1; j++)
                {
                    for (int k = kitContoursLength[i] - 1; k >= j; k--) 
                    {
                        if (lineParamater[kitContours[i, k + 1], 6] < lineParamater[k, 6])
                        {
                            int switchPos= (int)kitContours[i, k];
                            kitContours[i, k] = kitContours[i, k + 1];
                            kitContours[i, k + 1] = switchPos;
                        }
                    }
                }

                // find main line position in line stack
                int mainPosition = 0;
                for (int j = 0; j < kitContoursLength[i]; j++)
                {
                    if(kitContours[i, j] == i)
                    {
                        mainPosition = j;
                    }
                }

                // split line from main line, if gap > tGap
                for (int j = mainPosition + 1; j < kitContoursLength[i]; j++) 
                {
                    if (Math.Abs(lineParamater[kitContours[i, j - 1], 7] - lineParamater[kitContours[i, j], 6]) > tGap)
                    {
                        for (int k = j; k < kitContoursLength[i]; k++)
                        {
                            kitContours[i, k] = -1;
                        }
                        j = kitContoursLength[i];
                    }
                }
                for (int j = mainPosition - 1; j >= 0; j--)
                {
                    if (Math.Abs(lineParamater[kitContours[i, j + 1], 6] - lineParamater[kitContours[i, j], 7]) > tGap)
                    {
                        for (int k = j; k >= 0; k--)
                        {
                            kitContours[i, k] = -1;
                        }
                        j = -1;
                    }
                }

            }
            // if a contour appears in more than one line segment, choose only the longest line segment to become a contour

            // merge line segments to contour
            int count = 0;
            for (int i = 0; i < contours.Size; i++)
            {
                if (kitContoursLength[i] > 0)
                {
                    int count2 = 0;
                    int kitContoursLength2 = kitContoursLength[i];
                    for (int j = 0; j < kitContoursLength[i]; j++)
                    {
                        if (kitContours[i, j] > -1)
                        {
                            for (int k = 0; k < contours[kitContours[i, j]].Size; k++)
                            {
                                mergedContourPoints[count, count2++] = contours[kitContours[i, j]][k];
                            }
                        }
                    }
                    kitContoursLength[count] = count2;
                    count++;
                }
            }
            Point[][] contourPoints = new Point[count][];
            for (int i = 0; i < count; i++)
            {
                contourPoints[i] = new Point[kitContoursLength[i]];
                for (int j = 0; j < kitContoursLength[i]; j++)
                {
                    contourPoints[i][j] = mergedContourPoints[i, j];
                }
            }

            contours = new VectorOfVectorOfPoint(contourPoints);
            //Image<Gray, byte> iOut = new Image<Gray, byte>(src.Width, src.Height);
            //iOut = drawContoursAsLines(iOut, contours, new MCvScalar(255), 1);
            //ImageViewer.Show(iOut);

            return contours;
        }
        /// <summary>
        /// Get points position on Line.
        /// </summary>
        /// <param name="line">Line</param>
        /// <param name="p">Point</param>
        /// <returns>Distance from lines origin</returns>
        public static double pointOnLine(LineSegment2D line, Point p)
        {
            double alpha = 0;
            double length = 0;

            alpha = Math.PI / 2 + Math.Atan2(line.Direction.Y * -1, line.Direction.X * -1);
            length = Math.Cos(alpha) * line.P1.X + Math.Sin(alpha) * line.P1.Y;
            double distance = pointOnLine(alpha, length, p);

            return distance;
        }
        /// <summary>
        /// Get points position on Line.
        /// </summary>
        /// <param name="alpha">Line parameter alpha</param>
        /// <param name="length">Line parameter length</param>
        /// <param name="p">Point</param>
        /// <returns>Distance from lines origin</returns>
        public static double pointOnLine(double alpha, double length, Point p)
        {
            PointF s = new PointF((float)(Math.Cos(alpha) * length), (float)(Math.Sin(alpha) * length));
            double sp = Math.Sqrt(Math.Pow(p.X - s.X, 2) + Math.Pow(p.Y - s.Y, 2));
            double rSP = Math.Atan2(p.X - s.X, p.Y - s.Y);
            double beta = Math.PI / 2 - rSP - (alpha - Math.PI / 2);
            double distance = Math.Cos(beta) * sp;

            return distance;
        }

        /// <summary>
        /// Split contours to lines.
        /// </summary>
        /// <param name="src">Image</param>
        /// <param name="contours">Contours</param>
        /// <returns>Splitted contours</returns>
        private static VectorOfVectorOfPoint splitContour2Lines(Mat src, VectorOfVectorOfPoint contours)
        {
            // ##### Find line direction between every 3rd pixel and check differences
            Image<Rgb, byte> cOut = new Image<Rgb, byte>(src.Width, src.Height);
            CvInvoke.DrawContours(cOut, contours, -1, new MCvScalar(255, 0, 0), 1);
            bool isSplit = false;
            int count = 0;
            int countNextContour = 0;
            double direction = 0;
            double directionLast = 0;
            int gap = 4;
            double angleIncrement = Math.PI / 8;
            Point[] point = new Point[30000];
            Point[] point2;
            Point[][] nextContour = new Point[1000000][];

            for (int i = 0; i < contours.Size; i++)
            {
                isSplit = false;
                countNextContour = 0;
                VectorOfPoint singleContour = contours[i];
                Point[] pp = singleContour.ToArray();
                Point[] pDistinct = pp.Distinct().ToArray();
                
                for (int j = 0; j < pDistinct.Length - gap; j += gap)
                {
                    double yDif = pDistinct[j + gap].X - pDistinct[j].X;
                    double xDif = pDistinct[j + gap].Y - pDistinct[j].Y;
                    direction = Math.Atan2(yDif, xDif);
                    if (direction < 0)
                        direction += Math.PI * 2;
                    if (j > 0 && Math.Abs(direction - directionLast) > angleIncrement)
                    {
                        if (isSplit == false)
                        {
                            point2 = new Point[countNextContour];
                            for (int k = 0; k < countNextContour; k++)
                                point2[k] = point[k];
                            nextContour[count] = point2;
                            count++;
                            countNextContour = 0;
                        }
                        isSplit = true;
                    }
                    else
                    {
                        isSplit = false;
                        for (int k = j; k < j + gap; k++)
                        {
                            point[countNextContour++] = pDistinct[k];
                        }
                        if (!(j + gap < pDistinct.Length - gap))
                        {
                            point2 = new Point[countNextContour];
                            for (int k = 0; k < countNextContour; k++)
                                point2[k] = point[k];
                            nextContour[count] = point2;
                            count++;
                            countNextContour = 0;
                        }
                    }
                    directionLast = direction;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Point[][] nextContour2 = new Point[count][];
            for (int i = 0; i < count; i++)
                nextContour2[i] = nextContour[i];

            contours = new VectorOfVectorOfPoint(nextContour2);
            CvInvoke.DrawContours(cOut, contours, -1, new MCvScalar(0, 255, 0), 1);
            //ImageViewer.Show(cOut);

            return contours;
        }

        /// <summary>
        /// Find template matching in an Mat image.
        /// </summary>
        /// <param name="refImage">Image to search in</param>
        /// <param name="template">Template image</param>
        /// <param name="approx">Approximated matching point</param>
        /// <returns></returns>
        public static SubPixelTemplateMatching findSubPixelTemplateMatching(Mat refImage, Mat template, Point2D approx)
        {
            SubPixelTemplateMatching sp = new SubPixelTemplateMatching();
            Image<Gray, float> reference = refImage.ToImage<Gray, float>();

            int halfZoomImageSize = 40;

            // Approximate template position for initial value
            Image<Gray, float> usedTemplate = template.ToImage<Gray, float>();
            Image<Gray, float> resampledImage = template.ToImage<Gray, float>();

            int w = reference.Width - usedTemplate.Width + 1;
            int h = reference.Height - usedTemplate.Height + 1;

            Image<Gray, float> corrImage = new Image<Gray, float>(w, h);
            CvInvoke.MatchTemplate(reference, usedTemplate, corrImage, TemplateMatchingType.SqdiffNormed);
            int shift = usedTemplate.Width / 2;

            double minVal = 0;
            double maxVal = 0;
            Point minLoc = new Point();
            Point maxLoc = new Point();

            Image<Gray, byte> mask = new Image<Gray, byte>(corrImage.Width, corrImage.Height);
            mask.SetZero();
            mask.Draw(new Rectangle(approx.XInt - 20 - shift, approx.YInt - 20 - shift, 40, 40), new Gray(255), -1);

            CvInvoke.MinMaxLoc(corrImage, ref minVal, ref maxVal, ref minLoc, ref maxLoc, mask);

            // Compensate with Least-square matching
            int nbrObs = usedTemplate.Width * usedTemplate.Height;
            float x0 = (float)minLoc.X + halfZoomImageSize;
            float y0 = (float)minLoc.Y + halfZoomImageSize;
            float r0 = 0;

            Image<Gray, float> m_grad_x = new Mat(usedTemplate.Height, usedTemplate.Width, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_grad_y = new Mat(usedTemplate.Height, usedTemplate.Width, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_A = new Mat(nbrObs, 3, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_AT = new Mat(3, nbrObs, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_ATA = new Mat(3, 3, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_Qxx = new Mat(3, 3, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_ATL = new Mat(3, 1, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_dX = new Mat(3, 1, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_L = new Mat(nbrObs, 1, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_AdX = new Mat(nbrObs, 1, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_V = new Mat(nbrObs, 1, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_VT = new Mat(1, nbrObs, DepthType.Cv32F, 1).ToImage<Gray, float>();
            Image<Gray, float> m_VTV = new Mat(1, 1, DepthType.Cv32F, 1).ToImage<Gray, float>();

            bool cont = true;
            bool ok;

            int kk = 0;

            double tol_corr_matching = 0.3;

            if (minVal > tol_corr_matching)
            {
                cont = false;
            }

            while (cont)
            {
                kk += 1;
                PointF center = new PointF(x0, y0);
                CvInvoke.GetRectSubPix(reference, resampledImage.Size, center, resampledImage);
                resampledImage.Add(new Gray(r0));
                
                gradient(resampledImage, ref m_grad_x, ref m_grad_y);

                // A
                int k = 0;
                for (int i = 0; i < resampledImage.Height; i++)
                {
                    for (int j = 0; j < resampledImage.Width; j++)
                    {
                        m_A.Data[k, 0, 0] = m_grad_x.Data[i, j, 0];
                        m_A.Data[k, 1, 0] = m_grad_y.Data[i, j, 0];
                        m_A.Data[k, 2, 0] = 1;
                        k += 1;
                    }
                }

                // L
                k = 0;
                for (int i = 0; i < resampledImage.Height; i++)
                {
                    for (int j = 0; j < resampledImage.Width; j++)
                    {
                        m_L.Data[k, 0, 0] = usedTemplate.Data[i, j, 0]- resampledImage.Data[i, j, 0];
                        k += 1;
                    }
                }

                CvInvoke.Transpose(m_A, m_AT);
                CvInvoke.Gemm(m_AT, m_A, 1, null, 1, m_ATA);
                CvInvoke.Invert(m_ATA, m_Qxx, DecompMethod.LU);
                CvInvoke.Gemm(m_AT, m_L, 1, null, 1, m_ATL);
                CvInvoke.Gemm(m_Qxx, m_ATL, 1, null, 1, m_dX);

                x0 += m_dX.Data[0, 0, 0];
                y0 += m_dX.Data[1, 0, 0];
                r0 += m_dX.Data[2, 0, 0];

                if (kk < 300 && (Math.Abs(m_dX.Data[0, 0, 0]) > 0.01 || Math.Abs(m_dX.Data[1, 0, 0]) > 0.01))
                {
                    cont = true;
                }
                else
                {
                    cont = false;
                }
            }

            if (Math.Abs(m_dX.Data[0, 0, 0]) > 0.01 || Math.Abs(m_dX.Data[1, 0, 0]) > 0.01)
                ok = false;
            else
                ok = true;


            if (ok)
            {
                // Standard deviation
                CvInvoke.Gemm(m_A, m_dX, 1, null, 1, m_AdX);
                CvInvoke.cvConvertScale(m_L, m_V, -1.0, 0);
                CvInvoke.Add(m_V, m_AdX, m_V);
                CvInvoke.Transpose(m_V, m_VT);
                CvInvoke.Gemm(m_VT, m_V, 1, null, 1, m_VTV);


                double s0 = Math.Sqrt(m_VTV.Data[0, 0, 0] / (nbrObs - 3));
                double s_x = s0 * Math.Sqrt(m_Qxx.Data[0, 0, 0]);
                double s_y = s0 * Math.Sqrt(m_Qxx.Data[1, 1, 0]);
                sp.Sigma = (float)Math.Sqrt(s_x * s_x + s_y * s_y);

                if (sp.Sigma > 0.1)
                {
                    ok = false;
                }
            }


            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (minVal > tol_corr_matching)
            {
                ok = false;
            }
            if(ok)
            {
                sp.X = x0;
                sp.Y = y0;

            }
            else
            {

            }

            return sp;
        }
        /// <summary>
        /// Calculates gradients in x and y directions
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="m_grad_x">X gradient array</param>
        /// <param name="m_grad_y">Y gradient array</param>
        public static void gradient(Image<Gray, float> img, ref Image<Gray, float> m_grad_x, ref Image<Gray, float> m_grad_y)
        {
            for (int i = 1; i < img.Height - 1; i++)
            {
                for (int j = 1; j < img.Width - 1; j++)
                {
                    m_grad_x.Data[i, j, 0] = (img.Data[i, j + 1, 0] - img.Data[i, j - 1, 0]) / 3.0F;
                    m_grad_y.Data[i, j, 0] = (img.Data[i + 1, j, 0] - img.Data[i - 1, j, 0]) / 3.0F;
                }
            }

            for (int i = 1; i < img.Width - 1; i++)
            {
                m_grad_x.Data[0, i, 0] = m_grad_x.Data[1, i, 0];
                m_grad_y.Data[0, i, 0] = m_grad_y.Data[1, i, 0];
            }
            for (int i = 1; i < img.Width - 1; i++)
            {
                m_grad_x.Data[img.Height - 1, i, 0] = m_grad_x.Data[img.Height - 2, i, 0];
                m_grad_y.Data[img.Height - 1, i, 0] = m_grad_y.Data[img.Height - 2, i, 0];
            }
            for (int i = 1; i < img.Height - 1; i++)
            {
                m_grad_x.Data[i, 0, 0] = m_grad_x.Data[i, 1, 0];
                m_grad_y.Data[i, 0, 0] = m_grad_y.Data[i, 1, 0];
            }
            for (int i = 1; i < img.Height - 1; i++)
            {
                m_grad_x.Data[i, img.Width - 1, 0] = m_grad_x.Data[i, img.Width - 2, 0];
                m_grad_y.Data[i, img.Width - 1, 0] = m_grad_y.Data[i, img.Width - 2, 0];
            }

            m_grad_x.Data[0, 0, 0] = m_grad_x.Data[1, 1, 0];
            m_grad_y.Data[0, 0, 0] = m_grad_y.Data[1, 1, 0];

            m_grad_x.Data[0, img.Width - 1, 0] = m_grad_x.Data[1, img.Width - 2, 0];
            m_grad_y.Data[0, img.Width - 1, 0] = m_grad_y.Data[1, img.Width - 2, 0];

            m_grad_x.Data[img.Height - 1, img.Width - 1, 0] = m_grad_x.Data[img.Height - 2, img.Width - 2, 0];
            m_grad_y.Data[img.Height - 1, img.Width - 1, 0] = m_grad_y.Data[img.Height - 2, img.Width - 2, 0];

            m_grad_x.Data[img.Height - 1, 0, 0] = m_grad_x.Data[img.Height - 2, 1, 0];
            m_grad_y.Data[img.Height - 1, 0, 0] = m_grad_y.Data[img.Height - 2, 1, 0];

        }

        /// <summary>
        /// Draws a crosshair into image. The size in 10 times higher than the static LineWeight. Colour == static LineColor
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="p">Point</param>
        private static void drawCrossHair(Mat img, Point2D p)
        {
            int length = LineWeight * 5;
            if (p.PointInt.X - length >= 0 && p.PointInt.X + length <= img.Width &&
                p.PointInt.Y - length >= 0 && p.PointInt.Y + length <= img.Height)
            {
                CvInvoke.Line(img,
                    new System.Drawing.Point(p.PointInt.X - length, p.PointInt.Y),
                    new System.Drawing.Point(p.PointInt.X + length, p.PointInt.Y),
                    LineColor, LineWeight);
                CvInvoke.Line(img,
                    new System.Drawing.Point(p.PointInt.X, p.PointInt.Y - length),
                    new System.Drawing.Point(p.PointInt.X, p.PointInt.Y + length),
                    LineColor, LineWeight);
            }
        }

        /// <summary>
        /// Draws sphere to image. Colour == static LineColor. Line weight == static LineWeight
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="s">Sphere</param>
        public static void drawSphere2D(Mat img, Sphere2D s)
        {
            LineWeight = (int)Math.Round(s.Diameter / 30, 0, MidpointRounding.AwayFromZero);
            if (LineWeight < 1)
                LineWeight = 1;
            CvInvoke.Circle(img, s.PointInt, (int)s.Diameter / 2, LineColor, LineWeight);
            drawCrossHair(img, s);
        }

        /// <summary>
        /// Draws spheres to image. Colour == static LineColor. Line weight == static LineWeight.
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="s">Sphere</param>
        public static void drawSphere2D(Mat img, Sphere2D[] s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                drawSphere2D(img, s[i]);
            }
        }

        /// <summary>
        /// Draws ellipse to image. Colour == static LineColor. Line weight == static LineWeight.
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="e">Ellipse</param>
        public static void drawEllipse2D(Mat img, Ellipse2D e)
        {
            if (e.X - e.A >= 0 && e.X + e.A <= img.Width &&
                e.Y - e.A >= 0 && e.Y + e.A <= img.Height)
            {
                CvInvoke.Ellipse(img, new RotatedRect(e.PointFloat, new SizeF(e.Size.Height, e.Size.Width), (float)(-e.Angle / Math.PI * 180)), LineColor, LineWeight);
            }
            drawCrossHair(img, e);
        }

        /// <summary>
        /// Draws ellipses to image. Colour == static LineColor. Line weight == static LineWeight.
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="e">Ellipse</param>
        public static void drawEllipse2D(Mat img, Ellipse2D[] e)
        {
            for (int i = 0; i < e.Length; i++)
            {
                drawEllipse2D(img, e[i]);
            }
        }

        /// <summary>
        /// Draws corner as crosshair to imgage.
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="c">Corner</param>
        public static void drawCorner2D(Mat img, Corner2D c)
        {
            drawCrossHair(img, c);
        }

        /// <summary>
        /// Draws corners as crosshair to imgage.
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="c">Corner</param>
        public static void drawCorner2D(Mat img, Corner2D[] c)
        {
            for (int i = 0; i < c.Length; i++)
            {
                drawCorner2D(img, c[i]);
            }
        }

        /// <summary>
        /// Draws SubPixelTemplateMatching as crosshair to imgage.
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="sp">SubPixelTemplateMatching</param>
        public static void drawSubPixelTemplateMatching(Mat img, SubPixelTemplateMatching sp)
        {
            drawCrossHair(img, sp);
        }

        /// <summary>
        /// Draws SubPixelTemplateMatchings as crosshair to imgage.
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="sp">SubPixelTemplateMatchings</param>
        public static void drawSubPixelTemplateMatching(Mat img, SubPixelTemplateMatching[] sp)
        {
            for (int i = 0; i < sp.Length; i++)
            {
                drawSubPixelTemplateMatching(img, sp[i]);
            }
        }

        /// <summary>
        /// Draws Point2D as crosshair to image.
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="p">Point</param>
        public static void drawAll2D(Mat img, Point2D p)
        {
            drawCrossHair(img, p);
        }

        /// <summary>
        /// Draws Point2D points as crosshair to image.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="p"></param>
        public static void drawAll2D(Mat img, Point2D[] p)
        {
            for (int i = 0; i < p.Length; i++)
            {
                drawAll2D(img, p[i]);
            }
        }

        /// <summary>
        /// Returns the median value of the grayscaled Mat image.
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>Median</returns>
        public static byte median8Bit(Mat image)
        {
            Image<Gray, Byte> img = toGray(image).ToImage<Gray, Byte>();
            Mat hist = new Mat();

            byte median = 0;
            byte[] pixels = new byte[img.Cols * img.Rows];
            long p=0;
            for (int c = 0; c < img.Cols; c++)
                for (int r = 0; r < img.Rows; r++)
                    pixels[p++] = img.Data[r, c, 0];
            Array.Sort(pixels);
            median = pixels[(long)(p/2)];
            return median;
        }
        /// <summary>
        /// Find the average value between maximum and minimum.
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>Average</returns>
        public static byte minMaxAverage8Bit(Mat image)
        {
            Image<Gray, Byte> img = toGray(image).ToImage<Gray, Byte>();
            ulong min = ulong.MaxValue;
            ulong max = ulong.MinValue;
            byte average = 0;
            byte px;
            for (int c = 0; c < img.Cols; c++)
                for (int r = 0; r < img.Rows; r++)
                {
                    px = img.Data[r, c, 0];
                    if (px < min)
                        min = px;
                    if (px > max)
                        max = px;
                }
            average = (byte)(min + ((max - min) / 2));
            return average;
        }
        /// <summary>
        /// Transform image to grayscale image.
        /// </summary>
        /// <param name="image">Color image</param>
        /// <returns>Grayscale image</returns>
        public static Mat toGray(Mat image)
        {
            Mat img = image.Clone();
            if(img.Depth != DepthType.Cv8U || img.NumberOfChannels != 1)
                CvInvoke.CvtColor(img, img, ColorConversion.Bgr2Gray,1);
            return img;
        }
        /// <summary>
        /// Get distance between two points.
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <returns>Distance</returns>
        public static double distBetweenPoints(Point2D p1, Point2D p2)
        {
            double dist = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            return dist;
        }
    }
    /// <summary>
    /// Point class with additional information about accuracy.
    /// </summary>
    public class Point2D
    {
        // ##### Properties #####
        protected FeatureType _fType = FeatureType.POINT_2D;
        /// <summary>
        /// Type of feature: Point, Sphere, Corner ... .
        /// </summary>
        [XmlAttribute("FeatureType")]
        public FeatureType FType
        {
            get { return _fType; }
        }
        
        private float _x;
        private float _y;

        [XmlAttribute("X")]
        public float X
        {
            get { return _x; }
            set { _x = value; }
        }
        [XmlAttribute("Y")]
        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }
        [XmlIgnore]
        public int XInt
        {
            get { return (int)Math.Round(X); }
            set { X = value; }
        }
        [XmlIgnore]
        public int YInt
        {
            get { return (int)Math.Round(Y); }
            set { Y = value; }
        }
        [XmlIgnore]
        public PointF PointFloat
        {
            get
            {
                return new PointF(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        [XmlIgnore]
        public Point PointInt
        {
            get
            {
                return new Point(XInt, YInt);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        private float _sigma;
        /// <summary>
        /// -1: unknown accuracy.
        /// </summary>
        [XmlAttribute("Sigma")]
        public float Sigma
        {
            get
            {
                return _sigma;
            }
            set
            {
                if (value < 0)
                    _sigma = -1;
                else
                    _sigma = value;
            }
        }
        // ##### Constructors #####
        /// <summary>
        /// Create Point2D. X = Y = 0, Sigma = -1.
        /// </summary>
        public Point2D()
        {
            X = 0;
            Y = 0;
            Sigma = -1;
        }
        /// <summary>
        /// Create Point2D
        /// </summary>
        /// <param name="pointF">Point</param>
        /// <param name="sigma">Sigma</param>
        public Point2D(PointF pointF, float sigma = -1)
        {
            X = pointF.X;
            Y = pointF.Y;
            Sigma = sigma;
        }
        /// <summary>
        /// Create Point2D
        /// </summary>
        /// <param name="point">Point</param>
        /// <param name="sigma">Sigma</param>
        public Point2D(Point point, float sigma = -1)
        {
            X = point.X;
            Y = point.Y;
            Sigma = sigma;
        }
        /// <summary>
        /// Create Point2D
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="sigma">Sigma</param>
        public Point2D(int x, int y, float sigma = -1)
        {
            X = x;
            Y = y;
            Sigma = sigma;
        }
        /// <summary>
        /// Create Point2D
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="sigma">Sigma</param>
        public Point2D(float x, float y, float sigma = -1)
        {
            X = x;
            Y = y;
            Sigma = sigma;
        }
        /// <summary>
        /// Get points information.
        /// </summary>
        /// <returns>'x, y, sigma'</returns>
        public override string ToString()
        {
            return X + ", " + Y + ", " + Sigma;
        }
    }
    /// <summary>
    /// 3D Point class with additional information about accuracy.
    /// </summary>
    public class Point3D : Point2D
    {
        // ##### Properties #####
        private float _z;
        [XmlAttribute("Z")]
        public float Z
        {
            get { return _z; }
            set { _z = value; }
        }
        [XmlIgnore]
        public int ZInt
        {
            get { return (int)Math.Round(Z); }
            set { Z = value; }
        }

        // ##### Constructors #####
        /// <summary>
        /// Create Point3D. X = Y = Z = 0; Sigma = -1
        /// </summary>
        public Point3D()
        {
            X = 0;
            Y = 0;
            Z = 0;
            Sigma = -1;
            _fType = FeatureType.POINT_3D;
        }
        /// <summary>
        /// Create Point3D
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        /// <param name="sigma">Sigma</param>
        public Point3D(int x, int y, int z, float sigma = -1)
        {
            X = x;
            Y = y;
            Z = z;
            Sigma = sigma;
            _fType = FeatureType.POINT_3D;
        }
        /// <summary>
        /// Create Point3D
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        /// <param name="sigma">Sigma</param>
        public Point3D(float x, float y, float z, float sigma = -1)
        {
            X = x;
            Y = y;
            Z = z;
            Sigma = sigma;
            _fType = FeatureType.POINT_3D;
        }
        /// <summary>
        /// Get points information.
        /// </summary>
        /// <returns>'x, y, z, sigma'</returns>
        public override string ToString()
        {
            return X + ", " + Y + ", " + Z + ", " + Sigma;
        }
    }
    /// <summary>
    /// Sphere class with additional information about accuracy.
    /// </summary>
    public class Sphere2D : Point2D
    {
        // ##### Properties #####
        private float _diameter;
        [XmlAttribute("Diameter")]
        public float Diameter
        {
            get { return _diameter; }
            set { _diameter = value; }
        }
        private float _sigmaX;
        [XmlAttribute("SigmaX")]
        public float SigmaX
        {
            get { return _sigmaX; }
            set
            {
                if (value < 0)
                    _sigmaX = -1;
                else
                    _sigmaX = value;
            }
        }
        private float _sigmaY;
        [XmlAttribute("SigmaY")]
        public float SigmaY
        {
            get { return _sigmaY; }
            set
            {
                if (value < 0)
                    _sigmaY = -1;
                else
                    _sigmaY = value;
            }
        }

        // ##### Constructor #####
        /// <summary>
        /// Create Sphere2D. X = Y = 0; Sigmas = -1.
        /// </summary>
        public Sphere2D()
        {
            X = 0;
            Y = 0;
            Sigma = -1;
            SigmaX = -1;
            SigmaY = -1;
            Diameter = -1;
            _fType = FeatureType.SPHERE_2D;
        }
        /// <summary>
        /// Create Sphere2D.
        /// </summary>
        /// <param name="pointF">Center</param>
        /// <param name="diameter">Diameter</param>
        /// <param name="sigma">Sigma</param>
        /// <param name="sigmaX">Sigma X</param>
        /// <param name="sigmaY">Sigma Y</param>
        public Sphere2D(PointF pointF, float diameter = -1, float sigma = -1, float sigmaX = -1, float sigmaY = -1)
        {
            X = pointF.X;
            Y = pointF.Y;
            Sigma = sigma;
            SigmaX = sigmaX;
            SigmaY = sigmaY;
            Diameter = diameter;
            _fType = FeatureType.SPHERE_2D;
        }
        /// <summary>
        /// Create Sphere2D.
        /// </summary>
        /// <param name="point">Center</param>
        /// <param name="diameter">Diameter</param>
        /// <param name="sigma">Sigma</param>
        /// <param name="sigmaX">Sigma X</param>
        /// <param name="sigmaY">Sigma Y</param>
        public Sphere2D(Point point, float diameter = -1, float sigma = -1, float sigmaX = -1, float sigmaY = -1)
        {
            X = point.X;
            Y = point.Y;
            Sigma = sigma;
            SigmaX = sigmaX;
            SigmaY = sigmaY;
            Diameter = diameter;
            _fType = FeatureType.SPHERE_2D;
        }
        /// <summary>
        /// Create Sphere2D.
        /// </summary>
        /// <param name="x">Center X</param>
        /// <param name="y">Center Y</param>
        /// <param name="diameter">Diameter</param>
        /// <param name="sigma">Sigma</param>
        /// <param name="sigmaX">Sigma X</param>
        /// <param name="sigmaY">Sigma Y</param>
        public Sphere2D(int x, int y, float diameter = -1, float sigma = -1, float sigmaX = -1, float sigmaY = -1)
        {
            X = x;
            Y = y;
            Sigma = sigma;
            SigmaX = sigmaX;
            SigmaY = sigmaY;
            Diameter = diameter;
            _fType = FeatureType.SPHERE_2D;
        }
        /// <summary>
        /// Create Sphere2D.
        /// </summary>
        /// <param name="x">Center X</param>
        /// <param name="y">Center Y</param>
        /// <param name="diameter">Diameter</param>
        /// <param name="sigma">Sigma</param>
        /// <param name="sigmaX">Sigma X</param>
        /// <param name="sigmaY">Sigma Y</param>
        public Sphere2D(float x, float y, float diameter = -1, float sigma = -1, float sigmaX = -1, float sigmaY = -1)
        {
            X = x;
            Y = y;
            Diameter = diameter;
            Sigma = sigma;
            SigmaX = sigmaX;
            SigmaY = sigmaY;
            _fType = FeatureType.SPHERE_2D;
        }
    }
    /// <summary>
    /// Ellipse class with additional information about accuracy.
    /// PointFloat equals the center of ellipse.
    /// </summary>
    public class Ellipse2D : Point2D
    {
        // ##### Properties #####
        private float _b;
        private float _a;
        [XmlAttribute("B")]
        public float B
        {
            get { return _b; }
            set { _b = value; }
        }
        [XmlAttribute("A")]
        public float A
        {
            get { return _a; }
            set { _a = value; }
        }
        [XmlIgnore]
        public SizeF Size
        {
            get
            {
                return new SizeF(B * 2, A * 2);
            }
            set
            {
                B = value.Width / 2;
                A = value.Height / 2;
            }
        }

        [XmlAttribute("Angle")]
        public float Angle { get; set; }

        // ##### Constructors #####

        /// <summary>
        /// Create Ellipse2D. X = Y = A = B = Angle = 0; Sigma = -1
        /// </summary>
        public Ellipse2D()
        {
            X = 0;
            Y = 0;
            Sigma = -1;
            B = 0;
            A = 0;
            Angle = 0;
            _fType = FeatureType.ELLIPSE_2D;
        }
        /// <summary>
        /// Create Ellipse2D.
        /// </summary>
        /// <param name="pointF">Center</param>
        /// <param name="size">Size as width (2 * B) and height (2 * A)</param>
        /// <param name="angle">Angle</param>
        /// <param name="sigma">Sigma</param>
        public Ellipse2D(PointF pointF, SizeF size = new SizeF(), float angle = 0, float sigma = -1)
        {
            X = pointF.X;
            Y = pointF.Y;
            Sigma = sigma;
            Size = size;
            Angle = angle;
            _fType = FeatureType.ELLIPSE_2D;
        }
        /// <summary>
        /// Create Ellipse2D.
        /// </summary>
        /// <param name="pointF">Center</param>
        /// <param name="b">B</param>
        /// <param name="a">A</param>
        /// <param name="angle">Angle</param>
        /// <param name="sigma">Sigma</param>
        public Ellipse2D(PointF pointF, float b = 0, float a = 0, float angle = 0, float sigma = -1)
        {
            X = pointF.X;
            Y = pointF.Y;
            Sigma = sigma;
            B = b;
            A = a;
            Angle = angle;
            _fType = FeatureType.ELLIPSE_2D;
        }
        /// <summary>
        /// Create Ellipse2D. Sigma = -1; A = B = Angle = 0.
        /// </summary>
        /// <param name="point">Center</param>
        public Ellipse2D(Point point)
        {
            X = point.X;
            Y = point.Y;
            Sigma = -1;
            B = 0;
            A = 0;
            Angle = 0;
            _fType = FeatureType.ELLIPSE_2D;
        }
        /// <summary>
        /// Create Ellipse2D. Sigma = -1; A = B = Angle = 0.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public Ellipse2D(int x, int y)
        {
            X = x;
            Y = y;
            Sigma = -1;
            B = 0;
            A = 0;
            Angle = 0;
            _fType = FeatureType.ELLIPSE_2D;
        }
        /// <summary>
        /// Create Ellipse2D.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="b">B</param>
        /// <param name="a">A</param>
        /// <param name="angle">Angle</param>
        /// <param name="sigma">Sigma</param>
        public Ellipse2D(float x, float y, float b = 0, float a = 0, float angle = 0, float sigma = -1)
        {
            X = x;
            Y = y;
            Sigma = sigma;
            B = b;
            A = a;
            Angle = angle;
            _fType = FeatureType.ELLIPSE_2D;
        }
    }
    /// <summary>
    /// Corner class with additional information about accuracy.
    /// </summary>
    public class Corner2D : Point2D
    {
        // ##### Properties #####
        private int _countLines = 3;

        [XmlAttribute("CountLines")]
        public int CountLines
        {
            get { return _countLines; }
            set { _countLines = value; }
        }
        private float _line1;

        [XmlAttribute("Line1")]
        public float Line1
        {
            get { return _line1; }
            set { _line1 = value; }
        }
        private float _line2;

        [XmlAttribute("Line2")]
        public float Line2
        {
            get { return _line2; }
            set { _line2 = value; }
        }
        private float _line3;

        [XmlAttribute("Line3")]
        public float Line3
        {
            get { return _line3; }
            set { _line3 = value; }
        }

        // ##### Constructors #####
        public Corner2D()
        {
            X = 0;
            Y = 0;
            Sigma = -1;
            _fType = FeatureType.CORNER_2D;
        }
        /// <summary>
        /// Create Corner with additional information about outgoing lines.
        /// <param name="directionLine1">Used to indicate direction of line one.</param>
        /// <param name="directionLine2">Used to indicate direction of line two.</param>
        /// <param name="directionLine3">Used to indicate direction of line three. If the Corner only have two lines, type -1000.</param>
        /// <param name="gon">True if directions given in gon. False if directions given in radiant.</param>
        /// </summary>
        public Corner2D(float x, float y, float directionLine1, float directionLine2, float directionLine3 = -1000, bool gon = false)
        {
            X = x;
            Y = y;
            Line1 = directionLine1;
            Line2 = directionLine2;
            Line3 = directionLine3;
            if (Line3 == -1000)
                CountLines = 2;
            if(gon == true)
            {
                Line1 = Line1 / 200 * (float)Math.PI;
                Line2 = Line2 / 200 * (float)Math.PI;
                if (directionLine3 != -1000)
                    Line3 = Line3 / 200 * (float)Math.PI;
            }
            Sigma = -1;
            _fType = FeatureType.CORNER_2D;
        }
    }
    /// <summary>
    /// Point saving SubPixelTemplateMatching class with additional information about accuracy.
    /// PointFloat equals the center of best matching.
    /// </summary>
    public class SubPixelTemplateMatching : Point2D
    {
        // ##### Constructors #####
        public SubPixelTemplateMatching()
        {
            X = -1;
            Y = -1;
            Sigma = -1;
            _fType = FeatureType.SUBPIXELTEMPLATEMATCHING;
        }
        public SubPixelTemplateMatching(float x, float y, float sigma =-1)
        {
            X = x;
            Y = y;
            Sigma = sigma;
            _fType = FeatureType.SUBPIXELTEMPLATEMATCHING;
        }
    }
    /// <summary>
    /// Represents a straight as normal through origin.
    /// </summary>
    public class Straight
    {
        // ##### Properties #####

        private double _alpha;

        [XmlAttribute("Alpha")]
        public double Alpha
        {
            get { return _alpha; }
            set { _alpha = value; }
        }
        private double _length;

        [XmlAttribute("Length")]
        public double Length
        {
            get { return _length; }
            set { _length = value; }
        }
        private int _countPixels;

        [XmlAttribute("CountPixels")]
        public int CountPixels
        {
            get { return _countPixels; }
            set { _countPixels = value; }
        }

        // ##### Static methods #####

        /// <summary>
        /// Find intersection between two straights.
        /// </summary>
        /// <param name="s1">Straight 1</param>
        /// <param name="s2">Straight 2</param>
        /// <returns>Intersection point</returns>
        public static Point2D intersect(Straight s1, Straight s2)
        {
            Point2D p = new Point2D();
            double y1, y2, x1, x2, vx1, vx2, vy1, vy2, r2;
            x1 = Math.Cos(s1.Alpha) * s1.Length;
            x2 = Math.Cos(s2.Alpha) * s2.Length;
            y1 = Math.Sin(s1.Alpha) * s1.Length;
            y2 = Math.Sin(s2.Alpha) * s2.Length;
            vx1 = Math.Cos(s1.Alpha - Math.PI / 2);
            vx2 = Math.Cos(s2.Alpha - Math.PI / 2);
            vy1 = Math.Sin(s1.Alpha - Math.PI / 2);
            vy2 = Math.Sin(s2.Alpha - Math.PI / 2);

            r2 = (y1 + (x2 * vy1 - x1 * vy1) / vx1 - y2) / (vy2 - vx2 * vy1 / vx1);

            p.X = (float)(x2 + r2 * vx2);
            p.Y = (float)(y2 + r2 * vy2);

            return p;
        }
        /// <summary>
        /// Drawing a straight.
        /// </summary>
        /// <param name="dst">Image</param>
        /// <param name="straight">Straight</param>
        /// <param name="halfDistance">Half length of line to draw</param>
        /// <returns>Image</returns>
        public static Image<Gray, byte> drawStraight(Image<Gray, byte> dst, Straight straight, double halfDistance)
        {
            double pointOnLine = FeatureDetection.pointOnLine(straight.Alpha, straight.Length,
                new Point((int)Math.Floor(dst.Width / 2.0), (int)Math.Floor(dst.Height / 2.0)));
            Point p = new Point();
            Point p1 = new Point();
            Point p2 = new Point();
            double beta = straight.Alpha - Math.PI / 2;
            p.X = (int)(Math.Cos(straight.Alpha) * straight.Length);
            p.Y = (int)(Math.Sin(straight.Alpha) * straight.Length);
            p1.X = (int)(p.X + Math.Cos(beta) * (pointOnLine - halfDistance));
            p1.Y = (int)(p.Y + Math.Sin(beta) * (pointOnLine - halfDistance));
            p2.X = (int)(p.X + Math.Cos(beta) * (pointOnLine + halfDistance));
            p2.Y = (int)(p.Y + Math.Sin(beta) * (pointOnLine + halfDistance));
            CvInvoke.Line(dst, p1, p2, new MCvScalar(255), 1);
            return dst;
        }
        /// <summary>
        /// Drawing a straight.
        /// </summary>
        /// <param name="dst">Image</param>
        /// <param name="straight">Straight</param>
        /// <param name="halfDistance">Half length of line to draw</param>
        /// <param name="pointOnLine">Starting position for drawing on straight</param>
        /// <returns>Image</returns>
        public static Image<Gray, byte> drawStraight(Image<Gray, byte> dst, Straight straight, double halfDistance, double pointOnLine)
        {
            Point p = new Point();
            Point p1 = new Point();
            Point p2 = new Point();
            double beta = straight.Alpha - Math.PI / 2;
            p.X = (int)(Math.Cos(straight.Alpha) * straight.Length);
            p.Y = (int)(Math.Sin(straight.Alpha) * straight.Length);
            p1.X = (int)(p.X + Math.Cos(beta) * (pointOnLine - halfDistance));
            p1.Y = (int)(p.Y + Math.Sin(beta) * (pointOnLine - halfDistance));
            p2.X = (int)(p.X + Math.Cos(beta) * (pointOnLine + halfDistance));
            p2.Y = (int)(p.Y + Math.Sin(beta) * (pointOnLine + halfDistance));
            CvInvoke.Line(dst, p1, p2, new MCvScalar(255), 1);
            return dst;
        }

        // ##### Constructors #####
        public Straight(double alpha = 0, double length = 0)
        {
            Alpha = alpha;
            Length = length;
        }
        public Straight(LineSegment2D line)
        {
            Alpha = Math.PI / 2 + Math.Atan2(line.Direction.Y * -1, line.Direction.X * -1);
            Length = Math.Cos(Alpha) * line.P1.X + Math.Sin(Alpha) * line.P1.Y;
        }

        public Straight(VectorOfFloat linePara)
        {
            double x0 = linePara[2];
            double y0 = linePara[3];
            double vX = linePara[0];
            double vY = linePara[1];
            // alpha
            Alpha = Math.PI / 2 + Math.Atan2(vY, vX);
            // length
            Length = Math.Cos(Alpha) * x0 +
                Math.Sin(Alpha) * y0;
        }
    }
    /// <summary>
    /// Enum to get type information from Point2D in case of polymorphy.
    /// </summary>
    public enum FeatureType
    {
        POINT_2D = 0,
        SPHERE_2D = 1,
        ELLIPSE_2D = 2,
        CORNER_2D = 3,
        SUBPIXELTEMPLATEMATCHING = 4,
        POINT_3D = 10
        //SPHERE_3D = 11,
        //ELLIPSE_3D = 12,
        //CORNER_3D = 13
    }
}
