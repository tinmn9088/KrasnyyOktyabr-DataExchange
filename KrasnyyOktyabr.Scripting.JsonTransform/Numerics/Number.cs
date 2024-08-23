using System.Globalization;

namespace KrasnyyOktyabr.JsonTransform.Numerics;

/// <remarks>
/// No symmetry when checking equivalence with <see cref="int"/> or <see cref="double"/>.
/// </remarks>
public readonly struct Number : IEquatable<Number>, IComparable<Number>
{
    public decimal? Decimal
    {
        get;
    }

    public long? Long
    {
        get;
    }

    /// <summary>
    /// Int 0.
    /// </summary>
    public Number()
    {
        Long = 0;
    }

    public Number(decimal value)
    {
        Decimal = value;
    }

    public Number(long value)
    {
        Long = value;
    }

    public static bool TryParse(string? value, out Number number)
    {
        // Long
        if (long.TryParse(value, out long parseLongResult))
        {
            number = new Number(parseLongResult);

            return true;
        }

        // Decimal
        if (decimal.TryParse(value,
            style: NumberStyles.Any,
            provider: CultureInfo.InvariantCulture,
            out decimal parseDoubleResult))
        {
            number = new Number(parseDoubleResult);

            return true;
        }

        // Failed to parse
        number = new Number(0);

        return false;
    }

    public static Number operator +(Number a, Number b)
    {
        if (a.Long is not null && b.Long is not null)
        {
            return new Number(a.Long.Value + b.Long.Value);
        }

        if (a.Long is not null && b.Decimal is not null)
        {
            return new Number(Convert.ToDecimal(a.Long) + b.Decimal.Value);
        }

        if (a.Decimal is not null && b.Long is not null)
        {
            return new Number(a.Decimal.Value + Convert.ToDecimal(b.Long));
        }

        if (a.Decimal is not null && b.Decimal is not null)
        {
            return new Number(a.Decimal.Value + b.Decimal.Value);
        }

        throw new NotImplementedException();
    }

    public static Number operator -(Number a, Number b)
    {
        if (a.Long is not null && b.Long is not null)
        {
            return new Number(a.Long.Value - b.Long.Value);
        }

        if (a.Long is not null && b.Decimal is not null)
        {
            return new Number(Convert.ToDecimal(a.Long) - b.Decimal.Value);
        }

        if (a.Decimal is not null && b.Long is not null)
        {
            return new Number(a.Decimal.Value - Convert.ToDecimal(b.Long));
        }

        if (a.Decimal is not null && b.Decimal is not null)
        {
            return new Number(a.Decimal.Value - b.Decimal.Value);
        }

        throw new NotImplementedException();
    }

    public static Number operator *(Number a, Number b)
    {
        if (a.Long is not null && b.Long is not null)
        {
            return new Number(a.Long.Value * b.Long.Value);
        }

        if (a.Long is not null && b.Decimal is not null)
        {
            return new Number(Convert.ToDecimal(a.Long) * b.Decimal.Value);
        }

        if (a.Decimal is not null && b.Long is not null)
        {
            return new Number(a.Decimal.Value * Convert.ToDecimal(b.Long));
        }

        if (a.Decimal is not null && b.Decimal is not null)
        {
            return new Number(a.Decimal.Value * b.Decimal.Value);
        }

        throw new NotImplementedException();
    }

    public static Number operator /(Number a, Number b)
    {
        if (a.Long is not null && b.Long is not null)
        {
            if (b.Long == 0)
            {
                throw new DivideByZeroException();
            }

            if (a.Long % b.Long == 0)
            {
                return new Number(a.Long.Value / b.Long.Value);
            }
            else
            {
                return new Number(Convert.ToDecimal(a.Long) / Convert.ToDecimal(b.Long));
            }
        }

        if (a.Long is not null && b.Decimal is not null)
        {
            if (b.Decimal == 0)
            {
                throw new DivideByZeroException();
            }

            return new Number(Convert.ToDecimal(a.Long) / b.Decimal.Value);
        }

        if (a.Decimal is not null && b.Long is not null)
        {
            if (b.Long == 0)
            {
                throw new DivideByZeroException();
            }

            return new Number(a.Decimal.Value / Convert.ToDecimal(b.Long));
        }

        if (a.Decimal is not null && b.Decimal is not null)
        {
            if (b.Decimal == 0)
            {
                throw new DivideByZeroException();
            }

            return new Number(a.Decimal.Value / b.Decimal.Value);
        }

        throw new NotImplementedException();
    }

    public static bool operator ==(Number a, Number b) => a.Equals(b);

    public static bool operator !=(Number a, Number b) => !a.Equals(b);

    public static bool operator <(Number a, Number b) => a.CompareTo(b) < 0;

    public static bool operator >(Number a, Number b) => a.CompareTo(b) > 0;

    public static bool operator <=(Number a, Number b) => a < b || a.Equals(b);

    public static bool operator >=(Number a, Number b) => a > b || a.Equals(b);

    public override bool Equals(object? obj)
    {
        if (obj is Number other)
        {
            if (Long is not null && other.Long is not null)
            {
                return Long == other.Long;
            }

            if (Long is not null && other.Decimal is not null)
            {
                return Long == other.Decimal;
            }

            if (Decimal is not null && other.Long is not null)
            {
                return Decimal == other.Long;
            }

            if (Decimal is not null && other.Decimal is not null)
            {
                return Decimal == other.Decimal;
            }
        }

        if (obj is long i)
        {
            if (Long is not null)
            {
                return Long == i;
            }

            if (Decimal is not null)
            {
                return Convert.ToInt64(Decimal) == i;
            }
        }

        if (obj is decimal d)
        {
            if (Decimal is not null)
            {
                return Decimal == d;
            }

            if (Long is not null)
            {
                return Convert.ToDecimal(Long) == d;
            }
        }

        return false;
    }

    public bool Equals(Number other) => Equals((object)other);

    public int CompareTo(Number other)
    {
        if (Long is not null && other.Long is not null)
        {
            return Math.Sign(Long.Value - other.Long.Value);
        }

        if (Long is not null && other.Decimal is not null)
        {
            return Math.Sign(Long.Value - other.Decimal.Value);
        }

        if (Decimal is not null && other.Long is not null)
        {
            return Math.Sign(Decimal.Value - other.Long.Value);
        }

        if (Decimal is not null && other.Decimal is not null)
        {
            return Math.Sign(Decimal.Value - other.Decimal.Value);
        }

        throw new NotImplementedException();
    }

    public override string ToString()
    {
        if (Long is not null)
        {
            return Long.ToString() ?? throw new NullReferenceException();
        }

        if (Decimal is not null)
        {
            return Decimal?.ToString("G29", CultureInfo.InvariantCulture) ?? throw new NullReferenceException();
        }

        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        if (Long is not null)
        {
            return Long.GetHashCode();
        }

        if (Decimal is not null)
        {
            return Decimal.GetHashCode();
        }

        throw new NotImplementedException();
    }
}
