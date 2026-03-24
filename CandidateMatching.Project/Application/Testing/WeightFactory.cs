using CandidateMatching.Lib;

namespace CandidateMatching.Application.Testing;

public static class WeightFactory
{
    private static readonly Random RndGen = new Random(Guid.NewGuid().GetHashCode());

    public static double[] CreateWeights(int? amount = null)
    {
        if (amount is null or 0)
        {
            return GetDefaultWeights();
        }
        
        double[] weights = new double[(int)amount];
        
        for (int i = 0; i < amount; i++)
        {
            var randomValue = (RndGen.Next() % 100) + 1;
            double divByHundred = randomValue / (double)100;
            weights[i] = divByHundred;
        }

        var normalized = Normalizer.NormalizeWeights(weights);
        MDebug.PrintWeights(normalized);
        
        if (!MHelpers.WeightsAddUptoOne(normalized))
        {
            throw new InvalidOperationException("Weights must add up to one");
        }

        return normalized;
    }
    
    public static double[] GetDefaultWeights()
    {
        return [0.3, 0.1, 0.1, 0.2, 0.3];
    }
}