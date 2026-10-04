const openTaxFormBtn =
    document.getElementById("openTaxFormBtn");

const closeTaxFormBtn =
    document.getElementById("closeTaxFormBtn");

const cancelTaxFormBtn =
    document.getElementById("cancelTaxFormBtn");

const taxInformationSection =
    document.getElementById("taxInformationSection");

const taxInformationForm =
    document.getElementById("taxInformationForm");

const taxFormMessage =
    document.getElementById("taxFormMessage");


function openTaxForm() {

    taxInformationSection.classList.add("show-tax-form");

    taxInformationSection.scrollIntoView({
        behavior: "smooth",
        block: "start"
    });

}


function closeTaxForm() {

    taxInformationSection.classList.remove("show-tax-form");

    taxFormMessage.className = "tax-form-message";
    taxFormMessage.textContent = "";

}


openTaxFormBtn.addEventListener(
    "click",
    openTaxForm
);


closeTaxFormBtn.addEventListener(
    "click",
    closeTaxForm
);


cancelTaxFormBtn.addEventListener(
    "click",
    closeTaxForm
);


taxInformationForm.addEventListener("submit", function (event) {

    event.preventDefault();

    const estimatedIncome =
        document.getElementById("estimatedIncome").value;

    const tradingStatus =
        document.getElementById("tradingStatus").value;

    const financialRecords =
        document.getElementById("financialRecords").value;

    const confirmation =
        document.getElementById("taxConfirmation").checked;


    // Validate required information
    if (
        estimatedIncome === "" ||
        tradingStatus === "" ||
        financialRecords === "" ||
        !confirmation
    ) {

        taxFormMessage.className =
            "tax-form-message error";

        taxFormMessage.textContent =
            "Please complete all required information before saving.";

        return;
    }


    // Success message
    taxFormMessage.className =
        "tax-form-message success";

    taxFormMessage.textContent =
        "Tax information completed successfully for this frontend prototype.";


    // Tax Information task
    const taxInformationTask =
        document.getElementById("taxInformationTask");

    const taxInformationStatus =
        document.getElementById("taxInformationStatus");

    if (taxInformationTask) {

        taxInformationTask.classList.remove("in-progress");
        taxInformationTask.classList.add("completed");

    }

    if (taxInformationStatus) {

        taxInformationStatus.classList.remove("progress");
        taxInformationStatus.classList.add("complete");

        taxInformationStatus.textContent = "Complete";

    }


    // Progress counters
    const sarsTaskCount =
        document.getElementById("sarsTaskCount");

    const sarsChecklistProgress =
        document.getElementById("sarsChecklistProgress");

    if (sarsTaskCount) {
        sarsTaskCount.textContent = "3 of 4";
    }

    if (sarsChecklistProgress) {
        sarsChecklistProgress.textContent =
            "3 of 4 complete";
    }


    // DigiDoc task
    const taxDocumentTask =
        document.getElementById("taxDocumentTask");

    const taxDocumentStatus =
        document.getElementById("taxDocumentStatus");

    if (taxDocumentTask) {

        taxDocumentTask.classList.remove("upcoming");
        taxDocumentTask.classList.add("in-progress");

    }

    if (taxDocumentStatus) {

        taxDocumentStatus.classList.remove("upcoming");
        taxDocumentStatus.classList.add("progress");

        taxDocumentStatus.textContent = "Ready";

    }


    // Change Add Information to Edit Information
    if (openTaxFormBtn) {

        openTaxFormBtn.textContent =
            "Edit Information →";

    }


    console.log(
        "SARS frontend status updated successfully."
    );

});

    
