using System;
using UnityEngine;
using System.Text;
using UnityEngine.Assertions;

namespace Rehawk.Foundation.CommandTerminal
{
    public class Terminal : MonoBehaviour
    {
        [Header("Window")] 
        
        [Range(0, 1)]
        [SerializeField] private float height = 0.4f;

        [Header("Theme")]

        [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.5f);
        [SerializeField] private Color foregroundColor = Color.white;
        [SerializeField] private Color shellColor = Color.white;
        [SerializeField] private Color inputColor = new Color(1, 0.6f, 0, 1);
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color errorColor = Color.red;
        
        private bool isOpen;
        
        private int bufferSize = 512;
        private string toggleHotkey = "slash";

        private bool inputFix;
        private bool moveCursor;
        private bool initialOpen;
        private float windowSize;
        private string commandText;
        private string cachedCommandText;
        
        private Vector2 scrollPosition;
        
        private Font font;
        private GUIStyle windowStyle;
        private GUIStyle labelStyle;
        private GUIStyle inputStyle;
        private Texture2D backgroundTexture;
        private Texture2D inputBackgroundTexture;

        private TextEditor editorState;

        public static event EventHandler Opened;
        public static event EventHandler Closed;
        
        public static CommandLog Buffer { get; private set; }
        public static CommandShell Shell { get; private set; }
        public static CommandHistory History { get; private set; }
        public static CommandAutocomplete Autocomplete { get; private set; }

        private static bool IssuedError
        {
            get { return Shell.IssuedErrorMessage != null; }
        }

        public static void Log(string format, params object[] message)
        {
            Log(TerminalLogType.ShellMessage, format, message);
        }

        public static void Log(TerminalLogType type, string format, params object[] message)
        {
            Buffer.HandleLog(string.Format(format, message), type);
        }

        public void Open()
        {
            isOpen = true;
            
            inputFix = true;
            cachedCommandText = commandText;
            commandText = "";
            scrollPosition.y = int.MaxValue;
            
            Opened?.Invoke(this, EventArgs.Empty);
        }

        public void Close()
        {
            isOpen = false;

            inputFix = true;
            cachedCommandText = commandText;
            commandText = "";
            
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public void Toggle()
        {
            if (!isOpen)
            {
                Open();
            }
            else
            {
                Close();
            }
        }

        private void OnEnable()
        {
            Buffer = new CommandLog(bufferSize);
            Shell = new CommandShell();
            History = new CommandHistory();
            Autocomplete = new CommandAutocomplete();

            // Hook Unity log events
            // Application.logMessageReceived += HandleUnityLog;
        }

        private void OnDisable()
        {
            // Application.logMessageReceived -= HandleUnityLog;
        }

        private void Start()
        {
            font = Font.CreateDynamicFontFromOSFont("Arial", 14);

            commandText = "";
            cachedCommandText = commandText;
            Assert.AreNotEqual(toggleHotkey.ToLower(), "return", "Return is not a valid ToggleHotkey");

            SetupWindow();
            SetupInput();
            SetupLabels();

            Shell.RegisterCommands();

            if (IssuedError)
            {
                Log(TerminalLogType.Error, "Error: {0}", Shell.IssuedErrorMessage);
            }

            foreach (var command in Shell.Commands)
            {
                Autocomplete.Register(command.Key);
            }
        }

        private void OnGUI()
        {
            if (Event.current.Equals(Event.KeyboardEvent(toggleHotkey)))
            {
                Toggle();
                initialOpen = true;
            }

            if (!isOpen)
            {
                return;
            }

            GUILayout.Window(88, new Rect(0, 0, Screen.width, Screen.height * height), DrawConsole, "", windowStyle);
        }

        private void SetupWindow()
        {
            backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, backgroundColor);
            backgroundTexture.Apply();

            windowStyle = new GUIStyle();
            windowStyle.normal.background = backgroundTexture;
            windowStyle.padding = new RectOffset(4, 4, 4, 4);
            windowStyle.normal.textColor = foregroundColor;
            windowStyle.font = font;
        }

        private void SetupLabels()
        {
            labelStyle = new GUIStyle();
            labelStyle.padding = new RectOffset(2, 2, 2, 2);
            labelStyle.normal.textColor = foregroundColor;
            labelStyle.font = font;
            labelStyle.wordWrap = true;
        }

