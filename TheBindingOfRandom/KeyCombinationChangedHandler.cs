using System;
using System.Windows.Forms;
using System.Windows.Input;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace TheBindingOfRandom
{
    public class KeyCombinationChangedHandler : ICommand
    {
        private const string ALPHANUMERIC = "ABCDEFGHIJKLMNOPQRSTUVWXYZÖÄÜ+*~#'-_.:,;<>|1234567890ß?\\!\"§$%&/()=²³{[]}°";
        private bool executing;

        public KeyCombinationChangedHandler(RandomizationModel randomizationModel)
        {
            RandomizationModel = randomizationModel;
            Keylogger.KeyDown += Keylogger_KeyDown;
        }

        public event EventHandler CanExecuteChanged;

        public bool Executing
        {
            get { return executing; }
            private set
            {
                executing = value;
                CanExecuteChanged?.Invoke(this, null);
            }
        }

        public RandomizationModel RandomizationModel { get; }

        public bool CanExecute(object parameter)
        {
            return !Executing;
        }

        public void Execute(object parameter)
        {
            Executing = true;
            RandomizationModel.StartKeyCombination = "Ctrl+Alt+...";
        }

        private void Keylogger_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Executing)
                return;
            Application.Current.Dispatcher.Invoke(() =>
                                                  {
                                                      char c = char.ToUpper((char)e.KeyValue);
                                                      if (ALPHANUMERIC.Contains(c + ""))
                                                      {
                                                          RandomizationModel.StartKeyCombination = "Ctrl+Alt+" + c;
                                                          Executing = false;
                                                      }
                                                  });
        }
    }
}