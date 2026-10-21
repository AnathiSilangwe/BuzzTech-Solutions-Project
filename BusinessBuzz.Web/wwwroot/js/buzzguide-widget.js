
document.addEventListener("DOMContentLoaded", function () {
    const widget = document.getElementById("buzzGuideWidget");
    if (!widget) return;

    const launcher = widget.querySelector("#buzzGuideLauncher");
    const panel = widget.querySelector("#buzzGuidePanel");
    const closeButton = widget.querySelector("#buzzGuideClose");
    const questionInput = widget.querySelector("#buzzGuideQuestion");
    const form = widget.querySelector("#buzzGuideForm");
    const messages = widget.querySelector("#buzzGuideMessages");
    const submitButton = form?.querySelector('button[type="submit"]');

    function setOpen(isOpen) {
        widget.classList.toggle("is-open", isOpen);
        launcher.setAttribute("aria-expanded", String(isOpen));
        panel.setAttribute("aria-hidden", String(!isOpen));

        if (isOpen && questionInput) {
            questionInput.focus();
        }
    }

    function scrollToLatest() {
        if (messages) {
            messages.scrollTop = messages.scrollHeight;
        }
    }

    function addMessage(sender, text) {
        const message = document.createElement("div");
        const isEntrepreneur = sender === "Entrepreneur";

        message.className = isEntrepreneur
            ? "buzzguide-message buzzguide-message-user"
            : "buzzguide-message buzzguide-message-assistant";

        const heading = document.createElement("strong");
        heading.textContent = isEntrepreneur ? "You" : "BuzzGuide";

        const paragraph = document.createElement("p");
        paragraph.textContent = text;

        message.append(heading, paragraph);
        messages.appendChild(message);
        scrollToLatest();

        return message;
    }

    launcher.addEventListener("click", function () {
        setOpen(!widget.classList.contains("is-open"));
        if (widget.classList.contains("is-open")) {
            scrollToLatest();
        }
    });

    closeButton.addEventListener("click", function () {
        setOpen(false);
        launcher.focus();
    });

    widget.querySelectorAll("[data-question]").forEach(function (button) {
        button.addEventListener("click", function () {
            questionInput.value = button.dataset.question;
            questionInput.focus();
        });
    });

    if (form) {
        form.addEventListener("submit", async function (event) {
            event.preventDefault();

            const question = questionInput.value.trim();
            if (!question || !submitButton) return;

            submitButton.disabled = true;
            submitButton.textContent = "…";

            try {
                const response = await fetch(form.action, {
                    method: "POST",
                    body: new FormData(form),
                    headers: {
                        "X-Requested-With": "XMLHttpRequest"
                    }
                });

                const data = await response.json();

                if (!response.ok || !data.success) {
                    throw new Error(
                        data.message || "BuzzGuide couldn't process your question."
                    );
                }

                addMessage("Entrepreneur", data.question);
                const answerMessage = addMessage("BuzzGuide", data.answer);

                questionInput.value = "";
                setOpen(true);
                scrollToLatest();
            } catch (error) {
                addMessage(
                    "BuzzGuide",
                    error.message || "Something went wrong. Please try again."
                );
                setOpen(true);
            } finally {
                submitButton.disabled = false;
                submitButton.textContent = "➤";
            }
        });
    }

    setOpen(false);
    scrollToLatest();
});
