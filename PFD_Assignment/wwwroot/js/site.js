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
    var container = $("#guidecontent");

    function attachSortEventHandler(linkSelector, sortFunction) {
        $(linkSelector).on("click", function (e) {
            e.preventDefault();
            sortFunction();
        });
    }

    function sortTableDesc() {
        var guides = container.find(".guides").get();
        sortRowsByDate(guides, true);
    }

    function sortTableAsc() {
        var guides = container.find(".guides").get();
        sortRowsByDate(guides, false);
    }

    function sortRowsByDate(rows, descending) {
        rows.sort(function (a, b) {
            var dateA = convertToDateSortableString($(a).data("date"));
            var dateB = convertToDateSortableString($(b).data("date"));

            return descending ? dateB.localeCompare(dateA) : dateA.localeCompare(dateB);
        });

        container.empty().append(rows);
    }

    function convertToDateSortableString(dateString) {
        // Custom parsing for the specific date format "DD/MM/YYYY h:mm:ss a"
        var parts = dateString.match(/(\d+)\/(\d+)\/(\d+) (\d+):(\d+):(\d+) (am|pm)/i);
        if (parts) {
            var year = parseInt(parts[3], 10);
            var month = parseInt(parts[2], 10) - 1; // Months are zero-based in JavaScript
            var day = parseInt(parts[1], 10);
            var hours = parseInt(parts[4], 10) + (parts[7].toLowerCase() === "pm" ? 12 : 0);
            var minutes = parseInt(parts[5], 10);
            var seconds = parseInt(parts[6], 10);

            var parsedDate = new Date(year, month, day, hours, minutes, seconds);
            if (!isNaN(parsedDate)) {
                // If parsing succeeds, return the ISO-formatted string
                return parsedDate.toISOString();
            }
        }

        // If parsing fails, log an error and return an empty string or null
        console.error("Invalid date string:", dateString);
        return "";
    }

    attachSortEventHandler("#sort-newest", sortTableDesc);
    attachSortEventHandler("#sort-oldest", sortTableAsc);
});

