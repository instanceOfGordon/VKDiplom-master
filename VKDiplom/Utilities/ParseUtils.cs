using System;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace VKDiplom.Utilities
{
    public static class ParseUtils
    {
        public static string DecimalsToString(double[] numbers, string delimiter)
        {
            var sb = new StringBuilder();
            foreach (var number in numbers)
            {
                sb.Append(number);
                sb.Append(delimiter);
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static string CoordinatesToString(double[] x, double[] y,
            string delimiter)
        {
            return CoordinatesToString(x, 0, y, delimiter);
        }

        public static string CoordinatesToString(double[] x, int fromX,
            double[] y, string delimiter)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < y.Length; i++)
            {
                sb.Append(y[i]);
                sb.Append(":");
                sb.Append(x[fromX + i]);
                sb.Append(delimiter);
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static double[] StringOfDecimalsToArray(string numbers)
        {
            return
                numbers.Split(new[] {'\t', '\n', ' ', '\r', ';'},
                    StringSplitOptions.RemoveEmptyEntries)
                    .Select(double.Parse)
                    .ToArray();
        }

        public static void DoubleAccepted(object sender, KeyEventArgs e)
        {
            const int keycodeHyphenOnKeyboard = 189;
            const int keycodeDotOnKeyboard = 190;
            const int keycodeDotOnNumericKeyPad = 110;

            e.Handled = !(
                (!( //No modifier key must be pressed
                    (System.Windows.Input.Keyboard.Modifiers
                     & ModifierKeys.Shift) == ModifierKeys.Shift
                    || (System.Windows.Input.Keyboard.Modifiers
                        & ModifierKeys.Control) == ModifierKeys.Control
                    || (System.Windows.Input.Keyboard.Modifiers
                        & ModifierKeys.Alt) == ModifierKeys.Alt
                    )
                 && ( //only these keys are supported
                     (e.Key >= Key.D0 && e.Key <= Key.D9)
                     || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                     || e.Key == Key.Subtract || e.Key == Key.Add
                     || e.Key == Key.Unknown
                     || e.Key == Key.Home || e.Key == Key.End
                     || e.Key == Key.Delete
                     || e.Key == Key.Tab || e.Key == Key.Enter
                     || e.Key == Key.Escape || e.Key == Key.Back
                     || (e.Key == Key.Unknown && (
                         e.PlatformKeyCode == keycodeHyphenOnKeyboard
                         || e.PlatformKeyCode == keycodeDotOnKeyboard
                         || e.PlatformKeyCode == keycodeDotOnNumericKeyPad
                         )
                         )
                     )
                    )
                );
        }

        public static void IntAccepted(object sender, KeyEventArgs e)
        {
            e.Handled = !(
                (!( //No modifier key must be pressed
                    (System.Windows.Input.Keyboard.Modifiers
                     & ModifierKeys.Shift) == ModifierKeys.Shift
                    || (System.Windows.Input.Keyboard.Modifiers
                        & ModifierKeys.Control) == ModifierKeys.Control
                    || (System.Windows.Input.Keyboard.Modifiers
                        & ModifierKeys.Alt) == ModifierKeys.Alt
                    )
                 && ( //only these keys are supported
                     (e.Key >= Key.D0 && e.Key <= Key.D9)
                     || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                     || e.Key == Key.Subtract || e.Key == Key.Add
                     || e.Key == Key.Unknown
                     || e.Key == Key.Home || e.Key == Key.End
                     || e.Key == Key.Delete
                     || e.Key == Key.Tab || e.Key == Key.Enter
                     || e.Key == Key.Escape || e.Key == Key.Back
                     )
                    )
                );
        }
    }
}