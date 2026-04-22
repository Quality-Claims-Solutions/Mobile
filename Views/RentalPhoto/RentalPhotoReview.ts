declare var SignaturePad: any;
import { FormValidator, ValidationField } from "../Shared/FormValidator.js";

document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("frmRentalPhotoReview") as HTMLFormElement;
    let signatureBlob: Blob | null = null;

    const btnEditDraft = document.getElementById("btnEditDraft") as HTMLButtonElement;
    const btnSubmitPreRental = document.getElementById("btnSubmit")!;
    const btnUploadTNCRental = document.getElementById("btnUploadTNCRental")!;
    const btnAcceptSignature = document.getElementById("btnAcceptSignature") as HTMLButtonElement;
    const divValidationSummary = document.getElementById("divValidationSummary") as HTMLElement;

    initializeSignaturePad(btnAcceptSignature);

    // Validation setup
    const validator = new FormValidator("divValidationSummary");
    let validationFields: ValidationField[] = [
        { element: document.getElementById("txtRentalRecord") as HTMLInputElement, name: "Rental Record" },
        { element: document.getElementById("ddlRentalType") as HTMLSelectElement, name: "Rental Type" }
    ];

    // Edit Vehicle/Add Photos Button Event
    const rentalPhotoId = (document.getElementById("RentalPhotoReview_Id") as HTMLInputElement).value;
    btnEditDraft.addEventListener("click", () => {
        // Navigate to the edit page for the current rental photo review, passing the ID as a query parameter
        window.location.href = `/RentalPhoto/RentalPhotoEntry?preRentalDraftId=${rentalPhotoId}&editRequired=true`;
    });

    // Accept Signature Button Event
    btnAcceptSignature.addEventListener("click", async (e) => {
        e.preventDefault();

        const canvas = document.getElementById("signature-pad") as HTMLCanvasElement;
        signatureBlob = await signaturePadToBlob(canvas, "image/png")
    });

    // Submit button events
    btnSubmitPreRental.addEventListener("click", async (e) => {
        e.preventDefault();
        const formData = new FormData(form);

        if (chkSendToRenterEmail.checked) {
            validationFields.push({ element: document.getElementById("txtRenterEmail") as HTMLInputElement, name: "Renter Email" });
        }
        else {
            validationFields = validationFields.filter(f => f.name !== "Renter Email");
        }

        divValidationSummary.innerHTML = "";
        var fieldsAreValid = validator.validate({ fields: validationFields, isDraft: false });

        formData.append("RentalPhotoReview.SignatureSubmission", signatureBlob, "signature.jpg")

        for (const [key, value] of formData.entries()) {
            console.log('FORMDATA: ' + key, 'IS ' + value);
        }

        if (fieldsAreValid) {
            const response = await fetch(form.action, {
                method: "PUT",
                body: formData
            });

            if (!response.ok) {
                alert("Upload failed.");
            }

            const result = await response.json();

            // If error, refresh, else back to home page.
            if (result.redirectUrl) {
                window.location.href = result.redirectUrl;
            }
        }
    });

    // Renter Email Visibility Settings
    const chkSendToRenterEmail = document.getElementById("chkSendToRenterEmail") as HTMLInputElement;
    const divRenterEmail = document.getElementById("divRenterEmail") as HTMLInputElement;
    const prerentalDisclaimer = document.getElementById("prerentalDisclaimer") as HTMLElement;

    chkSendToRenterEmail.checked = true; // default to checked
    prerentalDisclaimer.classList.add('hidden')

    chkSendToRenterEmail.addEventListener("change", () => {
        if (chkSendToRenterEmail.checked) {
            divRenterEmail.classList.remove('hidden');
            prerentalDisclaimer.classList.add('hidden');
        }
        else {
            divRenterEmail.classList.add('hidden');
            prerentalDisclaimer.classList.remove('hidden');
        }
    });
});

function initializeSignaturePad(btnAcceptSignature : HTMLButtonElement) {
    // Initialize signature pad.
    const canvas = document.getElementById("signature-pad") as HTMLCanvasElement;
    const signaturePad = new SignaturePad(canvas, {
        backgroundColor: "rgba(255,255,255,0)",
        penColor: "blue"
    });

    function resizeCanvas() {
        const ratio = Math.max(window.devicePixelRatio || 1, 1);
        const rect = canvas.getBoundingClientRect();

        canvas.width = rect.width * ratio;
        canvas.height = rect.height * ratio;
        const ctx = canvas.getContext('2d');
        ctx?.scale(ratio, ratio);
        signaturePad.clear();
    }
    window.addEventListener("resize", resizeCanvas);

    // fade in icon on write
    const container = document.querySelector('.signature-container') as HTMLElement;
    canvas.addEventListener('pointerdown', () => {
        container.classList.add('active');
        btnAcceptSignature.classList.remove('disabled');
    });

    resizeCanvas();

    document.getElementById("clear-signature").addEventListener("click", () => {
        signaturePad.clear();
        setTimeout(() => {
            container.classList.remove('active');
            btnAcceptSignature.classList.add('disabled');
        }, 300);
    });
}

function signaturePadToBlob(canvas: HTMLCanvasElement, type = "image/jpeg", quality = 1): Promise<Blob> {
    return new Promise((resolve, reject) => {
        canvas.toBlob((blob) => {
            if (!blob) reject(new Error("Blob conversion failed"));
            else resolve(blob);
        }, type, quality);
    });
}