using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ELSA.WebAPI.Models.Question;
using ELSA.WebAPI.ViewModels.Question;
using ELSA.Services.Interface;
using ELSA.Services.Utils;

namespace ELSA.WebAPI.Controllers.Admin
{
    [Route("api/admin/v{version:apiVersion}/questions")]
    public class QuestionController(IQuestionService questionService, IMapper mapper) : BaseAdminController
    {
        [Route("fetch")]
        [HttpPost]
        public async Task<IActionResult> FetchAsync([FromBody] FetchRequest fetchRequest)
        {
            var pageResult = await questionService.FetchAsync(fetchRequest);
            return Ok(ConvertPageResult(pageResult));
        }
        [HttpPost]
        public async Task<IActionResult> CreateQuestionAsync([FromForm] CreateQuestionModel model)
        {
            Services.Models.Questions.QuestionModel mapperModel;
            switch (model.Type)
            {
                case QuestionType.Essay:
                    mapperModel = mapper.Map<Services.Models.Questions.EssayQuestionModel>(model);
                    break;
                case QuestionType.Binary:
                    mapperModel = mapper.Map<Services.Models.Questions.BinaryQuestionModel>(model);
                    break;
                case QuestionType.MultiChoice:
                    mapperModel = mapper.Map<Services.Models.Questions.MultiChoiceQuestionModel>(model);
                    break;
                case QuestionType.SingleChoice:
                    mapperModel = mapper.Map<Services.Models.Questions.SingleChoiceQuestionModel>(model);
                    break;
                case QuestionType.ImageMultiChoice:
                    mapperModel = mapper.Map<Services.Models.Questions.CreateImageMultiChoiceQuestionModel>(model);
                    break;
                case QuestionType.ImageSingleChoice:
                    mapperModel = mapper.Map<Services.Models.Questions.CreateImageSingleChoiceQuestionModel>(model);
                    break;
                default:
                    throw new InvalidOperationException($"Provide invalid {nameof(model.Type)}!");
            }
            var response = await questionService.CreateQuestionAsync(mapperModel);
            return Ok(response);
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> UpdateQuesionAsync([FromForm] UpdateQuestionModel model)
        {
            Services.Models.Questions.QuestionModel mapperModel;
            switch (model.Type)
            {
                case QuestionType.Essay:
                    mapperModel = mapper.Map<Services.Models.Questions.EssayQuestionModel>(model);
                    break;
                case QuestionType.Binary:
                    mapperModel = mapper.Map<Services.Models.Questions.BinaryQuestionModel>(model);
                    break;
                case QuestionType.MultiChoice:
                    mapperModel = mapper.Map<Services.Models.Questions.MultiChoiceQuestionModel>(model);
                    break;
                case QuestionType.SingleChoice:
                    mapperModel = mapper.Map<Services.Models.Questions.SingleChoiceQuestionModel>(model);
                    break;
                case QuestionType.ImageMultiChoice:
                    mapperModel = mapper.Map<Services.Models.Questions.UpdateImageMultiChoiceQuestionModel>(model);
                    break;
                case QuestionType.ImageSingleChoice:
                    mapperModel = mapper.Map<Services.Models.Questions.UpdateImageSingleChoiceQuestionModel>(model);
                    break;
                default:
                    throw new InvalidOperationException($"Provide invalid {nameof(model.Type)}!");
            }
            await questionService.UpdateQuestionAsync(mapperModel);
            return Ok();
        }

        [Route("from-quiz/{quizId}")]
        [HttpGet]
        public async Task<IActionResult> GetQuizAsync(ObjectId quizId)
        {
            var response = await questionService.GetQuestionByQuizAsync(quizId);
            return Ok(response);
        }
        
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetQuesionAsync(ObjectId id)
        {
            return Ok(await questionService.GetAsync(id));
        }
        private PageResult<ViewQuestionModel> ConvertPageResult(PageResult<Repositories.Models.QuestionModel> pageResult)
        {
            var result = new PageResult<ViewQuestionModel>()
            {
                Total = pageResult.Total
            };

            foreach (var model in pageResult.Data)
            {
                switch (model.Type)
                {
                    case Repositories.Models.QuestionType.Essay:
                        result.Data.Add(mapper.Map<ViewEssayQuestionModel>(model));
                        break;
                    case Repositories.Models.QuestionType.Binary:
                        result.Data.Add(mapper.Map<ViewBinaryQuestionModel>(model));
                        break;
                    case Repositories.Models.QuestionType.MultiChoice:
                        result.Data.Add(mapper.Map<ViewMultiChoiceQuestionModel>(model));
                        break;
                    case Repositories.Models.QuestionType.SingleChoice:
                        result.Data.Add(mapper.Map<ViewSingleChoiceQuestionModel>(model));
                        break;
                    case Repositories.Models.QuestionType.ImageMultiChoice:
                        result.Data.Add(mapper.Map<ViewImageMultiChoiceQuestionModel>(model));
                        break;
                    case Repositories.Models.QuestionType.ImageSingleChoice:
                        result.Data.Add(mapper.Map<ViewImageSingleChoiceQuestionModel>(model));
                        break;
                    default:
                        throw new InvalidOperationException($"Provide invalid {nameof(model.Type)}!");
                }
            }

            return result;
        }
    }
}
