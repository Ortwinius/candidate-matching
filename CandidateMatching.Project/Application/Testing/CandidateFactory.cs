using CandidateMatching.Domain;

namespace CandidateMatching.Application.Testing;

public static class CandidateFactory
{
    private static readonly Random RndGen = new Random(Guid.NewGuid().GetHashCode());
    
    public static CandidateDto CreateCandidate(int criteriaAmount)
    {
        var criteriaVals = new List<int>();
        for (int i = 0; i < criteriaAmount; i++)
        {
            var randomValue = (RndGen.Next() % MConstants.CriteriaValueRange) + 1;
            criteriaVals.Add(randomValue);
        }
        var name = GenerateUniqueName();

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
    
    public static string GenerateUniqueName()
    {
        var firstName = FirstNames[RndGen.Next(FirstNames.Length)];
        var lastName = LastNames[RndGen.Next(LastNames.Length)];
        var code = RndGen.Next(100, 1000); 
        
        return $"{firstName}{lastName}{code}";
    }
    
    private static readonly string[] FirstNames =
    [
        "James", "Mary", "Robert", "Patricia", "John", "Jennifer", "Michael", "Linda", 
        "David", "Elizabeth", "William", "Barbara", "Richard", "Susan", "Joseph", "Jessica", 
        "Thomas", "Sarah", "Christopher", "Karen", "Charles", "Lisa", "Daniel", "Nancy", 
        "Matthew", "Betty", "Anthony", "Sandra", "Mark", "Ashley", "Donald", "Dorothy", 
        "Steven", "Kimberly", "Andrew", "Emily", "Paul", "Donna", "Joshua", "Michelle", 
        "Kenneth", "Carol", "Kevin", "Amanda", "Brian", "Melissa", "George", "Deborah"
    ];

    private static readonly string[] LastNames =
    [
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", 
        "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson"
    ];
}