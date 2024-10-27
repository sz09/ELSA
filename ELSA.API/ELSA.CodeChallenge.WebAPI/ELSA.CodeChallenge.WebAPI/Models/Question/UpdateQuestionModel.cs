using MongoDB.Bson;

namespace ELSA.WebAPI.Models.Question
{
    public abstract class UpdateQuestionModel
    {
        public ObjectId Id { get; set; }
        public string Question { get; set; }
        public QuestionType Type { get; set; }
        public double Points { get; set; }
    }

    public class UpdateMultiChoiceQuestionModel : UpdateQuestionModel
    {
        public UpdateMultiChoiceQuestionModel()
        {
            Type = QuestionType.MultiChoice;
        }

        public string[] Options { get; set; }
        public int[] CorrectAnswers { get; set; }
    }

    public class UpdateSingleChoiceQuestionModel : UpdateQuestionModel
    {
        public UpdateSingleChoiceQuestionModel()
        {
            Type = QuestionType.SingleChoice;
        }

        public int CorrectAnswer { get; set; }
        public string[] Options { get; set; }
    }

    public class UpdateImageMultiChoiceQuestionModel : UpdateQuestionModel
    {
        public UpdateImageMultiChoiceQuestionModel()
        {
            Type = QuestionType.ImageMultiChoice;
        }

        public int[] CorrectAnswers { get; set; }
        public string[] Options { get; set; }
        public Dictionary<int, IFormFile> AppendOrReplaceFiles { get; set; }
    }

    public class UpdateImageSingleChoiceQuestionModel : UpdateQuestionModel
    {
        public UpdateImageSingleChoiceQuestionModel()
        {
            Type = QuestionType.ImageSingleChoice;
        }

        public int CorrectAnswer { get; set; }
        public string[] Options { get; set; }
        public Dictionary<int, IFormFile> AppendOrReplaceFiles { get; set; }
    }

    public class UpdateEssayQuestionModel : UpdateQuestionModel
    {
        public UpdateEssayQuestionModel()
        {
            Type = QuestionType.Essay;
        }
        public string CorrectAnswer { get; set; }
        public bool IgnoreCase { get; set; }
        public bool AnyAnswer { get; set; }
    }

    public class UpdateBinaryQuestionModel : UpdateQuestionModel
    {
        public UpdateBinaryQuestionModel()
        {
            Type = QuestionType.Binary;
        }
        public bool CorrectAnswer { get; set; }
    }
}
