using System.Data;

public static class Extensions
{
    public static bool IsNumeric(this string s)
    {
        double numberValue = 0;
        return double.TryParse(s, out numberValue);
    }

    public static bool ToBoolean(this string s)
    {
        int value = 0;

        if (int.TryParse(s, out value))
        {
            return value.Equals(1);
        }
        else
        {
            if (s.Length > 0)
            {
                return s.Trim().Equals("true", System.StringComparison.CurrentCultureIgnoreCase);
            }
            else
            {
                return false;
            }
        }
    }

    public static float ToFloat(this string s)
    {
        float value = 0;

        if (s.IsNumeric())
        {
            if (float.TryParse(s, out value))
            {
                return value;
            }
            else
            {
                throw new System.Exception("Could not parse string value: " + s);
            }
        }
        else
        {
            throw new System.ArgumentException("String is not numeric.");
        }
    }
}