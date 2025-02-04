document.addEventListener('DOMContentLoaded', async () => {
    const form = document.getElementById('create-form');
    const cancelButton = document.getElementById('cancel-button');

    //Setting up map
    let map = L.map('map').setView([60.39, 5.52], 13);
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);
    let popup = L.popup();

    function handleMapClick(e)
    {
        popup.setLatLng(e.latlng).setContent("Selected location").openOn(map);
        map.setView(e.latlng);
    }

    map.addEventListener("click", handleMapClick)


    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        const center = map.getCenter();
        const newEvent = {
            eventName: document.getElementById('eventName').value,
            date: document.getElementById('date').value,
            public: document.getElementById('public').checked,
            genre: document.getElementById('genre').value,
            maxAttendees: parseInt(document.getElementById('maxAttendees').value),
            latLong: [center.lat, center.lng]
        };

        try {
            const response = await fetch('/event', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify(newEvent)
            });

            if (!response.ok) {
                throw new Error('Failed to create event');
            }

            alert('Event created successfully!');
            window.location.href = '/';
        } catch (error) {
            console.error('Error creating event:', error);
            alert('Failed to create event. Please try again.');
        }
    });
    
    cancelButton.addEventListener('click', () => {
        window.location.href = '/';
    });
});