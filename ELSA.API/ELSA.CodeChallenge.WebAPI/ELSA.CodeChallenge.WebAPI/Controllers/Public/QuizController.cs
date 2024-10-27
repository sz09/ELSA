using AutoMapper;
using ELSA.Services.Interface;
using ELSA.Services.Utils;
using ELSA.WebAPI.Utitlities;
using ELSA.WebAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ELSA.WebAPI.Controllers.Public
{
    [Route("api/v{version:apiVersion}/quizzes")]
    public class QuizController(IQuizService quizService, IMapper mapper) : BaseUserController
    {
        [Route("")]
        [HttpPost]
        public async Task<IActionResult> FetchAsync([FromBody] FetchRequest fetchRequest)
        {
            var pageResult = await quizService.FetchAsync(fetchRequest);
            return Ok(pageResult.ConvertPageResult<Repositories.Models.QuizModel, ViewQuizModel>(mapper));
        }

        [Route("quiz/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetQuizAsync(ObjectId id)
        {
            var response = await quizService.GetQuizByIdAsync(id);
            return Ok(response);
        }

        [Route("quiz-scored-questions/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetQuizScoredQuestionsAsync(ObjectId id)
        {
            var response = await quizService.GetQuizScoredQuestionsAsync(id);
            return Ok(response);
        }
    }
}
