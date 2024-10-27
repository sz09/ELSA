using ELSA.WebAPI.Models.Question;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using System.Runtime.CompilerServices;

namespace ELSA.WebAPI.ModelBinders
{

    public class CustomQuestionBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(CreateQuestionModel))
            {
                return new CreateQuestionModelBinder();
            }
            else if (context.Metadata.ModelType == typeof(UpdateQuestionModel))
            {
                return new UpdateQuestionModelBinder();
            }
            return null;
        }
    }

    public class CreateQuestionModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var form = bindingContext.HttpContext.Request.Form;

            if (form.TryGetValue(nameof(CreateQuestionModel.Type), out var typeAsString) && int.TryParse(typeAsString, out var typeAsInt))
            {
                if (Enum.IsDefined(typeof(QuestionType), typeAsInt))
                {
                    CreateQuestionModel? createQuestionModel = null;
                    switch ((QuestionType)typeAsInt)
                    {
                        case QuestionType.Essay:
                            createQuestionModel = new CreateEssayQuestionModel
                            {
                                CorrectAnswer = form["correctAnswer"].ToString(),
                                AnyAnswer =  form.TryParseBool("anyAnswer"),
                                IgnoreCase = form.TryParseBool("ignoreCase"),
                                Question = form["question"].ToString(),
                                Points = form.TryParsePoint()
                            };
                            break;
                        case QuestionType.Binary:
                            {
                                if (bool.TryParse(form["correctAnswer"], out bool correctAnswer))
                                {
                                    createQuestionModel = new CreateBinaryQuestionModel
                                    {
                                        CorrectAnswer = correctAnswer,
                                        Question = form["question"].ToString(),
                                        Points = form.TryParsePoint()
                                    };
                                }
                                else
                                {
                                    throw new InvalidDataException("correctAnswer must be type of bool");
                                }
                            }
                            break;

                        case QuestionType.SingleChoice:
                            {
                                if (int.TryParse(form["correctAnswer"], out int correctAnswer))
                                {
                                    createQuestionModel = new CreateSingleChoiceQuestionModel
                                    {

                                        CorrectAnswer = correctAnswer,
                                        Question = form["question"].ToString(),
                                        Options = form["options"].ToString().Split(','),
                                        Points = form.TryParsePoint()
                                    };
                                }
                                else
                                {
                                    throw new InvalidDataException("correctAnswer must be type of int");
                                }
                            }
                            break;
                        case QuestionType.MultiChoice:
                            {
                                var correctAnswers = form["correctAnswers"].ToString().Split(',').Select(d =>
                                {
                                    if (!int.TryParse(d, out int correctAnswer))
                                    {
                                        throw new InvalidDataException("correctAnswer must be type of int");
                                    }

                                    return correctAnswer;
                                }).ToArray();
                                createQuestionModel = new CreateMultiChoiceQuestionModel
                                {
                                    CorrectAnswers = correctAnswers,
                                    Question = form["question"].ToString(),
                                    Options = form["options"].ToString().Split(','),
                                    Points = form.TryParsePoint()
                                };
                            }
                            break;
                        case QuestionType.ImageSingleChoice:
                            {
                                if (int.TryParse(form["correctAnswer"], out int correctAnswer))
                                {
                                    createQuestionModel = new CreateImageSingleChoiceQuestionModel
                                    {
                                        CorrectAnswer = correctAnswer,
                                        Question = form["question"].ToString(),
                                        Files = [.. form.Files],
                                        Points = form.TryParsePoint()
                                    };
                                }

                            }
                            break;
                        case QuestionType.ImageMultiChoice:
                            {
                                var correctAnswers = form["correctAnswers"].ToString().Split(',').Select(d =>
                                {
                                    if (!int.TryParse(d, out int correctAnswer))
                                    {
                                        throw new InvalidDataException("correctAnswer must be type of int");
                                    }

                                    return correctAnswer;
                                }).ToArray();

                                createQuestionModel = new CreateImageMultiChoiceQuestionModel
                                {
                                    CorrectAnswers = correctAnswers,
                                    Question = form["question"].ToString(),
                                    Files = [.. form.Files],
                                    Points = form.TryParsePoint()
                                };
                            }
                            break;
                    }

                    bindingContext.Result = ModelBindingResult.Success(createQuestionModel);
                }
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }
    }

    public class UpdateQuestionModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var form = bindingContext.HttpContext.Request.Form;

            if (form.TryGetValue(nameof(UpdateQuestionModel.Type), out var typeAsString) && int.TryParse(typeAsString, out var typeAsInt))
            {
                if (Enum.IsDefined(typeof(QuestionType), typeAsInt))
                {
                    UpdateQuestionModel? updateQuestionModel = null;
                    switch ((QuestionType)typeAsInt)
                    {
                        case QuestionType.Essay:
                            updateQuestionModel = new UpdateEssayQuestionModel
                            {
                                Id = ObjectId.Parse(form["id"]),
                                CorrectAnswer = form["correctAnswer"].ToString(),
                                Question = form["question"].ToString(),
                                AnyAnswer = form.TryParseBool("anyAnswer"),
                                IgnoreCase = form.TryParseBool("ignoreCase"),
                                Points = form.TryParsePoint()
                            };
                            break;
                        case QuestionType.Binary:
                            {
                                if (bool.TryParse(form["correctAnswer"], out bool correctAnswer))
                                {
                                    updateQuestionModel = new UpdateBinaryQuestionModel
                                    {
                                        Id = ObjectId.Parse(form["id"]),
                                        CorrectAnswer = correctAnswer,
                                        Question = form["question"].ToString(),
                                        Points = form.TryParsePoint()
                                    };
                                }
                                else
                                {
                                    throw new InvalidDataException("correctAnswer must be type of bool");
                                }
                            }
                            break;

                        case QuestionType.SingleChoice:
                            {
                                if (int.TryParse(form["correctAnswer"], out int correctAnswer))
                                {
                                    updateQuestionModel = new UpdateSingleChoiceQuestionModel
                                    {
                                        Id = ObjectId.Parse(form["id"]),
                                        CorrectAnswer = correctAnswer,
                                        Question = form["question"].ToString(),
                                        Options = form["options"].ToString().Split(','),
                                        Points = form.TryParsePoint()
                                    };
                                }
                                else
                                {
                                    throw new InvalidDataException("correctAnswer must be type of int");
                                }
                            }
                            
                            break;
                        case QuestionType.MultiChoice:
                            {
                                var correctAnswers = form["correctAnswers"].ToString().Split(',').Select(d =>
                                {
                                    if (!int.TryParse(d, out int correctAnswer))
                                    {
                                        throw new InvalidDataException("correctAnswer must be type of int");
                                    }

                                    return correctAnswer;
                                }).ToArray();
                                updateQuestionModel = new UpdateMultiChoiceQuestionModel
                                {
                                    Id = ObjectId.Parse(form["id"]),
                                    CorrectAnswers = correctAnswers,
                                    Question = form["question"].ToString(),
                                    Options = form["options"].ToString().Split(','),
                                    Points = form.TryParsePoint()
                                };
                            }

                            break;
                        case QuestionType.ImageMultiChoice:
                            {
                                var correctAnswers = form["correctAnswers"].ToString().Split(',').Select(d =>
                                {
                                    if (!int.TryParse(d, out int correctAnswer))
                                    {
                                        throw new InvalidDataException("correctAnswer must be type of int");
                                    }

                                    return correctAnswer;
                                }).ToArray();

                                updateQuestionModel = new UpdateImageMultiChoiceQuestionModel
                                {
                                    Id = ObjectId.Parse(form["id"]),
                                    CorrectAnswers = correctAnswers,
                                    Question = form["question"].ToString(),
                                    Options = form["options"].ToString().Split(','),
                                    Points = form.TryParsePoint(),
                                    AppendOrReplaceFiles = form.ExtractAppendOrReplaceFiles()
                                };
                            }
                            break;
                        case QuestionType.ImageSingleChoice:
                            {
                                if (int.TryParse(form["correctAnswer"], out int correctAnswer))
                                {
                                    updateQuestionModel = new UpdateImageSingleChoiceQuestionModel
                                    {
                                        Id = ObjectId.Parse(form["id"]),
                                        CorrectAnswer = correctAnswer,
                                        Question = form["question"].ToString(),
                                        Options = form["options"].ToString().Split(','),
                                        Points = form.TryParsePoint(),
                                        AppendOrReplaceFiles = form.ExtractAppendOrReplaceFiles()
                                    };
                                }
                            }
                            break;
                    }

                    bindingContext.Result = ModelBindingResult.Success(updateQuestionModel);
                }
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }
    }

    public static class FormExtensions
    {
        internal static double TryParsePoint(this IFormCollection form, string key = "points")
        {
            if(!double.TryParse(form[key], out var value))
            {
                throw new InvalidDataException($"{key} must be type of double");
            }

            return value;
        }

        internal static bool TryParseBool(this IFormCollection form, string key)
        {
            if(!bool.TryParse(form[key], out var value))
            {
                throw new InvalidDataException($"{key} must be type of bool");
            }

            return value;
        }

        internal static Dictionary<int, IFormFile> ExtractAppendOrReplaceFiles(this IFormCollection form)
        {
            if (form.Files?.Count == 0)
            {
                return new();
            }

            Dictionary<int, IFormFile> result = [];
            var files = form.Files.ToList();
            foreach (var file in files)
            {
                result.Add(int.Parse(file.Name), file);
            }

            return result;
        }
    }
}
