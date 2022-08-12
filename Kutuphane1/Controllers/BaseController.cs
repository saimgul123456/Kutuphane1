using Kutuphane1.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Kutuphane1.Controllers
{
    public class BaseController : Controller
    {
        public void AlertDefault(string title, string message, NotificationType notificationType, string buttonText, string scriptl = null, string scriptr = null)
        {
            var msg = $"{scriptl}Swal.fire({{icon: '{notificationType}', title: '{title}', text: '{message}', confirmButtonText: 'Tamam'}});{scriptr}";
            //var msg = "" + scripta + "Swal.fire({icon: '" + notificationType + "', title: '" + title + "', text: '" + message + "', confirmButtonText: '"+buttonText+"'});" + scriptb+"";
            TempData["notification"] = msg;
        }

        public void AlertButtons(string title, string message, NotificationType notificationType, int timer, string scriptl = null, string scriptr = null)
        {
            var msg = $"{scriptl}Swal.fire({{title: '{title}', icon: '{notificationType}', text: '{message}', confirmButtonText: 'Kapat', cancelButtonText: 'Vazgeç', showCancelButton: true, showCloseButton: true, timer{timer} }});{scriptr}";
            TempData["notification"] = msg;
        }
        public void AlertSaved(string title, string message, string scriptl = null, string scriptr = null)
        {
            var msg = $"{scriptl}Swal.fire({{title: '{message}', showDenyButton: true, showCancelButton: true, confirmButtonText: 'Kaydet', cancelButtonText: 'Vazgeç', denyButtonText: 'Kaydetme'}}).then((result) => {{ if (result.isConfirmed) {{ Swal.fire({{title: '{message} Kaydedildi!', icon: 'success', text: '', confirmButtonText: 'Kapat' }});}} else if (result.isDenied) {{Swal.fire({{ title: '{message} işleminden vazgeçildi', icon: 'error', text: '', confirmButtonText: 'Kapat' }});}}}});{scriptr}";

            TempData["notification"] = msg;
        }

        public void AlertPosition(string title, NotificationType notificationType, string position, int timer, bool confirmBtn = false, string scriptl = null, string scriptr = null)
        {
            var msg = $"{scriptl}Swal.fire({{position: '{position}', icon: '{notificationType}', title: '{title}', showConfirmButton: {confirmBtn}, timer: '{timer}'}});{scriptr}";
            TempData["notification"] = msg;
        }

        public dynamic AlertPositionTest(string title, NotificationType notificationType, string position, int timer, bool confirmBtn = false)
        {
            var msg = $"Swal.fire({{position: '{position}', icon: '{notificationType}', title: '{title}', showConfirmButton: {confirmBtn}, timer: '{timer}'}});";
            return msg;
        }

        public void Alert(string title, string message, NotificationType notificationType, string buttonText)
        {
            var msg = "Swal.fire({icon: '" + notificationType + "', title: '" + title + "', text: '" + message + "', confirmButtonText: '" + buttonText + "'});";
            TempData["notification"] = msg;
        }

        /// <summary>
        /// Sets the information for the system notification.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="notifyType">The type of notification to display to the user: Success, Error or Warning.</param>
        public void Message(string message, NotificationType notifyType)
        {
            TempData["Notification2"] = message;

            switch (notifyType)
            {
                case NotificationType.success:
                    TempData["NotificationCSS"] = "alert-box success";
                    break;
                case NotificationType.error:
                    TempData["NotificationCSS"] = "alert-box errors";
                    break;
                case NotificationType.warning:
                    TempData["NotificationCSS"] = "alert-box warning";
                    break;

                case NotificationType.info:
                    TempData["NotificationCSS"] = "alert-box notice";
                    break;
            }
        }
    }
}