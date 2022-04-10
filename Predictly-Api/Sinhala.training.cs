using System.Linq;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML;

namespace Predictly_Api
{
    public partial class Sinhala
    {
        public static ITransformer RetrainPipeline(MLContext context, IDataView trainData)
        {
            var pipeline = BuildPipeline(context);
            var model = pipeline.Fit(trainData);

            return model;
        }

        /// <summary>
        /// build the pipeline that is used from model builder. Use this function to retrain model.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            // Data process configuration with pipeline data transformations
            var pipeline = mlContext.Transforms.ReplaceMissingValues(new []{new InputOutputColumnPair(@"Father's Highest Education Level", @"Father's Highest Education Level"),new InputOutputColumnPair(@"Mother's Highest Education Level", @"Mother's Highest Education Level"),new InputOutputColumnPair(@"Average Previous Marks", @"Average Previous Marks"),new InputOutputColumnPair(@"Study Hours", @"Study Hours")})      
                                    .Append(mlContext.Transforms.Conversion.ConvertType(@"Class Status", @"Class Status"))      
                                    .Append(mlContext.Transforms.Concatenate(@"Features", new []{@"Father's Highest Education Level",@"Mother's Highest Education Level",@"Average Previous Marks",@"Study Hours",@"Class Status"}))      
                                    .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:@"Actual Result",inputColumnName:@"Actual Result"))      
                                    .Append(mlContext.MulticlassClassification.Trainers.OneVersusAll(binaryEstimator:mlContext.BinaryClassification.Trainers.FastForest(new FastForestBinaryTrainer.Options(){NumberOfTrees=1997,NumberOfLeaves=60,FeatureFraction=0.4569804F,LabelColumnName=@"Actual Result",FeatureColumnName=@"Features"}),labelColumnName:@"Actual Result"))      
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName:@"PredictedLabel",inputColumnName:@"PredictedLabel"));

            return pipeline;
        }
    }
}
