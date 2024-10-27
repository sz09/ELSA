using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace ELSA.Services.Models.Questions
{
    public abstract class QuestionModel
    {
        public ObjectId Id { get; set; }
        public string Question { get; set; }
        public QuestionType Type { get; set; }
        public double Points { get; set; }
    }

    public enum QuestionType
    {
        Essay = 1, // Short text answer question
        Binary = 2, // Yes no question
        MultiChoice = 3, // For multi options question
        SingleChoice = 4, // For single choice question
        ImageSingleChoice = 5, // For multi options question but use image
        ImageMultiChoice = 6, // For single choice question but use image
    }

    public class MultiChoiceQuestionModel : QuestionModel
    {
        public MultiChoiceQuestionModel()
        {
            Type = QuestionType.MultiChoice;
        }
        public string[] Options { get; set; } = [];
        public int[] CorrectAnswers { get; set; }
    }

    public class SingleChoiceQuestionModel : QuestionModel
    {
        public SingleChoiceQuestionModel()
        {
            Type = QuestionType.SingleChoice;
        }
        public string[] Options { get; set; } = [];
        public int CorrectAnswer { get; set; }
    }
    

    public class ImageMultiChoiceQuestionModel : QuestionModel
    {
        public ImageMultiChoiceQuestionModel()
        {
            Type = QuestionType.ImageMultiChoice;
        }
        public string[] Options { get; set; } = [];
        public int[] CorrectAnswers { get; set; }
    }
    

    public class CreateImageMultiChoiceQuestionModel : ImageMultiChoiceQuestionModel
    {
        public IFormFile[] Files { get; set; } = [];
    }

    public class UpdateImageMultiChoiceQuestionModel : ImageMultiChoiceQuestionModel
    {
        public  Dictionary<int, IFormFile> AppendOrReplaceFiles { get; set; } = [];
    }

    public class ImageSingleChoiceQuestionModel : QuestionModel
    {
        public ImageSingleChoiceQuestionModel()
        {
            Type = QuestionType.ImageSingleChoice;
        }
        public string[] Options { get; set; } = [];
        public int CorrectAnswer { get; set; }
    }

    public class CreateImageSingleChoiceQuestionModel : ImageSingleChoiceQuestionModel
    {
        public IFormFile[] Files { get; set; } = [];
    }

    public class UpdateImageSingleChoiceQuestionModel : ImageSingleChoiceQuestionModel
    {
        public Dictionary<int, IFormFile> AppendOrReplaceFiles { get; set; } = [];
    }

    public class EssayQuestionModel : QuestionModel
    {
        public EssayQuestionModel()
        {
            Type = QuestionType.Essay;
        }
        public string CorrectAnswer { get; set; }
        public bool IgnoreCase { get; set; }
        public bool AnyAnswer { get; set; }
    }

    public class BinaryQuestionModel : QuestionModel
    {
        public BinaryQuestionModel()
        {
            Type = QuestionType.Binary;
        }
        public bool CorrectAnswer { get; set; }
    }
}
