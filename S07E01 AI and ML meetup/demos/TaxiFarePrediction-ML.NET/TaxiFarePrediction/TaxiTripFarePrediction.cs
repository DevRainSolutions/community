using Microsoft.ML.Runtime.Api;

namespace TaxiFarePrediction
{
    public class TaxiTripFarePrediction
    {
        [ColumnName("Score")]
        public float FareAmount;
    }
}