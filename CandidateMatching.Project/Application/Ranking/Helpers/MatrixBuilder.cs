using CandidateMatching.Domain;

namespace CandidateMatching.Application.Ranking.Helpers;

public class MatrixBuilder()
{
    private int Columns { get; set; } = 0;
    private readonly List<List<double>> _matrixSkeleton = [];

    public void AddRows(List<CandidateDto> candidates)
    {
        foreach (var c in candidates)
        {
            AddRow(c);
        }
    }
    public void AddRow(CandidateDto candidate)
    {
        if (Columns == 0) Columns = candidate.CriteriaVals.Count;
        
        if (candidate.CriteriaVals.Count != Columns)
        {
            Console.WriteLine($"Error: Candidate criteria amount does not match Matrix columns");
            return;
        }
        _matrixSkeleton.Add(candidate.CriteriaVals);
    }
    
    // outputs a 2-dim matrix with m rows and n Cols
    public double[,] Build()
    {
        var rows = _matrixSkeleton.Count;
        var matrix = new double[rows, Columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                matrix[i, j] = _matrixSkeleton[i][j];
            }
        }

        return matrix;
    }
}