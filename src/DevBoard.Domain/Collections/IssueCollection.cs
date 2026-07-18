using System.Collections;
using DevBoard.Domain.Entities;

namespace DevBoard.Domain.Collections;

public sealed class IssueCollection : IEnumerable<Issue>
{
    private readonly Dictionary<string, Issue> _issues = [];

    public void Add(Issue issue)
    {
        _issues.Add(issue.IssueKey, issue);
    }

    public Issue? this[string issueKey]
    {
        get
        {
            return _issues.TryGetValue(issueKey, out var issue)
                ? issue
                : null;
        }
    }

    public IEnumerator<Issue> GetEnumerator()
    {
        return _issues.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}