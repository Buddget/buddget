document.addEventListener("DOMContentLoaded", function () {
    const tabButtons = document.querySelectorAll(".tab-btn, .tab-banned-btn");
    const tabPanes = document.querySelectorAll(".tab-pane");

    tabButtons.forEach(button => {
        button.addEventListener("click", function () {

            tabButtons.forEach(btn => btn.classList.remove("active"));
            this.classList.add("active");

            const targetTab = this.getAttribute("data-tab");

            tabPanes.forEach(pane => {
                pane.classList.remove("active");
                if (pane.id === targetTab) {
                    pane.classList.add("active");
                }
            });
        });
    });
});