let currentEventId = null;
const API_BASE_URL = (window.ApiConfig?.getEndpoint?.('EVENTS')) || 'http://localhost:5233/api/event';

document.addEventListener('DOMContentLoaded', () => {
    loadEvents();
    loadEventTypes();
});

document.getElementById('searchInput').addEventListener('input', debounce(loadEvents, 300));
document.getElementById('eventTypeFilter').addEventListener('change', loadEvents);

async function loadEvents() {
    try {
        const search = document.getElementById('searchInput').value.trim();
        const eventType = document.getElementById('eventTypeFilter').value;

        const params = new URLSearchParams({
            pageNumber: 1,
            pageSize: 50,
            ...(eventType && { eventType })
        });

        const response = await fetch(`${API_BASE_URL}?${params}`);
        if (!response.ok) throw new Error(`Failed to load events: ${response.status} ${response.statusText}`);

        const result = await response.json();
        let events = result.data || [];

        // Filter by search term
        if (search) {
            events = events.filter(e =>
                e.name.toLowerCase().includes(search.toLowerCase()) ||
                e.location.toLowerCase().includes(search.toLowerCase())
            );
        }

        displayEvents(events);
    } catch (error) {
        console.error('Error loading events:', error);
        showMessage('Failed to load events. Please try again.', 'error');
    }
}

function displayEvents(events) {
    const grid = document.getElementById('eventsGrid');
    const noEventsDiv = document.getElementById('noEvents');

    if (events.length === 0) {
        grid.innerHTML = '';
        noEventsDiv.style.display = 'block';
        return;
    }

    noEventsDiv.style.display = 'none';

    grid.innerHTML = events.map(event => {
        const eventDate = new Date(event.eventDate);
        const capacityPercent = (event.currentRegistrations / event.capacity) * 100;
        const isFull = event.currentRegistrations >= event.capacity;
        // Ensure imageUrl is properly resolved
        const imageUrl = event.cardImage || event.imageUrl || 'images/placeholder-event.jpg';

        return `
            <div class="event-card">
                <img src="${imageUrl}" alt="${event.name}" class="event-image" onerror="this.src='images/placeholder-event.jpg'">
                <div class="event-content">
                    <span class="event-type">${escapeHtml(event.eventType)}</span>
                    <h3 class="event-title">${escapeHtml(event.name)}</h3>
                    <p class="event-description">${escapeHtml(event.description.substring(0, 100))}...</p>
                    
                    <div class="event-details">
                        <span>📅 <strong>${eventDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })}</strong></span>
                    </div>
                    <div class="event-details">
                        <span>🕐 <strong>${eventDate.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })}</strong></span>
                    </div>
                    <div class="event-details">
                        <span>📍 <strong>${escapeHtml(event.location)}</strong></span>
                    </div>
                    <div class="event-details">
                        <span>👥 <strong>${event.currentRegistrations}/${event.capacity}</strong> Registered</span>
                    </div>
                    
                    <div class="capacity-bar">
                        <div class="capacity-filled" style="width: ${Math.min(capacityPercent, 100)}%"></div>
                    </div>

                    <div class="event-actions">
                        <button class="register-btn ${isFull ? 'disabled' : ''}" 
                            onclick="openRegistrationModal(${event.id})"
                            ${isFull ? 'disabled' : ''}>
                            ${isFull ? 'Full' : 'Register'}
                        </button>
                        <button class="details-btn" onclick="openDetailsModal(${event.id})">Details</button>
                    </div>
                </div>
            </div>
        `;
    }).join('');
}

async function loadEventTypes() {
    try {
        const response = await fetch(`${API_BASE_URL}/types/all`);
        if (!response.ok) throw new Error('Failed to load event types');

        const types = await response.json();
        const select = document.getElementById('eventTypeFilter');

        types.forEach(type => {
            const option = document.createElement('option');
            option.value = type;
            option.textContent = type;
            select.appendChild(option);
        });
    } catch (error) {
        console.error('Error loading event types:', error);
    }
}

function openRegistrationModal(eventId) {
    currentEventId = eventId;
    document.getElementById('registrationModal').classList.add('active');
}

