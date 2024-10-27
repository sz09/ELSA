using ELSA.Config;
using ELSA.Repositories.Base;
using ELSA.Repositories.Interface;
using ELSA.Repositories.Models;

namespace ELSA.Repositories
{
    public class ScoreRepository(IApplicationConfig config) : BaseSimpleRepository<ScoreModel>(config), IScoreRepository
    {
    }
}
