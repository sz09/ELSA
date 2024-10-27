using AutoMapper;
using ELSA.WebAPI.ViewModels;

namespace ELSA.WebAPI.AutoMappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MapToRead();
            MapToWrite();
        }


        void MapToRead()
        {
            CreateMap<Repositories.Models.QuizModel, ViewQuizModel>();

            CreateMap<Repositories.Models.MultiChoiceQuestionModel, ViewModels.Question.ViewMultiChoiceQuestionModel>();
            CreateMap<Repositories.Models.SingleChoiceQuestionModel, ViewModels.Question.ViewSingleChoiceQuestionModel>();
            CreateMap<Repositories.Models.ImageMultiChoiceQuestionModel, ViewModels.Question.ViewImageMultiChoiceQuestionModel>();
            CreateMap<Repositories.Models.ImageSingleChoiceQuestionModel, ViewModels.Question.ViewImageSingleChoiceQuestionModel>();
            CreateMap<Repositories.Models.EssayQuestionModel, ViewModels.Question.ViewEssayQuestionModel>();
            CreateMap<Repositories.Models.BinaryQuestionModel, ViewModels.Question.ViewBinaryQuestionModel>();

            CreateMap<Repositories.Models.MultiChoiceQuestionModel, Services.Models.Questions.MultiChoiceQuestionModel>();
            CreateMap<Repositories.Models.SingleChoiceQuestionModel, Services.Models.Questions.SingleChoiceQuestionModel>();
            CreateMap<Repositories.Models.ImageMultiChoiceQuestionModel, Services.Models.Questions.ImageMultiChoiceQuestionModel>();
            CreateMap<Repositories.Models.ImageSingleChoiceQuestionModel, Services.Models.Questions.ImageSingleChoiceQuestionModel>();
            CreateMap<Repositories.Models.EssayQuestionModel, Services.Models.Questions.EssayQuestionModel>();
            CreateMap<Repositories.Models.BinaryQuestionModel, Services.Models.Questions.BinaryQuestionModel>();
        }

        void MapToWrite()
        {
            CreateMap<Models.RegisterUserModel, Services.Models.Users.RegisterUserModel>();
            CreateMap<Models.Quiz.CreateQuizModel, Repositories.Models.QuizModel>();

            CreateMap<Models.Question.QuestionType, Services.Models.Questions.QuestionType>();
            CreateMap<Models.Question.CreateMultiChoiceQuestionModel, Services.Models.Questions.MultiChoiceQuestionModel>();
            CreateMap<Models.Question.CreateSingleChoiceQuestionModel, Services.Models.Questions.SingleChoiceQuestionModel>();
            CreateMap<Models.Question.CreateImageMultiChoiceQuestionModel, Services.Models.Questions.CreateImageMultiChoiceQuestionModel>();
            CreateMap<Models.Question.CreateImageSingleChoiceQuestionModel, Services.Models.Questions.CreateImageSingleChoiceQuestionModel>();
            CreateMap<Models.Question.CreateEssayQuestionModel, Services.Models.Questions.EssayQuestionModel>();
            CreateMap<Models.Question.CreateBinaryQuestionModel, Services.Models.Questions.BinaryQuestionModel>();

            CreateMap<Models.Question.UpdateMultiChoiceQuestionModel, Services.Models.Questions.MultiChoiceQuestionModel>();
            CreateMap<Models.Question.UpdateSingleChoiceQuestionModel, Services.Models.Questions.SingleChoiceQuestionModel>();
            CreateMap<Models.Question.UpdateImageMultiChoiceQuestionModel, Services.Models.Questions.UpdateImageMultiChoiceQuestionModel>();
            CreateMap<Models.Question.UpdateImageSingleChoiceQuestionModel, Services.Models.Questions.UpdateImageSingleChoiceQuestionModel>();
            CreateMap<Models.Question.UpdateEssayQuestionModel, Services.Models.Questions.EssayQuestionModel>();
            CreateMap<Models.Question.UpdateBinaryQuestionModel, Services.Models.Questions.BinaryQuestionModel>();

            CreateMap<Models.Quiz.UpdateQuizModel, Services.Models.Quizzes.UpdateQuizModel>();
        }
    }
}
