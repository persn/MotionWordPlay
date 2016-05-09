namespace NTNU.MotionWordPlay.UserInterface
{
    using Controls;

    public interface IUserInterface : IGameLoop
    {
        TextLine Time { get; set; }
        TextLine Task { get; set; }
        TextLine Score { get; set; }

        void AddNewPuzzleFractions(int amount);
        void UpdatePuzzleFraction(int index, string text);
        void UpdatePuzzleFraction(int index, int x, int y);
        void UpdatePuzzleFraction(int index, string text, int x, int y);
        void ResetUI();
    }
}
