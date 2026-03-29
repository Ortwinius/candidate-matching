using CandidateMatching.Domain;
using CandidateMatching.Lib;

namespace CandidateMatching.Application.Ranking;

public class WsmRankingService(ILogger<WsmRankingService> logger) : RankingService
{
    public override RankingStrategy StrategyKey { get; } = RankingStrategy.Wsm;
    
    public override RankingResultDto PerformRanking(List<CandidateDto> candidates, double[] weights)
    {
        AssertValidInput(candidates, weights);
        
        logger.Log(LogLevel.Information, "Starting WSM ranking process");
        
        var matrixBuilder = new MatrixBuilder();
        matrixBuilder.AddRows(candidates);
        var matrix = matrixBuilder.Build();
            
        var normalized = GetNormalizedMatrix(matrix);
        var weightedNormalized = GetWeightedNormalizedMatrix(normalized, weights);

        var performances = GetWsmPerformances(weightedNormalized);
        var ranking = MapCandidatesToResults(performances, candidates);
        
        // MDebug.PrintMatrix(normalized, label: "Normalized (without weights)", candidates: candidates);
        // MDebug.PrintMatrix(weightedNormalized, label: "Weighted Normalized", candidates: candidates);
        // MDebug.PrintVector(performances, label:"Performances (Results)");
        // MDebug.PrintRanking(ranking);
        
        return ranking;
    }

    public double[] GetWsmPerformances(double[,] matrix)
    {
        int m = matrix.GetLength(0);
        int n = matrix.GetLength(1);

        var performances = new double[m];
        
        for (int i = 0; i < m; i++)
        {
            performances[i] = 0;
            for (int j = 0; j < n; j++)
            {
                performances[i] += matrix[i, j];
            }
        }

        return performances;
    }
}