using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mandara.Business.Client.Extensions;
using Mandara.Business.Extensions;
using Mandara.Entities.Calculation;
using Ninject.Extensions.Logging;

namespace Mandara.Business.Model.Positions
{
    public static class CoefficientsCalculator
    {
        public static bool AssertCoefficientErrors = false;
        private static readonly ILogger Logger = new NLogLoggerFactory().GetCurrentClassLogger();

        public static Dictionary<CoeffEntityId, decimal> AddCoefficients(
            Dictionary<CoeffEntityId, decimal> existingCoefficients,
            CoeffEntityId newPosEntity,
            decimal newPosAmount,
            decimal currentAbsAmount,
            decimal oldAbsAmount)
        {
            decimal newAbsAmount = currentAbsAmount.IsInRange(0M, 0.00001M) ? 0M : currentAbsAmount;
            decimal newDetailCoeff = 0M;

            if (newAbsAmount > 0M)
            {
                foreach (var pair in existingCoefficients.ToList())
                {
                    decimal newCoeff = (pair.Value * oldAbsAmount) / newAbsAmount;

                    existingCoefficients.Remove(pair.Key);
                    existingCoefficients.Add(pair.Key, newCoeff);
                }

                newDetailCoeff = newPosAmount / newAbsAmount;
            }
            else
            {
                int breakHere = 1;
            }

            decimal coeff;
            if (existingCoefficients.TryGetValue(newPosEntity, out coeff))
            {
                decimal signFromCoeff = Math.Abs(coeff) > Math.Abs(newDetailCoeff) ? coeff : newDetailCoeff;
                decimal sign = signFromCoeff < 0M ? -1M : 1M;

                coeff = sign * (Math.Abs(coeff) + Math.Abs(newDetailCoeff));
                existingCoefficients.Remove(newPosEntity);
            }
            else
            {
                coeff = newDetailCoeff;
            }

            existingCoefficients.Add(newPosEntity, coeff);
#if DEBUG
            // check that sum is still equal to 1.0M
            decimal sum = SumOfCoefficients(existingCoefficients.Values);

            if (!sum.IsInRange(1.0M, 0.0001M))
            {
                Logger.Error(
                    "Coefficients no longer sum to 1 for [{0}]",
                    String.Join(", ", StringifiedPairs(existingCoefficients)));
            }

            if (AssertCoefficientErrors)
            {
                Debug.Assert(0.9999M < sum && sum < 1.0001M, "Position coefficients sum not equals to 1");
            }
#endif
            return existingCoefficients;
        }

        private static decimal SumOfCoefficients(IEnumerable<decimal> coefficients)
        {
            return coefficients.Sum(Math.Abs);
        }

        private static IEnumerable<string> StringifiedPairs<TKey, TVal>(IEnumerable<KeyValuePair<TKey, TVal>> pairs)
        {
            return pairs.Select(pair => pair.Stringify());
        }

        public static Dictionary<CoeffEntityId, decimal> RemoveCoefficients(
            Dictionary<CoeffEntityId, decimal> existingCoefficients,
            CoeffEntityId newEntity,
            decimal amountToRemove,
            decimal currentAbsAmount,
            decimal oldAbsAmount)
        {
            decimal newAbsAmount = currentAbsAmount.IsInRange(0M, 0.00001M) ? 0M : currentAbsAmount;
            decimal newDetailCoeff = 0M;

            if (newAbsAmount > 0M)
            {
                foreach (var pair in existingCoefficients.ToList())
                {
                    decimal newCoeff = (pair.Value * oldAbsAmount) / newAbsAmount;

                    existingCoefficients.Remove(pair.Key);
                    existingCoefficients.Add(pair.Key, newCoeff);
                }

                newDetailCoeff = amountToRemove / newAbsAmount;
            }

            decimal coeff;
            if (existingCoefficients.TryGetValue(newEntity, out coeff))
            {
                decimal signFromCoeff = Math.Abs(coeff) > Math.Abs(newDetailCoeff) ? coeff : newDetailCoeff;
                decimal sign = signFromCoeff < 0M ? -1M : 1M;

                coeff = sign * (Math.Abs(coeff) - Math.Abs(newDetailCoeff));
                existingCoefficients.Remove(newEntity);

                if (coeff != 0M)
                {
                    existingCoefficients.Add(newEntity, coeff);
                }
            }

#if DEBUG
            // check that sum is still equal to 1.0M
            if (existingCoefficients.Any())
            {
                decimal sum = SumOfCoefficients(existingCoefficients.Values);

                if (!sum.IsInRange(1.0M, 0.0001M))
                {
                    Logger.Error(
                        "Coefficients no longer sum to 1 for [{0}]",
                        String.Join(", ", StringifiedPairs(existingCoefficients)));
                }

                if (AssertCoefficientErrors)
                {
                    Debug.Assert(0.9999M < sum && sum < 1.0001M, "Position coefficients sum not equals to 1");
                }
            }
#endif
            return existingCoefficients;
        }
    }

    public static class KeyValuePairExtensions
    {
        public static string Stringify<TKey, TVal>(this KeyValuePair<TKey, TVal> pair)
        {
            return $"{{{pair.Key} -> {pair.Value}}}";
        }
    }
}
