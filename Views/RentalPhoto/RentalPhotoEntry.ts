import { FormValidator, ValidationField } from "../Shared/FormValidator.js";
import { validatePhotosOnSubmit } from "../Camera/Camera.js"; 

let claimStorage: any | null = null;
const localforage = (window as any).localforage;

document.addEventListener("DOMContentLoaded", () => {
    const btnProceedToRenterDetails = document.getElementById("btnProceedToRenterDetails")!;
    const btnSaveAsDraft = document.getElementById("btnSaveAsDraft")!;
    const frmRentalPhoto = document.getElementById("frmRentalPhoto") as HTMLFormElement;

    // Validation setup
    const validator = new FormValidator("divValidationSummary");
    const validationFields: ValidationField[] = [
        { element: document.getElementById("txtOwnCityUnit") as HTMLInputElement, name: "Unit Number", requiredForDraft: true },
        { element: document.getElementById("txtRentLocation") as HTMLInputElement, name: "Rental Location", requiredForDraft: true },
        { element: document.getElementById("txtVin") as HTMLInputElement, name: "VIN", requiredForDraft: false },
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
            submitFormWithPhotos(false)
        }
    });

    btnSaveAsDraft.addEventListener("click", async (e) => {
        e.preventDefault();
        var photosAreValid = await validatePhotosOnSubmit();
        var fieldsAreValid = validator.validate({ fields: validationFields, isDraft: true });

        if (photosAreValid && fieldsAreValid) {
            frmRentalPhoto.requestSubmit(btnProceedToRenterDetails);
            //let photoUploadSuccess = await uploadPhotos();
            //if (!photoUploadSuccess) {
            //    alert("There was an issue uploading your photos. Please try again.");
            //    return;
            //}
        }
    });

    // Initialize Camera control
    const unitNumberInput = document.getElementById("txtOwnCityUnit") as HTMLInputElement;
    unitNumberInput.addEventListener("blur", () => {
        const claimNumber = unitNumberInput.value.trim();
        if (!claimNumber) {
            return;
        }

        // Trigger an event that camera partial listens to in order to set up localForage
        let localForageUniqueIdentifier = `prerental-${claimNumber}`;
        claimStorage = localforage.createInstance({ name: localForageUniqueIdentifier });
        document.dispatchEvent(new CustomEvent("uniqueIdentifierSet", { detail: { localForageUniqueIdentifier } }));
    });
});


async function submitFormWithPhotos(isDraft : boolean) {
    const form = document.getElementById("frmRentalPhoto") as HTMLFormElement;
    const formData = new FormData(form);

    // Add antiforgery token automatically included in the <form>

    // Load blobs from LocalForage
    const photoEntries = await loadAllPhotoBlobs();

    for (const { key, blob } of photoEntries) {
        formData.append("PhotoSubmissions", blob, key + ".jpg");
    }

    formData.append("IsDraft", isDraft.toString());

    const response = await fetch(form.action, {
        method: "POST",
        body: formData
    });

    if (!response.ok) {
        alert("Upload failed.");
        return;
    }
}


async function loadAllPhotoBlobs(): Promise<{ key: string; blob: Blob; }[]> {
    const keys = await claimStorage.keys();
    const photos: { key: string; blob: Blob }[] = [];

    for (const key of keys) {
        if (!key.startsWith("photo-")) continue;

        const blob = await claimStorage.getItem(key);
        if (blob instanceof Blob) {
            photos.push({ key, blob });
        }
    }

    return photos;
}