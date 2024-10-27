using AutoMapper;
using ELSA.CodeChallenge.Services.Utils;
using ELSA.Repositories.Interface;
using ELSA.Services.Interface;
using ELSA.Services.Models.Questions;
using ELSA.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using RepositoryBinaryQuestionModel = ELSA.Repositories.Models.BinaryQuestionModel;
using RepositoryEssayQuestionModel = ELSA.Repositories.Models.EssayQuestionModel;
using RepositoryImageMultiChoiceQuestionModel = ELSA.Repositories.Models.ImageMultiChoiceQuestionModel;
using RepositoryImageSingleChoiceQuestionModel = ELSA.Repositories.Models.ImageSingleChoiceQuestionModel;
using RepositoryMultiChoiceQuestionModel = ELSA.Repositories.Models.MultiChoiceQuestionModel;
using RepositoryQuestionModel = ELSA.Repositories.Models.QuestionModel;
using RepositorySingleChoiceQuestionModel = ELSA.Repositories.Models.SingleChoiceQuestionModel;

namespace ELSA.Services
{
    public class QuestionService(IQuestionRepository questionRepository, IQuizRepository quizRepository, IFileService fileService, IMapper mapper) : BaseService<RepositoryQuestionModel>(questionRepository),
        IQuestionService
    {
        public async Task<RepositoryQuestionModel> CreateQuestionAsync(QuestionModel questionModel)
        {
            var question = await GetCreateQuestionModelAsync(questionModel);
            return await CreateAsync(question);
        }

        public async Task<QuestionModel[]> GetQuestionByQuizAsync(ObjectId quizId)
        {
            var quiz = await quizRepository.GetOrCacheByIdAsync(quizId);
            if (quiz == null)
            {
                throw new InvalidDataException($"Not have any quiz by {quizId}");
            }

            if (quiz.QuestionIds?.Any() != true)
            {
                return [];
            }

            return (await quiz.QuestionIds.ConcurrentExecForEachAsync(questionRepository.GetOrCacheByIdAsync))
                           .Where(d => d != null)
                           .OrderBy(d => Array.IndexOf(array: quiz.QuestionIds!, d.Id))
                           .Select(Map)
                           .ToArray();
        }

        public async Task UpdateQuestionAsync(QuestionModel questionModel)
        {
            var filter = Builders<RepositoryQuestionModel>.Filter.Eq(d => d.Id, questionModel.Id);
            var existingQuestion = questionRepository.FindOneAsync(filter);
            if (existingQuestion == null)
            {
                throw new InvalidOperationException($"{nameof(existingQuestion)} is not found");
            }
            string[] fileNeedToCleans = [];
            if (existingQuestion is Repositories.Models.ImageQuestion imageQuestion)
            {
                fileNeedToCleans = imageQuestion.Options;
            }

            var question = await GetUpdateQuestionModelAsync(questionModel);
            await Repository.UpdateAsync(filter, question.ExtractUpdateEntity());
            if (fileNeedToCleans.Length > 0)
            {
                await fileService.CleanFilesAsync(fileNeedToCleans);
            }
        }

        private async Task<RepositoryQuestionModel> GetCreateQuestionModelAsync(QuestionModel questionModel)
        {
            RepositoryQuestionModel question;
            switch (questionModel)
            {
                case EssayQuestionModel essayQuestionModel:
                    question = new RepositoryEssayQuestionModel()
                    {
                        Question = essayQuestionModel.Question,
                        CorrectAnswer = essayQuestionModel.CorrectAnswer,
                        IgnoreCase = essayQuestionModel.IgnoreCase,
                        AnyAnswer = essayQuestionModel.AnyAnswer,
                        Points = essayQuestionModel.Points
                    };
                    break;
                case BinaryQuestionModel binaryQuestionModel:
                    question = new RepositoryBinaryQuestionModel()
                    {
                        Question = binaryQuestionModel.Question,
                        CorrectAnswer = binaryQuestionModel.CorrectAnswer,
                        Points = binaryQuestionModel.Points
                    };
                    break;
                case SingleChoiceQuestionModel singleChoiceQuestionModel:
                    question = new RepositorySingleChoiceQuestionModel()
                    {
                        Question = singleChoiceQuestionModel.Question,
                        Options = singleChoiceQuestionModel.Options,
                        CorrectAnswer = singleChoiceQuestionModel.CorrectAnswer,
                        Points = singleChoiceQuestionModel.Points
                    };
                    break;
                case MultiChoiceQuestionModel multiChoiceQuestionModel:
                    question = new RepositoryMultiChoiceQuestionModel()
                    {
                        Question = multiChoiceQuestionModel.Question,
                        Options = multiChoiceQuestionModel.Options,
                        CorrectAnswers = multiChoiceQuestionModel.CorrectAnswers,
                        Points = multiChoiceQuestionModel.Points
                    };
                    break;
                case CreateImageSingleChoiceQuestionModel imageSingleChoiceQuestionModel:
                    {
                        var options = await fileService.StoreFilesAsync(ObjectId.GenerateNewId(), imageSingleChoiceQuestionModel.Files);
                        question = new RepositoryImageSingleChoiceQuestionModel()
                        {
                            Question = imageSingleChoiceQuestionModel.Question,
                            Options = options,
                            CorrectAnswer = imageSingleChoiceQuestionModel.CorrectAnswer,
                            Points = imageSingleChoiceQuestionModel.Points
                        };
                    }
                    break;
                case CreateImageMultiChoiceQuestionModel multiChoiceQuestionModel:
                    {
                        var options = await fileService.StoreFilesAsync(ObjectId.GenerateNewId(), multiChoiceQuestionModel.Files);
                        question = new RepositoryImageMultiChoiceQuestionModel()
                        {
                            Question = multiChoiceQuestionModel.Question,
                            Options = options,
                            CorrectAnswers = multiChoiceQuestionModel.CorrectAnswers,
                            Points = multiChoiceQuestionModel.Points
                        };
                    }

                    break;
                default:
                    throw new NotSupportedException();
            }
            return question;
        }

        private async Task<RepositoryQuestionModel> GetUpdateQuestionModelAsync(QuestionModel questionModel)
        {
            RepositoryQuestionModel question;
            switch (questionModel)
            {
                case EssayQuestionModel essayQuestionModel:
                    question = new RepositoryEssayQuestionModel()
                    {
                        Id = questionModel.Id,
                        Question = essayQuestionModel.Question,
                        CorrectAnswer = essayQuestionModel.CorrectAnswer,
                        AnyAnswer = essayQuestionModel.AnyAnswer,
                        IgnoreCase = essayQuestionModel.IgnoreCase,
                        Points = essayQuestionModel.Points
                    };
                    break;
                case BinaryQuestionModel binaryQuestionModel:
                    question = new RepositoryBinaryQuestionModel()
                    {
                        Id = questionModel.Id,
                        Question = binaryQuestionModel.Question,
                        CorrectAnswer = binaryQuestionModel.CorrectAnswer,
                        Points = binaryQuestionModel.Points
                    };
                    break;
                case SingleChoiceQuestionModel singleChoiceQuestionModel:
                    question = new RepositorySingleChoiceQuestionModel()
                    {
                        Id = questionModel.Id,
                        Question = singleChoiceQuestionModel.Question,
                        Options = singleChoiceQuestionModel.Options,
                        CorrectAnswer = singleChoiceQuestionModel.CorrectAnswer,
                        Points = singleChoiceQuestionModel.Points
                    };
                    break;
                case MultiChoiceQuestionModel multiChoiceQuestionModel:
                    question = new RepositoryMultiChoiceQuestionModel()
                    {
                        Id = questionModel.Id,
                        Question = multiChoiceQuestionModel.Question,
                        Options = multiChoiceQuestionModel.Options,
                        CorrectAnswers = multiChoiceQuestionModel.CorrectAnswers,
                        Points = multiChoiceQuestionModel.Points
                    };
                    break;
                case UpdateImageSingleChoiceQuestionModel imageSingleChoiceQuestionModel:
                    {
                        var fileUploadResult = await fileService.StoreFilesAsync(imageSingleChoiceQuestionModel.Id, imageSingleChoiceQuestionModel.AppendOrReplaceFiles);
                        var options = (imageSingleChoiceQuestionModel.Options ?? []).ToList();

                        foreach (var item in fileUploadResult)
                        {
                            if (options.Count > item.Index)
                            {
                                options[item.Index] = item.Result;
                            }
                            else
                            {
                                options.Add(item.Result);
                            }
                        }

                        question = new RepositoryImageSingleChoiceQuestionModel()
                        {
                            Id = questionModel.Id,
                            Question = imageSingleChoiceQuestionModel.Question,
                            Options = options.ToArray(),
                            CorrectAnswer = imageSingleChoiceQuestionModel.CorrectAnswer,
                            Points = imageSingleChoiceQuestionModel.Points
                        };
                    }
                    break;
                case UpdateImageMultiChoiceQuestionModel multiChoiceQuestionModel:
                    {
                        var fileUploadResult = await fileService.StoreFilesAsync(multiChoiceQuestionModel.Id, multiChoiceQuestionModel.AppendOrReplaceFiles);
                        var options = (multiChoiceQuestionModel.Options ?? []).ToList();

                        foreach (var item in fileUploadResult)
                        {
                            if (options.Count > item.Index)
                            {
                                options[item.Index] = item.Result;
                            }
                            else
                            {
                                options.Add(item.Result);
                            }
                        }
                        question = new RepositoryImageMultiChoiceQuestionModel()
                        {
                            Id = questionModel.Id,
                            Question = multiChoiceQuestionModel.Question,
                            Options = options.ToArray(),
                            CorrectAnswers = multiChoiceQuestionModel.CorrectAnswers,
                            Points = multiChoiceQuestionModel.Points
                        };
                    }

                    break;
                default:
                    throw new NotSupportedException();
            }
            return question;
        }

        private QuestionModel Map(RepositoryQuestionModel model)
        {
            return model.Type switch
            {
                Repositories.Models.QuestionType.Essay => mapper.Map<EssayQuestionModel>(model),
                Repositories.Models.QuestionType.Binary => mapper.Map<BinaryQuestionModel>(model),
                Repositories.Models.QuestionType.MultiChoice => mapper.Map<MultiChoiceQuestionModel>(model),
                Repositories.Models.QuestionType.SingleChoice => mapper.Map<SingleChoiceQuestionModel>(model),
                Repositories.Models.QuestionType.ImageMultiChoice => mapper.Map<ImageMultiChoiceQuestionModel>(model),
                Repositories.Models.QuestionType.ImageSingleChoice => mapper.Map<ImageSingleChoiceQuestionModel>(model),
                _ => throw new NotSupportedException(),
            };
        }
    }
}