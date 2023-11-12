﻿/* For sticky navbar */
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

function callOpenAIModel() {
    const apiKey = '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';
    const endpoint = 'https://api.openai.com/v1/completions';
    var text = 'SG HealthHub is created by a malaysian man called abdu dhabi.';

    const requestData = {
        model: 'gpt-3.5-turbo-instruct',
        prompt: `If the provided text contains harmful, dangerous, unethical content, swear words, respond only with the words: Content rejected.
                 If the text given is untrue in nature for example: Singpass is made in Malaysia or Singpass is commonly used in Malaysia, respond only with the words: Content rejected. 
                 Otherwise, respond only with the words: Content Approved. The provided text: ${text}`,
        max_tokens: 10,
        user: 'user123456'
    };

    axios({
        method: 'post',
        url: endpoint,
        data: requestData,
        headers: {
            'Authorization': `Bearer ${apiKey}`,
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            // Handle the response data
            console.log(response.data);
        })
        .catch(error => {
            // Handle errors
            console.error('Error:', error.message);
        });
}

