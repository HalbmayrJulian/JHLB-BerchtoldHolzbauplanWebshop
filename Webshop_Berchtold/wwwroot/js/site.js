// Bitte siehe die Dokumentation unter https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// für Einzelheiten zur Konfiguration dieses Projekts zum Bündeln und Minimieren von statischen Webressourcen.

// Schreibe deinen JavaScript-Code.

// ========================================
// LIVE SEARCH FUNKTIONALITÄT
// ========================================

document.addEventListener('DOMContentLoaded', function() {
    const searchDropdown = document.getElementById('searchDropdown');
    const searchInput = document.getElementById('searchInput');
    const searchResults = document.getElementById('searchResults');
    const searchDropdownMenu = document.querySelector('.search-dropdown-menu');
    let searchTimeout;

    if (searchDropdown && searchInput && searchResults) {
        // Focus auf Input wenn Dropdown geöffnet wird
        searchDropdown.addEventListener('shown.bs.dropdown', function () {
            searchInput.focus();
        });

        // Dropdown offen halten beim Klicken ins Input-Feld
        if (searchDropdownMenu) {
            searchDropdownMenu.addEventListener('click', function(e) {
                e.stopPropagation();
            });
        }

        // Input zurücksetzen beim Schließen
        searchDropdown.addEventListener('hidden.bs.dropdown', function () {
            searchInput.value = '';
            searchResults.innerHTML = '';
            searchResults.classList.remove('show');
        });

        // Live-Suche bei jedem Tastendruck
        searchInput.addEventListener('input', function() {
            const query = this.value.trim();
            
            // Clear previous timeout
            clearTimeout(searchTimeout);
            
            if (query.length === 0) {
                searchResults.innerHTML = '';
                searchResults.classList.remove('show');
                return;
            }
            
            // Zeige Loading Indicator
            searchResults.innerHTML = `
                <div class="search-loading">
                    <div class="spinner-border spinner-border-sm" role="status">
                        <span class="visually-hidden">Suchen...</span>
                    </div>
                    <p>Suche nach "${query}"...</p>
                </div>
            `;
            searchResults.classList.add('show');
            
            // Debounce: Warte 300ms nach letztem Tastendruck
            searchTimeout = setTimeout(function() {
                performSearch(query);
            }, 300);
        });

        // Enter-Taste: Gehe zur Suchseite
        searchInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                const query = this.value.trim();
                if (query.length > 0) {
                    window.location.href = `/Search?query=${encodeURIComponent(query)}`;
                }
            }
        });
    }
});

function performSearch(query) {
    const searchResults = document.getElementById('searchResults');
    
    fetch(`/Search?handler=Search&query=${encodeURIComponent(query)}`)
        .then(response => response.json())
        .then(data => {
            displaySearchResults(data.products, query);
        })
        .catch(error => {
            console.error('Search error:', error);
            searchResults.innerHTML = `
                <div class="alert alert-danger alert-sm m-0">
                    <i class="bi bi-exclamation-triangle"></i> 
                    Ein Fehler ist aufgetreten.
                </div>
            `;
        });
}

function displaySearchResults(products, query) {
    const searchResults = document.getElementById('searchResults');
    
    if (!products || products.length === 0) {
        searchResults.innerHTML = `
            <div class="no-results">
                <i class="bi bi-search"></i>
                <p>Keine Produkte gefunden</p>
                <small class="text-muted">für "${query}"</small>
            </div>
        `;
        return;
    }
    
    let html = '<div class="list-group list-group-flush">';
    
    products.forEach(function(product) {
        const icon = product.iconClass || 'bi-box';
        const category = product.kategorieName || 'Allgemein';
        const description = product.beschreibung || '';
        const shortDesc = description.length > 60 ? description.substring(0, 60) + '...' : description;
        
        html += `
            <a href="/ProductDetail?id=${product.id}" class="search-result-item">
                <div class="search-result-icon">
                    ${product.bildUrl ? 
                        `<img src="${product.bildUrl}" alt="${product.name}" class="img-fluid" />` :
                        `<i class="${icon}"></i>`
                    }
                </div>
                <div class="search-result-details">
                    <div class="search-result-title">${product.name}</div>
                    <div class="search-result-description">${shortDesc}</div>
                    <div class="search-result-category">
                        <i class="bi bi-tag"></i> ${category}
                    </div>
                </div>
                <div class="search-result-price">
                    €${product.preis.toFixed(2)}
                    <small>${product.einheit}</small>
                </div>
            </a>
        `;
    });
    
    html += '</div>';
    
    // "Alle Ergebnisse anzeigen" Link
    html += `
        <div class="text-center mt-2 pt-2 border-top">
            <a href="/Search?query=${encodeURIComponent(query)}" class="btn btn-sm btn-outline-success">
                <i class="bi bi-grid"></i> Alle anzeigen (${products.length}${products.length >= 8 ? '+' : ''})
            </a>
        </div>
    `;
    
    searchResults.innerHTML = html;
    searchResults.classList.add('show');
}

// ========================================
// PRODUKTDETAIL SEITE FUNKTIONALITÄT
// ========================================

