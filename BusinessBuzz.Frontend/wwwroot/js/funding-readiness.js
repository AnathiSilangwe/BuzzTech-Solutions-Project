
document.addEventListener("DOMContentLoaded", function () {

    // Frontend demonstration data only.
    // These values do not come from the database.
    const requirements = [
        {
            name: "Business Profile",
            description: "Business details have been captured.",
            complete: true
        },
        {
            name: "Registration Preparation",
            description: "Business registration preparation documents are available.",
            complete: true
        },
        {
            name: "Tax Compliance Preparation",
            description: "SARS preparation requirements are still in progress.",
            complete: false
        },
        {
            name: "Financial Information",
            description: "Basic revenue and funding information can be supplied in the enquiry.",
            complete: false
        },
        {
            name: "Business Readiness Review",
            description: "The overall business readiness review is still pending.",
            complete: false
        }
    ];

    const checklist = document.getElementById("fundingChecklist");
    const completed = requirements.filter(item => item.complete).length;
    const percentage = Math.round(completed / requirements.length * 100);

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
        status.textContent = item.complete ? "Complete" : "Pending";

        details.append(heading, description);
        row.append(details, status);
        checklist.appendChild(row);
    });

    document.getElementById("fundingPercentage").textContent =
        percentage + "%";

    const progressBar = document.getElementById("fundingProgressBar");
    progressBar.style.width = percentage + "%";
    progressBar.setAttribute("aria-valuenow", percentage);

    document.getElementById("fundingProgressText").textContent =
        completed + " of " + requirements.length +
        " preparation requirements completed.";

    const form = document.getElementById("fundingEnquiryForm");
    const message = document.getElementById("fundingMessage");
    const result = document.getElementById("fundingResult");

    form.addEventListener("submit", function (event) {
        event.preventDefault();

        if (!form.reportValidity()) return;

        const enquiry = {
            amount: Number(document.getElementById("fundingAmount").value),
            purpose: document.getElementById("fundingPurpose").value,
            monthlyRevenue: Number(document.getElementById("monthlyRevenue").value),
            trading: document.getElementById("businessTrading").value,
            description: document.getElementById("fundingDescription").value.trim()
        };

        if (enquiry.amount <= 0 || !enquiry.description) {
            message.className = "tax-form-message error";
            message.textContent = "Please provide a valid amount and description.";
            return;
        }

        // Temporary session-only prototype state.
        // Do not treat this as a backend or bank submission.
        sessionStorage.setItem(
            "businessBuzzFundingEnquiry",
            JSON.stringify(enquiry)
        );


        // Save the enquiry to the shared frontend demo repository.
        // This is not a real submission to a bank.

        const bankStorageKey = "businessBuzzBankEnquiries";

        let bankEnquiries = [];

        try {
            const saved = localStorage.getItem(bankStorageKey);

            if (saved) {
                const parsed = JSON.parse(saved);

                if (Array.isArray(parsed)) {
                    bankEnquiries = parsed;
                }
            }
        } catch (error) {
            console.error("Unable to read bank enquiries:", error);
            message.className = "tax-form-message error";
            message.textContent =
                "Could not read the demo enquiry repository.";
            return;
        }

        // Use a UUID so different submissions have unique IDs.
        const reference = "BB-FUND-" + crypto.randomUUID();

        // Use the entrepreneur's saved Business Profile.
        let businessName = "Business name not provided";

        try {
            const profile = JSON.parse(
                localStorage.getItem("businessBuzzBusinessProfile")
            );

            if (profile && typeof profile.businessName === "string"
                && profile.businessName.trim()) {
                businessName = profile.businessName.trim();
            }
        } catch (error) {
            console.error("Unable to read business profile:", error);
        }

        const bankRecord = {
            id: reference,
            business: businessName,
            amount: enquiry.amount,
            purpose: enquiry.purpose,
            monthlyRevenue: enquiry.monthlyRevenue,
            trading: enquiry.trading,
            description: enquiry.description,
            status: "Pending Review",
            submittedAt: new Date().toISOString(),
            source: "Entrepreneur Portal Demo"
        };

        bankEnquiries.push(bankRecord);

        try {
            localStorage.setItem(
                bankStorageKey,
                JSON.stringify(bankEnquiries)
            );
        } catch (error) {
            console.error("Unable to save funding enquiry:", error);
            message.className = "tax-form-message error";
            message.textContent =
                "The enquiry could not be saved. Please try again.";
            return;
        }


        message.className = "tax-form-message success";
        message.textContent =
            "Funding enquiry prepared successfully for this frontend demonstration.";


    

        const formattedAmount = new Intl.NumberFormat("en-ZA", {
            style: "currency",
            currency: "ZAR"
        }).format(enquiry.amount);

        document.getElementById("fundingReference").textContent =
            reference;

        document.getElementById("fundingResultAmount").textContent =
            formattedAmount;

        document.getElementById("fundingResultPurpose").textContent =
            enquiry.purpose;

        result.hidden = false;
        renderMyFundingEnquiries();

        document.getElementById("fundingStatus").textContent =
            "Enquiry Prepared";

        document.getElementById("fundingResult").scrollIntoView({
            behavior: "smooth",
            block: "start"
        

        });
    });

    function renderMyFundingEnquiries() {
        const container = document.getElementById(
            "myFundingEnquiries"
        );

        if (!container) return;

        container.replaceChildren();

        let enquiries = [];

        try {
            enquiries = JSON.parse(
                localStorage.getItem("businessBuzzBankEnquiries")
            ) || [];
        } catch (error) {
            console.error("Unable to load funding enquiries:", error);
        }

        // Only show enquiries created through the entrepreneur
        // funding form, not the five bank demonstration records.
        const myEnquiries = Array.isArray(enquiries)
            ? enquiries.filter(
                item => item.source === "Entrepreneur Portal Demo"
            )
            : [];

        if (myEnquiries.length === 0) {
            const empty = document.createElement("p");
            empty.textContent =
                "No funding enquiries have been prepared yet.";

            container.appendChild(empty);
            return;
        }

        myEnquiries.slice().reverse().forEach(enquiry => {
            const card = document.createElement("article");
            card.className = "readiness-requirement";

            const details = document.createElement("div");

            const heading = document.createElement("h3");
            heading.textContent = enquiry.id;

            const amount = document.createElement("p");
            amount.textContent =
                "Amount: " +
                new Intl.NumberFormat("en-ZA", {
                    style: "currency",
                    currency: "ZAR"
                }).format(enquiry.amount);

            const purpose = document.createElement("p");
            purpose.textContent =
                "Purpose: " + enquiry.purpose;

            const status = document.createElement("span");
            status.className = "status-pill";
            status.textContent = enquiry.status;

            details.append(heading, amount, purpose);
            card.append(details, status);

            container.appendChild(card);
        });
    }

    renderMyFundingEnquiries();

});
