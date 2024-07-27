const productStorage = new ProductStorage();
const orderStorage = new OrderStorage();

const clearSearchResult = (_, orderNumber) => {
    if(orderNumber === undefined) return;

    $(`#autocomplete-list-${orderNumber}`).empty();
}

$('body').on('click', function () {
    $('.autocomplete-items').each(function() {
        $(this).empty();
    })
});

// 18/07/2024
async function search (e, orderNumber) {
    clearSearchResult(orderNumber);

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
            handleItemSelect(id, name, price, image, orderNumber);
        };

        $(item).append(itemContent);
        $(`#autocomplete-list-${orderNumber}`).append(item);
    }
}

async function checkout(orderNumber) {
    const order = orderStorage.getOrder(orderNumber);

    if(order.items.length === 0) {
        alert("Phải có ít nhất 1 sản phẩm trong giỏ hàng");
        return;
    }

    // if($(`#customerName-${orderNumber}`).val() === '') {
    //     const key = `#order-${orderNumber}-customer-name`;
    //     $(`#order-${orderNumber}-customer-name`).css({'display': 'block'});
    //     return;
    // }

    // if($(`#customerPhoneNumber-${orderNumber}`).val() === '') {
    //     $(`#order-${orderNumber}-customer-phone`).css({'display': 'block'});
    //     return;
    // }

    // if($(`#customerAddress-${orderNumber}`).val() === '') {
    //     $(`#order-${orderNumber}-customer-address`).css({'display': 'block'});
    //     return;
    // }

    const res = await fetch(ORDER_SUBMIT_API, {
        headers: {
            'Content-Type': 'application/json'
        },
        method: 'POST',
        body: JSON.stringify(order)
    });

    if (res.status === 204) {
        alert("Tạo thành công");
        orderStorage.dropOrder(orderNumber);
        $(`#order-${orderNumber}-tab`).remove();
        $(`#order-${orderNumber}`).remove();

        if(orderStorage.isEmpty()) {
            addNewEmptyOrder();
            $(`#order-0-tab`).click();
            return;
    }

        const nextOrder = orderStorage.getNextOrderNumber();
        $(`#order-${nextOrder}-tab`).click();
}
}

function customerAreaInputChange(e, orderNumber) {
    const order = orderStorage.getOrder(orderNumber);
    order.updateCustomerInfo(e);
}

function generateCustomerArea(orderNumber) {
    return `
        <div class="p-3 border mb-3">
        <h5>Thông tin khách hàng</h5>
            <div class="row">
                <div class="mb-2 col-4">
                    <label>Tên</label>
                    <input class="form-control" id="customerName-${orderNumber}" name="name" type="text" onchange="customerAreaInputChange(event, ${orderNumber})"/>
                    <p class="text-danger" style="display:none" id="order-${orderNumber}-customer-name">Trường này là bắt buộc</p>
                </div>
                <div class="mb-2 col-4">
                    <label>Số điện thoại</label>
                    <input class="form-control" id="customerPhoneNumber-${orderNumber}" name="phoneNumber" type="text" onchange="customerAreaInputChange(event, ${orderNumber})"/>
                    <p class="text-danger" style="display:none" id="order-${orderNumber}-customer-phone">Trường này là bắt buộc</p>
                </div>
                <div class="mb-2 col-4">
                    <label>Địa chỉ</label>
                    <input class="form-control" id="customerAddress-${orderNumber}" name="address" type="text" onchange="customerAreaInputChange(event, ${orderNumber})"/>
                    <p class="text-danger" style="display:none" id="order-${orderNumber}-customer-address">Trường này là bắt buộc</p>
                </div>
            </div>
        </div>
    `;
}

function generateCheckoutArea(orderNumber) {
    const order = orderStorage.getOrder(orderNumber);

    return `
        <div class="d-flex justify-content-end">
            <div class="me-3">
                <p>Khuyến mãi: <span id="discount-amount-${orderNumber}">${order.discountAmount.toLocaleString('vi', {style : 'currency', currency : 'VND'})}</span></p>
                <p>Tổng: <span id="total-amount-${orderNumber}">${order.totalAmount.toLocaleString('vi', {style : 'currency', currency : 'VND'})}</span></p>
            </div>
            <button class="btn btn-primary" onclick="checkout(${orderNumber})">Thanh toán</button>
        </div>
    `
}

function generateSearchForm(orderNumber) {
    const form = document.createElement('form');
    const container = document.createElement('div');
    const autoCompleteContainer = document.createElement('div');
    const inputSearch = document.createElement('input');

    form.setAttribute('autocomplete', 'off');
    form.setAttribute('class', 'w-100');
    form.setAttribute('onsubmit', 'return false');

    container.setAttribute('class', 'mb-3 autocomplete w-100');

    autoCompleteContainer.setAttribute('class', 'autocomplete-items');
    autoCompleteContainer.setAttribute('id', `autocomplete-list-${orderNumber}`);

    inputSearch.setAttribute('placeholder', 'Tên, mã sản phẩm...');
    inputSearch.setAttribute('class', 'form-control');

    inputSearch.oninput = async function(e) {
        await search(e, orderNumber);
    }

    container.append(inputSearch);
    container.append(autoCompleteContainer);
    form.append(container);

    return form;
}

function generateTableHeader() {
    return `
        <thead>
            <tr>
                <th>STT</th>
                <th>Ảnh</th>
                <th>Tên sản phẩm</th>
                <th>Đơn giá</th>
                <th>Số lượng</th>
                <th></th>
            </tr>
        </thead>
    `;
}

