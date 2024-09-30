namespace FakeUserDataGenerator.RequestModels
{
    public class GenerateRequest
    {
        public int Count {get; set;}
        public double Errors {get; set;}
        public int Seed {get; set;}
        public int Page {get; set;}
        public string Locale {get; set;}
    }
}
