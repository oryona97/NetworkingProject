// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Navbar scroll effect
window.addEventListener('scroll', function() {
	const navbar = document.querySelector('.navbar');
	if (window.scrollY > 50) {
		navbar.classList.add('scrolled');
	} else {
		navbar.classList.remove('scrolled');
	}
});

// Cart counter animation
function updateCartCount(count) {
	const cartCount = document.querySelector('.cart-count');
	cartCount.textContent = count;
	cartCount.classList.add('animate__animated', 'animate__bounce');
	setTimeout(() => {
		cartCount.classList.remove('animate__animated', 'animate__bounce');
	}, 1000);
}
