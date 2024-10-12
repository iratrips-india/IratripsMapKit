// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function showPrompt(message) {
  return prompt(message, 'Type anything here');
}

export function initMap(lat, lng, zoom) {

  var map = L.map('map').setView([lng, lat], zoom);

  L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
  }).addTo(map);

  window.mapkit = {
    map: map
  };

}

export function AddMarkers() {
  
  var marker = L.marker([51.5, -0.09]).addTo(map);
}


