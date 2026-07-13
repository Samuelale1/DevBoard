using DevBoard.Domain.Entities;
namespace DevBoard.Domain.Entities
{
    public class AuditLogEntry: BaseEntity
    {
       public string action {get; set;}= string.Empty;
       public string description {get; set;}= string.Empty;
    }
}