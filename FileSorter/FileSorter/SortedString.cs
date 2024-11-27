namespace FileSorter;

/// <summary>
/// Class for compare strings from a file.
/// </summary>
public class SortedString : IComparable<SortedString>
{
    public string String { get; private set; }
    public int Prefix { get; private set; }

    /// <summary>
    /// Initialize a new SortedString instance.
    /// </summary>
    public SortedString()
    {
        String = string.Empty; Prefix = 0;
    }

    /// <summary>
    /// Initialize a new SortedString instance.
    /// </summary>
    /// <param name="str">A string from a file which is going to be compared</param>
    /// <exception cref="ArgumentException">If the input string has a wrong syntax.</exception>
    public SortedString(string str)
    {
        Initialize(str);
    }

    /// <summary>
    /// Overwrite object's properties with input string.
    /// </summary>
    /// <param name="str">A string from a file which is going to be compared</param>
    /// <exception cref="ArgumentException">If the input string has a wrong syntax.</exception>
    public void Initialize(string str)
    {
        var parts = str.Split(". ");
        if (parts.Length < 2)
            throw new ArgumentException("Format is incorrect.");

        var prefix = 0;
        if (!int.TryParse(parts[0], out prefix))
            throw new ArgumentException("The first part must be a number.");

        Prefix = prefix;
        String = parts[1];
    }

    public int CompareTo(SortedString? other)
    {
        if (this == other) return 0;
        if (this < other) return -1;
        return 1;
    }

    public override string ToString()
    {
        return string.Join(". ", Prefix, String);
    }

    public static bool operator >(SortedString s1, SortedString s2)
    {
        var compareStrings = s1.String.CompareTo(s2.String);
        if (compareStrings != 0)
            return compareStrings > 0;
        return s1.Prefix > s2.Prefix;
    }

    public static bool operator <(SortedString s1, SortedString s2)
    {
        var compareStrings = s1.String.CompareTo(s2.String);
        if (compareStrings != 0)
            return compareStrings < 0;
        return s1.Prefix < s2.Prefix;
    }

    public static bool operator >=(SortedString s1, SortedString s2)
    {
        var compareStrings = s1.String.CompareTo(s2.String);
        if (compareStrings != 0)
            return compareStrings > 0;
        return s1.Prefix >= s2.Prefix;
    }

    public static bool operator <=(SortedString s1, SortedString s2)
    {
        var compareStrings = s1.String.CompareTo(s2.String);
        if (compareStrings != 0)
            return compareStrings < 0;
        return s1.Prefix <= s2.Prefix;
    }

    public static bool operator ==(SortedString s1, SortedString s2)
    {
        var compareStrings = s1.String.CompareTo(s2.String);
        if (compareStrings != 0)
            return false;
        return s1.Prefix == s2.Prefix;
    }

    public static bool operator !=(SortedString s1, SortedString s2)
    {
        var compareStrings = s1.String.CompareTo(s2.String);
        if (compareStrings != 0)
            return true;
        return s1.Prefix != s2.Prefix;
    }
}
