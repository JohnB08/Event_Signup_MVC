document.addEventListener('DOMContentLoaded', async () => {
    const form = document.getElementById('create-form');
    const cancelButton = document.getElementById('cancel-button');

    form.addEventListener('submit', async (event) => {
        event.preventDefault();

        const newEvent = {
            eventName: document.getElementById('eventName').value,
            date: document.getElementById('date').value,
            public: document.getElementById('public').checked,
            genre: document.getElementById('genre').value,
            maxAttendees: parseInt(document.getElementById('maxAttendees').value)
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