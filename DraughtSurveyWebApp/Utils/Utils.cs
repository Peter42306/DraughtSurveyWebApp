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

        public static double? TryDetectTableStep(List<double>draughts)
        {
            if (draughts == null || draughts.Count < 4)
            {
                return null;
            }

            if (draughts.Count == 4)
            {
                var stepFromFourDraughts = GetStepIf4Draughts(draughts);
                if (stepFromFourDraughts.HasValue)
                {
                    return stepFromFourDraughts;
                }
            }

            if (draughts.Count >= 5 && draughts.Count < 10)
            {
                var stepFromManyDraughts = GetTableStepIfMore10Draughts(draughts, reliability: 0.6);
                if (stepFromManyDraughts.HasValue)
                {
                    return stepFromManyDraughts;
                }

                return null;
            }

            if (draughts.Count >=10)
            {
                var stepFromManyDraughts = GetTableStepIfMore10Draughts(draughts, reliability: 0.8);
                if (stepFromManyDraughts.HasValue)
                {
                    return stepFromManyDraughts;
                }

                return null;
            }

            return null;
        }



        
        public static double? GetStepIf4Draughts(List<double>draughts, double tolerance = 0.0001)
        {
            
            if (draughts == null || draughts.Count < 4)
            {
                return null;
            }


            var sortedDraughts = draughts
                .Select(d => Math.Round(d, 4))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            if (sortedDraughts.Count != 4)
            {
                return null;
            }

            var step1 = Math.Abs(sortedDraughts[1] - sortedDraughts[0]);
            var step2 = Math.Abs(sortedDraughts[3] - sortedDraughts[2]);

            if (Math.Abs(step1-step2) < tolerance)
            {
                return step1;
            }
            else
            {
                return null;
            }

        }



        public static double? GetTableStepIfMore10Draughts(List<double>draughts, double tolerance=0.0001, double reliability = 0.8)
        {
            if(draughts.Count < 5)
            {
                return null;
            }

            var sortedDraughts = draughts
                .Select(d => Math.Round(d, 4))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

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
                .GroupBy(s => Math.Round(s, 4))
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
