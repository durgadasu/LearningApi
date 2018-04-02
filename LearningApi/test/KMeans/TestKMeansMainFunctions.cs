﻿using LearningFoundation;
using LearningFoundation.DataMappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using LearningFoundation.Clustering.KMeans;
using System.Reflection;

namespace Test
{
    /// <summary>
    /// UnitTest00 is a class that contains tests for the main functions of "KMeans.cs" 
    /// </summary>
    public class TestKMeansMainFunctions
    {
        private static string rootFolder = System.IO.Path.GetFullPath(@"..\..\..\") + "KMeans\\TestFiles\\JsonSaves\\";

        static TestKMeansMainFunctions()
        {
            if (Directory.Exists(rootFolder) == false)
            {
                Directory.CreateDirectory(rootFolder);
            }
        }

        #region Tests

        /// <summary>
        /// Test_TrainAndPredict is a test for the KMeans.Run or KMeans.Train and KMeans.Predict functions through LearningApi
        /// </summary>
        [Fact]
        public void Test_TrainAndPredict()
        {
            double[][] clusterCenters = new double[3][];
            clusterCenters[0] = new double[] { 5.0, 5.0 };
            clusterCenters[1] = new double[] { 15.0, 15.0 };
            clusterCenters[2] = new double[] { 30.0, 30.0 };

            string[] attributes = new string[] { "Height", "Weight" };

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 3;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            ClusteringSettings settings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 1);
          
            // Creates learning api object
            LearningApi api = new LearningApi(loadDescriptor());
            api.UseActionModule<object, double[][]>((data, ctx) =>
            {
                var rawData = Helpers.CreateSampleData(clusterCenters, 2, 10000, 0.5);
                return rawData ;
            });

            api.UseKMeans(settings);
            
            // train
            var resp = api.Run() as KMeansScore;
            
            Assert.True(resp.Model.Clusters != null);
            Assert.True(resp.Model.Clusters.Length == clusterCenters.Length);

            // Predict
            var result = api.Algorithm.Predict(clusterCenters, api.Context) as KMeansResult;
            Assert.True(result.PredictedClusters[0] == 0);
            Assert.True(result.PredictedClusters[1] == 1);
            Assert.True(result.PredictedClusters[2] == 2);
        }


        [Fact]
        public void Test_TrainAndPredictPartials()
        {
            double[][] clusterCenters = new double[3][];
            clusterCenters[0] = new double[] { 5.0, 5.0 };
            clusterCenters[1] = new double[] { 15.0, 15.0 };
            clusterCenters[2] = new double[] { 30.0, 30.0 };

            double[][] clusterCenters2 = new double[3][];
            clusterCenters2[0] = new double[] { 6, 5 };
            clusterCenters2[1] = new double[] { 17, 18};
            clusterCenters2[2] = new double[] { 28, 30 };

            string[] attributes = new string[] { "Height", "Weight" };

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 3;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            double[][] apiResp1Centroid = new double[numClusters][];
            double[][] apiResp2Centroid = new double[numClusters][];
            double[][] api2Resp1Centroid = new double[numClusters][];

            ClusteringSettings settings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 2);

            // Creates learning api object
            LearningApi api = new LearningApi(loadDescriptor());
            LearningApi api2 = new LearningApi(loadDescriptor());

            double[][] rawData = Helpers.CreateSampleData(clusterCenters, 2, 10000, 0.5);
            double[][] rawData2 = Helpers.CreateSampleData(clusterCenters2, 2, 5000, 0.5);

            int runNum = 0;

            api.UseActionModule<object, double[][]>((data, ctx) =>
            {
                if (runNum == 0)
                    return rawData; 
                else
                    return rawData2;
                
            });

            api2.UseActionModule<object, double[][]>((data, ctx) =>
            {
                return rawData2;
            });

            api.UseKMeans(settings);

            // train
            var apiResp = api.Run() as KMeansScore;

            Assert.True(apiResp.Model.Clusters != null);
            Assert.True(apiResp.Model.Clusters.Length == clusterCenters.Length);

            for (int i = 0; i < numClusters; i++)
            {
                apiResp1Centroid[i] = apiResp.Model.Clusters[i].Centroid;
            }
            
            
            // Predict
            var apiResult = api.Algorithm.Predict(clusterCenters, api.Context) as KMeansResult;
            Assert.True(apiResult.PredictedClusters[0] == 0);
            Assert.True(apiResult.PredictedClusters[1] == 1);
            Assert.True(apiResult.PredictedClusters[2] == 2);

            /// run with new data
            runNum++;

