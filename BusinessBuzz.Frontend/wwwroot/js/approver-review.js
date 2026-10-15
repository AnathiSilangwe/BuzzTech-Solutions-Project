
document.addEventListener("DOMContentLoaded", function () {
    const storageKey = "businessBuzzApproverApplications";
    const historyKey = "businessBuzzApproverDecisionHistory";

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

    function readStorage(key, fallback) {
        try {
            const value = localStorage.getItem(key);
            return value ? JSON.parse(value) : fallback;
        } catch (error) {
            console.error("Could not read demo data:", error);
            return fallback;
        }
    }

    function saveStorage(key, value) {
        localStorage.setItem(key, JSON.stringify(value));
    }

    const applications = readStorage(storageKey, demoApplications);
    const history = readStorage(historyKey, []);

    const params = new URLSearchParams(window.location.search);
    const applicationId = params.get("id");

    const application = applications.find(app => app.id === applicationId);

    const reviewContent = document.getElementById("reviewContent");
    const reviewError = document.getElementById("reviewError");

    function getStatusClass(status) {
        if (status === "Approved") return "status-approved";
        if (status === "Rejected") return "status-rejected";
        if (status === "More Information") {
            return "status-more-information";
        }
        return "status-pending";
    }

    function renderHistory() {
        const historyContainer = document.getElementById("decisionHistory");
        historyContainer.replaceChildren();

        const applicationHistory = history.filter(
            item => item.applicationId === applicationId
        );

        if (applicationHistory.length === 0) {
            const message = document.createElement("p");
            message.className = "review-muted";
            message.textContent =
                "No decisions recorded for this application.";
            historyContainer.appendChild(message);
            return;
        }

        applicationHistory.slice().reverse().forEach(item => {
            const entry = document.createElement("div");
            entry.className = "review-history-item";

            const heading = document.createElement("strong");
            heading.textContent = item.status;

            const reason = document.createElement("p");
            reason.textContent = item.reason;

            const timestamp = document.createElement("small");
            timestamp.textContent = new Date(item.date).toLocaleString();

            entry.append(heading, reason, timestamp);
            historyContainer.appendChild(entry);
        });
    }

    if (!application) {
        reviewContent.hidden = true;
        reviewError.hidden = false;
        return;
    }

    document.getElementById("reviewReference").textContent = application.id;
    document.getElementById("reviewBusiness").textContent = application.business;
    document.getElementById("detailBusiness").textContent = application.business;
    document.getElementById("detailStage").textContent = application.stage;
    document.getElementById("detailDate").textContent = application.date;
    document.getElementById("detailReference").textContent = application.id;

    const statusBadge = document.getElementById("reviewStatus");
    statusBadge.textContent = application.status;
    statusBadge.className =
        "approver-status " + getStatusClass(application.status);

    document.getElementById("detailStatus").textContent = application.status;

    const form = document.getElementById("decisionForm");
    const decisionSelect = document.getElementById("decision");
    const reasonInput = document.getElementById("decisionReason");
    const decisionError = document.getElementById("decisionError");
    const successMessage = document.getElementById("decisionSuccess");

    form.addEventListener("submit", async function (event) {
        event.preventDefault();

        const decision = decisionSelect.value;
        const reason = reasonInput.value.trim();

        if (!decision || !reason) {
            decisionError.hidden = false;
            return;
        }

        decisionError.hidden = true;

        // Require verified mandatory documents before approval.
        // This applies only to the linked demo application.
        if (decision === "Approved" &&
            application.id === "BB-2026-001") {

            const mandatoryDocuments = [
                "identityDocument",
                "addressDocument"
            ];

            const unverifiedDocuments = [];

            const missingDocuments = [];

            try {
                for (const documentType of mandatoryDocuments) {
                    const savedDocument = await getDocument(documentType);

                    if (!savedDocument) {
                        missingDocuments.push(
                            documentType === "identityDocument"
                                ? "ID / Passport"
                                : "Proof of Business Address"
                        );
                    }
                }
            } catch (error) {
                console.error("Unable to check uploaded documents:", error);

                decisionError.hidden = false;
                decisionError.textContent =
                    "Unable to verify the uploaded documents. " +
                    "Please try again.";

                successMessage.hidden = true;
                return;
            }

            if (missingDocuments.length > 0) {
                decisionError.hidden = false;
                decisionError.textContent =
                    "Approval cannot proceed. These mandatory " +
                    "documents are missing: " +
                    missingDocuments.join(", ") + ".";

                successMessage.hidden = true;
                return;
            }


            for (const documentType of mandatoryDocuments) {
                const verificationKey =
                    "businessBuzzDocumentVerification_" +
                    application.id + "_" + documentType;

                let verification = null;

                try {
                    verification = JSON.parse(
                        localStorage.getItem(verificationKey)
                    );
                } catch (error) {
                    console.error(
                        "Unable to read document verification:",
                        error
                    );
                }

                if (!verification ||
                    verification.status !== "Verified") {
                    unverifiedDocuments.push(
                        documentType === "identityDocument"
                            ? "ID / Passport"
                            : "Proof of Business Address"
                    );
                }
            }

            if (unverifiedDocuments.length > 0) {
                decisionError.hidden = false;
                decisionError.textContent =
                    "Approval cannot proceed. Verify these " +
                    "mandatory documents first: " +
                    unverifiedDocuments.join(", ") + ".";

                successMessage.hidden = true;
                return;
            }
        }

        // Continue with the existing decision workflow.
        application.status = decision;

        saveStorage(storageKey, applications);

        history.push({
            applicationId: application.id,
            status: decision,
            reason: reason,
            date: new Date().toISOString()
        });

        saveStorage(historyKey, history);

        statusBadge.textContent = decision;
        statusBadge.className =
            "approver-status " + getStatusClass(decision);

        document.getElementById("detailStatus").textContent = decision;

        successMessage.textContent =
            "Demo decision recorded: " + decision +
            ". The dashboard will reflect this simulated status.";

        successMessage.hidden = false;

        renderHistory();

        form.reset();
    });

    document.getElementById("approverLogout")
        .addEventListener("click", function () {
            if (window.confirm("Log out of the Approver Portal?")) {
                window.location.replace("/pages/login.html");
            }
        });


    // Document verification - frontend prototype only.
    // Documents are currently stored in browser IndexedDB.

    const documentTypes = [
        {
            id: "identityDocument",
            name: "ID / Passport"
        },
        {
            id: "addressDocument",
            name: "Proof of Business Address"
        },
        {
            id: "cipcDocument",
            name: "CIPC Registration Certificate"
        }
    ];

    const DOCUMENT_DB = "BusinessBuzzDocuments";
    const DOCUMENT_STORE = "applicationDocuments";
    const DOCUMENT_DB_VERSION = 2;

    // Current demo documents belong only to BB-2026-001.
    // Other applications must not display these files.
    const DEMO_DOCUMENT_APPLICATION = "BB-2026-001";


    function openDocumentDB() {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(
                DOCUMENT_DB,
                DOCUMENT_DB_VERSION
            );

            request.onsuccess = () => resolve(request.result);
            request.onerror = () => reject(request.error);
        });
    }


    async function getDocument(documentType) {
        const db = await openDocumentDB();

        try {
            return await new Promise((resolve, reject) => {
                const transaction = db.transaction(
                    DOCUMENT_STORE,
                    "readonly"
                );

                const request = transaction
                    .objectStore(DOCUMENT_STORE)
                    .get(applicationId + "_" + documentType);

                request.onsuccess = () => resolve(request.result);
                request.onerror = () => reject(request.error);
            });
        } finally {
            db.close();
        }
    }

    function getVerificationKey(documentType) {
        return (
            "businessBuzzDocumentVerification_" +
            applicationId + "_" + documentType
        );
    }

    function getVerification(documentType) {
        try {
            const saved = localStorage.getItem(
                getVerificationKey(documentType)
            );

            return saved ? JSON.parse(saved) : null;
        } catch {
            return null;
        }
    }

    function saveVerification(documentType, status, note) {
        const verification = {
            applicationId: applicationId,
            documentType: documentType,
            status: status,
            note: note,
            date: new Date().toISOString()
        };

        localStorage.setItem(
            getVerificationKey(documentType),
            JSON.stringify(verification)
        );
    }

    async function renderDocuments() {
        const container = document.getElementById(
            "reviewDocuments"
        );

        const message = document.getElementById(
            "documentReviewMessage"
        );

        if (!container || !message) return;

        container.replaceChildren();

        if (applicationId !== DEMO_DOCUMENT_APPLICATION) {
            message.textContent =
                "Document verification is currently available " +
                "only for the linked demonstration application " +
                DEMO_DOCUMENT_APPLICATION + ".";

            return;
        }

        message.textContent = "";

        for (const documentType of documentTypes) {
            const saved = await getDocument(documentType.id);

            const item = document.createElement("div");
            item.className = "approver-document-item";

            const heading = document.createElement("h3");
            heading.textContent = documentType.name;

            item.appendChild(heading);

            if (!saved) {
                const missing = document.createElement("p");
                missing.className = "review-muted";
                missing.textContent =
                    "Document not uploaded.";

                item.appendChild(missing);
                container.appendChild(item);
                continue;
            }

            const filename = document.createElement("p");
            filename.textContent = saved.fileName;

            const viewButton = document.createElement("button");
            viewButton.type = "button";
            viewButton.className = "approver-review-btn";
            viewButton.textContent = "View Document";

            viewButton.addEventListener("click", function () {
                const url = URL.createObjectURL(saved.file);

                window.open(url, "_blank", "noopener,noreferrer");

                setTimeout(() => {
                    URL.revokeObjectURL(url);
                }, 60000);
            });

            const select = document.createElement("select");

            [
                ["", "Select Verification"],
                ["Verified", "Verified"],
                ["Rejected", "Rejected"],
                ["More Information", "More Information Required"]
            ].forEach(([value, label]) => {
                const option = document.createElement("option");
                option.value = value;
                option.textContent = label;
                select.appendChild(option);
            });

            const noteInput = document.createElement("textarea");
            noteInput.placeholder = "Verification notes";
            noteInput.rows = 3;

            const previous = getVerification(documentType.id);

            if (previous) {
                select.value = previous.status;
                noteInput.value = previous.note;
            }

            const saveButton = document.createElement("button");
            saveButton.type = "button";
            saveButton.className = "approver-review-btn";
            saveButton.textContent = "Save Verification";

            const status = document.createElement("p");
            status.className = "review-muted";

            if (previous) {
                status.textContent =
                    "Current verification: " + previous.status;
            }

            saveButton.addEventListener("click", function () {
                if (!select.value || !noteInput.value.trim()) {
                    alert(
                        "Please select a verification status " +
                        "and provide a note."
                    );
                    return;
                }

                saveVerification(
                    documentType.id,
                    select.value,
                    noteInput.value.trim()
                );

                status.textContent =
                    "Current verification: " + select.value;
            });

            item.append(
                filename,
                viewButton,
                select,
                noteInput,
                saveButton,
                status
            );

            container.appendChild(item);
        }
    }

    renderDocuments().catch(error => {
        console.error("Unable to load documents:", error);

        const message = document.getElementById(
            "documentReviewMessage"
        );

        if (message) {
            message.textContent =
                "Documents could not be loaded.";
        }
    });


    renderHistory();
});
