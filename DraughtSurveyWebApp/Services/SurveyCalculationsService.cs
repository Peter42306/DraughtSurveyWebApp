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

        public double CalculatLBD(double lbp, double distanceFwd, bool distanceFwdShiftedToFwd, double distanceAft, bool distanceAftShiftedToFwd)
        {
            if (distanceFwdShiftedToFwd == false)
            {
                distanceFwd *= -1;
            }
            
            if (distanceAftShiftedToFwd == true)
            {
                distanceAft *= -1;
            }

            double result = lbp + distanceFwd + distanceAft;

            return Math.Round(result, 3);
        }

        public double CalculateTrimCorrection(double distance, double apparentTrim, bool isDraughtShiftedToForward, double LBD)
        {
            if (LBD == 0)
            {
                throw new ArgumentException("LBP cannot be zero", nameof(LBD));
            }

            double result = 0;

            if (apparentTrim == 0)
            {
                result = 0;
                return result;
            }           

            int sign = (isDraughtShiftedToForward == (apparentTrim < 0)) ? -1 : 1;

            result = sign * Math.Abs(distance*apparentTrim/LBD);

            return Math.Round(result, 3);
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

        public double CalculateFirstTrimCorrection(double correctedTrim, double LCF, bool isLCFForward, double TPC, double LBP)
        {
            if (LBP == 0)
            {
                throw new ArgumentException("LBP cannot be zero", nameof(LBP));
            }

            double result = 0;
            
            if (correctedTrim == 0)
            {
                result = 0;
                return result;
            }

            bool isTrimForward = false;

            if(correctedTrim < 0)
            {
                isTrimForward = true;
            }            

            int sign = (isLCFForward == isTrimForward) ? 1 : -1;                        
            
            result = sign * Math.Abs((correctedTrim * LCF * TPC * 100) / LBP);

            return Math.Round(result, 3);
        }

        public double CalculateSecondTrimCorrection(double correctedTrim, double MTC1, double MTC2, double LBP)
        {
            if (LBP == 0)
            {
                throw new ArgumentException("LBP cannot be zero", nameof(LBP));
            }

            double result = 0;
            
            if (correctedTrim == 0)
            {
                result = 0;
                return result;
            }

            result = (correctedTrim * correctedTrim * Math.Abs(MTC1 - MTC2) * 50) / LBP;

            return Math.Round(result, 3);
        }

        public double CalculateDisplacementCorrectedForTrim(double tableDisplacement, double firstTrimCorrection, double secondTrimCorrection)
        {           
            double result = tableDisplacement + firstTrimCorrection + secondTrimCorrection;

            return Math.Round(result, 3);
        }

        public double CalculateDisplacementCorrectedForDensity(double displacementCorrectedForTrim, double seaWaterDensity)
        {            
            double result = displacementCorrectedForTrim * seaWaterDensity / 1.025;

            return Math.Round(result, 3);
        }

        public double CalculateNettoDisplacement(double displacementCorrectedForDensity, double totalDeductibles)
        {
            double result = displacementCorrectedForDensity - totalDeductibles;

            return Math.Round(result, 3);
        }

        public double CalculateCargoPlusConstant(double nettoDisplacement, double lightShip)
        {
            double result = nettoDisplacement - lightShip;

            return Math.Round(result, 3);
        }


        //internal double CalculateApparentMean(double? draughtFwdPS, double? draughtFwdSS)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
