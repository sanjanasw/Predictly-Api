using System.Linq;
using Microsoft.ML.Trainers.LightGbm;
using Microsoft.ML;

namespace Predictly_Prediction
{
    public partial class Buddhism
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
            var pipeline = mlContext.Transforms.Categorical.OneHotEncoding(new []{new InputOutputColumnPair(@"Father's Highest Education Level", @"Father's Highest Education Level"),new InputOutputColumnPair(@"Mother's Highest Education Level", @"Mother's Highest Education Level"),new InputOutputColumnPair(@"Study Hours", @"Study Hours")})      
                                    .Append(mlContext.Transforms.ReplaceMissingValues(@"Average Previous Marks", @"Average Previous Marks"))      
                                    .Append(mlContext.Transforms.Conversion.ConvertType(@"Class Status", @"Class Status"))      
                                    .Append(mlContext.Transforms.Concatenate(@"Features", new []{@"Father's Highest Education Level",@"Mother's Highest Education Level",@"Study Hours",@"Average Previous Marks",@"Class Status"}))      
                                    .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:@"Actual Result",inputColumnName:@"Actual Result"))      
                                    .Append(mlContext.MulticlassClassification.Trainers.LightGbm(new LightGbmMulticlassTrainer.Options(){NumberOfLeaves=4,NumberOfIterations=4,MinimumExampleCountPerLeaf=20,LearningRate=1,LabelColumnName=@"Actual Result",FeatureColumnName=@"Features",ExampleWeightColumnName=null,Booster=new GradientBooster.Options(){SubsampleFraction=1,FeatureFraction=1,L1Regularization=2E-10,L2Regularization=1},MaximumBinCountPerFeature=256}))      
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName:@"PredictedLabel",inputColumnName:@"PredictedLabel"));

            return pipeline;
        }
    }
}
