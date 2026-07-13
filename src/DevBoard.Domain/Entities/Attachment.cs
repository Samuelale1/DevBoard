using DevBoard.Domain.Entities;
namespace DevBoard.Domain.Entities
{
    public class Attachment: BaseEntity
    {
       public string fileName {get; set;}= string.Empty;
       public string filePath {get; set;}= string.Empty;
    }
}