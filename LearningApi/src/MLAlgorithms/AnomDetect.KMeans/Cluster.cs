﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LearningFoundation.Clustering.KMeans
{
    /// <summary>
    /// Cluster is a class representing a cluster object (detailed results).
    /// </summary>
    [DataContract]
    public class Cluster
    {
        /// <summary>
        /// The original index of this cluster's samples before clustering
        /// </summary>
        [DataMember]
        public int[] ClusterDataOriginalIndex { get; internal set; }

        /// <summary>
        /// The samples that belong to this cluster
        /// </summary>
        [DataMember]
        public double[][] ClusterData { get; internal set; }

        /// <summary>
        /// Distance between eanch sample of this cluster and it's cetroid
        /// </summary>
        [DataMember]
        public double[] ClusterDataDistanceToCentroid { get; internal set; }

        /// <summary>
        /// The centroid of the cluster
        /// </summary>
        [DataMember]
        public double[] Centroid { get; internal set; }

        /// <summary>
        /// Currentlly calculated mean value of the cluster.
        /// M1 = mean of numOfSamples/2 (minibatch 1)
        /// M2 = mean for numbers from numOfSamples/2 to numOfSamples (minibatch 2)
        /// mean is batch for numbers from 1 to numOfSamples
        /// (1/q1+q2)[q1*M1+q2*M2]
        /// where q1 is number of elements inside of M1 and q2 number of elements inside of M2
        /// </summary>
        [DataMember]
        public double[] Mean { get; internal set; }

        /// <summary>
        /// The standard deviation in the cluster
        /// </summary>
        [DataMember]
        public double[] StandardDeviation { get; internal set; }

        /// <summary>
        /// The index of the farthest sample, of this cluster, from centroid (not original index)
        /// </summary>
        [DataMember]
        public int InClusterFarthestSampleIndex { get; internal set; }

        /// <summary>
        /// The farthest sample, of this cluster, from centroid
        /// </summary>
        [DataMember]
        public double[] InClusterFarthestSample { get; internal set; }

        /// <summary>
        /// Distance between the centroid and the farthest sample of this cluster
        /// </summary>
        [DataMember]
        public double InClusterMaxDistance { get; internal set; }

        /// <summary>
        /// Nearest cluster number
        /// </summary>
        [DataMember]
        public int NearestCluster { get; internal set; }

        /// <summary>
        /// Distance between the centroid of this cluster and that of nearest cluster
        /// </summary>
        [DataMember]
        public double DistanceToNearestClusterCentroid { get; internal set; }

        /// <summary>
        /// Nearest sample belonging of the nearest cluster to this cluster's centroid
        /// </summary>
        [DataMember]
        public double[] NearestForeignSampleInNearestCluster { get; internal set; }

        /// <summary>
        /// Distance between the nearest sample of the nearest cluster and this cluster's centroid
        /// </summary>
        [DataMember]
        public double DistanceToNearestForeignSampleInNearestCluster { get; internal set; }

        /// <summary>
        /// Nearest sample not belonging to this cluster and this cluster's centroid
        /// </summary>
        [DataMember]
        public double[] NearestForeignSample { get; internal set; }

        /// <summary>
        /// Distance between the nearest foreign sample and this cluster's centroid
        /// </summary>
        [DataMember]
        public double DistanceToNearestForeignSample { get; internal set; }

        /// <summary>
        /// The cluster to which the nearest foreign sample belongs
        /// </summary>
        [DataMember]
        public int ClusterOfNearestForeignSample { get; internal set; }
    }
}
