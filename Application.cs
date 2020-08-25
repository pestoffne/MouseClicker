// Encoding: UTF-8
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

static class Application
{
  [DllImport("user32.dll")]
  static extern short GetKeyState(Keys k);

  static bool IsPressed(Keys k)
  {
    return GetKeyState(k) < 0;
  }

  [DllImport("user32.dll",
    CharSet=CharSet.Auto,
    CallingConvention=CallingConvention.StdCall)]
  static extern void mouse_event(uint dwFlags,
    uint dx, uint dy, uint cButtons, uint dwExtraInfo);

  const int MOUSEEVENTF_LEFTDOWN = 0x02;
  const int MOUSEEVENTF_LEFTUP = 0x04;
  const int MOUSEEVENTF_RIGHTDOWN = 0x08;
  const int MOUSEEVENTF_RIGHTUP = 0x10;

  static bool is_left_down = false;

  static void MouseLeftDown()
  {
    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
    is_left_down = true;
  }

  static void MouseLeftUp()
  {
    is_left_down = false;
    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
  }

  static void Main()
  {
    Int16 state = 0;
    Int16 phase = 0;

    Console.WriteLine(
        "Use \"]\" button to turn on/off auto clicker");

    while (true) {
      bool kp_brc = IsPressed(Keys.OemCloseBrackets); // "]"

      if (state == 0) {
        if (kp_brc) {
          state = 1;
        }
      } else if (state == 1) {
        if (!kp_brc) {
          state = 2;
          phase = 0;
          Console.WriteLine("On");
        }
      } else if (state == 2) {
        if (kp_brc) {
          state = 3;
        }

        if (phase == 0) {
          MouseLeftDown();
        } else if (phase == 4) {
          MouseLeftUp();
        }

        if (phase >= 8) {
          phase = 0;
        } else {
          phase++;
        }
      } else if (state == 3) {
        if (!kp_brc) {
          state = 0;
          Console.WriteLine("Off");
        }
        if (is_left_down) {
          MouseLeftUp();
        }
      }
      //Console.WriteLine("{0} {1}", state, phase);
      Thread.Sleep(25);
    }
  }
}
