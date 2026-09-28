const loginForm = document.getElementById("loginForm");

loginForm.addEventListener("submit", function (event) {

    event.preventDefault();

    const email =
        document.getElementById("email").value;

    const password =
        document.getElementById("password").value;

    console.log("Login form ready for API integration.");

    console.log("Email:", email);
    window.location.href =
        "/pages/entrepreneur/profile-confirmation.html";
});