document.addEventListener("DOMContentLoaded", function () {
    const widget = document.getElementById("buzzGuideWidget");

    if (!widget) return;

    const launcher = widget.querySelector("#buzzGuideLauncher");
    const panel = widget.querySelector("#buzzGuidePanel");
    const closeButton = widget.querySelector("#buzzGuideClose");
    const questionInput = widget.querySelector("#buzzGuideQuestion");
    const form = widget.querySelector("#buzzGuideForm");
    const messages = widget.querySelector("#buzzGuideMessages");

    function setOpen(isOpen) {
        widget.classList.toggle("is-open", isOpen);
        launcher.setAttribute("aria-expanded", String(isOpen));
        panel.setAttribute("aria-hidden", String(!isOpen));

        if (isOpen && questionInput) {
            questionInput.focus();
        }
    }

    launcher.addEventListener("click", function () {
        setOpen(!widget.classList.contains("is-open"));
    });

    closeButton.addEventListener("click", function () {
        setOpen(false);
        launcher.focus();
    });

    widget.querySelectorAll("[data-question]").forEach(function (button) {
        button.addEventListener("click", function () {
            questionInput.value = button.dataset.question;
            questionInput.focus();
        });
    });

    if (form) {
        form.addEventListener("submit", function () {
            const submitButton = form.querySelector('button[type="submit"]');

            if (submitButton && questionInput.value.trim()) {
                submitButton.disabled = true;
                submitButton.textContent = "…";
            }
        });
    }

    // Show the latest answer automatically after the server returns it.
    if (messages && messages.querySelectorAll(".buzzguide-message").length > 1) {
        setOpen(true);
        messages.scrollTop = messages.scrollHeight;
    }
});
