namespace VKDiplom
{
    public static class WindowsMediaColorExtensions
    {
        public static Microsoft.Xna.Framework.Color WindowsColorToXnaColor(this System.Windows.Media.Color color)
        {
            return Microsoft.Xna.Framework.Color.FromNonPremultiplied(color.R, color.G, color.B, color.A);
        }
    }

    public static class XnaMediaColorExtensions
    {
        public static System.Windows.Media.Color WindowsColorToXnaColor(this Microsoft.Xna.Framework.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.R, color.G, color.B, color.A);
        }
    }
}
