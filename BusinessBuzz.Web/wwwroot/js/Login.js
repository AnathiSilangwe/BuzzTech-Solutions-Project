
document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("loginForm");

    if (!loginForm) {
        console.error("Business Buzz: Login form not found.");
        return;
    }

    const emailInput = document.getElementById("Email")
        || document.getElementById("email");

    const passwordInput = document.getElementById("Password")
        || document.getElementById("password");

    const submitButton = loginForm.querySelector(
        'button[type="submit"], input[type="submit"]'
    );

    // Allow normal HTML validation and Razor Pages form submission.
    loginForm.addEventListener("submit", function (event) {
        if (!loginForm.checkValidity()) {
            event.preventDefault();
            loginForm.reportValidity();
            return;
        }

        if (emailInput) {
            emailInput.value = emailInput.value.trim();
        }

        if (submitButton) {
            submitButton.disabled = true;
            submitButton.classList.add("is-loading");
            submitButton.setAttribute("aria-busy", "true");

            if (submitButton.tagName === "BUTTON") {
                submitButton.dataset.originalText =
                    submitButton.textContent.trim();

                submitButton.textContent = "Logging in...";
            }
        }

        // IMPORTANT:
        // Do not call preventDefault() for valid submissions.
        // The browser must POST the form to Login.cshtml.cs.
    });
});
