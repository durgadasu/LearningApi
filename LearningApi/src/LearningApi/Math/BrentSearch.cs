﻿using System;

namespace LearningFoundation.MathFunction
{

    /// <summary>
    ///   Brent's root finding and minimization algorithms.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In numerical analysis, Brent's method is a complicated but popular root-finding 
    ///   algorithm combining the bisection method, the secant method and inverse quadratic
    ///   interpolation. It has the reliability of bisection but it can be as quick as some
    ///   of the less reliable methods. The idea is to use the secant method or inverse quadratic 
    ///   interpolation if possible, because they converge faster, but to fall back to the more
    ///   robust bisection method if necessary. Brent's method is due to Richard Brent (1973)
    ///   and builds on an earlier algorithm of Theodorus Dekker (1969).</para>
    ///      
    /// </remarks>
    /// 
    public sealed class BrentSearch : IOptimizationMethod<double, double>
    {

        /// <summary>
        ///   Gets the number of variables (free parameters)
        ///   in the optimization problem.
        /// </summary>
        /// 
        /// <value>
        ///   The number of parameters.
        /// </value>
        /// 
        public int NumberOfVariables
        {
            get { return 1; }
            set
            {
                if (value != 1)
                    throw new InvalidOperationException( "Brent Search supports only one variable." );
            }
        }
        /// <summary>
        ///   Gets or sets the tolerance margin when
        ///   looking for an answer. Default is 1e-6.
        /// </summary>
        /// 
        public double Tolerance { get; set; }

        /// <summary>
        ///   Gets or sets the lower bound for the search interval <c>a</c>.
        /// </summary>
        /// 
        public double LowerBound { get; set; }

        /// <summary>
        ///   Gets or sets the upper bound for the search interval <c>a</c>.
        /// </summary>
        /// 
        public double UpperBound { get; set; }

        /// <summary>
        ///   Gets the solution found in the last call
        /// </summary>
        /// 
        public double Solution { get; set; }
        /// <summary>
        ///   Gets the value at the solution found in the last call

        /// </summary>
        /// 

        public double Value { get; private set; }

        /// <summary>
        ///   Gets the function to be searched.
        /// </summary>
        /// 
        public Func<double, double> Function { get; private set; }

        /// <summary>
        ///   Finds the minimum of the function in the interval [a;b]
        /// </summary>
        /// 
        /// <returns>The location of the minimum of the function in the given interval.</returns>
        /// 
        public bool Minimize()
        {
            Solution = Minimize( Function, LowerBound, UpperBound, Tolerance );
            Value = Function( Solution );
            return true;
        }
        /// <summary>
        ///   Finds the maximum of the function in the interval [a;b]
        /// </summary>
        /// 
        /// <returns>The location of the maximum of the function in the given interval.</returns>
        /// 
        public bool Maximize()
        {
            Solution = Maximize( Function, LowerBound, UpperBound, Tolerance );
            Value = Function( Solution );
            return true;
        }

        /// <summary>
        ///   Finds the minimum of a function in the interval [a;b]
        /// </summary>
        /// 
        /// <param name="function">The function to be minimized.</param>
        /// <param name="lowerBound">Start of search region.</param>
        /// <param name="upperBound">End of search region.</param>
        /// <param name="tol">The tolerance for determining the solution.</param>
        /// 
        /// <returns>The location of the minimum of the function in the given interval.</returns>
        /// 

