using CandidateMatching.Domain;

namespace CandidateMatching.Utils;

// TODO: add logger
public class CandidateMatrixBuilder()
{
    private int _cols { get; set; } = 0;
    private readonly List<List<int>> _matrixSkeleton = new List<List<int>>();

    public void AddRows(List<CandidateDto> candidates)
    {
        foreach (var c in candidates)
        {
            AddRow(c);
        }
    }
    public void AddRow(CandidateDto candidate)
    {
        if (_cols == 0) _cols = candidate.CriteriaVals.Count;
        
        if (candidate.CriteriaVals?.Count != _cols)
        {
            Console.WriteLine($"Error: Candidate criteria amount does not match Matrix columns");
            return;
        }
        _matrixSkeleton.Add(candidate.CriteriaVals);
    }
    
    // outputs a 2-dim matrix with m rows and n cols
    public double[,] Build()
    {
        var rows = _matrixSkeleton.Count;
        var matrix = new double[rows, _cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < _cols; j++)
            {
                matrix[i, j] = _matrixSkeleton[i][j];
            }
        }

        return matrix;
    }
}