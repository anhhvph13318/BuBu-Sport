const productStorage = new ProductStorage();

// toast setup
toastr.options.timeOut = 3000;
toastr.options.closeButton = true;

// end toast setup

// read-only
const shippingField = {
    '#receiverName': '#receiverNameError',
    '#receiverPhone': '#receiverPhoneError',
    '#receiverAddress': '#receiverAddressError'
};

const customerInfoField = {
    '#customerName': '#order-customer-name',
    '#customerPhoneNumber': '#order-customer-phone',
    '#customerAddress': '#order-customer-address'
};

// end read-only

function backupQuantityValue(e) {
    productStorage.OldQuantity = parseInt(e.target.value);
}

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
        if(products[i].quantity <= 0) continue;

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
    const shipping = $('#shippingLocation').val();

    if (shipping == "1") {
        $('#shippingLocationInfo').css({ 'display': 'block' });
    } else {
        $('#shippingLocationInfo').css({ 'display': 'none' });
    }

    fetch(CHANGE_SHIPPING_METHOD(shipping))
        .then(res => res.json())
        .then(data => {
            $('#orderPaymentInfoContainer').html('');
            $('#orderPaymentInfoContainer').html(data.payment);
        })
        .catch(err => console.log(err));
}

function checkCustomerHasBoughtSomething(e) {
    const phone = e.target.value;

    if (phone == undefined || phone === '') return;

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
        for (const [key, value] of Object.entries(customerInfoField)) {
            $(value).css({ 'display': 'none' });
        }
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

function showOutOfStockToastMessage() {
    toastr.error("Có lỗi xảy ra! Số lượng sản phẩm không đủ")
}

function show(id) {
    fetch(SHOW_ORDER_API(id))
        .then(res => res.json())
        .then(data => {
            updateAllView(data);
            $('#tempSaveButtonContainer').html('');
            $('#tempSaveButtonContainer').html(data.tempSaveButton);
            $('#shippingLocation').val(data.isCustomerTakeYourSelf ? '0' : '1');
            $('#orderStatus').val(data.status.toString());
            $('#shippingLocation').trigger('change');
            $('#orderStatus').trigger('change');
            $('#isSameAsCustomerAddress').trigger('change');

            changeResetButtonText(true);

            if (!data.isDraft) {
                interactiveCartItemAndVoucherButton(true);
            }
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
        .then(data => updateItemAndPaymentView(data))
        .catch(_ => {
            showOutOfStockToastMessage();
        });
};

function updateQuantity(event, id) {
    const value = parseInt(event.target.value);

    if (value < 1) return;

    fetch(ORDER_UPDATE_ITEM_QUANTITY(id, value), {
        method: 'PATCH'
    })
        .then(res => res.json())
        .then(data => updateItemAndPaymentView(data))
        .catch(_ => {
            showOutOfStockToastMessage();
            event.target.value = productStorage.OldQuantity;
        });
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
        isValid = false;
        return isValid;
    }

    if($('#shippingLocation').val() != "1") return isValid;

    // validate shipping info
    if($('#isSameAsCustomerAddress').is(':checked') == false) {
        for (const [key, value] of Object.entries(shippingField)) {
            if ($(key).val().trim() === "") {
                isValid = false;
                $(value).css(displayErrorMessage);
            } else {
                $(value).css(hideErrorMessage);
            }
        }
    } else {
        for (const [key, value] of Object.entries(customerInfoField)) {
            if ($(key).val().trim() === "") {
                isValid = false;
                $(value).css(displayErrorMessage);
            } else {
                $(value).css(hideErrorMessage);
            }
        }
    }

    return isValid;
}

function clearOrder() {
    fetch(ORDER_CLEAR_API, {
        method: 'DELETE'
    })
        .then(res => res.json())
        .then(data => {
            updateAllView(data);
            interactiveCartItemAndVoucherButton(false);
            changeResetButtonText(false);
            $('#tempSaveButtonContainer').html('');
            $('#tempSaveButtonContainer').html(data.tempSaveButton);
        });
}


function saveOrder(isDraft) {
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
        shippingInfo,
        isDraft
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
            $('#orderTempSaveContainer').html(data.orders);
            $('#orderButtonActionContainer').html('');
            $('#orderButtonActionContainer').html(data.buttons);
            
            changeResetButtonText(false);
            interactiveCartItemAndVoucherButton(false);
        })
        .then(_ => toastr.success("Thành công!"))
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
            $('#orderTempSaveContainer').html(data.orders);
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

function interactiveCartItemAndVoucherButton(disable) {
    const disableAttribute = 'disabled'
    if (disable) {
        $('#applyVoucherBtn').attr(disableAttribute, true);
        $('#search').attr(disableAttribute, true);
        $('.order-item-quantity, .remove-item-btn').each(function () {
            $(this).attr(disableAttribute, true)
        });
        $('#customerName').attr(disableAttribute, true);
        $('#customerPhoneNumber').attr(disableAttribute, true);
        $('#customerAddress').attr(disableAttribute, true);
        $('#shippingLocation').attr(disableAttribute, true);
        $('#isSameAsCustomerAddress').attr(disableAttribute, true);
        $('#receiverName').attr(disableAttribute, true);
        $('#receiverPhone').attr(disableAttribute, true);
        $('#receiverAddress').attr(disableAttribute, true);
    } else {
        $('#applyVoucherBtn').removeAttr(disableAttribute);
        $('#search').removeAttr(disableAttribute);
        $('#customerName').removeAttr(disableAttribute);
        $('#customerPhoneNumber').removeAttr(disableAttribute);
        $('#customerAddress').removeAttr(disableAttribute);
        $('#shippingLocation').removeAttr(disableAttribute);
        $('#isSameAsCustomerAddress').removeAttr(disableAttribute);
        $('#receiverName').removeAttr(disableAttribute);
        $('#receiverPhone').removeAttr(disableAttribute);
        $('#receiverAddress').removeAttr(disableAttribute);
    }
}

function changeResetButtonText(isView) {
    if(isView) {
        $('#btnReset').html('Bỏ chọn');
    } else {
        $('#btnReset').html('Làm mới giỏ hàng');
    }
}

function paymentMethodChange() {
    const method = $('#paymentMethod').val()

    fetch(CHANGE_PAYMENT_METHOD(method))
        .then(res => res.json())
        .then(data => {
            $('#orderButtonActionContainer').html('');
            $('#orderButtonActionContainer').html(data.buttons)
        })
        .catch(err => console.log(err));
    }

function vnpayCheckout() {
    
    
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
        shippingInfo,
        isDraft: false
    }

    fetch(GET_VNPAY_URL, {
        headers: {
            'Content-Type': 'application/json'
        },
        method: 'POST',
        body: JSON.stringify(payload)
    })
        .then(res => res.json())
        .then(data => {
            window.open(data.url, '_blank').focus();
        }).catch(err => console.log(err));
}

