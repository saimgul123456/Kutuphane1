namespace Kutuphane1.Helpers
{
    public static class SweetAlertMessages
    {
        public static dynamic SuccesMessage { get; set; }
        public static dynamic InfoMessage { get; set; }
        public static dynamic WarningMessage { get; set; }
        public static dynamic ErrorMessage { get; set; }
        public static dynamic SavedMessage { get; set; }
        public static dynamic Message { get; set; }

        //html sayfasında @Html.Raw(AlertDefault); ile dönüştür
        //public static dynamic AlertDefault(string title, string message, NotificationType notificationType)
        //{
        //    var msg = $"<script type='text/javascript'>Swal.fire({{ icon: '{notificationType}', title: '{title}', text: '{message}',, confirmButtonText: 'Tamam' }})</script>";
        //    return msg;
        //}

        public static dynamic AlertDefault(string title, string message, NotificationType notificationType, string buttonText, string scriptl = null, string scriptr = null)
        {
            var msg = $"{scriptl}Swal.fire({{icon: '{notificationType}', title: '{title}', text: '{message}', confirmButtonText: '{buttonText}'}});{scriptr}";
            //var msg = "" + scripta + "Swal.fire({icon: '" + notificationType + "', title: '" + title + "', text: '" + message + "', confirmButtonText: '"+buttonText+"'});" + scriptb+"";
            return msg;
        }

        public static dynamic AlertButtons(string title, string message, NotificationType notificationType, int timer, string scriptl = null, string scriptr = null)
        {
            var msg = $"{scriptl}Swal.fire({{title: '{title}', icon: '{notificationType}', text: '{message}', confirmButtonText: 'Kapat', cancelButtonText: 'Vazgeç', showCancelButton: true, showCloseButton: true, timer{timer} }});{scriptr}";
            return msg;
        }

        //public static dynamic AlertSaved(string title, string message)
        //{
        //    var msg = $"<script type='text/javascript'>Swal.fire({{title: '{message}', showDenyButton: true, showCancelButton: true, confirmButtonText: 'Kaydet', cancelButtonText: 'Vazgeç', denyButtonText: 'Kaydetme'}}).then((result) => {{ if (result.isConfirmed) {{ Swal.fire({{title: '{message} Kaydedildi!', icon: 'success', text: '', confirmButtonText: 'Kapat' }});}} else if (result.isDenied) {{Swal.fire({{ title: '{message} işleminden vazgeçildi', icon: 'error', text: '', confirmButtonText: 'Kapat' }});}}}});</script>";
        //    return msg;
        //}

        public static dynamic AlertSaved(string title, string message, string scriptl = null, string scriptr = null)
        {
            var msg = $"{scriptl}Swal.fire({{title: '{message}', showDenyButton: true, showCancelButton: true, confirmButtonText: 'Kaydet', cancelButtonText: 'Vazgeç', denyButtonText: 'Kaydetme'}}).then((result) => {{ if (result.isConfirmed) {{ Swal.fire({{title: '{message} Kaydedildi!', icon: 'success', text: '', confirmButtonText: 'Kapat' }});}} else if (result.isDenied) {{Swal.fire({{ title: '{message} işleminden vazgeçildi', icon: 'error', text: '', confirmButtonText: 'Kapat' }});}}}});{scriptr}";
            return msg;
        }

        //public static dynamic AlertPosition(string title, NotificationType notificationType, string position, int timer)
        //{
        //    var msg = $"<script type='text/javascript'>Swal.fire({{position: '{position}', icon: '{notificationType}', title: '{title}', showConfirmButton: false, timer: '{timer}'}});</script>";
        //    return msg;
        //}

        public static dynamic AlertPosition(string title, NotificationType notificationType, string position, int timer, bool confirmBtn = false, string scriptl = null, string scriptr = null)
        {
            var msg = $"{scriptl}Swal.fire({{position: '{position}', icon: '{notificationType}', title: '{title}', showConfirmButton: {confirmBtn.ToString().ToLower()}, timer: '{timer}'}});{scriptr}";
            return msg;
        }
    }
}