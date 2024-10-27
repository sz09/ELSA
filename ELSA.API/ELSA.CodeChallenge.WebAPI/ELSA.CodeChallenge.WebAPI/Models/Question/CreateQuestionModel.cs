namespace ELSA.WebAPI.Models.Question
{
    public enum QuestionType
    {
        Essay = 1, // Short text answer question
        Binary = 2, // Yes no question
        MultiChoice = 3, // For multi options question
        SingleChoice = 4, // For single choice question
        ImageSingleChoice = 5, // For multi options question but use image
        ImageMultiChoice = 6, // For single choice question but use image
    }
    public abstract class CreateQuestionModel
    {
        public string Question { get; set; }
        public QuestionType Type { get; set; }
        public double Points { get; set; }
    }

    public class CreateMultiChoiceQuestionModel : CreateQuestionModel
    {
        public CreateMultiChoiceQuestionModel()
        {
            Type = QuestionType.MultiChoice;
        }

        public string[] Options { get; set; }
        public int[] CorrectAnswers { get; set; }
    }

    public class CreateSingleChoiceQuestionModel : CreateQuestionModel
    {
        public CreateSingleChoiceQuestionModel()
        {
            Type = QuestionType.SingleChoice;
        }

        public string[] Options { get; set; }
        public int CorrectAnswer { get; set; }
    }

    public class CreateImageMultiChoiceQuestionModel : CreateQuestionModel
    {
        public CreateImageMultiChoiceQuestionModel()
        {
            Type = QuestionType.ImageMultiChoice;
        }

        public IFormFile[] Files { get; set; }
        public int[] CorrectAnswers { get; set; }
    }

    public class CreateImageSingleChoiceQuestionModel : CreateQuestionModel
    {
        public CreateImageSingleChoiceQuestionModel()
        {
            Type = QuestionType.ImageSingleChoice;
        }

        public IFormFile[] Files { get; set; }
        public int CorrectAnswer { get; set; }
    }

    public class CreateEssayQuestionModel : CreateQuestionModel
    {
        public CreateEssayQuestionModel()
        {
            Type = QuestionType.Essay;
        }
        public string CorrectAnswer { get; set; }
        public bool IgnoreCase { get; set; }
        public bool AnyAnswer { get; set; }
    }

    public class CreateBinaryQuestionModel : CreateQuestionModel
    {
        public CreateBinaryQuestionModel()
        {
            Type = QuestionType.Binary;
        }
        public bool CorrectAnswer { get; set; }
    }
}
