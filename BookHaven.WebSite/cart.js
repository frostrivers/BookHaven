// Shopping Cart Page JavaScript
// This script handles the display and management of items in the shopping cart

document.addEventListener('DOMContentLoaded', () => {
    // Initialize cart display
    cart.updateCartDisplay();
    displayCartItems();
    setupCartEventListeners();
});

// Display cart items on the cart page
function displayCartItems() {
    const emptyMessage = document.getElementById('empty-cart-message');
    const cartContent = document.getElementById('cart-content');
    const cartItemsList = document.getElementById('cart-items-list');

    if (cart.items.length === 0) {
        emptyMessage.style.display = 'block';
        cartContent.style.display = 'none';
        return;
    }

    emptyMessage.style.display = 'none';
    cartContent.style.display = 'block';

    // Clear existing rows
    cartItemsList.innerHTML = '';

    // Add each item to the table
    cart.items.forEach(item => {
        const row = document.createElement('tr');
        const subtotal = (item.price * item.quantity).toFixed(2);

        row.innerHTML = `
            <td class="item-title">${escapeHtml(item.title)}</td>
            <td class="item-price">$${parseFloat(item.price).toFixed(2)}</td>
            <td class="item-quantity">
                <div class="quantity-controls">
                    <button class="qty-btn minus-btn" data-book-id="${item.bookId}" aria-label="Decrease quantity">−</button>
                    <input type="number" 
                           class="qty-input" 
                           value="${item.quantity}" 
                           min="1" 
                           data-book-id="${item.bookId}"
                           aria-label="Quantity for ${escapeHtml(item.title)}">
                    <button class="qty-btn plus-btn" data-book-id="${item.bookId}" aria-label="Increase quantity">+</button>
                </div>
            </td>
            <td class="item-subtotal">$${subtotal}</td>
            <td class="item-remove">
                <button class="remove-btn" data-book-id="${item.bookId}" aria-label="Remove ${escapeHtml(item.title)} from cart">
                    🗑️ Remove
                </button>
            </td>
        `;

        cartItemsList.appendChild(row);
    });

    // Update summary
    updateCartSummary();

    // Attach event listeners
    attachQuantityListeners();
    attachRemoveListeners();
}

// Update the cart summary (subtotal, tax, total)
function updateCartSummary() {
    const subtotal = cart.getTotal();
    const shippingCost = subtotal > 50 ? 0 : 5.99; // Free shipping over $50
    const taxRate = 0.08; // 8% tax
    const tax = (subtotal * taxRate).toFixed(2);
    const total = (subtotal + shippingCost + parseFloat(tax)).toFixed(2);

    const subtotalEl = document.getElementById('subtotal');
    const shippingEl = document.getElementById('shipping');
    const taxEl = document.getElementById('tax');
    const totalEl = document.getElementById('total');

    if (subtotalEl) subtotalEl.textContent = `$${subtotal.toFixed(2)}`;
    if (shippingEl) shippingEl.textContent = `$${shippingCost.toFixed(2)}`;
    if (taxEl) taxEl.textContent = `$${tax}`;
    if (totalEl) totalEl.textContent = `$${total}`;
}

// Attach event listeners to quantity buttons
function attachQuantityListeners() {
    // Minus button listeners
    document.querySelectorAll('.minus-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            const bookId = parseInt(btn.getAttribute('data-book-id'));
            const input = document.querySelector(`.qty-input[data-book-id="${bookId}"]`);
            if (input) {
                const newQty = Math.max(1, parseInt(input.value) - 1);
                cart.updateQuantity(bookId, newQty);
                input.value = newQty;
                displayCartItems();
            }
        });
    });

    // Plus button listeners
    document.querySelectorAll('.plus-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            const bookId = parseInt(btn.getAttribute('data-book-id'));
            const input = document.querySelector(`.qty-input[data-book-id="${bookId}"]`);
            if (input) {
                const newQty = parseInt(input.value) + 1;
                cart.updateQuantity(bookId, newQty);
                input.value = newQty;
                displayCartItems();
            }
        });
    });

    // Input field listeners (manual entry)
    document.querySelectorAll('.qty-input').forEach(input => {
        input.addEventListener('change', (e) => {
            const bookId = parseInt(input.getAttribute('data-book-id'));
            let newQty = parseInt(input.value) || 1;
            
            // Validate quantity
            if (newQty < 1) newQty = 1;
            if (newQty > 99) newQty = 99;
            
            cart.updateQuantity(bookId, newQty);
            input.value = newQty;
            displayCartItems();
        });
    });
}

// Attach event listeners to remove buttons
function attachRemoveListeners() {
    document.querySelectorAll('.remove-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            const bookId = parseInt(btn.getAttribute('data-book-id'));
            const item = cart.items.find(i => i.bookId === bookId);
            
            if (item && confirm(`Remove "${item.title}" from cart?`)) {
                cart.removeItem(bookId);
                showCartNotification(`"${item.title}" removed from cart`);
                displayCartItems();
            }
        });
    });
}

// Setup checkout and clear cart buttons
function setupCartEventListeners() {
    const checkoutBtn = document.getElementById('checkout-btn');
    const clearCartBtn = document.getElementById('clear-cart-btn');

    if (checkoutBtn) {
        checkoutBtn.addEventListener('click', () => {
            if (cart.items.length === 0) {
                alert('Your cart is empty. Add some books first!');
                return;
            }
            handleCheckout();
        });
    }

    if (clearCartBtn) {
        clearCartBtn.addEventListener('click', () => {
            if (confirm('Are you sure you want to clear your entire cart?')) {
                cart.clear();
                displayCartItems();
                showCartNotification('Cart cleared');
            }
        });
    }
}

// Handle checkout (placeholder for future payment integration)
function handleCheckout() {
    const total = cart.getTotal();
    const itemCount = cart.getItemCount();

    // Prepare order data
    const orderData = {
        items: cart.items.map(item => ({
            bookId: item.bookId,
            title: item.title,
            price: item.price,
            quantity: item.quantity
        })),
        subtotal: total,
        itemCount: itemCount,
        timestamp: new Date().toISOString()
    };

    // Log order data (replace with actual API call in production)
    console.log('Order Data:', orderData);

    // Show success message
    showCheckoutMessage();
}

// Show checkout success message
function showCheckoutMessage() {
    const message = document.createElement('div');
    message.className = 'checkout-message';
    message.innerHTML = `
        <div class="checkout-modal">
            <h3>Order Summary</h3>
            <p>Items in cart: <strong>${cart.getItemCount()}</strong></p>
            <p>Total: <strong>${document.getElementById('total').textContent}</strong></p>
            <p>Thank you for your order! We'll process it shortly.</p>
            <button class="cta-button" onclick="this.closest('.checkout-modal').parentElement.remove(); window.location.href='shop.html'">
                Continue Shopping
            </button>
            <button class="cta-button" onclick="this.closest('.checkout-modal').parentElement.remove()">
                Stay on Cart Page
            </button>
        </div>
    `;

    document.body.appendChild(message);
    message.classList.add('show');
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, m => map[m]);
}