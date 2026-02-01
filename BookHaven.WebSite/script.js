// Configuration
// I put this in the api-config.js  Easy to find there

// Error timeout constant
const REQUEST_TIMEOUT = 15000; // 5 seconds

// Pagination and search state
const appState = {
    currentPage: 1,
    pageSize: 6,
    totalPages: 0,
    totalBooks: 0,
    allBooks: [],
    isSearching: false,
    searchQuery: '',
    debounceTimer: null,
    isLoading: false,
    selectedItemTypeFilter: null,
    selectedItemTypeName: null
};

// ===== SHOPPING CART FUNCTIONALITY =====
// Initialize cart from localStorage
const cart = {
    items: JSON.parse(localStorage.getItem('cart') || '[]'),

    save() {
        localStorage.setItem('cart', JSON.stringify(this.items));
        this.updateCartDisplay();
    },

    addItem(bookId, title, price) {
        const existingItem = this.items.find(item => item.bookId === bookId);

        if (existingItem) {
            existingItem.quantity += 1;
        } else {
            this.items.push({
                bookId,
                title,
                price: parseFloat(price),
                quantity: 1
            });
        }

        this.save();
        showCartNotification(`"${title}" added to cart!`);
    },

    removeItem(bookId) {
        this.items = this.items.filter(item => item.bookId !== bookId);
        this.save();
    },

    updateQuantity(bookId, quantity) {
        const item = this.items.find(item => item.bookId === bookId);
        if (item) {
            item.quantity = Math.max(1, quantity);
            this.save();
        }
    },

    getTotal() {
        return this.items.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    },

    getItemCount() {
        return this.items.reduce((sum, item) => sum + item.quantity, 0);
    },

    clear() {
        this.items = [];
        this.save();
    },

    updateCartDisplay() {
        const cartCount = document.getElementById('cart-count');
        const itemCount = this.getItemCount();

        if (cartCount) {
            cartCount.textContent = itemCount;
            cartCount.style.display = itemCount > 0 ? 'block' : 'none';
        }
    }
};

// Handle "Add to Cart" button click
function handleAddToCart(event) {
    const btn = event.target;
    const bookId = parseInt(btn.getAttribute('data-book-id'));
    const title = btn.getAttribute('data-book-title');
    const price = btn.getAttribute('data-book-price');

    cart.addItem(bookId, title, price);

    // Provide visual feedback
    btn.classList.add('success');
    btn.textContent = '✓ Added!';
    setTimeout(() => {
        btn.classList.remove('success');
        btn.textContent = 'Add to Cart';
    }, 2000);
}

// Show cart notification
function showCartNotification(message) {
    const notification = document.createElement('div');
    notification.className = 'cart-notification';
    notification.textContent = message;
    document.body.appendChild(notification);

    setTimeout(() => {
        notification.classList.add('show');
    }, 10);

    setTimeout(() => {
        notification.classList.remove('show');
        notification.remove();
    }, 3000);
}

document.addEventListener('DOMContentLoaded', () => {
    // Verify ApiConfig is loaded before proceeding
    if (typeof ApiConfig === 'undefined') {
        console.error('ApiConfig is not loaded. Make sure api-config.js is included before script.js');
        return;
    }

    // Only initialize shop features if product-grid exists
    const productGrid = document.getElementById('product-grid');
    if (productGrid) {
        loadBooks();
        setupPaginationControls();
        setupSearchControls();
        loadItemTypeFilters(); // Add this line
    }
    setupNavigation();
    cart.updateCartDisplay();
});

