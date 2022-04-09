using Microsoft.ML.Data;

namespace Predictly_Api.Models
{
    public class PredictionModelOutput
    {
        [ColumnName(@"Father's Highest Education Level")]
        public float Father_s_Highest_Education_Level { get; set; }

        [ColumnName(@"Mother's Highest Education Level")]
        public float Mother_s_Highest_Education_Level { get; set; }

        [ColumnName(@"Actual Result")]
        public uint Actual_Result { get; set; }

        [ColumnName(@"Class Status")]
        public float Class_Status { get; set; }

        [ColumnName(@"Average Previous Marks")]
        public float Average_Previous_Marks { get; set; }

        [ColumnName(@"Study Hours")]
        public float Study_Hours { get; set; }

        [ColumnName(@"Features")]
        public float[] Features { get; set; }

        [ColumnName(@"PredictedLabel")]
        public string PredictedLabel { get; set; }

        [ColumnName(@"Score")]
        public float[] Score { get; set; }
    }
}