document.addEventListener('DOMContentLoaded', function() {
    
    // Mengenauswahl Funktionalität
    const decreaseBtn = document.getElementById('decrease-qty');
    const increaseBtn = document.getElementById('increase-qty');
    const quantityInput = document.getElementById('quantity');
    
    if (decreaseBtn && increaseBtn && quantityInput) {
        decreaseBtn.addEventListener('click', function() {
            let currentValue = parseInt(quantityInput.value) || 1;
            let minValue = parseInt(quantityInput.min) || 1;
            if (currentValue > minValue) {
                quantityInput.value = currentValue - 1;
                updateQuantity();
            }
        });
        
        increaseBtn.addEventListener('click', function() {
            let currentValue = parseInt(quantityInput.value) || 1;
            let maxValue = parseInt(quantityInput.max) || 999;
            if (currentValue < maxValue) {
                quantityInput.value = currentValue + 1;
                updateQuantity();
            }
        });
        
        quantityInput.addEventListener('change', function() {
            updateQuantity();
        });
    }
    
    // Thumbnail Bilder Funktionalität
    const thumbnailImages = document.querySelectorAll('.thumbnail-image');
    const mainImage = document.querySelector('.main-image .image-placeholder');
    
    thumbnailImages.forEach(function(thumbnail, index) {
        thumbnail.addEventListener('click', function() {
            // Entferne active Klasse von allen Thumbnails
            thumbnailImages.forEach(function(img) {
                img.classList.remove('active');
            });
            
            // Füge active Klasse zum geklickten Thumbnail hinzu
            thumbnail.classList.add('active');
            
            // Update main image (hier würde normalerweise das Bild gewechselt)
            if (mainImage) {
                const icon = thumbnail.querySelector('i');
                if (icon) {
                    const iconClass = icon.className;
                    mainImage.innerHTML = `<i class="${iconClass}" style="font-size: 6rem; color: var(--color-medium-gray);"></i>`;
                }
            }
        });
    });
    
    // Warenkorb Button Funktionalität
    const addToCartBtn = document.querySelector('.add-to-cart-btn');
    
    if (addToCartBtn) {
        addToCartBtn.addEventListener('click', function() {
            const quantity = quantityInput ? parseInt(quantityInput.value) || 1 : 1;
            
            // Zeige Feedback
            const originalText = addToCartBtn.innerHTML;
            addToCartBtn.innerHTML = '<i class="bi bi-check"></i> Hinzugefügt!';
            addToCartBtn.classList.add('btn-success');
            addToCartBtn.classList.remove('btn-primary');
            
            // Update Warenkorb Badge in Navigation
            updateCartBadge(quantity);
            
            // Zurück zum ursprünglichen Zustand nach 2 Sekunden
            setTimeout(function() {
                addToCartBtn.innerHTML = originalText;
                addToCartBtn.classList.remove('btn-success');
                addToCartBtn.classList.add('btn-primary');
            }, 2000);
        });
    }
    
    // Wishlist Button Funktionalität
    const wishlistBtn = document.querySelector('.wishlist-btn');
    
    if (wishlistBtn) {
        wishlistBtn.addEventListener('click', function() {
            const icon = wishlistBtn.querySelector('i');
            const text = wishlistBtn.querySelector('span') || wishlistBtn.childNodes[wishlistBtn.childNodes.length - 1];
            
            if (icon.classList.contains('bi-heart')) {
                icon.classList.remove('bi-heart');
                icon.classList.add('bi-heart-fill');
                wishlistBtn.classList.add('btn-outline-danger');
                wishlistBtn.classList.remove('btn-outline-secondary');
                if (text && text.textContent) {
                    text.textContent = ' Gemerkt';
                }
            } else {
                icon.classList.remove('bi-heart-fill');
                icon.classList.add('bi-heart');
                wishlistBtn.classList.remove('btn-outline-danger');
                wishlistBtn.classList.add('btn-outline-secondary');
                if (text && text.textContent) {
                    text.textContent = ' Merken';
                }
            }
        });
    }
    
});

// Hilfsfunktionen
function updateQuantity() {
    const quantityInput = document.getElementById('quantity');
    if (quantityInput) {
        let value = parseInt(quantityInput.value) || 1;
        let min = parseInt(quantityInput.min) || 1;
        let max = parseInt(quantityInput.max) || 999;
        
        // Wert in gültigen Bereich halten
        if (value < min) value = min;
        if (value > max) value = max;
        
        quantityInput.value = value;
        
        // Hier könnten Preisberechnungen oder andere Updates stattfinden
        console.log('Menge aktualisiert auf:', value);
    }
}

function updateCartBadge(addedQuantity) {
    const cartBadge = document.querySelector('.nav-link .badge');
    if (cartBadge) {
        let currentCount = parseInt(cartBadge.textContent) || 0;
        cartBadge.textContent = currentCount + addedQuantity;
        
        // Animation für das Badge
        cartBadge.style.transform = 'scale(1.3)';
        setTimeout(function() {
            cartBadge.style.transform = 'scale(1)';
        }, 200);
    }
}

// Smooth scrolling für interne Links
document.addEventListener('DOMContentLoaded', function() {
    const links = document.querySelectorAll('a[href^="#"]');
    
    links.forEach(function(link) {
        link.addEventListener('click', function(e) {
            const targetId = this.getAttribute('href').substring(1);
            const targetElement = document.getElementById(targetId);
            
            if (targetElement) {
                e.preventDefault();
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
});
