using Predictly_Api.ViewModels.Dashboard;

namespace Predictly_Api.Services
{
    public interface IPredictionService
    {
        public ResultViewModel GetPrediction(Buddhism.ModelInput model);
        public ResultViewModel FormatResult(float[] result);
    }

    public class PredictionService : IPredictionService
    {
        public ResultViewModel GetPrediction(Buddhism.ModelInput model)
        {
            return FormatResult(Buddhism.Predict(model).Score);
        }

        public ResultViewModel FormatResult(float[] result)
        {
            return new ResultViewModel
            {
                A = result[1],
                B = result[0],
                C = result[2],
                S = result[3],
                W = result[4],
            };
        }
    }
}
