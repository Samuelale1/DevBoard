// src/DevBoard.Application/Import/CsvIssueParser.cs
namespace DevBoard.Application.Import;

using DevBoard.Domain.Enums;

public static class CsvIssueParser
{
    // Parses "Title,Bug,3" without allocating a string[] via string.Split —
    // each slice below is a view into the original line, no copy until .ToString() is called.
    public static IssueImportRow ParseLine(ReadOnlySpan<char> line)
    {
        var titleEnd = line.IndexOf(',');
        if (titleEnd < 0) throw new FormatException("Malformed CSV row: missing title separator.");
        var title = line[..titleEnd].ToString();

        var rest = line[(titleEnd + 1)..];
        var typeEnd = rest.IndexOf(',');
        if (typeEnd < 0) throw new FormatException("Malformed CSV row: missing type separator.");
        var type = Enum.Parse<IssueType>(rest[..typeEnd], ignoreCase: true);

        var priorityText = rest[(typeEnd + 1)..];
        var priority = int.Parse(priorityText);

        return new IssueImportRow(title, type, priority);
    }
}