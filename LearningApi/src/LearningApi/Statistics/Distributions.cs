﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.Statistics
{
    static public class Distributions
    {
        /// <summary>
        /// calculates the "inverse standard normal cumulative distribution"
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double NormSInverse(double p)
        {
            // Coefficients in rational approximations
            var a = new double[]{-3.969683028665376e+01,  2.209460984245205e+02,
                      -2.759285104469687e+02,  1.383577518672690e+02,
                      -3.066479806614716e+01,  2.506628277459239e+00};


            var b = new double[]{-5.447609879822406e+01,  1.615858368580409e+02,
                      -1.556989798598866e+02,  6.680131188771972e+01,
                      -1.328068155288572e+01 };


            var c = new double[]{-7.784894002430293e-03, -3.223964580411365e-01,
                      -2.400758277161838e+00, -2.549732539343734e+00,
                      4.374664141464968e+00,  2.938163982698783e+00};


            var d = new double[]{7.784695709041462e-03, 3.224671290700398e-01,
                       2.445134137142996e+00,  3.754408661907416e+00};


            // Define break-points.
            var plow = 0.02425;
            var phigh = 1 - plow;


            // Rational approximation for lower region:
            if (p < plow)
            {
                var q = Math.Sqrt(-2 * Math.Log(p));
                return (((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
                                                ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
            }


            // Rational approximation for upper region:
            if (phigh < p)
            {
                var q = Math.Sqrt(-2 * Math.Log(1 - p));
                return -(((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
                                                       ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
            }


            // Rational approximation for central region:
            var q1 = p - 0.5;
            var r = q1 * q1;
            return (((((a[0] * r + a[1]) * r + a[2]) * r + a[3]) * r + a[4]) * r + a[5]) * q1 /
                                     (((((b[0] * r + b[1]) * r + b[2]) * r + b[3]) * r + b[4]) * r + 1);
        }

        /// <summary>
        /// calculates normal distribution
        /// </summary>
        /// <param name="x"> value</param>
        /// <param name="mean"> mean</param>
        /// <param name="standard_dev"> standard deviation</param>
        /// <param name="cumalative"> is cumulative</param>
        /// <returns></returns>
        public static double NormDist(double x, double mean, double standard_dev, bool cumalative)
        {
            if (cumalative == false)
            {
                double fact = standard_dev * Math.Sqrt(2.0 * Math.PI);
                double expo = (x - mean) * (x - mean) / (2.0 * standard_dev * standard_dev);
                return Math.Exp(-expo) / fact;
            }
            else
            {
                x = (x - mean) / standard_dev;
                if (x == 0)
                    return 0.5;
                double t = 1.0 / (1.0 + 0.2316419 * Math.Abs(x));
                double cdf = t * (1.0 / (Math.Sqrt(2.0 * Math.PI)))
                                * Math.Exp(-0.5 * x * x)
                                * (0.31938153 + t
                                * (-0.356563782 + t
                                * (1.781477937 + t
                                * (-1.821255978 + t * 1.330274429))));
                return x >= 0 ? 1.0 - cdf : cdf;
            }
        }

    }
}
