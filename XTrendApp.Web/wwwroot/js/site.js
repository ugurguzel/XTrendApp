// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).on("click", "#btnChangeMyPassword", function (e) {

    e.preventDefault();

    $.ajax({

        url: "/Account/ChangeMyPassword",

        type: "POST",

        data: $("#myPasswordForm").serialize(),

        success: function () {

            Swal.fire({

                icon: "success",

                title: "Password Changed",

                text: "Your password has been changed successfully. Please sign in again.",

                confirmButtonText: "Login"

            }).then(() => {

                window.location = "/Account/Login";

            });

        },

        error: function (xhr) {

            Swal.fire({

                icon: "error",

                title: "Error",

                text: xhr.responseText

            });

        }

    });

});

$('#myPasswordModal').on('show.bs.modal', function () {

    $("#myPasswordForm")[0].reset();

});
