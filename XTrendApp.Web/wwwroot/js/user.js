// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {

    bindSaveUser();
    bindEditUser();
    bindDeleteUser();
    bindActivateUser();
    bindAddUser();
    bindPassword();
    bindChangePassword();

});

function bindSaveUser() {

    $("#btnSaveUser").on("click", function (e) {

        e.preventDefault();

        let id = $("#Id").val();

        let url = id == 0
            ? "/User/Create"
            : "/User/Update";

        $.ajax({

            url: url,

            type: "POST",

            data: $("#userForm").serialize(),

            success: function () {

                Swal.fire({

                    icon: "success",
                    title: "Success",
                    text: id == 0
                        ? "User created successfully."
                        : "User updated successfully.",

                    timer: 1500,
                    showConfirmButton: false

                }).then(() => {

                    location.reload();

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

}

function bindEditUser() {

    $(document).on("click", ".btn-edit", function () {

        let id = $(this).data("id");

        $.get("/User/Edit/" + id, function (user) {

            $("#Id").val(user.id);
            $("#Username").val(user.username);
            $("#FullName").val(user.fullName);
            $("#Email").val(user.email);
            $("#IsAdmin").prop("checked", user.isAdmin);
            $("#IsActive").prop("checked", user.isActive);

            $("#userModalTitle").text("Edit User");

            $("#btnSaveUser").text("Save Changes");

            // Şifre alanlarını gizle
            $(".password-group").hide();

            $("#Password").val("");
            $("#ConfirmPassword").val("");

           

        });

    });

}

function bindDeleteUser() {

    $(document).on("click", ".btn-delete", function () {

        const button = $(this);

        const id = button.data("id");

        const username = button.data("name");

        Swal.fire({

            title: "Delete User?",

            text: username + " user will be deactivated.",

            icon: "warning",

            showCancelButton: true,

            confirmButtonText: "Yes",

            cancelButtonText: "Cancel",

            confirmButtonColor: "#d33"

        }).then((result) => {

            if (!result.isConfirmed)
                return;

            $.ajax({

                url: "/User/Delete",

                type: "POST",

                data: {

                    id: id,

                    __RequestVerificationToken:
                        $('input[name="__RequestVerificationToken"]').val()

                },

                success: function () {

                    Swal.fire({

                        icon: "success",

                        title: "Deleted",

                        text: "User has been deactivated.",

                        timer: 1200,

                        showConfirmButton: false

                    }).then(() => {

                        location.reload();

                    });

                },

                error: function (xhr) {

                    Swal.fire({

                        icon: "error",

                        title: "Error",

                        text: xhr.responseText

                    });

                    Swal.fire({
                        icon: "info",
                        title: "Information",
                        text: xhr.responseText
                    });

                }

            });

        });

    });

}

function bindActivateUser() {

    $(document).on("click", ".btn-activate", function () {

        const id = $(this).data("id");

        const username = $(this).data("name");

        Swal.fire({

            title: "Activate User?",

            text: username + " user will be reactivated.",

            icon: "question",

            showCancelButton: true,

            confirmButtonText: "Activate",

            cancelButtonText: "Cancel"

        }).then((result) => {

            if (!result.isConfirmed)
                return;

            $.ajax({

                url: "/User/Activate",

                type: "POST",

                data: {

                    id: id,

                    __RequestVerificationToken:
                        $('input[name="__RequestVerificationToken"]').val()

                },

                success: function () {

                    Swal.fire({

                        icon: "success",

                        title: "Success",

                        text: "User activated.",

                        timer: 1200,

                        showConfirmButton: false

                    }).then(() => {

                        location.reload();

                    });

                },

                error: function (xhr) {

                    Swal.fire({

                        icon: "info",

                        title: "Information",

                        text: xhr.responseText

                    });

                }

            });

        });

    });

}

function resetUserForm() {

    $("#Id").val(0);

    $("#userForm")[0].reset();

    $(".password-group").show();

    $("#userModalTitle").text("Add User");

    $("#btnSaveUser").text("Save User");

}


function bindAddUser() {

    $("#btnAddUser").on("click", function () {

        resetUserForm();

    });

}

function bindPassword() {

    $(document).on("click", ".btn-password", function () {

        $("#passwordForm")[0].reset();

        $("#passwordForm #Id").val($(this).data("id"));

        $("#passwordForm #Username").val($(this).data("name"));

    });

}

function bindChangePassword() {

    $("#btnChangePassword").on("click", function (e) {

        e.preventDefault();

        $.ajax({

            url: "/User/ChangePassword",

            type: "POST",

            data: $("#passwordForm").serialize(),

            success: function () {

                Swal.fire({

                    icon: "success",

                    title: "Success",

                    text: "Password changed successfully.",

                    timer: 1500,

                    showConfirmButton: false

                }).then(() => {

                    location.reload();

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

}