// ===== ACCESSIBILITY PREFERENCES MANAGER =====

const a11yManager = {
    // Default preferences
    defaults: {
        fontSize: '100%', // small, normal, large, x-large
        highContrast: false,
        darkMode: false,
        spacing: false,
        reduceMotion: false,
        focusIndicator: false
    },

    // Storage key
    storageKey: 'a11y-preferences',

    /**
     * Initialize accessibility manager
     */
    init() {
        this.loadPreferences();
        this.setupEventListeners();
        this.applyPreferences();
        this.addRoleToModal();
    },

    /**
     * Load preferences from localStorage
     */
    loadPreferences() {
        const stored = localStorage.getItem(this.storageKey);
        if (stored) {
            this.preferences = { ...this.defaults, ...JSON.parse(stored) };
        } else {
            this.preferences = { ...this.defaults };
        }
    },

    /**
     * Save preferences to localStorage
     */
    savePreferences() {
        localStorage.setItem(this.storageKey, JSON.stringify(this.preferences));
    },

    /**
     * Setup event listeners for the modal
     */
    setupEventListeners() {
        // Toggle modal
        const toggleBtn = document.getElementById('a11y-toggle');
        const closeBtn = document.getElementById('a11y-close');
        const modal = document.getElementById('a11y-modal');

        if (toggleBtn) {
            toggleBtn.addEventListener('click', () => this.toggleModal());
        }

        if (closeBtn) {
            closeBtn.addEventListener('click', () => this.closeModal());
        }

        // Close modal when clicking outside
        if (modal) {
            modal.addEventListener('click', (e) => {
                if (e.target === modal) {
                    this.closeModal();
                }
            });
        }

        // Font size buttons
        document.querySelectorAll('.a11y-size-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const size = e.target.dataset.size;
                this.changeFontSize(size);
            });
        });

        // Checkboxes
        document.getElementById('a11y-high-contrast')?.addEventListener('change', (e) => {
            this.preferences.highContrast = e.target.checked;
            this.applyPreferences();
            this.savePreferences();
        });

        document.getElementById('a11y-dark-mode')?.addEventListener('change', (e) => {
            this.preferences.darkMode = e.target.checked;
            this.applyPreferences();
            this.savePreferences();
        });

        document.getElementById('a11y-spacing')?.addEventListener('change', (e) => {
            this.preferences.spacing = e.target.checked;
            this.applyPreferences();
            this.savePreferences();
        });

        document.getElementById('a11y-reduce-motion')?.addEventListener('change', (e) => {
            this.preferences.reduceMotion = e.target.checked;
            this.applyPreferences();
            this.savePreferences();
        });

        document.getElementById('a11y-focus-indicator')?.addEventListener('change', (e) => {
            this.preferences.focusIndicator = e.target.checked;
            this.applyPreferences();
            this.savePreferences();
        });

        // Reset button
        document.getElementById('a11y-reset')?.addEventListener('click', () => {
            this.resetPreferences();
        });

        // Close modal with Escape key
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && !document.getElementById('a11y-modal').hidden) {
                this.closeModal();
            }
        });
    },

    /**
     * Toggle modal visibility
     */
    toggleModal() {
        const modal = document.getElementById('a11y-modal');
        if (modal.hidden) {
            this.openModal();
        } else {
            this.closeModal();
        }
    },

    /**
     * Open modal and sync checkbox states
     */
    openModal() {
        const modal = document.getElementById('a11y-modal');
        modal.hidden = false;
        modal.setAttribute('aria-hidden', 'false');

        // Sync checkbox states
        document.getElementById('a11y-high-contrast').checked = this.preferences.highContrast;
        document.getElementById('a11y-dark-mode').checked = this.preferences.darkMode;
        document.getElementById('a11y-spacing').checked = this.preferences.spacing;
        document.getElementById('a11y-reduce-motion').checked = this.preferences.reduceMotion;
        document.getElementById('a11y-focus-indicator').checked = this.preferences.focusIndicator;

        // Focus on close button for accessibility
        document.getElementById('a11y-close').focus();
    },

    /**
     * Close modal
     */
    closeModal() {
        const modal = document.getElementById('a11y-modal');
        modal.hidden = true;
        modal.setAttribute('aria-hidden', 'true');

        // Return focus to toggle button
        document.getElementById('a11y-toggle').focus();
    },

    /**
     * Change font size
     */
    changeFontSize(size) {
        const body = document.body;
        const display = document.getElementById('size-display');

        // Remove all font size classes
        body.classList.remove('a11y-font-small', 'a11y-font-large', 'a11y-font-x-large');

        switch (size) {
            case 'small':
                body.classList.add('a11y-font-small');
                this.preferences.fontSize = 'small';
                display.textContent = '85%';
                break;
            case 'large':
                body.classList.add('a11y-font-large');
                this.preferences.fontSize = 'large';
                display.textContent = '115%';
                break;
            default:
                this.preferences.fontSize = '100%';
                display.textContent = '100%';
        }

        this.savePreferences();
    },

    /**
     * Apply all preferences to the page
     */
    applyPreferences() {
        const body = document.body;

        // Remove all accessibility classes
        body.classList.remove('a11y-high-contrast', 'a11y-dark-mode', 'a11y-spacing', 'a11y-reduce-motion', 'a11y-focus-indicator');

        // Apply active preferences
        if (this.preferences.highContrast) {
            body.classList.add('a11y-high-contrast');
        }
        if (this.preferences.darkMode) {
            body.classList.add('a11y-dark-mode');
        }
        if (this.preferences.spacing) {
            body.classList.add('a11y-spacing');
        }
        if (this.preferences.reduceMotion) {
            body.classList.add('a11y-reduce-motion');
        }
        if (this.preferences.focusIndicator) {
            body.classList.add('a11y-focus-indicator');
        }

        // Apply font size
        if (this.preferences.fontSize === 'small') {
            body.classList.add('a11y-font-small');
        } else if (this.preferences.fontSize === 'large') {
            body.classList.add('a11y-font-large');
        }
    },

    /**
     * Reset to default preferences
     */
    resetPreferences() {
        this.preferences = { ...this.defaults };
        this.applyPreferences();
        this.savePreferences();

        // Update UI
        document.getElementById('size-display').textContent = '100%';
        this.openModal();

        // Announce to screen readers
        const announcement = document.createElement('div');
        announcement.setAttribute('role', 'status');
        announcement.setAttribute('aria-live', 'polite');
        announcement.textContent = 'Preferences reset to defaults';
        announcement.style.position = 'absolute';
        announcement.style.left = '-10000px';
        document.body.appendChild(announcement);
        setTimeout(() => announcement.remove(), 1000);
    },

    /**
     * Add proper ARIA role to modal (fallback if not in HTML)
     */
    addRoleToModal() {
        const modal = document.getElementById('a11y-modal');
        if (modal && !modal.getAttribute('role')) {
            modal.setAttribute('role', 'dialog');
        }
    }
};

// Initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => a11yManager.init());
} else {
    a11yManager.init();
}