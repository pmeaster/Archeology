﻿using System;
using System.Globalization;

namespace FractalSource.Mapping.Projection.Utm
{
    /// <summary>
    ///     UTM Coordinates need additionally the zone and the band info
    ///     to identify the grid to which the X/Y values belong
    /// </summary>
    public class UtmCoordinate : EuclideanCoordinate, IEquatable<UtmCoordinate>
    {
        private const double DefaultPrecision = 0.01;
        private bool _computed;
        private double _meridianConvergence; // stored in radians
        private double _scaleFactor;

        /// <summary>
        ///     Instantiate a new point on an UTM map
        /// </summary>
        /// <param name="grid">The UTM grid</param>
        /// <param name="easting">The X value</param>
        /// <param name="northern">The Y value</param>
        public UtmCoordinate(
            UtmGrid grid,
            double easting,
            double northern) : base(grid.Projection, easting, northern)
        {
            Grid = grid;
            _computed = false;
        }

        internal UtmCoordinate(UtmGrid grid,
            double easting,
            double northern,
            double scaleFactor,
            double meridianConvergence) : this(grid, easting, northern)
        {
            _scaleFactor = scaleFactor;
            _meridianConvergence = meridianConvergence;
            _computed = true;
        }

        /// <summary>
        ///     The UTM Grid this point belongs to.
        /// </summary>
        public UtmGrid Grid { get; private set; }

        /// <summary>
        ///     The scale factor at this location
        /// </summary>
        public double ScaleFactor
        {
            get
            {
                if (!_computed)
                    Compute();
                return _scaleFactor;
            }
        }

        /// <summary>
        ///     The meridian convergence at this location
        /// </summary>
        public Angle MeridianConvergence
        {
            get
            {
                if (!_computed)
                    Compute();
                return Angle.RadiansToDegrees(_meridianConvergence);
            }
        }

        /// <summary>
        ///     Check whether another Euclidean point belongs to the same projection
        /// </summary>
        /// <param name="other">The other point</param>
        /// <returns>True if they belong to the same projection, false otherwise</returns>
        protected override bool IsSameProjection(EuclideanCoordinate other)
        {
            return other is UtmCoordinate utmOther && utmOther.Grid.Equals(Grid);
        }

        private void Compute()
        {
            Grid.Projection.FromEuclidean(this, out _scaleFactor, out _meridianConvergence);
            _computed = true;
        }

        /// <summary>
        ///     Compute the Euclidean distance to another point
        /// </summary>
        /// <param name="other">The other point</param>
        /// <returns>The distance</returns>
        /// <exception cref="ArgumentException">Raised if the two points don't belong to the same projection</exception>
        public override double DistanceTo(EuclideanCoordinate other)
        {
            if (!(other is UtmCoordinate obj) || !Grid.Equals(obj.Grid))
                throw new ArgumentException();
            return base.DistanceTo(other);
        }

        /// <summary>
        ///     Check another UtmCoordinate for equality
        /// </summary>
        /// <param name="other">The other coordinates</param>
        /// <returns>True if they are equal</returns>
        public bool Equals(UtmCoordinate other)
        {
            return (other != null && other.Grid.Equals(Grid) &&
                    IsApproximatelyEqual(other, DefaultPrecision));
        }

        /// <summary>
        ///     Check another object for being an UtmCoordinate and for equality
        /// </summary>
        /// <param name="obj">The other object</param>
        /// <returns>True if the other object is an UtmCoordinate equal to this one</returns>
        public override bool Equals(object obj)
        {
            var other = obj as UtmCoordinate;
            return ((IEquatable<UtmCoordinate>)this).Equals(other);
        }

        /// <summary>
        ///     Get the hash code for this object
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        ///     The culture invariant string representation of the UTM coordinate
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var x = (long)Math.Floor(X);
            var y = (long)Math.Floor(Y);
            return Grid + " "
                        + x.ToString(NumberFormatInfo.InvariantInfo) + " "
                        + y.ToString(NumberFormatInfo.InvariantInfo);
        }

    }
}