// Load and display ItemType filters
async function loadItemTypeFilters() {
    try {
        const response = await fetchWithTimeout(
            `${ApiConfig.getEndpoint('BOOKS')}/categories`
        );

        const categories = await handleApiResponse(response, 'loading item type filters');

        // Get unique item types with proper names
        const uniqueTypes = new Map();
        
        // We need to fetch all books first to get ItemType names
        const allBooksResponse = await fetchWithTimeout(
            `${ApiConfig.getEndpoint('BOOKS')}?pageNumber=1&pageSize=50`
        );
        const allBooksData = await handleApiResponse(allBooksResponse, 'loading books for filters');
        
        // Extract unique ItemTypes
        const itemTypes = new Map();
        allBooksData.data.forEach(book => {
            if (book.itemTypeId && !itemTypes.has(book.itemTypeId)) {
                itemTypes.set(book.itemTypeId, book.itemTypeName);
            }
        });

        displayItemTypeFilters(Array.from(itemTypes.entries()));
        setupFilterControls();
    } catch (error) {
        console.error('Error loading item type filters:', error);
    }
}

// Display ItemType filter buttons
function displayItemTypeFilters(itemTypes) {
    const filterContainer = document.getElementById('itemtype-filter');
    if (!filterContainer) return;

    filterContainer.innerHTML = ''; // Clear existing buttons

    if (itemTypes.length === 0) {
        filterContainer.innerHTML = '<p class="no-filters">No item types available.</p>';
        return;
    }

    itemTypes.forEach(([typeId, typeName]) => {
        const button = document.createElement('button');
        button.classList.add('filter-btn');
        button.setAttribute('data-itemtype-id', typeId);
        button.setAttribute('data-itemtype-name', typeName);
        button.textContent = typeName;
        
        button.addEventListener('click', () => {
            handleItemTypeFilterClick(typeId, typeName, button);
        });
        
        filterContainer.appendChild(button);
    });
}

// Handle ItemType filter button click
function handleItemTypeFilterClick(itemTypeId, itemTypeName, buttonElement) {
    // Toggle filter
    if (appState.selectedItemTypeFilter === itemTypeId) {
        // Deselect filter
        appState.selectedItemTypeFilter = null;
        appState.selectedItemTypeName = null;
        buttonElement.classList.remove('active');
        clearSearch(); // Reset to show all books
    } else {
        // Select new filter
        // Remove active class from all buttons
        document.querySelectorAll('.filter-btn').forEach(btn => {
            btn.classList.remove('active');
        });
        
        // Add active class to clicked button
        appState.selectedItemTypeFilter = itemTypeId;
        appState.selectedItemTypeName = itemTypeName;
        buttonElement.classList.add('active');
        
        // Update search input and search
        const searchInput = document.getElementById('search-input');
        if (searchInput) {
            searchInput.value = itemTypeName;
        }
        
        appState.searchQuery = itemTypeName;
        appState.currentPage = 1;
        appState.isSearching = true;
        searchBooks();
        
        // Scroll to top
        const shopSection = document.getElementById('shop');
        if (shopSection) {
            shopSection.scrollIntoView({ behavior: 'smooth' });
        }
    }
}

// Setup filter controls
function setupFilterControls() {
    const clearFilterBtn = document.getElementById('clear-filter-btn');
    if (clearFilterBtn) {
        clearFilterBtn.addEventListener('click', () => {
            appState.selectedItemTypeFilter = null;
            appState.selectedItemTypeName = null;
            document.querySelectorAll('.filter-btn').forEach(btn => {
                btn.classList.remove('active');
            });
            clearSearch();
        });
    }
}

// Setup search event listeners
function setupSearchControls() {
    const searchInput = document.getElementById('search-input');
    const clearBtn = document.getElementById('clear-search-btn');

    if (!searchInput || !clearBtn) return;

    searchInput.addEventListener('input', (e) => {
        handleSearchInput(e.target.value);
    });

    clearBtn.addEventListener('click', () => {
        searchInput.value = '';
        clearSearch();
    });

    // Allow Enter key to search
    searchInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            handleSearchInput(searchInput.value);
        }
    });
}

// Handle search input with debouncing
function handleSearchInput(query) {
    // Clear previous timer
    clearTimeout(appState.debounceTimer);

    // Set new timer for debounced search (300ms delay)
    appState.debounceTimer = setTimeout(() => {
        if (query.trim().length > 0) {
            appState.searchQuery = query.trim();
            appState.currentPage = 1;
            appState.isSearching = true;
            searchBooks();
        } else {
            clearSearch();
        }
    }, 300);
}

