using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftID.Utilities
{
    public static class MathExtension
    {
        public static T Min<T>(params T[] comparables) where T : IComparable<T>
        {
            if (comparables == null)
                throw new ArgumentNullException("comparables");
            if (comparables.Length == 0)
                throw new ArgumentOutOfRangeException("comparables");
            T minNumber = comparables[0];
            for (int i = 1; i < comparables.Length; i++)
            {
                if (minNumber.CompareTo(comparables[i]) > 0)
                    minNumber = comparables[i];
            }
            return minNumber;
        }

        public static T Max<T>(params T[] comparables) where T : IComparable<T>
        {
            if (comparables == null)
                throw new ArgumentNullException("comparables");
            if (comparables.Length == 0)
                throw new ArgumentOutOfRangeException("comparables");
            T maxNumber = comparables[0];
            for (int i = 1; i < comparables.Length; i++)
            {
                if (maxNumber.CompareTo(comparables[i]) < 0)
                    maxNumber = comparables[i];
            }
            return maxNumber;
        }
    }
}
