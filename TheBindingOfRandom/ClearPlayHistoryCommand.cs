using System;
using System.Linq;
using System.Windows.Input;

namespace TheBindingOfRandom
{
    public class ClearPlayHistoryCommand : ICommand
    {
        public ClearPlayHistoryCommand(RandomizationModel randomizationModel)
        {
            RandomizationModel = randomizationModel;
            RandomizationModel.NewPlayStarted += RandomizationModel_NewPlayStarted;
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

        private void RandomizationModel_NewPlayStarted(RandomizationModel sender, CharacterModel characterModel)
        {
            CanExecuteChangedInvoke();
        }

        internal void CanExecuteChangedInvoke()
        {
            CanExecuteChanged?.Invoke(this, null);
        }
    }
}