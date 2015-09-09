using System.Windows;
using System.Windows.Graphics;

namespace VKDiplom.Engine.Utils
{
    public static class VkDiplomGraphicsInitializationUtils
    {
        //public static readonly bool IsHardwareAccelerated = GraphicsDeviceManager.Current.RenderMode != RenderMode.Hardware;

        public static bool IsHardwareAccelerated()
        {
            return GraphicsDeviceManager.Current.RenderMode == RenderMode.Hardware;
        }

        public static void ShowReport()
        {
            if (GraphicsDeviceManager.Current.RenderMode == RenderMode.Hardware)
                return;
            string message;
            switch (GraphicsDeviceManager.Current.RenderModeReason)
            {
                case RenderModeReason.Not3DCapable:
                    message = "You graphics hardware is not capable of displaying this page ";
                    break;
                case RenderModeReason.GPUAccelerationDisabled:
                    message = "Hardware graphics acceleration has not been enabled on this web page.\n\n" +
                              "Please notify the web site owner.";
                    break;
                case RenderModeReason.TemporarilyUnavailable:
                    message = "Your graphics hardware is temporarily unavailable.\n\n" +
                              "Try reloading the web page or restarting your browser.";
                    break;
                case RenderModeReason.SecurityBlocked:
                    message =
                        "You need to configure your system to allow this web site to display 3D graphics:\n\n" +
                        "  1. Right-click the page\n" +
                        "  2. Select 'Silverlight'\n" +
                        "     (The 'Microsoft Silverlight Configuration' dialog will be displayed)\n" +
                        "  3. Select the 'Permissions' tab\n" +
                        "  4. Find this site in the list and change its 3D Graphics permission from 'Deny' to 'Allow'\n" +
                        "  5. Click 'OK'\n" +
                        "  6. Reload the page";
                    break;
                default:
                    message = "Unknown error";
                    break;
            }
            MessageBox.Show(message, "3D Content Blocked", MessageBoxButton.OK);
        }
    }
}