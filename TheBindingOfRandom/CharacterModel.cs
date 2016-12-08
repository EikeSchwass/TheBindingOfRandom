using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using TheBindingOfRandom.Annotations;
using TheBindingOfRandom.Properties;

namespace TheBindingOfRandom
{
    public class CharacterModel : INotifyPropertyChanged
    {
        private Characters character;
        private double disabledOpacity = 1;
        private ImageSource imageSource;
        private bool isSelected;
        private string text;
        private bool wasPlayed;
        private bool isAvailable;

        public CharacterModel(Characters character, bool wasPlayed)
        {
            Character = character;
            WasPlayed = wasPlayed;
            ImageSource = Character.GetImageSource();
            Text = character.GetName();
            IsSelected = ((Characters)Settings.Default.SelectedCharacters).HasFlag(character);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Characters Character
        {
            get { return character; }
            set
            {
                if (value == character)
                    return;
                character = value;
                OnPropertyChanged();
            }
        }

        public double DisabledOpacity
        {
            get { return disabledOpacity; }
            set
            {
                if (value.Equals(disabledOpacity))
                    return;
                disabledOpacity = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                if (Equals(value, imageSource))
                    return;
                imageSource = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value == isSelected)
                    return;
                isSelected = value;
                var selectedCharacters = (Characters)Settings.Default.SelectedCharacters;
                if (selectedCharacters.HasFlag(Character))
                {
                    if (!value)
                        Settings.Default.SelectedCharacters -= (long)Character;
                }
                else
                {
                    if (value)
                        Settings.Default.SelectedCharacters += (long)Character;
                }
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public bool IsAvailable
        {
            get { return isAvailable; }
            set
            {
                if (value == isAvailable) return;
                isAvailable = value;
                var notAvailableCharacters = (Characters)Settings.Default.NotAvailableCharacters;
                if (notAvailableCharacters.HasFlag(Character))
                {
                    if (!value)
                        Settings.Default.NotAvailableCharacters -= (long)Character;
                }
                else
                {
                    if (value)
                        Settings.Default.NotAvailableCharacters += (long)Character;
                }
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get { return text; }
            set
            {
                if (value == text)
                    return;
                text = value;
                OnPropertyChanged();
            }
        }

        public bool WasPlayed
        {
            get { return wasPlayed; }

            set
            {
                if (value == wasPlayed)
                    return;
                wasPlayed = value;
                var playedCharacters = (Characters)Settings.Default.PlayedCharacters;
                if (playedCharacters.HasFlag(Character))
                {
                    if (!value)
                        Settings.Default.PlayedCharacters -= (long)Character;
                }
                else
                {
                    if (value)
                        Settings.Default.PlayedCharacters += (long)Character;
                }
                Settings.Default.Save();
                DisabledOpacity = !wasPlayed ? 1 : 0.25;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}