            settings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 2, initialCentroids: apiResp1Centroid);
            
            // train
            apiResp = api.Run() as KMeansScore;

            Assert.True(apiResp.Model.Clusters != null);
            Assert.True(apiResp.Model.Clusters.Length == clusterCenters.Length);

            for (int i = 0; i < numClusters; i++)
            {
                apiResp2Centroid[i] = apiResp.Model.Clusters[i].Centroid;
            }

            // Predict
            apiResult = api.Algorithm.Predict(clusterCenters, api.Context) as KMeansResult;
            Assert.True(apiResult.PredictedClusters[0] == 0);
            Assert.True(apiResult.PredictedClusters[1] == 1);
            Assert.True(apiResult.PredictedClusters[2] == 2);


            api2.UseKMeans(settings);

            // train
            var api2Resp = api2.Run() as KMeansScore;

            Assert.True(api2Resp.Model.Clusters != null);
            Assert.True(api2Resp.Model.Clusters.Length == clusterCenters.Length);

            for (int i = 0; i < numClusters; i++)
            {
                api2Resp1Centroid[i] = api2Resp.Model.Clusters[i].Centroid;
            }

            // Predict
            var api2Result = api2.Algorithm.Predict(clusterCenters, api2.Context) as KMeansResult;
            Assert.True(api2Result.PredictedClusters[0] == 0);
            Assert.True(api2Result.PredictedClusters[1] == 1);
            Assert.True(api2Result.PredictedClusters[2] == 2);

            //// compare

        }


        /// <summary>
        /// Test_Save is a test for KMeans.Save function
        /// </summary>
        [Fact]
        public void Test_Save()
        {

            double[][] clusterCenters = new double[3][];
            clusterCenters[0] = new double[] { 5.0, 5.0 };
            clusterCenters[1] = new double[] { 15.0, 15.0 };
            clusterCenters[2] = new double[] { 30.0, 30.0 };

            string[] attributes = new string[] { "Height", "Weight" };

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 3;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            ClusteringSettings settings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 2);

            // Creates learning api object
            LearningApi api = new LearningApi(loadDescriptor());

            // creates data
            var rawData = Helpers.CreateSampleData(clusterCenters, 2, 10000, 0.5);
            KMeans kMeans = new KMeans(settings);
            // train
            var response = kMeans.Run(rawData, api.Context);

            string fileName = "Test01.json";

            kMeans.Save(rootFolder + fileName);
        }

        /// <summary>
        /// Test_Load is a test for KMeans.Load function
        /// </summary>
        [Fact]
        public void Test_Load()
        {
            string fileName = "Test01.json";

            KMeans kMeans = new KMeans();
            kMeans.Load(rootFolder + fileName);

            Assert.True(kMeans.Instance != null);
            Assert.True(kMeans.Instance.Clusters != null);
        }

        /// <summary>
        /// Test_LoadSave is a test for the api.Save method
        /// </summary>
        [Fact]
        public void Test_ApiSave()
        {
            double[][] clusterCenters = new double[3][];
            clusterCenters[0] = new double[] { 5.0, 5.0 };
            clusterCenters[1] = new double[] { 15.0, 15.0 };
            clusterCenters[2] = new double[] { 30.0, 30.0 };

            string[] attributes = new string[] { "Height", "Weight" };

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 3;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            ClusteringSettings settings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 2);

            // Creates learning api object
            LearningApi api = new LearningApi(loadDescriptor());
            api.UseActionModule<object, double[][]>((data, ctx) =>
            {
                var rawData = Helpers.CreateSampleData(clusterCenters, 2, 10000, 0.5);
                return rawData;
            });

            api.UseKMeans(settings);

            var resp = api.Run() as KMeansScore;

            Assert.True(resp.Model.Clusters != null);
            Assert.True(resp.Model.Clusters.Length == clusterCenters.Length);

            var result = api.Algorithm.Predict(clusterCenters, api.Context) as KMeansResult;
            Assert.True(result.PredictedClusters[0] == 0);
            Assert.True(result.PredictedClusters[1] == 1);
            Assert.True(result.PredictedClusters[2] == 2);

            api.Save("unittestmodel.json");
        }

        #endregion

        #region Private Functions

        private static DataDescriptor loadDescriptor()
        {
            var des = new DataDescriptor();

            des.Features = new Column[2];
            des.Features[0] = new Column { Id = 1, Name = "Height", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0.0 };
            des.Features[1] = new Column { Id = 2, Name = "Weight", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0.0 };

            return des;
        }

        #endregion
    }
}