function generateTableBody(order) {
    let rows = ''

    for(let i = 0; i < order.items.length; i++) {
        const inputQuantity = document.createElement('input');
        inputQuantity.setAttribute('class', 'form-control');

        rows += `
            <tr>
                <td>${i + 1}</td>
                <td style="width: 15%">
                    <img src="${order.items[i].image}" class="img-thumbnail" style="width: 50%"/>
                </td>
                <td>${order.items[i].name}</td>
                <td>${order.items[i].price.toLocaleString('vi', {style : 'currency', currency : 'VND'})}</td>
                <td>
                    <input class="form-control" value="${order.items[i].quantity}" type="number" onchange="handleInputChange(event, '${order.items[i].id.toString()}', ${order.tempId})"/>
                    <p class="text-danger" id="order-${order.tempId}-${order.items[i].id}"></p>
                </td>
                <td>
                    <button class="btn btn-danger" onclick="handleRemoveItem(${order.tempId}, '${order.items[i].id}')">Xoá</button>
                </td>
            </tr>
        `;
    }

    return rows;
}

function generateTableItem(orderNumber) {
    const order = orderStorage.getOrder(orderNumber);
    // createEmptyTable
    if (order.items.length === 0) {
        const title = document.createElement('p');
        title.setAttribute('class', 'text-center m-3');
        title.setAttribute('id', `empty-order-${orderNumber}-message`);
        title.innerText = "Giỏ hàng trống"
        return title;
    }

    const table = document.createElement('table');
    const body = document.createElement('tbody');
    table.setAttribute('class', 'table');
    body.setAttribute('id', `order-table-body-${orderNumber}`)

    $(body).append(generateTableBody(order));
    $(table).append(generateTableHeader());
    $(table).append(body);

    return table;
}

function generateOrderViewContent(orderNumber) {
    const container = document.createElement('div');
    const tableArea = document.createElement('div');
    const checkoutArea = document.createElement('div');

    container.setAttribute('class', 'tab-pane fade');
    container.setAttribute('id', `order-${orderNumber}`);
    container.setAttribute('role', 'tabpanel');
    container.setAttribute('aria-labelledby', `order-${orderNumber}-tab`);
    container.setAttribute('tabindex', orderNumber.toString());

    tableArea.setAttribute('id', `table-area-${orderNumber}`);
    checkoutArea.setAttribute('id', `checkout-area-${orderNumber}`);

    $(tableArea).append(generateTableItem(orderNumber));
    $(checkoutArea).append(generateCheckoutArea(orderNumber));

    $(container)
        .append(generateCustomerArea(orderNumber))
        .append(generateSearchForm(orderNumber))
        .append(tableArea)
        .append(checkoutArea);

    return container;
}

function reRenderTableBody(orderNumber) {
    const order = orderStorage.getOrder(orderNumber);
    $(`#empty-order-${orderNumber}-message`).remove();

    if($(`#order-table-body-${orderNumber}`).length) {
        $(`#order-table-body-${orderNumber}`).empty();
        $(`#order-table-body-${orderNumber}`).append(generateTableBody(order));
    }
    else {
        $(`#table-area-${orderNumber}`).append(generateTableItem(orderNumber));
    }
}

function reRenderCheckoutArea(orderNumber) {
    const order = orderStorage.getOrder(orderNumber);
    $(`#discount-amount-${orderNumber}`).text(order.discountAmount.toLocaleString('vi', {style : 'currency', currency : 'VND'}));
    $(`#total-amount-${orderNumber}`).text(order.totalAmount.toLocaleString('vi', {style : 'currency', currency : 'VND'}));
    $
}

function handleRemoveItem (orderNumber, id) {
    const order = orderStorage.getOrder(orderNumber);
    order.removeItem(id);
    updateOrderPrice();
    reRenderTableBody(orderNumber);
    reRenderCheckoutArea(orderNumber);
}

async function handleInputChange (e, id, orderNumber) {
    const quantity = parseInt(e.target.value);
    const order = orderStorage.getOrder(orderNumber);
    const product = await productStorage.getProductStock(id);
    const elementKey = `#order-${orderNumber}-${id}`;
    const message = `Chỉ còn ${product.stock} trong kho.`;

    if(product.stock < quantity) {
        $(elementKey).css({ 'display': 'block' });
        $(elementKey).text(message)
        order.pushError(elementKey, message);
        return;
    }
    
    order.updateQuantity(id, quantity);
    reRenderCheckoutArea(orderNumber);
    $(elementKey).css({ 'display': 'none' });
    order.dropError(elementKey);
}

function handleItemSelect (id, name, price, image, orderNumber) {
    clearSearchResult(orderNumber);
    const order = orderStorage.getOrder(orderNumber);
    order.addItem(id, 1, price, name, image);
    reRenderTableBody(orderNumber);
    reRenderCheckoutArea(orderNumber);
};

function addNewEmptyOrder() {
    const nextOrderNumber = orderStorage.getNextOrderNumber();
    if(nextOrderNumber >= 10) return;
    orderStorage.addOrder(nextOrderNumber);
    $('#order-tab').append(`<button class="nav-link nav-order-button" id="order-${nextOrderNumber}-tab" data-bs-toggle="tab" data-bs-target="#order-${nextOrderNumber}" type="button" role="tab" aria-controls="order-${nextOrderNumber}" aria-selected="true">Giỏ hàng ${nextOrderNumber + 1}</button>`)
    $('#order-tabContent').append(generateOrderViewContent(nextOrderNumber));
}

$('#newCartAddBtn').on('click', addNewEmptyOrder);

$('#search-0').on('input', async function(e) {
    await search(e, 0);
})