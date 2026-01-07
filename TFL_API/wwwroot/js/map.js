document.addEventListener("DOMContentLoaded", init);
const MODE_STYLES = {
    tube: { color: "#e74c3c", weight: 6 },   // red
    walking: { color: "#7f8c8d", weight: 4, dashArray: "6,6" },
    bus: { color: "#2980b9", weight: 5 },
    dlr: { color: "#1abc9c", weight: 5 },
    overground: { color: "#e67e22", weight: 5 },
    "national-rail": { color: "#2c3e50", weight: 5 },
    default: { color: "#555", weight: 5 }
}
const state = {
    map: null,
    lastItinerary: null,
    routeLayers: [],
    cardEl: null,
    markersByNaptan: new Map(),
    peakChart: null,
    quietChart: null,
    activeNaptan: null,
    bounds: L.latLngBounds([51.20, -0.65], [51.80, 0.45]),
    selectedFrom: null,
    selectedTo: null,
};

function init()
{
    state.cardEl = document.getElementById("stationCard");
    state.map = createMap();
    addBaseLayers(state.map);
    wireGlobalUI();

    loadStations();
}
/* ------------------- Map Setup ------------------ */

function createMap() {
    const map = L.map('map', {
        minZoom: 11,
        maxZoom: 14,
        maxBounds: state.bounds,
        maxBoundsViscosity: 0.5,
        zoomControl: false
    }).fitBounds(state.bounds);
    map.on('click', hideCard);
    return map;
}
function addBaseLayers(map) {
    L.tileLayer('https://{s}.basemaps.cartocdn.com/light_nolabels/{z}/{x}/{y}{r}.png', {
        maxZoom: 19,
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    L.tileLayer("https://{s}.tiles.openrailwaymap.org/standard/{z}/{x}/{y}.png", {
        attribution: "&copy; OpenRailwayMap contributors",
        opacity: 0.25,
    }).addTo(map);
}

/* ------------------- UI Logic ------------------ */

function wireGlobalUI() {
    const fromInput = document.getElementById("fromInput");
    const toInput = document.getElementById("toInput");
    const fromResults = document.getElementById("fromResults");
    const toResults = document.getElementById("toResults");

    fromInput.addEventListener("input", (e) => searchStations(e.target.value, fromResults, (s) => {
        state.selectedFrom = s;
        fromInput.value = s.name;
        fromResults.innerHTML = "";
    }));
    toInput.addEventListener("input", (e) => searchStations(e.target.value, toResults, (s) => {
        state.selectedTo = s;
        toInput.value = s.name;
        toResults.innerHTML = "";
    }));
}

/* ------------------- Markers ------------------ */
async function loadStations() {
    const res = await fetch(`/api/train/crowding/tube-stations`);
    if (!res.ok)
    {
        console.error("Failed to load stations");
        return;
    }
    const stations = await res.json();
    for (const station of stations) {
        if (!station.lat || !station.lon) continue;
        const naptan = station.naptan;
        const marker = L.circleMarker([station.lat, station.lon], {
            radius: 6,
            weight: 0.3,
            fillColor: "#6FAF8E",
            fillOpacity: 0.9,
            color: "#2c3e50",

        });
        state.markersByNaptan.set(naptan, marker);

        marker.addTo(state.map).bindTooltip(station.name, { direction: "top", opacity: 0.9 });
        marker.on("click", (e) => {
            e.originalEvent?.stopPropagation();
            onStationClick(station);
        });
    }


}

function clearRoute() {
    (state.routeLayers ?? []).forEach(l => state.map.removeLayer(l));
    state.routeLayers = [];
}

function drawJourneyOnMap(journey) {
    clearRoute();
    const legs = journey?.legs ?? [];
    const allLatLngs = [];

    for (const leg of legs) {
        const coords = parseLineString(leg?.path?.lineString);
        if (coords.length) {
            const modeId = leg?.mode?.id ?? "default";
            const style = MODE_STYLES[modeId] ?? MODE_STYLES.default;
            allLatLngs.push(...coords);
            const poly = L.polyline(coords, {
                ...style,
                opacity: 0.9
            }).addTo(state.map);

            poly.on("click", (e) => {
                e.originalEvent?.stopPropagation();
                clearRoute();
            });
            poly.bindTooltip(
                `${modeId.replace("-", " ")} — click to clear`,
                { sticky: true }
            );

            state.routeLayers.push(poly);
        }
    }
    if (allLatLngs.length) {
        const bounds = L.latLngBounds(allLatLngs);
        state.map.fitBounds(bounds, { padding: [30, 30] });
    }
}
/* ------------------- Station Interaction ------------------ */

async function onStationClick(station) {
    state.activeNaptan = station.naptan;
    state.map.setView([station.lat, station.lon], 14, { animate: true });
    const live = await fetch(`/api/train/crowding/live?naptan=${encodeURIComponent(station.naptan)}`).then(res => res.json());
    const pct = live?.percentageOfBaseline ?? null;
    const safePct100 = pct == null ? null : pct * 100;

    const marker = state.markersByNaptan.get(station.naptan);
    if (marker) marker.setStyle({ fillColor: crowdColor(safePct100) });

    showCard({
        name: station.name ?? station.Name,
        pct,
        timeLocal: live?.timeLocal ?? "",
        naptan: station.naptan,
    });
}

/* ------------------- Card Logic ------------------ */

function showCard({ name, pct, timeLocal, naptan }) {
    state.cardEl.innerHTML = renderCrowdingCard({ name, pct, timeLocal });

    state.cardEl.classList.add("open");

    document.getElementById("closeCardBtn")?.addEventListener("click", hideCard);

    const timeStart = document.getElementById("timeStart");
    const timeEnd = document.getElementById("timeEnd");

    if (!timeStart || !timeEnd) return;
    timeStart.addEventListener("change", onTimeChange);
    timeEnd.addEventListener("change", onTimeChange);

    renderPeakChart(naptan);
    renderQuietChart(naptan);

}
function showJourneyCard(station1, station2, itineraryResponse) {
    state.lastItinerary = itineraryResponse;
    state.cardEl.innerHTML = renderJourneyCard({ station1, station2, itineraryResponse });

    state.cardEl.classList.add("open");

    document.getElementById("closeCardBtn")?.addEventListener("click", () => {
        clearRoute();
        hideCard();
    });

    state.cardEl.querySelectorAll(".journeySelectBtn").forEach(btn => {
        btn.addEventListener("click", (e) => {
            const idx = Number(e.currentTarget.dataset.journeyIndex);
            const journeys = state.lastItinerary?.journeys ?? [];
            const journey = journeys[idx];
            drawJourneyOnMap(journey);

            state.cardEl.innerHTML = renderJourneyDetailsCard({ station1, station2, journey });

            document.getElementById("closeCardBtn")?.addEventListener("click", () => {
                clearRoute();
                hideCard();
            });

            document.getElementById("backToJourneysBtn")?.addEventListener("click", () => {
                showJourneyCard(station1, station2, state.lastItinerary);
            });

            document.getElementById("clearRouteBtn")?.addEventListener("click", clearRoute);
        });
    });

}

function hideCard() {
    state.cardEl.classList.remove("open");
    setTimeout(() => {
        if (!state.cardEl.classList.contains("open")) {
            state.cardEl.innerHTML = "";
        }
    }, 250);
}

/* ------------------- Rendering Helpers ------------------ */

function crowdLabel(pct100) {
    if (pct100 == null) return "Unknown";
    if (pct100 < 10) return "Very Quiet";
    if (pct100 < 40) return "Quiet";
    if (pct100 < 80) return "Busy";
    return "Very Busy";
}

function crowdColor(pct100) {
    if (pct100 == null) return "#9e9e9e";
    if (pct100 < 10) return "#FFFFFF";
    if (pct100 < 40) return "#FFA500";
    if (pct100 < 80) return "#FF2400";
    return "#f44336";
}

function parseLineString(lineString) {
    if (!lineString) return [];
    try {
        return JSON.parse(lineString);
    } catch {
        return [];
    }
}

function renderJourneyCard({ station1, station2, itineraryResponse }) {
    const journeys = itineraryResponse?.journeys ?? [];

    const items = journeys.slice(0, 5).map((j, idx) => {
        const depart = j.startDateTime ? new Date(j.startDateTime).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }) : "-";
        const arrive = j.arrivalDateTime ? new Date(j.arrivalDateTime).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }) : "-";
        const duration = j.duration ?? "-";

        const firstLeg = j.legs?.[0];
        const summary = firstLeg?.instruction?.summary ?? "Journey";
        const detailed = firstLeg?.instruction?.detailed ?? "";

        return `
      <li style="margin:8px 0;">
        <button class="journeySelectBtn" data-journey-index="${idx}" style="width:100%; text-align:left; padding:10px; border:1px solid #eee; border-radius:12px; background:#fff; cursor:pointer;">
          <div style="font-weight:700;">${depart} → ${arrive} (${duration} min)</div>
          <div style="font-size:12px; color:#555;">${summary}${detailed ? ` — ${detailed}` : ""}</div>
        </button>
      </li>
    `;
    }).join("");

    return `
    <div style="display:flex; justify-content:space-between; align-items:center;">
      <div>
        <div style="font-size:18px; font-weight:700;">${station1} to ${station2}</div>
        <div style="font-size:12px; color:#555;">Found ${journeys.length} journey option(s)</div>
      </div>
      <button id="closeCardBtn" style="border:none; background:transparent; font-size:18px; cursor:pointer;">✕</button>
    </div>

    <ul style="list-style:none; padding:0; margin-top:12px;">
      ${journeys.length ? items : `<li style="color:#b00;">No journeys returned (maybe disambiguation or bad IDs).</li>`}
    </ul>
  `;
}

