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

const handleRemoveItem = (item, id) => {
    $(item).remove();
    order.removeItem(id);
    updateOrderPrice();
}

const handleInputChange = (id, quantity) => {
    if (quantity < 1) return;
    const element = `#${id}-value`;

    $(element).val(quantity);

    order.updateQuantity(id, quantity) && $(element).val(quantity);
    updateOrderPrice();
}

const handleIncreaseClick = (id) => {
    const element = `#${id}-value`;
    let value = parseInt($(element).val());

    value++;

    $(element).val(value);
    order.updateQuantity(id, value);
    updateOrderPrice();
}

const handleReduceClick = (id) => {
    const element = `#${id}-value`;
    let value = parseInt($(element).val());

    if(value <= 1) return;

    value--;

    $(element).val(value);
    order.updateQuantity(id, value);
    updateOrderPrice();
}


const handleItemSelect = (id, name, price, image) => {
    clearSearchResult();

    const isSame = order.addItem(id, 1, price);
    updateOrderPrice();

    if(isSame) {
        handleIncreaseClick(id);
        return;
    }

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
        handleReduceClick(id);
    }

    increaseButton.innerText = '+';
    increaseButton.setAttribute('class', 'btn btn-primary');
    increaseButton.onclick = function () {
        handleIncreaseClick(id);
    }

    deleteButton.innerText = 'Xoá';
    deleteButton.setAttribute('class', 'btn btn-danger');
    deleteButton.onclick = function () {
        handleRemoveItem(item, id);
    }

    input.setAttribute('class', 'form-control');
    input.setAttribute('value', 1);
    input.setAttribute('id', `${id}-value`);
    input.onblur = function (e) {
        const value =parseInt(e.target.value);
        handleInputChange(id, value);
    };

    imageContainer.setAttribute('class', 'col-2');
    $(imageContainer).append(`<img class="img-thumbnail" src=${image} alrt="" />`);

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

    item.setAttribute('class', 'row mb-3 p-1 border align-items-center cart-item');
    item.setAttribute('id', id);
    item.append(imageContainer);
    item.append(productContainer);
    item.append(quantityContainer);
    item.append(deleteContainer);

    $('#cart-container').append(item);
};

const resetData = () => {
    order.initialize();

    $('#customerName').val('');
    $('#customerAddress').val('');
    $('#customerPhoneNumber').val('');
    
    $('#amount').text('');
    $('#totalAmount').text('');
    $('#discountAmount').text('');

    $('.cart-item').remove();
}

$('body').on('click', clearSearchResult);

$('#productSearch').on('input', async function (e) {
    clearSearchResult();

    const value = e.target.value;
    if(value === undefined || value === "") return;

    const products = await productStorage.filter(value);

    for(let i = 0; i < products.length; i++) {
        const item = document.createElement('a');
        item.setAttribute('class', 'text-decoration-none');
        const itemContent = `
            <div class="d-flex">
                <img class="img-thumbnail" style="min-width: 100px; height: 50px" src=${products[i].image} alrt=""/>
                <p class="fs-bold ms-3 text-decoration-none text-black nav-link">${products[i].name}</p>
            </div>
            
        `;
        
        item.onclick = function () {
            const { id, name, price, image } = products[i]
            handleItemSelect(id, name, price, image);
        };

        $(item).append(itemContent);
        $('#autocomplete-list').append(item);
    }
});

$('#customerName').on('blur', function (e) {
    order.customer.name = e.target.value;
});

$('#customerPhoneNumber').on('blur', function (e) {
    order.customer.phoneNumber = e.target.value;
});

$('#customerAddress').on('blur', function (e) {
    order.customer.address = e.target.value;
});

$('#createOrder').on('click', async function() {
    const res = await fetch(ORDER_SUBMIT_API, {
        headers: {
            'Content-Type': 'application/json'
        },
        method: 'POST',
        body: JSON.stringify(order)
    });

    if (res.status === 204) {
        alert("Tạo thành công");
    }
});

$('#reset').on('click', function () {
    resetData();
});