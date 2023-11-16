

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

/* Sorting for guide by dates and likes*/
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
            console.log(dateA,dateB);
            return descending ? dateB.localeCompare(dateA) : dateA.localeCompare(dateB);
        });

        container.empty().append(rows);
    }

    function sortRowsByLikes(rows, descending) {
        rows.sort(function (a, b) {
            var likeA = parseInt($(a).data("likes")) || 0;
            var likeB = parseInt($(b).data("likes")) || 0;

            return descending ? likeB - likeA : likeA - likeB;
        });

        // Assuming "container" is defined somewhere in your code
        // If not, you may need to replace it with the appropriate container reference

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

    function sortTableDescLikes() {
        var guides = container.find(".guides").get();
        console.log("Before sorting:", guides);
        sortRowsByLikes(guides, false); // Change to true for descending order
        console.log("After sorting:", guides);
    }

    function sortTableAscLikes() {
        var guides = container.find(".guides").get();
        console.log("Before sorting:", guides);
        sortRowsByLikes(guides, false); // Change to true for descending order
        console.log("After sorting:", guides);
    }
    /*function sortRowsByLikes(rows, descending) {
        // Create a new container to append the sorted rows
        var newContainer = $("<div>");

        // Sort the rows
        rows.sort(function (a, b) {
            var likesA = parseInt($(a).data("likes"));
            var likesB = parseInt($(b).data("likes"));

            // Compare the number of likes
            return descending ? likesB - likesA : likesA - likesB;
        });

        // Append the sorted rows to the new container
        newContainer.append(rows);

        // Replace the content of the original container with the new container's content
        container.html(newContainer.html());
    }
*/


    attachSortEventHandler("#sort-newest", sortTableDesc);
    attachSortEventHandler("#sort-oldest", sortTableAsc);
    attachSortEventHandler("#sort-mostliked", sortTableDescLikes);
    attachSortEventHandler("#sort-leastliked", sortTableAscLikes);
});


/* api */
function callOpenAIModel() {
    const apiKeyContainer = document.getElementById('apiKeyContainer');
    const apiKey = apiKeyContainer.dataset.apiKey;
    const endpoint = 'https://api.openai.com/v1/completions';
    var text = 'SG HealthHub is created by a singaporean man.';

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

let popupCount = 0;
let zIndexCounter = 1;
const activePopups = [];

function showMessage() {
    const message = `Message ${popupCount + 1}`; // Unique message for each pop-up

    const popUp = document.createElement('div');
    popUp.classList.add('popup-message');
    popUp.textContent = message;
    popUp.style.zIndex = zIndexCounter; // Set the z-index for new pop-up

    document.body.appendChild(popUp);

    const popupData = {
        element: popUp,
        index: zIndexCounter,
    };

    activePopups.push(popupData);
    zIndexCounter++;

    setTimeout(() => {
        removePopup(popupData);
    }, 5000);

    popupCount++;
}

function removePopup(popupData) {
    const index = activePopups.indexOf(popupData);
    if (index > -1) {
        activePopups.splice(index, 1);
        popupData.element.remove();
    }
}

function showProfile() {
        document.getElementById("profileContent").style.display = "block";
        document.getElementById("loginSecurityContent").style.display = "none";
    }

    function showLoginSecurity() {
        document.getElementById("profileContent").style.display = "none";
        document.getElementById("loginSecurityContent").style.display = "block";
}

/* To hide featured post when sort button is clicked */

$(document).ready(function () {
    // Define a flag to track whether the sorting button was clicked
    var sortingButtonClicked = false;

    // Toggle the visibility of featured posts based on the flag
    function toggleFeaturedPosts() {
        if (sortingButtonClicked) {
            $("#guidecontent2").hide();
        } else {
            $("#guidecontent2").show();
        }
    }

    // Attach a click event handler to the sorting buttons
    $("#sort-newest, #sort-oldest, #sort-mostliked").click(function () {
        sortingButtonClicked = true;
        toggleFeaturedPosts();
    });
});
