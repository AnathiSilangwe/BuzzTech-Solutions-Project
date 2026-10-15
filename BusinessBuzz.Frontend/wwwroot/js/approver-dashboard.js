
document.addEventListener("DOMContentLoaded", function () {
    
const demoApplications = [
    {
        id: "BB-2026-001",
        business: "Noni's Student Catering",
        stage: "CIPC Registration",
        date: "2026-10-08",
        status: "Pending"
    },
    {
        id: "BB-2026-002",
        business: "Lerato Designs",
        stage: "SARS Preparation",
        date: "2026-10-07",
        status: "Pending"
    },
    {
        id: "BB-2026-003",
        business: "Thando Tech Services",
        stage: "Business Profile",
        date: "2026-10-05",
        status: "More Information"
    },
    {
        id: "BB-2026-004",
        business: "Mvelo Fresh Produce",
        stage: "CIPC Registration",
        date: "2026-10-03",
        status: "Approved"
    },
    {
        id: "BB-2026-005",
        business: "Khanyisa Cleaning",
        stage: "SARS Preparation",
        date: "2026-10-02",
        status: "Rejected"
    }
];

let applications;

try {
    const saved = localStorage.getItem(
        "businessBuzzApproverApplications"
    );

    applications = saved
        ? JSON.parse(saved)
        : demoApplications;

    if (!Array.isArray(applications)) {
        applications = demoApplications;
    }
} catch {
    applications = demoApplications;
}

    //const activity = [
    //    "Demo application BB-2026-001 is awaiting review.",
    //    "Demo application BB-2026-003 needs additional information.",
    //    "Demo application BB-2026-004 has a simulated approval."
    //];

    const rows = document.getElementById("applicationRows");
    const filter = document.getElementById("statusFilter");
    const emptyMessage = document.getElementById("emptyApplications");

    function getStatusClass(status) {
        if (status === "Approved") return "status-approved";
        if (status === "Rejected") return "status-rejected";
        if (status === "More Information") return "status-more-information";
        return "status-pending";
    }

    function renderCounts() {
        document.getElementById("pendingCount").textContent =
            applications.filter(app => app.status === "Pending").length;

        document.getElementById("approvedCount").textContent =
            applications.filter(app => app.status === "Approved").length;

        document.getElementById("rejectedCount").textContent =
            applications.filter(app => app.status === "Rejected").length;

        document.getElementById("informationCount").textContent =
            applications.filter(app => app.status === "More Information").length;
    }

    function renderApplications() {
        const selectedStatus = filter.value;

        const filtered = applications.filter(app =>
            selectedStatus === "All" || app.status === selectedStatus
        );

        rows.replaceChildren();

        filtered.forEach(app => {
            const tr = document.createElement("tr");

            const values = [
                app.id,
                app.business,
                app.stage,
                app.date
            ];

            values.forEach(value => {
                const td = document.createElement("td");
                td.textContent = value;
                tr.appendChild(td);
            });

            const statusCell = document.createElement("td");
            const statusBadge = document.createElement("span");
            statusBadge.className =
                "approver-status " + getStatusClass(app.status);
            statusBadge.textContent = app.status;
            statusCell.appendChild(statusBadge);
            tr.appendChild(statusCell);

            const actionCell = document.createElement("td");
            const reviewButton = document.createElement("button");
            reviewButton.type = "button";
            reviewButton.className = "approver-review-btn";


            reviewButton.textContent = "Review";

            reviewButton.addEventListener("click", function () {
                window.location.href =
                    "/pages/approver/review.html?id=" +
                    encodeURIComponent(app.id);
            });

            actionCell.appendChild(reviewButton);
            tr.appendChild(actionCell);
            rows.appendChild(tr);

        });

        emptyMessage.hidden = filtered.length !== 0;
        renderCounts();
    }

    filter.addEventListener("change", renderApplications);

    document.querySelectorAll(".approver-nav-link").forEach(link => {
        link.addEventListener("click", function (event) {
            const href = link.getAttribute("href");

            if (href === "#pending") {
                event.preventDefault();
                filter.value = "Pending";
                renderApplications();
                document.getElementById("applications").scrollIntoView({
                    behavior: "smooth"
                });
            }

            if (href === "#applications") {
                event.preventDefault();
                filter.value = "All";
                renderApplications();
                document.getElementById("applications").scrollIntoView({
                    behavior: "smooth"
                });
            }

            if (href === "#activity") {
                event.preventDefault();
                document.getElementById("activity").scrollIntoView({
                    behavior: "smooth"
                });
            }
        });
    });


    document.getElementById("approverLogout")
        .addEventListener("click", function () {
            const confirmed = window.confirm("Log out of the Approver Portal?");

            if (confirmed) {
                window.location.replace("/pages/login.html");
            }
        });


    const activityList = document.getElementById("activityList");

    function renderActivityHistory() {
        activityList.replaceChildren();

        let history = [];

        try {
            const savedHistory = localStorage.getItem(
                "businessBuzzApproverDecisionHistory"
            );

            history = savedHistory ? JSON.parse(savedHistory) : [];

            if (!Array.isArray(history)) {
                history = [];
            }
        } catch (error) {
            console.error("Unable to load decision history:", error);
            history = [];
        }

        if (history.length === 0) {
            const emptyMessage = document.createElement("p");
            emptyMessage.textContent =
                "No approval decisions have been recorded yet.";

            emptyMessage.className = "review-muted";
            activityList.appendChild(emptyMessage);
            return;
        }

        // Display the most recent decisions first
        const recentHistory = [...history].reverse();

        recentHistory.forEach(record => {
            const item = document.createElement("div");
            item.className = "approver-activity-item";

            const application = applications.find(
                app => app.id === record.applicationId
            );

            const businessName = application
                ? application.business
                : "Unknown Business";

            const heading = document.createElement("p");
            heading.textContent =
                businessName + " (" + record.applicationId + ")";

            const decision = document.createElement("p");
            decision.textContent = "Decision: " + record.status;

            const reason = document.createElement("p");
            reason.textContent = "Reason: " + record.reason;

            const date = document.createElement("small");

            const parsedDate = new Date(record.date);

            date.textContent = Number.isNaN(parsedDate.getTime())
                ? "Date unavailable"
                : parsedDate.toLocaleString("en-ZA");

            item.append(heading, decision, reason, date);
            activityList.appendChild(item);
        });
    }


    renderApplications();
    renderActivityHistory();
});
