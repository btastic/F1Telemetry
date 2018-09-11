using System.Runtime.InteropServices;

namespace F1Telemetry
{
    public static class StructUtility
    {
        public static T ConvertToPacket<T>(byte[] bytes) where T : struct
        {
            GCHandle gchandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T result = (T)Marshal.PtrToStructure(gchandle.AddrOfPinnedObject(), typeof(T));
            gchandle.Free();
            return result;
        }
    }
}
