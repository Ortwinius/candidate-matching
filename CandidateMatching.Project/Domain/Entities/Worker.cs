using System.ComponentModel.DataAnnotations;

namespace CandidateMatching.Domain.Entities;

public class Worker
{
    [Key] public int Id { get; }
}