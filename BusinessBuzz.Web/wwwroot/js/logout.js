document.addElementListener("DOMContentLoaded",
    function () {
        const logoutForm = document.getElementById("logoutForm");

        if (!logoutForm){
            return;
        }
        logoutForm.addEventListener("submit", function (event){
            const confirmed = window.confirm(
                "Are you sure you wi=ant to log out?"
            );

            if(!confirmed){
                event.preventDefault();
            }
        });
    }
);