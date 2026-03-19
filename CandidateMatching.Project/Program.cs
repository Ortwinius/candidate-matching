using CandidateMatching.Application.Benchmark;
using CandidateMatching.Application.Ranking;
using CandidateMatching.Domain;
using Microsoft.Extensions.Logging.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddScoped<IRankingService, TopsisRankingService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// var candidates = new List<CandidateDto>
// {
//     new() { Name = "Elena", CriteriaVals = [15, 40, 25, 40] },
//     new() { Name = "Marcus", CriteriaVals = [20, 30, 20, 35] },
//     new() { Name = "Sasha", CriteriaVals = [30, 10, 30, 15] }
// };
//
// var weights = new double[4] { 0.3, 0.1, 0.4, 0.2 };

// var correctNormalizedMatrix = new double[3, 4]
// {
//     { 0.384, 0.784, 0.570, 0.724 },
//     { 0.512, 0.588, 0.456, 0.634 },
//     { 0.768, 0.196, 0.684, 0.272 }
// };
// var correctNormalizedWeightedMatrix = new double[3, 4]
// {
//     { 0.115, 0.078, 0.228, 0.145 },
//     { 0.154, 0.059, 0.182, 0.127 },
//     { 0.230, 0.020, 0.274, 0.054 }
// };
// var correctIdealSolution = new double[] { 0.230, 0.078, 0.274, 0.145 };
// var correctNegativeIdealSolution = new double[] { 0.115, 0.020, 0.182, 0.054 };
// var correctRelativeClosenessToIdealSolution = new double[] { 0.486, 0.427, 0.576 };
// var correctIdealSolutionSeparation = new double[] { 0.124, 0.122, 0.108 };
// var correctNegativeIdealSolutionSeparation = new double[] { 0.117, 0.091, 0.147 };

var candidates = new List<CandidateDto>
{
    new() { Name = "Bob", CriteriaVals = [35, 90, 80, 40] },
    new() { Name = "Anna", CriteriaVals = [90, 15, 75, 30] },
    new() { Name = "Karl", CriteriaVals = [85, 10, 95, 70] },
    new() { Name = "Johanna", CriteriaVals = [95, 70, 45, 80] },
    new() { Name = "Mohammed", CriteriaVals = [10, 90, 70, 85] },
};

var weights = new double[] { 0.3, 0.2, 0.2, 0.3 };  

using var scope = app.Services.CreateScope();
// var rankingService = scope.ServiceProvider.GetRequiredService<IRankingService>();

var topsisLogger = new NullLogger<TopsisRankingService>();
var wsmLogger = new NullLogger<WsmRankingService>();
var topsisRankingService = new TopsisRankingService(topsisLogger);
var wsmRankingService = new WsmRankingService(wsmLogger);

var benchmark = new MetricTestRunner<TopsisRankingService, WsmRankingService>(topsisRankingService, wsmRankingService);

benchmark.RunBenchmark(iterations: 100000, candidateAmount: 25);

// var ranking = rankingService.PerformRanking(candidates, weights);


app.Run();