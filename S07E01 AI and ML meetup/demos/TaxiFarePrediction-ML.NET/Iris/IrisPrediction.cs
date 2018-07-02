using Microsoft.ML.Runtime.Api;

namespace Iris
{
    // IrisPrediction is the result returned from prediction operations
    public class IrisPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel;
    }

}
