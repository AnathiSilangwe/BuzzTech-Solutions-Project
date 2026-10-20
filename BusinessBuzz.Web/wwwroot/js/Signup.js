
document.addEventListener("DOMContentLoaded", function () {
    const signupForm = document.getElementById("signupForm");

    if (!signupForm) {
        console.error("Signup form not found.");
        return;
    }

    signupForm.addEventListener("submit", function (event) {
        const password =
            document.getElementById("password").value;

        const confirmPassword =
            document.getElementById("confirmPassword").value;

        if (password !== confirmPassword) {
            event.preventDefault();
            alert("Passwords do not match.");
            return;
        }

        const signupButton =
            signupForm.querySelector('button[type="submit"]');

        if (signupButton) {
            signupButton.disabled = true;
            signupButton.textContent = "Creating Account...";
        }

        // Do not prevent the valid form submission.
        // ASP.NET Core will handle the POST request.
    });
});
