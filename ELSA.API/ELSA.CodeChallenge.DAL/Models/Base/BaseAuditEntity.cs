using MongoDB.Bson;

namespace ELSA.DAL.Models.Base
{
    public abstract class BaseAuditEntity : BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ObjectId CreatedBy { get; set; }
        public ObjectId? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