// Helper function: Make API call with timeout and error handling
async function fetchWithTimeout(url, options = {}) {
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), REQUEST_TIMEOUT);

    try {
        console.log(`Fetching from: ${url}`);
        const response = await fetch(url, {
            method: options.method || 'GET',
            headers: {
                'Content-Type': 'application/json',
                ...options.headers
            },
            mode: 'cors',
            ...options,
            signal: controller.signal
        });

        clearTimeout(timeoutId);

        if (!response.ok) {
            console.error(`API returned status ${response.status}: ${response.statusText}`);
        }

        return response;
    } catch (error) {
        clearTimeout(timeoutId);
        console.error('Fetch error details:', error);

        if (error.name === 'AbortError') {
            throw new Error('Request timeout: The server took too long to respond. Please try again.');
        }

        if (error instanceof TypeError && error.message === 'Failed to fetch') {
            console.error('CORS or network error. Check browser console for details.');
            throw new Error('Unable to connect to the API server. Verify it\'s running on ' + ApiConfig.BASE_URL + ' and allows cross-origin requests.');
        }

        throw error;
    }
}

// Helper function: Handle API response
async function handleApiResponse(response, errorContext) {
    if (!response.ok) {
        let errorMessage = `Error: ${response.status} ${response.statusText}`;

        try {
            const errorData = await response.json();
            errorMessage = errorData.message || errorMessage;
        } catch {
            // If response is not JSON, use default message
        }

        const error = new Error(errorMessage);
        error.status = response.status;
        throw error;
    }

    try {
        return await response.json();
    } catch (error) {
        throw new Error('Invalid server response. Please try again.');
    }
}

// Search books with query
async function searchBooks() {
    if (appState.isLoading) return; // Prevent multiple concurrent requests

    try {
        appState.isLoading = true;
        displayLoadingState();

        const params = new URLSearchParams({
            query: appState.searchQuery,
            pageNumber: appState.currentPage,
            pageSize: appState.pageSize
        });

        const response = await fetchWithTimeout(
            `${ApiConfig.getEndpoint('BOOKS')}/search?${params}`
        );

        const data = await handleApiResponse(response, 'searching books');

        // Update state
        appState.totalPages = data.totalPages;
        appState.totalBooks = data.totalBooks;
        appState.allBooks = data.data;

        displayBooks(data.data);
        updatePaginationControls();
        updatePaginationInfo();
        updateSearchResultsInfo(data);
    } catch (error) {
        console.error('Error searching books:', error);
        handleFetchError(error, 'search for books');
    } finally {
        appState.isLoading = false;
    }
}

// Clear search and load all books
function clearSearch() {
    const searchInput = document.getElementById('search-input');
    const searchResultsInfo = document.getElementById('search-results-info');

    appState.searchQuery = '';
    appState.currentPage = 1;
    appState.isSearching = false;

    if (searchInput) searchInput.value = '';
    if (searchResultsInfo) searchResultsInfo.textContent = '';

    loadBooks();
}

// Update search results info
function updateSearchResultsInfo(data) {
    const infoElement = document.getElementById('search-results-info');
    if (!infoElement) return;

    if (appState.searchQuery) {
        infoElement.textContent = `Found ${data.totalBooks} result${data.totalBooks !== 1 ? 's' : ''} for "${appState.searchQuery}"`;
        infoElement.style.display = 'block';
    }
}

// Setup pagination event listeners
function setupPaginationControls() {
    const prevBtn = document.getElementById('prev-page');
    const nextBtn = document.getElementById('next-page');
    const pageSizeSelect = document.getElementById('page-size');

    if (!prevBtn || !nextBtn || !pageSizeSelect) return;

    prevBtn.addEventListener('click', () => goToPreviousPage());
    nextBtn.addEventListener('click', () => goToNextPage());
    pageSizeSelect.addEventListener('change', (e) => {
        appState.pageSize = parseInt(e.target.value);
        appState.currentPage = 1;
        if (appState.isSearching) {
            searchBooks();
        } else {
            loadBooks();
        }
    });
}

