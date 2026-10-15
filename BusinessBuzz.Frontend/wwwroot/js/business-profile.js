

document.addEventListener("DOMContentLoaded", function () {

    const form = document.getElementById("businessProfileForm");
    const editButton = document.getElementById("editProfileBtn");
    const cancelButton = document.getElementById("cancelProfileBtn");
    const actions = document.getElementById("profileFormActions");
    const message = document.getElementById("profileMessage");

    const STORAGE_KEY = "businessBuzzBusinessProfile";

    const fields = Array.from(
        form.querySelectorAll("input, select")
    );

    let originalValues = {};

    function getProfileData() {
        const profile = {};

        fields.forEach(function (field) {
            profile[field.id] = field.value;
        });

        return profile;
    }

    function captureValues() {
        originalValues = getProfileData();
    }

    function loadSavedProfile() {
        try {
            const saved = localStorage.getItem(STORAGE_KEY);

            if (!saved) {
                return;
            }

            const profile = JSON.parse(saved);

            if (!profile || typeof profile !== "object" ||
                Array.isArray(profile)) {
                return;
            }

            fields.forEach(function (field) {
                if (typeof profile[field.id] === "string") {
                    field.value = profile[field.id];
                }
            });

        } catch (error) {
            console.error("Unable to load business profile:", error);
        }
    }

    function setEditing(enabled) {
        fields.forEach(function (field) {
            field.disabled = !enabled;
        });

        editButton.hidden = enabled;
        actions.hidden = !enabled;
    }

    function showMessage(text, type) {
        message.className = "tax-form-message " + type;
        message.textContent = text;
    }

    editButton.addEventListener("click", function () {
        captureValues();

        message.textContent = "";
        message.className = "tax-form-message";

        setEditing(true);
    });

    cancelButton.addEventListener("click", function () {
        fields.forEach(function (field) {
            field.value = originalValues[field.id];
        });

        setEditing(false);

        showMessage(
            "Your edits have been cancelled.",
            "success"
        );
    });

    form.addEventListener("submit", function (event) {
        event.preventDefault();

        if (!form.reportValidity()) {
            return;
        }

        const profile = getProfileData();

        try {
            localStorage.setItem(
                STORAGE_KEY,
                JSON.stringify(profile)
            );
        } catch (error) {
            console.error("Unable to save business profile:", error);

            showMessage(
                "Unable to save changes in this browser.",
                "error"
            );

            return;
        }

        captureValues();
        setEditing(false);

        showMessage(
            "Business profile saved successfully for this prototype.",
            "success"
        );
    });

    loadSavedProfile();
    captureValues();
    setEditing(false);
});
