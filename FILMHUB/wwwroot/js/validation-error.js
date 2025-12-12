document.getElementById('registerForm').addEventListener('submit', function (e) {
    let isValid = true;
    
    const name = this.querySelector('[name="Name"]').value.trim();
    const email = this.querySelector('[name="Email"]').value.trim();
    const password = this.querySelector('[name="Password"]').value;
    const confirm = this.querySelector('[name="ConfirmPassword"]').value;
    
    
    document.getElementById('nameError').textContent = '';
    document.getElementById('emailError').textContent = '';
    document.getElementById('passwordError').textContent = '';
    document.getElementById('confirmPasswordError').textContent = '';
    
    if (!name) {
        document.getElementById('nameError').textContent = 'O nome é obrigatório.';
        isValid = false;
    }
    
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!email) {
        document.getElementById('emailError').textContent = 'O email é obrigatório.'
        isValid = false;
    } else if (!emailPattern.test(email)) {
        document.getElementById('emailError').textContent = 'Email inválido.';
        isValid = false;
    }

    if (!password) {
        document.getElementById('passwordError').textContent = 'A senha é obrigatória.';
        isValid = false;
    } else if (password.length < 6) {
        document.getElementById('passwordError').textContent = 'A senha deve ter pelo menos 6 caracteres.';
        isValid = false;
    }

    if (!confirm && password && password.length >= 6) {
        document.getElementById('confirmPasswordError').textContent = 'Confirme a senha.';
        isValid = false;
    } else if (password !== confirm && password) {
        document.getElementById('confirmPasswordError').textContent = 'As senhas não coincidem.';
        isValid = false;
    }

    if (!isValid) {
        e.preventDefault();
    }
})