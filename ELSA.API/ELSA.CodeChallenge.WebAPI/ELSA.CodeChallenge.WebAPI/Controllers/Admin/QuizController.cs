using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ELSA.WebAPI.Models.Quiz;
using ELSA.Services.Interface;
using ELSA.Services.Utils;
using ELSA.WebAPI.ViewModels;
using ELSA.WebAPI.Utitlities;

namespace ELSA.WebAPI.Controllers.Admin
{
    [Route("api/admin/v{version:apiVersion}/quizzes")]
    public class QuizController(IQuizService quizService, IMapper mapper) : BaseAdminController
    {
        [Route("fetch")]
        [HttpPost]
        public async Task<IActionResult> FetchAsync([FromBody] FetchRequest fetchRequest)
        {
            var pageResult = await quizService.FetchAsync(fetchRequest);
            return Ok(pageResult.ConvertPageResult<Repositories.Models.QuizModel, ViewQuizModel>(mapper));
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateQuizAsync([FromBody] CreateQuizModel model)
        {
            return Ok(await quizService.CreateAsync(mapper.Map<Repositories.Models.QuizModel>(model)));
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> UpdateQuizAsync([FromBody] UpdateQuizModel model)
        {
            await quizService.UpdateQuizAsync(mapper.Map<Services.Models.Quizzes.UpdateQuizModel>(model));
            return Ok();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetQuizAsync(ObjectId id)
        {
            return Ok(await quizService.GetAsync(id));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteQuizAsync(ObjectId id)
        {
            await quizService.DeleteQuizByIdAsync(id);
            return Ok();
        }

        [HttpPut]
        [Route("assign-related-question/{id}/{questionId}")]
        public async Task<IActionResult> AssignRelatedQuestionAsync(ObjectId id, ObjectId questionId)
        {
            await quizService.AssignRelatedQuestionAsync(id, questionId);
            return Ok();
        }
    }
}
