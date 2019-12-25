using System.Runtime.InteropServices;

namespace DH.MUI.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left, top, right, bottom;
    }
}
