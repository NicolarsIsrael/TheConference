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

function ShowDepartmentMemo() {
    $.ajax({
        url: "/Home/DepartmentMemo/",
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

function ShowDeleteUser(id) {
    $.ajax({
        url: "/Users/DeleteUser/",
        dataType: "html",
        data: {id:id},
        success: function (result) {
            $("#sidebarbody").html(result);
            openNav();
        },
        error: function (xhr, status, error) {
            ShowSnackbarError("Oops, sorry! Error");
        }
    });
}

function DeleteUser(id) {
    window.location.href = "/Users/ConfirmDeleteUser?id=" + id;
}

function CopyToClipBoard(copyValue) {
    // Create new element
    var el = document.createElement('textarea');
    // Set value (string to be copied)
    el.value = copyValue;
    // Set non-editable to avoid focus and move outside of view
    el.setAttribute('readonly', '');
    el.style = { position: 'absolute', left: '-9999px' };
    document.body.appendChild(el);
    // Select text inside element
    el.select();
    // Copy text to clipboard
    document.execCommand('copy');
    // Remove temporary element
    document.body.removeChild(el);

    openAlert();
    var customAlert = document.getElementById("custom-alert");
    customAlert.style.width = "100px";
    customAlert.style.minHeight = "50px";
    customAlert.style.backgroundColor = "#e6630e";
    customAlert.style.color = "white";
    document.getElementById("custom-alert-body").innerHTML = "<p class='px-3'>Copied!</p>";

    setTimeout(function () {
        closeAlert();
    }, 3000);
    
}

function SelectDeptMeeting() {
    var deptID = $('#Department').val();
    window.location.href = "/Department/Index/" + deptID;
}

function SelectDeptMemo() {
    var deptID = $('#Department').val();
    window.location.href = "/Memo/Index/" + deptID;
}

function openAlert() {
    var customAlert = document.getElementById("custom-alert");
    customAlert.style.width = "300px";
    customAlert.classList.add("sideNavBorder");
}

function closeAlert() {
    var customAlert = document.getElementById("custom-alert");
    customAlert.style.width = "0";
    customAlert.classList.remove("sideNavBorder");
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