function cancelApplyVoucher() {
    fetch(CANCEL_APPLY_VOUCHER, {
        method: 'POST'
    })
        .then(res => res.json())
        .then(data => {
            $('#orderPaymentInfoContainer').html('');
            $('#orderPaymentInfoContainer').html(data.payment);
            toastr.success("Đã huỷ apply voucher");
        })
}

// setup signalR
const conection = new signalR.HubConnectionBuilder().withUrl("/order-hub").build();

conection.on("PaymentSuccess", (result) => {
    toastr.success('Thanh toán và tạo đơn hàng thành công');
    const data = JSON.parse(result);

    $('#orderItemContainer').html('');
    $('#customerInfoContainer').html('');
    $('#shippingInfoContainer').html('');
    $('#orderPaymentInfoContainer').html('');
    $('#orderButtonActionContainer').html('');
    // show new data
    $('#orderItemContainer').html(data.Items);
    $('#customerInfoContainer').html(data.Customer);
    $('#shippingInfoContainer').html(data.Shipping);
    $('#orderPaymentInfoContainer').html(data.Payment);
    $('#orderButtonActionContainer').html(data.Buttons);

    interactiveCartItemAndVoucherButton(false);
    hangeResetButtonText(false);
    $('#tempSaveButtonContainer').html('');
    $('#tempSaveButtonContainer').html(data.TempSaveButton);
});

conection.on("PaymentFail", (message) => {
    toastr.error(message);
});

conection.start();