let claimStorage = null;
let localforage;
document.addEventListener("DOMContentLoaded", () => {
    const yearSelect = document.getElementById('cboYear');
    const currentYear = new Date().getFullYear();
    const earliestYear = 1928; // go back until 1928
    // Populate years from current year back to 1928
    for (let y = currentYear; y >= earliestYear; y--) {
        const option = document.createElement('option');
        option.value = y.toString();
        option.textContent = y.toString();
        yearSelect.appendChild(option);
    }
    // Date picker
    flatpickr('#fpDate', {
        dateFormat: 'Y-m-d', // ISO-like
        defaultDate: 'today',
        minDate: 'today',
        allowInput: true,
        disableMobile: false // use Flatpickr UI even on mobile
    });
    // Time picker
    flatpickr('#fpTime', {
        enableTime: true,
        noCalendar: true,
        time_24hr: false, // switch to true if you prefer 24h
        dateFormat: 'h:i K', // e.g., 9:30 AM
        minuteIncrement: 5,
        defaultDate: '09:00'
    });
    const vehicleNumber = document.getElementById("txtVehicleNumber");
    vehicleNumber.addEventListener("blur", () => {
        const claimNumber = vehicleNumber.value.trim();
        if (!claimNumber)
            return;
        // Trigger an event that camera partial listens to in order to set up localForage
        let localForageUniqueIdentifier = `claim-${claimNumber}`;
        claimStorage = localforage.createInstance({ name: localForageUniqueIdentifier });
        document.dispatchEvent(new CustomEvent("uniqueIdentifierSet", { detail: { localForageUniqueIdentifier } }));
    });
});
//# sourceMappingURL=PhotosExpressEntry.js.map