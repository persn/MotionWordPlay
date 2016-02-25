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
        /// Getter for the size properties of the silhouette frames
        /// </summary>
        Size SilhouetteFrameSize { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        byte[] MostRecentColorFrame { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        byte[] MostRecentDepthFrame { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        byte[] MostRecentInfraredFrame { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        byte[] MostRecentSilhouetteFrame { get; }

        /// <summary>
        /// 
        /// </summary>
        void PollMostRecentColorFrame();

        /// <summary>
        /// 
        /// </summary>
        void PollMostRecentDepthFrame();

        /// <summary>
        /// 
        /// </summary>
        void PollMostRecentInfraredFrame();

        /// <summary>
        /// 
        /// </summary>
        void PollMostRecentSilhouetteFrame();
    }
}
