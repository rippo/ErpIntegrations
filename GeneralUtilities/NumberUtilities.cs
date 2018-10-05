using System;

namespace Webcrm.ErpIntegrations.GeneralUtilities
{
    public static class NumberUtilities
    {
        public static bool AreEquivalent(decimal? a, double? b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            // Check if only one of them is null.
            if (a == null ^ b == null)
            {
                return false;
            }

            return AreEquivalent(Convert.ToDouble(a.Value), b.Value);
        }

        // Based on https://stackoverflow.com/a/44355368/37147.
        // ReSharper disable CompareOfFloatsByEqualityOperator
        private static bool AreEquivalent(double a, double b)
        {
            if (a == b)
            {
                // Shortcut, handles infinities.
                return true;
            }

            double difference = Math.Abs(a - b);
            const double epsilon = 1e-15;
            if (a == 0 || b == 0 || difference < double.Epsilon)
            {
                // a or b is zero or both are extremely close to it. Relative error is less meaningful here.
                return difference < epsilon;
            }

            // Use relative error.
            return difference / (Math.Abs(a) + Math.Abs(b)) < epsilon;
        }
        // ReSharper enable CompareOfFloatsByEqualityOperator
    }
}