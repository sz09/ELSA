using ELSA.CodeChallenge.Utilities;
using ELSA.Repositories.Interface;
using ELSA.Repositories.Models;
using ELSA.Services.Interface;
using ELSA.Services.Utils;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Runtime.InteropServices;
using ServiceEssayQuestionModel = ELSA.Services.Models.Questions.EssayQuestionModel;
using ServiceImageMultiChoiceQuestionModel = ELSA.Services.Models.Questions.ImageSingleChoiceQuestionModel;
using ServiceImageSingleChoiceQuestionModel = ELSA.Services.Models.Questions.ImageMultiChoiceQuestionModel;
using ServiceMultiChoiceQuestionModel = ELSA.Services.Models.Questions.MultiChoiceQuestionModel;
using ServiceQuizModel = ELSA.Services.Models.Quizzes.QuizModel;
using ServiceSingleChoiceQuestionModel = ELSA.Services.Models.Questions.SingleChoiceQuestionModel;

namespace ELSA.Services
{
    public class QuizService(IQuizRepository quizRepository, IScoreRepository scoreRepository, IQuestionRepository questionRepository, ILoggedInUserService loggedInUserService) : BaseService<QuizModel>(quizRepository),
            IQuizService
    {
        public Task AssignRelatedQuestionAsync(ObjectId quizId, ObjectId questionId)
        {
            var filter = Builders<QuizModel>.Filter.Eq(d => d.Id, quizId);
            var update = Builders<QuizModel>.Update.AddToSet(d => d.QuestionIds, questionId)
                                                   .Inc(d => d.QuestionLength, 1);
            return Repository.UpdateAsync(filter, update);
        }


        public async Task DeleteQuizByIdAsync(ObjectId objectId)
        {
            var filter = Builders<QuizModel>.Filter.Eq(d => d.Id, objectId);
            var quiz = await Repository.FindOneAsync(filter);
            if (quiz == null)
            {
                throw new Exception($"Not found {nameof(quiz)} with id: {objectId}");
            }

            await Repository.SoftDeleteOneAsync(filter);
            if (quiz.QuestionIds.Length > 0)
            {
               await questionRepository.SoftDeleteManyAsync(Builders<QuestionModel>.Filter.In(d => d.Id, quiz.QuestionIds));
            }
        }

        public async Task<ServiceQuizModel> GetQuizByIdAsync(ObjectId quizId)
        {
            return await GetQuizAsync(quizId);
        }

        public async Task<ObjectId[]> GetQuizScoredQuestionsAsync(ObjectId quizId)
        {
            var quiz = await Repository.GetOrCacheByIdAsync(quizId);
            var scoreModel = await scoreRepository.FindOneAsync(Builders<ScoreModel>.Filter.Eq(d => d.UserId, loggedInUserService.UserId));
            return scoreModel?.ScoreByQuesitons.Where(d => quiz.QuestionIds.Contains(d.QuestionId)).Select(d => d.QuestionId).ToArray() ?? [];
        }

        public async Task<ObjectId[]> GetRelatedQuestionsAsync(ObjectId questionId)
        {
            var filter = Builders<QuizModel>.Filter.Eq(NameCollector.Get<QuizModel>(d => d.QuestionIds), questionId);
            var repsonse = await (await Repository.FindAsync(filter, findOptions: new FindOptions<QuizModel, QuizModel>
            {
                Projection = Builders<QuizModel>.Projection
                    .Include(d => d.Id)
                    .Exclude(d => d.CreatedAt)
                    .Exclude(d => d.UpdatedAt)
                    .Exclude(d => d.UpdatedBy)
                    .Exclude(d => d.CreatedBy)
                    .Exclude(d => d.QuestionIds)
                    .Exclude(d => d.Subject)
            })).ToListAsync();
            return repsonse.Select(d => d.Id).ToArray();
        }

        public async Task UpdateQuizAsync(Models.Quizzes.UpdateQuizModel model)
        {
            var filter = Builders<QuizModel>.Filter.Eq(d => d.Id, model.Id);
            var quiz = await Repository.FindOneAsync(filter);
            if (quiz == null)
            {
                throw new InvalidDataException($"Not found {nameof(quiz)} with id: {model.Id}");
            }

            var update = Builders<QuizModel>.Update.Set(d => d.Subject, model.Subject);
            await Repository.UpdateAsync(filter, update);
        }

        private async Task<ServiceQuizModel> GetQuizAsync(ObjectId quizId)
        {
            var quiz = await Repository.GetOrCacheByIdAsync(quizId);
            if (quiz == null)
            {
                throw new InvalidDataException($"Invalid quiz with id {quizId}");
            }
            var result = new ServiceQuizModel
            {
                Id = quiz.Id,
                Subject = quiz.Subject,
            };

            if (quiz.QuestionIds?.Length == 0)
            {
                return result;
            }

            var questions = await (await questionRepository.FindAsync(Builders<QuestionModel>.Filter.In(d => d.Id, quiz.QuestionIds))).ToListAsync();
            result.Questions = [];
            foreach (var d in questions)
            {
                switch (d)
                {
                    case EssayQuestionModel essayQuestionModel:
                        result.Questions.Add(new ServiceEssayQuestionModel()
                        {
                            Id = essayQuestionModel.Id,
                            Question = essayQuestionModel.Question,
                        });
                        break;
                    case SingleChoiceQuestionModel singleChoiceQuestionModel:
                        result.Questions.Add(new ServiceSingleChoiceQuestionModel()
                        {
                            Id = singleChoiceQuestionModel.Id,
                            Question = singleChoiceQuestionModel.Question,
                            Options = singleChoiceQuestionModel.Options,
                        });
                        break;
                    case MultiChoiceQuestionModel multiChoiceQuestionModel:
                        result.Questions.Add(new ServiceMultiChoiceQuestionModel()
                        {
                            Id = multiChoiceQuestionModel.Id,
                            Question = multiChoiceQuestionModel.Question,
                            Options = multiChoiceQuestionModel.Options,
                        });
                        break;
                    // TODO: use azure blob here 
                    case ImageSingleChoiceQuestionModel imageSingleChoiceQuestionModel:
                        result.Questions.Add(new ServiceImageSingleChoiceQuestionModel()
                        {
                            Id = imageSingleChoiceQuestionModel.Id,
                            Question = imageSingleChoiceQuestionModel.Question,
                            Options = imageSingleChoiceQuestionModel.Options,
                        });
                        break;
                    case ImageMultiChoiceQuestionModel imageMultiChoiceQuestionModel:
                        result.Questions.Add(new ServiceImageMultiChoiceQuestionModel()
                        {
                            Id = imageMultiChoiceQuestionModel.Id,
                            Question = imageMultiChoiceQuestionModel.Question,
                            Options = imageMultiChoiceQuestionModel.Options,
                        });
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            return result;
        }
    }
}