const productStorage = new ProductStorage();

$('body').on('click', function () {
    $('.autocomplete-items').each(function () {
        $(this).empty();
    })
});

const clearSearchResult = () => {
    $('#autocomplete-list').empty();
}

$('#search').on('input', async function (e) {
    clearSearchResult();

    const value = e.target.value;

    const products = await productStorage.filter(value);

    for (let i = 0; i < products.length; i++) {
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

function isCustomerTakeYourSelfChange() {
    if ($('#shippingLocation').val() == "1") {
        $('#shippingLocationInfo').css({ 'display': 'block' });
    } else {
        $('#shippingLocationInfo').css({ 'display': 'none' });
    }
}

function checkCustomerHasBoughtSomething(e) {
    fetch(GET_BASIC_CUSTOMER_INFO(e.target.value))
        .then(res => res.json())
        .then(data => {
            if (data.found) {
                $('#customerInfoContainer').html('');
                $('#orderPaymentInfoContainer').html('');
                $('#customerInfoContainer').html(data.customer);
                $('#orderPaymentInfoContainer').html(data.payment);
            }
        })
}

function isSameAsCustomerAddressChange() {
    const isChecked = $('#isSameAsCustomerAddress').is(':checked');

    if (isChecked) {
        $('#shippingAddress').css({ 'display': 'none' });
    } else {
        $('#shippingAddress').css({ 'display': 'block' });
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
            $('#isSameAsCustomerAddress').trigger('change');
        });
}

function handleItemSelect(id, name, price, image) {
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

    if (value < 1) return;

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

function verify() {
    let isValid = true;
    const displayErrorMessage = { 'display': 'block' };
    const hideErrorMessage = { 'display': 'none' };

    // validate item have in cart
    const items = $('#orderItemContainer table tbody tr').length;
    if (items === 0) {
        alert("Giỏ hàng đang không có sản phẩm nào!");
        return;
    }

    // validate customer info
    const customerInfoField = {
        '#customerName': '#order-customer-name',
        '#customerPhoneNumber': '#order-customer-phone',
        '#customerAddress': '#order-customer-address'
    };

    for (const [key, value] of Object.entries(customerInfoField)) {
        if ($(key).val() === "") {
            isValid = false;
            $(value).css(displayErrorMessage);
        } else {
            $(value).css(hideErrorMessage);
        }
    }

    // validate shipping info
    if ($('#shippingLocation').val() == "1" && $('#isSameAsCustomerAddress').is(':checked') == false) {
        const shippingField = {
            '#receiverName': '#receiverNameError',
            '#receiverPhone': '#receiverPhoneError',
            '#receiverAddress': '#receiverAddressError'
        }

        for (const [key, value] of Object.entries(shippingField)) {
            if ($(key).val() === "") {
                isValid = false;
                $(value).css(displayErrorMessage);
            } else {
                $(value).css(hideErrorMessage);
            }
        }
    }

    return isValid;
}

function checkout() {
    if (!verify()) return;

    const customerInfo = {
        name: $('#customerName').val(),
        phoneNumber: $('#customerPhoneNumber').val(),
        address: $('#customerAddress').val(),
    };

    const shippingInfo = {
        name: $('#receiverName').val(),
        phoneNumber: $('#receiverPhone').val(),
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
        phoneNumber: $('#receiverPhone').val(),
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
    if (!verify()) return;

    const customerInfo = {
        name: $('#customerName').val(),
        phoneNumber: $('#customerPhoneNumber').val(),
        address: $('#customerAddress').val(),
    };

    const shippingInfo = {
        name: $('#receiverName').val(),
        phoneNumber: $('#receiverPhone').val(),
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

function removeDraft(id) {
    const isRemove = confirm("Bạn có chắc muốn xóa?")
    if (!isRemove) return;

    fetch(REMOVE_ORDER_TEMP_API(id), {
        method: 'DELETE'
    })
        .then(res => res.json())
        .then(data => {
            $('#orderTempSaveContainer').html('');
            $('#orderTempSaveContainer').html(data.tempOrders);
        });
}

function showAvailableVoucher() {
    const customerPhone = $('#customerPhoneNumber').val();

    fetch(GET_AVAILABLE_VOUCHER(customerPhone))
        .then(res => res.json())
        .then(data => {
            $('#voucher-list').html('');
            $('#voucher-list').html(data.vouchers);
        })
}

function applyVoucher(id) {
    fetch(APPLY_VOUCHER(id))
        .then(res => res.json())
        .then(data => {
            $('#orderPaymentInfoContainer').html('');
            $('#orderPaymentInfoContainer').html(data.payment);
            toastr.success("Áp dụng mã khuyến mãi thành công!");
            $('#voucherModal').modal('hide');
        }).catch(err => alert("Mã khuyến mãi không hợp lệ"));
}