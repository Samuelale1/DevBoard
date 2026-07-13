using DevBoard.Domain.Entities;
namespace DevBoard.Domain.Entities
{
    public class Issue: BaseEntity
    {
       public string title {get; set;}= string.Empty;
       public string description {get; set;}= string.Empty;
    }
}