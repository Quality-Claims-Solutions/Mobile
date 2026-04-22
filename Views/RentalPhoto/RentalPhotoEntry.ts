import { FormValidator, ValidationField } from "../Shared/FormValidator.js";
import { validatePhotosOnSubmit } from "../Camera/Camera.js"; 

declare var bootstrap: any;

let claimStorage: any | null = null;
const localforage = (window as any).localforage;
let draftModal: any;
let fleetMatchModal: any;

let VehicleMatchupData: VehicleMatchupData;

document.addEventListener("DOMContentLoaded", () => {
    const btnProceedToRenterDetails = document.getElementById("btnProceedToRenterDetails")!;
    const btnSaveAsDraft = document.getElementById("btnSaveAsDraft")!;

    // Validation setup
    const validator = new FormValidator("divValidationSummary");
    const validationFields: ValidationField[] = [
        { element: document.getElementById("txtOwnCityUnit") as HTMLInputElement, name: "Unit Number", requiredForDraft: true },
        { element: document.getElementById("txtRentLocation") as HTMLInputElement, name: "Rental Location", requiredForDraft: true },
        { element: document.getElementById("txtVin") as HTMLInputElement, name: "VIN", requiredForDraft: true },
        { element: document.getElementById("ddlYear") as HTMLSelectElement, name: "Year", requiredForDraft: false },
        { element: document.getElementById("txtMake") as HTMLInputElement, name: "Make", requiredForDraft: false },
        { element: document.getElementById("txtModel") as HTMLInputElement, name: "Model", requiredForDraft: false },
        { element: document.getElementById("txtLicensePlate") as HTMLInputElement, name: "License Plate", requiredForDraft: false },
        { element: document.getElementById("ddlPlateState") as HTMLSelectElement, name: "License Plate State", requiredForDraft: false },
    ];

    // Submit button events
    btnProceedToRenterDetails.addEventListener("click", async (e) => {
        e.preventDefault();
        var photosAreValid = await validatePhotosOnSubmit();
        var fieldsAreValid = validator.validate({ fields: validationFields, isDraft: false });

        if (photosAreValid && fieldsAreValid) {
            submitFormWithPhotos("final");
        }
    });

    btnSaveAsDraft.addEventListener("click", async (e) => {
        e.preventDefault();
        var fieldsAreValid = validator.validate({ fields: validationFields, isDraft: true });

        if (fieldsAreValid) {
            submitFormWithPhotos("draft");
        }
    });

    // Initialize Camera control and trigger Fleet/Draft lookup
    const unitNumberInput = document.getElementById("txtOwnCityUnit") as HTMLInputElement;
    unitNumberInput.addEventListener("blur", async () => {
        const unitNumber = unitNumberInput.value.trim();
        if (!unitNumber) {
            return;
        }

        // Trigger an event that camera partial listens to in order to set up localForage
        let localForageUniqueIdentifier = `prerental-${unitNumber}`;
        claimStorage = localforage.createInstance({ name: localForageUniqueIdentifier });
        // For Testing purposes only, clear the storage.
        await claimStorage.clear();
        document.dispatchEvent(new CustomEvent("uniqueIdentifierSet", { detail: { localForageUniqueIdentifier } }));

        await getDraftOrFleetData(unitNumber);
    });

    // Draft Modal Setup
    const draftModalElement = document.getElementById("draftModal");
    draftModal = new bootstrap.Modal(draftModalElement!);

    const btnDiscardDraft = document.getElementById("btnDiscardDraft")!;
    btnDiscardDraft.addEventListener("click", async () => { safeCloseModal(draftModal); });

    const btnUseDraft = document.getElementById("btnUseDraft")!;
    btnUseDraft.addEventListener("click", async () => {
        safeCloseModal(draftModal);
        // reload the page with the VehicleMatchdata.Id as a parameter
        window.location.href = `/RentalPhoto/RentalPhotoEntry?preRentalDraftId=${VehicleMatchupData.rentalRecordId}`;
    });

    const btnUseDraftFleetData = document.getElementById("btnUseDraftFleetData")!;
    btnUseDraftFleetData.addEventListener("click", async () => {
        safeCloseModal(draftModal);
        populateVehicleInformation()
    });

    // Fleet Match Modal Setup
    const fleetMatchModalElement = document.getElementById("fleetMatchModal");
    fleetMatchModal = new bootstrap.Modal(fleetMatchModalElement!);

    const btnDismissFleetMatch = document.getElementById("btnDiscardVehicleData")!;
    btnDismissFleetMatch.addEventListener("click", async () => { safeCloseModal(fleetMatchModal); });

    const btnUseVehicleData = document.getElementById("btnUseVehicleData")!;
    btnUseVehicleData.addEventListener("click", async () => {
        safeCloseModal(fleetMatchModal);
        populateVehicleInformation()
    });

    // If the page is loaded from a draft (has an ID), initialize the camera.
    const preRentalDraftId = (document.getElementById("preRentalDraftId") as HTMLInputElement).value;
    if (preRentalDraftId != '0') {
        const unitNumber = unitNumberInput.value.trim();
        let localForageUniqueIdentifier = `prerental-${unitNumber}`;
        claimStorage = localforage.createInstance({ name: localForageUniqueIdentifier });
        claimStorage.clear().then(() => {
            console.log("Cleared");
        });
        document.dispatchEvent(new CustomEvent("uniqueIdentifierSet", { detail: { localForageUniqueIdentifier } }));
    }
});

