using System;
using System.Runtime.CompilerServices;
using MessageBox.Avalonia.Enums;

namespace MessageBox.Avalonia.Models
{
    public class ButtonDefinition
    {
        public string Name { get; set; } = "OK";
        private ButtonType _type = ButtonType.Default;

        public ButtonType Type
        {
            set { _type = value; }
        }

        public bool? IsDefault { get; set; }

        public string TypeName => _type.ToString();
    }
}