function renderJourneyDetailsCard({ station1, station2, journey }) {
    const depart = journey?.startDateTime
        ? new Date(journey.startDateTime).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })
        : "-";

    const arrive = journey?.arrivalDateTime
        ? new Date(journey.arrivalDateTime).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })
        : "-";
    const duration = journey?.duration ?? "-";

    const farePence = journey?.fare?.totalCost;
    const fareText = (typeof farePence === "number")
        ? `£${(farePence / 100).toFixed(2)}`
        : "Unknown"
    const legs = journey?.legs ?? [];

    const legItems = legs.map((leg, idx) => {
        const mode = leg?.mode?.id ?? "unknown";
        const summary = leg?.instruction?.summary ?? "Leg";
        const detailed = leg?.instruction?.detailed ?? "";
        const legDur = leg?.duration ?? "-";

        const fromName = leg?.departurePoint?.commonName ?? leg?.departurePoint?.name ?? "";
        const toName = leg?.arrivalPoint?.commonName ?? leg?.arrivalPoint?.name ?? "";

        return `
      <li style="padding:10px; border:1px solid #eee; border-radius:12px; margin:8px 0;">
        <div style="display:flex; justify-content:space-between; gap:12px;">
          <div style="font-weight:700;">${idx + 1}. ${summary}</div>
          <div style="font-size:12px; color:#555;">${mode} • ${legDur} min</div>
        </div>
        ${detailed ? `<div style="font-size:12px; color:#555; margin-top:4px;">${detailed}</div>` : ""}
        ${(fromName || toName) ? `<div style="font-size:12px; margin-top:6px;"><b>${fromName}</b> → <b>${toName}</b></div>` : ""}
      </li>
    `;
    }).join("");

    return `
    <div style="display:flex; justify-content:space-between; align-items:center;">
      <div>
        <div style="font-size:18px; font-weight:700;">${station1} to ${station2}</div>
        <div style="font-size:12px; color:#555;">${depart} → ${arrive} • ${duration} min • Fare: ${fareText}</div>
      </div>
      <button id="closeCardBtn" style="border:none; background:transparent; font-size:18px; cursor:pointer;">✕</button>
    </div>

    <div style="display:flex; gap:8px; margin-top:10px;">
      <button id="backToJourneysBtn" style="padding:8px 10px; border:1px solid #eee; border-radius:10px; background:#fff; cursor:pointer;">
        ← Back
      </button>
      <button id="clearRouteBtn" style="padding:8px 10px; border:1px solid #eee; border-radius:10px; background:#fff; cursor:pointer;">
        Clear route
      </button>
    </div>

    <ul style="list-style:none; padding:0; margin-top:12px;">
      ${legItems || `<li>No legs available.</li>`}
    </ul>
  `;
}

