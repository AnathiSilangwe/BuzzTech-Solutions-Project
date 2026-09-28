const confirmProfileBtn =
    document.getElementById("confirmProfileBtn");

confirmProfileBtn.addEventListener("click", function () {

    console.log("Business profile confirmed.");

    // Temporary frontend navigation.
    // Later this will occur after the backend saves
    // the confirmed business profile.

    window.location.href =
        "/pages/entrepreneur/dashboard.html";
});
