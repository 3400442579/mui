using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Models;
using MessageBox.Avalonia.Views;
using System.Collections.Generic;

namespace MessageBox.Avalonia.ViewModels
{
    public class MsBoxCustomViewModel : AbstractMsBoxViewModel
    {
        public IEnumerable<ButtonDefinition> ButtonDefinitions { get; }
        private MsBoxCustomWindow _window;

        // public ReactiveCommand<string, Unit> ButtonClickCommand { get; private set; }

        public MsBoxCustomViewModel(MessageBoxCustomParams @params) : base(@params)
        {
            _window = @params.Window;
            ButtonDefinitions = @params.ButtonDefinitions;
        }

        public void ButtonClick(string parameter)
        {
            foreach (var bd in ButtonDefinitions)
            {
                if (parameter.Equals(bd.Name))
                {
                    _window.ButtonResult = bd.Name;
                    break;
                }
            }

            _window.Close();
            // Code for executing the command here.
        }
    }
}