// Fetch books from the API with pagination
async function loadBooks() {
    if (appState.isLoading) return; // Prevent multiple concurrent requests

    try {
        appState.isLoading = true;
        displayLoadingState();

        const params = new URLSearchParams({
            pageNumber: appState.currentPage,
            pageSize: appState.pageSize
        });

        const response = await fetchWithTimeout(
            `${ApiConfig.getEndpoint('BOOKS')}?${params}`
        );

        const data = await handleApiResponse(response, 'loading books');

        // Update state
        appState.totalPages = data.totalPages;
        appState.totalBooks = data.totalBooks;
        appState.allBooks = data.data;

        displayBooks(data.data);
        updatePaginationControls();
        updatePaginationInfo();
    } catch (error) {
        console.error('Error loading books from API:', error);
        handleFetchError(error, 'load books from the API', true);
    } finally {
        appState.isLoading = false;
    }
}

// Handle fetch errors with user-friendly messages
function handleFetchError(error, action, offerFallback = false) {
    let userMessage = '';

    if (error.message.includes('Failed to fetch')) {
        userMessage = `Unable to connect to the server. Please check your internet connection and try again.`;
    } else if (error.message.includes('timeout')) {
        userMessage = `The request took too long. Please try again.`;
    } else if (error.status === 404) {
        userMessage = `The requested resource was not found.`;
    } else if (error.status === 500) {
        userMessage = `Server error. Please try again later.`;
    } else if (error.message.includes('Invalid server response')) {
        userMessage = `Received an unexpected response from the server. Please try again.`;
    } else {
        userMessage = `Unable to ${action}. Please try again.`;
    }

    if (offerFallback) {
        displayError(userMessage);
        // Optionally load fallback data
        setTimeout(() => {
            console.log('Attempting to load fallback data...');
            loadBooksFromLocalJSON();
        }, 1000);
    } else {
        displayError(userMessage);
    }
}

// Display books in the grid
function displayBooks(books) {
    const grid = document.getElementById('product-grid');
    if (!grid) return;

    grid.innerHTML = ''; // Clear existing content

    if (books.length === 0) {
        grid.innerHTML = '<p class="no-results">No books available. Try adjusting your search.</p>';
        return;
    }

    books.forEach(book => {
        const card = document.createElement('div');
        card.classList.add('product-card');
        card.innerHTML = `
            <div class="product-image">
                <img src="${book.coverImage || 'images/book-placeholder.jpg'}" alt="${escapeHtml(book.title)}">
            </div>
            <h3>${highlightSearchTerm(book.title)}</h3>
            <p class="author">Author: <a href="#" class="author-link" data-author="${escapeHtml(book.authorName || 'Unknown')}">${escapeHtml(book.authorName || 'Unknown')}</a></p>
            <p class="itemType">Item Type: <a href="#" class="itemtype-link" data-itemtype-id="${book.itemTypeId}" data-itemtype-name="${escapeHtml(book.itemTypeName || 'Unknown')}">${escapeHtml(book.itemTypeName || 'Unknown')}</a></p>
            <p class="description">${escapeHtml(book.description || 'No description available')}</p>
            <p class="price">$${parseFloat(book.price).toFixed(2)}</p>
            <p class="stock">In Stock: ${book.stockQuantity}</p>
            <button class="cta-button add-to-cart-btn" data-book-id="${book.id}" data-book-title="${escapeHtml(book.title)}" data-book-price="${book.price}">Add to Cart</button>
        `;
        grid.appendChild(card);
    });

    // Attach event listeners to "Add to Cart" buttons
    document.querySelectorAll('.add-to-cart-btn').forEach(btn => {
        btn.addEventListener('click', handleAddToCart);
    });

    // Attach event listeners to author links
    document.querySelectorAll('.author-link').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            const authorName = link.getAttribute('data-author');
            searchByAuthor(authorName);
        });
    });

    // Attach event listeners to item type links
    document.querySelectorAll('.itemtype-link').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            const itemTypeId = link.getAttribute('data-itemtype-id');
            const itemTypeName = link.getAttribute('data-itemtype-name');
            searchByItemType(itemTypeId, itemTypeName);
        });
    });
}

