namespace WebApiCore.Models
{
    public interface IErrorCode
    {
        public string Code { get; }
        public string EnglishTranslation { get; }
        public int? HTTPStatusCode { get; }
    }
}
