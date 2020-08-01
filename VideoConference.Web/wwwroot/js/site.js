// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function ShowPassword() {
    var passwordInput = document.getElementById("passwordInput");
    var passwordLockIcon = document.getElementById("password-lock-icon");

    if (passwordInput.type === "password") {
        passwordInput.type = "text";
    } else {
        passwordInput.type = "password";
    }

    passwordLockIcon.classList.toggle("fa-eye-slash");
}

function ShowConfirmPassword() {
    var ConfirmPasswordInput = document.getElementById("ConfirmPasswordInput");
    var ConfirmPasswordLockIcon = document.getElementById("confirm-password-lock-icon")
    if (ConfirmPasswordInput.type === "password") {
        ConfirmPasswordInput.type = "text";
    } else {
        ConfirmPasswordInput.type = "password";
    }

    ConfirmPasswordLockIcon.classList.toggle("fa-eye-slash");
}

function ShowDeptMeetings() {
    $.ajax({
        url: "/Home/DepartmentMeeting/",
        dataType: "html",
        data: {},
        success: function (result) {
            $("#sidebarbody").html(result);
            openNav();
        },
        error: function (xhr, status, error) {
            ShowSnackbarError("Oops, sorry! Error");
        }
    });
}

function SelectDeptMeeting() {
    var deptID = $('#Department').val();
    window.location.href = "/Department/Index/" + deptID;
}

function openNav() {
    var sideNav = document.getElementById("sidenav");
    sideNav.style.width = "300px";
    sideNav.classList.add("sideNavBorder");
}

function closeNav() {
    var sideNav = document.getElementById("sidenav");
    sideNav.style.width = "0";
    sideNav.classList.remove("sideNavBorder");
}