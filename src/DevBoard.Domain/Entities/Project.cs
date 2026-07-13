using DevBoard.Domain.Entities;
namespace DevBoard.Domain.Entities
{

public class Project : BaseEntity

{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public Project(string name, string description)
    {
        Name = name;
        Description = description;
    }
}

}