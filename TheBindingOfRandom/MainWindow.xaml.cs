using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TheBindingOfRandom
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed)
                return;
            var checkBox = (CheckBox)sender;
            var characterModel = (CharacterModel)checkBox.DataContext;
            characterModel.WasPlayed = !characterModel.WasPlayed;
            var randomizationModel = (RandomizationModel)DataContext;
            randomizationModel.ClearPlayHistoryCommand.CanExecuteChangedInvoke();
        }
    }
}
