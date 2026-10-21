/* ========================================
   BUSINESS BUZZ - APPROVER PORTAL
   ======================================== */

document.addEventListener("DOMContentLoaded", () => {
    const decisionForms = document.querySelectorAll(
        ".approver-decision-form"
    );

    decisionForms.forEach((form) => {
        const decisionSelect = form.querySelector(
            'select[name="Decision"]'
        );

        const reasonField = form.querySelector(
            'textarea[name="ReasonText"]'
        );

        const submitButton = form.querySelector(
            'button[type="submit"]'
        );

        if (!decisionSelect || !reasonField || !submitButton) {
            return;
        }

        const originalButtonText = submitButton.textContent.trim();

        // Show the reason field only when appropriate.
        const updateReasonField = () => {
            const decision = decisionSelect.value;
            const requiresReason =
                decision === "Rejected" ||
                decision === "Action required";

            reasonField.required = requiresReason;

            reasonField.placeholder = requiresReason
                ? "Please explain the reason for this decision."
                : "Optional notes";

            reasonField.setAttribute(
                "aria-required",
                String(requiresReason)
            );

            reasonField.closest(".approver-decision-form")
                ?.classList.toggle(
                    "requires-reason",
                    requiresReason
                );
        };

        decisionSelect.addEventListener(
            "change",
            updateReasonField
        );

        updateReasonField();

        // Confirm the decision before submitting.
        form.addEventListener("submit", (event) => {
            if (!form.checkValidity()) {
                return;
            }

            const decision = decisionSelect.value;

            const confirmed = window.confirm(
                `Are you sure you want to record "${decision}"?`
            );

            if (!confirmed) {
                event.preventDefault();
                return;
            }

            // Prevent accidental double submissions.
            submitButton.disabled = true;
            submitButton.textContent = "Saving decision...";
            submitButton.setAttribute("aria-busy", "true");
        });
    });
});
