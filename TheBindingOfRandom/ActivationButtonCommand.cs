using System;
using System.Windows.Input;

namespace TheBindingOfRandom
{
    public class ActivationButtonCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var characterModel = (CharacterModel)parameter;
            characterModel.IsAvailable = !characterModel.IsAvailable;
        }

        public event EventHandler CanExecuteChanged;
    }
}