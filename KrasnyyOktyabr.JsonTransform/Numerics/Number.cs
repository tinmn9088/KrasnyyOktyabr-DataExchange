using System.Globalization;

namespace KrasnyyOktyabr.JsonTransform.Numerics;

public readonly struct Number
{
    public double? Double
    {
        get;
    }

    public int? Int
    {
        get;
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

    public static bool operator ==(Number a, Number b)
    {
        if (a.Int != null && b.Int != null)
        {
            return a.Int == b.Int;
        }

        if (a.Int != null && b.Double != null)
        {
            return a.Int == b.Double;
        }

        if (a.Double != null && b.Int != null)
        {
            return a.Double == b.Int;
        }

        if (a.Double != null && b.Double != null)
        {
            return a.Double == b.Double;
        }

        throw new NotImplementedException();
    }

    public static bool operator !=(Number a, Number b)
    {
        if (a.Int != null && b.Int != null)
        {
            return a.Int != b.Int;
        }

        if (a.Int != null && b.Double != null)
        {
            return a.Int != b.Double;
        }

        if (a.Double != null && b.Int != null)
        {
            return a.Double != b.Int;
        }

        if (a.Double != null && b.Double != null)
        {
            return a.Double != b.Double;
        }

        throw new NotImplementedException();
    }

    public static bool operator <(Number a, Number b)
    {
        if (a.Int != null && b.Int != null)
        {
            return a.Int < b.Int;
        }

        if (a.Int != null && b.Double != null)
        {
            return a.Int < b.Double;
        }

        if (a.Double != null && b.Int != null)
        {
            return a.Double < b.Int;
        }

        if (a.Double != null && b.Double != null)
        {
            return a.Double < b.Double;
        }

        throw new NotImplementedException();
    }

    public static bool operator >(Number a, Number b)
    {
        if (a.Int != null && b.Int != null)
        {
            return a.Int > b.Int;
        }

        if (a.Int != null && b.Double != null)
        {
            return a.Int > b.Double;
        }

        if (a.Double != null && b.Int != null)
        {
            return a.Double > b.Int;
        }

        if (a.Double != null && b.Double != null)
        {
            return a.Double > b.Double;
        }

        throw new NotImplementedException();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Number other)
        {
            return this == other;
        }
        else
        {
            return false;
        }
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
