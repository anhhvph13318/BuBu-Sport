const productStorage = new ProductStorage();
const order = new Order();

const clearSearchResult = () => {
    $('#autocomplete-list').empty();
}

const updateOrderPrice = () => {
    const { amount, totalAmount, discountAmount} = order.reCalculateOrderPrice();

    $('#amount').text(amount.toString());
    $('#totalAmount').text(totalAmount.toString());
    $('#discountAmount').text(discountAmount.toString());
}

const handleRemoveItem = (item, productId) => {
    $(item).remove();
    order.removeItem(productId);
    updateOrderPrice();
}

const handleInputChange = (productId, quantity) => {
    if (quantity < 1) return;
    const element = `#${productId}-value`;

    $(element).val(quantity);

    order.updateQuantity(productId, quantity) && $(element).val(quantity);
    updateOrderPrice();
}

const handleIncreaseClick = (productId) => {
    const element = `#${productId}-value`;
    let value = parseInt($(element).val());

    value++;

    $(element).val(value);
    order.updateQuantity(productId, value);
    updateOrderPrice();
}

const handleReduceClick = (productId) => {
    const element = `#${productId}-value`;
    let value = parseInt($(element).val());

    if(value <= 1) return;

    value--;

    $(element).val(value);
    order.updateQuantity(productId, value);
    updateOrderPrice();
}


const handleItemSelect = (productId, name, price) => {
    clearSearchResult();

    const item = document.createElement('div');
    const reduceButton = document.createElement('button');
    const increaseButton = document.createElement('button');
    const deleteButton = document.createElement('button');
    const input = document.createElement('input');
    const imageContainer = document.createElement('div');
    const productContainer = document.createElement('div');
    const quantityContainer = document.createElement('div');
    const deleteContainer = document.createElement('div');

    reduceButton.innerText = '-';
    reduceButton.setAttribute('class', 'btn btn-primary');
    reduceButton.onclick = function () {
        handleReduceClick(productId);
    }

    increaseButton.innerText = '+';
    increaseButton.setAttribute('class', 'btn btn-primary');
    increaseButton.onclick = function () {
        handleIncreaseClick(productId);
    }

    deleteButton.innerText = 'Xoá';
    deleteButton.setAttribute('class', 'btn btn-danger');
    deleteButton.onclick = function () {
        handleRemoveItem(item, productId);
    }

    input.setAttribute('class', 'form-control');
    input.setAttribute('value', 1);
    input.setAttribute('id', `${productId}-value`);
    input.onblur = function (e) {
        const value =parseInt(e.target.value);
        handleInputChange(productId, value);
    };

    imageContainer.setAttribute('class', 'col-2');
    $(imageContainer).append(`<img class="img-thumbnail" src=${''} alrt="" />`);

    productContainer.setAttribute('class', 'col-5');
    $(productContainer).append(`
        <div class="col-5">
            <h5>${name}</h5>
            <p>${price}</p>
        </div>`);

    quantityContainer.setAttribute('class', 'col-2 d-flex');
    quantityContainer.append(reduceButton);
    quantityContainer.append(input);
    quantityContainer.append(increaseButton);

    deleteContainer.setAttribute('class', 'col-2');
    deleteContainer.append(deleteButton);

    item.setAttribute('class', 'row mb-3 p-1 border align-items-center');
    item.setAttribute('id', productId);
    item.append(imageContainer);
    item.append(productContainer);
    item.append(quantityContainer);
    item.append(deleteContainer);

    $('#cart-container').append(item);

    order.addItem(productId, 1, price);
    updateOrderPrice();
}

$('body').on('click', clearSearchResult);

$('#productSearch').on('input', (e) => {
    clearSearchResult();

    const value = e.target.value;
    if(value === undefined || value === "") return;

    const products = productStorage.filter(value);

    for(let i = 0; i < products.length; i++) {
        const item = document.createElement('a');
        item.setAttribute('class', 'text-decoration-none');
        const itemContent = `
            <div class="d-flex">
                <img class="img-thumbnail" style="min-width: 100px;" src=${''} alrt=""/>
                <p class="fs-bold ms-3 text-decoration-none text-black nav-link">${products[i].name}</p>
            </div>
            
        `;
        
        item.onclick = function () {
            const { id, name, price } = products[i]
            handleItemSelect(id, name, price);
        };

        $(item).append(itemContent);
        $('#autocomplete-list').append(item);
    }
})
