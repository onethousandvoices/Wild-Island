using System;
using System.Linq;
using Views.UI;
using WildIsland.Processors;
using Zenject;

namespace WildIsland.Controllers
{
    public class DebugConsoleController : IInitializable, IConsoleHandler
    {
        [Inject] private DebugConsoleView _view;
        [Inject] private IGetCheats _cheats;

        private DebugCommandBase[] _commands;
        private string _previousCommand;

        public void Initialize()
        {
            DebugCommand help = new DebugCommand("help", "List of all commands", "help", _view.UpdateHelpState);
            DebugCommand damagePlayer = new DebugCommand("damage", "Damage player", "damage", _cheats.DamagePlayer);
            DebugCommand tempEffect = new DebugCommand("effect_temp", "Temporary hunger decrease for 3 secs", "effect_temp", _cheats.TemporaryEffectApply);
            DebugCommand periodicEffect = new DebugCommand("effect_periodic", "Periodic damage to head for 3 secs every 1 secs", "effect_periodic", _cheats.PeriodicEffectApply);

            DebugCommand<int> setFps = new DebugCommand<int>("fps_", "Set fps", "fps_<value>", i => _cheats.FrameRateChange(i));
            DebugCommand<int> setTime = new DebugCommand<int>("time_", "Set time", "time_<value>", i => _cheats.TimeSpeedUp(i));

            _commands = new DebugCommandBase[]
            {
                help,
                damagePlayer,
                tempEffect,
                periodicEffect,
                setFps,
                setTime,
            };

            _view.Init(_commands);
        }

        private void HandeInput()
        {
            if (!_view.ConsoleShown)
                return;

            _previousCommand = _view.Input;
            string properties = string.Concat(_view.Input.SkipWhile(x => x != '_').Skip(1));

            foreach (DebugCommandBase command in _commands)
            {
                if (!_view.Input.Contains(command.Id))
                    continue;

                switch (command)
                {
                    case DebugCommand debugCommand:
                        debugCommand.Invoke();
                        break;
                    case DebugCommand<int> debugCommandInt:
                        debugCommandInt.Invoke(int.Parse(properties));
                        break;
                }
            }
        }

        public void ShowConsole()
        {
            _view.UpdateConsoleState();
            _view.ResetInput();
        }

        public void OnReturn()
        {
            HandeInput();
            _view.ResetInput();
        }

        public void OnUpArrow()
            => _view.SetInput(_previousCommand);
    }

    public interface IConsoleHandler
    {
        public void ShowConsole();
        public void OnReturn();
        public void OnUpArrow();
    }

    public abstract class DebugCommandBase
    {
        public readonly string Id;
        public readonly string Description;
        public readonly string Format;

        protected DebugCommandBase(string id, string description, string format)
        {
            Id = id;
            Description = description;
            Format = format;
        }
    }

    public class DebugCommand<T> : DebugCommandBase
    {
        private readonly Action<T> _command;

        public DebugCommand(string id, string description, string format, Action<T> command) : base(id, description, format)
            => _command = command;

        public void Invoke(T value)
            => _command?.Invoke(value);
    }

    public class DebugCommand : DebugCommandBase
    {
        private readonly Action _command;

        public DebugCommand(string id, string description, string format, Action command) : base(id, description, format)
            => _command = command;

        public void Invoke()
            => _command?.Invoke();
    }
}