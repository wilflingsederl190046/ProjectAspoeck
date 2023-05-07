import $ from '../jquery.module.js';

console.log('index.js loaded!');

function deleteUser(userId) {
    if (confirm('Sind Sie sicher, dass Sie diesen Benutzer löschen möchten?')) {
        $.ajax({
            url: '@Url.Action("DeleteUser", "Admin")',
            type: 'POST',
            data: { userId: userId },
            success: function(result) {
                if (result.success) {
                    location.reload();
                } else {
                    alert(result.message);
                }
            },
            error: function(xhr, textStatus, errorThrown) {
                alert('Ein Fehler ist aufgetreten: ' + errorThrown);
            }
        });
    }
}