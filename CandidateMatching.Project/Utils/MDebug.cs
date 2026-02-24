using CandidateMatching.Domain;

namespace CandidateMatching.Utils;

public static class MDebug
{
    private const int DefaultPrecision = 3;
    public static void PrintMatrix(double[,] matrix, string? label = "", int? precision = null)
    {
        int precisionVal = precision ?? DefaultPrecision;
        
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

            Console.Write(")\n");
        }
    }
    
    public static void PrintVector(double[] vec, string? label = "", int? precision = null)
    {
        int precisionVal = precision ?? DefaultPrecision;

        if (label != String.Empty)
        {
            Console.WriteLine($"\n{label}:");
        }
        foreach(var a in vec)
        {
            Console.Write($"{a.ToString("F" + precisionVal)} | ");
        }
    }

    public static void PrintIdealDistances(IdealDistances[] vecPair, string? label = "", int? precision = null)
    {
        int precisionVal = precision ?? DefaultPrecision;
        string format = "F" + precisionVal; 
        
        if (label != String.Empty)
        {
            Console.WriteLine($"\n{label}:");
        }
        
        Console.WriteLine(); 

        foreach (var d in vecPair)
        {
            Console.WriteLine($"D+ ({d.IdealDistance.ToString(format)}) | D- ({d.AntiIdealDistance.ToString(format)})");    
        }

        Console.WriteLine("\n");
    }

    public static void PrintRanking(RankingResultDto ranking, string? label = "", int? precision = null)
    {
        int precisionVal = precision ?? DefaultPrecision;
        string format = "F" + precisionVal; 
        
        Console.WriteLine($"\n{label ?? "Final Ranking"}:");
        
        Console.WriteLine();

        for(int i = 0; i < ranking.Rankings.Count; i++)
        {
            var current = ranking.Rankings[i];
            Console.WriteLine($"{i+1}.: {current.Candidate.Name} - Score: {current.RankingVal.ToString(format)}");
        }
    }
}