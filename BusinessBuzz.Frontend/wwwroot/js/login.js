

document.addEventListener("DOMContentLoaded", function () {

    const loginForm = document.getElementById("loginForm");

    if (!loginForm) {
        console.error("Login form not found.");
        return;
    }

    const params = new URLSearchParams(window.location.search);

    const selectedRole = params.get("portal");

    const dashboardRoutes = {
        entrepreneur: "/pages/entrepreneur/profile-confirmation.html",
        approver: "/pages/approver/dashboard.html",
        bank: "/pages/bank/dashboard.html"
    };

    loginForm.addEventListener("submit", function (event) {
        event.preventDefault();

        const email = document.getElementById("email").value.trim();
        const password = document.getElementById("password").value;

        if (!email || !password) {
            alert("Please enter your email and password.");
            return;
        }

        if (!Object.hasOwn(dashboardRoutes, selectedRole)) {
            alert(
                "Please select a valid portal from the portal selection page."
            );

            window.location.href = "/pages/choose-portal.html";
            return;
        }

        // Frontend demonstration only.
        // No real authentication takes place here.
        const confirmed = window.confirm(
            "Development demonstration only. " +
            "Credentials are not verified. " +
            "Continue to the " + selectedRole + " portal?"
        );

        if (!confirmed) {
            return;
        }

        window.location.href = dashboardRoutes[selectedRole];
    });

});
