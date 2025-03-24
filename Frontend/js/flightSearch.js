async function searchFlight() {
    const flightNumber = parseInt(document.getElementById('flightSearchInput').value.trim(), 10);

    console.log("[DEBUG] Ingevoerde FlightNumber (als int):", flightNumber);  // Debuglog

    if (isNaN(flightNumber)) {
        alert("Please enter a valid flight number.");
        console.log("[DEBUG] Ongeldige flightNumber ingevoerd.");  // Log voor foutieve input
        return;
    }

    try {
        const response = await fetch(`http://localhost:5000/api/Flight/${flightNumber}`);
        console.log("[DEBUG] Response status:", response.status);  // Log de HTTP-status

        if (response.ok) {
            const flightData = await response.json();
            console.log("[DEBUG] Flight data ontvangen:", flightData);  // Log de vluchtdata
            displayFlightData(flightData);
        } else {
            const errorData = await response.json();
            console.log("[DEBUG] Backend-foutmelding ontvangen:", errorData);
            document.getElementById('flightResult').innerHTML = `
                <p style="color: red;">Flight not found. Please check the flight number.</p>
            `;
        }
    } catch (error) {
        console.error("[DEBUG] Error tijdens het ophalen van flight data:", error);
        alert("An error occurred while fetching flight data.");
    }
}

function displayFlightData(flightData) {
    console.log("[DEBUG] Toon flight data in UI:", flightData);  // Debuglog

    const resultDiv = document.getElementById('flightResult');

    resultDiv.innerHTML = `
        <div class="flight-data">
            <h2>Flight Details</h2>
            <p><strong>Flight Number:</strong> ${flightData.flightNumber}</p>
            <p><strong>Airline:</strong> ${flightData.airlineFullname}</p>
            <p><strong>Airport:</strong> ${flightData.airport}</p>
            <p><strong>Scheduled Time (Local):</strong> ${new Date(flightData.scheduledLocal).toLocaleString()}</p>
            <p><strong>Gate:</strong> ${flightData.gate}</p>
            <p><strong>Total Passengers:</strong> ${flightData.totalPax}</p>
        </div>
    `;
}