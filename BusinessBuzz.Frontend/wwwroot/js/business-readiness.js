
document.addEventListener("DOMContentLoaded", function () {

    // Sample readiness data for frontend demonstration only.
    // Replace with backend API data during integration.
    const requirements = [
        {
            name: "Business Profile",
            description: "Your business information has been captured.",
            complete: true
        },
        {
            name: "CIPC Registration Preparation",
            description: "Your registration preparation stage is complete.",
            complete: true
        },
        {
            name: "Business Documentation",
            description: "Your business preparation documents are available.",
            complete: true
        },
        {
            name: "SARS Compliance Preparation",
            description: "Complete the remaining tax preparation requirements.",
            complete: false
        },
        {
            name: "Compliance Review",
            description: "Review the outstanding compliance requirements.",
            complete: false
        }
    ];

    const checklist = document.getElementById("readinessChecklist");
    const completed = requirements.filter(item => item.complete).length;
    const percentage = Math.round(
        (completed / requirements.length) * 100
    );

    requirements.forEach(function (item) {
        const row = document.createElement("article");
        row.className = "readiness-requirement";

        const details = document.createElement("div");
        const heading = document.createElement("h3");
        const description = document.createElement("p");
        const status = document.createElement("span");

        heading.textContent = item.name;
        description.textContent = item.description;

        status.className = item.complete
            ? "status-pill complete"
            : "status-pill progress";

        status.textContent = item.complete
            ? "Complete"
            : "Pending";

        details.append(heading, description);
        row.append(details, status);
        checklist.appendChild(row);
    });

    document.getElementById("readinessPercentage").textContent =
        percentage + "%";

    const progressBar = document.getElementById("readinessProgressBar");
    progressBar.style.width = percentage + "%";
    progressBar.setAttribute("aria-valuenow", percentage);

    document.getElementById("readinessSummary").textContent =
        completed + " of " + requirements.length +
        " readiness requirements completed.";

    if (percentage === 100) {
        document.getElementById("readinessStatus").textContent =
            "Ready";

        document.getElementById("readinessNextStep").textContent =
            "Your simulated business readiness requirements are complete.";
    }
});
