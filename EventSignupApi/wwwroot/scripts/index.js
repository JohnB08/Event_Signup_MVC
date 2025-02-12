document.addEventListener('DOMContentLoaded', async () => {
    const authLink = document.getElementById('auth-link');
    const eventsList = document.getElementById('events-list');

    const user = await checkAuthentication();
    if (user) {
        const signoutButton = document.createElement("button");
        signoutButton.id = "logout-button";
        signoutButton.textContent = "Sign out!";
        signoutButton.addEventListener('click', async () => {
            await fetch('/Login/SignOut', {
                method: 'POST',
                credentials: 'include'
            });
            window.location.reload();
        })
        authLink.appendChild(signoutButton);
        const createEventLink = document.createElement('a');
        createEventLink.href = '/event/create';
        createEventLink.textContent = 'Create Event';
        authLink.appendChild(createEventLink);
    } else {
        authLink.innerHTML = `<a href="/Login">Login</a>`;
    }

    const events = await fetchEvents();
    events.forEach((event) => {
        const osmCoordinates = getOpenStreetMapCoordinates(event.latLong[0], event.latLong[1], 14);
        const eventElement = document.createElement('div');
        eventElement.classList.add('event-card');
        eventElement.innerHTML = `
        <div class="content">
            <div class="textContent">
                <h2>${event.eventName}</h2>
                <p><strong>Date & Time:</strong> ${event.date}</p>
                <p><strong>Public:</strong> ${event.public ? 'Yes' : 'No'}</p>
                <p><strong>Genre:</strong> ${event.genre}</p>
            </div>
            <img src='https://tile.openstreetmap.org/${osmCoordinates.zoom}/${osmCoordinates.x}/${osmCoordinates.y}.png'
        </div>
        `;

        if (event.canEdit) {
            const editButton = document.createElement('button');
            editButton.textContent = 'Edit';
            editButton.onclick = () => {
                window.location.href = `/Event/Edit/${event.id}`;
            };
            eventElement.appendChild(editButton);
            const deleteButton = document.createElement('button');
            deleteButton.textContent = 'Delete';
            deleteButton.onclick = async() => 
            {
                const response = await fetch(`event/${event.id}`, {
                    method: "DELETE",
                    credentials: "include"
                })
                if (response.ok) window.location.reload();
            }
            eventElement.appendChild(deleteButton);
        }
        if (user)
        {
            const subButton = document.createElement('button');
            if (event.isSubscriber)
            {
                subButton.textContent = 'UnSubscribe';
                subButton.onclick = async() => {
                    const response = await fetch(`event/unsubscribe/${event.id}`, {
                        method: 'POST',
                        credentials: 'include'
                    })
                    if (response.ok) window.location.reload();
                }
                
            }
            else
            {
                subButton.textContent = 'Subscribe';
                subButton.onclick = async() => {
                    const response = await fetch(`event/subscribe/${event.id}`, {
                        method: 'POST',
                        credentials: 'include'
                    })
                    if (response.ok) window.location.reload();
                }
            }
            eventElement.appendChild(subButton);
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

function getOpenStreetMapCoordinates(lat, lon, zoom)
{
    const n = Math.pow(2, zoom);
    const x = Math.floor((lon + 180) / 360 * n);
    const y = Math.floor((1 - Math.log(Math.tan(lat * Math.PI / 180) + 1 / Math.cos(lat * Math.PI / 180)) / Math.PI) / 2 * n);
    return {x, y, zoom};
}