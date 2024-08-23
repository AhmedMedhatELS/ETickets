
function validateCinemaSelection(movieId) {
    var selectElement = document.getElementById("CinemaSelect-" + movieId);
    var validationMessage = document.getElementById("validationMessage-" + movieId);

    if (!selectElement.value) {
        // Show validation message if no cinema is selected
        validationMessage.style.display = "block";
        return false; // Prevent form submission
    }

    validationMessage.style.display = "none"; // Hide validation message
    return true; // Allow form submission
}

// Add this event listener to hide validation message when a cinema is selected
function addCinemaSelectionListener(movieId) {
    var selectElement = document.getElementById("CinemaSelect-" + movieId);
    var validationMessage = document.getElementById("validationMessage-" + movieId);
    var quantity = document.getElementById("TicketQuantity-" + movieId);

    selectElement.addEventListener("change", function () {
        if (selectElement.value) {
            validationMessage.style.display = "none"; // Hide validation message

            quantity.value = selectElement.options[selectElement.selectedIndex].getAttribute("data-quantity");
        }
    });
}

function incrementTicketQuantity(movieId) {
    var quantityInput = document.getElementById("TicketQuantity-" + movieId);
    quantityInput.value = parseInt(quantityInput.value) + 1;
}

function decrementTicketQuantity(movieId) {
    var quantityInput = document.getElementById("TicketQuantity-" + movieId);
    if (quantityInput.value > 0) {
        quantityInput.value = parseInt(quantityInput.value) - 1;
    }
}

