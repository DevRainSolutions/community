using System;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Models;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;


namespace TaxiFarePrediction
{
    class Program
    {
        const string _datapath = @".\Data\taxi-fare-train.csv";
        const string _testdatapath = @".\Data\taxi-fare-test.csv";
        const string _modelpath = @".\Data\Model.zip";

        static async Task Main(string[] args)
        {
            PredictionModel<TaxiTrip, TaxiTripFarePrediction> model = 
                await Train();
            Evaluate(model);
            var prediction = model.Predict(TestTrips.Trip1);
            Console.WriteLine("Predicted fare: {0}, actual fare: 29.5", prediction.FareAmount);
            Console.ReadLine();
        }

        public static async Task<PredictionModel<TaxiTrip, TaxiTripFarePrediction>> Train()
        {
            var pipeline = new LearningPipeline
            {
                new TextLoader(_datapath).CreateFrom<TaxiTrip>(separator: ','),
                new ColumnCopier(("FareAmount", "Label")),

                new CategoricalOneHotVectorizer("VendorId",
                    "RateCode",
                    "PaymentType"),

                new ColumnConcatenator("Features",
                    "VendorId",
                    "RateCode",
                    "PassengerCount",
                    "TripDistance",
                    "PaymentType"),

                new FastTreeRegressor()
            };

            PredictionModel<TaxiTrip, TaxiTripFarePrediction> model = 
                pipeline.Train<TaxiTrip, TaxiTripFarePrediction>();

            await model.WriteAsync(_modelpath);
            return model;
        }
        private static void Evaluate(PredictionModel<TaxiTrip, TaxiTripFarePrediction> model)
        {
            var testData = new TextLoader(_datapath).CreateFrom<TaxiTrip>(useHeader: true, separator: ',');
            var evaluator = new RegressionEvaluator();
            RegressionMetrics metrics = evaluator.Evaluate(model, testData);
            // Rms should be around 2.795276
            Console.WriteLine("Rms=" + metrics.Rms);
            Console.WriteLine("RSquared = " + metrics.RSquared);
        }
    }
}
