
document.addEventListener("DOMContentLoaded", function () {

    const storageKey = "businessBuzzBankEnquiries";

    const demoEnquiries = [
        {
            id: "BB-FUND-001",
            business: "Noni's Student Catering",
            amount: 50000,
            purpose: "Business Expansion",
            status: "Pending Review"
        },
        {
            id: "BB-FUND-002",
            business: "Lerato Designs",
            amount: 80000,
            purpose: "Equipment Purchase",
            status: "Under Review"
        },
        {
            id: "BB-FUND-003",
            business: "Thando Tech Services",
            amount: 120000,
            purpose: "Working Capital",
            status: "Pending Review"
        },
        {
            id: "BB-FUND-004",
            business: "Mvelo Fresh Produce",
            amount: 65000,
            purpose: "Business Expansion",
            status: "Approved in Simulation"
        },
        {
            id: "BB-FUND-005",
            business: "Khanyisa Cleaning",
            amount: 30000,
            purpose: "Equipment Purchase",
            status: "Declined in Simulation"
        }
    ];

    function readEnquiries() {
        try {
            const saved = localStorage.getItem(storageKey);

            if (saved) {
                const parsed = JSON.parse(saved);
                if (Array.isArray(parsed)) return parsed;
            }
        } catch (error) {
            console.error("Unable to load bank enquiries:", error);
        }

        // Copy sample data so decisions can be stored later.
        return demoEnquiries.map(item => ({ ...item }));
    }

    const enquiries = readEnquiries();

    const total = enquiries.length;

    const pending = enquiries.filter(
        item => item.status === "Pending Review"
    ).length;

    const underReview = enquiries.filter(
        item => item.status === "Under Review"
    ).length;

    const decided = enquiries.filter(
        item =>
            item.status === "Approved in Simulation" ||
            item.status === "Declined in Simulation"
    ).length;

    document.getElementById("totalEnquiries").textContent = total;

    document.getElementById("pendingEnquiries").textContent = pending;

    document.getElementById("underReviewEnquiries").textContent =
        underReview;

    document.getElementById("decidedEnquiries").textContent = decided;

    const tableBody = document.getElementById(
        "fundingEnquiriesTable"
    );

    const emptyMessage = document.getElementById(
        "bankEmptyMessage"
    );

    const formatCurrency = amount =>
        new Intl.NumberFormat("en-ZA", {
            style: "currency",
            currency: "ZAR",
            maximumFractionDigits: 0
        }).format(amount);

    function getStatusClass(status) {
        if (status === "Under Review") {
            return "status-under-review";
        }

        if (status === "Approved in Simulation") {
            return "status-approved";
        }

        if (status === "Declined in Simulation") {
            return "status-declined";
        }

        return "";
    }

    if (enquiries.length === 0) {
        emptyMessage.hidden = false;
    }

    enquiries.forEach(function (enquiry) {

        const row = document.createElement("tr");

        const values = [
            enquiry.id,
            enquiry.business,
            formatCurrency(enquiry.amount),
            enquiry.purpose
        ];

        values.forEach(value => {
            const cell = document.createElement("td");
            cell.textContent = value;
            row.appendChild(cell);
        });

        const statusCell = document.createElement("td");
        const statusBadge = document.createElement("span");

        statusBadge.className =
            "status-pill " + getStatusClass(enquiry.status);

        statusBadge.textContent = enquiry.status;
        statusCell.appendChild(statusBadge);
        row.appendChild(statusCell);

        const actionCell = document.createElement("td");
        const reviewLink = document.createElement("a");

        reviewLink.href =
            "/pages/bank/review.html?id=" +
            encodeURIComponent(enquiry.id);

        reviewLink.className = "review-button";
        reviewLink.textContent = "Review";

        actionCell.appendChild(reviewLink);
        row.appendChild(actionCell);

        tableBody.appendChild(row);
    });

    document.getElementById("bankLogout")
        .addEventListener("click", function () {

            if (window.confirm("Log out of the Bank Portal?")) {
                window.location.replace("/pages/login.html?portal=bank");
            }
        });
});
