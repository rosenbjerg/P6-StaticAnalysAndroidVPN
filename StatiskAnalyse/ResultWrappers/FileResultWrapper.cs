namespace StatiskAnalyse
{
    public class FileResultWrapper : IResult
    {
        public string File { get; set; }
        public int Line { get; set; }
        public int Index { get; set; }
    }

    public interface IRegexResult
    {
        
    }

    public interface IResult
    {
        
    }
}