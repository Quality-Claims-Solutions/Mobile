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
document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("frmRentalPhotoReview");
    let signatureBlob = null;
    const btnEditDraft = document.getElementById("btnEditDraft");
    const btnSubmitPreRental = document.getElementById("btnSubmit");
    const btnUploadTNCRental = document.getElementById("btnUploadTNCRental");
    const btnAcceptSignature = document.getElementById("btnAcceptSignature");
    const divValidationSummary = document.getElementById("divValidationSummary");
    initializeSignaturePad(btnAcceptSignature);
    // Validation setup
    const validator = new FormValidator("divValidationSummary");
    let validationFields = [
        { element: document.getElementById("txtRentalRecord"), name: "Rental Record" },
        { element: document.getElementById("ddlRentalType"), name: "Rental Type" }
    ];
    // Edit Vehicle/Add Photos Button Event
    const rentalPhotoId = document.getElementById("RentalPhotoReview_Id").value;
    btnEditDraft.addEventListener("click", () => {
        // Navigate to the edit page for the current rental photo review, passing the ID as a query parameter
        window.location.href = `/RentalPhoto/RentalPhotoEntry?preRentalDraftId=${rentalPhotoId}&editRequired=true`;
    });
    // Accept Signature Button Event
    btnAcceptSignature.addEventListener("click", (e) => __awaiter(void 0, void 0, void 0, function* () {
        e.preventDefault();
        const canvas = document.getElementById("signature-pad");
        signatureBlob = yield signaturePadToBlob(canvas, "image/png");
    }));
    // Submit button events
    btnSubmitPreRental.addEventListener("click", (e) => __awaiter(void 0, void 0, void 0, function* () {
        e.preventDefault();
        const formData = new FormData(form);
        if (chkSendToRenterEmail.checked) {
            validationFields.push({ element: document.getElementById("txtRenterEmail"), name: "Renter Email" });
        }
        else {
            validationFields = validationFields.filter(f => f.name !== "Renter Email");
        }
        divValidationSummary.innerHTML = "";
        var fieldsAreValid = validator.validate({ fields: validationFields, isDraft: false });
        formData.append("RentalPhotoReview.SignatureSubmission", signatureBlob, "signature.jpg");
        for (const [key, value] of formData.entries()) {
            console.log('FORMDATA: ' + key, 'IS ' + value);
        }
        if (fieldsAreValid) {
            const response = yield fetch(form.action, {
                method: "PUT",
                body: formData
            });
            if (!response.ok) {
                alert("Upload failed.");
            }
            const result = yield response.json();
            // If error, refresh, else back to home page.
            if (result.redirectUrl) {
                window.location.href = result.redirectUrl;
            }
        }
    }));
    // Renter Email Visibility Settings
    const chkSendToRenterEmail = document.getElementById("chkSendToRenterEmail");
    const divRenterEmail = document.getElementById("divRenterEmail");
    const prerentalDisclaimer = document.getElementById("prerentalDisclaimer");
    chkSendToRenterEmail.checked = true; // default to checked
    prerentalDisclaimer.classList.add('hidden');
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
function initializeSignaturePad(btnAcceptSignature) {
    // Initialize signature pad.
    const canvas = document.getElementById("signature-pad");
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
        ctx === null || ctx === void 0 ? void 0 : ctx.scale(ratio, ratio);
        signaturePad.clear();
    }
    window.addEventListener("resize", resizeCanvas);
    // fade in icon on write
    const container = document.querySelector('.signature-container');
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
function signaturePadToBlob(canvas, type = "image/jpeg", quality = 1) {
    return new Promise((resolve, reject) => {
        canvas.toBlob((blob) => {
            if (!blob)
                reject(new Error("Blob conversion failed"));
            else
                resolve(blob);
        }, type, quality);
    });
}
//# sourceMappingURL=RentalPhotoReview.js.map