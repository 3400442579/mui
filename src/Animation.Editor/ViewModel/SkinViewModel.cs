using DH.MUI.Core;
using Reactive.Bindings;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace Animation.Editor.ViewModel
{
    public class SkinViewModel
    {
        private const string FontSmall = "small";
        private const string FontLarge = "large";



        public SkinViewModel()
        {
            // add the default themes
            this.Themes.Add(new Link { DisplayName = "dark", Source = AppearanceManager.DarkThemeSource });
            this.Themes.Add(new Link { DisplayName = "light", Source = AppearanceManager.LightThemeSource });
            // add additional themes
            //this.Themes.Add(new Link { DisplayName = "bing image", Source = new Uri("/ModernUIDemo;component/Assets/ModernUI.BingImage.xaml", UriKind.Relative) });
            //this.Themes.Add(new Link { DisplayName = "hello kitty", Source = new Uri("/ModernUIDemo;component/Assets/ModernUI.HelloKitty.xaml", UriKind.Relative) });
            //this.Themes.Add(new Link { DisplayName = "love", Source = new Uri("/ModernUIDemo;component/Assets/ModernUI.Love.xaml", UriKind.Relative) });
            //this.Themes.Add(new Link { DisplayName = "snowflakes", Source = new Uri("/ModernUIDemo;component/Assets/ModernUI.Snowflakes.xaml", UriKind.Relative) });

            //AppearanceManager.Current.PropertyChanged += OnAppearanceManagerPropertyChanged;

            SelectedFontSize.Subscribe(v =>
            {
                if (string.IsNullOrEmpty(v)) return;
                AppearanceManager.Current.FontSize = v == FontLarge ? FontSize.Large : FontSize.Small;
            });
            SelectedTheme.Subscribe(v =>
            {
                if (v == null) return;
                if (SelectedAccentColor.Value.A == 0)
                    SelectedAccentColor.Value = AccentColors.FirstOrDefault();
                AppearanceManager.Current.ThemeSource = v.Source;
            });
            SelectedAccentColor.Subscribe(v =>
            {
                if (v.A==0) return; 
                AppearanceManager.Current.AccentColor = v;
            });

            SyncThemeAndColor();
        }

        private void SyncThemeAndColor()
        {
            SelectedFontSize.Value = AppearanceManager.Current.FontSize == FontSize.Large ? FontLarge : FontSmall;

            // synchronizes the selected viewmodel theme with the actual theme used by the appearance manager.
            SelectedTheme.Value = this.Themes.FirstOrDefault(l => l.Source.Equals(AppearanceManager.Current.ThemeSource));

            // and make sure accent color is up-to-date
            SelectedAccentColor.Value = AppearanceManager.Current.AccentColor;
        }

        private void OnAppearanceManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "ThemeSource" || e.PropertyName == "AccentColor")
            //{
            //    SyncThemeAndColor();
            //}
        }

        public LinkCollection Themes { get; } = new LinkCollection();

        public string[] FontSizes
        {
            get { return new string[] { FontSmall, FontLarge }; }
        }

        // 9 accent colors from metro design principles
        public Color[] AccentColors { get; private set; } = new Color[]{
            Color.FromRgb(0x33, 0x99, 0xff),   // blue
            Color.FromRgb(0x00, 0xab, 0xa9),   // teal
            Color.FromRgb(0x33, 0x99, 0x33),   // green
            Color.FromRgb(0x8c, 0xbf, 0x26),   // lime
            Color.FromRgb(0xf0, 0x96, 0x09),   // orange
            Color.FromRgb(0xff, 0x45, 0x00),   // orange red
            Color.FromRgb(0xe5, 0x14, 0x00),   // red
            Color.FromRgb(0xff, 0x00, 0x97),   // magenta
            Color.FromRgb(0xa2, 0x00, 0xff),   // purple            
        };

        public ReactiveProperty<Link> SelectedTheme { get; set; } = new ReactiveProperty<Link>();
        //public Link SelectedTheme
        //{
        //    get { return this.selectedTheme; }
        //    set
        //    {
        //        if (this.selectedTheme != value)
        //        {
        //            this.selectedTheme = value;
        //            //RaisePropertyChanged("SelectedTheme");

        //            // and update the actual theme
        //            AppearanceManager.Current.ThemeSource = value.Source;
        //        }
        //    }
        //}

        public ReactiveProperty<string> SelectedFontSize { get; set; } = new ReactiveProperty<string>();
        //public string SelectedFontSize
        //{
        //    get { return this.selectedFontSize; }
        //    set
        //    {
        //        if (this.selectedFontSize != value)
        //        {
        //            this.selectedFontSize = value;
        //            AppearanceManager.Current.FontSize = value == FontLarge ? FontSize.Large : FontSize.Small;
        //        }
        //    }
        //}

        public ReactiveProperty<Color> SelectedAccentColor { get; set; } = new ReactiveProperty<Color>();
        //{
        //    get { return this.selectedAccentColor; }
        //    set
        //    {
        //        if (this.selectedAccentColor != value)
        //        {
        //            this.selectedAccentColor = value;
        //            AppearanceManager.Current.AccentColor = value;
        //        }
        //    }
        //}
    }
}
