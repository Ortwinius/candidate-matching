using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.WebSockets;
using System.Reflection.Emit;
using CandidateMatching.Domain;
using CandidateMatching.Lib;

namespace CandidateMatching.Services;

// Topsis Implementation of Ranking
public class TopsisRankingService(ILogger<TopsisRankingService> logger): RankingService
{
    public override RankingResultDto PerformRanking(List<CandidateDto> candidates, double[] weights)
    {
        logger.Log(LogLevel.Information, "Starting TOPSIS ranking process");

        if (candidates[0].CriteriaVals.Count != weights.Length)
        {
            throw new ArgumentException("Amount of criteria must match weights");
        }

        if (!MHelpers.WeightsAddUptoOne(weights))
        {
            throw new InvalidOperationException("Sum of weights must equal 1");
        }
        
        var matrixBuilder = new MMatrixBuilder();
        matrixBuilder.AddRows(candidates);
        var matrix = matrixBuilder.Build();
            
        var normalized = GetNormalizedMatrix(matrix);
        var weightedNormalized = GetWeightedNormalizedMatrix(normalized, weights);
        var ideals = GetIdealSolutions(weightedNormalized);
        var distances = GetDistancesToIdealSolutions(weightedNormalized, ideals);
        var closenessFactors = GetTopsisPerformances(distances);
        var ranking = MapCandidatesToResults(closenessFactors, candidates);
        
        // MDebug.PrintMatrix(normalized, label: "Normalized (without weights)", candidates: candidates);
        // MDebug.PrintMatrix(weightedNormalized, label: "Weighted Normalized", candidates: candidates);
        // MDebug.PrintVector(ideals.Ideal, label: "Best Possible (A*)");
        // MDebug.PrintVector(ideals.AntiIdeal, label: "Worst Possible (A-)"); 
        // MDebug.PrintIdealDistances(distances, candidates: candidates);
        // MDebug.PrintVector(closenessFactors, label:"Closeness Factors (Results)");
        // MDebug.PrintRanking(ranking);
        
        return ranking;
    }
    
    public override double[,] GetNormalizedMatrix(double[,] decisionMatrix)
    {
        return decisionMatrix.ApplyVectorNormalization();
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
    
    // calculated as closeness factors (relative distance)
    public double[] GetTopsisPerformances(IdealDistances[] distances)
    {
        var performances = new double [distances.Length];
        for (int i = 0; i < distances.Length; i++)
        {
            performances[i] = distances[i].AntiIdealDistance / (distances[i].AntiIdealDistance + distances[i].IdealDistance);
        }

        return performances;
    }
}