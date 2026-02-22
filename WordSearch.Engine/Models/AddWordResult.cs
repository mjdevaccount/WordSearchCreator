namespace WordSearch.Engine.Models
{
    public class AddWordResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public static AddWordResult Ok() => new AddWordResult { Success = true };
        public static AddWordResult Failed(string message) => new AddWordResult { Success = false, ErrorMessage = message };
    }
}
