using System.ComponentModel.DataAnnotations;

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