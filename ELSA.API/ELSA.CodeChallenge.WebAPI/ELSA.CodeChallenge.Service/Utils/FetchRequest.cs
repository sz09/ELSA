using MongoDB.Driver;
using ELSA.DAL.Models.Base;

namespace ELSA.Services.Utils
{
    public class FetchRequest
    {
        public int Skip => (Page - 1) * PageSize; 
        /// <summary>
        /// Page equal 1 means first page
        /// </summary>
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; }
        public Direction OrderDirection { get; set; }
        
        public SortDefinition<T> ToSort<T>() where T: BaseAuditEntity
        {
           FieldDefinition<T> fieldDefinition = new StringFieldDefinition<T>(OrderBy);
            switch (OrderDirection)
            {
                case Direction.Asc:
                    return Builders<T>.Sort.Ascending(fieldDefinition);
                case Direction.Desc:
                    return Builders<T>.Sort.Descending(fieldDefinition);
                default:
                    throw new NotSupportedException(nameof(OrderDirection));
        }
           
        }
    }

    public enum Direction
    {
        Asc = 1,
        Desc = -1
    }
}
