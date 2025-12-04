// auth.js
const API_BASE_URL = 'https://maf-api-service-179671765640.us-central1.run.app/api';

class AuthService {
    constructor() {
        this.token = localStorage.getItem('auth_token');
        this.userEmail = localStorage.getItem('user_email');
    }

    async login(email, password) {
        try {
            const formData = new FormData();
            formData.append('EmailCurrent', email);
            formData.append('Password', password);

            const response = await fetch(`${API_BASE_URL}/maf/u/login`, {
                method: 'POST',
                body: formData
            });

            if (!response.ok) {
                throw new Error('Credenciais inválidas');
            }

            const token = await response.text();

            // Salva token e email no localStorage
            localStorage.setItem('auth_token', token);
            localStorage.setItem('user_email', email);

            this.token = token;
            this.userEmail = email;

            return { success: true, token };
        } catch (error) {
            console.error('Erro no login:', error);
            return { success: false, error: error.message };
        }
    }

    async register(name, email, password) {
        try {
            const formData = new FormData();
            formData.append('Name', name);
            formData.append('EmailCurrent', email);
            formData.append('Password', password);

            const response = await fetch(`${API_BASE_URL}/maf/u/register_user`, {
                method: 'POST',
                body: formData
            });

            if (!response.ok) {
                throw new Error('Erro ao registrar usuário');
            }

            return { success: true };
        } catch (error) {
            console.error('Erro no registro:', error);
            return { success: false, error: error.message };
        }
    }

    logout() {
        localStorage.removeItem('auth_token');
        localStorage.removeItem('user_email');
        this.token = null;
        this.userEmail = null;
        window.location.href = 'index.html';
    }

    isAuthenticated() {
        return !!this.token;
    }

    getAuthHeaders() {
        const headers = {};
        if (this.token) {
            headers['Authorization'] = `Bearer ${this.token}`;
        }
        return headers;
    }

    getUserEmail() {
        return this.userEmail;
    }
}

// Instância global do serviço de autenticação
const authService = new AuthService();