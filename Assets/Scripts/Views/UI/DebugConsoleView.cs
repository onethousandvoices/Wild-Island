using UnityEngine;
using WildIsland.Controllers;

namespace Views.UI
{
    public class DebugConsoleView : MonoBehaviour, IConsoleState
    {
        public string Input { get; private set; }
        public bool ConsoleShown { get; private set; }
        
        private bool _helpShown;
        private bool _previousInput;

        private DebugCommandBase[] _commands;
        private Vector2 _scroll;

        public void Init(DebugCommandBase[] commands)
            => _commands = commands;

        public void UpdateConsoleState()
            => ConsoleShown = !ConsoleShown;

        public void UpdateHelpState()
            => _helpShown = !_helpShown;

        public void ResetInput()
            => Input = "";

        public void SetInput(string previous)
        {
            Input = previous;
            _previousInput = true;
        }

        private void OnGUI()
        {
            if (!ConsoleShown)
                return;
            
            float y = Screen.height / 2f;
            const float commandHeight = 22;
            const float width = 600;
            const float helpHeight = 100;

            if (_helpShown)
            {
                GUI.Box(new Rect(0, y, width, helpHeight), "");
                Rect viewport = new Rect(0, 0, width - 30, commandHeight * _commands.Length);
                _scroll = GUI.BeginScrollView(new Rect(0, y + 5f, width, helpHeight - 10), _scroll, viewport);

                for (int i = 0; i < _commands.Length; i++)
                {
                    DebugCommandBase command = _commands[i];
                    string label = $"{command.Format} - {command.Description}";
                    Rect labelRect = new Rect(5, commandHeight * i, viewport.width - 100, commandHeight);
                    GUI.Label(labelRect, label);
                }

                GUI.EndScrollView();
                y += helpHeight;
            }

            GUI.Box(new Rect(0, y, width, 30), "");
            GUI.backgroundColor = new Color(0, 0, 0, 255);

            GUI.SetNextControlName("console");
            Input = GUI.TextField(new Rect(10f, y + 5f, width - 20f, 20f), Input);

            GUI.FocusControl("console");
            
            if (!_previousInput)
                return;
            
            _previousInput = false;
            
            TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            textEditor.MoveTextEnd();
        }
    }

    public interface IConsoleState
    {
        public bool ConsoleShown { get; }
    }
}