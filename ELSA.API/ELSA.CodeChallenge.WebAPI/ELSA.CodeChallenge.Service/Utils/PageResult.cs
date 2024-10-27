namespace ELSA.Services.Utils
{
    public class PageResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public long Total { get; set; }
    }
}
