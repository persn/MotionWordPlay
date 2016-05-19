namespace NTNU.MotionControlWrapper
{
    using System.Collections.Generic;

    public class GestureResults
    {
        private readonly int _playerCount;
        private IList<GestureResult>[] _gestures;
        private int[] _xPositions;
        private int[] _yPositions;

        public GestureResults(int playerCount)
        {
            _playerCount = playerCount;
            _gestures = new IList<GestureResult>[_playerCount];
            _xPositions = new int[playerCount];
            _yPositions = new int[playerCount];

            for (int i = 0; i < _gestures.Length; i++)
            {
                _gestures[i] = new List<GestureResult>();
            }
        }

        public IList<GestureResult> GetGestures(int player)
        {
            return _gestures[player];
        }

        public GestureResult GetGesture(int player, int gesture)
        {
            return GetGestures(player)[gesture];
        }

        public void AddGestures(int player, IEnumerable<GestureResult> gestures)
        {
            foreach (GestureResult gestureResult in gestures)
            {
                _gestures[player].Add(gestureResult);
            }
        }

        public void SetXPosition(int player, int xPosition)
        {
            _xPositions[player] = xPosition;
        }

        public int GetXPosition(int player)
        {
            return _xPositions[player];
        }

        public void SetYPosition(int player, int yPosition)
        {
            _yPositions[player] = yPosition;
        }

        public int GetYPosition(int player)
        {
            return _yPositions[player];
        }

        public void Clear()
        {
            _gestures = new IList<GestureResult>[_playerCount];
        }

        public void Clear(int player)
        {
            _gestures[player].Clear();
        }
    }
}
