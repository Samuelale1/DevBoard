using System.Text.RegularExpressions;

namespace DevBoard.Domain.ValueObjects;

public sealed record Slug
{
    public string Value { get; }

    private Slug(string value)
    {
        Value = value;
    }

    public static Slug From(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new ArgumentException("Slug cannot be empty.");

        var slug = Regex.Replace(
            raw.Trim().ToLowerInvariant(),
            @"[^a-z0-9\-]",
            "-");

        return new Slug(slug);
    }

    public override string ToString()
        => Value;
}