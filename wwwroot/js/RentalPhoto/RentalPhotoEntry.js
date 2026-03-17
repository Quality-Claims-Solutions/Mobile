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
document.addEventListener("DOMContentLoaded", () => {
    const btnProceedToRenterDetails = document.getElementById("btnProceedToRenterDetails");
    const btnSaveAsDraft = document.getElementById("btnSaveAsDraft");
    const frmRentalPhoto = document.getElementById("frmRentalPhoto");
    // Validation setup
    const validator = new FormValidator("divValidationSummary");
    const validationFields = [
        { element: document.getElementById("txtOwnCityUnit"), name: "Unit Number", requiredForDraft: true },
        { element: document.getElementById("txtRentLocation"), name: "Rental Location", requiredForDraft: true },
        { element: document.getElementById("txtVin"), name: "VIN", requiredForDraft: false },
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
            frmRentalPhoto.requestSubmit(btnProceedToRenterDetails);
        }
    }));
    btnSaveAsDraft.addEventListener("click", (e) => __awaiter(void 0, void 0, void 0, function* () {
        e.preventDefault();
        var photosAreValid = yield validatePhotosOnSubmit();
        var fieldsAreValid = validator.validate({ fields: validationFields, isDraft: true });
        if (photosAreValid && fieldsAreValid) {
            frmRentalPhoto.requestSubmit(btnProceedToRenterDetails);
        }
    }));
    // Initialize Camera control
    const unitNumberInput = document.getElementById("txtOwnCityUnit");
    unitNumberInput.addEventListener("blur", () => {
        const claimNumber = unitNumberInput.value.trim();
        if (!claimNumber) {
            return;
        }
        // Trigger an event that camera partial listens to in order to set up localForage
        let localForageUniqueIdentifier = `claim-${claimNumber}`;
        claimStorage = localforage.createInstance({ name: localForageUniqueIdentifier });
        document.dispatchEvent(new CustomEvent("uniqueIdentifierSet", { detail: { localForageUniqueIdentifier } }));
    });
});
//# sourceMappingURL=RentalPhotoEntry.js.map