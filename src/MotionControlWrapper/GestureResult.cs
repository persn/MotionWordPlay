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

        public string Name { get; }

        public float Confidence { get; }

        public bool IsDetected { get; }
    }
}
