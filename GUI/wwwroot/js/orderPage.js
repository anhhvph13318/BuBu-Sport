
const productStorage = new ProductStorage();

// toast setup
toastr.options.timeOut = 5000; 
toastr.options.closeButton = true;
toastr.options.progressBar = true;

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

    try {
        const products = await productStorage.filter(value);

        if (!Array.isArray(products)) {
            console.warn("Sản phẩm trả về không phải mảng:", products);
            return;
        }

        for (let i = 0; i < products.length; i++) {
            if (products[i].quantity <= 0) continue;

            const item = document.createElement('a');
            item.setAttribute('class', 'text-decoration-none');
            const itemContent = `
                <div class="d-flex">
                    <img class="img-thumbnail" style="min-width: 100px; height: 50px" src=${products[i].image} alrt=""/>
                    <p class="fs-bold ms-3 text-decoration-none text-black nav-link">${products[i].name}</p>
                    <p class="fs-bold ms-3 text-decoration-none text-black nav-link">${products[i].color}</p>
                    <p class="fs-bold ms-3 text-decoration-none text-black nav-link">${products[i].size}</p>
                </div>
            `;

            item.onclick = function () {
                const { id, code , name, price, image ,color ,size} = products[i]
                handleItemSelect(id, code, name, price, image, color, size);
            };

            $(item).append(itemContent);
            $('#autocomplete-list').append(item);
        }
    } catch (error) {
        console.error("Lỗi khi tìm kiếm sản phẩm:", error);
    }
});

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

            // Cập nhật lại nội dung nút Lưu tạm
            $('#tempSaveButtonContainer').html('');
            $('#tempSaveButtonContainer').html(data.tempSaveButton);

            // Ẩn lại nút Lưu tạm sau khi render
            setTimeout(() => {
                $('#tempSaveButton').hide(); // ẩn đúng nút
            }, 0);

            // Cập nhật các trường khác
            $('#shippingLocation').val(data.isCustomerTakeYourSelf ? '0' : '1');
            $('#orderStatus').val(data.status.toString());
            $('#shippingLocation').trigger('change');
            $('#orderStatus').trigger('change');
            $('#isSameAsCustomerAddress').trigger('change');
        });
}

function handleItemSelect(id, code, name, price, image, color, size) {
    clearSearchResult();

    fetch(ORDER_ADD_ITEM, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            id: id,
            code: code,
            productName: name,
            quantity: 1,
            productImage: image,
            price: price,
            color: color,
            size: size,
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

            // Hiển thị lại nút "Lưu tạm"
            $('#tempSaveButtonContainer').html('');
            $('#tempSaveButtonContainer').html(data.tempSaveButton);
        });
}


function saveOrder(isDraft) {
    if (!verify()) return;
    $('#loading-overlay').css('display', 'flex');
    const customerInfo = {
        name: $('#customerName').val(),
        phoneNumber: $('#customerPhoneNumber').val(),
        address: $('#customerAddress').val(),
        email: $('#customerEmail').val() 
    };

    const shippingInfo = {
        name: $('#receiverName').val(),
        phoneNumber: $('#receiverPhone').val(),
        address: $('#receiverAddress').val()
    };

    // 🟡 Lấy dữ liệu sản phẩm đang hiển thị trong bảng
    const orderItems = [];
    $('#orderItemContainer table tbody tr').each(function () {
        const row = $(this);


        orderItems.push({
            id: row.find('.order-item-id').text().trim(),
            code: row.find('td:nth-child(2)').text().trim(),
            productImage: row.find('img').attr('src'),
            productName: row.find('td:nth-child(4)').text().trim(),
            color: row.find('td:nth-child(5)').text().trim(),
            size: row.find('td:nth-child(6)').text().trim(),
            price: parseFloat(row.find('td:nth-child(7)').text().replace(/[^\d]/g, '')),
            quantity: parseInt(row.find('input.order-item-quantity').val())
        });
    });


    const payload = {
        isCustomerTakeYourSelf: $('#shippingLocation').val() === "0",
        isShippingAddressSameAsCustomerAddress: $('#isSameAsCustomerAddress').is(':checked'),
        status: parseInt($('#orderStatus').val()),
        customerInfo,
        shippingInfo,
        isDraft,
        orderItems // 🔥 Thêm orderItems vào payload
    };

    fetch(ORDER_TEMP_SAVE_API, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
    })
        .then(res => res.json())
        .then(data => {
            $('#orderTempSaveContainer').html(data.orders);
            $('#orderButtonActionContainer').html(data.buttons);
            changeResetButtonText(false);
            interactiveCartItemAndVoucherButton(false);
        })
        .then(_ => {
            if (isDraft) {
                toastr.success("Lưu tạm hóa đơn thành công!");
            } else {
                toastr.success("Tạo hóa đơn thành công!");
            }
            setTimeout(() => {
                window.location.reload();
            }, 1000);
        })
        .catch(err => {
            console.error("Lỗi khi lưu hóa đơn:", err);
            toastr.error("Có lỗi xảy ra khi lưu hóa đơn!");
        })
        .finally(() => {
            $('#loading-overlay').css('display', 'none');
        })
        .then(_ => clearOrder());
}

