using System.Collections.Generic;
using System.Linq;
using Predictly_Api.Enums;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.SchoolDashboard;

namespace Predictly_Api.Services
{
    public interface IPredictionService
    {
        public List<SchoolDashboardResultsPredictionData> GetSchoolStudentsPredictions(List<PredictedResultModel> results);
    }

    public class PredictionService : IPredictionService
    {
        public List<SchoolDashboardResultsPredictionData> GetSchoolStudentsPredictions(List<PredictedResultModel> results)
        {

            var predictedResults = new List<UserSubjectPrediction>();
            foreach (var result in results)
            {
                IDictionary<double, Results> predictions = new Dictionary<double, Results>();
                predictions.Add(new KeyValuePair<double, Results>((double)result.A, Results.A));
                predictions.Add(new KeyValuePair<double, Results>((double)result.B, Results.B));
                predictions.Add(new KeyValuePair<double, Results>((double)result.C, Results.C));
                predictions.Add(new KeyValuePair<double, Results>((double)result.S, Results.S));
                predictions.Add(new KeyValuePair<double, Results>((double)result.W, Results.W));
                var max = predictions.OrderByDescending(x => x.Key).First();
                predictedResults.Add(new UserSubjectPrediction
                {
                    PredictedResult = max.Value,
                    SubjectId = result.SubjectId,
                    UserId = result.UserId,
                });
            }

            var predictionCounts = predictedResults.GroupBy(x => x.SubjectId).ToList();
            var dashboardPredictionData = new List<SchoolDashboardResultsPredictionData>();
            foreach (var prediction in predictionCounts)
            {
                var predictionList = prediction.ToList();
                int A = 0, B = 0, C = 0, S = 0, W = 0;
                foreach (var predictedValue in predictionList)
                {
                    switch (predictedValue.PredictedResult)
                    {
                        case Results.A:
                            A++;
                            break;
                        case Results.B:
                            B++;
                            break;
                        case Results.C:
                            C++;
                            break;
                        case Results.S:
                            S++;
                            break;
                        case Results.W:
                            W++;
                            break;
                    }
                }
                dashboardPredictionData.Add(new SchoolDashboardResultsPredictionData
                {
                    SubjectId = prediction.Key,
                    A = A,
                    B = B,
                    C = C,
                    S = S,
                    W = W,
                    TotalCount = predictionList.Count(),
                });
            }

            return dashboardPredictionData;
        }

    }
}
