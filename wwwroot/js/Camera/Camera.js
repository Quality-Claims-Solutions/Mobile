var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
let claimStorage = null;
let prompts = [];
document.addEventListener("DOMContentLoaded", () => {
    const overlay = document.getElementById("camera-overlay");
    const video = document.getElementById("video");
    const canvas = document.getElementById("canvas");
    const captureButton = document.getElementById("capture");
    const doneButton = document.getElementById("done");
    const clearButton = document.getElementById("clear");
    const labelOverlay = document.getElementById("camera-label");
    const photoDisplay = document.getElementById("photo-display");
    const carouselItems = Array.from(document.querySelectorAll(".custom-carousel-item"));
    let stream = null;
    // Build prompts array from image placeholders for dynamic "Add Photo"
    document.querySelectorAll(".image-placeholder").forEach(el => {
        var _a;
        const item = el;
        if (item.dataset.elementid === 'add-photo-item') {
            return;
        }
        prompts.push({
            id: item.dataset.elementid,
            label: (_a = item.dataset.label) !== null && _a !== void 0 ? _a : 'no-label',
            required: item.dataset.required === 'True',
            placeholderImage: item.dataset.elementid,
            existingAttachment: item.dataset.existingPath
        });
    });
    // Initially disable placeholders until we have a localForage unique identifier
    document.querySelectorAll(".image-placeholder").forEach(item => {
        item.style.pointerEvents = "none";
        item.style.opacity = "0.5";
    });
    // Listen for the unique identifier event from the main page.
    // When we have a unique identifier, we can begin saving photos in the localForage instance.
    document.addEventListener("uniqueIdentifierSet", (e) => __awaiter(void 0, void 0, void 0, function* () {
        const localForageUniqueIdentifier = e.detail.localForageUniqueIdentifier;
        if (!localForageUniqueIdentifier)
            return;
        // Remove additional placeholder images
        document.querySelectorAll('.image-placeholder').forEach(placeholderElement => {
            if (placeholderElement.id.startsWith('placeholder-extra-')) {
                placeholderElement.remove();
            }
        });
        // Remove additional carousel items
        document.querySelectorAll('.custom-carousel-item').forEach(carouselElement => {
            if (carouselElement.id.startsWith('extra-')) {
                carouselElement.remove();
            }
        });
        // Reset placeholder images to original background images
        prompts.forEach(prompt => {
            let placeholderImage = document.getElementById('placeholder-' + prompt.id);
            if (placeholderImage && prompt.placeholderImage) {
                placeholderImage.src = '/content/' + prompt.placeholderImage.replace(/-/g, "_") + '.jpg';
            }
        });
        // Initialize localForage instance for this claim
        claimStorage = localforage.createInstance({ name: localForageUniqueIdentifier });
        // Populate local storage with existing attachments if they exist.
        // This only needs to happen on Rental Photo drafts being edited after photo submission.
        yield populateLocalStorageFromExistingAttachments();
        // Enable placeholders inside the partial
        document.querySelectorAll(".image-placeholder").forEach(item => {
            item.style.pointerEvents = "auto";
            item.style.opacity = "1";
        });
        // Attach click event listeners to all image placeholders.
        document.querySelectorAll('.image-placeholder').forEach(placeholderElement => {
            placeholderElement.addEventListener('click', () => addPlaceholderClickEvent(placeholderElement));
        });
        // Attach click event listeners to all Carousel items
        document.querySelectorAll(".custom-carousel-item").forEach(carouselElement => {
            carouselElement.addEventListener("click", () => addCarouselClickEvent(carouselElement));
        });
        loadImagesFromLocalStorage();
    }));
    // Capture photo
    let capturing = false;
    if (captureButton && canvas && video) {
        console.log("Capture handler attached");
        captureButton.addEventListener("click", () => __awaiter(void 0, void 0, void 0, function* () {
            //if (capturing) return; // prevent re-entry
            //capturing = true;
            try {
                const context = canvas.getContext("2d");
                if (!context)
                    return;
                // size canvas to video
                canvas.width = video.videoWidth;
                canvas.height = video.videoHeight;
                context.drawImage(video, 0, 0, canvas.width, canvas.height);
                let photoBlob = yield canvasToBlobAsync(canvas, "image/jpeg");
                let photoURL = URL.createObjectURL(photoBlob);
                // Use selected carousel item
                let selected = document.querySelector(".custom-carousel-item.selected");
                if (!selected) {
                    selected = document.querySelector(".custom-carousel-item") || undefined;
                }
                if (!selected)
                    return;
                const elementId = selected.id;
                // Check if Selected is the Add Photo option, which requires dynamic handling
                if (selected.id === 'add-photo-item') { // Photo is an "Add Photo"
                    createAndInsertExtraPhoto(photoBlob, true);
                }
                else { // Photo corresponds to an existing prompt
                    const imgTag = document.getElementById("preview-" + elementId);
                    if (imgTag) {
                        imgTag.src = photoURL;
                    }
                    claimStorage.setItem("new-photo-" + elementId, photoBlob);
                    removeChip(elementId);
                    // Show captured image in main camera area
                    if (photoDisplay) {
                        photoDisplay.src = photoURL;
                        photoDisplay.style.display = "block";
                    }
                    // Set the placeholder image
                    const placeholder = document.getElementById('placeholder-' + elementId);
                    if (placeholder) {
                        placeholder.src = photoURL;
                    }
                    // Select the next carousel item that doesn't currently have a photo
                    const currentIndex = carouselItems.indexOf(selected);
                    for (let i = currentIndex + 1; i < carouselItems.length; i++) {
                        const key = "photo-" + carouselItems[i].id;
                        if (!(yield claimStorage.getItem(key))) {
                            carouselItems[i].click();
                            return;
                        }
                    }
                }
            }
            finally {
                capturing = false;
            }
        }));
    }
    function canvasToBlobAsync(canvas, type = "image/jpeg", quality = 1) {
        return new Promise((resolve, reject) => {
            canvas.toBlob((blob) => {
                if (!blob)
                    reject(new Error("Blob conversion failed"));
                else
                    resolve(blob);
            }, type, quality);
        });
    }
    // Function to handle carousel item click events.
    // Called on initial setup and when new items are dynamically created.
    function addCarouselClickEvent(carouselElement) {
        return __awaiter(this, void 0, void 0, function* () {
            // Deselect all items and select the clicked one
            document.querySelectorAll(".custom-carousel-item").forEach(i => i.classList.remove("selected"));
            carouselElement.classList.add("selected");
            // Update label overlay with fade effect
            if (labelOverlay) {
                // Fade out
                labelOverlay.style.opacity = "0";
                // After fade out duration, change text and fade back in
                setTimeout(() => {
                    labelOverlay.textContent = carouselElement.dataset.label || "";
                    labelOverlay.style.opacity = "1";
                }, 300); // match the 0.3s transition in CSS
            }
            // Center the selected item in the carousel
            carouselElement.scrollIntoView({ behavior: "smooth", inline: "center" });
            // Show either previously captured photo or video feed
            const key = "photo-" + carouselElement.id;
            const savedPhoto = yield claimStorage.getItem(key);
            if (savedPhoto) {
                if (photoDisplay) {
                    photoDisplay.src = URL.createObjectURL(savedPhoto);
                    photoDisplay.style.display = "block";
                }
            }
            else {
                if (photoDisplay)
                    photoDisplay.style.display = "none";
            }
        });
    }
    // Function to handle placeholder click events.
    // Called on initial setup and when new items are dynamically created.
    function addPlaceholderClickEvent(placeholderElement) {
        const elementId = placeholderElement.dataset.elementid;
        if (!elementId)
            return;
        console.log('Opening camera for angle ' + elementId + '.');
        openCamera(elementId);
    }
    // This function dynamically adds the photo taken in the extra "Add Photo" option of the camera.
    // Dynamically adds for both the carousel and the placeholder area.
    // Add to local forage controls whether or not the photo is saved to localForage storage, because in initialization cases, the photo is already saved.
    // AddToLocalForage should only be set to true if the image has just been taken.
    function createAndInsertExtraPhoto(photoBlob, addToLocalForage) {
        const id = `extra-${crypto.randomUUID()}`;
        const label = "Other";
        let photoURL = URL.createObjectURL(photoBlob);
        // Create and insert carousel image
        const carouselItem = document.createElement("div");
        carouselItem.className = "custom-carousel-item";
        carouselItem.id = id;
        carouselItem.dataset.label = label;
        carouselItem.innerHTML =
            `<span class="custom-carousel-item-label">${label}</span>
        <img id="preview-${id}" src="${photoURL}" />`;
        // Add click event to the new carousel item
        carouselItem.addEventListener("click", () => addCarouselClickEvent(carouselItem));
        const addPhotoItem = document.getElementById('add-photo-item');
        addPhotoItem.parentElement.insertBefore(carouselItem, addPhotoItem);
        // Create and insert placeholder image
        const placeholderItem = document.createElement("div");
        placeholderItem.className = "image-placeholder";
        placeholderItem.id = "placeholder-" + id;
        placeholderItem.dataset.elementid = id;
        placeholderItem.innerHTML =
            `<img src="${photoURL}" alt="Photo Placeholder" />
             <span class="placeholder-label">${label}</span>`;
        // Add click event to the new placeholder item
        placeholderItem.addEventListener("click", () => addPlaceholderClickEvent(placeholderItem));
        const addPhotoPlaceholder = document.querySelector('#add-photo-item.image-placeholder');
        addPhotoPlaceholder.parentElement.insertBefore(placeholderItem, addPhotoPlaceholder);
        if (addToLocalForage) {
            claimStorage.setItem("new-photo-" + id, photoBlob);
            // Select the "Add Photo" element again for further additions
            addPhotoItem.click();
        }
    }
    // Done button closes camera
    doneButton === null || doneButton === void 0 ? void 0 : doneButton.addEventListener("click", () => {
        closeCamera();
    });
    // Clear button allows retake of image
    clearButton === null || clearButton === void 0 ? void 0 : clearButton.addEventListener("click", () => {
        const selected = document.querySelector(".custom-carousel-item.selected");
        if (!selected)
            return;
        const elementId = selected.id;
        // Clear from localForage
        claimStorage.removeItem("photo-" + elementId);
        claimStorage.removeItem("new-photo-" + elementId);
        // Clear photo display
        const photoURL = URL.createObjectURL(new Blob());
        if (photoDisplay) {
            photoDisplay.src = photoURL;
            photoDisplay.style.display = "none";
        }
        // Reset placeholder image to original
        const placeholder = document.getElementById('placeholder-' + elementId);
        if (placeholder) {
            const prompt = prompts.find(p => p.id === elementId);
            if (prompt && prompt.placeholderImage) {
                placeholder.src = '/content/' + prompt.placeholderImage.replace(/-/g, "_") + '.jpg';
            }
        }
    });
    // Function to open the camera
    function openCamera(selectedItem) {
        overlay === null || overlay === void 0 ? void 0 : overlay.classList.remove("hidden");
        navigator.mediaDevices.getUserMedia({ video: { facingMode: "environment" } })
            .then(s => {
            stream = s;
            if (video) {
                video.srcObject = stream;
                // Start playing if autoplay isn't set on the element
                video.play().catch(() => { });
            }
        })
            .catch(err => console.error("Camera error: ", err));
        // Select the appropriate carousel item
        const item = document.getElementById(selectedItem);
        if (item) {
            item.click();
        }
    }
    ;
    // Function to close the camera
    function closeCamera() {
        overlay === null || overlay === void 0 ? void 0 : overlay.classList.add("hidden");
        if (stream) {
            stream.getTracks().forEach(track => track.stop());
            stream = null;
        }
        if (video) {
            video.srcObject = null;
        }
    }
    ;
    // Uses the paths provided in the data tag of the HTML element (stored in prompts array)
    // to get the server images for a Rental Photo draft that has submitted photos already.
    function populateLocalStorageFromExistingAttachments() {
        return __awaiter(this, void 0, void 0, function* () {
            for (const prompt of prompts) {
                if (prompt.existingAttachment == '')
                    continue;
                const key = "photo-" + prompt.id;
                const url = `/RentalPhoto/Attachment?filepath=${encodeURIComponent(prompt.existingAttachment)}`;
                const response = yield fetch(url);
                const blob = yield response.blob();
                yield claimStorage.setItem(key, blob);
            }
        });
    }
    // Cycles through the placeholder and carousel images and loads any saved images from localForage
    // Then checks the localForage for any extra photos saved with the "Add Photo" option and recreates those in the carousel and placeholders.
    function loadImagesFromLocalStorage() {
        // Restore Image Placeholders
        document.querySelectorAll('.image-placeholder').forEach((el) => __awaiter(this, void 0, void 0, function* () {
            const placeholder = el;
            const elementId = placeholder.dataset.elementid;
            const photoBlob = yield claimStorage.getItem('photo-' + elementId);
            if (photoBlob) {
                const placeholder = document.getElementById('placeholder-' + elementId);
                placeholder.src = URL.createObjectURL(photoBlob);
            }
        }));
        // Restore Carousel Items
        document.querySelectorAll(".custom-carousel-item").forEach((item) => __awaiter(this, void 0, void 0, function* () {
            const key = "photo-" + item.id;
            const photoBlob = yield claimStorage.getItem(key);
            if (photoBlob) {
                const imgEl = document.getElementById("preview-" + item.id);
                if (imgEl) {
                    imgEl.src = URL.createObjectURL(photoBlob);
                }
            }
        }));
        // Get all localForage where key starts with "photo-extra-"
        claimStorage.keys().then(keys => {
            keys.forEach((key) => __awaiter(this, void 0, void 0, function* () {
                if (key.startsWith("photo-extra-")) {
                    const photoBlob = yield claimStorage.getItem(key);
                    if (photoBlob) {
                        createAndInsertExtraPhoto(photoBlob, false);
                    }
                }
            }));
        });
    }
});
export function validatePhotosOnSubmit() {
    return __awaiter(this, void 0, void 0, function* () {
        const missingRequiredPrompts = [];
        const validationSummary = document.getElementById("divValidationSummary");
        for (const prompt of prompts) {
            if (!prompt.required)
                continue;
            const value = yield claimStorage.getItem("photo-" + prompt.id);
            if (!value) {
                missingRequiredPrompts.push(prompt);
            }
        }
        if (missingRequiredPrompts.length === 0)
            return true;
        missingRequiredPrompts.forEach(prompt => {
            const placeholder = document.getElementById("placeholder-" + prompt.id);
            if (placeholder) {
                placeholder.classList.add("missing-required");
            }
            if (!document.getElementById("divValidationSummary").innerHTML.includes("Missing Data")) {
                const header = document.createElement("h4");
                header.textContent = "Missing Data";
                validationSummary.appendChild(header);
            }
            addChip(prompt.label, prompt.id);
        });
        return false;
    });
}
function addChip(name, id) {
    const validationSummary = document.getElementById("divValidationSummary");
    if (document.getElementById("chip-" + id) != null) {
        return;
    }
    const chip = document.createElement("a");
    chip.className = "chip z-depth-1";
    chip.href = "#" + id;
    chip.textContent = name;
    chip.id = "chip-" + id;
    chip.addEventListener("click", e => {
        e.preventDefault();
        const el = document.getElementById('placeholder-' + id);
        if (el) {
            el.scrollIntoView({ behavior: "smooth", block: "center" });
            el.focus();
        }
    });
    validationSummary.appendChild(chip);
}
function removeChip(id) {
    const validationSummary = document.getElementById("divValidationSummary");
    const chip = document.getElementById("chip-" + id);
    if (chip) {
        chip.remove();
    }
    const placeholder = document.getElementById("placeholder-" + id);
    if (placeholder) {
        placeholder.classList.remove("missing-required");
    }
    // Remove the validation container if there are no chips.
    if (!validationSummary.querySelector(".chip")) {
        validationSummary.innerHTML = "";
    }
}
//# sourceMappingURL=Camera.js.map