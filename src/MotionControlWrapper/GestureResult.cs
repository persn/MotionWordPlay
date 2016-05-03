namespace NTNU.MotionControlWrapper
{
    public class GestureResult
    {
        public GestureResult(string name, float confidence, bool isDetected)
        {
            Name = name;
            Confidence = confidence;
            IsDetected = isDetected;
        }

        public string Name { get; private set; }

        public float Confidence { get; private set; }

        public bool IsDetected { get; private set; }
    }
}
