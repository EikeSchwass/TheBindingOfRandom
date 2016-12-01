using System;
using System.Collections.ObjectModel;
using System.Linq;
using TheBindingOfRandom.Properties;

namespace TheBindingOfRandom
{
    public class RandomizationModel
    {
        public ObservableCollection<CharacterModel> Characters { get; } = new ObservableCollection<CharacterModel>();

        public RandomizationModel()
        {
            InitCharacters();
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
    }
}