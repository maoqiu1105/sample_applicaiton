// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('#btnToggleForm').click(function () {
    $('#divForm').show();
})

document.body.addEventListener('click', function (e) {
    var id = e.target.id;

    if (id == 'divForm') {
        $('#divForm').hide();
    }
    
})