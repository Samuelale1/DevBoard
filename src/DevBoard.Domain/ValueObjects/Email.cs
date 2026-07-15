namespace DevBoard.Domain.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email From(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new ArgumentException("Email cannot be empty.");

        if (!raw.Contains('@'))
            throw new ArgumentException("Invalid email.");

        return new Email(raw.Trim().ToLowerInvariant());
    }

    public override string ToString()
        => Value;
}