document.addEventListener("DOMContentLoaded", function () {
    const forms = document.querySelectorAll("form");

    forms.forEach(form => {

        form.addEventListener("submit", function (e) {
            let isValid = true;

            const inputs = form.querySelectorAll("input[data-val='true']");

            inputs.forEach(input => {
                if (!validateField(input)) {
                    isValid = false;
                }
            });

            if (!isValid) {
                e.preventDefault();
            }
        });
        form.querySelectorAll("input[data-val='true']").forEach(input => {
            input.addEventListener("blur", function () {
                validateField(this);
            });
        });
    });
});

function validateField(input) {
    let errorMsg = "";
    const value = input.value.trim();

    const dependency = input.getAttribute("data-val-equalto-other");
    if (dependency) {
        // Acha o campo original (Senha)
        const otherFieldName = dependency.replace("*.", "");
        const otherInput = input.form.querySelector(`[name="${otherFieldName}"]`);

        if (otherInput) {
            const otherValue = otherInput.value.trim();
            const otherMinLength = otherInput.getAttribute("data-val-minlength-min");

            // Condição 1: Se a senha principal estiver vazia
            const isPasswordEmpty = !otherValue;

            // Condição 2: Se a senha principal for curta (se tiver regra de tamanho)
            let isPasswordShort = false;
            if (otherMinLength && otherValue.length < parseInt(otherMinLength)) {
                isPasswordShort = true;
            }

            // SE a senha for inválida, nós limpamos o erro do Confirm e PARAMOS aqui.
            if (isPasswordEmpty || isPasswordShort) {
                // Limpa visualmente o erro antes de sair
                const span = input.form.querySelector(`span[data-valmsg-for="${input.name}"]`);
                if (span) {
                    span.textContent = "";
                    span.classList.remove("field-validation-error");
                }
                return true; // Retorna true (válido) para não bloquear o formulário por enquanto
            }
        }
    }

    // --- Regra: Required ---
    const requiredMsg = input.getAttribute("data-val-required");
    if (requiredMsg && !value) {
        errorMsg = requiredMsg;
    }

    // --- Regra: Email ---
    const emailMsg = input.getAttribute("data-val-email");
    if (!errorMsg && emailMsg && value) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(value)) {
            errorMsg = emailMsg;
        }
    }

    // --- Regra: MinLength ---
    const minLength = input.getAttribute("data-val-minlength-min");
    const minLengthMsg = input.getAttribute("data-val-minlength");
    if (!errorMsg && minLength && value) {
        if (value.length < parseInt(minLength)) {
            errorMsg = minLengthMsg;
        }
    }

    // --- Regra: Compare (Senha igual) ---
    const equalTo = input.getAttribute("data-val-equalto-other");
    const equalToMsg = input.getAttribute("data-val-equalto");
    if (!errorMsg && equalTo && value) {
        // O ASP.NET coloca "*.Password", limpamos o "*."
        const otherFieldName = equalTo.replace("*.", "");

        // CORREÇÃO: Busca o outro campo APENAS dentro do mesmo formulário
        const otherInput = input.form.querySelector(`[name="${otherFieldName}"]`);

        if (otherInput && value !== otherInput.value) {
            errorMsg = equalToMsg;
        }
    }

    // --- Exibição do Erro ---
    const span = input.form.querySelector(`span[data-valmsg-for="${input.name}"]`);

    if (span) {
        span.textContent = errorMsg;
        if (errorMsg) {
            span.classList.add("field-validation-error"); // Classe padrão do .NET
        } else {
            span.classList.remove("field-validation-error");
        }
    }

    return !errorMsg;
}