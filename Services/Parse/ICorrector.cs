namespace Bible_Blazer_PWA.Services.Parse
{
    public interface ICorrector
    {
        string ApplyHighLevelReplacements(string input, string unitId);
        string HandleBrackets(string stringToParse);
        string ReplaceBookNames(string stringToParse);
        IRegexHelper RegexHelper { get; }
    }
}