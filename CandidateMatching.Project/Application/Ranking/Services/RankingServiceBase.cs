using CandidateMatching.Application.Ranking.Helpers;
using CandidateMatching.Domain;
using CandidateMatching.Domain.Ranking;
using CandidateMatching.Lib;

namespace CandidateMatching.Application.Ranking.Services;

    public abstract class RankingServiceBase : IRankingService
    {
        public abstract RankingStrategy StrategyKey { get; }
        
        public abstract RankingResultDto PerformRanking(List<CandidateDto> candidates, double[] weights);
        
        public virtual double[,] GetNormalizedMatrix(double[,] decisionMatrix)
        {
            return decisionMatrix.ApplyLinearMaxNormalization();
        }

        public double[,] GetWeightedNormalizedMatrix(double[,] normalizedMatrix, double[] weights)
        {
            return normalizedMatrix.ApplyWeights(weights);
        }

        protected void AssertValidInput(List<CandidateDto> candidates, double[] weights)
        {
            if (candidates == null || candidates.Count == 0)
                throw new ArgumentException("Candidates must not be empty.", nameof(candidates));

            if (candidates[0].CriteriaVals.Count != weights.Length)
            {
                throw new ArgumentException("Amount of criteria must match weights");
            }

            if (!MHelpers.WeightsAddUptoOne(weights))
            {
                throw new InvalidOperationException("Sum of weights must equal 1");
            }
        }
        
        protected RankingResultDto MapCandidatesToResults(double[] performances, List<CandidateDto> candidates)
        {
            var result = new RankingResultDto
            {
                Rankings = new List<CandidateResult>()
            };
            
            if (performances.Length != candidates.Count)
            {
                throw new InvalidOperationException("Closeness factors count must equal candidate count");
            }
            
            for(int i = 0; i < candidates.Count; i++)
            {
                result.Rankings.Add(new CandidateResult(Candidate: candidates[i], RankingVal: performances[i]));
            }
            
            var sorted = MHelpers.SortResultsByPerformance(result);
            
            return sorted;
        }
    }