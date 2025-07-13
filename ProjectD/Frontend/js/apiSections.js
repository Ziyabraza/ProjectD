// Define your API base URL here. Change this to your deployed API URL when going live.
const API_BASE_URL = 'https://projectje-d6d4arfxb6anhrcj.westeurope-01.azurewebsites.net';

// --- Auth Section ---
// Moved to login.js:
// function updateTokenStatus() { ... }
// async function loginUser() { ... }
// function copyToken() { ... }
// function setManualToken() { ... }

function getAuthHeader() {
    const token = sessionStorage.getItem('bearerToken');
    return token ? { 'Authorization': 'Bearer ' + token.trim() } : {};
}

function checkToken(resultDiv) {
    const token = sessionStorage.getItem('bearerToken');
    if (!token) {
        if (resultDiv) {
            resultDiv.innerHTML = `<span style="color:red">You must be logged in. <a href="login.html">Go to login page</a>.</span>`;
        }
        return false;
    }
    return true;
}

function updateTokenStatus() {
    const token = sessionStorage.getItem('bearerToken');
    const statusDiv = document.getElementById('token-status');
    if (statusDiv) {
        if (token) {
            statusDiv.innerHTML = '<span style="color:green">Logged in with active bearer token.</span>';
        } else {
            statusDiv.innerHTML = '<span style="color:red">You are not logged in. <a href="login.html">Go to login page</a>.</span>';
        }
    }
}

function logout() {
    sessionStorage.removeItem('bearerToken');
    window.location.href = 'login.html';
}

window.addEventListener('DOMContentLoaded', () => {
    // This page (index.html) is only loaded if a token exists due to login.js redirection.
    // So we just need to update the status here.
    updateTokenStatus();
});

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
    const token = sessionStorage.getItem('bearerToken');
    return token ? { 'Authorization': 'Bearer ' + token.trim() } : {};
}


