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

    public static Number operator +(Number a, Number b)
    {
        if (a.Long != null && b.Long != null)
        {
            return new Number(a.Long.Value + b.Long.Value);
        }

        if (a.Long != null && b.Decimal != null)
        {
            return new Number(Convert.ToDecimal(a.Long) + b.Decimal.Value);
        }

        if (a.Decimal != null && b.Long != null)
        {
            return new Number(a.Decimal.Value + Convert.ToDecimal(b.Long));
        }

        if (a.Decimal != null && b.Decimal != null)
        {
            return new Number(a.Decimal.Value + b.Decimal.Value);
        }

        throw new NotImplementedException();
    }

    public static Number operator -(Number a, Number b)
    {
        if (a.Long != null && b.Long != null)
        {
            return new Number(a.Long.Value - b.Long.Value);
        }

        if (a.Long != null && b.Decimal != null)
        {
            return new Number(Convert.ToDecimal(a.Long) - b.Decimal.Value);
        }

        if (a.Decimal != null && b.Long != null)
        {
            return new Number(a.Decimal.Value - Convert.ToDecimal(b.Long));
        }

        if (a.Decimal != null && b.Decimal != null)
        {
            return new Number(a.Decimal.Value - b.Decimal.Value);
        }

        throw new NotImplementedException();
    }

    public static Number operator *(Number a, Number b)
    {
        if (a.Long != null && b.Long != null)
        {
            return new Number(a.Long.Value * b.Long.Value);
        }

        if (a.Long != null && b.Decimal != null)
        {
            return new Number(Convert.ToDecimal(a.Long) * b.Decimal.Value);
        }

        if (a.Decimal != null && b.Long != null)
        {
            return new Number(a.Decimal.Value * Convert.ToDecimal(b.Long));
        }

        if (a.Decimal != null && b.Decimal != null)
        {
            return new Number(a.Decimal.Value * b.Decimal.Value);
        }

        throw new NotImplementedException();
    }

    public static Number operator /(Number a, Number b)
    {
        if (a.Long != null && b.Long != null)
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

        if (a.Long != null && b.Decimal != null)
        {
            if (b.Decimal == 0)
            {
                throw new DivideByZeroException();
            }

            return new Number(Convert.ToDecimal(a.Long) / b.Decimal.Value);
        }

        if (a.Decimal != null && b.Long != null)
        {
            if (b.Long == 0)
            {
                throw new DivideByZeroException();
            }

            return new Number(a.Decimal.Value / Convert.ToDecimal(b.Long));
        }

        if (a.Decimal != null && b.Decimal != null)
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
            if (Long != null && other.Long != null)
            {
                return Long == other.Long;
            }

            if (Long != null && other.Decimal != null)
            {
                return Long == other.Decimal;
            }

            if (Decimal != null && other.Long != null)
            {
                return Decimal == other.Long;
            }

            if (Decimal != null && other.Decimal != null)
            {
                return Decimal == other.Decimal;
            }
        }

        if (obj is long i)
        {
            if (Long != null)
            {
                return Long == i;
            }

            if (Decimal != null)
            {
                return Convert.ToInt64(Decimal) == i;
            }
        }

        if (obj is decimal d)
        {
            if (Decimal != null)
            {
                return Decimal == d;
            }

            if (Long != null)
            {
                return Convert.ToDecimal(Long) == d;
            }
        }

        return false;
    }

    public bool Equals(Number other) => Equals((object)other);

    public int CompareTo(Number other)
    {
        if (Long != null && other.Long != null)
        {
            return Math.Sign(Long.Value - other.Long.Value);
        }

        if (Long != null && other.Decimal != null)
        {
            return Math.Sign(Long.Value - other.Decimal.Value);
        }

        if (Decimal != null && other.Long != null)
        {
            return Math.Sign(Decimal.Value - other.Long.Value);
        }

        if (Decimal != null && other.Decimal != null)
        {
            return Math.Sign(Decimal.Value - other.Decimal.Value);
        }

        throw new NotImplementedException();
    }

    public override string ToString()
    {
        if (Long != null)
        {
            return Long.ToString() ?? throw new NullReferenceException();
        }

        if (Decimal != null)
        {
            return Decimal?.ToString("G29", CultureInfo.InvariantCulture) ?? throw new NullReferenceException();
        }

        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        if (Long != null)
        {
            return Long.GetHashCode();
        }

        if (Decimal != null)
        {
            return Decimal.GetHashCode();
        }

        throw new NotImplementedException();
    }
}
