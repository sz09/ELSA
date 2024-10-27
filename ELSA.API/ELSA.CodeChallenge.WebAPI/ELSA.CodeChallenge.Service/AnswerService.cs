using MongoDB.Bson;
using MongoDB.Driver;
using ELSA.Repositories.Interface;
using ELSA.Repositories.Models;
using ELSA.Services.Interface;
using ELSA.Services.Utils;
using Newtonsoft.Json.Linq;

namespace ELSA.Services
{
    public class AnswerService(IQuestionRepository questionRepository, IScoreService scoreService) : IAnswerService
    {
        public async Task<bool> AnswerAQuestionAsync(ObjectId questionId, object answer)
        {
            var question = await questionRepository.GetOrCacheByIdAsync(questionId);
            if (question == null)
            {
                throw new InvalidDataException($"Not a valid {nameof(question)} with id: {questionId}");
            }

            var isCorrect = DecideCorrectAswer(question, answer);
            if (isCorrect)
            {
                await scoreService.ScoreAsync(questionId, question.Points);
            }

            return isCorrect;
        }
        
        private bool DecideCorrectAswer(QuestionModel questionModel, object answer)
        {
            switch (questionModel)
            {
                case SingleChoiceQuestionModel singleChoiceQuestionModel:
                    return singleChoiceQuestionModel.CorrectAnswer.Equals(Convert.ToInt32(answer));  
                case ImageSingleChoiceQuestionModel imageSingleChoiceQuestionModel:
                    return imageSingleChoiceQuestionModel.CorrectAnswer.Equals(Convert.ToInt32(answer));  
                case MultiChoiceQuestionModel multiChoiceQuestionModel:
                    {
                        if (answer is JArray jArray)
                        {
                            var ints = jArray.Select(d => d.ToObject<int>()).ToArray();
                            return multiChoiceQuestionModel.CorrectAnswers.EqualsWithoutOrdering(ints);
                        }

                        return false;
                    }
                case ImageMultiChoiceQuestionModel imageMultiChoiceQuestionModel:
                    {
                        if (answer is JArray jArray)
                        {
                            var ints = jArray.Select(d => d.ToObject<int>()).ToArray();
                            return imageMultiChoiceQuestionModel.CorrectAnswers.EqualsWithoutOrdering(ints);
                        }
                        return false;
                    }
                case EssayQuestionModel essayQuestionModel:
                    if(essayQuestionModel.AnyAnswer)
                    {
                        return true;
                    }

                    return essayQuestionModel.CorrectAnswer.Equals(answer.ToString(), essayQuestionModel.IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.Ordinal); 
                case BinaryQuestionModel binaryQuestionModel:
                    return binaryQuestionModel.CorrectAnswer.Equals(answer); 
                default:
                    throw new NotSupportedException();
            }
        }
    }
}