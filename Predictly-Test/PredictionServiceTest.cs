using Predictly_Api.Models;
using Predictly_Api.Services;
using Predictly_Api.ViewModels.SchoolDashboard;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Predictly_Test
{
    public class PredictionServiceTest
    {
        [Fact]
        public void GetSchoolStudentsPredictionsTest()
        {
            var resultInput = new List<PredictedResultModel>() {
            new PredictedResultModel {
                    Id = 1,
                    UserId = "cc014c5c-2bd4-4c26-a1f6-75111ccace1a",
                    SubjectId = 1,
                    A = 75,
                    B = 10,
                    C = 8,
                    S = 5,
                    W = 2
                },
             new PredictedResultModel {
                    Id = 2,
                    UserId = "cc014c5c-2bd4-4c26-a1f6-75111ccace1b",
                    SubjectId = 1,
                    A = 10,
                    B = 75,
                    C = 8,
                    S = 5,
                    W = 2
                },
            };

            var dashboardPredictionData = new List<SchoolDashboardResultsPredictionData>()
            {
                new SchoolDashboardResultsPredictionData {
                    SubjectId = 1,
                    A = 1,
                    B = 1,
                    C = 0,
                    S = 0,
                    W = 0,
                    TotalCount = 2,
                }
             };

            PredictionService predictionService = new PredictionService();
            var result = predictionService.GetSchoolStudentsPredictions(resultInput);
            Assert.Equal(result.First().TotalCount, dashboardPredictionData.First().TotalCount);

        }
    }
}