// Search for all books by a specific author
function searchByAuthor(authorName) {
    // Set search state
    appState.searchQuery = authorName;
    appState.currentPage = 1;
    appState.isSearching = true;

    // Update search input to reflect the query
    const searchInput = document.getElementById('search-input');
    if (searchInput) {
        searchInput.value = authorName;
    }

    // Perform the search
    searchBooks();

    // Scroll to top of shop section
    const shopSection = document.getElementById('shop');
    if (shopSection) {
        shopSection.scrollIntoView({ behavior: 'smooth' });
    }
}

// Search for all books by a specific item type
function searchByItemType(itemTypeId, itemTypeName) {
    // Set search state
    appState.searchQuery = itemTypeName;
    appState.currentPage = 1;
    appState.isSearching = true;

    // Update search input to reflect the query
    const searchInput = document.getElementById('search-input');
    if (searchInput) {
        searchInput.value = itemTypeName;
    }

    // Perform the search
    searchBooks();

    // Scroll to top of shop section
    const shopSection = document.getElementById('shop');
    if (shopSection) {
        shopSection.scrollIntoView({ behavior: 'smooth' });
    }
}

// Escape HTML to prevent XSS attacks
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

// Highlight search term in results
function highlightSearchTerm(text) {
    if (!appState.searchQuery) return escapeHtml(text);

    const regex = new RegExp(`(${appState.searchQuery.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')})`, 'gi');
    const highlighted = text.replace(regex, '<mark>$1</mark>');
    return highlighted;
}

// Display loading state
function displayLoadingState() {
    const grid = document.getElementById('product-grid');
    if (!grid) return;

    grid.innerHTML = '<p class="loading-message">Loading books...</p>';
}

// Display error message
function displayError(message) {
    const grid = document.getElementById('product-grid');
    if (!grid) return;

    grid.innerHTML = `<p class="error-message">${escapeHtml(message)}</p>`;
}

// Update pagination buttons
function updatePaginationControls() {
    const prevBtn = document.getElementById('prev-page');
    const nextBtn = document.getElementById('next-page');
    const pageNumbersContainer = document.getElementById('page-numbers');

    if (!prevBtn || !nextBtn || !pageNumbersContainer) return;

    // Disable/enable buttons
    prevBtn.disabled = appState.currentPage === 1;
    nextBtn.disabled = appState.currentPage === appState.totalPages;

    // Generate page number buttons
    pageNumbersContainer.innerHTML = '';
    const maxPagesToShow = 5;
    const startPage = Math.max(1, appState.currentPage - Math.floor(maxPagesToShow / 2));
    const endPage = Math.min(appState.totalPages, startPage + maxPagesToShow - 1);

    // Show "first page" button if needed
    if (startPage > 1) {
        const firstBtn = createPageButton(1, 1 === appState.currentPage);
        pageNumbersContainer.appendChild(firstBtn);

        if (startPage > 2) {
            const dots = document.createElement('span');
            dots.className = 'pagination-dots';
            dots.textContent = '...';
            pageNumbersContainer.appendChild(dots);
        }
    }

    // Generate page buttons
    for (let i = startPage; i <= endPage; i++) {
        const btn = createPageButton(i, i === appState.currentPage);
        pageNumbersContainer.appendChild(btn);
    }

    // Show "last page" button if needed
    if (endPage < appState.totalPages) {
        if (endPage < appState.totalPages - 1) {
            const dots = document.createElement('span');
            dots.className = 'pagination-dots';
            dots.textContent = '...';
            pageNumbersContainer.appendChild(dots);
        }

        const lastBtn = createPageButton(appState.totalPages,
            appState.totalPages === appState.currentPage);
        pageNumbersContainer.appendChild(lastBtn);
    }
}

