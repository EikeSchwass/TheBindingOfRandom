using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using KeyEventHandler = System.Windows.Forms.KeyEventHandler;

namespace TheBindingOfRandom
{
    public class Keylogger
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_KEYPRESS = 0x102;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;
        private const int WM_SYSKEYPRESS = 0x0106;

        /// <summary>
        /// windows virtual key codes
        /// </summary>
        private const byte VK_RETURN = 0X0D; //Enter

        private const byte VK_SPACE = 0X20; //Space
        private const byte VK_SHIFT = 0x10;
        private const byte VK_CAPITAL = 0x14;

        private static readonly byte[] DistinctVirtualKeys = Enumerable.Range(0, 256).Select(KeyInterop.KeyFromVirtualKey).Where(item => item != Key.None).Distinct().Select(item => (byte)KeyInterop.VirtualKeyFromKey(item)).ToArray();
        private IntPtr? isaacHandle;

        static Keylogger()
        {
            KHP = HookProc;
            Start();
        }

        /// <summary>
        /// Occurs when one of the hooked keys is pressed
        /// </summary>
        public static event KeyEventHandler KeyDown;

        /// <summary>
        /// Occurs when one of the hooked keys is released
        /// </summary>
        public static event KeyPressEventHandler KeyPress;

        /// <summary>
        /// Occurs when one of the hooked keys is released
        /// </summary>
        public static event KeyEventHandler KeyUp;

        public static List<Keys> HookedKeys { get; } = new List<Keys>();
        private static IntPtr Hhook { get; set; } = IntPtr.Zero;
        private static KeyboardHookProc KHP { get; }

        private static List<Keys> LastKeysPressed { get; } = new List<Keys>();

        public static bool IsKeyDown(Keys key)
        {
            var asyncKeyState = GetAsyncKeyState((int)key);
            return asyncKeyState == 1 || asyncKeyState == -32767;
        }

        public static void PostKey(Keys key)
        {
            var handle = GetForegroundWindow();
            PostMessage(handle, WM_KEYDOWN, new IntPtr((int)key), new IntPtr(0));
            Task.Delay(50).Wait();
            PostMessage(handle, WM_KEYUP, new IntPtr((int)key), new IntPtr(0));
        }

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static void Start()
        {
            IntPtr hInstance = LoadLibrary("User32");
            Hhook = SetWindowsHookEx(WH_KEYBOARD_LL, KHP, hInstance, 0);
        }

        public static void Stop()
        {
            //UnhookWindowsHookEx(Hhook);
        }

        /// <summary>
        /// Calls the next hook.
        /// </summary>
        /// <param name="idHook">The hook id</param>
        /// <param name="nCode">The hook code</param>
        /// <param name="wParam">The wparam.</param>
        /// <param name="lParam">The lparam.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookStruct lParam);

        [DllImport("user32.dll")]
        private static extern int GetAsyncKeyState(int i);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        ///
        /// </summary>
        /// <param name="pbKeyState"></param>
        /// <returns></returns>
        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        /// <summary>
        ///
        /// </summary>
        /// <param name="vKey"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        private static int HookProc(int code, int wParam, ref KeyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                if (wParam == WM_KEYDOWN || wParam == WM_KEYUP)
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

        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="lpFileName">Name of the library</param>
        /// <returns>A handle to the library</returns>
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        /// <summary>
        /// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
        /// </summary>
        /// <param name="idHook">The id of the event you want to hook</param>
        /// <param name="callback">The callback.</param>
        /// <param name="hInstance">The handle you want to attach the event to, can be null</param>
        /// <param name="threadId">The thread you want to attach the event to, can be null</param>
        /// <returns>a handle to the desired hook</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);

        /// <summary>
        ///
        /// </summary>
        /// <param name="uVirtKey"></param>
        /// <param name="uScanCode"></param>
        /// <param name="lpbKeyState"></param>
        /// <param name="lpwTransKey"></param>
        /// <param name="fuState"></param>
        /// <returns></returns>
        [DllImport("user32")]
        private static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        /// <summary>
        /// Unhooks the windows hook.
        /// </summary>
        /// <param name="hInstance">The hook handle that was returned from SetWindowsHookEx</param>
        /// <returns>True if successful, false otherwise</returns>
        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        /// <summary>
        /// defines the callback type for the hook
        /// </summary>
        private delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHookStruct lParam);

        private struct KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
    }
}