namespace DevBoard.Domain.ValueObjects;

public readonly struct IssuePriority : IComparable<IssuePriority>
{
    private readonly int _level;

    private IssuePriority(int level)
    {
        _level = level;
    }
    // Predefined priority levels for issues. 
    // These static readonly fields represent different levels of priority that can be assigned to an issue.
    public static readonly IssuePriority None   = new(0);
    public static readonly IssuePriority Low    = new(1);
    public static readonly IssuePriority Medium = new(2);
    public static readonly IssuePriority High   = new(3);
    public static readonly IssuePriority Urgent = new(4);

    public int CompareTo(IssuePriority other)
        => _level.CompareTo(other._level);

    public static bool operator >(IssuePriority left, IssuePriority right)
        => left._level > right._level;

    public static bool operator <(IssuePriority left, IssuePriority right)
        => left._level < right._level;

    public static bool operator >=(IssuePriority left, IssuePriority right)
        => left._level >= right._level;

    public static bool operator <=(IssuePriority left, IssuePriority right)
        => left._level <= right._level;

    public override string ToString() => _level switch
    {
        0 => "None",
        1 => "Low",
        2 => "Medium",
        3 => "High",
        4 => "Urgent",
        _ => "Unknown"
    };
}