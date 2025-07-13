const API_BASE_URL = 'https://projectje-d6d4arfxb6anhrcj.westeurope-01.azurewebsites.net';

function updateTokenStatus() {
    const token = sessionStorage.getItem('bearerToken');
    const statusDiv = document.getElementById('token-status');
    if (statusDiv) {
        if (token) {
            statusDiv.innerHTML = '<span style="color:green">Logged in with active bearer token.</span>';
        } else {
            statusDiv.innerHTML = '<span style="color:red">You are not logged in. Please login.</span>';
        }
    }
}

async function loginUser() {
    const username = document.getElementById('login-username').value.trim();
    const password = document.getElementById('login-password').value.trim();
    const resultDiv = document.getElementById('login-result');
    const tokenContainer = document.getElementById('token-container');
    const tokenInput = document.getElementById('bearer-token');

    resultDiv.innerHTML = '';
    tokenContainer.style.display = 'none'; // Hide token display until successful login
    tokenInput.value = '';

    if (!username || !password) {
        resultDiv.innerHTML = '<span style="color:red">Please enter both username and password.</span>';
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Cache-Control': 'no-cache, no-store, must-revalidate',
                'Pragma': 'no-cache',
                'Expires': '0'
            },
            body: JSON.stringify({ Username: username, Password: password })
        });

        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            const data = await response.json();
            if (response.ok && data.token) {
                // Display the token but DO NOT automatically set it for other requests
                tokenInput.value = data.token.trim(); // Trim here as well
                tokenContainer.style.display = 'block';
                resultDiv.innerHTML = `<span style='color:green'>Login successful! Copy token and paste below to activate.</span>`;
            } else {
                console.error('Login error response (JSON):', response.status, data);
                resultDiv.innerHTML = `<span style='color:red'>${data.message || data.details || `Login failed. Status: ${response.status}`}</span>`;
            }
        } else {
            const text = await response.text();
            console.error('Login error response (text):', response.status, text);
            resultDiv.innerHTML = `<span style='color:red'>${text || `Login failed. Status: ${response.status}`}</span>`;
        }
    } catch (err) {
        console.error('Fetch error on login:', err);
        resultDiv.innerHTML = `<span style='color:red'>Error: ${err.message}</span>`;
    }
}

function copyToken() {
    const tokenInput = document.getElementById('bearer-token');
    tokenInput.select();
    tokenInput.setSelectionRange(0, 99999);
    document.execCommand('copy');
}

function setManualToken() {
    const manualTokenInput = document.getElementById('manual-token');
    const manualToken = manualTokenInput.value.trim(); // Trim before using
    const tokenInput = document.getElementById('bearer-token');
    const tokenContainer = document.getElementById('token-container');

    if (manualToken) {
        sessionStorage.setItem('bearerToken', manualToken); // Store in sessionStorage
        // Also update the displayed token if it was empty or different
        if (tokenInput.value !== manualToken) {
            tokenInput.value = manualToken;
        }
        tokenContainer.style.display = 'block';
        // Redirect to index.html after setting the token
        window.location.href = 'index.html'; 
    } else {
        sessionStorage.removeItem('bearerToken');
        tokenInput.value = ''; // Clear displayed token
        tokenContainer.style.display = 'none';
    }
    updateTokenStatus();
}

// On page load, update token status and show token if present
window.addEventListener('DOMContentLoaded', () => {
    updateTokenStatus();
    const token = sessionStorage.getItem('bearerToken');
    const tokenInput = document.getElementById('bearer-token');
    const tokenContainer = document.getElementById('token-container');
    if (token && tokenInput && tokenContainer) {
        tokenInput.value = token;
        tokenContainer.style.display = 'block';
    }
}); 