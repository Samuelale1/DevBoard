// src/DevBoard.Application/Import/IssueImportRow.cs
using DevBoard.Domain.Enums;

namespace DevBoard.Application.Import;

public sealed record IssueImportRow(string Title, IssueType Type, int Priority);