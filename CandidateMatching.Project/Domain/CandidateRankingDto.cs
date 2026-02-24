using System.ComponentModel.DataAnnotations;

namespace CandidateMatching.Domain;

// Criteria vals <int> should be replaced by custom type
public record CandidateRankingDto
{
    public Guid Id = System.Guid.NewGuid();
    public string? Name { get; set; } = String.Empty;
    public List<int>? CriteriaVals { get; set; } 
}