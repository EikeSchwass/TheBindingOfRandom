using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TheBindingOfRandom
{
    public static class Extensions
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached("Text", typeof(string), typeof(Extensions), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.RegisterAttached("ImageSource", typeof(ImageSource), typeof(Extensions), new PropertyMetadata(default(ImageSource)));
        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.RegisterAttached("ImageHeight", typeof(double), typeof(Extensions), new PropertyMetadata(default(double)));
        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.RegisterAttached("ImageWidth", typeof(double), typeof(Extensions), new PropertyMetadata(default(double)));

        public static void SetText(UIElement checkBox, string text)
        {
            checkBox.SetValue(TextProperty, text);
        }

        public static string GetText(UIElement checkBox)
        {
            return (string)checkBox.GetValue(TextProperty);
        }

        public static void SetImageSource(UIElement checkBox, ImageSource source)
        {
            checkBox.SetValue(ImageSourceProperty, source);
        }

        public static ImageSource GetImageSource(UIElement checkBox)
        {
            return (ImageSource)checkBox.GetValue(ImageSourceProperty);
        }

        public static void SetImageWidth(UIElement checkBox, double imageWidth)
        {
            checkBox.SetValue(ImageWidthProperty, imageWidth);
        }

        public static double GetImageWidth(UIElement checkBox)
        {
            return (double)checkBox.GetValue(ImageWidthProperty);
        }

        public static void SetImageHeight(UIElement checkBox, double imageHeight)
        {
            checkBox.SetValue(ImageHeightProperty, imageHeight);
        }

        public static double GetImageHeight(UIElement checkBox)
        {
            return (double)checkBox.GetValue(ImageHeightProperty);
        }

        public static string GetName(this Characters character)
        {
            switch (character)
            {
                case Characters.None:
                    return "---";
                case Characters.Isaac:
                    return "Isaac";
                case Characters.Magdalene:
                    return "Magdalene";
                case Characters.Cain:
                    return "Cain";
                case Characters.Judas:
                    return "Judas";
                case Characters.BlueBaby:
                    return "???";
                case Characters.Eve:
                    return "Eve";
                case Characters.Samson:
                    return "Samson";
                case Characters.Azazel:
                    return "Azazel";
                case Characters.Lazarus:
                    return "Lazarus";
                case Characters.Eden:
                    return "Eden";
                case Characters.TheLost:
                    return "The Lost";
                case Characters.Lilith:
                    return "Lilith";
                case Characters.Keeper:
                    return "Keeper";
                default:
                    throw new ArgumentOutOfRangeException(nameof(character), character, null);
            }
        }

        public static ImageSource GetImageSource(this Characters characters)
        {
            return new BitmapImage(new Uri($"/Images/{characters.ToString().ToLower()}.png", UriKind.RelativeOrAbsolute));
        }

        public static int CountAvaiblable(this ObservableCollection<CharacterModel> source) => source.Count(c => c.IsAvailable);
    }
}