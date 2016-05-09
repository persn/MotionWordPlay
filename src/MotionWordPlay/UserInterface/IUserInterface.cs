namespace NTNU.MotionWordPlay.UserInterface
{
    public interface IUserInterface : IGameLoop
    {
        string Time { get; set; }
        string Task { get; set; }
        string Score { get; set; }

        void AddNewPuzzleFractions(int amount);
        void UpdatePuzzleFraction(int index, string text);
        void UpdatePuzzleFraction(int index, int x, int y);
        void UpdatePuzzleFraction(int index, string text, int x, int y);
        void ResetUI();
    }
}
