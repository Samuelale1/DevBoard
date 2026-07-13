using DevBoard.Domain.Entities;
namespace DevBoard.Domain.Entities
{
    public class User: BaseEntity
    {
       public string firstName {get; set;}= string.Empty;
       public string lastName {get; set;}= string.Empty;
    }
}