function removeDraft(id) {
    console.log("Bắt đầu hàm removeDraft với id:", id); // Debug: Kiểm tra id truyền vào

    Swal.fire({
        title: 'Xác nhận xóa đơn hàng',
        text: 'Bạn có chắc muốn xóa đơn hàng này không?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Có, xóa!',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        console.log("Kết quả confirm dialog:", result); // Debug: Kiểm tra kết quả confirm

        if (result.isConfirmed) {
            console.log("Người dùng đã xác nhận xóa"); // Debug: Xác nhận người dùng đồng ý

            // Hiển thị trạng thái loading
            Swal.fire({
                title: 'Đang xóa...',
                text: 'Vui lòng chờ trong giây lát',
                allowOutsideClick: false,
                showConfirmButton: false,
                didOpen: () => {
                    Swal.showLoading();
                    console.log("Đang hiển thị loading..."); // Debug: Kiểm tra loading
                }
            });

            console.log("Gọi API xóa đơn hàng:", REMOVE_ORDER_TEMP_API(id)); // Debug: Kiểm tra API endpoint

            fetch(REMOVE_ORDER_TEMP_API(id), {
                method: 'DELETE'
            })
                .then(res => {
                    console.log("Phản hồi từ API:", res); // Debug: Kiểm tra response

                    if (!res.ok) {
                        return res.json().then(data => {
                            console.error("Lỗi từ API:", data); // Debug: Log lỗi chi tiết
                            throw new Error(data.message || "Xóa đơn hàng thất bại!");
                        });
                    }
                    return res.json().catch(() => ({})); // Trả về object rỗng nếu không có dữ liệu JSON
                })
                .then(() => {
                    console.log("Xóa thành công, hiển thị thông báo"); // Debug: Kiểm tra bước thành công

                    // Hiển thị thông báo thành công
                    Swal.fire({
                        title: 'Thành công!',
                        text: 'Đơn hàng đã được xóa thành công',
                        icon: 'success',
                        confirmButtonText: 'Đóng'
                    }).then(() => {
                        console.log("Người dùng đóng thông báo, reload trang"); // Debug: Kiểm tra reload
                        location.reload();
                    });

                    console.log("Xóa đơn hàng thành công!"); // Thay thế toastr.success
                })
                .catch(error => {
                    console.error("Lỗi trong quá trình xóa:", error); // Debug: Log lỗi đầy đủ

                    Swal.fire({
                        title: 'Lỗi!',
                        text: error.message || "Đã xảy ra lỗi khi xóa đơn hàng!",
                        icon: 'error',
                        confirmButtonText: 'Đóng'
                    });

                    console.error("Lỗi khi xóa đơn hàng:", error.message || "Đã xảy ra lỗi!"); // Thay thế toastr.error
                });
        } else {
            console.log("Người dùng đã hủy xóa"); // Debug: Kiểm tra trường hợp hủy
        }
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
        email: $('#customerEmail').val()
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

function validatePhone(e) {
    const regex = /[0-9]{10}/
    const value = e.value;

        // Check if the current value matches the regex
        if (!regex.test(value)) {
            // If not, remove the last character
            e.value = value.slice(0, -1);
        }
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