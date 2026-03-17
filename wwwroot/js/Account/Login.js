document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('loginForm');
    if (!form)
        return;
    const emailRadio = document.getElementById('useEmailRadio');
    const employeeRadio = document.getElementById('useEmployeeRadio');
    const emailGroup = document.getElementById('emailGroup');
    const employeeGroup = document.getElementById('employeeGroup');
    const emailInput = emailGroup.querySelector('input');
    const employeeInput = employeeGroup.querySelector('input');
    const applyMode = () => {
        const usingEmail = emailRadio.checked;
        if (usingEmail) {
            emailGroup.classList.remove('d-none');
            employeeGroup.classList.add('d-none');
            emailInput.disabled = false;
            emailInput.required = true;
            employeeInput.disabled = true;
            employeeInput.required = false;
            employeeInput.setCustomValidity('');
        }
        else {
            emailGroup.classList.add('d-none');
            employeeGroup.classList.remove('d-none');
            emailInput.disabled = true;
            emailInput.required = false;
            emailInput.setCustomValidity('');
            employeeInput.disabled = false;
            employeeInput.required = true;
        }
    };
    // Switch between modes
    emailRadio.addEventListener('change', applyMode);
    employeeRadio.addEventListener('change', applyMode);
    // Initial state (respects whatever the server rendered as checked)
    applyMode();
    // Bootstrap-like HTML5 validation
    form.addEventListener('submit', (event) => {
        if (!form.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        }
        form.classList.add('was-validated');
    });
    // Re-validate on input once user has attempted submit
    form.querySelectorAll('input').forEach((el) => {
        el.addEventListener('input', () => {
            if (form.classList.contains('was-validated')) {
                el.checkValidity();
            }
        });
    });
});
//# sourceMappingURL=Login.js.map