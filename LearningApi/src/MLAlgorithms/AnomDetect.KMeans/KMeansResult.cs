﻿using AnomalyDetection.Interfaces;
using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnomDetect.KMeans
{
    public class KMeansResult : IScore
    {
        public KMeansResult()
        {
        }

        public double[] Errors { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double[] Weights { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Cluster[] Clusters { get; internal set; }
    }
}
