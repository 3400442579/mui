using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ToastNotifications.Core;

namespace ToastNotifications.Messages.Confirm
{
    public class ConfirmCommand : NotificationBase, INotifyPropertyChanged
    {
        private ConfirmDisplayPart _displayPart;

        private readonly Action<ConfirmCommand> _confirmAction;
        private readonly Action<ConfirmCommand> _declineAction;
        private readonly Action<ConfirmCommand> _cancelAction;

        public ICommand ConfirmCmd { get; set; }
        public ICommand CancelCmd { get; set; }
        public ICommand DeclineCmd { get; set; } = null;


        public ConfirmCommand(string message,
            Action<ConfirmCommand> confirmAction,
            Action<ConfirmCommand> cancelAction,
            Action<ConfirmCommand> declineAction,
            ConfirmOptions messageOptions)
            : base(message, messageOptions)
        {
            Message = message;
            _confirmAction = confirmAction;
            _declineAction = declineAction;
            _cancelAction = cancelAction;

            ConfirmCmd = new RelayCommand(x => _confirmAction(this));
            if (declineAction != null)
                DeclineCmd = new RelayCommand(x => _declineAction(this));
            if (cancelAction != null)
                CancelCmd = new RelayCommand(x => _cancelAction(this));
        }
        //public ConfirmCommand(string message,
        //  Action<ConfirmCommand> confirmAction,
        //  Action<ConfirmCommand> cancelAction,

        //  ConfirmOptions messageOptions)
        //  : base(message, messageOptions)
        //{
        //    Message = message;
        //    _confirmAction = confirmAction;
        //    _cancelAction = cancelAction;

        //    ConfirmCmd = new RelayCommand(x => _confirmAction(this));
        //    CancelCmd = new RelayCommand(x => _cancelAction(this));
        //}
        public override NotificationDisplayPart DisplayPart => _displayPart ?? (_displayPart = new ConfirmDisplayPart(this));

        #region binding properties

        private string _message;

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
