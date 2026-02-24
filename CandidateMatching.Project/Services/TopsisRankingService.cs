using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.WebSockets;
using System.Reflection.Emit;
using CandidateMatching.Domain;
using CandidateMatching.Utils;

namespace CandidateMatching.Services;

// Topsis Implementation of Ranking
public class TopsisRankingService(ILogger<TopsisRankingService> _logger): IRankingService
{
    public RankingResultDto PerformRanking(List<CandidateDto> candidates, double[] weights)
    {
        _logger.Log(LogLevel.Information, "Starting ranking process");
        
        var matrixBuilder = new CandidateMatrixBuilder();
        matrixBuilder.AddRows(candidates);
        var matrix = matrixBuilder.Build();
            
        var normalized = GetNormalizedMatrix(matrix);
        var weightedNormalized = GetWeightedNormalizedMatrix(normalized, weights);
        var ideals = GetIdealSolutions(weightedNormalized);
        var distances = GetDistancesToIdealSolutions(weightedNormalized, ideals);
        var closenessFactors = GetRelativeClosenessToIdeal(distances);
        var ranking = MapCandidatesToResults(closenessFactors, candidates);
        
        MDebug.PrintMatrix(weightedNormalized, label: "Normalized (without weights)");
        MDebug.PrintMatrix(weightedNormalized, label: "Weighted Normalized");
        MDebug.PrintVector(ideals.Ideal, label: "Best Possible (A*)");
        MDebug.PrintVector(ideals.AntiIdeal, label: "Worst Possible (A-)"); 
        MDebug.PrintIdealDistances(distances);
        MDebug.PrintVector(closenessFactors, label:"Closeness Factors (Results)");
        MDebug.PrintRanking(ranking);
        return ranking;
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

    public RankingResultDto MapCandidatesToResults(double[] closenessFactors, List<CandidateDto> candidates)
    {
        var result = new RankingResultDto(new List<CandidateResult>());
        
        if (closenessFactors.Length != candidates.Count)
        {
            throw new InvalidOperationException("Closeness factors count must equal candidate count");
        }
        
        for(int i = 0; i < candidates.Count; i++)
        {
            result.Rankings.Add(new CandidateResult(Candidate: candidates[i], RankingVal: closenessFactors[i]));
        }
        
        // TODO: check for identicals (lexical sorting) 
        
        var sorted = SortResults(result);
        return sorted;
    }

    public RankingResultDto SortResults(RankingResultDto ranking)
    {
        var res = ranking.Rankings.OrderByDescending(x => x.RankingVal).ToList();
        return new RankingResultDto(res);
    }
}