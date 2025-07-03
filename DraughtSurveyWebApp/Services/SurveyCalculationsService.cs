namespace DraughtSurveyWebApp.Services
{
    public class SurveyCalculationsService
    {
        public double CalculateApparentMean(double port, double starboard)
        {
            double result = Math.Round((port+starboard)/2, 3);
            return result;
        }

        public double CalculateTrim(double forward, double aft)
        {
            double result = Math.Round((aft-forward), 3);
            return result;
        }

        public double CalculateHoggingSagging(double forward, double mid, double aft)
        {
            double result = Math.Round(mid-((forward+aft)/2), 3);
            return result;
        }

        public double CalculateHeel(double draughtMidPS, double draughtMidSS, double BM)
        {
            double differenceBetweenPsAndSs = Math.Abs((draughtMidPS-draughtMidSS)/BM);
            double atan = Math.Atan(differenceBetweenPsAndSs);
            double degrees = CalculateRadToDeg(atan);
            double result = Math.Round(degrees, 3);            
            return result;
        }

        public double CalculateRadToDeg(double radians)
        {
            double result = radians * (180 / Math.PI);
            return result;
        }

        public double CalculateTrimCorrection(double distance, double apparentTrim, double LBD)
        {
            double result = Math.Round(distance*apparentTrim/LBD, 3);
            return result;
        }

        public double CalculateCorrectedDraught(double draughtApparentMean, double correctionForDistasnce)
        {
            double result = Math.Round(draughtApparentMean + correctionForDistasnce, 3);
            return result;
        }

        public double CalculateMeanOfMean(double draughtMeanFwd, double draughtMeanMid, double draughtMeanAft)
        {
            double result = Math.Round((draughtMeanFwd + draughtMeanAft + (draughtMeanMid*6))/8, 3);
            return result;
        }

        public double CalculateDraughtForMTCAbovePlus50(double tableDraughtAbove)
        {
            double result = tableDraughtAbove + 0.5;
            return result;
        }

        public double CalculateDraughtForMTCAboveMinus50(double tableDraughtAbove)
        {
            double result = tableDraughtAbove - 0.5;
            return result;
        }

        public double CalculateDraughtForMTCBelowPlus50(double tableDraughtBelow)
        {
            double result = tableDraughtBelow + 0.5;
            return result;
        }

        public double CalculateDraughtForMTCBelowMinus50(double tableDraughtBelow)
        {
            double result = tableDraughtBelow - 0.5;
            return result;
        }


        //internal double CalculateApparentMean(double? draughtFwdPS, double? draughtFwdSS)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
