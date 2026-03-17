document.addEventListener("DOMContentLoaded", () => {
    configureToastNotifications();
    
});

function configureToastNotifications() {
    const container = document.getElementById("toastContainer");

    if (!container) return;

    const message = container.dataset.toastMessage;
    const type = container.dataset.toastType || "info";

    if (!message) return;

    const toast = document.createElement("div");
    toast.id = "toast";
    toast.classList.add(type);
    toast.textContent = message;

    document.body.appendChild(toast);

    setTimeout(() => toast.classList.add("show"), 100);

    setTimeout(() => {
        toast.classList.remove("show");
        setTimeout(() => toast.remove(), 300);
    }, 5000);
}