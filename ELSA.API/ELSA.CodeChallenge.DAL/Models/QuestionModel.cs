using ELSA.DAL.Models.Base;

namespace ELSA.Repositories.Models
{
    public interface ImageQuestion
    {
        string[] Options { get; set; }
    }
    public abstract class QuestionModel : BaseAuditEntity
    {
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

    public class MultiChoiceQuestionModel: QuestionModel
    {
        public MultiChoiceQuestionModel()
        {
            Type = QuestionType.MultiChoice;
        }
        public string[] Options { get; set; } = [];
        public int[] CorrectAnswers { get; set; } = [];
    }

    public class SingleChoiceQuestionModel: QuestionModel
    {
        public SingleChoiceQuestionModel()
        {
            Type = QuestionType.SingleChoice;
        }
        public string[] Options { get; set; } = [];
        public int CorrectAnswer { get; set; }
    }

    public class ImageMultiChoiceQuestionModel : QuestionModel, ImageQuestion
    {
        public ImageMultiChoiceQuestionModel()
        {
            Type = QuestionType.ImageMultiChoice;
        }
        public string[] Options { get; set; } = [];
        public int[] CorrectAnswers { get; set; }
    }

    public class ImageSingleChoiceQuestionModel : QuestionModel, ImageQuestion
    {
        public ImageSingleChoiceQuestionModel()
        {
            Type = QuestionType.ImageSingleChoice;
        }
        public string[] Options { get; set; } = [];
        public int CorrectAnswer { get; set; }
    }
    
    public class BinaryQuestionModel : QuestionModel
    {
        public BinaryQuestionModel()
        {
            Type = QuestionType.Binary;
        }
        public bool CorrectAnswer { get; set; }
    }

    public class EssayQuestionModel: QuestionModel
    {
        public EssayQuestionModel()
        {
            Type = QuestionType.Essay;
        }
        public bool IgnoreCase { get; set; } = true;
        public bool AnyAnswer { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