// Create a page number button
function createPageButton(pageNumber, isActive) {
    const btn = document.createElement('button');
    btn.className = `pagination-btn page-number ${isActive ? 'active' : ''}`;
    btn.textContent = pageNumber;
    btn.disabled = isActive;
    btn.addEventListener('click', () => goToPage(pageNumber));
    return btn;
}

// Update pagination info text
function updatePaginationInfo() {
    const infoElement = document.getElementById('pagination-info');
    if (!infoElement) return;

    const startItem = (appState.currentPage - 1) * appState.pageSize + 1;
    const endItem = Math.min(
        appState.currentPage * appState.pageSize,
        appState.totalBooks
    );

    infoElement.textContent = `Showing ${startItem}-${endItem} of ${appState.totalBooks} books (Page ${appState.currentPage} of ${appState.totalPages})`;
}

// Navigation functions
function goToPage(pageNumber) {
    appState.currentPage = pageNumber;
    if (appState.isSearching) {
        searchBooks();
    } else {
        loadBooks();
    }
    // Scroll to top of shop section
    const shopSection = document.getElementById('shop');
    if (shopSection) {
        shopSection.scrollIntoView({ behavior: 'smooth' });
    }
}

function goToPreviousPage() {
    if (appState.currentPage > 1) {
        goToPage(appState.currentPage - 1);
    }
}

function goToNextPage() {
    if (appState.currentPage < appState.totalPages) {
        goToPage(appState.currentPage + 1);
    }
}

// Fallback: Load from local JSON if API fails
async function loadBooksFromLocalJSON() {
    try {
        const response = await fetchWithTimeout('shop.json');

        if (!response.ok) {
            throw new Error('Unable to load fallback data');
        }

        const products = await response.json();
        const grid = document.getElementById('product-grid');
        if (!grid) return;

        grid.innerHTML = '';

        products.forEach(product => {
            const card = document.createElement('div');
            card.classList.add('product-card');
            card.innerHTML = `
                <img src="${escapeHtml(product.image)}" alt="${escapeHtml(product.name)}">
                <h3>${escapeHtml(product.name)}</h3>
                <p>Category: ${escapeHtml(product.category)}</p>
                <p>Price: $${parseFloat(product.price).toFixed(2)}</p>
                <p>${escapeHtml(product.description)}</p>
                <a href="index.html#contact" class="cta-button">Inquire</a>
            `;
            grid.appendChild(card);
        });
    } catch (error) {
        console.error('Error loading fallback data:', error);
        displayError('Unable to load products. Please try again later.');
    }
}

// Setup navigation
function setupNavigation() {
    const hamburger = document.getElementById('hamburger');
    const navMenu = document.getElementById('nav-menu');

    if (!hamburger || !navMenu) return;

    const navLinks = navMenu.querySelectorAll('a');

    // Toggle menu on hamburger click
    hamburger.addEventListener('click', function () {
        const isOpen = hamburger.getAttribute('aria-expanded') === 'true';
        hamburger.setAttribute('aria-expanded', !isOpen);
        hamburger.classList.toggle('active');
        navMenu.classList.toggle('active');
    });

    // Close menu when a link is clicked
    navLinks.forEach(link => {
        link.addEventListener('click', function () {
            hamburger.setAttribute('aria-expanded', 'false');
            hamburger.classList.remove('active');
            navMenu.classList.remove('active');
        });
    });

    // Close menu when clicking outside
    document.addEventListener('click', function (event) {
        const isClickInsideNav = navMenu.contains(event.target);
        const isClickOnHamburger = hamburger.contains(event.target);

        if (!isClickInsideNav && !isClickOnHamburger && hamburger.classList.contains('active')) {
            hamburger.setAttribute('aria-expanded', 'false');
            hamburger.classList.remove('active');
            navMenu.classList.remove('active');
        }
    });

    // Close menu on Escape key
    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape' && hamburger.classList.contains('active')) {
            hamburger.setAttribute('aria-expanded', 'false');
            hamburger.classList.remove('active');
            navMenu.classList.remove('active');
            hamburger.focus();
        }
    });
}