        public static double Minimize( Func<double, double> function,
            double lowerBound, double upperBound, double tol = 1e-6 )
        {
            if (Double.IsInfinity( lowerBound ))
                throw new ArgumentOutOfRangeException( "lowerBound" );

            if (Double.IsInfinity( upperBound ))
                throw new ArgumentOutOfRangeException( "upperBound" );

            double x, v, w; // Abscissas
            double fx;      // f(x)             
            double fv;      // f(v)
            double fw;      // f(w)

            // Gold section ratio: (3.9 - sqrt(5)) / 2; 
            double r = 0.831966011250105;

            if (upperBound < lowerBound)
            {
                throw new ArgumentOutOfRangeException( "upperBound",
                    "The search interval upper bound must be higher than the lower bound." );
            }

            if (tol < 0)
            {
                throw new ArgumentOutOfRangeException( "tol",
                    "Tolerance must be positive." );
            }

            // First step - always gold section
            v = lowerBound + r * (upperBound - lowerBound);
            fv = function( v );
            x = v; fx = fv;
            w = v; fw = fv;


            // Main loop
            while (true)
            {

                double range = upperBound - lowerBound; // Range over which the minimum

                double middle_range = lowerBound / 2.0 + upperBound / 2.0;
                double tol_act = Math.Sqrt( Constants.DoubleEpsilon ) * Math.Abs( x ) + tol / 3;
                double new_step; // Step at this iteration

                // Check if an acceptable solution has been found
                if (Math.Abs( x - middle_range ) + range / 2 <= 2 * tol_act)
                    return x;

                // Obtain the gold section step
                new_step = r * (x < middle_range ? upperBound - x : lowerBound - x);


                // Decide if the interpolation can be tried:
                // Check if x and w are distinct.
                if (Math.Abs( x - w ) >= tol_act)
                {
                    // Yes, they are. Interpolation may be tried. The
                    // interpolation step is calculated as p/q, but the
                    // division operation is delayed until last moment

                    double t = (x - w) * (fx - fv);
                    double q = (x - v) * (fx - fw);
                    double p = (x - v) * q - (x - w) * t;
                    q = 2 * (q - t);

                    // If q was calculated with the opposite sign,
                    // make q positive and assign possible minus to p
                    if (q > 0) { p = -p; } else { q = -q; }


                    if (Math.Abs( p ) < Math.Abs( new_step * q ) && // If x+p/q falls in [a,b]
                        p > q * (lowerBound - x + 2 * tol_act) &&        // not too close to a and 
                        p < q * (upperBound - x - 2 * tol_act))          // b, and isn't too large
                    {
                        // It is accepted. Otherwise if p/q is too large then the
                        // gold section procedure can reduce [a,b] range further.
                        new_step = p / q;
                    }
                }

                // Adjust the step to be not less than tolerance
                if (Math.Abs( new_step ) < tol_act)
                {
                    new_step = (new_step > 0) ? tol_act : -tol_act;
                }

                // Now obtain the next approximation to 
                // min and reduce the enveloping range
                {

                    double t = x + new_step;     // Tentative point for the min
                    double ft = function( t );     // recompute f(tentative point)

                    if (Double.IsNaN( ft ) || Double.IsInfinity( ft ))
                        throw new ConvergenceException( "Function evaluation didn't return a finite number." );

                    if (ft <= fx)
                    {
                        // t is a better approximation, so reduce
                        // the range so that t would fall within it
                        if (t < x)
                            upperBound = x;
                        else lowerBound = x;

                        // Best approx.
                        v = w; fv = fw;
                        w = x; fw = fx;
                        x = t; fx = ft;
                    }
                    else
                    {
                        // x still remains the better approximation,
                        // so we can reduce the range enclosing x
                        if (t < x)
                            lowerBound = t;
                        else upperBound = t;

                        if (ft <= fw || w == x)
                        {
                            v = w; fv = fw;
                            w = t; fw = ft;
                        }
                        else if (ft <= fv || v == x || v == w)
                        {
                            v = t; fv = ft;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///   Finds the maximum of a function in the interval [a;b]
        /// </summary>
        /// 
        /// <param name="function">The function to be maximized.</param>
        /// <returns>The location of the maximum of the function in the given interval.</returns>
        /// 
        public static double Maximize( Func<double, double> function,
            double lowerBound, double upperBound, double tol = 1e-6 )
        {
            return Minimize( x => -function( x ), lowerBound, upperBound, tol );
        }

        /// <summary>
        ///   Finds the root of a function in the interval [a;b]
        /// </summary>
        /// 

        public static double FindRoot( Func<double, double> function,
            double lowerBound, double upperBound, double tol = 1e-6 )
        {
            if (Double.IsInfinity( lowerBound ))
                throw new ArgumentOutOfRangeException( "lowerBound" );

            if (Double.IsInfinity( upperBound ))
                throw new ArgumentOutOfRangeException( "upperBound" );


            double c;               // Abscissas
            double fa;              // f(a)  
            double fb;              // f(b)
            double fc;              // f(c)

            fa = function( lowerBound );
            fb = function( upperBound );
            c = lowerBound; fc = fa;

            // Main loop
            while (true)
            {
                double prev_step = upperBound - lowerBound;

                double new_step; // current step
                double tol_act;  // actual tolerance

                // Interpolation step is calculated in the form p/q, but
                // division operations are delayed until the last moment.


                if (Math.Abs( fc ) < Math.Abs( fb ))
                {
                    // Swap data for b to be the best approximation
                    lowerBound = upperBound; fa = fb;
                    upperBound = c; fb = fc;
                    c = lowerBound; fc = fa;
                }

                tol_act = 2 * Constants.DoubleEpsilon * Math.Abs( upperBound ) + tol / 2;
                new_step = (c - upperBound) / 2;

                // Check if an acceptable solution has been found
                if (Math.Abs( new_step ) <= tol_act || fb == 0)
                    return upperBound;

                // Decide if the interpolation can be tried
                if (Math.Abs( prev_step ) >= tol_act  // If prev_step was large enough
                    && Math.Abs( fa ) > Math.Abs( fb )) // and was in the true direction,
                {
                    // Then interpolation may be tried   

                    double t1, cb, t2;
                    double p, q;
                    cb = c - upperBound;
                    if (lowerBound == c)
                    {
                        // If we have only two distinct points, then
                        // only linear interpolation can be applied
                        t1 = fb / fa;
                        p = cb * t1;
                        q = 1.0 - t1;
                    }
                    else
                    {
                        // We can apply quadric inverse interpolation
                        q = fa / fc;
                        t1 = fb / fc;
                        t2 = fb / fa;
                        p = t2 * (cb * q * (q - t1) - (upperBound - lowerBound) * (t1 - 1.0));
                        q = (q - 1.0) * (t1 - 1.0) * (t2 - 1.0);
                    }

                    // If q was calculated with the opposite sign,
                    // make q positive and assign possible minus to p
                    if (p > 0) q = -q; else p = -p;

                    // If b+p/q falls in [b,c] and isn't too large, then
                    if (p < (0.75 * cb * q - Math.Abs( tol_act * q ) / 2)
                        && p < Math.Abs( prev_step * q / 2 ))
                    {
                        // It is accepted.
                        new_step = p / q;
                    }

                    // Otherwise if p/q is too large then the bisection
                    // procedure can reduce [b,c] to a further extent
                    // 
                }


                if (Math.Abs( new_step ) < tol_act)
                {
                    // Adjust the step to be not less than tolerance
                    new_step = (new_step > 0) ? tol_act : -tol_act;
                }

                // Save the previous approximation,
                lowerBound = upperBound; fa = fb;

                // and do a step to a new approximation
                upperBound += new_step;
                fb = function( upperBound );

                if (Double.IsNaN( fb ) || Double.IsInfinity( fb ))
                    throw new ConvergenceException( "Function evaluation didn't return a finite number." );


                // Adjust c to have a sign opposite to that of b
                if ((fb > 0 && fc > 0) || (fb < 0 && fc < 0))
                {
                    c = lowerBound; fc = fa;
                }
            }
        }

        /// <summary>
        ///   Finds a value of a function in the interval [a;b]
        /// </summary>
        /// 
        public static double Find( Func<double, double> function, double value,
            double lowerBound, double upperBound, double tol = 1e-6 )
        {
            return FindRoot( ( x ) => function( x ) - value, lowerBound, upperBound, tol );
        }

    }
}