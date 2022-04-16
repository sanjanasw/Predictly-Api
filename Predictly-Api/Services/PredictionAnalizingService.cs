using System.Collections.Generic;
using System.Linq;
using Predictly_Api.Enums;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.Dashboard;
using Predictly_Api.ViewModels.SchoolDashboard;

namespace Predictly_Api.Services
{
    public interface IPredictionAnalizingService
    {
        public List<PredictedResultViewModel> GetStudentsOwnPredictions(List<PredictedResultModel> results, List<SubjectModel> subjects, List<GoalModel> goals);
        public List<SchoolDashboardResultsPredictionDataViewModel> GetSchoolStudentsPredictions(List<PredictedResultModel> results, List<SubjectModel> subjects);
        public List<ClassStatusViewModel> GetClassStatus(List<SubjectClassStatusViewModel> studyData, List<SubjectModel> subjects);
    }

    public class PredictionAnalizingService : IPredictionAnalizingService
    {
        public List<ClassStatusViewModel> GetClassStatus(List<SubjectClassStatusViewModel> studyData, List<SubjectModel> subjects)
        {
            var output = new List<ClassStatusViewModel>();
            var subjectClass = studyData.Where(x => x.ClassStatus == true).GroupBy(x => x.SubjectId).Select(y => new ClassStatusViewModel { Name = y.Key.ToString(), Value = y.Count() }).ToList();
            foreach (var item in subjectClass)
            {
                output.Add(new ClassStatusViewModel
                {
                    Name = subjects.Where(x => x.Id.ToString() == item.Name).Select(x => x.Name).FirstOrDefault(),
                    Value = item.Value,
                });
            }
            return output;
        }

        public List<SchoolDashboardResultsPredictionDataViewModel> GetSchoolStudentsPredictions(List<PredictedResultModel> results, List<SubjectModel> subjects)
        {

            var predictedResults = new List<UserSubjectPredictionViewModel>();
            foreach (var result in results)
            {
                IDictionary<Results, double> predictions = new Dictionary<Results, double>();
                predictions.Add(new KeyValuePair<Results, double>(Results.A, (double)result.A));
                predictions.Add(new KeyValuePair<Results, double>(Results.B, (double)result.B));
                predictions.Add(new KeyValuePair<Results, double>(Results.C, (double)result.C));
                predictions.Add(new KeyValuePair<Results, double>(Results.S, (double)result.S));
                predictions.Add(new KeyValuePair<Results, double>(Results.W, (double)result.W));
                var max = predictions.OrderByDescending(x => x.Value).First();
                predictedResults.Add(new UserSubjectPredictionViewModel
                {
                    PredictedResult = max.Key,
                    SubjectId = result.SubjectId,
                    UserId = result.UserId,
                });
            }

            var predictionCounts = predictedResults.GroupBy(x => x.SubjectId).ToList();
            var dashboardPredictionData = new List<SchoolDashboardResultsPredictionDataViewModel>();
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
                dashboardPredictionData.Add(new SchoolDashboardResultsPredictionDataViewModel
                {
                    Subject = subjects.Where(x => x.Id == prediction.Key).Select(x => x.Name).FirstOrDefault(),
                    A = A,
                    B = B,
                    C = C,
                    S = S,
                    W = W,
                    TotalCount = predictionList.Count,
                });
            }

            return dashboardPredictionData;
        }

        public List<PredictedResultViewModel> GetStudentsOwnPredictions(List<PredictedResultModel> results, List<SubjectModel>subjects, List<GoalModel> goals)
        {
            var predictedResults = new List<PredictedResultViewModel>();
            foreach (var item in results)
            {
                var subjectGoal = goals.Where(x => x.SubjectId == item.SubjectId).FirstOrDefault();
                string goal = null;
                if (subjectGoal != null)
                {
                    goal = subjectGoal.Goal.ToString();
                }
                predictedResults.Add(new PredictedResultViewModel
                {
                    Subject = subjects.Where(x => x.Id == item.SubjectId).Select(x => x.Name).FirstOrDefault(),
                    Goal = goal,
                    Result = new ResultViewModel
                    {
                        A = item.A,
                        B = item.B,
                        C = item.C,
                        S = item.S,
                        W = item.W,
                    }
                });
            }
            return predictedResults;
        }
    }
}
