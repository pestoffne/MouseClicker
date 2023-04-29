// Encoding: UTF-8
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

class MouseCursor
{
  public MouseCursor()
  {
    _isLeftDown = false;
  }

  public void LeftDown()
  {
    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
    _isLeftDown = true;
  }

  public void LeftUp()
  {
    if (_isLeftDown) {
       mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
      _isLeftDown = false;
    }
  }

  public Point Position()
  {
    POINT pos;
    GetCursorPos(out pos);
    return (Point)pos;
  }

  public void Move(Point pos)
  {
    // TODO
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct POINT
  {
    public static implicit operator Point(POINT point)
    {
      return new Point(point._x, point._y);
    }

    private int _x;
    private int _y;
  }

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool GetCursorPos(out POINT lpPoint);

  [DllImport("user32.dll",
    CharSet=CharSet.Auto,
    CallingConvention=CallingConvention.StdCall)]
  private static extern void mouse_event(uint dwFlags,
    uint dx, uint dy, uint cButtons, uint dwExtraInfo);

  private const int MOUSEEVENTF_LEFTDOWN  = 0x02;
  private const int MOUSEEVENTF_LEFTUP    = 0x04;
  private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
  private const int MOUSEEVENTF_RIGHTUP   = 0x10;

  private bool _isLeftDown;
}

static class Application
{
  [DllImport("user32.dll")]
  static extern short GetKeyState(Keys k);

  static bool IsPressed(Keys k)
  {
    return GetKeyState(k) < 0;
  }

  static void Main()
  {
    MouseCursor mouseCursor = new MouseCursor();
    Point clickCursorPos = new Point();
    // Point() just for disable use uninitialized variable error
    Int16 state = 0;
    Int32 phase = 0;

    Console.WriteLine(
        "Use \"]\" button to turn on/off auto clicker");

    while (true) {
      bool kp_brc = IsPressed(Keys.OemCloseBrackets);  // "]"
      Point currentCursorPos = mouseCursor.Position();

      if (state == 0) {  // Idle state
        if (kp_brc) {
          state = 1;
        }
      } else if (state == 1) {  // Pre-click state
        if (!kp_brc) {
          state = 2;
          phase = 0;
          clickCursorPos = currentCursorPos;
          Console.WriteLine("On");
        }
      } else if (state == 2) {  // Click state
        if (kp_brc) {  // Turn off by key press
          state = 3;
          Console.WriteLine("Turn off by key press");
        }

        // Turn off by mouse move
        int dx = Math.Abs(clickCursorPos.X - currentCursorPos.X);
        int dy = Math.Abs(clickCursorPos.Y - currentCursorPos.Y);
        if (state == 2 && (dx > 10 || dy > 10)) {
          state = 3;
          Console.WriteLine("Turn off by mouse move");
        }

        if (state == 2 && phase == 0) {
          mouseCursor.LeftDown();
          //Console.WriteLine("Click to {0}", currentCursorPos);
        } else if (phase == 4) {
          mouseCursor.LeftUp();
        }

        //phase = (phase + 1) % 8;
        if (phase < 8) {
          phase++;
        } else {
          phase = 0;
        }
      } else if (state == 3) {  // Pre-idle state
        if (!kp_brc) {
          state = 0;
        }
        mouseCursor.LeftUp();
      }
      //Console.WriteLine("{0} {1}", state, phase);
      Thread.Sleep(50);
    }
  }
}
