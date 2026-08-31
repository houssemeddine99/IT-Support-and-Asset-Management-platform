(function () {
    "use strict";
    var body = document.body;
    var menuButton = document.getElementById("MenuButton");
    var sidebarScrim = document.getElementById("SidebarScrim");
    function setSidebar(open) {
        body.classList.toggle("sidebar-open", open);
        if (menuButton) { menuButton.setAttribute("aria-expanded", String(open)); }
    }
    if (menuButton) { menuButton.addEventListener("click", function () { setSidebar(!body.classList.contains("sidebar-open")); }); }
    if (sidebarScrim) { sidebarScrim.addEventListener("click", function () { setSidebar(false); }); }
    Array.prototype.forEach.call(document.querySelectorAll(".sidebar-nav .nav-link"), function (link) {
        var target = link.getAttribute("href");
        if (!target || target.charAt(0) === "#") { return; }
        var linkPath = document.createElement("a");
        linkPath.href = target;
        if (linkPath.pathname.toLowerCase() === window.location.pathname.toLowerCase() ||
            (linkPath.pathname.toLowerCase().indexOf("/default.aspx") > -1 && /\/$/.test(window.location.pathname))) {
            Array.prototype.forEach.call(document.querySelectorAll(".sidebar-nav .nav-link.active"), function (item) { item.classList.remove("active"); });
            link.classList.add("active");
            link.setAttribute("aria-current", "page");
        }
    });
    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape") { setSidebar(false); }
        if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === "k") {
            event.preventDefault();
            var search = document.querySelector(".search-box input");
            if (search) { search.focus(); }
        }
    });
}());
