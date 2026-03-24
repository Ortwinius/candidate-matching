using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using CandidateMatching.Application;
using CandidateMatching.Application.Ranking;

namespace CandidateMatching.Domain;

public enum RankingStrategy
{
    [EnumMember(Value="topsis")]
    Topsis,
    [EnumMember(Value="wsm")]
    Wsm
}

public sealed record CandidateDto
{
    public Guid Id { get; } = System.Guid.NewGuid();
    public string? Name { get; set; } = String.Empty;
    public required List<int> CriteriaVals { get; set; }  
}

public sealed record RankingRequestDto
{
    public required List<CandidateDto>? Candidates { get; set; }
    public required double[]? Weights{ get; set; }
    public required RankingStrategy Strategy { get; set; }
}

public sealed record TestRequestDto
{
    public required int Iterations { get; set; }
    public int? CandidateAmount { get; set; }
    public int? CriteriaAmount { get; set; } 
    public double[]? Weights { get; set; }
}

public sealed record TestResultDto
{ 
    public required int Iterations { get; set; }
    public required int CandidateAmount { get; set; }
    public required int CriteriaAmount { get; set; } 
    public required double[] Weights { get; set; }
    public required Dictionary<string, double> PairResults { get; set; }
    public required Dictionary<string, double> TopsisResults { get; set; }
    public required Dictionary<string, double> WsmResults { get; set; }
}

public sealed record Ideals(double[] Ideal, double[] AntiIdeal);

public sealed record IdealDistances(double IdealDistance, double AntiIdealDistance);

public sealed record CandidateResult(CandidateDto Candidate, double RankingVal);

public sealed record RankingResultDto(List<CandidateResult> Rankings);

public sealed record RankingResultsPair(RankingResultDto TopsisResult, RankingResultDto WsmResult);

public sealed record TestingContext(
    List<CandidateDto> Candidates,
    double[] Weights,
    RankingResultsPair Results
);

public sealed record PairMetric(
    string Key,
    Func<TestingContext, double> Calculate
);

public sealed record SingleMetric(
    string Key,
    Func<TestingContext, RankingResultDto, IRankingService, double> Calculate
);
