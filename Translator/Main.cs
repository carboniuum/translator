using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Translator
{
    public class Main
    {
        private IntPtr hWnd;

        public Main(IntPtr hWnd)
        {
            this.hWnd = hWnd;
        }

        public void RegisterHotKeys()
        {
            RegisterHotKey(hWnd, 1, (uint)ModifierKeys.Control, (uint)Keys.T);
        }

        public void UnRegisterHotKeys()
        {
            UnregisterHotKey(hWnd, 1);
        }

        public void TranslateText(out string input, out string output, string lang)
        {
            string url = String.Empty, language1 = String.Empty, language2 = String.Empty;

            input = GetClipBoardText();

            if (!String.IsNullOrEmpty(input))
            {
                switch (lang)
                {
                    case "English": language1 = "en"; language2 = "ru"; break;
                    case "Russian": language1 = "ru"; language2 = "en"; break;
                }
                url = String.Format
                ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                 language1, language2, Uri.EscapeUriString(input));
            }

            HttpClient httpClient = new HttpClient();
            string result = httpClient.GetStringAsync(url).Result;

            var jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);

            var translationItems = jsonData[0];

            output = String.Empty;

            foreach (object item in translationItems)
            {
                IEnumerable translationLineObject = item as IEnumerable;
                IEnumerator translationLineString = translationLineObject.GetEnumerator();
                translationLineString.MoveNext();
                output += string.Format(" {0}", Convert.ToString(translationLineString.Current));
            }

            if (output.Length > 1) 
            { 
                output = output.Substring(1); 
            }
        }

        private void SendCtrlC(IntPtr hWnd)
        {
            uint KEYEVENTF_KEYUP = 2;
            byte VK_CONTROL = 0x11;
            SetForegroundWindow(hWnd);
            keybd_event(VK_CONTROL, 0, 0, 0);
            keybd_event(0x43, 0, 0, 0); //Send the C key (43 is "C")
            keybd_event(0x43, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);// 'Left Control Up
        }

        [STAThread]
        private string GetClipBoardText()
        {
            SendCtrlC(hWnd);
            string text = Clipboard.ContainsText(TextDataFormat.UnicodeText) ?
                Clipboard.GetText(TextDataFormat.UnicodeText) : null;

            return text;
        }

        #region Win32 API
        [DllImport("User32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        #endregion
    }

    public enum ModifierKeys
    {
        Alt = 0x0001,
        Control = 0x0002,
        Shift = 0x0004,
        Window = 0x0008
    }
}
