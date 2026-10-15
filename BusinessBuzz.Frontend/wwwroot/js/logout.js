
document.addEventListener("DOMContentLoaded", function () {
    const logoutBtn = document.getElementById("logoutBtn");

    if (!logoutBtn) return;

    logoutBtn.addEventListener("click", function () {
        const confirmed = window.confirm(
            "Are you sure you want to log out?"
        );

        if (!confirmed) return;

        sessionStorage.removeItem("businessBuzzUser");
        sessionStorage.removeItem("businessBuzzRole");
        sessionStorage.removeItem("businessBuzzAuthenticated");

        localStorage.removeItem("businessBuzzUser");
        localStorage.removeItem("businessBuzzRole");
        localStorage.removeItem("businessBuzzAuthenticated");

        window.location.replace("/pages/login.html");
    });
});
