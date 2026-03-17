// using CandidateMatching.Application.Ranking;
// using CandidateMatching.Test.Helpers;
// using Microsoft.Extensions.Logging.Abstractions;
//
// namespace CandidateMatching.Test.Comparison;
//
// public class HitRatioCompTests 
// {
//     private TopsisRankingService _topsisService;
//     private WsmRankingService _wsmService;
//     
//     [SetUp]
//     public void Setup()
//     {
//         var topsisLogger = new NullLogger<TopsisRankingService>();
//         var wsmLogger = new NullLogger<WsmRankingService>();
//         _topsisService = new TopsisRankingService(topsisLogger);
//         _wsmService = new WsmRankingService(wsmLogger);
//     }
//
//     [Test]
//     public void GenerateHitRatioCompStatistics()
//     {
//         int iterations = 100000;
//         int candidateAmount = 5;
//             
//         var comp = new AlgoComparator<TopsisRankingService, WsmRankingService>(_topsisService, _wsmService);
//         comp.RunBenchmark(Metrics.HitRatioMetric, iterations, candidateAmount: candidateAmount);
//     }
// }