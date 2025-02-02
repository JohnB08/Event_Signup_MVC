document.addEventListener('DOMContentLoaded', async () => {
    const authLink = document.getElementById('auth-link');
    const eventsList = document.getElementById('events-list');

    const user = await checkAuthentication();
    if (user) {
        authLink.innerHTML = `<a href="/Login/SignOut">Sign Out (${user.username})</a>`;
        const createEventLink = document.createElement('a');
        createEventLink.href = '/event/create';
        createEventLink.textContent = 'Create Event';
        authLink.appendChild(createEventLink);
    } else {
        authLink.innerHTML = `<a href="/Login">Login</a>`;
    }

    const events = await fetchEvents();
    events.forEach((event) => {
        const eventElement = document.createElement('div');
        eventElement.classList.add('event-card');

        eventElement.innerHTML = `
            <h2>${event.eventName}</h2>
            <p><strong>Location:</strong> ${event.location}</p>
            <p><strong>Date & Time:</strong> ${event.date}</p>
            <p><strong>Public:</strong> ${event.public ? 'Yes' : 'No'}</p>
            <p><strong>Genre:</strong> ${event.genre}</p>
        `;

        if (event.editable) {
            const editButton = document.createElement('button');
            editButton.textContent = 'Edit';
            editButton.onclick = () => {
                window.location.href = `/Event/Edit/${event.id}`;
            };
            eventElement.appendChild(editButton);
        }

        eventsList.appendChild(eventElement);
    });
});

async function checkAuthentication() {
    try {
        const response = await fetch('/Login/IsAuthenticated', {
            method: 'GET',
            credentials: 'include'
        });
        if (response.ok) {
            return await response.json();
        }
    } catch (error) {
        console.error('Error checking authentication status:', error);
    }
    return null;
}

async function fetchEvents() {
    try {
        const response = await fetch('/event', {
            method: 'GET',
            credentials: 'include' 
        });
        if (response.ok) {
            var result = await response.json();
            console.log(result);
            return result; 
        }
    } catch (error) {
        console.error('Error fetching events:', error);
        alert('Failed to fetch events. Please try again later.');
    }
    return [];
}