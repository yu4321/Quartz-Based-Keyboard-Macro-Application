using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace QuartzBaseMacroProgramWPF.Utils
{
    public class KeyboardInputs
    {

        public static void PressKey(int i)
        {
            new KeyboardSimulator(new InputSimulator()).KeyPress((VirtualKeyCode)i);
        }

        public static void PressKeyMulti(List<int> i)
        {
            int first = i.First();
            IEnumerable<int> bunch = from x in i where x != first select x;
            new KeyboardSimulator(new InputSimulator()).ModifiedKeyStroke((VirtualKeyCode)first, bunch.Cast<VirtualKeyCode>());
        }

        public static void PressKeyMulti(int i1, int i2)
        {
            new KeyboardSimulator(new InputSimulator()).ModifiedKeyStroke((VirtualKeyCode)i1, (VirtualKeyCode)i2);
        }

        public static int VKStringtoInt(string s)
        {
            try
            {
                if (s.Length > 1)
                {
                    return (int)Enum.Parse(typeof(VirtualKeyCode), s.ToUpper());
                }
                else
                {
                    return (int)Enum.Parse(typeof(VirtualKeyCode), ("VK_" + s).ToUpper());
                }
            }
            catch
            {
                return (int)Enum.Parse(typeof(VirtualKeyCode), s.ToUpper().Replace("VK_", ""));
            }
        }

    }
}
