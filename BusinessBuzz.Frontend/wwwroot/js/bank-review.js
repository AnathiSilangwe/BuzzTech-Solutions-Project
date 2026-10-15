
document.addEventListener("DOMContentLoaded", function () {

    const storageKey = "businessBuzzBankEnquiries";
    const historyKey = "businessBuzzBankDecisionHistory";

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

    function readStorage(key, fallback) {
        try {
            const saved = localStorage.getItem(key);
            return saved ? JSON.parse(saved) : fallback;
        } catch (error) {
            console.error("Unable to read data:", error);
            return fallback;
        }
    }

    const enquiries = readStorage(storageKey, demoEnquiries);
    const history = readStorage(historyKey, []);

    const params = new URLSearchParams(window.location.search);
    const enquiryId = params.get("id");

    const enquiry = enquiries.find(
        item => item.id === enquiryId
    );

    const reviewContent = document.getElementById("reviewContent");
    const reviewError = document.getElementById("reviewError");

    if (!enquiry) {
        reviewContent.hidden = true;
        reviewError.hidden = false;
        return;
    }

    const formatCurrency = amount =>
        new Intl.NumberFormat("en-ZA", {
            style: "currency",
            currency: "ZAR"
        }).format(amount);

    document.getElementById("reviewReference").textContent =
        enquiry.id;

    document.getElementById("reviewBusiness").textContent =
        enquiry.business;

    document.getElementById("reviewAmount").textContent =
        formatCurrency(enquiry.amount);

    document.getElementById("reviewPurpose").textContent =
        enquiry.purpose;

    const statusElement = document.getElementById("reviewStatus");
    statusElement.textContent = enquiry.status;


    // Display additional funding information.
    // Older demonstration enquiries may not have these fields.

    const monthlyRevenueElement = document.getElementById(
        "reviewMonthlyRevenue"
    );

    monthlyRevenueElement.textContent =
        enquiry.monthlyRevenue !== undefined &&
            enquiry.monthlyRevenue !== null &&
            Number.isFinite(Number(enquiry.monthlyRevenue))
            ? formatCurrency(Number(enquiry.monthlyRevenue))
            : "Not provided";

    const tradingElement = document.getElementById("reviewTrading");

    tradingElement.textContent =
        enquiry.trading !== undefined &&
            enquiry.trading !== null &&
            String(enquiry.trading).trim() !== ""
            ? String(enquiry.trading)
            : "Not provided";

    const descriptionElement = document.getElementById(
        "reviewDescription"
    );

    descriptionElement.textContent =
        enquiry.description &&
            String(enquiry.description).trim()
            ? String(enquiry.description)
            : "No additional description provided.";

    const submissionDateElement = document.getElementById(
        "reviewSubmissionDate"
    );

    const submissionDate = enquiry.submittedAt
        ? new Date(enquiry.submittedAt)
        : null;

    submissionDateElement.textContent =
        submissionDate && !Number.isNaN(submissionDate.getTime())
            ? submissionDate.toLocaleString("en-ZA")
            : "Not available";


    const form = document.getElementById("fundingDecisionForm");
    const decisionInput = document.getElementById("fundingDecision");
    const reasonInput = document.getElementById("fundingDecisionReason");

    const errorMessage = document.getElementById(
        "fundingDecisionError"
    );

    const successMessage = document.getElementById(
        "fundingDecisionSuccess"
    );

    function renderHistory() {
        const container = document.getElementById(
            "fundingDecisionHistory"
        );

        container.replaceChildren();

        const enquiryHistory = history.filter(
            item => item.enquiryId === enquiryId
        );

        if (enquiryHistory.length === 0) {
            const message = document.createElement("p");
            message.className = "empty-message";
            message.textContent =
                "No decisions recorded for this enquiry.";

            container.appendChild(message);
            return;
        }

        enquiryHistory.slice().reverse().forEach(item => {
            const entry = document.createElement("div");
            entry.className = "decision-history-item";

            const heading = document.createElement("strong");
            heading.textContent = item.status;

            const reason = document.createElement("p");
            reason.textContent = item.reason;

            const date = document.createElement("small");
            date.textContent =
                new Date(item.date).toLocaleString("en-ZA");

            entry.append(heading, reason, date);
            container.appendChild(entry);
        });
    }

    form.addEventListener("submit", function (event) {
        event.preventDefault();

        const decision = decisionInput.value;
        const reason = reasonInput.value.trim();

        if (!decision || !reason) {
            errorMessage.hidden = false;
            return;
        }

        errorMessage.hidden = true;

        enquiry.status = decision;

        localStorage.setItem(
            storageKey,
            JSON.stringify(enquiries)
        );

        history.push({
            enquiryId: enquiry.id,
            status: decision,
            reason: reason,
            date: new Date().toISOString()
        });

        localStorage.setItem(
            historyKey,
            JSON.stringify(history)
        );

        statusElement.textContent = decision;

        successMessage.textContent =
            "Simulated funding decision saved successfully.";

        successMessage.hidden = false;

        renderHistory();
        form.reset();
    });

    document.getElementById("bankLogout")
        .addEventListener("click", function () {
            if (window.confirm("Log out of the Bank Portal?")) {
                window.location.replace(
                    "/pages/login.html?portal=bank"
                );
            }
        });

    renderHistory();
});
