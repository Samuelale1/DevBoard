using DevBoard.Domain.Entities;

namespace DevBoard.Domain.Entities
{
    public class Webhook: BaseEntity
    {
       public string url {get; set;}= string.Empty;
       public string secret {get; set;}= string.Empty;
    }
}