function renderCrowdingCard({ name, pct, timeLocal }) {
    const pct100 = pct == null ? null : pct * 100;
    const safePct = pct100 == null ? 0 : Math.min(100, Math.max(0, pct100));
    const label = crowdLabel(pct100);

    const barColor =
        pct100 == null ? "#ccc" :
        pct100 < 10 ? "#D3D3D3" :
        pct100 < 40 ? "#FFA500" :
        pct100 < 80 ? "#FF2400" : "#f44336";
    
    return `
      <div style="display:flex; justify-content:space-between; align-items:center;">
        <div>
          <div style="font-size:18px; font-weight:700;">${name}</div>
        </div>
        <button id="closeCardBtn" style="border:none; background:transparent; font-size:18px; cursor:pointer;">✕</button>
      </div>

      <div style="margin-top:12px; padding:12px; border:1px solid #eee; border-radius:12px;">
        <div style="display:flex; align-items:center; gap:8px;">
          <div style="font-size:16px; font-weight:700;">${label}</div>
        </div>
           <div style="margin-top:10px; height:10px; background:#f1f1f1; border-radius:999px; overflow:hidden;">
          <div style="
            height:100%;
            width:${safePct}%;
            background:${barColor};
          "></div>
        </div>

        <div style="margin-top:10px; font-size:12px; color:#555; display:grid; gap:4px;">
          <div><b>Updated:</b> ${timeLocal ?? ""}</div>
        </div>

        
      </div>
      <canvas id="weeklyPeakChart" height="160"></canvas>
      <label for="timeStart">Select a start time (06:00–00:00)</label>
      <input id="timeStart" type="time" min="06:00" max="23:59"value="08:00">

      <label for="timeEnd">Select a end time (06:00–00:00)</label>
      <input id="timeEnd" type="time" min="06:00" max="23:59" value="20:00">
      <canvas id="weeklyQuietChart" height="160"></canvas>
    `;
}

