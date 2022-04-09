using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Predictly_Api.Models;

namespace Predictly_Api
{
    public partial class Science
    {
        private static string MLNetModelPath = Path.GetFullPath("Science.zip");

        public static readonly Lazy<PredictionEngine<PredictionModelInput, PredictionModelOutput>> PredictEngine = new Lazy<PredictionEngine<PredictionModelInput, PredictionModelOutput>>(() => CreatePredictEngine(), true);

        /// <summary>
        /// Use this method to predict on <see cref="PredictionModelInput"/>.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref="PredictionModelOutput"/></returns>
        public static Dictionary<string, float> Predict(PredictionModelInput input)
        {
            var predEngine = PredictEngine.Value;
            var result = predEngine.Predict(input);
            return GetScoresWithLabelsSorted(predEngine.OutputSchema, "Score", result.Score);
        }

        private static Dictionary<string, float> GetScoresWithLabelsSorted(DataViewSchema schema, string name, float[] scores)
        {
            Dictionary<string, float> result = new Dictionary<string, float>();

            var column = schema.GetColumnOrNull(name);

            var slotNames = new VBuffer<ReadOnlyMemory<char>>();
            column.Value.GetSlotNames(ref slotNames);
            var names = new string[slotNames.Length];
            var num = 0;
            foreach (var denseValue in slotNames.DenseValues())
            {
                result.Add(denseValue.ToString(), scores[num++]);
            }

            return result.OrderByDescending(c => c.Value).ToDictionary(i => i.Key, i => i.Value);
        }

        private static PredictionEngine<PredictionModelInput, PredictionModelOutput> CreatePredictEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<PredictionModelInput, PredictionModelOutput>(mlModel);
        }
    }
}
