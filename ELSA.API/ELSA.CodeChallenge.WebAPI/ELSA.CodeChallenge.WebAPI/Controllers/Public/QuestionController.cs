using AutoMapper;
using ELSA.Services.Interface;
using ELSA.WebAPI.Models.Answer;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ELSA.WebAPI.Controllers.Public
{
    [Route("api/v{version:apiVersion}/questions")]
    public class QuestionController(IQuestionService questionService, IAnswerService answerService) : BaseUserController
    {
        [Route("from-quiz/{quizId}")]
        [HttpGet]
        public async Task<IActionResult> GetQuizAsync(ObjectId quizId)
        {
            var response = await questionService.GetQuestionByQuizAsync(quizId);
            return Ok(response);
        }

        [Route("answers")]
        [HttpPut]
        public async Task<IActionResult> AnswerQuestionAsync([FromBody] AnswerModel model)
        {
            return Ok(await answerService.AnswerAQuestionAsync(model.QuestionId, model.Answer));
        }
    }
}