let lastCheckedUnitNumber = "";
async function getDraftOrFleetData(unitNumber: string) {
    if (unitNumber === lastCheckedUnitNumber) {
        return;
    }

    const response = await fetch(`/RentalPhoto/FleetOrDraftData?unitNumber=${unitNumber}`);
    lastCheckedUnitNumber = unitNumber;

    if (response.ok) {
        VehicleMatchupData = await response.json();

        let licensePlateData = '';
        if (VehicleMatchupData.licensePlate && VehicleMatchupData.licensePlateState) {
            licensePlateData = ` (${VehicleMatchupData.licensePlate} ${VehicleMatchupData.licensePlateState})`;
        }

        if (VehicleMatchupData.rentalRecordId) {
            // This is a Hertz Rental Draft, launch draft modal.
            showDraftModal({
                unit: VehicleMatchupData.fullUnitNumber,
                vin: VehicleMatchupData.vin,
                vehicleInfo: VehicleMatchupData.year + ' ' + VehicleMatchupData.make + ' ' + VehicleMatchupData.model + licensePlateData
            });
        }
        else if (VehicleMatchupData.unitNumber) {
            // This is a Fleet Match, launch Fleet modal.
            showFleetMatchModal({
                unit: VehicleMatchupData.fullUnitNumber,
                vin: VehicleMatchupData.vin,
                vehicleInfo: VehicleMatchupData.year + ' ' + VehicleMatchupData.make + ' ' + VehicleMatchupData.model + licensePlateData
            });
        }
    }
}

function showDraftModal(data: { unit: string; vin: string; vehicleInfo: string; }) {
    (document.getElementById("draftUnit") as HTMLElement).innerText = data.unit;
    (document.getElementById("draftVin") as HTMLElement).innerText = data.vin;
    (document.getElementById("draftVehicleInfo") as HTMLElement).innerText = data.vehicleInfo;

    draftModal.show();
}

function showFleetMatchModal(data: { unit: string; vin: string; vehicleInfo: string; }) {
    (document.getElementById("fleetMatchUnit") as HTMLElement).innerText = data.unit;
    (document.getElementById("fleetMatchVin") as HTMLElement).innerText = data.vin;
    (document.getElementById("fleetMatchVehicleInfo") as HTMLElement).innerText = data.vehicleInfo;

    fleetMatchModal.show();
}

function safeCloseModal(modal: any) {
    (document.activeElement as HTMLElement)?.blur();
    modal.hide();
}

function populateVehicleInformation() {
    if (VehicleMatchupData == null) {
        return;
    }

    (document.getElementById("txtVin") as HTMLInputElement).value = VehicleMatchupData.vin;
    (document.getElementById("ddlYear") as HTMLSelectElement).value = VehicleMatchupData.year;
    (document.getElementById("txtMake") as HTMLInputElement).value = VehicleMatchupData.make;
    (document.getElementById("txtModel") as HTMLInputElement).value = VehicleMatchupData.model;
    (document.getElementById("txtLicensePlate") as HTMLInputElement).value = VehicleMatchupData.licensePlate;
    (document.getElementById("ddlPlateState") as HTMLSelectElement).value = VehicleMatchupData.licensePlateState;
}

async function submitFormWithPhotos(submissionType : string) {
    const form = document.getElementById("frmRentalPhoto") as HTMLFormElement;
    const formData = new FormData(form);

    // Add antiforgery token automatically included in the <form>

    // Load blobs from LocalForage
    const photoEntries = await loadAllPhotoBlobs();

    // May be zero if the user is submitting a draft.
    if (photoEntries.length !== 0) {
        for (const { key, blob } of photoEntries) {
            formData.append("PhotoSubmissions", blob, key + ".jpg");
        }
    }

    formData.append("SubmissionType", submissionType);

    const preRentalId = (document.getElementById("preRentalDraftId") as HTMLInputElement).value;
    if (preRentalId == '0') { // First time submission
        const response = await fetch(form.action, {
            method: "POST",
            body: formData
        });

        if (!response.ok) {
            alert("Upload failed.");
        }

        const result = await response.json();

        // If Proceed to Renter Detials: Redirect to Review page.
        // If Save Draft or Error: Redirect to home.
        if (result.redirectUrl) {
            window.location.href = result.redirectUrl;
        }
    }
    else { // Updating existing submission
        const response = await fetch("UpdateRentalPhotoEntry", {
            method: "PUT",
            body: formData
        });

        if (!response.ok) {
            alert("Update failed.");
        }

        const result = await response.json();

        // If Proceed to Renter Detials: Redirect to Review page.
        // If Save Draft or Error: Redirect to home.
        if (result.redirectUrl) {
            window.location.href = result.redirectUrl;
        }
    }
}

async function loadAllPhotoBlobs(): Promise<{ key: string; blob: Blob; }[]> {
    const keys = await claimStorage.keys();
    const photos: { key: string; blob: Blob }[] = [];

    for (const key of keys) {
        if (!key.startsWith("new-photo-")) continue;

        const blob = await claimStorage.getItem(key);
        if (blob instanceof Blob) {
            photos.push({ key: key.replace("new-photo-",""), blob });
        }
    }

    return photos;
}

// This is an interface designed to capture either a Fleet match or a 
// PreRentalDraft from the controller.
interface VehicleMatchupData
{
    rentalRecordId: number;
    owningLocation: string;
    location: string;
    unitNumber: string;
    fullUnitNumber: string;
    year: string;
    make: string;
    model: string;
    vin: string;
    licensePlate: string;
    licensePlateState: string;
}