/* ------------------- Charts ------------------ */



async function onTimeChange() {
    if (!state.activeNaptan) return;
    const start = document.getElementById("timeStart").value;
    const end = document.getElementById("timeEnd").value;
    if (end < start) return;
    await renderQuietChart(state.activeNaptan, start, end);
}

async function renderPeakChart(naptan) {
    const data = await fetch(`/api/train/crowding/peak/${encodeURIComponent(naptan)}`).then(res => res.json());

    const labels = data.map(d => d.day);
    const values = data.map(d => d.peakPercentage ?? 0);

    const canvas = document.getElementById("weeklyPeakChart");
    if (!canvas) return;

    state.peakChart?.destroy();
    state.peakChart = new Chart(document.getElementById("weeklyPeakChart"), {
        type: "bar",
        data: {
            labels,
            datasets: [{
                label: "Peak crowding (%)",
                data: values,
                backgroundColor: "#6FAF8E"
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    max: 100
                }
            },
            plugins: {
                tooltip: {
                    callbacks: {
                        label: (ctx) => {
                            const item = data[ctx.dataIndex];
                            return `${ctx.raw}% @ ${item.peakTimeBand ?? "Unknown"}`;
                        }
                    }
                }
            }
        }
    });

}
async function renderQuietChart(naptan, start = "08:00", end = "20:00") {
    const url = `/api/train/crowding/quiet/${encodeURIComponent(naptan)}?start=${encodeURIComponent(start)}&end=${encodeURIComponent(end)}`;
    const data = await fetch(url).then(r => r.json());

    const labels = data.map(d => d.day);
    const values = data.map(d => d.peakPercentage ?? 0);

    const canvas = document.getElementById("weeklyQuietChart");
    if (!canvas) return;

    state.quietChart?.destroy();
    state.quietChart = new Chart(document.getElementById("weeklyQuietChart"), {
        type: "bar",
        data: {
            labels,
            datasets: [{
                label: "Quiet crowding (%)",
                data: values,
                backgroundColor: "#6FAF8E"
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    max: 100
                }
            },
            plugins: {
                tooltip: {
                    callbacks: {
                        label: (ctx) => {
                            const item = data[ctx.dataIndex];
                            return `${ctx.raw}% @ ${item.peakTimeBand ?? "Unknown"}`;
                        }
                    }
                }
            }
        }
    });

}

/* ------------------- Search ------------------ */

async function searchJourneys() {
    if (!state.selectedFrom || !state.selectedTo) {
        console.error("Pick both from and To stations first");
        return;
    }

    const from = state.selectedFrom.naptan;
    const to = state.selectedTo.naptan;

    const response = await fetch(`/api/train/crowding/journeys?from=${encodeURIComponent(from)}&to=${encodeURIComponent(to)}`);
    if (!response.ok) {
        console.error("Failed to test station");
        return;
    }
    const data = await response.json();
    showJourneyCard(state.selectedFrom.name, state.selectedTo.name, data);

    
}

async function searchStations(query, resultsEl, onPick){
    query = query.trim();
    if (query.length < 2) {
        resultsEl.innerHTML = "";
        return;
    }
    const stations = await fetch(`/api/train/crowding/search?query=${encodeURIComponent(query)}`).then(res => res.json());
    resultsEl.innerHTML = "";
    (stations ?? []).slice(0, 8).forEach((s) => {
        const li = document.createElement("li");
        li.textContent = s.name;
        li.onclick = () => {
            onPick?.(s);
            onStationClick(s);
            resultsEl.innerHTML = "";
        };
        resultsEl.appendChild(li);
    });
}
