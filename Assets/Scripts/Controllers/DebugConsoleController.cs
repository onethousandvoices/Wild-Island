using Effects;
using System;
using System.Linq;
using UnityEngine;
using Views.UI;
using WildIsland.Processors;
using WildIsland.SOs;
using WildIsland.Utility;
using WildIsland.Views;
using Zenject;

namespace WildIsland.Controllers
{
    public class DebugConsoleController : IInitializable, IConsoleHandler
    {
        [Inject] private DebugConsoleView _view;
        [Inject] private IDaySetter _daySetter;
        [Inject] private IGetPlayerStats _player;
        [Inject] private IDataProcessor _dataProcessor;
        [Inject] private IEffectProcessor _effectProcessor;

        private DebugCommandBase[] _commands;
        
        private TestTemporaryEffect _testTemporary;
        private TestPeriodicEffect _testPeriodic;

        private string _previousCommand;

        public void Initialize()
        {
            DebugCommand help = new DebugCommand("help", "List of all commands", "help", _view.UpdateHelpState);
            DebugCommand damagePlayer = new DebugCommand("damage", "Damage player", "damage", DamagePlayer);
            DebugCommand tempEffect = new DebugCommand("effect_temp", "Temporary hunger decrease for 3 secs", "effect_temp", TemporaryEffectApply);
            DebugCommand periodicEffect = new DebugCommand("effect_periodic", "Periodic damage to head for 3 secs every 1 secs", "effect_periodic", PeriodicEffectApply);
            
            // DebugCommand setDay = new DebugCommand("time_day", "Set day", "time_day", () => _daySetter.SetPreset(DayPresetType.Day));
            // DebugCommand setNight = new DebugCommand("time_night", "Set night", "time_night", () => _daySetter.SetPreset(DayPresetType.Night));

            DebugCommand<int> setFps = new DebugCommand<int>("fps_", "Set fps (0 - uncapped)", "fps_<value>", FrameRateChange);
            DebugCommand<string> setTime = new DebugCommand<string>("time_", "Set time speed or day/night", "time_<value>", TimeCommandDeterminate);

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
                    case DebugCommand<string> debugCommandString:
                        debugCommandString.Invoke(properties);
                        break;
                }
            }
        }

        private void TimeCommandDeterminate(string value)
        {
            Array dayPresetTypes = Enum.GetValues(typeof(DayPresetType));

            foreach (object presetType in dayPresetTypes)
            {
                if (!string.Equals(presetType.ToString(), value, StringComparison.CurrentCultureIgnoreCase))
                    continue;
                _daySetter.SetPreset((DayPresetType)presetType);
                return;
            }

            Time.timeScale = Mathf.Clamp(int.Parse(value), 0, 100 );
        }
        
        private void DamagePlayer()
            => _dataProcessor.SetAllHealths(isRandomizing: true);

        private void FrameRateChange(int fps)
        {
            Application.targetFrameRate = fps;
            Debug.Log($"Fps set to {Application.targetFrameRate}");
        }

        private void TemporaryEffectApply()
        {
            _testTemporary = new TestTemporaryEffect(5f);
            _testTemporary.AffectedStats.Add(new AffectedStat(_player.Stats.HungerDecrease, 3));
            _effectProcessor.AddEffect(_testTemporary);
        }

        private void PeriodicEffectApply()
        {
            _testPeriodic = new TestPeriodicEffect(1f, 3f);
            _testPeriodic.AffectedStats.Add(new AffectedStat(_player.Stats.HeadHealth, -10));
            _effectProcessor.AddEffect(_testPeriodic);
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