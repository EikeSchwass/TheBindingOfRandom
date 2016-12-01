using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using TheBindingOfRandom.Annotations;

namespace TheBindingOfRandom
{
    public class CharacterModel : INotifyPropertyChanged
    {
        private Characters character;
        private ImageSource imageSource;
        private string text;
        private bool wasPlayed;

        public CharacterModel(Characters character, bool wasPlayed)
        {
            Character = character;
            WasPlayed = wasPlayed;
            ImageSource = Character.GetImageSource();
            Text = character.GetName();
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