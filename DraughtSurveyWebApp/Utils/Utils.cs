namespace DraughtSurveyWebApp.Utils
{
    public static class Utils
    {
        public static bool AreEqual(double? a, double? b, double tolerance = 0.0001)
        {
            if (a.HasValue && b.HasValue)
            {
                return Math.Abs(a.Value - b.Value) < tolerance;
            }

            return a.HasValue == b.HasValue;
        }

        public static bool AreEqual(bool? a, bool? b)
        {
            if (a.HasValue && b.HasValue)
            {
                return a.Value == b.Value;
            }

            return a.HasValue == b.HasValue;
        }
    }
}
