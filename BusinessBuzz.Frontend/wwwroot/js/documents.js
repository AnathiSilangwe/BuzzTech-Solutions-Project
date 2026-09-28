const previewDocumentBtn =
    document.getElementById("previewDocumentBtn");

const closePreviewBtn =
    document.getElementById("closePreviewBtn");

const documentPreview =
    document.getElementById("documentPreview");

const printDocumentBtn =
    document.getElementById("printDocumentBtn");

const downloadDocumentBtn =
    document.getElementById("downloadDocumentBtn");


previewDocumentBtn.addEventListener("click", function () {

    documentPreview.classList.add("show-preview");

    documentPreview.scrollIntoView({
        behavior: "smooth"
    });

});


closePreviewBtn.addEventListener("click", function () {

    documentPreview.classList.remove("show-preview");

});


printDocumentBtn.addEventListener("click", function () {

    documentPreview.classList.add("show-preview");

    setTimeout(function () {
        window.print();
    }, 200);

});


downloadDocumentBtn.addEventListener("click", function () {

    alert(
        "PDF generation will be connected to the Business Buzz backend."
    );

});