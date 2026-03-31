// using CandidateMatching.Application.Testing;
// using CandidateMatching.Domain;
//
// namespace CandidateMatching.UnitTests.MetricTests;
//
// [TestFixture]
// public class SpearmanTests
// {
//     [Test]
//     public void SpearmanMetric_IdenticalOrderings_ReturnsOne()
//     {
//         var c1 = new CandidateDto { Name = "A", CriteriaVals = [1] };
//         var c2 = new CandidateDto { Name = "B", CriteriaVals = [1] };
//         var c3 = new CandidateDto { Name = "C", CriteriaVals = [1] };
//         var c4 = new CandidateDto { Name = "D", CriteriaVals = [1] };
//         var c5 = new CandidateDto { Name = "E", CriteriaVals = [1] };
//
//         var topsis = new RankingResultDto(new List<CandidateResult>
//         {
//             new(c1, 0.833),
//             new(c2, 0.738),
//             new(c3, 0.679),
//             new(c4, 0.620),
//             new(c5, 0.581),
//         });
//
//         var wsm = new RankingResultDto(new List<CandidateResult>
//         {
//             new(c1, 0.757),
//             new(c2, 0.601),
//             new(c3, 0.4987),
//             new(c4, 0.498),
//             new(c5, 0.460),
//         });
//
//         var pair = new RankingResultsPair(topsis, wsm);
//
//         var result = StatisticMetrics.SpearmanMetric(pair);
//
//         Assert.That(result, Is.EqualTo(1d));
//     }
//     
//     [Test]
//     public void SpearmanMetric_ReversedOrderings_ReturnsZeroBecauseCorrelationIsBelowThreshold()
//     {
//         var c1 = new CandidateDto { Name = "A", CriteriaVals = [1] };
//         var c2 = new CandidateDto { Name = "B", CriteriaVals = [1] };
//         var c3 = new CandidateDto { Name = "C", CriteriaVals = [1] };
//         var c4 = new CandidateDto { Name = "D", CriteriaVals = [1] };
//         var c5 = new CandidateDto { Name = "E", CriteriaVals = [1] };
//
//         var topsis = new RankingResultDto(new List<CandidateResult>
//         {
//             new(c1, 5),
//             new(c2, 4),
//             new(c3, 3),
//             new(c4, 2),
//             new(c5, 1),
//         });
//
//         var wsm = new RankingResultDto(new List<CandidateResult>
//         {
//             new(c5, 5),
//             new(c4, 4),
//             new(c3, 3),
//             new(c2, 2),
//             new(c1, 1),
//         });
//
//         var pair = new RankingResultsPair(topsis, wsm);
//
//         var result = StatisticMetrics.SpearmanMetric(pair);
//
//         Assert.That(result, Is.EqualTo(-1d));
//     }
// }