        private void SetupInput()
        {
            inputStyle = new GUIStyle();
            inputStyle.padding = new RectOffset(4, 4, 4, 4);
            inputStyle.font = font;
            inputStyle.fixedHeight = font.fontSize * 1.6f;
            inputStyle.normal.textColor = inputColor;

            var darkBackground = new Color();
            darkBackground.r = backgroundColor.r - 1;
            darkBackground.g = backgroundColor.g - 1;
            darkBackground.b = backgroundColor.b - 1;
            darkBackground.a = 0.5f;

            inputBackgroundTexture = new Texture2D(1, 1);
            inputBackgroundTexture.SetPixel(0, 0, darkBackground);
            inputBackgroundTexture.Apply();
            inputStyle.normal.background = inputBackgroundTexture;
        }

        private void DrawConsole(int window)
        {
            GUILayout.BeginVertical();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUIStyle.none, GUIStyle.none);
            
            GUILayout.FlexibleSpace();
            DrawLogs();
            GUILayout.EndScrollView();
            
            if (moveCursor)
            {
                CursorToEnd();
                moveCursor = false;
            }

            Event evnt = Event.current;
            
            if (evnt.Equals(Event.KeyboardEvent("escape")))
            {
                Close();
            }
            else if (evnt.Equals(Event.KeyboardEvent("return")))
            {
                EnterCommand();
            }
            else if (evnt.Equals(Event.KeyboardEvent("up")))
            {
                commandText = History.Previous();
                moveCursor = true;
            }
            else if (evnt.Equals(Event.KeyboardEvent("down")))
            {
                commandText = History.Next();
            }
            else if (evnt.Equals(Event.KeyboardEvent(toggleHotkey)))
            {
                Toggle();
            }
            else if (evnt.Equals(Event.KeyboardEvent("tab")))
            {
                CompleteCommand();
                moveCursor = true;
            }

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            
            GUILayout.Label(">", inputStyle, GUILayout.Width(font.fontSize));

            GUI.SetNextControlName("commandTextField");
            commandText = GUILayout.TextField(commandText, inputStyle);

            if (inputFix && commandText.Length > 0)
            {
                commandText = cachedCommandText;
                inputFix = false;
            }

            if (initialOpen)
            {
                GUI.FocusControl("commandTextField");
                initialOpen = false;
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawLogs()
        {
            foreach (var log in Buffer.Logs)
            {
                labelStyle.normal.textColor = GetLogColor(log.type);
                GUILayout.Label(log.message, labelStyle);
            }
        }

        private void EnterCommand()
        {
            Log(TerminalLogType.Input, "{0}", commandText);
            Shell.RunCommand(commandText);
            History.Push(commandText);

            if (IssuedError)
            {
                Log(TerminalLogType.Error, "Error: {0}", Shell.IssuedErrorMessage);
            }

            commandText = "";
            scrollPosition.y = int.MaxValue;
        }

        private void CompleteCommand()
        {
            string headText = commandText;
            int formatWidth = 0;

            string[] completionBuffer = Autocomplete.Complete(ref headText, ref formatWidth);
            int completionLength = completionBuffer.Length;

            if (completionLength != 0)
            {
                commandText = headText;
            }

            if (completionLength > 1)
            {
                var logBuffer = new StringBuilder();

                foreach (string completion in completionBuffer)
                {
                    logBuffer.Append(completion.PadRight(formatWidth + 4));
                }

                Log("{0}", logBuffer);
                scrollPosition.y = int.MaxValue;
            }
        }

        private void CursorToEnd()
        {
            if (editorState == null)
            {
                editorState = (TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            }

            editorState.MoveCursorToPosition(new Vector2(999, 999));
        }

        private void HandleUnityLog(string message, string stackTrace, LogType type)
        {
            Buffer.HandleLog(message, stackTrace, (TerminalLogType) type);
            scrollPosition.y = int.MaxValue;
        }

        private Color GetLogColor(TerminalLogType type)
        {
            switch (type)
            {
                case TerminalLogType.Message: 
                    return foregroundColor;
                case TerminalLogType.Warning: 
                    return warningColor;
                case TerminalLogType.Input: 
                    return inputColor;
                case TerminalLogType.ShellMessage: 
                    return shellColor;
                default: 
                    return errorColor;
            }
        }
    }
}