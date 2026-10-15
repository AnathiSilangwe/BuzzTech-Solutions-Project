
document.addEventListener("DOMContentLoaded", function () {


    const MAX_FILE_SIZE = 5 * 1024 * 1024;

    const documentInputs = [
        ["identityDocument", "identityStatus"],
        ["addressDocument", "addressStatus"],
        ["cipcDocument", "cipcStatus"]
    ];


    const DB_NAME = "BusinessBuzzDocuments";
    const STORE_NAME = "applicationDocuments";
    const DB_VERSION = 2;

    // Temporary application reference for this frontend demo.
    // Later, the backend will provide the authenticated
    // entrepreneur's actual application reference.
    const APPLICATION_ID = "BB-2026-001";

    function documentKey(applicationId, documentType) {
        return applicationId + "_" + documentType;
    }

    function openDocumentDB() {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(DB_NAME, DB_VERSION);

            request.onupgradeneeded = function (event) {
                const db = request.result;
                const transaction = event.target.transaction;

                if (!db.objectStoreNames.contains(STORE_NAME)) {
                    db.createObjectStore(STORE_NAME, {
                        keyPath: "documentId"
                    });
                }

                // One-time migration of existing demo files.
                // The original store remains intact.
                if (db.objectStoreNames.contains("documents")) {
                    const oldStore =
                        transaction.objectStore("documents");
                    const newStore =
                        transaction.objectStore(STORE_NAME);

                    oldStore.openCursor().onsuccess = function (e) {
                        const cursor = e.target.result;

                        if (!cursor) return;

                        const oldDocument = cursor.value;

                        newStore.put({
                            ...oldDocument,
                            documentId: documentKey(
                                APPLICATION_ID,
                                oldDocument.documentType
                            ),
                            applicationId: APPLICATION_ID
                        });

                        cursor.continue();
                    };
                }
            };

            request.onsuccess = function () {
                resolve(request.result);
            };

            request.onerror = function () {
                reject(request.error);
            };

            request.onblocked = function () {
                console.warn(
                    "Close other BusinessBuzz tabs and reload."
                );
            };
        });
    }

    async function saveDocument(documentType, file) {
        const db = await openDocumentDB();

        try {
            await new Promise((resolve, reject) => {
                const transaction = db.transaction(
                    STORE_NAME,
                    "readwrite"
                );

                transaction.objectStore(STORE_NAME).put({
                    documentId: documentKey(
                        APPLICATION_ID,
                        documentType
                    ),
                    applicationId: APPLICATION_ID,
                    documentType: documentType,
                    fileName: file.name,
                    file: file,
                    uploadedAt: new Date().toISOString()
                });

                transaction.oncomplete = resolve;
                transaction.onerror = () =>
                    reject(transaction.error);
                transaction.onabort = () =>
                    reject(transaction.error);
            });
        } finally {
            db.close();
        }
    }

    function resetDocumentVerification(documentType) {
        const verificationKey =
            "businessBuzzDocumentVerification_" +
            APPLICATION_ID + "_" + documentType;

        localStorage.removeItem(verificationKey);
    }

    async function getDocument(documentType) {
        const db = await openDocumentDB();

        try {
            return await new Promise((resolve, reject) => {
                const transaction = db.transaction(
                    STORE_NAME,
                    "readonly"
                );

                const request = transaction
                    .objectStore(STORE_NAME)
                    .get(documentKey(APPLICATION_ID, documentType));

                request.onsuccess = () => resolve(request.result);
                request.onerror = () => reject(request.error);
            });
        } finally {
            db.close();
        }
    }

    async function deleteDocument(documentType) {
        const db = await openDocumentDB();

        try {
            await new Promise((resolve, reject) => {
                const transaction = db.transaction(
                    STORE_NAME,
                    "readwrite"
                );

                transaction.objectStore(STORE_NAME)
                    .delete(documentKey(
                        APPLICATION_ID,
                        documentType
                    ));

                transaction.oncomplete = resolve;
                transaction.onerror = () =>
                    reject(transaction.error);
                transaction.onabort = () =>
                    reject(transaction.error);
            });
        } finally {
            db.close();
        }
    }


    async function refreshDocumentStatus(documentType, statusId) {
        const status = document.getElementById(statusId);
        const saved = await getDocument(documentType);

        status.replaceChildren();

        if (!saved) {
            status.textContent = "No document uploaded";
            return;
        }

        const filename = document.createElement("span");
        filename.textContent = "Saved: " + saved.fileName + " ";

        const viewButton = document.createElement("button");
        viewButton.type = "button";
        viewButton.textContent = "View";

        viewButton.addEventListener("click", function () {
            const url = URL.createObjectURL(saved.file);
            window.open(url, "_blank", "noopener,noreferrer");

            // Release the temporary object URL later.
            setTimeout(() => URL.revokeObjectURL(url), 60000);
        });

        const downloadButton = document.createElement("button");
        downloadButton.type = "button";
        downloadButton.textContent = "Download";

        downloadButton.addEventListener("click", function () {
            const url = URL.createObjectURL(saved.file);
            const link = document.createElement("a");

            link.href = url;
            link.download = saved.fileName;
            link.click();

            setTimeout(() => URL.revokeObjectURL(url), 60000);
        });

        const deleteButton = document.createElement("button");
        deleteButton.type = "button";
        deleteButton.textContent = "Delete";

        deleteButton.addEventListener("click", async function () {
            const confirmed = confirm(
                "Delete " + saved.fileName + "?"
            );

            if (!confirmed) return;

            try {

                await deleteDocument(documentType);

                // A deleted document must not remain verified.
                resetDocumentVerification(documentType);

                await refreshDocumentStatus(documentType, statusId);

            } catch (error) {
                console.error("Delete failed:", error);
                alert("Could not delete the document.");
            }
        });

        status.append(
            filename,
            viewButton,
            downloadButton,
            deleteButton
        );
    }

    documentInputs.forEach(([inputId, statusId]) => {
        const input = document.getElementById(inputId);

        input.addEventListener("change", async function () {
            const file = input.files[0];

            if (!file) return;

            const allowedTypes = [
                "application/pdf",
                "image/jpeg",
                "image/png"
            ];

            if (!allowedTypes.includes(file.type)) {
                alert("Please select a PDF, JPG or PNG file.");
                input.value = "";
                return;
            }

            if (file.size > MAX_FILE_SIZE) {
                alert("Maximum file size is 5 MB.");
                input.value = "";
                return;
            }

            try {

                await saveDocument(inputId, file);

                // The uploaded file has changed, so any previous
                // verification is no longer valid.
                resetDocumentVerification(inputId);

                await refreshDocumentStatus(inputId, statusId);


                document.getElementById("uploadMessage")
                    .textContent =
                    "Document saved successfully in this browser.";
            } catch (error) {
                console.error("Upload failed:", error);
                alert("Unable to save the document.");
            }

            input.value = "";
        });

        refreshDocumentStatus(inputId, statusId)
            .catch(error => console.error(error));
    });


    function createPDF(title, sections, filename) {
        if (!window.jspdf) {
            alert("PDF library could not be loaded.");
            return;
        }

        const { jsPDF } = window.jspdf;
        const pdf = new jsPDF();

        pdf.setFontSize(20);
        pdf.setTextColor(7, 17, 47);
        pdf.text("BusinessBuzz", 20, 22);

        pdf.setFontSize(15);
        pdf.text(title, 20, 36);

        pdf.setFontSize(10);
        pdf.setTextColor(100, 100, 100);
        pdf.text(
            "Generated: " + new Date().toLocaleDateString("en-ZA"),
            20,
            45
        );

        let y = 60;

        sections.forEach(section => {
            pdf.setFontSize(12);
            pdf.setTextColor(7, 17, 47);

            if (y > 255) {
                pdf.addPage();
                y = 20;
            }

            pdf.text(section.heading, 20, y);
            y += 9;

            pdf.setFontSize(10);
            pdf.setTextColor(50, 50, 50);


            const lines = String(section.content)
                .split("\n")
                .flatMap(paragraph =>
                    paragraph.trim() === ""
                        ? [""]
                        : pdf.splitTextToSize(paragraph, 165)
                );


            lines.forEach(line => {
                if (y > 275) {
                    pdf.addPage();
                    y = 20;
                }

                pdf.text(line, 20, y);
                y += 6;
            });

            y += 10;
        });

        pdf.setFontSize(9);
        pdf.setTextColor(120, 120, 120);

        if (y > 275) {
            pdf.addPage();
            y = 20;
        }

        pdf.text(
            "BusinessBuzz prototype report - Not an official certificate.",
            20,
            y
        );

        pdf.save(filename);
    }


    document.getElementById("generateProfile")
        .addEventListener("click", function () {

            let profile;

            try {
                const saved = localStorage.getItem(
                    "businessBuzzBusinessProfile"
                );

                if (!saved) {
                    alert(
                        "Please open your Business Profile, " +
                        "review your details and click Save Changes " +
                        "before generating this document."
                    );
                    return;
                }

                profile = JSON.parse(saved);

                if (!profile || typeof profile !== "object" ||
                    Array.isArray(profile)) {
                    throw new Error("Invalid profile data");
                }

            } catch (error) {
                console.error("Unable to read business profile:", error);

                alert(
                    "Your saved business profile could not be loaded. " +
                    "Please save it again."
                );
                return;
            }

            const display = value =>
                typeof value === "string" && value.trim()
                    ? value
                    : "Not provided";

            createPDF(
                "Business Profile",
                [
                    {
                        heading: "Business Name",
                        content: display(profile.businessName)
                    },
                    {
                        heading: "Business Structure",
                        content: display(profile.businessType)
                    },
                    {
                        heading: "Industry",
                        content: display(profile.industry)
                    },
                    {
                        heading: "Province",
                        content: display(profile.province)
                    },
                    {
                        heading: "Employment Status",
                        content:
                            profile.employsPeople === "Yes"
                                ? "Employs people"
                                : profile.employsPeople === "No"
                                    ? "No employees"
                                    : "Not provided"
                    },
                    {
                        heading: "Interested in Funding",
                        content: display(profile.interestedInFunding)
                    }
                ],
                "BusinessBuzz-Business-Profile.pdf"
            );
        });


    document.getElementById("generateReadiness")
        .addEventListener("click", function () {

            // Read the entrepreneur's saved business profile.
            let profile = {};
            let fundingEnquiry = null;

            try {
                profile = JSON.parse(
                    localStorage.getItem("businessBuzzBusinessProfile")
                ) || {};
            } catch (error) {
                console.error("Unable to read business profile:", error);
            }

            // Funding enquiry is saved in sessionStorage
            // by your existing funding-readiness.js.
            try {
                fundingEnquiry = JSON.parse(
                    sessionStorage.getItem("businessBuzzFundingEnquiry")
                );
            } catch (error) {
                console.error("Unable to read funding enquiry:", error);
            }

            const display = value =>
                value === undefined ||
                    value === null ||
                    String(value).trim() === ""
                    ? "Not provided"
                    : String(value);

            // These requirements match the existing
            // business-readiness.js demonstration checklist.
            const businessRequirements = [
                { name: "Business Profile", complete: true },
                { name: "CIPC Registration Preparation", complete: true },
                { name: "Business Documentation", complete: true },
                { name: "SARS Compliance Preparation", complete: false },
                { name: "Compliance Review", complete: false }
            ];

            // These requirements match the existing
            // funding-readiness.js demonstration checklist.
            const fundingRequirements = [
                { name: "Business Profile", complete: true },
                { name: "Registration Preparation", complete: true },
                { name: "Tax Compliance Preparation", complete: false },
                { name: "Financial Information", complete: false },
                { name: "Business Readiness Review", complete: false }
            ];

            function getProgress(requirements) {
                const completed = requirements.filter(
                    item => item.complete
                ).length;

                return {
                    completed: completed,
                    total: requirements.length,
                    percentage: Math.round(
                        completed / requirements.length * 100
                    )
                };
            }

            function formatRequirements(requirements) {
                return requirements.map(item =>
                    (item.complete ? "[Complete] " : "[Pending] ") +
                    item.name
                ).join("\n");
            }

            const businessProgress = getProgress(businessRequirements);
            const fundingProgress = getProgress(fundingRequirements);

            const currency = amount =>
                new Intl.NumberFormat("en-ZA", {
                    style: "currency",
                    currency: "ZAR"
                }).format(amount);

            const fundingDetails = fundingEnquiry
                ? [
                    "Amount Requested: " +
                    currency(fundingEnquiry.amount),
                    "Purpose: " +
                    display(fundingEnquiry.purpose),
                    "Monthly Revenue: " +
                    currency(fundingEnquiry.monthlyRevenue),
                    "Currently Trading: " +
                    display(fundingEnquiry.trading),
                    "Description: " +
                    display(fundingEnquiry.description)
                ].join("\n")
                : "No funding enquiry has been prepared in this browser session.";

            const pendingRequirements = [
                ...businessRequirements
                    .filter(item => !item.complete)
                    .map(item => item.name),
                ...fundingRequirements
                    .filter(item => !item.complete)
                    .map(item => item.name)
            ];

            createPDF(
                "Business Readiness Report",
                [
                    {
                        heading: "Business Information",
                        content:
                            "Business Name: " +
                            display(profile.businessName) + "\n" +
                            "Business Structure: " +
                            display(profile.businessType) + "\n" +
                            "Industry: " +
                            display(profile.industry) + "\n" +
                            "Province: " +
                            display(profile.province)
                    },
                    {
                        heading: "Business Readiness - Demo Assessment",
                        content:
                            businessProgress.completed + " of " +
                            businessProgress.total +
                            " requirements completed (" +
                            businessProgress.percentage + "%).\n\n" +
                            formatRequirements(businessRequirements)
                    },
                    {
                        heading: "Funding Readiness - Demo Assessment",
                        content:
                            fundingProgress.completed + " of " +
                            fundingProgress.total +
                            " requirements completed (" +
                            fundingProgress.percentage + "%).\n\n" +
                            formatRequirements(fundingRequirements)
                    },
                    {
                        heading: "Funding Enquiry Information",
                        content: fundingDetails
                    },
                    {
                        heading: "Compliance Preparation",
                        content:
                            "SARS compliance preparation and the " +
                            "overall compliance review are marked " +
                            "as pending in the demonstration checklist. " +
                            "No separate compliance assessment " +
                            "data has been verified."
                    },
                    {
                        heading: "Recommended Next Steps",
                        content:
                            pendingRequirements.map(
                                item => "- Complete " + item
                            ).join("\n")
                    },
                    {
                        heading: "Important Disclaimer",
                        content:
                            "This report uses demonstration readiness " +
                            "checklists and any business information " +
                            "saved in the current browser. It is not " +
                            "an official compliance assessment, " +
                            "CIPC certificate, SARS clearance or " +
                            "funding approval."
                    }
                ],
                "BusinessBuzz-Readiness-Report.pdf"
            );
        });

    
});
