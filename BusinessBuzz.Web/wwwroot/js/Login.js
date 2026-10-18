document.addEventListener("DOMContentLoaded",
    function () {
        const loginForm =document.getElementById("loginForm");
        
        if (!loginForm) {
            return;
        }
        
        loginForm.addEventListener("submit", function (event) {
            if(!loginForm.checkValidity()){
                event.preventDefault();
                loginForm.reportValidity();
                return
            }
            const loginButton = loginForm.querySelector('button[type="submit"]');
            if (loginButton) {
                loginButton.disabled = true;
                loginButton.textContent ="Logging in... ";
            }
        });
    }
);