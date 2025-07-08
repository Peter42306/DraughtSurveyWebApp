using DraughtSurveyWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DraughtSurveyWebApp.Services
{
    public class SurveyCalculationsService
    {
        public void RecalculateAll(DraughtSurveyBlock draughtSurveyBlock)
        {
            if (draughtSurveyBlock == null) return;

            // Recalculation of DraughtsInpus and DraughtsResults
            if (draughtSurveyBlock.DraughtsInput != null)
            {
                draughtSurveyBlock.DraughtsResults ??= new DraughtsResults() 
                { 
                    DraughtSurveyBlockId = draughtSurveyBlock.Id,
                    DraughtSurveyBlock = draughtSurveyBlock
                };

                var input = draughtSurveyBlock.DraughtsInput;
                var result = draughtSurveyBlock.DraughtsResults;

                double? fwdMean = null;
                double? draughtFwdPS = input.DraughtFwdPS;
                double? draughtFwdSS = input.DraughtFwdSS;

                if (draughtFwdPS.HasValue &&
                    draughtFwdSS.HasValue)
                {
                    fwdMean = CalculateApparentMean(
                        draughtFwdPS.Value,
                        draughtFwdSS.Value
                        );
                }

                double? midMean = null;
                double? draughtMidPS = input.DraughtMidPS;
                double? draughtMidSS = input.DraughtMidSS;

                if (draughtMidPS.HasValue &&
                    draughtMidSS.HasValue)
                {
                    midMean = CalculateApparentMean(
                        draughtMidPS.Value,
                        draughtMidSS.Value
                        );
                }

                double? aftMean = null;
                double? draughtAftPS = input.DraughtAftPS;
                double? draughtAftSS = input.DraughtAftSS;

                if (draughtAftPS.HasValue &&
                    draughtAftSS.HasValue)
                {
                    aftMean = CalculateApparentMean(
                        draughtAftPS.Value,
                        draughtAftSS.Value
                        );
                }

                double? trimApparent = null;

                if (fwdMean.HasValue &&
                    aftMean.HasValue)
                {
                    trimApparent = CalculateTrim(
                        fwdMean.Value,
                        aftMean.Value
                        );
                }




                double? bm = draughtSurveyBlock.Inspection?.VesselInput?.BM;
                double? lbp = draughtSurveyBlock.Inspection?.VesselInput?.LBP;

                double? lbd = null;
                double? distanceFwd = input.DistanceFwd;
                bool? isFwdDistancetoFwd = input.isFwdDistancetoFwd;
                double? distanceAft = input.DistanceAft;
                bool? isAftDistanceToFwd = input.isAftDistanceToFwd;

                if (lbp.HasValue &&
                    distanceFwd.HasValue &&
                    isFwdDistancetoFwd.HasValue &&
                    distanceAft.HasValue &&
                    isAftDistanceToFwd.HasValue
                    )
                {
                    lbd = CalculatLBD(
                        lbp.Value,
                        distanceFwd.Value,
                        isFwdDistancetoFwd.Value,
                        distanceAft.Value,
                        isAftDistanceToFwd.Value
                        );
                }


                double? draughtCorrectionFwd = null;

                if (distanceFwd.HasValue &&
                    trimApparent.HasValue &&
                    isFwdDistancetoFwd.HasValue &&
                    lbd.HasValue)
                {
                    draughtCorrectionFwd = CalculateTrimCorrection(
                        distanceFwd.Value,
                        trimApparent.Value,
                        isFwdDistancetoFwd.Value,
                        lbd.Value
                        );
                }

                double? draughtCorrectionMid = null;

                double? distanceMid = input.DistanceMid;
                bool? isMidDistanceToFwd = input.isMidDistanceToFwd;

                if (distanceMid.HasValue &&
                    trimApparent.HasValue &&
                    isMidDistanceToFwd.HasValue &&
                    lbd.HasValue)
                {
                    draughtCorrectionMid = CalculateTrimCorrection(
                        distanceMid.Value,
                        trimApparent.Value,
                        isMidDistanceToFwd.Value,
                        lbd.Value
                        );
                }


                double? draughtCorrectionAft = null;

                if (distanceAft.HasValue &&
                    trimApparent.HasValue &&
                    isAftDistanceToFwd.HasValue &&
                    lbd.HasValue)
                {
                    draughtCorrectionAft = CalculateTrimCorrection(
                        distanceAft.Value,
                        trimApparent.Value,
                        isAftDistanceToFwd.Value,
                        lbd.Value
                        );
                }


                double? draughtCorrectedFwd = null;

                if (fwdMean.HasValue &&
                    draughtCorrectionFwd.HasValue)
                {
                    draughtCorrectedFwd = CalculateCorrectedDraught(
                    fwdMean.Value,
                    draughtCorrectionFwd.Value
                    );
                }



                double? draughtCorrectedMid = null;

                if (midMean.HasValue &&
                    draughtCorrectionMid.HasValue)
                {
                    draughtCorrectedMid = CalculateCorrectedDraught(
                        midMean.Value,
                        draughtCorrectionMid.Value
                        );
                }


                double? draughtCorrectedAft = null;

                if (aftMean.HasValue &&
                    draughtCorrectionAft.HasValue)
                {
                    draughtCorrectedAft = CalculateCorrectedDraught(
                        aftMean.Value,
                        draughtCorrectionAft.Value
                        );
                }


                double? trimCorrected = null;

                if (draughtCorrectedFwd.HasValue &&
                    draughtCorrectedAft.HasValue)
                {
                    trimCorrected = CalculateTrim(
                        draughtCorrectedFwd.Value,
                        draughtCorrectedAft.Value
                        );
                }


                double? heel = null;

                if (draughtMidPS.HasValue &&
                    draughtMidSS.HasValue &&
                    bm.HasValue)
                {
                    heel = CalculateHeel(
                        draughtMidPS.Value,
                        draughtMidSS.Value,
                        bm.Value
                        );
                }


                double? hogSag = null;

                if (draughtCorrectedFwd.HasValue &&
                    draughtCorrectedMid.HasValue &&
                    draughtCorrectedAft.HasValue)
                {
                    hogSag = CalculateHoggingSagging(
                        draughtCorrectedFwd.Value,
                        draughtCorrectedMid.Value,
                        draughtCorrectedAft.Value);
                }


                double? meanAdjustedDraught = null;

                if (draughtCorrectedFwd.HasValue &&
                    draughtCorrectedMid.HasValue &&
                    draughtCorrectedAft.HasValue)
                {
                    meanAdjustedDraught = CalculateMeanOfMean(
                        draughtCorrectedFwd.Value,
                        draughtCorrectedMid.Value,
                        draughtCorrectedAft.Value);
                }


                double? meanAdjustedDraughtAfterKeelCorrection = null;
                double? keelCorrection = input.KeelCorrection;

                if (!keelCorrection.HasValue)
                {
                    keelCorrection = 0;
                }

                if (meanAdjustedDraught.HasValue &&
                    keelCorrection.HasValue)
                {
                    meanAdjustedDraughtAfterKeelCorrection = CalculateMeanAdjustedDraughtAfterKeelCorrection(
                        meanAdjustedDraught.Value,
                        keelCorrection.Value
                        );
                }


                

                result.DraughtMeanFwd = fwdMean;
                result.DraughtMeanMid = midMean;
                result.DraughtMeanAft = aftMean;
                    
                result.TrimApparent = trimApparent;

                result.DraughtCorrectionFwd = draughtCorrectionFwd;
                result.DraughtCorrectionMid = draughtCorrectionMid;
                result.DraughtCorrectionAft = draughtCorrectionAft;

                result.DraughtCorrectedFwd = draughtCorrectedFwd;
                result.DraughtCorrectedMid = draughtCorrectedMid;
                result.DraughtCorrectedAft = draughtCorrectedAft;

                result.TrimCorrected = trimCorrected;
                result.Heel = heel;
                result.HoggingSagging = hogSag;
                result.MeanAdjustedDraught = meanAdjustedDraught;
                result.MeanAdjustedDraughtAfterKeelCorrection = meanAdjustedDraughtAfterKeelCorrection;
            }
        }




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

        public double CalculateMeanAdjustedDraughtAfterKeelCorrection(double meanAdjustedDraught, double keelCorrection)
        {           
            double result = meanAdjustedDraught + keelCorrection;
            return Math.Round(result, 3);
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