function renderFlightData(flight, short = false) {
    if (!flight) return '';

    // List of properties to display from the Flight model
    const properties = [
        { key: 'type', label: 'Type' },
        { key: 'id', label: 'ID' },
        { key: 'timetableId', label: 'Timetable ID' },
        { key: 'trafficType', label: 'Traffic Type' },
        { key: 'flightNumber', label: 'Flight Number' },
        { key: 'diverted', label: 'Diverted', type: 'boolean' },
        { key: 'nachtvlucht', label: 'Night Flight', type: 'boolean' },
        { key: 'flightCode', label: 'Flight Code' },
        { key: 'flightCodeDescription', label: 'Flight Code Description' },
        { key: 'flightCodeIATA', label: 'Flight Code IATA' },
        { key: 'publicAnnouncement', label: 'Public Announcement', type: 'boolean' },
        { key: 'scheduledUTC', label: 'Scheduled UTC', type: 'datetime' },
        { key: 'actualUTC', label: 'Actual UTC', type: 'datetime' },
        { key: 'scheduledLocal', label: 'Scheduled Local', type: 'datetime' },
        { key: 'actualLocal', label: 'Actual Local', type: 'datetime' },
        { key: 'bewegingen', label: 'Movement Type' },
        { key: 'parkeerpositie', label: 'Parking Position' },
        { key: 'bus', label: 'Bus Required', type: 'boolean' },
        { key: 'gate', label: 'Gate' },
        { key: 'bagageband', label: 'Baggage Belt' },
        { key: 'airportICAO', label: 'Airport ICAO' },
        { key: 'airport', label: 'Airport Name' },
        { key: 'country', label: 'Country' },
        { key: 'viaAirportICAO', label: 'Via Airport ICAO (Stopover)' },
        { key: 'viaAirport', label: 'Via Airport Name (Stopover)' },
        { key: 'aircraftRegistration', label: 'Aircraft Registration' },
        { key: 'seats', label: 'Seats' },
        { key: 'mtow', label: 'MTOW' },
        { key: 'aircraftType', label: 'Aircraft Type' },
        { key: 'aircraftDescription', label: 'Aircraft Description' },
        { key: 'eu', label: 'EU Flight', type: 'boolean' },
        { key: 'schengen', label: 'Schengen Flight', type: 'boolean' },
        { key: 'airlineFullname', label: 'Airline Fullname' },
        { key: 'airlineShortname', label: 'Airline Shortname' },
        { key: 'airlineICAO', label: 'Airline ICAO' },
        { key: 'airlineIATA', label: 'Airline IATA' },
        { key: 'debiteur', label: 'Debtor' },
        { key: 'debiteurNr', label: 'Debtor Number' },
        { key: 'paxMale', label: 'Passengers Male' },
        { key: 'paxFemale', label: 'Passengers Female' },
        { key: 'paxChild', label: 'Passengers Child' },
        { key: 'paxInfant', label: 'Passengers Infant' },
        { key: 'paxTransitMale', label: 'Transit Passengers Male' },
        { key: 'paxTransitFemale', label: 'Transit Passengers Female' },
        { key: 'paxTransitChild', label: 'Transit Passengers Child' },
        { key: 'paxTransitInfant', label: 'Transit Passengers Infant' },
        { key: 'crewCabin', label: 'Cabin Crew' },
        { key: 'crewCockpit', label: 'Cockpit Crew' },
        { key: 'bagsWeight', label: 'Bags Weight (kg)' },
        { key: 'bagsTransitWeight', label: 'Transit Bags Weight (kg)' },
        { key: 'bags', label: 'Total Bags' },
        { key: 'bagsTransit', label: 'Transit Bags' },
        { key: 'afhandelaar', label: 'Handler' },
        { key: 'forecastPercentage', label: 'Forecast Percentage' },
        { key: 'forecastPax', label: 'Forecast Passengers' },
        { key: 'forecastBabys', label: 'Forecast Infants' },
        { key: 'flightClass', label: 'Flight Class' },
        { key: 'datasource', label: 'Data Source' },
        { key: 'totalPax', label: 'Total Passengers' },
        { key: 'terminalPax', label: 'Terminal Passengers' },
        { key: 'totalPaxBetalend', label: 'Total Paying Passengers' },
        { key: 'terminalPaxBetalend', label: 'Terminal Paying Passengers' },
        { key: 'transitPax', label: 'Transit Passengers' },
        { key: 'transitPaxBetalend', label: 'Transit Paying Passengers' },
        { key: 'totalCrew', label: 'Total Crew' },
        { key: 'terminalCrew', label: 'Terminal Crew' },
        { key: 'totalSeats', label: 'Total Seats' },
        { key: 'terminalSeats', label: 'Terminal Seats' },
        { key: 'totalBags', label: 'Total Bags' },
        { key: 'terminalBags', label: 'Terminal Bags' },
        { key: 'transitBags', label: 'Transit Bags' },
        { key: 'totalBagsWeight', label: 'Total Bags Weight' },
        { key: 'terminalBagsWeight', label: 'Terminal Bags Weight' },
        { key: 'transitBagsWeight', label: 'Transit Bags Weight' },
        { key: 'runway', label: 'Runway' },
        { key: 'longitude', label: 'Longitude' },
        { key: 'elevation', label: 'Elevation' },
        { key: 'latitude', label: 'Latitude' },
        { key: 'distanceKilometers', label: 'Distance (km)' },
        { key: 'direction', label: 'Direction' },
        { key: 'airportIATA', label: 'Airport IATA' },
        { key: 'parked', label: 'Parked', type: 'boolean' },
        { key: 'seizoen', label: 'Season' }
    ];

    // Helper to get property value, checking both camelCase and PascalCase
    const getPropValue = (obj, key) => {
        const pascalCaseKey = key.charAt(0).toUpperCase() + key.slice(1);
        return obj[key] !== undefined ? obj[key] : obj[pascalCaseKey];
    };

    if (short) {
        // Original short display for filter results
        return `<strong>ID:</strong> ${getPropValue(flight, 'id')} | <strong>Flight Number:</strong> ${getPropValue(flight, 'flightNumber')}`;
    }

    let html = '<div class="flight-data">';
    html += '<h2>Flight Details</h2>';

    for (const prop of properties) {
        const value = getPropValue(flight, prop.key);
        let displayValue = 'N/A';

        if (value !== null && value !== undefined) {
            if (prop.type === 'datetime') {
                try {
                    displayValue = new Date(value).toLocaleString();
                } catch (e) {
                    displayValue = value; // Fallback if date parsing fails
                }
            } else if (prop.type === 'boolean') {
                displayValue = value ? 'Yes' : 'No';
            } else {
                displayValue = value;
            }
        }
        html += `<p><strong>${prop.label}:</strong> ${displayValue}</p>`;
    }

    html += '</div>';
    return html;
}

function renderTouchpointsPage(pageManager) {
    if (!pageManager || !pageManager.touchpoints) return '';
    console.log('Touchpoints response:', pageManager);
    const total = pageManager.touchpoints.length;
    let html = `<div>Page: ${pageManager.pageNumber} / ${pageManager.totalPages} | Total on this page: ${total}</div>`;
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
    const token = sessionStorage.getItem('bearerToken');
    if (!token) {
        window.location.href = 'login.html'; // forceer login als token ontbreekt
    } else {
        updateTokenStatus();
    }
});