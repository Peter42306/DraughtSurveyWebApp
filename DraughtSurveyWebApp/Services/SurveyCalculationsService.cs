using DraughtSurveyWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DraughtSurveyWebApp.Services
{
    public class SurveyCalculationsService
    {
        public void RecalculateAll(DraughtSurveyBlock block)
        {
            if (block == null) return;

            RecalculateDraughts(block);
            RecalculateDeductibles(block);
            RecalculateHydrostatics(block);
            
        }

        // Recalculation of DeductiblesInput and DeductiblesResult
        public void RecalculateDeductibles(DraughtSurveyBlock draughtSurveyBlock)
        {
            if (draughtSurveyBlock == null) return;

            if (draughtSurveyBlock.DeductiblesInput == null ||                
                draughtSurveyBlock.DeductiblesResults == null)
                return;

            var inputs = draughtSurveyBlock.DeductiblesInput;

            double? totalDeductible = null;

            if (inputs.Ballast.HasValue ||
                inputs.FreshWater.HasValue ||
                inputs.FuelOil.HasValue ||
                inputs.DieselOil.HasValue ||
                inputs.LubOil.HasValue ||
                inputs.Others.HasValue)
            {
                totalDeductible =
                (inputs.Ballast ?? 0) +
                (inputs.FreshWater ?? 0) +
                (inputs.FuelOil ?? 0) +
                (inputs.DieselOil ?? 0) +
                (inputs.LubOil ?? 0) +
                (inputs.Others ?? 0);
                //CalculateTotalDeductibles(inputs);
            }

            var results = draughtSurveyBlock.DeductiblesResults;

            results.TotalDeductibles = totalDeductible;
                        

        }


        // Recalculation of HydrostaticsInput and HydrostaticResults
        public void RecalculateHydrostatics(DraughtSurveyBlock draughtSurveyBlock)
        {
            if (draughtSurveyBlock == null) return;

            if (draughtSurveyBlock.HydrostaticInput == null ||
                draughtSurveyBlock.HydrostaticResults == null ||
                draughtSurveyBlock.DraughtsResults == null)
                return;


            var input = draughtSurveyBlock.HydrostaticInput;
            var results = draughtSurveyBlock.HydrostaticResults;
            var resultsDraughts = draughtSurveyBlock.DraughtsResults;

            double? displacementFromTable = null;
            double? tPCFromTable = null;
            double? lCFFromTable = null;

            double? meanAdjustedDraughtAfterKeelCorrection = resultsDraughts?.MeanAdjustedDraughtAfterKeelCorrection;

            double? draughtAbove = input.DraughtAbove;
            double? draughtBelow = input.DraughtBelow;

            double? displacementAbove = input.DisplacementAbove;
            double? displacementBelow = input.DisplacementBelow;
            double? tpcAbove = input.TPCAbove;
            double? tpcBelow = input.TPCBelow;
            double? lcfAbove = input.LCFAbove;
            double? lcfBelow = input.LCFBelow;


            if (!meanAdjustedDraughtAfterKeelCorrection.HasValue)
            {
                meanAdjustedDraughtAfterKeelCorrection = 0.0;
            }


            if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                draughtAbove.HasValue &&
                displacementAbove.HasValue &&
                draughtBelow.HasValue &&
                displacementBelow.HasValue)
            {
                displacementFromTable = CalculateLinearInterpolation(
                    draughtAbove.Value,
                    displacementAbove.Value,
                    draughtBelow.Value,
                    displacementBelow.Value,
                    meanAdjustedDraughtAfterKeelCorrection.Value);
            }
            else if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                     draughtAbove.HasValue &&
                     displacementAbove.HasValue)
            {
                displacementFromTable = displacementAbove;
            }
            else if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                     draughtBelow.HasValue &&
                     displacementBelow.HasValue)
            {
                displacementFromTable = displacementBelow;
            }
            
            




            if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                draughtAbove.HasValue && draughtAbove.Value != 0 &&
                tpcAbove.HasValue && tpcAbove.Value != 0 &&
                draughtBelow.HasValue && draughtBelow.Value != 0 &&
                tpcBelow.HasValue && tpcBelow.Value != 0)
            {
                tPCFromTable = CalculateLinearInterpolation(
                    draughtAbove.Value,
                    tpcAbove.Value,
                    draughtBelow.Value,
                    tpcBelow.Value,
                    meanAdjustedDraughtAfterKeelCorrection.Value);
            }
            else if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                     draughtAbove.HasValue && draughtAbove.Value != 0 &&
                     tpcAbove.HasValue && tpcAbove.Value != 0)
            {
                tPCFromTable = tpcAbove;
            }
            else if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                     draughtBelow.HasValue && draughtBelow.Value != 0 &&
                     tpcBelow.HasValue && tpcBelow.Value != 0)
            {
                tPCFromTable = tpcBelow;
            }
            else
            {
                tPCFromTable = null;
            }


            if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                draughtAbove.HasValue && draughtAbove.Value != 0 &&
                lcfAbove.HasValue && lcfAbove.Value != 0 &&
                draughtBelow.HasValue && draughtBelow.Value != 0 &&
                lcfBelow.HasValue && lcfBelow.Value != 0)
            {
                lCFFromTable = CalculateLinearInterpolation(
                    draughtAbove.Value,
                    lcfAbove.Value,
                    draughtBelow.Value,
                    lcfBelow.Value,
                    meanAdjustedDraughtAfterKeelCorrection.Value);
            }
            else if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                     draughtAbove.HasValue && draughtAbove.Value != 0 &&
                     lcfAbove.HasValue && lcfAbove.Value != 0)
            {
                lCFFromTable = lcfAbove;
            }
            else if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                     draughtBelow.HasValue && draughtBelow.Value != 0 &&
                     lcfBelow.HasValue && lcfBelow.Value != 0)
            {
                lCFFromTable = lcfBelow;
            }
            else
            {
                lCFFromTable = null;
            }



            // Calculation of draughts +/- 50 cm, above and below 
            double? mTCPlus50FromTable = null;
            double? mTCMinus50FromTable = null;
            double? mtcPlus50Above = input.MTCPlus50Above;
            double? mtcPlus50Below = input.MTCPlus50Below;
            double? mtcMinus50Above = input.MTCMinus50Above;
            double? mtcMinus50Below = input.MTCMinus50Below;



            if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                draughtAbove.HasValue && draughtAbove.Value != 0 &&
                mtcPlus50Above.HasValue && mtcPlus50Above.Value != 0 &&
                draughtBelow.HasValue && draughtBelow.Value != 0 &&
                mtcPlus50Below.HasValue && mtcPlus50Below.Value != 0
                )
            {
                mTCPlus50FromTable = CalculateLinearInterpolation(
                    draughtAbove.Value + 0.5,
                    mtcPlus50Above.Value,
                    draughtBelow.Value + 0.5,
                    mtcPlus50Below.Value,
                    meanAdjustedDraughtAfterKeelCorrection.Value + 0.5
                    );
            }
            else if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                     draughtAbove.HasValue && draughtAbove.Value != 0 &&
                     mtcPlus50Above.HasValue && mtcPlus50Above.Value != 0)
            {
                mTCPlus50FromTable = mtcPlus50Above;
            }
            else if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                     draughtBelow.HasValue && draughtBelow.Value != 0 &&
                     mtcPlus50Below.HasValue && mtcPlus50Below.Value != 0)
            {
                mTCPlus50FromTable = mtcPlus50Below;
            }
            else
            {
                mTCPlus50FromTable = null;
            }



            if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                draughtAbove.HasValue && draughtAbove.Value != 0 &&
                mtcMinus50Above.HasValue && mtcMinus50Above.Value != 0 &&
                draughtBelow.HasValue && draughtBelow.Value != 0 &&
                mtcMinus50Below.HasValue && mtcMinus50Below.Value != 0)
            {
                mTCMinus50FromTable = CalculateLinearInterpolation(
                    draughtAbove.Value - 0.5,
                    mtcMinus50Above.Value,
                    draughtBelow.Value - 0.5,
                    mtcMinus50Below.Value,
                    meanAdjustedDraughtAfterKeelCorrection.Value - 0.5
                    );
            }
            else if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                     draughtAbove.HasValue && draughtAbove.Value != 0 &&
                     mtcMinus50Above.HasValue && mtcMinus50Above.Value != 0)
            {
                mTCMinus50FromTable = mtcMinus50Above;
            }
            else if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                     draughtBelow.HasValue && draughtBelow.Value != 0 &&
                     mtcMinus50Below.HasValue && mtcMinus50Below.Value != 0)
            {
                mTCMinus50FromTable = mtcMinus50Below;
            }
            else
            {
                mTCMinus50FromTable = null;
            }



            // Calculation of the 1st trim corrrection            
            double? firstTrimCorrection = null;

            double? trimCorrected = draughtSurveyBlock.DraughtsResults?.TrimCorrected;
            double? lcf = draughtSurveyBlock.HydrostaticResults?.LCFFromTable;
            bool? isLCFForward = draughtSurveyBlock.HydrostaticInput?.IsLCFForward;
            double? tpc = draughtSurveyBlock.HydrostaticResults?.TPCFromTable;
            double? lbp = draughtSurveyBlock.Inspection?.VesselInput?.LBP;

            if (trimCorrected.HasValue &&
                lcf.HasValue &&
                isLCFForward.HasValue &&
                tpc.HasValue &&
                lbp.HasValue)
            {
                firstTrimCorrection = CalculateFirstTrimCorrection(
                    trimCorrected.Value,
                    lcf.Value,
                    isLCFForward.Value,
                    tpc.Value,
                    lbp.Value
                    );
            }



            // Calculcation of the 2nd trim correction

            double? secondTrimCorrection = null;

            double? mtcMinus50 = draughtSurveyBlock.HydrostaticResults?.MTCMinus50FromTable;
            double? mtcPlus50 = draughtSurveyBlock.HydrostaticResults?.MTCPlus50FromTable;

            if (trimCorrected.HasValue &&
                mtcMinus50.HasValue &&
                mtcPlus50.HasValue &&
                lbp.HasValue)
            {
                secondTrimCorrection = CalculateSecondTrimCorrection(
                    trimCorrected.Value,
                    mtcMinus50.Value,
                    mtcPlus50.Value,
                    lbp.Value
                    );
            }



            // Calculation of Displacement corrected for trim corrections

            double? displacementCorrectedForTrim = null;

            if (displacementFromTable.HasValue &&
                firstTrimCorrection.HasValue &&
                secondTrimCorrection.HasValue)
            {
                displacementCorrectedForTrim = CalculateDisplacementCorrectedForTrim(
                    displacementFromTable.Value,
                    firstTrimCorrection.Value,
                    secondTrimCorrection.Value
                    );
            }


            // Calculation of Displacement corrected for density

            double? displacementCorrectedForDensity = null;
            double? seaWaterDensity = draughtSurveyBlock.DraughtsInput?.SeaWaterDensity;

            if (displacementCorrectedForTrim.HasValue &&
                seaWaterDensity.HasValue)
            {
                displacementCorrectedForDensity = CalculateDisplacementCorrectedForDensity(
                    displacementCorrectedForTrim.Value,
                    seaWaterDensity.Value
                    );
            }



            // Calculation of NETTO displacement

            double? totalDeductibles = draughtSurveyBlock.DeductiblesResults?.TotalDeductibles;
            double? nettoDisplacement = null;

            if (displacementCorrectedForDensity.HasValue &&
                totalDeductibles.HasValue)
            {
                nettoDisplacement = CalculateNettoDisplacement(
                    displacementCorrectedForDensity.Value,
                    totalDeductibles.Value
                    );
            }



            // Calculation of Cargo + Constant

            double? cargoPlusConstant = null;
            double? lightShip = draughtSurveyBlock.Inspection?.VesselInput?.LS;

            if (nettoDisplacement.HasValue &&
                lightShip.HasValue
                )
            {
                cargoPlusConstant = CalculateCargoPlusConstant(
                    nettoDisplacement.Value,
                    lightShip.Value
                    );
            }







            if (displacementFromTable.HasValue)
            {
                results.DisplacementFromTable = displacementFromTable.Value;
            }

            if (tPCFromTable.HasValue)
            {
                results.TPCFromTable = tPCFromTable.Value;
            }

            if (lCFFromTable.HasValue)
            {
                results.LCFFromTable = lCFFromTable.Value;
            }

            if (mTCPlus50FromTable.HasValue)
            {
                results.MTCPlus50FromTable = mTCPlus50FromTable.Value;
            }

            if (mTCMinus50FromTable.HasValue)
            {
                results.MTCMinus50FromTable = mTCMinus50FromTable.Value;
            }

            if (firstTrimCorrection.HasValue)
            {
                results.FirstTrimCorrection = firstTrimCorrection.Value;
            }

            if (secondTrimCorrection.HasValue)
            {
                results.SecondTrimCorrection = secondTrimCorrection.Value;
            }

            if (displacementCorrectedForTrim.HasValue)
            {
                results.DisplacementCorrectedForTrim = displacementCorrectedForTrim.Value;
            }

            if (displacementCorrectedForDensity.HasValue)
            {
                results.DisplacementCorrectedForDensity = displacementCorrectedForDensity.Value;
            }

            if (nettoDisplacement.HasValue)
            {
                results.NettoDisplacement = nettoDisplacement.Value;
            }

            if (cargoPlusConstant.HasValue)
            {
                results.CargoPlusConstant = cargoPlusConstant.Value;
            }
        }

        // Recalculation of DraughtsInpus and DraughtsResults 
        public void RecalculateDraughts(DraughtSurveyBlock draughtSurveyBlock)
        {
            if (draughtSurveyBlock == null) return;

            if (draughtSurveyBlock.DraughtsInput == null || draughtSurveyBlock.DraughtsResults == null) return;


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
        




        public double CalculateApparentMean(double port, double starboard)
        {
            double result = Math.Round((port+starboard)/2, 3, MidpointRounding.AwayFromZero);
            return result;
        }

        public double CalculateTrim(double forward, double aft)
        {
            double result = Math.Round((aft-forward), 3, MidpointRounding.AwayFromZero);
            return result;
        }

        public double CalculateHoggingSagging(double forward, double mid, double aft)
        {
            double result = Math.Round(mid-((forward+aft)/2), 3, MidpointRounding.AwayFromZero);
            return result;
        }

        public double CalculateHeel(double draughtMidPS, double draughtMidSS, double BM)
        {
            double differenceBetweenPsAndSs = Math.Abs((draughtMidPS-draughtMidSS)/BM);
            double atan = Math.Atan(differenceBetweenPsAndSs);
            double degrees = CalculateRadToDeg(atan);
            double result = Math.Round(degrees, 3, MidpointRounding.AwayFromZero);            
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

            return Math.Round(result, 3, MidpointRounding.AwayFromZero);
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

            return Math.Round(result, 3, MidpointRounding.AwayFromZero);
        }

        public double CalculateCorrectedDraught(double draughtApparentMean, double correctionForDistasnce)
        {
            double result = Math.Round(draughtApparentMean + correctionForDistasnce, 3, MidpointRounding.AwayFromZero);
            return result;
        }

        public double CalculateMeanOfMean(double draughtMeanFwd, double draughtMeanMid, double draughtMeanAft)
        {
            double result = Math.Round((draughtMeanFwd + draughtMeanAft + (draughtMeanMid*6))/8, 3, MidpointRounding.AwayFromZero);
            return result;
        }

        public double CalculateMeanAdjustedDraughtAfterKeelCorrection(double meanAdjustedDraught, double keelCorrection)
        {           
            double result = meanAdjustedDraught + keelCorrection;
            return Math.Round(result, 3, MidpointRounding.AwayFromZero);
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
            return Math.Round(y, 3, MidpointRounding.AwayFromZero);
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

            return Math.Round(result, 3, MidpointRounding.AwayFromZero);
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

            return Math.Round(result, 3, MidpointRounding.AwayFromZero);
        }

        public double CalculateDisplacementCorrectedForTrim(double tableDisplacement, double firstTrimCorrection, double secondTrimCorrection)
        {           
            double result = tableDisplacement + firstTrimCorrection + secondTrimCorrection;

            return Math.Round(result, 3, MidpointRounding.AwayFromZero);
        }

        public double CalculateDisplacementCorrectedForDensity(double displacementCorrectedForTrim, double seaWaterDensity)
        {            
            double result = displacementCorrectedForTrim * seaWaterDensity / 1.025;

            return Math.Round(result, 3, MidpointRounding.AwayFromZero);
        }

        public double CalculateNettoDisplacement(double displacementCorrectedForDensity, double totalDeductibles)
        {
            double result = displacementCorrectedForDensity - totalDeductibles;

            return Math.Round(result, 3, MidpointRounding.AwayFromZero);
        }

        public double CalculateCargoPlusConstant(double nettoDisplacement, double lightShip)
        {
            double result = nettoDisplacement - lightShip;

            return Math.Round(result, 3, MidpointRounding.AwayFromZero);
        }

        public double CalculateTotalDeductibles(DeductiblesInput input)
        {
            double result =
                (input.Ballast ?? 0) +
                (input.FreshWater ?? 0) +
                (input.FuelOil ?? 0) +
                (input.DieselOil ?? 0) +
                (input.LubOil ?? 0) +
                (input.Others ?? 0); ;

            return Math.Round(result, 3, MidpointRounding.AwayFromZero);
        }


        //internal double CalculateApparentMean(double? draughtFwdPS, double? draughtFwdSS)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
