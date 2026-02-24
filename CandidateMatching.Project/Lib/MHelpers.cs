using System.Diagnostics;
using CandidateMatching.Domain;

namespace CandidateMatching.Lib;

public static class MHelpers
{
    extension(double[,] matrix)
    {
        public double[,] ApplyVectorNormalization()
        {
            int m = matrix.GetLength(0);
            int n = matrix.GetLength(1);

            double[,] resMatrix = new double[m,n];
        
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double columnSum = 0;
                    for (int k = 0; k < m; k++)
                    {
                        columnSum += Math.Pow(matrix[k, j], 2);
                    }
                    resMatrix[i, j] = matrix[i, j] / Math.Sqrt(columnSum);
                }
            }
        
            return resMatrix;
        }
        
        public double[,] ApplyLinearSumNormalization()
        {
            int m = matrix.GetLength(0);
            int n = matrix.GetLength(1);

            double[,] resMatrix = new double[m,n];

            for (int j = 0; j < n; j++)
            {
                double columnSum = 0;
                for (int i = 0; i < m; i++)
                {
                    columnSum += matrix[i,j];
                }

                for (int i = 0; i < m; i++)
                {
                    resMatrix[i, j] = matrix[i, j] / columnSum;
                }
            }
        
            return resMatrix;
        }
        
        public double[,] ApplyLinearMaxNormalization()
        {
            int m = matrix.GetLength(0);
            int n = matrix.GetLength(1);

            double[,] resMatrix = new double[m,n];

            for (int j = 0; j < n; j++)
            {
                double columnMax = Double.MinValue;
                
                for (int i = 0; i < m; i++)
                {
                    if (matrix[i, j] > columnMax) columnMax = matrix[i, j];
                }

                for (int i = 0; i < m; i++)
                {
                    resMatrix[i, j] = matrix[i, j] / columnMax;
                }
            }
        
            return resMatrix;
        }
        
        public double[,] ApplyMinMaxNormalization()
        {
            int m = matrix.GetLength(0);
            int n = matrix.GetLength(1);

            double[,] resMatrix = new double[m,n];

            for (int j = 0; j < n; j++)
            {
                double max = Double.MinValue;
                double min = Double.MaxValue;
                
                for (int i = 0; i < m; i++)
                {
                    if (matrix[i,j] < min) min = matrix[i, j];
                    if (matrix[i, j] > max) max = matrix[i, j];
                }

                var range = Math.Abs(min - max);

                for (int i = 0; i < m; i++)
                {
                    resMatrix[i, j] = (range < 1e-9) ? 0.5 : (matrix[i, j] - min) / (max - min);
                }
            }
        
            return resMatrix;
        }
        
        public double[,] ApplyWeights(double[] weights)
        {
            int m = matrix.GetLength(0);
            int n = matrix.GetLength(1);

            double[,] resMatrix = new double[m, n];
        
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    resMatrix[i, j] = matrix[i, j] * weights[j];
                }
            }

            return resMatrix;
        }
    }

    public static RankingResultDto SortResults(RankingResultDto ranking)
    {
        var res = ranking.Rankings.OrderByDescending(x => x.RankingVal).ToList();
        return new RankingResultDto(res);
    }

    public static bool WeightsAddUptoOne(double[] weights) => Math.Abs(weights.Sum() - 1.0) < 1e-9;

    public static double[] RoundRankingValues(RankingResultDto ranking, int? precision = null)
    {
        return ranking.Rankings.Select(c => Math.Round(c.RankingVal, precision ?? MConstants.RoundingPrecision)).ToArray();
    }
}