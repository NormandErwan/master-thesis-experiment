﻿using System;

namespace NormandErwan.MasterThesisExperiment.Utilities
{
    /// <summary>
    /// Represent a range.
    /// </summary>
    /// <typeparam name="T">Generic parameter.</typeparam>
    /// <remarks>
    /// Based on: https://stackoverflow.com/a/5343033
    /// </remarks>
    public class Range<T> where T : IComparable<T>
    {
        // Properties

        /// <summary>
        /// Minimum value of the range.
        /// </summary>
        public virtual T Minimum { get; protected set; }

        /// <summary>
        /// Maximum value of the range.
        /// </summary>
        public virtual T Maximum { get; protected set; }

        // Constructors

        public Range(T minimum, T maximum) : base()
        {
            Minimum = minimum;
            Maximum = maximum;
            CheckValidity();
        }

        protected Range() : base()
        {
        }

        // Methods

        /// <summary>
        /// Presents the Range in readable format.
        /// </summary>
        /// <returns>String representation of the Range</returns>
        public override string ToString()
        {
            return string.Format("[{0} - {1}]", Minimum, Maximum);
        }

        /// <summary>
        /// Determines if the provided value is inside the range.
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <returns>True if the value is inside Range, else false</returns>
        public virtual bool ContainsValue(T value)
        {
            return (Minimum.CompareTo(value) <= 0) && (value.CompareTo(Maximum) <= 0);
        }

        /// <summary>
        /// Determines if another range is inside the bounds of this range.
        /// </summary>
        /// <param name="Range">The range to test</param>
        /// <returns>True if range is inside, else false</returns>
        public virtual bool ContainsRange(Range<T> range)
        {
            return ContainsValue(range.Minimum) && ContainsValue(range.Maximum);
        }

        protected virtual void CheckValidity()
        {
            if (Minimum.CompareTo(Maximum) > 0)
            {
                throw new ArgumentOutOfRangeException("minimum should be smaller or equal to maximum.");
            }
        }
    }
}