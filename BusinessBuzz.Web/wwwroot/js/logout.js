
document.addEventListener("DOMContentLoaded", function () {
    const logoutForm = document.getElementById("logoutForm");

    if (!logoutForm) {
        // This page does not contain a logout form.
        return;
    }

    const logoutButton = document.getElementById("logoutBtn");

    logoutForm.addEventListener("submit", function (event) {
        const confirmed = window.confirm(
            "Are you sure you want to log out of Business Buzz?"
        );

        if (!confirmed) {
            event.preventDefault();
            return;
        }

        if (logoutButton) {
            logoutButton.disabled = true;
            logoutButton.textContent = "Logging out...";
            logoutButton.setAttribute("aria-busy", "true");
        }

        // Allow the form to POST to the server-side logout handler.
        // The server, not browser storage, clears the session.
    });
});
