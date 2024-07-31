const productStorage = new ProductStorage();

$('body').on('click', function () {
    $('.autocomplete-items').each(function() {
        $(this).empty();
    })
});

const clearSearchResult = () => {
    $('#autocomplete-list').empty();
}

$('#search').on('input', async function(e) {
    clearSearchResult();

    const value = e.target.value;

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
        $(`#autocomplete-list`).append(item);
    }
})

function isCustomerTakeYourSelfChange () {
    if($('#shippingLocation').val() == "1") {
        $('#shippingLocationInfo').css({'display': 'block'});
    } else {
        $('#shippingLocationInfo').css({'display': 'none'});
    }
}

function checkCustomerHasBoughtSomething (e) {
    fetch(GET_BASIC_CUSTOMER_INFO(e.target.value))
    .then(res => res.json())
    .then(data => {
        if(data.found) {
            $('#customerInfoContainer').html('');
            $('#customerInfoContainer').html(data.customer);
        }
    })
}

function isSameAsCustomerAddressChange() {
    const isChecked = $('#isSameAsCustomerAddress').is(':checked');

    if(isChecked) {
        $('#shippingAddress').css({'display': 'none'});
    } else {
        $('#shippingAddress').css({'display': 'block'});
    }
}

function updateItemAndPaymentView(data) {
    $('#orderItemContainer').html('');
    $('#orderPaymentInfoContainer').html('');

    $('#orderItemContainer').html(data.items);
    $('#orderPaymentInfoContainer').html(data.payment);
}

function updateAllView(data) {
    $('#orderItemContainer').html('');
        $('#customerInfoContainer').html('');
        $('#shippingInfoContainer').html('');
        $('#orderPaymentInfoContainer').html('');
        $('#orderButtonActionContainer').html('');
        // show new data
        $('#orderItemContainer').html(data.items);
        $('#customerInfoContainer').html(data.customer);
        $('#shippingInfoContainer').html(data.shipping);
        $('#orderPaymentInfoContainer').html(data.payment);
        $('#orderButtonActionContainer').html(data.buttons);
}

function show(id) {
    fetch(SHOW_ORDER_API(id))
    .then(res => res.json())
    .then(data => {
        updateAllView(data);
        $('#shippingLocation').val(data.isCustomerTakeYourSelf ? '0' : '1');
        $('#orderStatus').val(data.status.toString());
        $('#shippingLocation').trigger('change');
        $('#orderStatus').trigger('change');
    });
}

function handleItemSelect (id, name, price, image) {
    clearSearchResult();

    fetch(ORDER_ADD_ITEM, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            id: id,
            productName: name,
            quantity: 1,
            productImage: image,
            price: price
        })
    })
    .then(res => res.json())
    .then(data => updateItemAndPaymentView(data));
};

function updateQuantity(event, id) {
    const value = parseInt(event.target.value);

    if(value < 1) return;

    fetch(ORDER_UPDATE_ITEM_QUANTITY(id, value), {
        method: 'PATCH'
    })
    .then(res => res.json())
    .then(data => updateItemAndPaymentView(data));
}

function removeItem(id) {
    fetch(ORDER_REMOVE_ITEM(id), {
        method: 'DELETE'
    })
    .then(res => res.json())
    .then(data => updateItemAndPaymentView(data));
}

function checkout() {
    const customerInfo = {
        name: $('#customerName').val(),
        phoneNumber: $('#customerPhoneNumber').val(),
        address: $('#customerAddress').val(),
    };

    const shippingInfo = {
        name: $('#receiverName').val(),
        phoneNumber: $('#receiverPhoneNumber').val(),
        address: $('#receiverAddress').val()
    }

    const payload = {
        isCustomerTakeYourSelf: $('#shippingLocation').val() === "0",
        isShippingAddressSameAsCustomerAddress: $('#isSameAsCustomerAddress').is(':checked'),
        status: $('#orderStatus').val(),
        customerInfo,
        shippingInfo
    }

    fetch(ORDER_CHECKOUT_API, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
    }).then(res => res.json())
    .then(data => updateAllView(data))
    .then(_ => toastr.success("Tạo thành công"));
}

function update() {
    const shippingInfo = {
        name: $('#receiverName').val(),
        phoneNumber: $('#receiverPhoneNumber').val(),
        address: $('#receiverAddress').val()
    }

    const payload = {
        isCustomerTakeYourSelf: $('#shippingLocation').val() === "0",
        isShippingAddressSameAsCustomerAddress: $('#isSameAsCustomerAddress').is(':checked'),
        status: $('#orderStatus').val(),
        shippingInfo
    }

    fetch(ORDER_UPDATE_API, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
    }).then(res => res.json())
    .then(data => updateAllView(data))
    .then(_ => toastr.success("Cập nhật thành công"));
}

function clearOrder() {
    fetch(ORDER_CLEAR_API, {
        method: 'DELETE'
    })
    .then(res => res.json())
    .then(data => updateAllView(data));
}

function saveTempOrder() {
    const customerInfo = {
        name: $('#customerName').val(),
        phoneNumber: $('#customerPhoneNumber').val(),
        address: $('#customerAddress').val(),
    };

    const shippingInfo = {
        name: $('#receiverName').val(),
        phoneNumber: $('#receiverPhoneNumber').val(),
        address: $('#receiverAddress').val()
    }

    const payload = {
        isCustomerTakeYourSelf: $('#shippingLocation').val() === "0",
        isShippingAddressSameAsCustomerAddress: $('#isSameAsCustomerAddress').is(':checked'),
        status: $('#orderStatus').val(),
        customerInfo,
        shippingInfo
    }


    fetch(ORDER_TEMP_SAVE_API, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
    })
    .then(res => res.json())
    .then(data => {
        $('#orderTempSaveContainer').html('');
        $('#orderTempSaveContainer').html(data.tempOrders);
    })
    .then(_ => toastr.success("Lưu thành công!"))
    .then(_ => clearOrder());
}