using Microsoft.ML.Runtime.Api;

namespace Iris
{
    partial class Program
    {
        // STEP 1: Define your data structures

        // IrisData is used to provide training data, and as 
        // input for prediction operations
        // - First 4 properties are inputs/features used to predict the label
        // - Label is what you are predicting, and is only set when training
        public class IrisData
        {
            [Column("0")]
            public float SepalLength;

            [Column("1")]
            public float SepalWidth;

            [Column("2")]
            public float PetalLength;

            [Column("3")]
            public float PetalWidth;

            [Column("4")]
            [ColumnName("Label")]
            public string Label;
        }
    }
}
