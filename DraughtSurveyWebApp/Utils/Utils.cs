namespace DraughtSurveyWebApp.Utils
{
    public static class Utils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool AreEqual(double? a, double? b, double tolerance = 0.0001)
        {
            if (a.HasValue && b.HasValue)
            {
                return Math.Abs(a.Value - b.Value) < tolerance;
            }

            return a.HasValue == b.HasValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AreEqual(bool? a, bool? b)
        {
            if (a.HasValue && b.HasValue)
            {
                return a.Value == b.Value;
            }

            return a.HasValue == b.HasValue;
        }



        // Necessary check if there are 4 inputs, 2 for initial and 2 for final draught survey
        public static double? GetStepIf4Draughts(List<double>draughts, double tolerance = 0.0001)
        {
            // Check if draughts list is null or has less than 4 elements
            if (draughts == null || draughts.Count < 4)
            {
                return null;
            }

            // Remove duplicates and sort the draughts
            var sortedDraughts = draughts.Distinct().OrderBy(d => d).ToList();

            // If there are less than 2 unique draughts after sorting, return null
            var steps = new List<double>();

            // Check the number of unique draughts
            for (int i = 0; i < sortedDraughts.Count - 1; i++)
            {
                // Calculate the difference between consecutive draughts
                double delta = sortedDraughts[i + 1] - sortedDraughts[i];
                
                if (Math.Abs(delta) > 0)
                {
                    // Add the delta to the steps list if it's not zero
                    steps.Add(delta);
                }
            }

            // If there are less than 2 steps, return null
            if (steps.Count < 2)
            {
                return null;
            }

            // Group the steps by their values and order them by count in descending order
            var mostCommonStep = steps
                .GroupBy(s => s)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key;

            // If there are less than 2 steps are not equal within the tolerance, return null
            if (
                (mostCommonStep != null) &&
                (steps.Count(s => Math.Abs(s - mostCommonStep.Value) < tolerance) < 2)
                )
            {
                return null;
            }

            // Return the most common step if it exists
            return mostCommonStep;

        }



        public static double? GetTableStepIfMore10Draughts(List<double>draughts, double tolerance=0.0001, double reliability = 0.8)
        {
            if(draughts.Count < 20)
            {
                return null;
            }

            var sortedDraughts = draughts.Distinct().OrderBy(d => d).ToList();
            var steps = new List<double>();
            for (int i = 0; i < sortedDraughts.Count - 1; i++)
            {
                double delta = Math.Round(sortedDraughts[i + 1] - sortedDraughts[i], 5);
                
                if (Math.Abs(delta) > 0)
                {
                    steps.Add(delta);
                }
            }

            if (steps.Count < 2)
            {
                return null;
            }

            var groupedSteps = steps
                .GroupBy(s => s)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            var mostCommonStep = groupedSteps?.Key;
            if (mostCommonStep == null)
            {
                return null;
            }

            int countWithTolerance = steps.Count(s => Math.Abs(s - mostCommonStep.Value) < tolerance);
            return countWithTolerance >= steps.Count * reliability ? mostCommonStep : null;
        }
    }
}
