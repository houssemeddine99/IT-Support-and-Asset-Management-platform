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
    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape") { setSidebar(false); }
        if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === "k") {
            event.preventDefault();
            var search = document.querySelector(".search-box input");
            if (search) { search.focus(); }
        }
    });
}());

