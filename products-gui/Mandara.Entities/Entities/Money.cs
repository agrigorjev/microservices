using System;

namespace Mandara.Entities
{
    [Serializable]
    public struct Money : IEquatable<Money>
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public const decimal InvalidAmount = Decimal.MinValue;
        public const decimal DefaultAmount = 0M;
        public const string DefaultCurrency = "DefaultMoney";

        public static Money Default = new Money(DefaultAmount, DefaultCurrency);

        public static Money CurrencyDefault(string currencyName)
        {
            return new Money(DefaultAmount, currencyName);
        }

        public static Money CurrencyInvalid(string currency)
        {
            return new Money(InvalidAmount, currency);
        }

        public Money(decimal amount, string currency) : this()
        {
            Amount = amount;
            Currency = currency;
        }

        public bool IsDefault()
        {
            return Amount == Money.DefaultAmount && Currency == DefaultCurrency;
        }

        public bool IsInvalid()
        {
            return Money.InvalidAmount == Amount;
        }

        public bool IsZero()
        {
            return Amount == Money.DefaultAmount;
        }

        public bool Equals(Money other)
        {
            return (Amount == other.Amount && Currency == other.Currency);
        }

        public static bool operator ==(Money lhs, Money rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Money lhs, Money rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Money && Equals((Money) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Amount.GetHashCode() * 397) ^ (Currency != null ? Currency.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return ToString("N2");
        }

        public string ToString(string numberFormat)
        {
            return string.Format("{0} {1}", Currency, Amount.ToString(numberFormat));
        }

        public static Money operator *(Money left, decimal right)
        {
            left.Amount *= right;

            return left;
        }

        public static Money operator /(Money left, decimal right)
        {
            left.Amount /= right;

            return left;
        }

        public static Money operator +(Money left, decimal right)
        {
            left.Amount += right;

            return left;
        }

        public static Money operator +(Money left, decimal? right)
        {
            if (right.HasValue)
            {
                left.Amount += right.Value;
            }

            return left;
        }

        public static Money operator -(Money left, decimal right)
        {
            left.Amount -= right;

            return left;
        }

        public static Money operator *(Money left, Money right)
        {
            throw new InvalidOperationException("Money: Cannot multiply two instances.");
        }

        public static Money operator /(Money left, Money right)
        {
            throw new InvalidOperationException("Money: Cannot divide two instances.");
        }

        public static Money operator +(Money left, Money right)
        {
            if (right.IsDefault())
            {
                return left;
            }

            if (left.IsDefault())
            {
                return right;
            }

            EnsureSameCurrency(left, right);
            left.Amount += right.Amount;
            return left;
        }

        public static Money operator -(Money left, Money right)
        {
            if (right.IsDefault())
            {
                return left;
            }

            if (left.IsDefault())
            {
                return right;
            }

            EnsureSameCurrency(left, right);
            left.Amount -= right.Amount;
            return left;
        }

        public static Money operator +(Money left, Money? right)
        {
            if (right.HasValue)
            {
                return left + right.Value;
            }

            return left;
        }

        private static void EnsureSameCurrency(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new ArithmeticException(
                    String.Format(
                        "Money: The currency of both arguments must match to perform this operation, but the "
                            + "currencies were '{0}' and '{1}'.",
                        left.Currency,
                        right.Currency));
        }

        public static implicit operator decimal(Money value)
        {
            return value.Amount;
        }
    }
}