async function openDetailsModal(eventId) {
    try {
        const response = await fetch(`${API_BASE_URL}/${eventId}`);
        if (!response.ok) throw new Error('Failed to load event details');

        const event = await response.json();
        const eventDate = new Date(event.eventDate);
        const imageUrl = event.cardImage || event.imageUrl || 'images/placeholder-event.jpg';

        document.getElementById('detailsTitle').textContent = event.name;
        document.getElementById('detailsBody').innerHTML = `
            <img src="${imageUrl}" 
                 alt="${event.name}" 
                 style="width: 100%; height: auto; border-radius: 4px; margin-bottom: 1rem;"
                 onerror="this.src='images/placeholder-event.jpg'">
            <div class="event-details">
                <p><strong>Type:</strong> ${escapeHtml(event.eventType)}</p>
                <p><strong>Date:</strong> ${eventDate.toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric', year: 'numeric' })}</p>
                <p><strong>Time:</strong> ${eventDate.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })}</p>
                <p><strong>Location:</strong> ${escapeHtml(event.location)}</p>
                <p><strong>Registrations:</strong> ${event.currentRegistrations}/${event.capacity}</p>
                <hr>
                <p><strong>Description:</strong></p>
                <p>${escapeHtml(event.description)}</p>
            </div>
        `;
        document.getElementById('detailsModal').classList.add('active');
    } catch (error) {
        console.error('Error loading event details:', error);
        showMessage('Failed to load event details.', 'error');
    }
}

async function registerForEvent(event) {
    event.preventDefault();

    const name = document.getElementById('regName').value.trim();
    const email = document.getElementById('regEmail').value.trim();

    if (!name || !email) {
        showMessage('Please fill in all fields.', 'error');
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/${currentEventId}/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name, email })
        });

        const result = await response.json();

        if (!response.ok) {
            showMessage(result.message || 'Registration failed.', 'error');
            return;
        }

        showMessage('Successfully registered for the event!', 'success');
        closeModal('registrationModal');
        document.getElementById('regName').value = '';
        document.getElementById('regEmail').value = '';
        loadEvents();
    } catch (error) {
        console.error('Error registering:', error);
        showMessage('An error occurred during registration.', 'error');
    }
}

function fileToBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = error => reject(error);
    });
}

async function createEvent(event) {
    event.preventDefault();

    let cardImage = '';
    const imageInput = document.getElementById('eventImageFile');
    
    if (imageInput.files.length > 0) {
        try {
            cardImage = await fileToBase64(imageInput.files[0]);
        } catch (error) {
            showMessage('Failed to process image file.', 'error');
            return;
        }
    }

    const eventData = {
        name: document.getElementById('eventName').value.trim(),
        eventType: document.getElementById('eventType').value.trim(),
        eventDate: new Date(document.getElementById('eventDate').value).toISOString(),
        location: document.getElementById('eventLocation').value.trim(),
        capacity: parseInt(document.getElementById('eventCapacity').value),
        imageUrl: document.getElementById('eventImage').value.trim(),
        cardImage: cardImage,
        description: document.getElementById('eventDescription').value.trim()
    };

    if (!eventData.name || !eventData.eventType || !eventData.location || !eventData.description) {
        showMessage('Please fill in all required fields.', 'error');
        return;
    }

    try {
        const response = await fetch(API_BASE_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(eventData)
        });

        const result = await response.json();

        if (!response.ok) {
            showMessage(result.message || 'Failed to create event.', 'error');
            return;
        }

        showMessage('Event created successfully!', 'success');
        closeModal('createEventModal');
        resetEventForm();
        loadEvents();
    } catch (error) {
        console.error('Error creating event:', error);
        showMessage('An error occurred while creating the event.', 'error');
    }
}

function openCreateEventModal() {
    document.getElementById('createEventModal').classList.add('active');
}

function closeModal(modalId) {
    document.getElementById(modalId).classList.remove('active');
}

function resetFilters() {
    document.getElementById('searchInput').value = '';
    document.getElementById('eventTypeFilter').value = '';
    loadEvents();
}

function resetEventForm() {
    document.getElementById('eventName').value = '';
    document.getElementById('eventType').value = '';
    document.getElementById('eventDate').value = '';
    document.getElementById('eventLocation').value = '';
    document.getElementById('eventCapacity').value = '';
    document.getElementById('eventImage').value = '';
    document.getElementById('eventImageFile').value = '';
    document.getElementById('eventDescription').value = '';
}

function showMessage(text, type) {
    const messageDiv = document.getElementById('message');
    messageDiv.textContent = text;
    messageDiv.className = `message ${type} active`;
    setTimeout(() => messageDiv.classList.remove('active'), 4000);
}

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

async function loadEventsAsAdmin() {
    showMessage('Loading all events (including archived)...', 'success');
    loadEvents();
}

// Close modal when clicking outside
document.addEventListener('click', (e) => {
    const modals = ['registrationModal', 'detailsModal', 'createEventModal'];
    modals.forEach(modalId => {
        const modal = document.getElementById(modalId);
        if (e.target === modal) {
            closeModal(modalId);
        }
    });
});