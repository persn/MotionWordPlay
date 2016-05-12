namespace NTNU.MotionWordPlay.UserInterface
{
    using System.Collections.Generic;
    using Controls;

    public interface IUserInterface : IGameLoop
    {
        TextLine Time { get; set; }
        TextLine Task { get; set; }
        TextLine Score { get; set; }
        TextLine Status { get; set; }
        IList<PuzzleFraction> PuzzleFractions { get; set; }

        void AddNewPuzzleFractions(int amount);
        void ResetUI();
    }
}
