// Subscription Form Handler
document.addEventListener('DOMContentLoaded', function () {
    const subscribeForm = document.getElementById('subscribe-form');
    const messageDiv = document.getElementById('subscribe-message');

    subscribeForm.addEventListener('submit', handleSubscribeSubmit);
});

async function handleSubscribeSubmit(event) {
    event.preventDefault();

    const form = event.target;
    const messageDiv = document.getElementById('subscribe-message');

    // Clear previous messages
    messageDiv.textContent = '';
    messageDiv.className = 'subscribe-message';

    // Collect form data
    const formData = {
        email: document.getElementById('sub-email').value.trim(),
        name: document.getElementById('sub-name').value.trim(),
        phoneNumber: document.getElementById('sub-phone').value.trim(),
        preferences: Array.from(document.querySelectorAll('input[name="preferences"]:checked'))
            .map(checkbox => checkbox.value)
    };

    // Validate
    if (!formData.email || !formData.name) {
        showMessage('Please fill in all required fields.', 'error', messageDiv);
        return;
    }

    try {
        const response = await fetch(ApiConfig.getEndpoint('SUBSCRIBERS'), {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData),
            timeout: 15000
        });

        if (!response.ok) {
            throw new Error(`Server error: ${response.status}`);
        }

        const result = await response.json();
        form.reset();
        
        // Show confirmation modal
        showConfirmationModal(formData.name);

    } catch (error) {
        console.error('Subscription error:', error);
        showMessage('An error occurred. Please try again later.', 'error', messageDiv);
    }
}

function showMessage(message, type, messageDiv) {
    messageDiv.textContent = message;
    messageDiv.className = `subscribe-message ${type}`;
}

function showConfirmationModal(userName) {
    // Create modal overlay
    const modalOverlay = document.createElement('div');
    modalOverlay.className = 'subscription-modal-overlay';
    modalOverlay.id = 'subscription-modal';

    // Create modal content
    const modalContent = document.createElement('div');
    modalContent.className = 'subscription-modal-content';
    
    modalContent.innerHTML = `
        <div class="modal-success-icon">✓</div>
        <h2>Welcome to Book Haven!</h2>
        <p class="modal-greeting">Hi <strong>${escapeHtml(userName)}</strong>,</p>
        <p class="modal-message">Thank you for subscribing to our newsletter! You'll soon receive updates about new releases, recommendations, and exclusive offers.</p>
        <p class="modal-confirmation">A confirmation email has been sent to your inbox.</p>
        <div class="modal-actions">
            <button class="cta-button" onclick="continueShopping()">Continue Shopping</button>
            <button class="cta-button secondary" onclick="goHome()">Back to Home</button>
        </div>
    `;

    modalOverlay.appendChild(modalContent);
    document.body.appendChild(modalOverlay);

    // Show modal with animation
    setTimeout(() => {
        modalOverlay.classList.add('show');
    }, 100);
}

function continueShopping() {
    const modal = document.getElementById('subscription-modal');
    if (modal) {
        modal.classList.remove('show');
        setTimeout(() => {
            modal.remove();
            window.location.href = 'shop.html';
        }, 300);
    }
}

function goHome() {
    const modal = document.getElementById('subscription-modal');
    if (modal) {
        modal.classList.remove('show');
        setTimeout(() => {
            modal.remove();
            window.location.href = 'index.html';
        }, 300);
    }
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}