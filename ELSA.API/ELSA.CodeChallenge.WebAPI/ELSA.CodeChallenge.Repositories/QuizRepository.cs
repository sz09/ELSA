using ELSA.Repositories.Interface;
using ELSA.Config;
using ELSA.Repositories.Base;
using ELSA.Repositories.Models;
using ELSA.CodeChallenge.Repositories.Caching;

namespace ELSA.Repositories
{
    public class QuizRepository : BaseRepository<QuizModel>, IQuizRepository
    {
        public QuizRepository(IApplicationConfig config, ILoggedInUserService loggedInUserService, InMemoryCache memoryCache) : base(config, loggedInUserService, memoryCache)
        {
        }
    }
}
