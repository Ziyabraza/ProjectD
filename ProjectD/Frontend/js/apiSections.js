// Define your API base URL here. Change this to your deployed API URL when going live.
const API_BASE_URL = 'https://projectje-d6d4arfxb6anhrcj.westeurope-01.azurewebsites.net';

// --- Auth Section ---
function updateTokenStatus() {
    const token = localStorage.getItem('bearerToken');
    const statusDiv = document.getElementById('token-status');
    if (token) {
        statusDiv.innerHTML = '<span style="color:green">Bearer token is set and active.</span>';
    } else {
        statusDiv.innerHTML = '<span style="color:red">No active bearer token. Please login and set token, or paste a token.</span>';
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
        localStorage.setItem('bearerToken', manualToken); // Store trimmed token
        // Also update the displayed token if it was empty or different
        if (tokenInput.value !== manualToken) {
            tokenInput.value = manualToken;
        }
        tokenContainer.style.display = 'block';
    } else {
        localStorage.removeItem('bearerToken');
        tokenInput.value = ''; // Clear displayed token
        tokenContainer.style.display = 'none';
    }
    updateTokenStatus();
}

function checkToken(resultDiv) {
    const token = localStorage.getItem('bearerToken');
    if (!token) {
        if (resultDiv) resultDiv.innerHTML = '<span style="color:red">You must set a bearer token to use this feature.</span>';
        return false;
    }
    return true;
}

// --- Flights Section ---
async function getFlightById() {
    const id = document.getElementById('flight-id-input').value.trim();
    const resultDiv = document.getElementById('flight-id-result');
    resultDiv.innerHTML = '';
    if (!checkToken(resultDiv)) return;
    if (!id) {
        resultDiv.innerHTML = '<span style="color:red">Please enter a flight ID.</span>';
        return;
    }
    try {
        const response = await fetch(`${API_BASE_URL}/api/flight/${id}`, {
            headers: getAuthHeader()
        });
        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            const data = await response.json();
            if (response.ok) {
                resultDiv.innerHTML = renderFlightData(data);
            } else {
                console.error('Get Flight by ID error response (JSON):', response.status, data);
                resultDiv.innerHTML = `<span style='color:red'>${data.message || data.details || `Error: Status ${response.status}`}</span>`;
            }
        } else {
            const text = await response.text();
            console.error('Get Flight by ID error response (text):', response.status, text);
            resultDiv.innerHTML = `<span style='color:red'>${text || `Error: Status ${response.status}`}</span>`;
        }
    } catch (err) {
        console.error('Fetch error on Get Flight by ID:', err);
        resultDiv.innerHTML = `<span style='color:red'>Error: ${err.message}</span>`;
    }
}

async function getFlightsWithUrls() {
    const resultDiv = document.getElementById('flights-urls-result');
    resultDiv.innerHTML = '';
    if (!checkToken(resultDiv)) return;

    const pageInput = document.getElementById('flights-page-input');
    let page = parseInt(pageInput.value) || 1;

    try {
        const response = await fetch(`${API_BASE_URL}/api/flight/Flights%20with%20IDs%20and%20URL?page=${page}&pageSize=100`, {
            headers: getAuthHeader()
        });

        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            const data = await response.json();
            if (response.ok) {
                let html = `<p>Page ${data.pageNumber} of ${data.totalPages} (Total Flights: ${data.totalItems})</p><ul>`;
                for (const flight of data.flights) {
                    html += `<li>Flight ID: ${flight.id} - <a href="${flight.url}" target="_blank">${flight.url}</a></li>`;
                }
                html += '</ul>';
                resultDiv.innerHTML = html;
            } else {
                console.error('Get Flights with URLs error response (JSON):', response.status, data);
                resultDiv.innerHTML = `<span style='color:red'>${data.message || data.details || `Error: Status ${response.status}`}</span>`;
            }
        } else {
            const text = await response.text();
            console.error('Get Flights with URLs error response (text):', response.status, text);
            resultDiv.innerHTML = `<span style='color:red'>${text || `Error: Status ${response.status}`}</span>`;
        }
    } catch (err) {
        console.error('Fetch error on Get Flights with URLs:', err);
        resultDiv.innerHTML = `<span style='color:red'>Error: ${err.message}</span>`;
    }
}

