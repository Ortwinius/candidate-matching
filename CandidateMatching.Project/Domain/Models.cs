using System.ComponentModel.DataAnnotations;
using CandidateMatching.Application.Benchmark;
using CandidateMatching.Application.Ranking;

namespace CandidateMatching.Domain;

// Criteria vals <int> should be replaced by custom type
public record CandidateDto
{
    public Guid Id = System.Guid.NewGuid();
    public string? Name { get; set; } = String.Empty;
    public required List<int> CriteriaVals { get; set; } 
}

public sealed record Ideals(double[] Ideal, double[] AntiIdeal);

public sealed record IdealDistances(double IdealDistance, double AntiIdealDistance);

public sealed record CandidateResult(CandidateDto Candidate, double RankingVal);

public sealed record RankingResultDto(List<CandidateResult> Rankings);

public sealed record RankingResultsPair(RankingResultDto TopsisResult, RankingResultDto WsmResult);

public sealed record BenchmarkContext(
    List<CandidateDto> Candidates,
    double[] Weights,
    RankingResultsPair Results
    // TopsisRankingService? RankingService,
    // WsmRankingService? AlternativeRankingService
);

public sealed record Criteria(
    double Competence,
    double SoftAvailability,
    double Experience,
    double DistanceWithinRegion,
    double KnowsCompany);

public sealed record PairMetric(
    string Key,
    Func<BenchmarkContext, double> Calculate
);

public sealed record SingleMetric(
    string Key,
    Func<BenchmarkContext, RankingResultDto, IRankingService, double> Calculate
);
