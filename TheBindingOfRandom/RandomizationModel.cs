using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using TheBindingOfRandom.Annotations;
using TheBindingOfRandom.Properties;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace TheBindingOfRandom
{
    public class RandomizationModel : INotifyPropertyChanged
    {
        private KeyCombinationChangedHandler keyCombinationChangedCommand;
        private bool preventDuplicates;
        private string startKeyCombination;

        public RandomizationModel()
        {
            InitCharacters();
            PreventDuplicates = Settings.Default.PreventDuplicates;
            StartKeyCombination = Settings.Default.StartKeyCombination;
            KeyCombinationChangedCommand = new KeyCombinationChangedHandler(this);
            ClearPlayHistoryCommand = new ClearPlayHistoryCommand(this);
            Keylogger.KeyDown += Keylogger_KeyDown;
        }

        public event NewPlayStartedEventHandler NewPlayStarted;

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<CharacterModel> Characters { get; } = new ObservableCollection<CharacterModel>();

        public ClearPlayHistoryCommand ClearPlayHistoryCommand { get; }

        public KeyCombinationChangedHandler KeyCombinationChangedCommand
        {
            get { return keyCombinationChangedCommand; }
            set
            {
                if (Equals(value, keyCombinationChangedCommand))
                    return;
                keyCombinationChangedCommand = value;
                OnPropertyChanged();
            }
        }

        public bool PreventDuplicates
        {
            get { return preventDuplicates; }
            set
            {
                if (value == preventDuplicates)
                    return;
                preventDuplicates = value;
                UpdateDuplicates();
                OnPropertyChanged();
            }
        }

        public string StartKeyCombination
        {
            get { return startKeyCombination; }
            set
            {
                if (value == startKeyCombination)
                    return;
                startKeyCombination = value;
                Settings.Default.StartKeyCombination = startKeyCombination;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        private static Random Random { get; } = new Random();

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitCharacters()
        {
            Settings.Default.Reload();
            var playedCharacters = (Characters)Settings.Default.PlayedCharacters;
            var values = Enum.GetValues(typeof(Characters)).Cast<Characters>();
            foreach (var character in values)
            {
                if (character == TheBindingOfRandom.Characters.None)
                    continue;
                var wasPlayed = playedCharacters.HasFlag(character);
                Characters.Add(new CharacterModel(character, wasPlayed));
            }
        }

        private void Keylogger_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.LMenu)
                return;
            if (KeyCombinationChangedCommand.Executing)
                return;
            if (Keylogger.IsKeyDown(Keys.LControlKey))
            {
                if (Keylogger.IsKeyDown(Keys.LMenu))
                {
                    var key = char.ToUpper((char)e.KeyValue);
                    var target = StartKeyCombination.Split('+')[2][0];
                    if (key == target)
                    {
                        StartRandom();
                    }
                }
            }
        }

        private void StartRandom()
        {
            int index;
            CharacterModel character;
            while (true)
            {
                index = Random.Next(Characters.Count);
                character = Characters[index];
                if (character.IsSelected && !(PreventDuplicates && character.WasPlayed))
                {
                    break;
                }
            }
            character.WasPlayed = PreventDuplicates;
            if (Characters.All(c => !c.IsSelected || c.WasPlayed))
            {
                foreach (var characterModel in Characters)
                {
                    characterModel.WasPlayed = false;
                }
            }
            for (int i = 0; i < index + Characters.Count + 1 - 3; i++)
            {
                Keylogger.PostKey(Keys.Right);
                Task.Delay((i + 1) * 10).Wait();
            }
            NewPlayStarted?.Invoke(this, character);
        }

        private void UpdateDuplicates()
        {
            Settings.Default.PreventDuplicates = PreventDuplicates;
            Settings.Default.Save();
            var playedCharacters = (Characters)Settings.Default.PlayedCharacters;
            foreach (var characterModel in Characters)
            {
                if (PreventDuplicates && playedCharacters.HasFlag(characterModel.Character))
                    characterModel.DisabledOpacity = 0.25;
                else
                    characterModel.DisabledOpacity = 1;
            }
        }
    }

    public delegate void NewPlayStartedEventHandler(RandomizationModel sender, CharacterModel characterModel);

    public class ClearPlayHistoryCommand : ICommand
    {
        public ClearPlayHistoryCommand(RandomizationModel randomizationModel)
        {
            RandomizationModel = randomizationModel;
            RandomizationModel.NewPlayStarted += RandomizationModel_NewPlayStarted;
        }

        private void RandomizationModel_NewPlayStarted(RandomizationModel sender, CharacterModel characterModel)
        {
            CanExecuteChanged?.Invoke(this, null);
        }

        public event EventHandler CanExecuteChanged;
        public RandomizationModel RandomizationModel { get; }

        public bool CanExecute(object parameter) => RandomizationModel.Characters.Any(c => c.WasPlayed);

        public void Execute(object parameter)
        {
            foreach (var characterModel in RandomizationModel.Characters)
            {
                characterModel.WasPlayed = false;
            }
            CanExecuteChanged?.Invoke(this, null);
        }
    }
}