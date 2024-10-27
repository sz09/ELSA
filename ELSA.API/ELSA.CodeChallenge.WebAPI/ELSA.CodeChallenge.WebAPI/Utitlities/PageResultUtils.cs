using AutoMapper;
using ELSA.Services.Utils;

namespace ELSA.WebAPI.Utitlities
{
    public static class PageResultUtils
    {
        public static PageResult<R> ConvertPageResult<T, R>(this PageResult<T> pageResult, IMapper mapper)
        {
            return new PageResult<R>
            {
                Data = pageResult.Data.Select(d => mapper.Map<R>(d)).ToList(),
                Total = pageResult.Total,
            };
        }
    }
}
