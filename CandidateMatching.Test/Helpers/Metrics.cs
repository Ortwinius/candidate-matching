namespace CandidateMatching.Test.Helpers;

public static class Metrics
{
    public static int HitRatioMetric(RankingResultsPair data)
    {
        var topsisTop1 = data.TopsisResult.Rankings.First().Candidate;
        var wsmTop1 = data.WsmResult.Rankings.First().Candidate;

        if (topsisTop1.Name == wsmTop1.Name)
        {
            // Console.WriteLine($"Incident: {topsisTop1.Name}: TOPSIS ({data.TopsisResult.Rankings.First().RankingVal}) = WSM ({data.WsmResult.Rankings.First().RankingVal})");
            return 1;
        }

        return 0;
    }
    // public static int TiebreakerMetric(RankingResultsPair data)
    // {
    //     int candidateCount = data.TopsisResult.Rankings.Count;
    //     
    //     for (int j = 1; j < candidateCount; j++)
    //     {
    //         var prev = roundedRanking[j - 1];
    //         var current = roundedRanking[j];
    //
    //         if (Math.Abs(prev - current) < 1e-9)
    //         {
    //             tiebreakerCount++;
    //             // Console.WriteLine($"[!] Tie Breaker in Iteration {i + 1}:");
    //             // Console.WriteLine($"    Affected No1: {ranking.Rankings[j - 1].Candidate.Name} ({ranking.Rankings[j - 1].RankingVal:F4})");
    //             // Console.WriteLine($"    Affected No2:    {ranking.Rankings[j].Candidate.Name} ({ranking.Rankings[j].RankingVal:F4})");
    //         }
    //     }
    //     var topsisTop1 = data.TopsisResult.Rankings.First().Candidate;
    //     var wsmTop1 = data.WsmResult.Rankings.First().Candidate;
    //
    //     if (topsisTop1.Name == wsmTop1.Name)
    //     {
    //         // Console.WriteLine($"Incident: {topsisTop1.Name}: TOPSIS ({data.TopsisResult.Rankings.First().RankingVal}) = WSM ({data.WsmResult.Rankings.First().RankingVal})");
    //         return 1;
    //     }
    //
    //     return 0;
    // }
}