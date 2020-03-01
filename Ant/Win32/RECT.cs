using System.Runtime.InteropServices;

namespace Ant.Wpf.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left, top, right, bottom;
    }
}
