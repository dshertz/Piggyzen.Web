// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function toggleCategory(categoryId) {
    const subcategoryList = document.getElementById(`subcategory-${categoryId}`);
    if (subcategoryList) {
        subcategoryList.style.display = subcategoryList.style.display === 'none' ? 'block' : 'none';
    }
}

function selectCategory(button) {
    const selectedCategoryId = button.getAttribute('data-category-id');
    document.getElementById("SelectedCategoryId").value = selectedCategoryId;

    // Highlight selected category
    document.querySelectorAll('.category-btn').forEach(btn => {
        btn.classList.remove('btn-primary');
        btn.classList.add('btn-outline-primary');
    });
    button.classList.remove('btn-outline-primary');
    button.classList.add('btn-primary');
}


