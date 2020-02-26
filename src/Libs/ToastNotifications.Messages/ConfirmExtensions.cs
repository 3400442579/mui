using System;
using ToastNotifications.Core;
using ToastNotifications.Messages.Confirm;

namespace ToastNotifications.Messages
{
    public static class ConfirmExtensions {
        public static void ShowConfirm(this Notifier notifier,
                string message,
                Action<ConfirmCommand> confirmAction,
                ConfirmOptions messageOptions = null)
        {
            var options = messageOptions ?? new ConfirmOptions { ShowDeclineButton = false };
            notifier.Notify(() => new ConfirmCommand(message, confirmAction, n => n.Close(), null, options));
        }
    public static void ShowConfirm(this Notifier notifier, 
            string message, 
            Action<ConfirmCommand> confirmAction,
            Action<ConfirmCommand> cancelAction,
            ConfirmOptions messageOptions = null)
        {
            var options = messageOptions ?? new ConfirmOptions { ShowDeclineButton = false };
            notifier.Notify(() => new ConfirmCommand(message, confirmAction, cancelAction,null, options));
        }

        public static void ShowConfirm(this Notifier notifier,
           string message,
           Action<ConfirmCommand> confirmAction,
           Action<ConfirmCommand> declineAction,
           Action<ConfirmCommand> cancelAction,
           ConfirmOptions messageOptions = null)
        {
            notifier.Notify(() => new ConfirmCommand(message, confirmAction, cancelAction, declineAction, messageOptions));
        }
    }
}
