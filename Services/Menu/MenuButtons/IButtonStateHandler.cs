namespace Bible_Blazer_PWA.Services.Menu
{
    public interface IButtonStateHandler
    {
        int StatesCount { get; }
        string Key { get; }
        void Handle(int state);
        string GetIcon(int state);
    }
}
