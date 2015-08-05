using System.Windows.Input;

namespace VKDiplom.Utils
{
    public class Keyboard
    {
        private static readonly bool[] KeyState = new bool[256];

        public static bool IsKeyDown(Key key)
        {
            return KeyState[(int) key];
        }

        public static void SetKeyDown(Key key, bool down)
        {
            KeyState[(int) key] = down;
        }
    }
}