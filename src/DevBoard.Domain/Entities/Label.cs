using DevBoard.Domain.Entities;
namespace DevBoard.Domain.Entities
{
    public class Label: BaseEntity
    {
       public string name {get; set;}= string.Empty;
       public string description {get; set;}= string.Empty;
    }
}