using CandidateMatching.Domain;

namespace CandidateMatching.Application.Testing.Factories;

public static class CandidateFactory
{
    public static CandidateDto CreateCandidate(int criteriaAmount)
    {
        
        var criteriaVals = new List<int>();
        for (int i = 0; i < criteriaAmount; i++)
        {
            var randomValue = Random.Shared.Next(1, MConstants.CriteriaValueRange + 1);
            criteriaVals.Add(randomValue);
        }
        var name = GenerateName();

        return new CandidateDto{Name = name, CriteriaVals = criteriaVals};
    }
    
    public static List<CandidateDto> CreateCandidateList(int candidateAmount, int? criteriaAmount = null)
    {
        int criteriaCount = criteriaAmount ?? MConstants.DefaultCriteriaAmount;
        
        var candidates = new List<CandidateDto>();
        for (int i = 0; i < candidateAmount; i++)
        {
            candidates.Add(CreateCandidate(criteriaCount));
        }

        return candidates;
    }
    
    private static string GenerateName()
    {
        var name = $"{Names[Random.Shared.Next(Names.Length)]}";
        return name;
    }
    
    // List of names so that you dont have to write them yourself
    // (obviously, ID must still be used tho for unique identifier)
    private static readonly string[] Names =
    [
        "James", "Mary", "Robert", "Patricia", "John", "Jennifer", "Michael", "Linda", 
        "David", "Elizabeth", "William", "Barbara", "Richard", "Susan", "Joseph", "Jessica", 
        "Thomas", "Sarah", "Christopher", "Karen", "Charles", "Lisa", "Daniel", "Nancy", 
        "Matthew", "Betty", "Anthony", "Sandra", "Mark", "Ashley", "Donald", "Dorothy", 
        "Steven", "Kimberly", "Andrew", "Emily", "Paul", "Donna", "Joshua", "Michelle", 
        "Kenneth", "Carol", "Kevin", "Amanda", "Brian", "Melissa", "George", "Deborah"
    ];
}