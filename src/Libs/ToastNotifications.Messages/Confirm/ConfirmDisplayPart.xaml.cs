using ToastNotifications.Core;

namespace ToastNotifications.Messages.Confirm
{
    /// <summary>
    /// Interaction logic for CustomCommandDisplayPart.xaml
    /// </summary>
    public partial class ConfirmDisplayPart : NotificationDisplayPart
    {
        public ConfirmDisplayPart(ConfirmCommand notification)
        {
            InitializeComponent();
            Bind(notification);
        }
    }
}
