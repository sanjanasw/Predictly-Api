using Predictly_Api.Enums;
using Predictly_Api.Models;
using Predictly_Api.Services;
using Predictly_Api.ViewModels.Dashboard;
using Predictly_Api.ViewModels.SchoolDashboard;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Predictly_Test
{
    public class PredictionServiceTest
    {
        public List<PredictedResultModel> resultInput = new()
        {
            new PredictedResultModel
            {
                Id = 1,
                UserId = "cc014c5c-2bd4-4c26-a1f6-75111ccace1a",
                SubjectId = 1,
                A = 75,
                B = 10,
                C = 8,
                S = 5,
                W = 2
            },
            new PredictedResultModel
            {
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

        [Fact]
        public void GetSchoolStudentsPredictionsTest()
        {
            var dashboardPredictionData = new List<SchoolDashboardResultsPredictionDataViewModel>()
            {
                new SchoolDashboardResultsPredictionDataViewModel {
                    SubjectId = 1,
                    A = 1,
                    B = 1,
                    C = 0,
                    S = 0,
                    W = 0,
                    TotalCount = 2,
                }
             };

            PredictionService predictionService = new();
            var result = predictionService.GetSchoolStudentsPredictions(resultInput);
            Assert.Equal(result.First().TotalCount, dashboardPredictionData.First().TotalCount);

        }

        [Fact]
        public void GetStudentsOwnPredictionsTest()
        {
            var subjects = new List<SubjectModel>()
           {
               new SubjectModel()
               {
                   Id= 1,
                   Name = "Buddhism"
               },
               new SubjectModel()
               {
                   Id= 2,
                   Name = "English"
               },
           };

            var goals = new List<GoalModel>()
           {
               new GoalModel()
               {
                   Id = 1,
                   Goal = Results.A,
                   SubjectId = 1,
                   UserId = "cc014c5c-2bd4-4c26-a1f6-75111ccace1a",
               },
               new GoalModel()
               {
                   Id = 2,
                   Goal = Results.B,
                   SubjectId = 2,
                   UserId = "cc014c5c-2bd4-4c26-a1f6-75111ccace1a",
               }
           };

            var studentOwnPredictionData = new List<PredictedResultViewModel>()
            {
                new PredictedResultViewModel()
                {
                    Goal = "A",
                    Result = new ResultViewModel()
                    {
                        A = 75,
                        B = 10,
                        C = 8,
                        S = 5,
                        W = 2
                    },
                    Subject = "Buddhism",
                },
                 new PredictedResultViewModel()
                {
                    Goal = "B",
                    Result = new ResultViewModel()
                    {
                        A = 10,
                        B = 75,
                        C = 8,
                        S = 5,
                        W = 2
                    },
                    Subject = "English",
                }
            };

            PredictionService predictionService = new();
            var result = predictionService.GetStudentsOwnPredictions(resultInput, subjects, goals);
            Assert.Equal(result.First().Subject, studentOwnPredictionData.First().Subject);

        }
    }
}
