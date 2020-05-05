using DH.Editor.Core;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Media;

namespace DH.Editor.ViewModels
{
    public class SettingViewModel : ReactiveObject
    {
        public SettingViewModel() {
            this.WhenAnyValue(o => o.SelectedTheme)
                .Where(o => !string.IsNullOrEmpty(o))
                .Do(o => { Config.Instance.Theme = o; Config.Save(); });

            this.WhenAnyValue(o => o.SelectedColor)
               .Where(o => o!=null)
               .Do(o => { Config.Instance.AccentColor = o.ToString(); Config.Save(); });

            this.WhenAnyValue(o => o.SelectedLang)
               .Where(o => !string.IsNullOrEmpty(o))
               .Do(o => { Config.Instance.Lang = o; Config.Save(); });

        }

        // 9 accent colors from metro design principles
        private static readonly Color[] accentColors = new Color[]{
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

        
        public Color[] AccentColors { get;private set; } = accentColors;

        [ReactiveUI.Fody.Helpers.Reactive]
        public Color SelectedColor { get; set; }

        [ReactiveUI.Fody.Helpers.Reactive]
        public string SelectedLang { get; set; }

        [ReactiveUI.Fody.Helpers.Reactive]
        public string SelectedTheme { get; set; }

    }
}
