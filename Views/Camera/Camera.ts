// wwwroot/js/Camera/camera.ts

declare const localforage: any;

let claimStorage: any | null = null;

interface PhotoPrompt {
    id: string;
    label: string;
    required: boolean;
    placeholderImage?: string;
}

let prompts: PhotoPrompt[] = [];

document.addEventListener("DOMContentLoaded", () => {
    const overlay = document.getElementById("camera-overlay") as HTMLElement | null;
    const video = document.getElementById("video") as HTMLVideoElement | null;
    const canvas = document.getElementById("canvas") as HTMLCanvasElement | null;
    const captureButton = document.getElementById("capture") as HTMLButtonElement | null;
    const doneButton = document.getElementById("done") as HTMLButtonElement | null;
    const clearButton = document.getElementById("clear") as HTMLButtonElement | null;
    const labelOverlay = document.getElementById("camera-label") as HTMLElement | null;
    const photoDisplay = document.getElementById("photo-display") as HTMLImageElement | null;
    const carouselItems = Array.from(document.querySelectorAll<HTMLElement>(".custom-carousel-item"));


    let stream: MediaStream | null = null;

    // Build prompts array from carousel items for dynamic "Add Photo"
    document.querySelectorAll(".custom-carousel-item").forEach(el => {
        const item = el as HTMLElement;
        prompts.push({
            id: el.id,
            label: item.dataset.label ?? 'no-label',
            required: item.dataset.required === 'True',
            placeholderImage: el.id
        });
    });

    // Initially disable placeholders until we have a localForage unique identifier
    document.querySelectorAll(".image-placeholder").forEach(item => {
        (item as HTMLElement).style.pointerEvents = "none";
        (item as HTMLElement).style.opacity = "0.5";
    });

    // Listen for the unique identifier event from the main page.
    // When we have a unique identifier, we can begin saving photos in the localForage instance.
    document.addEventListener("uniqueIdentifierSet", async (e: any) => {
        const localForageUniqueIdentifier = e.detail.localForageUniqueIdentifier;
        if (!localForageUniqueIdentifier) return;

        // Remove additional placeholder images
        document.querySelectorAll<HTMLElement>('.image-placeholder').forEach(placeholderElement => {
            if (placeholderElement.id.startsWith('placeholder-extra-')) {
                placeholderElement.remove();
            }
        });

        // Remove additional carousel items
        document.querySelectorAll<HTMLElement>('.custom-carousel-item').forEach(carouselElement => {
            if (carouselElement.id.startsWith('extra-')) {
                carouselElement.remove();
            }
        });

        // Reset placeholder images to original background images
        prompts.forEach(prompt => {
            let placeholderImage = document.getElementById('placeholder-' + prompt.id) as HTMLImageElement | null;
            if (placeholderImage && prompt.placeholderImage) {
                placeholderImage.src = '/content/' + prompt.placeholderImage.replace(/-/g, "_") + '.jpg';
            }
        });


        // Initialize localForage instance for this claim
        claimStorage = localforage.createInstance({ name: localForageUniqueIdentifier });

        // Enable placeholders inside the partial
        document.querySelectorAll(".image-placeholder").forEach(item => {
            (item as HTMLElement).style.pointerEvents = "auto";
            (item as HTMLElement).style.opacity = "1";
        });

        // Attach click event listeners to all image placeholders.
        document.querySelectorAll<HTMLElement>('.image-placeholder').forEach(placeholderElement => {
            placeholderElement.addEventListener('click', () => addPlaceholderClickEvent(placeholderElement));
        });

        // Attach click event listeners to all Carousel items
        document.querySelectorAll<HTMLElement>(".custom-carousel-item").forEach(carouselElement => {
            carouselElement.addEventListener("click", () => addCarouselClickEvent(carouselElement));
        });

        loadImagesFromLocalStorage();
    });

    // Capture photo
    if (captureButton && canvas && video) {
        captureButton.addEventListener("click", async () => {
            const context = canvas.getContext("2d");
            if (!context) return;

            // size canvas to video
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            context.drawImage(video, 0, 0, canvas.width, canvas.height);

            const imgData = canvas.toDataURL("image/png");

            // Use selected carousel item
            let selected = document.querySelector<HTMLElement>(".custom-carousel-item.selected");
            if (!selected) {
                selected = document.querySelector<HTMLElement>(".custom-carousel-item") || undefined as any;
            }
            if (!selected) return;
            const elementId = selected.id;

            // Check if Selected is the Add Photo option, which requires dynamic handling
            if (selected.id === 'add-photo-item') { // Photo is an "Add Photo"
                createAndInsertExtraPhoto(imgData, true);
            }
            else { // Photo corresponds to an existing prompt
                const imgTag = document.getElementById("preview-" + elementId) as HTMLImageElement | null;
                if (imgTag) imgTag.src = imgData;

                claimStorage.setItem("photo-" + elementId, imgData);

                // Show captured image in main camera area
                if (photoDisplay) {
                    photoDisplay.src = imgData;
                    photoDisplay.style.display = "block";
                }

                // Set the placeholder image
                const placeholder = document.getElementById('placeholder-' + elementId) as HTMLImageElement;
                if (placeholder) {
                    placeholder.src = imgData;
                }

                // Select the next carousel item that doesn't currently have a photo
                const currentIndex = carouselItems.indexOf(selected);
                for (let i = currentIndex + 1; i < carouselItems.length; i++) {
                    const key = "photo-" + carouselItems[i].id;
                    if (!await claimStorage.getItem(key)) {
                        carouselItems[i].click();
                        return;
                    }
                }
            }
        });
    }

    // Function to handle carousel item click events.
    // Called on initial setup and when new items are dynamically created.
    async function addCarouselClickEvent(carouselElement: HTMLElement) {
        // Deselect all items and select the clicked one
        document.querySelectorAll<HTMLElement>(".custom-carousel-item").forEach(i => i.classList.remove("selected"));
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
        const savedPhoto: string = await claimStorage.getItem(key);
        if (savedPhoto) {
            if (photoDisplay) {
                photoDisplay.src = savedPhoto;
                photoDisplay.style.display = "block";
            }
        } else {
            if (photoDisplay) photoDisplay.style.display = "none";
        }
    }

    // Function to handle placeholder click events.
    // Called on initial setup and when new items are dynamically created.
    function addPlaceholderClickEvent(placeholderElement: HTMLElement) {
        const elementId = placeholderElement.dataset.elementid;
        if (!elementId) return;
        console.log('Opening camera for angle ' + elementId + '.');
        openCamera(elementId);
    }

    // This function dynamically adds the photo taken in the extra "Add Photo" option of the camera.
    // Dynamically adds for both the carousel and the placeholder area.
    // Add to local forage controls whether or not the photo is saved to localForage storage, because in initialization cases, the photo is already saved.
    // AddToLocalForage should only be set to true if the image has just been taken.
    function createAndInsertExtraPhoto(imgData: string, addToLocalForage: boolean) {
        const id = `extra-${crypto.randomUUID()}`;
        const label = "Other";

        // Create and insert carousel image
        const carouselItem = document.createElement("div");
        carouselItem.className = "custom-carousel-item";
        carouselItem.id = id;
        carouselItem.dataset.label = label;

        carouselItem.innerHTML =
            `<span class="custom-carousel-item-label">${label}</span>
        <img id="preview-${id}" src="${imgData}" />`;

        // Add click event to the new carousel item
        carouselItem.addEventListener("click", () => addCarouselClickEvent(carouselItem));

        const addPhotoItem = document.getElementById('add-photo-item')!;
        addPhotoItem.parentElement!.insertBefore(carouselItem, addPhotoItem);

        // Create and insert placeholder image
        const placeholderItem = document.createElement("div");
        placeholderItem.className = "image-placeholder";
        placeholderItem.id = "placeholder-" + id;
        placeholderItem.dataset.elementid = id;

        placeholderItem.innerHTML =
            `<img src="${imgData}" alt="Photo Placeholder" />
             <span class="placeholder-label">${label}</span>`;

        // Add click event to the new placeholder item
        placeholderItem.addEventListener("click", () => addPlaceholderClickEvent(placeholderItem));

        const addPhotoPlaceholder = document.querySelector('#add-photo-item.image-placeholder');
        addPhotoPlaceholder.parentElement!.insertBefore(placeholderItem, addPhotoPlaceholder);

        if (addToLocalForage) {
            claimStorage.setItem("photo-" + id, imgData);

            // Select the "Add Photo" element again for further additions
            addPhotoItem.click();
        }
    }

    // Done button closes camera
    doneButton?.addEventListener("click", () => {
        closeCamera();
    });



    // Clear button wipes out claimStorage photos
    clearButton?.addEventListener("click", () => {
        document.querySelectorAll<HTMLElement>(".custom-carousel-item").forEach(item => {
            const key = "photo-" + item.id;
            claimStorage.removeItem(key);
            const imgEl = document.getElementById("preview-" + item.id) as HTMLImageElement | null;
            if (imgEl) imgEl.src = "";
        });
    });


    // Function to open the camera
    function openCamera(selectedItem: string) {
        overlay?.classList.remove("hidden");

        navigator.mediaDevices.getUserMedia({ video: { facingMode: "environment" } })
            .then(s => {
                stream = s;
                if (video) {
                    video.srcObject = stream;
                    // Start playing if autoplay isn't set on the element
                    video.play().catch(() => { /* ignore play errors */ });
                }
            })
            .catch(err => console.error("Camera error: ", err));

        // Select the appropriate carousel item
        const item = document.getElementById(selectedItem) as HTMLElement | null;
        if (item) {
            item.click();
        }
    };

    // Function to close the camera
    function closeCamera() {
        overlay?.classList.add("hidden");
        if (stream) {
            stream.getTracks().forEach(track => track.stop());
            stream = null;
        }
        if (video) {
            video.srcObject = null;
        }
    };

    // Cycles through the placeholder and carousel images and loads any saved images from localForage
    // Then checks the localForage for any extra photos saved with the "Add Photo" option and recreates those in the carousel and placeholders.
    function loadImagesFromLocalStorage() {
        // Restore Image Placeholders
        document.querySelectorAll('.image-placeholder').forEach(async el => {
            const placeholder = el as HTMLElement;
            const elementId = placeholder.dataset.elementid;
            let imageData: string = await claimStorage.getItem('photo-' + elementId)
            if (imageData) {
                const placeholder = document.getElementById('placeholder-' + elementId) as HTMLImageElement;
                placeholder.src = imageData;
            }
        });

        // Restore Carousel Items
        document.querySelectorAll<HTMLElement>(".custom-carousel-item").forEach(async item => {
            const key = "photo-" + item.id;
            const imgData: string = await claimStorage.getItem(key);
            if (imgData) {
                const imgEl = document.getElementById("preview-" + item.id) as HTMLImageElement | null;
                if (imgEl) imgEl.src = imgData;
            }
        });

        // Get all localForage where key starts with "photo-extra-"
        claimStorage.keys().then(keys => {
            keys.forEach(async key => {
                if (key.startsWith("photo-extra-")) {
                    const imgData: string = await claimStorage.getItem(key);
                    if (imgData) {
                        createAndInsertExtraPhoto(imgData, false);
                    }
                }
            });
        });
    }
});

export async function validatePhotosOnSubmit() {
    const missingRequiredPrompts = [];

    for (const prompt of prompts) {
        if (!prompt.required) continue;

        const value = await claimStorage.getItem("photo-" + prompt.id);

        if (!value) {
            missingRequiredPrompts.push(prompt);
        }
    }

    if (missingRequiredPrompts.length === 0) return true;

    missingRequiredPrompts.forEach(prompt => {
        const placeholder = document.getElementById("placeholder-" + prompt.id);
        if (placeholder) {
            placeholder.classList.add("missing-required");
        }
    });

    return false;
}