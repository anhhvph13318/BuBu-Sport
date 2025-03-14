
document.addEventListener('DOMContentLoaded', function() {
    const mainImage = document.querySelector('.gallery-main img');
    const thumbnails = document.querySelectorAll('.thumbnail-item input[type="radio"]');
    
    let originalImageSrc = mainImage.src;

    thumbnails.forEach(thumb => {
        const thumbImg = thumb.nextElementSibling.querySelector('img');
        
        // Change on hover
        thumbImg.addEventListener('mouseenter', () => {
            originalImageSrc = mainImage.src;
            mainImage.src = thumbImg.src;
        });

        // Revert on mouse leave
        thumbImg.addEventListener('mouseleave', () => {
            mainImage.src = originalImageSrc;
        });

        // Change permanently on click
        thumb.addEventListener('change', () => {
            originalImageSrc = thumbImg.src;
            mainImage.src = thumbImg.src;
        });
    });

    // Color selection with auto-changing images
    const colorInputs = document.querySelectorAll('.swatches__form--input');
    const selectedColorText = document.getElementById('selected-option-1');
    
    // Define image sets for each color
    const colorImages = {
        'White': [
            'https://static.nike.com/a/images/t_PDP_1728_v1/f_auto,q_auto:eco,u_126ab356-44d8-4a06-89b4-fcdcc8df0245,c_scale,fl_relative,w_1.0,h_1.0,fl_layer_apply/288a2235-54ce-4f8e-a133-0117cbc381b4/AIR+JORDAN+1+MID.png',
            'https://static.nike.com/a/images/t_PDP_1728_v1/f_auto,q_auto:eco,u_126ab356-44d8-4a06-89b4-fcdcc8df0245,c_scale,fl_relative,w_1.0,h_1.0,fl_layer_apply/d48d9a72-3123-49c7-893f-e3cb60be0a8c/AIR+JORDAN+1+MID.png',
            'https://static.nike.com/a/images/t_PDP_1728_v1/f_auto,q_auto:eco,u_126ab356-44d8-4a06-89b4-fcdcc8df0245,c_scale,fl_relative,w_1.0,h_1.0,fl_layer_apply/a7d138bb-f05b-4708-9f1f-ccf5a6d180d2/AIR+JORDAN+1+MID.png',
            'https://static.nike.com/a/images/t_PDP_1728_v1/f_auto,q_auto:eco,u_126ab356-44d8-4a06-89b4-fcdcc8df0245,c_scale,fl_relative,w_1.0,h_1.0,fl_layer_apply/852a384c-a6ef-45af-9bf7-2ed47d64ae4f/AIR+JORDAN+1+MID.pngs'
        ],
        'Brown': [
            'https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/54a2f75f-144f-4921-8503-f8233ce11c5f/air-jordan-1-mid-shoes-SQf7DM.png',
            'https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/1eeaacc0-e07c-4024-a5f7-57f2fd395b2d/air-jordan-1-mid-shoes-SQf7DM.png',
            'https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/0ac482bd-f8e4-46cc-9d6e-5e6b43667926/air-jordan-1-mid-shoes-SQf7DM.png',
            'https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/5d61e6d5-c098-4557-b9fa-ca8275fba184/air-jordan-1-mid-shoes-SQf7DM.png'
        ],
        'Green': [
            'https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/0ac482bd-f8e4-46cc-9d6e-5e6b43667926/air-jordan-1-mid-shoes-SQf7DM.png',
            'https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/54a2f75f-144f-4921-8503-f8233ce11c5f/air-jordan-1-mid-shoes-SQf7DM.png',
            'https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/1eeaacc0-e07c-4024-a5f7-57f2fd395b2d/air-jordan-1-mid-shoes-SQf7DM.png',
            'https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/5d61e6d5-c098-4557-b9fa-ca8275fba184/air-jordan-1-mid-shoes-SQf7DM.png'
        ],
        'Yellow': [
            'https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/5d61e6d5-c098-4557-b9fa-ca8275fba184/air-jordan-1-mid-shoes-SQf7DM.png',
            'https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/54a2f75f-144f-4921-8503-f8233ce11c5f/air-jordan-1-mid-shoes-SQf7DM.png',
            'https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/0ac482bd-f8e4-46cc-9d6e-5e6b43667926/air-jordan-1-mid-shoes-SQf7DM.png',
            'https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/1eeaacc0-e07c-4024-a5f7-57f2fd395b2d/air-jordan-1-mid-shoes-SQf7DM.png'
        ]
    };
    
    function updateImages(color) {
        const images = colorImages[color];
        // Update carousel images
        mainCarousel.cells.forEach((cell, index) => {
            const img = cell.element.querySelector('img');
            img.src = images[index];
        });
        
        // Update thumbnails
        const thumbnails = document.querySelectorAll('.js-thumb-item-img');
        thumbnails.forEach((thumb, index) => {
            thumb.src = images[index];
        });
    }

    colorInputs.forEach(input => {
        input.addEventListener('change', () => {
            selectedColorText.textContent = input.value;
            updateImages(input.value);
            mainCarousel.select(0);
        });
    });

    // Size selection
    const sizeButtons = document.querySelectorAll('.size-buttons button');
    sizeButtons.forEach(button => {
        button.addEventListener('click', () => {
            // Remove selected class from all buttons
            sizeButtons.forEach(btn => btn.style.background = '');
            sizeButtons.forEach(btn => btn.style.color = '');
            
            // Add selected class to clicked button
            button.style.background = '#000';
            button.style.color = '#fff';
        });
    });

    // Add to cart functionality
    const addToCartButton = document.querySelector('.add-to-cart');
    addToCartButton.addEventListener('click', () => {
        alert('Item added to cart!');
    });
});
