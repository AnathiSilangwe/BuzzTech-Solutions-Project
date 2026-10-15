
document.addEventListener("DOMContentLoaded", function () {
    alert("Signup JavaScript loaded successfully.");

    const signupForm = document.getElementById("signupForm");
    const signupButton = signupForm
        ? signupForm.querySelector('button[type="submit"]')
        : null;

    if (!signupForm || !signupButton) {
        alert("ERROR: Signup form or Create Account button was not found.");
        return;
    }

    signupButton.addEventListener("click", function () {
        alert("Create Account button was clicked.");
    });

    signupForm.addEventListener("submit", function (event) {
        event.preventDefault();

        alert("Signup form submitted successfully.");

        const password = document.getElementById("password").value;
        const confirmPassword =
            document.getElementById("confirmPassword").value;

        if (password !== confirmPassword) {
            alert("Passwords do not match.");
            return;
        }

        alert("Form works! Actual account creation still needs backend integration.");
    });
});
