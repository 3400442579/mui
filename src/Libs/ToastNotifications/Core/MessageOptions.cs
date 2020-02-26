using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ToastNotifications.Core
{
    public class MessageOptions
    {
        public double? FontSize { get; set; }

        public bool ShowCloseButton { get; set; } = true;

        public object Tag { get; set; }

        public bool FreezeOnMouseEnter { get; set; } = true;

        public Action<NotificationBase> NotificationClickAction { get; set; }

        public Action<NotificationBase> CloseClickAction { get; set; }
        public bool UnfreezeOnMouseLeave { get; set; } = false;



    }

    public class ConfirmOptions : MessageOptions
    {
        private bool showDeclineButton = true;
        public bool ShowDeclineButton {
            get { return showDeclineButton; }
            set {
                showDeclineButton = value;
                OnPropertyChanged("ShowDeclineButton");
            }
        }

        private string declineButtonText = "";
        public string DeclineButtonText {
            get { return declineButtonText; }
            set
            {
                declineButtonText = value;
                OnPropertyChanged("DeclineButtonText");
            }
        }

        private string confirmButtonText = "是";
        public string ConfirmButtonText {
            get { return confirmButtonText; }
            set
            {
                confirmButtonText = value;
                OnPropertyChanged("ConfirmButtonText");
            }
        }

        private string cancelButtonText = "是";
        public string CancelButtonText {
            get { return cancelButtonText; }
            set
            {
                cancelButtonText = value;
                OnPropertyChanged("CancelButtonText");
            }
        } 

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