async function filterFlights() {
    const id = document.getElementById('filter-id').value.trim();
    const date = document.getElementById('filter-date').value.trim();
    const country = document.getElementById('filter-country').value.trim();
    const page = document.getElementById('filter-page').value.trim();
    const resultDiv = document.getElementById('filter-flights-result');
    resultDiv.innerHTML = '';
    if (!checkToken(resultDiv)) return;
    const params = new URLSearchParams();
    if (id) params.append('id', id);
    if (date) params.append('date', date);
    if (country) params.append('country', country);
    if (page) params.append('page', page);
    try {
        const response = await fetch(`${API_BASE_URL}/api/flight/filter?${params.toString()}`, {
            headers: getAuthHeader()
        });
        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            const data = await response.json();
            if (response.ok) {
                let html = `<div>Page: ${data.page || data.pageNumber} / ${data.totalPages} | Total: ${data.totalItems || data.totalTouchpointRecords}</div>`;
                html += '<ul>';
                for (const flight of data.data || data.flights || []) {
                    html += `<li>${renderFlightData(flight, true)}</li>`;
                }
                html += '</ul>';
                resultDiv.innerHTML = html;
            } else {
                console.error('Filter Flights error response (JSON):', response.status, data);
                resultDiv.innerHTML = `<span style='color:red'>${data.message || data.details || `Error: Status ${response.status}`}</span>`;
            }
        } else {
            const text = await response.text();
            console.error('Filter Flights error response (text):', response.status, text);
            resultDiv.innerHTML = `<span style='color:red'>${text || `Error: Status ${response.status}`}</span>`;
        }
    } catch (err) {
        console.error('Fetch error on Filter Flights:', err);
        resultDiv.innerHTML = `<span style='color:red'>Error: ${err.message}</span>`;
    }
}

// --- Touchpoints Section ---
async function getTouchpointsByPage() {
    const page = document.getElementById('touchpoint-page-input').value.trim();
    const resultDiv = document.getElementById('touchpoints-page-result');
    resultDiv.innerHTML = '';
    if (!checkToken(resultDiv)) return;
    if (!page) {
        resultDiv.innerHTML = '<span style="color:red">Please enter a page number.</span>';
        return;
    }
    try {
        const response = await fetch(`${API_BASE_URL}/api/touchpoint/page/${page}`, {
            headers: getAuthHeader()
        });
        const contentType = response.headers.get('content-type');
        if (response.status === 204) {
            resultDiv.innerHTML = '<span style="color:orange">No content for this page.</span>';
            return;
        }
        if (response.status === 302) {
            resultDiv.innerHTML = '<span style="color:orange">Redirected. Please try another page.</span>';
            return;
        }
        if (contentType && contentType.includes('application/json')) {
            const data = await response.json();
            if (response.ok && data.touchpoints) {
                resultDiv.innerHTML = renderTouchpointsPage(data);
            } else {
                console.error('Get Touchpoints by Page error response (JSON):', response.status, data);
                resultDiv.innerHTML = `<span style='color:red'>${data.message || data.details || `Error: Status ${response.status}`}</span>`;
            }
        } else {
            const text = await response.text();
            console.error('Get Touchpoints by Page error response (text):', response.status, text);
            resultDiv.innerHTML = `<span style='color:red'>${text || `Error: Status ${response.status}`}</span>`;
        }
    } catch (err) {
        console.error('Fetch error on Get Touchpoints by Page:', err);
        resultDiv.innerHTML = `<span style='color:red'>Error: ${err.message}</span>`;
    }
}

async function getTouchpointsByFlightId() {
    const id = document.getElementById('touchpoint-flightid-input').value.trim();
    const resultDiv = document.getElementById('touchpoints-flightid-result');
    resultDiv.innerHTML = '';
    if (!checkToken(resultDiv)) return;
    if (!id) {
        resultDiv.innerHTML = '<span style="color:red">Please enter a flight ID.</span>';
        return;
    }
    try {
        const response = await fetch(`${API_BASE_URL}/api/touchpoint/SearchByFlightID/${id}`, {
            headers: getAuthHeader()
        });
        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            const data = await response.json();
            if (response.ok && Array.isArray(data)) {
                resultDiv.innerHTML = renderTouchpointsList(data);
            } else {
                console.error('Get Touchpoints by Flight ID error response (JSON):', response.status, data);
                resultDiv.innerHTML = `<span style='color:red'>${data.message || data.details || `Error: Status ${response.status}`}</span>`;
            }
        } else {
            const text = await response.text();
            console.error('Get Touchpoints by Flight ID error response (text):', response.status, text);
            resultDiv.innerHTML = `<span style='color:red'>${text || `Error: Status ${response.status}`}</span>`;
        }
    } catch (err) {
        console.error('Fetch error on Get Touchpoints by Flight ID:', err);
        resultDiv.innerHTML = `<span style='color:red'>Error: ${err.message}</span>`;
    }
}

