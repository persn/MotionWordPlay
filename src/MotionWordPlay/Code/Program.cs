namespace NTNU.MotionWordPlay
{
    using System;

    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            using (Game game = new Game())
            {
                game.Run();
            }
        }
    }
}
