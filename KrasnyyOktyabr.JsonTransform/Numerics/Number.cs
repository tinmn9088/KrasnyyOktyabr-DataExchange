using System.Globalization;

namespace KrasnyyOktyabr.JsonTransform.Numerics;

/// <remarks>
/// No symmetry when checking equivalence with <see cref="int"/> or <see cref="double"/>.
/// </remarks>
public readonly struct Number : IEquatable<Number>, IComparable<Number>
{
    public double? Double
    {
        get;
    }

    public int? Int
    {
        get;
    }

    /// <summary>
    /// Int 0.
    /// </summary>
    public Number()
    {
        Int = 0;
    }

    public Number(double value)
    {
        Double = value;
    }

    public Number(int value)
    {
        Int = value;
    }

    public static Number operator +(Number a, Number b)
    {
        if (a.Int != null && b.Int != null)
        {
            return new Number((int)(a.Int + b.Int));
        }

        if (a.Int != null && b.Double != null)
        {
            return new Number((double)(a.Int + b.Double));
        }

        if (a.Double != null && b.Int != null)
        {
            return new Number((double)(a.Double + b.Int));
        }

        if (a.Double != null && b.Double != null)
        {
            return new Number((double)(a.Double + b.Double));
        }

        throw new NotImplementedException();
    }

    public static Number operator -(Number a, Number b)
    {
        if (a.Int != null && b.Int != null)
        {
            return new Number((int)(a.Int - b.Int));
        }

        if (a.Int != null && b.Double != null)
        {
            return new Number((double)(a.Int - b.Double));
        }

        if (a.Double != null && b.Int != null)
        {
            return new Number((double)(a.Double - b.Int));
        }

        if (a.Double != null && b.Double != null)
        {
            return new Number((double)(a.Double - b.Double));
        }

        throw new NotImplementedException();
    }

    public static Number operator *(Number a, Number b)
    {
        if (a.Int != null && b.Int != null)
        {
            return new Number((int)(a.Int * b.Int));
        }

        if (a.Int != null && b.Double != null)
        {
            return new Number((double)(a.Int * b.Double));
        }

        if (a.Double != null && b.Int != null)
        {
            return new Number((double)(a.Double * b.Int));
        }

        if (a.Double != null && b.Double != null)
        {
            return new Number((double)(a.Double * b.Double));
        }

        throw new NotImplementedException();
    }

    public static Number operator /(Number a, Number b)
    {
        if (a.Int != null && b.Int != null)
        {
            if (b.Int == 0)
            {
                throw new DivideByZeroException();
            }

            if (a.Int % b.Int == 0)
            {
                return new Number((int)(a.Int / b.Int));
            }
            else
            {
                return new Number((double)a.Int / (double)b.Int);
            }
        }

        if (a.Int != null && b.Double != null)
        {
            if (b.Double == 0)
            {
                throw new DivideByZeroException();
            }

            return new Number((double)(a.Int / b.Double));
        }

        if (a.Double != null && b.Int != null)
        {
            if (b.Int == 0)
            {
                throw new DivideByZeroException();
            }

            return new Number((double)(a.Double / b.Int));
        }

        if (a.Double != null && b.Double != null)
        {
            if (b.Double == 0)
            {
                throw new DivideByZeroException();
            }

            return new Number((double)(a.Double / b.Double));
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
            if (Int != null && other.Int != null)
            {
                return Int == other.Int;
            }

            if (Int != null && other.Double != null)
            {
                return Int == other.Double;
            }

            if (Double != null && other.Int != null)
            {
                return Double == other.Int;
            }

            if (Double != null && other.Double != null)
            {
                return Double == other.Double;
            }
        }

        if (obj is int i)
        {
            if (Int != null)
            {
                return Int == i;
            }

            if (Double != null)
            {
                return Convert.ToInt32(Double) == i;
            }
        }

        if (obj is double d)
        {
            if (Double != null)
            {
                return Double == d;
            }

            if (Int != null)
            {
                return Convert.ToDouble(Int) == d;
            }
        }

        return false;
    }

    public bool Equals(Number other) => Equals((object)other);

    public int CompareTo(Number other)
    {
        if (Int != null && other.Int != null)
        {
            return Int.Value - other.Int.Value;
        }

        if (Int != null && other.Double != null)
        {
            return Math.Sign(Int.Value - other.Double.Value);
        }

        if (Double != null && other.Int != null)
        {
            return Math.Sign(Double.Value - other.Int.Value);
        }

        if (Double != null && other.Double != null)
        {
            return Math.Sign(Double.Value - other.Double.Value);
        }

        throw new NotImplementedException();
    }

    public override string ToString()
    {
        if (Int != null)
        {
            return Int.ToString() ?? throw new NullReferenceException();
        }

        if (Double != null)
        {
            return Double?.ToString(CultureInfo.InvariantCulture) ?? throw new NullReferenceException();
        }

        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        if (Int != null)
        {
            return Int.GetHashCode();
        }

        if (Double != null)
        {
            return Double.GetHashCode();
        }

        throw new NotImplementedException();
    }
}
