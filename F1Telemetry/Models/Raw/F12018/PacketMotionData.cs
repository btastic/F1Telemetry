using System.Runtime.InteropServices;

namespace F1Telemetry.Models.Raw.F12018
{
    /// <summary>
    /// Frequency: Rate as specified in menus
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketMotionData
    {
        /// <summary>
        /// Header
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public PacketHeader Header;

        /// <summary>
        /// Data for all cars on track
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CarMotionData[] CarMotionData;

        /// <summary>
        /// Note: All wheel arrays have the following order:
        /// RL, RR, FL, FR
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] SuspensionPosition;

        /// <summary>
        /// RL, RR, FL, FR
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] SuspensionVelocity;

        /// <summary>
        /// RL, RR, FL, FR
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] SuspensionAcceleration;

        /// <summary>
        /// Speed of each wheel
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] WheelSpeed;

        /// <summary>
        /// Slip ratio for each wheel
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] WheelSlip;

        /// <summary>
        /// Velocity in local space
        /// </summary>
        public float LocalVelocityX;

        /// <summary>
        /// Velocity in local space
        /// </summary>
        public float LocalVelocityY;

        /// <summary>
        /// Velocity in local space
        /// </summary>
        public float LocalVelocityZ;

        /// <summary>
        /// Angular velocity x-component
        /// </summary>
        public float AngularVelocityX;

        /// <summary>
        /// Angular velocity y-component
        /// </summary>
        public float AngularVelocityY;

        /// <summary>
        /// Angular velocity z-component
        /// </summary>
        public float AngularVelocityZ;

        /// <summary>
        /// Angular acceleration x-component
        /// </summary>
        public float AngularAccelerationX;

        /// <summary>
        /// Angular acceleration y-component
        /// </summary>
        public float AngularAccelerationY;

        /// <summary>
        /// Angular acceleration z-component
        /// </summary>
        public float AngularAccelerationZ;

        /// <summary>
        /// Current front wheels angle in radians
        /// </summary>
        public float FrontWheelsAngle;
    }
}
