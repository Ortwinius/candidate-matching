using System.Text.Json.Serialization;
using CandidateMatching.Application.Ranking.Context;
using CandidateMatching.Application.Ranking.Services;
using CandidateMatching.Application.Testing.Services;
using CandidateMatching.Domain;
using CandidateMatching.Domain.Ranking;
using CandidateMatching.Domain.Testing;
using CandidateMatching.Lib;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IRankingService, TopsisRankingServiceBase>();
builder.Services.AddScoped<IRankingService, WsmRankingServiceBase>();
builder.Services.AddScoped<IRankingContext, RankingContext>();

builder.Services.AddScoped<ITestService, TopsisWsmTestService>();
    
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

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

using var scope = app.Services.CreateScope();

// var context = scope.ServiceProvider.GetRequiredService<IRankingContext>();
// var strategy = context.Resolve(RankingStrategy.Wsm);
// var result = strategy.PerformRanking(candidates, weights);
// MDebug.PrintRanking(result, precision: 5);

app.Run();