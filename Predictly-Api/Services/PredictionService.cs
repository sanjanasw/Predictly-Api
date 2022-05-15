using Microsoft.Extensions.Logging;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.Dashboard;
using System;
using System.Collections.Generic;

namespace Predictly_Api.Services
{
    public interface IPredictionService
    {
        public ResultViewModel GetPrediction(PredictionModelInput model, int subjectId);
        public ResultViewModel FormatResult(Dictionary<string, float> result);
    }

    public class PredictionService : IPredictionService
    {
        private readonly ILogger<PredictionService> _logger;
        public PredictionService(ILogger<PredictionService> logger)
        {
            _logger = logger;   
        }
        public ResultViewModel GetPrediction(PredictionModelInput model, int subjectId)
        {
            try
            {
                switch (subjectId)
                {
                    case 1:
                        return FormatResult(Buddhism.Predict(model));
                    case 2:
                        return FormatResult(Sinhala.Predict(model));
                    case 3:
                        return FormatResult(English.Predict(model));
                    case 4:
                        return FormatResult(History.Predict(model));
                    case 5:
                        return FormatResult(Science.Predict(model));
                    case 6:
                        return FormatResult(Mathematics.Predict(model));
                    case 7:
                        return FormatResult(Commerce.Predict(model));
                    case 8:
                        return FormatResult(Geography.Predict(model));
                    case 9:
                        return FormatResult(CivicEducation.Predict(model));
                    case 10:
                        return FormatResult(Tamil.Predict(model));
                    case 11:
                        return FormatResult(OrientedMusic.Predict(model));
                    case 12:
                        return FormatResult(Dancing.Predict(model));
                    case 13:
                        return FormatResult(Art.Predict(model));
                    case 14:
                        return FormatResult(ICT.Predict(model));
                    case 15:
                        return FormatResult(Helth.Predict(model));
                    default:
                        break;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Prediction Error");
                return null;
            }

        }

        public ResultViewModel FormatResult(Dictionary<string, float> result)
        {
            var A = 0.0;
            var B = 0.0;
            var C = 0.0;
            var S = 0.0;
            var W = 0.0;
            foreach (var scoreEntry in result)
            {
                switch (scoreEntry.Key)
                {
                    case "A":
                        A = scoreEntry.Value;
                        break;
                    case "B":
                        B = scoreEntry.Value;
                        break;
                    case "C":
                        C = scoreEntry.Value;
                        break;
                    case "S":
                        S = scoreEntry.Value;
                        break;
                    case "W":
                        W = scoreEntry.Value;
                        break;
                }
                Console.WriteLine($"Area: {scoreEntry.Key} Score: {scoreEntry.Value * 100}%");
            }
            return new ResultViewModel
            {
                W = Math.Round(W * 100, 2),
                S = Math.Round(S * 100, 2),
                A = Math.Round(A * 100, 2),
                B = Math.Round(B * 100, 2),
                C = Math.Round(C * 100, 2),
            };
        }
    }
}
