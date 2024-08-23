// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

    document.addEventListener("DOMContentLoaded", function () {
            const searchInput = document.querySelector('.search-input');
    const dropdown = document.querySelector('.dropdown-select');

    // Show dropdown when the search input is focused
    searchInput.addEventListener('focus', function () {
        dropdown.style.display = 'block'; // Show the dropdown
        dropdown.style.opacity = '1'; // Make it fully opaque
        dropdown.style.visibility = 'visible'; // Ensure it's visible
            });

    // Hide dropdown when clicking outside of the input and dropdown
    document.addEventListener('click', function (event) {
                if (!searchInput.contains(event.target) && !dropdown.contains(event.target)) {
        dropdown.style.display = 'none'; // Hide the dropdown
                }
            });
        });

