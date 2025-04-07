$(document).ready(function () {
    var notification = $('#notification');
    if (notification.length > 0) {
        var notificationMessage = notification.html();
        var notificationType = notification.data('notify-type');

        $.notify({
            message: notificationMessage
        }, {
            type: notificationType,
            placement: {
                from: 'top',
                align: 'right'
            },
            delay: 3000,
            offset: { x: 20, y: 60 }
        });

        notification.remove();
    }
});
$(document).ready(function () {
    'use strict';

    $('.needs-validation').each(function () {
        $(this).on('submit', function (event) {
            if (!this.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            $(this).addClass('was-validated');
        });
    });
});
function showConfirmationDialog(title, text, confirmText, cancelText, onConfirm) {
    Swal.fire({
        title: title || 'Ви вневнені?',
        text: text || 'Цю дію неможливо відмінити!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#31ce36',
        cancelButtonColor: '#f25961',
        confirmButtonText: confirmText || 'Ок',
        cancelButtonText: cancelText || 'Відмінити'
    }).then((result) => {
        if (result.isConfirmed && typeof onConfirm === 'function') {
            onConfirm();
        }
    });
}