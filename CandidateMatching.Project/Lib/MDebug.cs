using CandidateMatching.Application;
using CandidateMatching.Domain;

namespace CandidateMatching.Lib;

// TODO: add optional ranking result name so that in hindsight it can be traced back?
public static class MDebug
{

    public static void PrintMatrix(double[,] matrix, string? label = null, int? precision = null, List<CandidateDto>? candidates = null)
    {
        int precisionVal = precision ?? MConstants.DefaultPrintingPrecision;
        
        if (!String.IsNullOrEmpty(label))
        {
            Console.WriteLine($"\n{label}:");
        }
        
        int m = matrix.GetLength(0);
        int n = matrix.GetLength(1);
        
        for (int i = 0; i < m; i++)
        {
            Console.Write("( ");
            for (int j = 0; j < n; j++)
            {
                Console.Write($"{matrix[i,j].ToString("F" + precisionVal)} ");
            }

            Console.Write(")");
            Console.Write(candidates != null ? $" => {candidates[i].Name}" : String.Empty);
            Console.Write("\n");
        }
    }
    
    public static void PrintVector(double[] vec, string? label = null, int? precision = null)
    {
        int precisionVal = precision ?? MConstants.DefaultPrintingPrecision;

        if (label != String.Empty)
        {
            Console.WriteLine($"\n{label}:");
        }
        foreach(var a in vec)
        {
            Console.Write($"{a.ToString("F" + precisionVal)} | ");
        }
    }

    public static void PrintIdealDistances(IdealDistances[] distances, string? label = null, int? precision = null, List<CandidateDto>? candidates = null)
    {
        int precisionVal = precision ?? MConstants.DefaultPrintingPrecision;
        string format = "F" + precisionVal; 
        
        Console.WriteLine($"\n\n{label ?? "Ideal Distances"}:");

        for(int i = 0; i < distances.Length; i++)
        {
            Console.Write($"D+ ({distances[i].IdealDistance.ToString(format)}) | D- ({distances[i].AntiIdealDistance.ToString(format)})");
            Console.Write(candidates != null ? $" => {candidates[i].Name}\n" : "\n");
        }
    }

    public static void PrintRanking(RankingResultDto ranking, string? label = null, int? precision = null)
    {
        int precisionVal = precision ?? MConstants.DefaultPrintingPrecision;
        string format = "F" + precisionVal; 
        
        Console.WriteLine($"\n\n{label ?? "Final Ranking"}:");

        for(int i = 0; i < ranking.Rankings.Count; i++)
        {
            var current = ranking.Rankings[i];
            Console.WriteLine($"{i+1}.: {current.Candidate.Name} - Score: {current.RankingVal.ToString(format)}");
        }
    }

    // TODO: also print weight name after Refactor of weights
    public static void PrintWeights(double[] weights)
    {
        Console.WriteLine("[ ");
        foreach (var weight in weights)
        {
            Console.WriteLine(weight + " ");
        }

        Console.WriteLine("]");
    }

    // public static void PrintCandidateNames(CandidateDto candidates)
    // {
    //     Console.WriteLine("Candidates: ");
    //      
    // }
}