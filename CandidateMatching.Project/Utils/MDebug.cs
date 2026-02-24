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
}