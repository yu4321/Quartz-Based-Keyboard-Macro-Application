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

    }
}
