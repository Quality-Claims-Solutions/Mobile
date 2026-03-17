
export interface ValidationField {
    element: HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement;
    name: string;
    requiredForDraft?: boolean;
}

export class FormValidator {
    container: HTMLElement;

    constructor(containerId: string) {
        const container = document.getElementById(containerId);
        if (!container) throw new Error(`Validation container '${containerId}' not found`);
        this.container = container;
    }

    validate({ fields, isDraft }: { fields: ValidationField[]; isDraft: boolean }): boolean {
        this.container.innerHTML = ""; // Clear previous validation messages
        const header = document.createElement("h4");
        header.textContent = "Missing Data";
        this.container.appendChild(header);
        let valid = true;


        fields.forEach(f => {
            const autoRemoval = () => {
                if (f.element.value.trim()) {
                    f.element.classList.remove("is-invalid");
                    this.removeChip(f.element.id);
                }
            };

            if (!(f.element as any)._validatorAttached) {
                // Add the listener on both input and change in case the element is a select or textarea.
                f.element.addEventListener("input", autoRemoval);
                f.element.addEventListener("change", autoRemoval);
                (f.element as any)._validatorAttached = true;
            }

            if ((!isDraft || f.requiredForDraft) && (!f.element.value.trim())) {
                f.element.classList.add("is-invalid");
                this.addChip(f.name, f.element.id);
                valid = false;
            } else {
                f.element.classList.remove("is-invalid");
            }
        });

        return valid;
    }

    private addChip(name: string, id: string) {
        const chip = document.createElement("a");
        chip.className = "chip z-depth-1";
        chip.href = "#" + id;
        chip.textContent = name;
        chip.id = "chip-" + id;

        chip.addEventListener("click", e => {
            e.preventDefault();
            const el = document.getElementById(id);
            if (el) {
                el.scrollIntoView({ behavior: "smooth", block: "center" });
                el.focus();
            }
        });

        this.container.appendChild(chip);
    }

    private removeChip(id: string) {
        const chip = document.getElementById("chip-" + id);
        if (chip) {
            chip.remove();
        }

        // Remove the validation container if there are no chips.
        if (!this.container.querySelector(".chip")) {
            this.container.innerHTML = "";
        }
    }
}