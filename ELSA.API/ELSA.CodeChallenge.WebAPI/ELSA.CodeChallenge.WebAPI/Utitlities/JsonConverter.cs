using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ELSA.WebAPI.Models.Question;

namespace ELSA.Services.Utils
{
    public class QuestionSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(CreateQuestionModel).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null;
            return base.ResolveContractConverter(objectType);
        }
    }

    public class CreateQuestionConverter : JsonConverter
    {
        static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new QuestionSpecifiedConcreteClassConverter() };

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(CreateQuestionModel));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            switch ((QuestionType)jo["Type"].Value<int>())
            {
                case QuestionType.Essay:
                    return JsonConvert.DeserializeObject<CreateEssayQuestionModel>(jo.ToString(), SpecifiedSubclassConversion);
                case QuestionType.Binary:
                    return JsonConvert.DeserializeObject<CreateBinaryQuestionModel>(jo.ToString(), SpecifiedSubclassConversion);
                case QuestionType.MultiChoice:
                    return JsonConvert.DeserializeObject<CreateMultiChoiceQuestionModel>(jo.ToString(), SpecifiedSubclassConversion);
                case QuestionType.SingleChoice:
                    return JsonConvert.DeserializeObject<CreateSingleChoiceQuestionModel>(jo.ToString(), SpecifiedSubclassConversion);
                case QuestionType.ImageMultiChoice:
                    return JsonConvert.DeserializeObject<CreateImageMultiChoiceQuestionModel>(jo.ToString(), SpecifiedSubclassConversion);
                case QuestionType.ImageSingleChoice:
                    return JsonConvert.DeserializeObject<CreateImageSingleChoiceQuestionModel>(jo.ToString(), SpecifiedSubclassConversion);
                default:
                    throw new Exception();
            }
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }
    }
}
