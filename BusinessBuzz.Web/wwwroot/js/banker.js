/* ==========================================
   BUSINESS BUZZ - BANKER PORTAL
   ========================================== */

document.addEventListener("DOMContentLoaded", () => {
    // Confirm logout before submitting the existing logout form.
    const logoutForm = document.querySelector(".bank-sidebar form");

    if (logoutForm) {
        logoutForm.addEventListener("submit", (event) => {
            const confirmed = window.confirm(
                "Are you sure you want to log out?"
            );

            if (!confirmed) {
                event.preventDefault();
            }
        });
    }

    // Prevent double submissions of the funding assessment form.
    const assessmentForm = document.querySelector(
        "#fundingDecisionForm, form[asp-page-handler='Assess']"
    );

    if (assessmentForm) {
        assessmentForm.addEventListener("submit", () => {
            const submitButton = assessmentForm.querySelector(
                "button[type='submit'], input[type='submit']"
            );

            if (submitButton && assessmentForm.checkValidity()) {
                submitButton.disabled = true;
                submitButton.textContent = "Submitting...";
            }
        });
    }

    // Optional table filtering.
    // Add an input with data-table-filter="pending-table"
    // and give the table the matching id to enable it.
    const filterInputs = document.querySelectorAll("[data-table-filter]");

    filterInputs.forEach((input) => {
        const tableId = input.dataset.tableFilter;
        const table = document.getElementById(tableId);

        if (!table) {
            return;
        }

        input.addEventListener("input", () => {
            const searchTerm = input.value.trim().toLowerCase();
            const rows = table.querySelectorAll("tbody tr");

            rows.forEach((row) => {
                const rowText = row.textContent.toLowerCase();
                row.hidden = !rowText.includes(searchTerm);
            });
        });
    });

    // Smoothly navigate to sections linked with a URL fragment.
    if (window.location.hash) {
        const target = document.getElementById(
            decodeURIComponent(window.location.hash.substring(1))
        );

        if (target) {
            window.requestAnimationFrame(() => {
                target.scrollIntoView({
                    behavior: "smooth",
                    block: "start"
                });
            });
        }
    }
});