using Predictly_Api.Enums;
using Predictly_Api.Models;
using Predictly_Api.Services;
using Predictly_Api.ViewModels.Dashboard;
using Predictly_Api.ViewModels.SchoolDashboard;
using System.Collections.Generic;
using System.Text.Json;
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
                SubjectId = 2,
                A = 10,
                B = 75,
                C = 8,
                S = 5,
                W = 2
            },
        };

        public List<SubjectModel> subjects = new List<SubjectModel>()
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

        [Fact]
        public void GetSchoolStudentsPredictionsTest()
        {
            var dashboardPredictionData = new List<SchoolDashboardResultsPredictionDataViewModel>()
            {
                new SchoolDashboardResultsPredictionDataViewModel {
                    Subject = "Buddhism",
                    A = 1,
                    B = 0,
                    C = 0,
                    S = 0,
                    W = 0,
                    TotalCount = 1,
                },
                 new SchoolDashboardResultsPredictionDataViewModel {
                    Subject = "English",
                    A = 0,
                    B = 1,
                    C = 0,
                    S = 0,
                    W = 0,
                    TotalCount = 1,
                }
             };

            PredictionService predictionService = new();
            var result = predictionService.GetSchoolStudentsPredictions(resultInput, subjects);
            Assert.Equal(JsonSerializer.Serialize(result), JsonSerializer.Serialize(dashboardPredictionData));

        }

        [Fact]
        public void GetStudentsOwnPredictionsTest()
        {
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
            Assert.Equal(JsonSerializer.Serialize(result), JsonSerializer.Serialize(studentOwnPredictionData));

        }

        [Fact]
        public void GetClassStatusDistribution()
        {
            var classData = new List<SubjectClassStatusViewModel>()
            {
                new SubjectClassStatusViewModel()
                {
                    SubjectId = 1,
                    ClassStatus = true
                },
                new SubjectClassStatusViewModel()
                {
                    SubjectId = 2,
                    ClassStatus = true
                },
                new SubjectClassStatusViewModel()
                {
                    SubjectId = 2,
                    ClassStatus = true
                },
                new SubjectClassStatusViewModel()
                {
                    SubjectId = 2,
                    ClassStatus = true
                },

            };

            var classDistribution = new List<ClassStatusViewModel>()
            {
                new ClassStatusViewModel()
                {
                    Value = 1,
                    Name = "Buddhism"
                },
                new ClassStatusViewModel()
                {
                    Value = 3,
                    Name = "English"
                }
            };

            PredictionService predictionService = new();
            var result = predictionService.GetClassStatus(classData, subjects);
            Assert.Equal(JsonSerializer.Serialize(result), JsonSerializer.Serialize(classDistribution));
        }
    }
}
