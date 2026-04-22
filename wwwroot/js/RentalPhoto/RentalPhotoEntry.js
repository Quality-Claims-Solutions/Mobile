var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { FormValidator } from "../Shared/FormValidator.js";
import { validatePhotosOnSubmit } from "../Camera/Camera.js";
let claimStorage = null;
const localforage = window.localforage;
let draftModal;
let fleetMatchModal;
let VehicleMatchupData;
document.addEventListener("DOMContentLoaded", () => {
    const btnProceedToRenterDetails = document.getElementById("btnProceedToRenterDetails");
    const btnSaveAsDraft = document.getElementById("btnSaveAsDraft");
    // Validation setup
    const validator = new FormValidator("divValidationSummary");
    const validationFields = [
        { element: document.getElementById("txtOwnCityUnit"), name: "Unit Number", requiredForDraft: true },
        { element: document.getElementById("txtRentLocation"), name: "Rental Location", requiredForDraft: true },
        { element: document.getElementById("txtVin"), name: "VIN", requiredForDraft: true },
        { element: document.getElementById("ddlYear"), name: "Year", requiredForDraft: false },
        { element: document.getElementById("txtMake"), name: "Make", requiredForDraft: false },
        { element: document.getElementById("txtModel"), name: "Model", requiredForDraft: false },
        { element: document.getElementById("txtLicensePlate"), name: "License Plate", requiredForDraft: false },
        { element: document.getElementById("ddlPlateState"), name: "License Plate State", requiredForDraft: false },
    ];
    // Submit button events
    btnProceedToRenterDetails.addEventListener("click", (e) => __awaiter(void 0, void 0, void 0, function* () {
        e.preventDefault();
        var photosAreValid = yield validatePhotosOnSubmit();
        var fieldsAreValid = validator.validate({ fields: validationFields, isDraft: false });
        if (photosAreValid && fieldsAreValid) {
            submitFormWithPhotos("final");
        }
    }));
    btnSaveAsDraft.addEventListener("click", (e) => __awaiter(void 0, void 0, void 0, function* () {
        e.preventDefault();
        var fieldsAreValid = validator.validate({ fields: validationFields, isDraft: true });
        if (fieldsAreValid) {
            submitFormWithPhotos("draft");
        }
    }));
    // Initialize Camera control and trigger Fleet/Draft lookup
    const unitNumberInput = document.getElementById("txtOwnCityUnit");
    unitNumberInput.addEventListener("blur", () => __awaiter(void 0, void 0, void 0, function* () {
        const unitNumber = unitNumberInput.value.trim();
        if (!unitNumber) {
            return;
        }
        // Trigger an event that camera partial listens to in order to set up localForage
        let localForageUniqueIdentifier = `prerental-${unitNumber}`;
        claimStorage = localforage.createInstance({ name: localForageUniqueIdentifier });
        // For Testing purposes only, clear the storage.
        yield claimStorage.clear();
        document.dispatchEvent(new CustomEvent("uniqueIdentifierSet", { detail: { localForageUniqueIdentifier } }));
        yield getDraftOrFleetData(unitNumber);
    }));
    // Draft Modal Setup
    const draftModalElement = document.getElementById("draftModal");
    draftModal = new bootstrap.Modal(draftModalElement);
    const btnDiscardDraft = document.getElementById("btnDiscardDraft");
    btnDiscardDraft.addEventListener("click", () => __awaiter(void 0, void 0, void 0, function* () { safeCloseModal(draftModal); }));
    const btnUseDraft = document.getElementById("btnUseDraft");
    btnUseDraft.addEventListener("click", () => __awaiter(void 0, void 0, void 0, function* () {
        safeCloseModal(draftModal);
        // reload the page with the VehicleMatchdata.Id as a parameter
        window.location.href = `/RentalPhoto/RentalPhotoEntry?preRentalDraftId=${VehicleMatchupData.rentalRecordId}`;
    }));
    const btnUseDraftFleetData = document.getElementById("btnUseDraftFleetData");
    btnUseDraftFleetData.addEventListener("click", () => __awaiter(void 0, void 0, void 0, function* () {
        safeCloseModal(draftModal);
        populateVehicleInformation();
    }));
    // Fleet Match Modal Setup
    const fleetMatchModalElement = document.getElementById("fleetMatchModal");
    fleetMatchModal = new bootstrap.Modal(fleetMatchModalElement);
    const btnDismissFleetMatch = document.getElementById("btnDiscardVehicleData");
    btnDismissFleetMatch.addEventListener("click", () => __awaiter(void 0, void 0, void 0, function* () { safeCloseModal(fleetMatchModal); }));
    const btnUseVehicleData = document.getElementById("btnUseVehicleData");
    btnUseVehicleData.addEventListener("click", () => __awaiter(void 0, void 0, void 0, function* () {
        safeCloseModal(fleetMatchModal);
        populateVehicleInformation();
    }));
    // If the page is loaded from a draft (has an ID), initialize the camera.
    const preRentalDraftId = document.getElementById("preRentalDraftId").value;
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
function getDraftOrFleetData(unitNumber) {
    return __awaiter(this, void 0, void 0, function* () {
        if (unitNumber === lastCheckedUnitNumber) {
            return;
        }
        const response = yield fetch(`/RentalPhoto/FleetOrDraftData?unitNumber=${unitNumber}`);
        lastCheckedUnitNumber = unitNumber;
        if (response.ok) {
            VehicleMatchupData = yield response.json();
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
    });
}
function showDraftModal(data) {
    document.getElementById("draftUnit").innerText = data.unit;
    document.getElementById("draftVin").innerText = data.vin;
    document.getElementById("draftVehicleInfo").innerText = data.vehicleInfo;
    draftModal.show();
}
function showFleetMatchModal(data) {
    document.getElementById("fleetMatchUnit").innerText = data.unit;
    document.getElementById("fleetMatchVin").innerText = data.vin;
    document.getElementById("fleetMatchVehicleInfo").innerText = data.vehicleInfo;
    fleetMatchModal.show();
}
function safeCloseModal(modal) {
    var _a;
    (_a = document.activeElement) === null || _a === void 0 ? void 0 : _a.blur();
    modal.hide();
}
function populateVehicleInformation() {
    if (VehicleMatchupData == null) {
        return;
    }
    document.getElementById("txtVin").value = VehicleMatchupData.vin;
    document.getElementById("ddlYear").value = VehicleMatchupData.year;
    document.getElementById("txtMake").value = VehicleMatchupData.make;
    document.getElementById("txtModel").value = VehicleMatchupData.model;
    document.getElementById("txtLicensePlate").value = VehicleMatchupData.licensePlate;
    document.getElementById("ddlPlateState").value = VehicleMatchupData.licensePlateState;
}
function submitFormWithPhotos(submissionType) {
    return __awaiter(this, void 0, void 0, function* () {
        const form = document.getElementById("frmRentalPhoto");
        const formData = new FormData(form);
        // Add antiforgery token automatically included in the <form>
        // Load blobs from LocalForage
        const photoEntries = yield loadAllPhotoBlobs();
        // May be zero if the user is submitting a draft.
        if (photoEntries.length !== 0) {
            for (const { key, blob } of photoEntries) {
                formData.append("PhotoSubmissions", blob, key + ".jpg");
            }
        }
        formData.append("SubmissionType", submissionType);
        const preRentalId = document.getElementById("preRentalDraftId").value;
        if (preRentalId == '0') { // First time submission
            const response = yield fetch(form.action, {
                method: "POST",
                body: formData
            });
            if (!response.ok) {
                alert("Upload failed.");
            }
            const result = yield response.json();
            // If Proceed to Renter Detials: Redirect to Review page.
            // If Save Draft or Error: Redirect to home.
            if (result.redirectUrl) {
                window.location.href = result.redirectUrl;
            }
        }
        else { // Updating existing submission
            const response = yield fetch("UpdateRentalPhotoEntry", {
                method: "PUT",
                body: formData
            });
            if (!response.ok) {
                alert("Update failed.");
            }
            const result = yield response.json();
            // If Proceed to Renter Detials: Redirect to Review page.
            // If Save Draft or Error: Redirect to home.
            if (result.redirectUrl) {
                window.location.href = result.redirectUrl;
            }
        }
    });
}
function loadAllPhotoBlobs() {
    return __awaiter(this, void 0, void 0, function* () {
        const keys = yield claimStorage.keys();
        const photos = [];
        for (const key of keys) {
            if (!key.startsWith("new-photo-"))
                continue;
            const blob = yield claimStorage.getItem(key);
            if (blob instanceof Blob) {
                photos.push({ key: key.replace("new-photo-", ""), blob });
            }
        }
        return photos;
    });
}
//# sourceMappingURL=RentalPhotoEntry.js.map