// --- Import Section ---
async function importExcel(event) {
    event.preventDefault();
    const flightsFile = document.getElementById('flights-file').files[0];
    const touchpointsFile = document.getElementById('touchpoints-file').files[0];
    const resultDiv = document.getElementById('import-result');
    resultDiv.innerHTML = '';
    if (!checkToken(resultDiv)) return;
    if (!flightsFile || !touchpointsFile) {
        resultDiv.innerHTML = '<span style="color:red">Please select both Excel files.</span>';
        return;
    }
    const formData = new FormData();
    formData.append('FlightsFile', flightsFile);
    formData.append('TouchpointsFile', touchpointsFile);
    try {
        const response = await fetch(`${API_BASE_URL}/api/excelimport/flights`, {
            method: 'POST',
            body: formData,
            headers: getAuthHeader()
        });
        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            const data = await response.json();
            if (response.ok) {
                resultDiv.innerHTML = `<span style='color:green'>${data.message || 'Import successful.'}</span>`;
            } else {
                console.error('Import Excel error response (JSON):', response.status, data);
                resultDiv.innerHTML = `<span style='color:red'>${data.message || data.details || `Error: Status ${response.status}`}</span>`;
            }
        } else {
            const text = await response.text();
            if (response.ok) {
                resultDiv.innerHTML = `<span style='color:green'>${text}</span>`;
            } else {
                console.error('Import Excel error response (text):', response.status, text);
                resultDiv.innerHTML = `<span style='color:red'>${text || `Error: Status ${response.status}`}</span>`;
            }
        }
    } catch (err) {
        console.error('Fetch error on Import Excel:', err);
        resultDiv.innerHTML = `<span style='color:red'>Error: ${err.message}</span>`;
    }
}

// --- Helpers ---
function getAuthHeader() {
    const token = localStorage.getItem('bearerToken');
    // Ensure token is trimmed before being sent in the header
    return token ? { 'Authorization': 'Bearer ' + token.trim() } : {};
}

function renderFlightData(flight, short = false) {
    if (!flight) return '';
    if (short) {
        return `<strong>ID:</strong> ${flight.id || flight.Id} | <strong>Flight Number:</strong> ${flight.flightNumber || flight.FlightNumber}`;
    }
    return `<div class="flight-data">
        <h2>Flight Details</h2>
        <p><strong>Flight Number:</strong> ${flight.flightNumber || flight.FlightNumber}</p>
        <p><strong>Airline:</strong> ${flight.airlineFullname || flight.AirlineFullname}</p>
        <p><strong>Airport:</strong> ${flight.airport || flight.Airport}</p>
        <p><strong>Scheduled Time (Local):</strong> ${flight.scheduledLocal ? new Date(flight.scheduledLocal).toLocaleString() : ''}</p>
        <p><strong>Gate:</strong> ${flight.gate || flight.Gate}</p>
        <p><strong>Total Passengers:</strong> ${flight.totalPax || flight.TotalPax}</p>
    </div>`;
}

function renderTouchpointsPage(pageManager) {
    if (!pageManager || !pageManager.touchpoints) return '';
    let html = `<div>Page: ${pageManager.pageNumber} / ${pageManager.totalPages} | Total: ${pageManager.totalTouchpointRecords}</div>`;
    html += '<ul>';
    for (const tp of pageManager.touchpoints) {
        html += `<li>${renderTouchpoint(tp)}</li>`;
    }
    html += '</ul>';
    return html;
}

function renderTouchpointsList(list) {
    if (!Array.isArray(list)) return '';
    let html = '<ul>';
    for (const tp of list) {
        html += `<li>${renderTouchpoint(tp)}</li>`;
    }
    html += '</ul>';
    return html;
}

function renderTouchpoint(tp) {
    return `<div><strong>ID:</strong> ${tp.id || tp.Id} | <strong>Flight ID:</strong> ${tp.flightId || tp.FlightId} | <strong>Type:</strong> ${tp.touchpointType || tp.TouchpointType} | <strong>Time:</strong> ${tp.touchpointTime ? new Date(tp.touchpointTime).toLocaleString() : ''} | <strong>Pax:</strong> ${tp.touchpointPax || tp.TouchpointPax}</div>`;
}

// On page load, update token status and show token if present
window.addEventListener('DOMContentLoaded', () => {
    updateTokenStatus();
    const token = localStorage.getItem('bearerToken');
    const tokenInput = document.getElementById('bearer-token');
    const tokenContainer = document.getElementById('token-container');
    if (token && tokenInput && tokenContainer) {
        tokenInput.value = token;
        tokenContainer.style.display = 'block';
    }
}); 