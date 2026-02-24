using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.WebSockets;

namespace CandidateMatching.Services;

public sealed record Ideals(double[] Ideal, double[] AntiIdeal);

public sealed record IdealDistances(double IdealDistance, double AntiIdealDistance);

// Topsis Implementation of Ranking
public class TopsisRankingService(ILogger<TopsisRankingService> _logger): IRankingService
{
    public double[] PerformRanking(double[,] decisionMatrix, double[] weights)
    {
        _logger.Log(LogLevel.Information, "Starting ranking process");
        
        var normalized = GetNormalizedMatrix(decisionMatrix);
        var weightedNormalized = GetWeightedNormalizedMatrix(normalized, weights);
        var ideals = GetIdealSolutions(weightedNormalized);
        
        PrintMatrix(weightedNormalized);
        Console.WriteLine($"Best Possible:");
        PrintVector(ideals.Ideal);
        Console.WriteLine($"\nWorst Possible:");
        PrintVector(ideals.AntiIdeal);
        Console.WriteLine("\n");
        
        var distances = GetDistancesToIdealSolutions(weightedNormalized, ideals);
        
        foreach(var d in distances)
        {
            Console.WriteLine($"Distance to Best: {d.IdealDistance:F3}, to Worst: {d.AntiIdealDistance:F3}");    
        }

        var closenessFactors = GetRelativeClosenessToIdeal(distances);
        
        foreach(var c in closenessFactors)
        {
            Console.WriteLine($"Closeness: {c:F3}");    
        }
        
        return [];
    }

    // uses vector normalization
    public double[,] GetNormalizedMatrix(double[,] decisionMatrix)
    {
        int m = decisionMatrix.GetLength(0);
        int n = decisionMatrix.GetLength(1);

        double[,] resMatrix = new double[m,n];
        
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                double columnSum = 0;
                for (int k = 0; k < m; k++)
                {
                    columnSum += Math.Pow(decisionMatrix[k, j], 2);
                }
                // Console.WriteLine($"Current value: {decisionMatrix[i, j]}");
                resMatrix[i, j] = decisionMatrix[i, j] / Math.Sqrt(columnSum);
            }
        }
        
        return resMatrix;
    }

    public double[,] GetWeightedNormalizedMatrix(double[,] normalizedMatrix, double[] weights)
    {
        int m = normalizedMatrix.GetLength(0);
        int n = normalizedMatrix.GetLength(1);

        double[,] resMatrix = new double[m, n];
        
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                resMatrix[i, j] = normalizedMatrix[i, j] * weights[j];
            }
        }

        return resMatrix;
    }

    public Ideals GetIdealSolutions(double[,] decisionMatrix)
    {
        int m = decisionMatrix.GetLength(0);
        int n = decisionMatrix.GetLength(1);

        double[] idealSolution = new double[n];
        double[] antiIdealSolution = new double[n];
        
        for (int j = 0; j < n; j++)
        {
            double[] currentCol = new double[m];
            for (int i = 0; i < m; i++)
            {
                currentCol[i] = decisionMatrix[i, j];
            }

            idealSolution[j] = currentCol.Max();
            antiIdealSolution[j] = currentCol.Min();
        }

        return new Ideals(Ideal: idealSolution, AntiIdeal: antiIdealSolution);
    }

    public IdealDistances[] GetDistancesToIdealSolutions(double[,] decisionMatrix, Ideals ideals)
    {
        int m = decisionMatrix.GetLength(0);
        int n = decisionMatrix.GetLength(1);

        var distances = new IdealDistances[m];
        
        for (int i = 0; i < m; i++)
        {
            double idealDistanceSum = 0;
            double antiIdealDistanceSum = 0;
            
            for (int j = 0; j < n; j++)
            {
                double idealDistance = decisionMatrix[i, j] - ideals.Ideal[j];
                idealDistanceSum += Math.Pow(idealDistance, 2);
                double antiIdealDistance = decisionMatrix[i, j] - ideals.AntiIdeal[j];
                antiIdealDistanceSum += Math.Pow(antiIdealDistance, 2);
            }

            distances[i] = new IdealDistances(IdealDistance: Math.Sqrt(idealDistanceSum),
                AntiIdealDistance: Math.Sqrt(antiIdealDistanceSum));
        }

        return distances;
    }

    public double[] GetRelativeClosenessToIdeal(IdealDistances[] distances)
    {
        var closenessFactors = new double [distances.Length];
        for (int i = 0; i < distances.Length; i++)
        {
            closenessFactors[i] = distances[i].AntiIdealDistance / (distances[i].AntiIdealDistance + distances[i].IdealDistance);
        }

        return closenessFactors;
    }

    public double GetBestAlternativeIndex(double[] closenessFactors)
    {
        var max = closenessFactors.Max();
        var index = closenessFactors.IndexOf(max);
        return index;
    }

    private void PrintMatrix(double[,] matrix)
    {
        int m = matrix.GetLength(0);
        int n = matrix.GetLength(1);
        for (int i = 0; i < m; i++)
        {
            Console.Write("( ");
            for (int j = 0; j < n; j++)
            {
                Console.Write($"{matrix[i,j]:F3} ");
            }

            Console.Write("),\n");
        }
    }

    private void PrintVector(double[] vec)
    {
        foreach(var a in vec)
        {
            Console.Write($"{a:F3} ");
        }
    }
}