using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using KeyEventHandler = System.Windows.Forms.KeyEventHandler;

namespace TheBindingOfRandom
{
    #region  Generated Code
    public class Keylogger
    {
        private const int WHKEYBOARDLL = 13;
        private const int WMKEYDOWN = 0x100;
        private const int WMKEYUP = 0x101;

        private static readonly byte[] DistinctVirtualKeys = Enumerable.Range(0, 256).Select(KeyInterop.KeyFromVirtualKey).Where(item => item != Key.None).Distinct().Select(item => (byte)KeyInterop.VirtualKeyFromKey(item)).ToArray();

        static Keylogger()
        {
            KHP = HookProc;
            Start();
        }

        private delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHookStruct lParam);

        public static event KeyEventHandler KeyDown;

        public static event KeyEventHandler KeyUp;

        public static List<Keys> HookedKeys { get; } = new List<Keys>();
        private static IntPtr Hhook { get; set; } = IntPtr.Zero;
        private static KeyboardHookProc KHP { get; }

        private static List<Keys> LastKeysPressed { get; } = new List<Keys>();

        #region  Generated Code

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookStruct lParam);

        [DllImport("user32.dll")]
        private static extern int GetAsyncKeyState(int i);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        #endregion

        public static bool IsKeyDown(Keys key)
        {
            var asyncKeyState = GetAsyncKeyState((int)key);
            return asyncKeyState == 1 || asyncKeyState == -32767;
        }

        public static void PostKey(Keys key)
        {
            var handle = GetForegroundWindow();
            PostMessage(handle, WMKEYDOWN, new IntPtr((int)key), new IntPtr(0));
            Task.Delay(50).Wait();
            PostMessage(handle, WMKEYUP, new IntPtr((int)key), new IntPtr(0));
        }

        public static void Start()
        {
            IntPtr hInstance = LoadLibrary("User32");
            Hhook = SetWindowsHookEx(WHKEYBOARDLL, KHP, hInstance, 0);
        }

        public static void Stop()
        {
            UnhookWindowsHookEx(Hhook);
        }

        private static int HookProc(int code, int wParam, ref KeyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                if (wParam == WMKEYDOWN || wParam == WMKEYUP)
                {
                    byte[] keyboardState = new byte[256];
                    int result = GetKeyboardState(keyboardState);
                    var downKeys = (from virtualKey in DistinctVirtualKeys
                                    where (GetKeyState(virtualKey) & 0x80) != 0
                                    select (Keys)KeyInterop.VirtualKeyFromKey(KeyInterop.KeyFromVirtualKey(virtualKey))).ToList();
                    foreach (var downKey in downKeys)
                    {
                        if (!LastKeysPressed.Contains(downKey))
                        {
                            KeyDown?.Invoke(null, new KeyEventArgs(downKey));
                            LastKeysPressed.Add(downKey);
                        }
                    }
                    List<Keys> keysToRemove = new List<Keys>();
                    foreach (var key in LastKeysPressed)
                    {
                        if (!downKeys.Contains(key))
                        {
                            KeyUp?.Invoke(null, new KeyEventArgs(key));
                            keysToRemove.Add(key);
                        }
                    }
                    LastKeysPressed.RemoveAll(k => keysToRemove.Contains(k));
                }
            }
            return CallNextHookEx(Hhook, code, wParam, ref lParam);
        }

        private struct KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
    }

    #endregion
}