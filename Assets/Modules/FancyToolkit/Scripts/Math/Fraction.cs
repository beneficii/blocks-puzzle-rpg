using System;

namespace FancyToolkit
{
    [Serializable]
    public class Fraction
    {
        public int numerator;
        public int denominator;

        public Fraction(int numerator, int denominator)
        {
            this.numerator = numerator;
            this.denominator = denominator;
        }

        [CreateFromString]
        public static Fraction Parse(string fractionString)
        {
            string[] parts = fractionString.Split('/');
            if (parts.Length == 0)
            {
                return new Fraction(0, 0);
            }
            else if (parts.Length == 1)
            {
                // the input string is an integer
                int integer = int.Parse(fractionString);
                return new Fraction(integer, 1);
            }
            else if (parts.Length == 2)
            {
                // the input string is a fraction
                int numerator = int.Parse(parts[0]);
                int denominator = int.Parse(parts[1]);
                return new Fraction(numerator, denominator);
            }
            else
            {
                throw new FormatException("Invalid fraction format: " + fractionString);
            }
        }

        public override string ToString()
        {
            return $"{numerator}/{denominator}";
        }

        public static Fraction operator +(Fraction f, int i)
        {
            return new Fraction(f.numerator + i * f.denominator, f.denominator);
        }

        public static Fraction operator -(Fraction f, int i)
        {
            return new Fraction(f.numerator - i * f.denominator, f.denominator);
        }

        public static Fraction operator *(Fraction f, int i)
        {
            return new Fraction(f.numerator * i, f.denominator);
        }

        public static Fraction operator /(Fraction f, int i)
        {
            return new Fraction(f.numerator, f.denominator * i);
        }

        public int ToInteger()
        {
            return numerator / denominator;
        }

        public static bool operator ==(Fraction f, int i)
        {
            return f.numerator == i * f.denominator;
        }

        public static bool operator !=(Fraction f, int i)
        {
            return f.numerator != i * f.denominator;
        }

        public static bool operator <(Fraction f, int i)
        {
            return f.numerator < i * f.denominator;
        }

        public static bool operator >(Fraction f, int i)
        {
            return f.numerator > i * f.denominator;
        }

        public static bool operator <=(Fraction f, int i)
        {
            return f.numerator <= i * f.denominator;
        }

        public static bool operator >=(Fraction f, int i)
        {
            return f.numerator >= i * f.denominator;
        }

        public static bool operator ==(Fraction f1, Fraction f2)
        {
            return f1.numerator == f2.numerator && f1.denominator == f2.denominator;
        }

        public static bool operator !=(Fraction f1, Fraction f2)
        {
            return f1.numerator != f2.numerator || f1.denominator != f2.denominator;
        }

        public static bool operator <(Fraction f1, Fraction f2)
        {
            int lcm = LCM(f1.denominator, f2.denominator);
            int numerator1 = f1.numerator * (lcm / f1.denominator);
            int numerator2 = f2.numerator * (lcm / f2.denominator);
            return numerator1 < numerator2;
        }

        public static bool operator >(Fraction f1, Fraction f2)
        {
            int lcm = LCM(f1.denominator, f2.denominator);
            int numerator1 = f1.numerator * (lcm / f1.denominator);
            int numerator2 = f2.numerator * (lcm / f2.denominator);
            return numerator1 > numerator2;
        }

        public static bool operator <=(Fraction f1, Fraction f2)
        {
            int lcm = LCM(f1.denominator, f2.denominator);
            int numerator1 = f1.numerator * (lcm / f1.denominator);
            int numerator2 = f2.numerator * (lcm / f2.denominator);
            return numerator1 <= numerator2;
        }

        public static bool operator >=(Fraction f1, Fraction f2)
        {
            int lcm = LCM(f1.denominator, f2.denominator);
            int numerator1 = f1.numerator * (lcm / f1.denominator);
            int numerator2 = f2.numerator * (lcm / f2.denominator);
            return numerator1 >= numerator2;
        }

        private static int LCM(int a, int b)
        {
            return Math.Abs(a * b) / GCD(a, b);
        }

        private static int GCD(int a, int b)
        {
            if (b == 0)
            {
                return a;
            }
            return GCD(b, a % b);
        }

        public override bool Equals(object obj)
        {
            return obj is Fraction fraction &&
                   numerator == fraction.numerator &&
                   denominator == fraction.denominator;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(numerator, denominator);
        }
    }
}
