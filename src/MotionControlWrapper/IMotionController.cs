using System.Drawing;

namespace NTNU.MotionControlWrapper
{
    public interface IMotionController
    {
        /// <summary>
        /// Getter for the size properties of the color frames
        /// </summary>
        Size ColorFrameSize { get; }

        /// <summary>
        /// Getter for the size properties of the depth frames
        /// </summary>
        Size DepthFrameSize { get; }

        /// <summary>
        /// Getter for the size properties of the infrared frames
        /// </summary>
        Size InfraredFrameSize { get; }

        /// <summary>
        /// HURR DURR
        /// </summary>
        Size SilhouetteFrameSize { get; }

        /// <summary>
        /// Acquire the most recent color frame produced by the MotionController
        /// </summary>
        /// <returns></returns>
        byte[] AcquireLatestColorFrame();

        /// <summary>
        /// Acquire the most recent depth frame produced by the MotionController
        /// </summary>
        /// <returns></returns>
        byte[] AcquireLatestDepthFrame();

        /// <summary>
        /// Acquire the most recent infrared frame produced by the MotionController
        /// </summary>
        /// <returns></returns>
        byte[] AcquireLatestInfraredFrame();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        byte[] AcquireLatestSilhouetteFrame();
    }
}
