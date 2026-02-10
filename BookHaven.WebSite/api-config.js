/**
 * API Configuration Module
 * Centralize API endpoint management
 */
// Check for browser environment FIRST, before any other code
const isBrowser = typeof window !== 'undefined' && typeof process === 'undefined';

const ApiConfig = {
    // Update this URL to match your API server
    BASE_URL: 'http://localhost:5233',

    ENDPOINTS: {
        BOOKS: '/api/book',
        AUTHORS: '/api/author',
        GENRES: '/api/genre',
        USERS: '/api/user',
        SUBSCRIBERS: '/api/subscriber',
        EVENTS: '/api/event',
    },

    // Get full endpoint URL
    getEndpoint(resource) {
        return `${this.BASE_URL}${this.ENDPOINTS[resource]}`;
    },

    // Get book by ID
    getBookUrl(id) {
        return `${this.getEndpoint('BOOKS')}/${id}`;
    },

    // Get author by ID
    getAuthorUrl(id) {
        return `${this.getEndpoint('AUTHORS')}/${id}`;
    },

    // Get genre by ID
    getGenreUrl(id) {
        return `${this.getEndpoint('GENRES')}/${id}`;
    }
};

// Make it accessible globally
if (typeof window !== 'undefined') {
    window.ApiConfig = ApiConfig;
    // For backward compatibility with events.js
    window.API_CONFIG = {
        baseUrl: ApiConfig.BASE_URL
    };
}