document.addEventListener("DOMContentLoaded", init);

const state = {
    map: null,
    cardEl: null,
    markersByNaptan: new Map(),
    peakChart: null,
    quietChart: null,
    bounds: L.latLngBounds([51.20, -0.65], [51.80, 0.45])
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
    const searchInput = document.getElementById("searchInput");
    searchInput.addEventListener("input", (e) => searchStations(e.target.value));
}

/* ------------------- markers ------------------ */
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

/* ------------------- Station Interaction ------------------ */

async function onStationClick(station) {
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
    state.cardEl.style.display = "block";
    state.cardEl.innerHTML = renderCrowdingCard({ name, pct, timeLocal });

    document.getElementById("closeCardBtn")?.addEventListener("click", hideCard);

    renderPeakChart(naptan);
    renderQuietChart(naptan);

}

function hideCard(){
    state.cardEl.style.display = "none";
    state.cardEl.innerHTML = "";
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
      <canvas id="weeklyQuietChart" height="160"></canvas>
    `;
}

/* ------------------- Charts ------------------ */

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
async function renderQuietChart(naptan) {
    const data = await fetch(`/api/train/crowding/quiet/${encodeURIComponent(naptan)}`).then(res => res.json());

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

async function searchStations(query){
    const resultsEl = document.getElementById("searchResults");
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
            onStationClick(s);
            resultsEl.innerHTML = "";
        };
        resultsEl.appendChild(li);
    });
}
