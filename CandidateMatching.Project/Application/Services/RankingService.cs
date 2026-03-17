using CandidateMatching.Domain;
using CandidateMatching.Lib;

namespace CandidateMatching.Application.Services;

public abstract class RankingService : IRankingService
{
    public abstract RankingResultDto PerformRanking(List<CandidateDto> candidates, double[] weights);

    public virtual double[,] GetNormalizedMatrix(double[,] decisionMatrix)
    {
        return decisionMatrix.ApplyLinearMaxNormalization();
    }

    public double[,] GetWeightedNormalizedMatrix(double[,] normalizedMatrix, double[] weights)
    {
        return normalizedMatrix.ApplyWeights(weights);
    }
    
    protected RankingResultDto MapCandidatesToResults(double[] performances, List<CandidateDto> candidates)
    {
        var result = new RankingResultDto(new List<CandidateResult>());
        
        if (performances.Length != candidates.Count)
        {
            throw new InvalidOperationException("Closeness factors count must equal candidate count");
        }
        
        for(int i = 0; i < candidates.Count; i++)
        {
            result.Rankings.Add(new CandidateResult(Candidate: candidates[i], RankingVal: performances[i]));
        }
        
        // TODO: check for identicals (lexical sorting) 
        
        var sorted = MHelpers.SortResults(result);
        return sorted;
    }
}