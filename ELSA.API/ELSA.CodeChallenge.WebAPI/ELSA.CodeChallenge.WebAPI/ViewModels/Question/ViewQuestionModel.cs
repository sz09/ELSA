using MongoDB.Bson;
using ELSA.WebAPI.Models.Question;

namespace ELSA.WebAPI.ViewModels.Question
{
    public abstract class ViewQuestionModel
    {
        public ObjectId Id { get; set; }
        public string Question { get; set; }
        public string ImageUrl { get; set; }
        public QuestionType Type { get; set; }
        public double Points { get; set; }
    }

    public class ViewMultiChoiceQuestionModel : ViewQuestionModel
    {
        public ViewMultiChoiceQuestionModel()
        {
            Type = QuestionType.MultiChoice;
        }

        public string[] Options { get; set; }
        public int[] CorrectAnswers { get; set; }
    }

    public class ViewSingleChoiceQuestionModel : ViewQuestionModel
    {
        public ViewSingleChoiceQuestionModel()
        {
            Type = QuestionType.SingleChoice;
        }

        public string[] Options { get; set; }
        public int CorrectAnswer { get; set; }
    }

    public class ViewImageMultiChoiceQuestionModel : ViewQuestionModel
    {
        public ViewImageMultiChoiceQuestionModel()
        {
            Type = QuestionType.ImageMultiChoice;
        }

        public string[] Options { get; set; }
        public int[] CorrectAnswers { get; set; }
    }

    public class ViewImageSingleChoiceQuestionModel : ViewQuestionModel
    {
        public ViewImageSingleChoiceQuestionModel()
        {
            Type = QuestionType.ImageSingleChoice;
        }

        public string[] Options { get; set; }
        public int CorrectAnswer { get; set; }
    }

    public class ViewEssayQuestionModel : ViewQuestionModel
    {
        public ViewEssayQuestionModel()
        {
            Type = QuestionType.Essay;
        }
        public string CorrectAnswer { get; set; }
    }

    public class ViewBinaryQuestionModel : ViewQuestionModel
    {
        public ViewBinaryQuestionModel()
        {
            Type = QuestionType.Binary;
        }
        public bool CorrectAnswer { get; set; }
    }
}
