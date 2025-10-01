// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

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
        console.log('Quantity updated to:', value);
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
