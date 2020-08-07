var dashboardSieNavOpen = true;
var dashboardSidenav = document.getElementById("dashboard-sidenav");

if (window.screen.width < 991) {
    dashboardSieNavOpen = false;
    console.log(dashboardSieNavOpen);
}

function toggleDashboardNav() {
    if (dashboardSieNavOpen == true)
        closeDashboardSideNav();
    else {
        openDashboardSideNav();
    }
}

window.onresize = function () {
    if ($(document).width() <= 990) {
        document.getElementById("dashboard-sidenav").style.width = "0";
        document.getElementById("main-content").style.marginLeft = "0";
        dashboardSieNavOpen = false;
    }
    dashboardSidenav.style.minHeight = $(document).height();
}

/* Set the width of the side navigation to 250px and the left margin of the page content to 250px */
function openDashboardSideNav() {
    if ($(document).width() < 991) {
        document.getElementById("dashboard-sidenav").style.width = "250px";

    } else {
        document.getElementById("dashboard-sidenav").style.width = "250px";
        document.getElementById("main-content").style.marginLeft = "250px";
    }
    dashboardSieNavOpen = true;
    SetSideNavHeight();
}

/* Set the width of the side navigation to 0 and the left margin of the page content to 0 */
function closeDashboardSideNav() {
    if ($(document).width() < 991) {
        document.getElementById("dashboard-sidenav").style.width = "0";
    } else {
        document.getElementById("dashboard-sidenav").style.width = "0px";
    }
    document.getElementById("main-content").style.marginLeft = "0";
    dashboardSieNavOpen = false;
}

//function mediumScreenCloseSideNav() {
//    if (window.screen.width < 991) {
//        closeDashboardSideNav();
//    }
//}

$(".dashboard-dropdown > a").click(function () {
    $(".dashboard-dropdown-item").slideUp(200);
    if (
        $(this)
            .parent()
            .hasClass("active")
    ) {
        $(".dashboard-dropdown").removeClass("active");
        $(this)
            .parent()
            .removeClass("active");
    } else {
        $(".dashboard-dropdown").removeClass("active");
        $(this)
            .next(".dashboard-dropdown-item")
            .slideDown(200);
        $(this)
            .parent()
            .addClass("active");
    }
});

$(document).ready(function () {
    SetSideNavHeight();
});

function SetSideNavHeight() {
    dashboardSidenav.style.minHeight = $(document).height() + "px";
}