namespace DraughtSurveyWebApp.Utils
{
    public static class Utils
    {
        
        public static bool AreEqual(double? a, double? b, double tolerance = 0.0001)
        {
            if (!a.HasValue || !b.HasValue)
            {
                return false;
            }

            return Math.Abs(a.Value - b.Value) <= tolerance;            
        }

        
        public static bool AreEqual(bool? a, bool? b)
        {
            if (!a.HasValue || !b.HasValue)
            {
                return false;
            }

            return a.Value == b.Value;
        }


        public static double? TryDetectTableStep(List<double>draughts)
        {
            if (draughts == null)
            {
                return null;
            }

            if (draughts.Count == 2)
            {
                var stepFromTwoDraughts = GetStepIf2Draughts(draughts);
                if (stepFromTwoDraughts.HasValue)
                {
                    return stepFromTwoDraughts;
                }
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



        public static double? GetStepIf2Draughts(List<double> draughts, double tolerance = 0.0001)
        {
            if (draughts == null || draughts.Count != 2)
            {
                return null;
            }

            double d1 = Math.Round(draughts[0], 4, MidpointRounding.AwayFromZero);
            double d2 = Math.Round(draughts[1], 4, MidpointRounding.AwayFromZero);

            double step = Math.Abs(d1 - d2);

            return step > tolerance
                ? step
                : (double?)null;

        }

        
        public static double? GetStepIf4Draughts(List<double>draughts, double tolerance = 0.0001)
        {
            
            if (draughts == null || draughts.Count != 4)
            {
                return null;
            }

            var sortedDraughts = draughts
                .Select(d => Math.Round(d, 4))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            double d1 = Math.Round(sortedDraughts[0], 4);
            double d2 = Math.Round(sortedDraughts[1], 4);
            double d3 = Math.Round(sortedDraughts[2], 4);
            double d4 = Math.Round(sortedDraughts[3], 4);

            double step1 = Math.Abs(d1 - d2);
            double step2 = Math.Abs(d3 - d4);

            //if (AreEqual(step1, step2))
            //{
            //    return step1;
            //}            
            //else
            //{
            //    return null;
            //}

            return AreEqual(step1, step2) 
                ? step1 
                : (double?)null;

        }


        //public static double? GetStepIf6Draughts(List<double> draughts, double tolerance = 0.0001)
        //{

        //    if (draughts == null || draughts.Count != 4)
        //    {
        //        return null;
        //    }

        //    var sortedDraughts = draughts
        //        .Select(d => Math.Round(d, 4))
        //        .Distinct()
        //        .OrderBy(d => d)
        //        .ToList();

        //    var d1 = Math.Round(sortedDraughts[0], 4);
        //    var d2 = Math.Round(sortedDraughts[1], 4);
        //    var d3 = Math.Round(sortedDraughts[2], 4);
        //    var d4 = Math.Round(sortedDraughts[3], 4);
        //    var d5 = Math.Round(sortedDraughts[4], 4);
        //    var d6 = Math.Round(sortedDraughts[5], 4);

        //    var step1 = Math.Abs(d1 - d2);
        //    var step2 = Math.Abs(d3 - d4);
        //    var step3 = Math.Abs(d5 - d6);

        //    if (AreEqual(step1, step2) &&)
        //    {

        //    }




        //    if (Math.Abs(step1-step2) < tolerance)
        //    {
        //        return step1;
        //    }
        //    else
        //    {
        //        return null;
        //    }

        //}



        public static double? GetTableStepIfMore10Draughts(List<double>draughts, double tolerance=0.0001, double reliability = 0.8)
        {
            if(draughts == null || draughts.Count < 5)
            {
                return null;
            }

            var sortedDraughts = draughts
                .Select(d => Math.Round(d, 4, MidpointRounding.AwayFromZero))                
                .Where(d => !double.IsNaN(d) && !double.IsInfinity(d))
                .OrderBy(d => d)
                .Distinct()
                .ToList();

            if (sortedDraughts.Count < 5)
            {
                return null;
            }

            var steps = new List<double>();

            for (int i = 0; i < sortedDraughts.Count - 1; i++)
            {
                double delta = Math.Round(sortedDraughts[i + 1] - sortedDraughts[i], 5, MidpointRounding.AwayFromZero);

                // отбрасываем нули/шумы
                if (Math.Abs(delta) > tolerance)
                {
                    steps.Add(delta);
                }
            }

            if (steps.Count < 2)
            {
                return null;
            }

            var groupedSteps = steps
                .GroupBy(s => Math.Round(s, 4, MidpointRounding.AwayFromZero))
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .FirstOrDefault();

            double? mostCommonStep = groupedSteps?.Key;
            if (mostCommonStep == null)
            {
                return null;
            }

            int ok = steps.Count(s => Math.Abs(s - mostCommonStep.Value) < tolerance);

            return ok >= Math.Ceiling(steps.Count * reliability) 
                ? mostCommonStep 
                : null;            
        }
    }
}
