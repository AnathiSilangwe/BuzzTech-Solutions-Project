const loginForm = document.getElementById("loginForm");

if (loginForm) {
    loginForm.addEventListener("submit", function () {
        const submitButton = loginForm.querySelector('button[type="submit"]');

        if (submitButton) {
            submitButton.disabled = true;
            submitButton.textContent = "Logging in...";
        }

        // Do not prevent the default form submission.
        // ASP.NET Razor Pages must receive the POST request.
    });
}