/* For sticky navbar */
window.onscroll = function() {myFunction()};

var navbar = document.getElementById("navbar");
var sticky = navbar.offsetTop;

function myFunction() {
if (window.pageYOffset >= sticky) {
    navbar.classList.add("sticky")
} else {
    navbar.classList.remove("sticky");
}
}


/* Fap material (Clarence noodes)*/
$(document).ready(function () {
    // Cache the table for better performance
    var table = $("#viewPost");

    // Attach click event handlers to the sorting links
    $("#sort-newest").on("click", function (e) {
        e.preventDefault();
        sortTableDesc();
    });

    $("#sort-oldest").on("click", function (e) {
        e.preventDefault();
        sortTableAsc();
    });

    function sortTableDesc() {
        var rows = table.find("tbody > tr").get();
        rows.sort(function (a, b) {
            var dateA = new Date($(a).find("td:nth-child(7)").text());
            var dateB = new Date($(b).find("td:nth-child(7)").text());
            return dateB - dateA; // Sort in descending order
        });
        table.find("tbody").empty().append(rows);
    }

    function sortTableAsc() {
        var rows = table.find("tbody > tr").get();
        rows.sort(function (a, b) {
            var dateA = new Date($(a).find("td:nth-child(7)").text());
            var dateB = new Date($(b).find("td:nth-child(7)").text());
            return dateA - dateB; // Sort in ascending order
        });
        table.find("tbody").empty().append(rows);
    }
});