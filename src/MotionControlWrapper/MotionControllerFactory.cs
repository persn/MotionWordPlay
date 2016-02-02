namespace NTNU.MotionControlWrapper
{
    using System;
    using Controllers;

    public static class MotionControllerFactory
    {
        public static IMotionController CreateMotionController(MotionControllerAPI api)
        {
            switch (api)
            {
                case MotionControllerAPI.Kinectv2:
                    return new Kinectv2();
                default:
                    throw new NotSupportedException(
                        "Attempted to make a motion controller that is not supported by the" +
                        "framework");
            }
        }
    }
}
