using System.Linq;
using Microsoft.ML.Trainers.LightGbm;
using Microsoft.ML;

namespace Predictly_Api
{
    public partial class English
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
                                    .Append(mlContext.MulticlassClassification.Trainers.LightGbm(new LightGbmMulticlassTrainer.Options(){NumberOfLeaves=4,NumberOfIterations=6,MinimumExampleCountPerLeaf=20,LearningRate=0.884736893021567,LabelColumnName=@"Actual Result",FeatureColumnName=@"Features",ExampleWeightColumnName=null,Booster=new GradientBooster.Options(){SubsampleFraction=0.500040982653742,FeatureFraction=0.982449635845245,L1Regularization=5.41261598791082E-10,L2Regularization=0.999999776672986},MaximumBinCountPerFeature=495}))      
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName:@"PredictedLabel",inputColumnName:@"PredictedLabel"));

            return pipeline;
        }
    }
}
