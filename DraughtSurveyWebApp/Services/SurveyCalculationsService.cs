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

        public double CalculateDraughtForMTCPlus50(double tableDraught)
        {
            double result = tableDraught + 0.5;
            return result;
        }

        public double CalculateDraughtForMTCMinus50(double tableDraught)
        {
            double result = tableDraught - 0.5;
            return result;
        }        

        public double CalculateLinearInterpolation(double x0, double y0, double x1, double y1, double x)
        {
            if (x1 == x0)
            {
                return Math.Round(y0, 3);
            }

            double y = y0 + ((y1 - y0) / (x1 - x0)) * (x - x0);
            return Math.Round(y, 3);
        }


        //internal double CalculateApparentMean(double? draughtFwdPS, double? draughtFwdSS)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
