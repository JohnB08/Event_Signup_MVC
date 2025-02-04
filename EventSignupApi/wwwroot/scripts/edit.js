document.addEventListener('DOMContentLoaded', async () => {
    const form = document.getElementById('edit-form');
    const cancelButton = document.getElementById('cancel-button');

    // Extract event ID from URL
    const eventId = window.location.pathname.split("/").pop();
    console.log(eventId);

    async function fetchAndPopulate()
    {
        // Fetch event details
        try {
            const response = await fetch(`/event/${eventId}`, {
                method: 'GET',
                credentials: 'include' // Send session cookie
            });

            if (!response.ok) {
                throw new Error('Failed to fetch event details');
            }

            const event = await response.json();
            console.log(event);
            // Populate the form fields
            document.getElementById('eventName').value = event.eventName;
            document.getElementById('date').value = new Date(event.date).toISOString().slice(0, 16);
            document.getElementById('public').checked = event.public;
            document.getElementById('genre').value = event.genre;
            document.getElementById('maxAttendees').value = event.maxAttendees;
            return event;
        } catch (error) {
            console.error('Error loading event:', error);
            alert('Failed to load event details. Please try again.');
        }

    }
    var events = await fetchAndPopulate();
    //Setting up map
    let map = L.map('map').setView(events.latLong, 14);
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

    // Handle form submission
    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        const center = map.getCenter();
        const updatedEvent = {
            eventName: document.getElementById('eventName').value,
            date: document.getElementById('date').value,
            public: document.getElementById('public').checked,
            genre: document.getElementById('genre').value,
            maxAttendees: parseInt(document.getElementById('maxAttendees').value),
            latLong: [center.lat, center.lng]
        };

        try {
            const response = await fetch(`/event/edit/${eventId}`, {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include', 
                body: JSON.stringify(updatedEvent)
            });

            if (!response.ok) {
                throw new Error('Failed to update event');
            }

            alert('Event updated successfully!');
            window.location.href = '/';
        } catch (error) {
            console.error('Error updating event:', error);
            alert('Failed to update event. Please try again.');
        }
    });

    // Cancel button redirects to home
    cancelButton.addEventListener('click', () => {
        window.location.href = '/';
    });
});