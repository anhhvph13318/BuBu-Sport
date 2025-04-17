function validateName(name) {
    if (!name || name.trim() === '') {
        return { isValid: false, message: 'Vui lòng nhập tên khách hàng' };
    }

    const nameRegex = /^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỂưăạảấầẩẫậắằẳẵặẹẻẽềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễếệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ\s]+$/;
    if (!nameRegex.test(name)) {
        return { isValid: false, message: 'Tên không được chứa số hoặc ký tự đặc biệt' };
    }

    if (name.length < 2 || name.length > 50) {
        return { isValid: false, message: 'Tên phải từ 2 đến 50 ký tự' };
    }

    return { isValid: true, message: '' };
}

function validatePhoneNumber(phone) {
    if (!phone || phone.trim() === '') {
        return { isValid: false, message: 'Vui lòng nhập số điện thoại' };
    }

    const phoneRegex = /^(0[3|5|7|8|9])+([0-9]{8})$/;
    if (!phoneRegex.test(phone)) {
        return { isValid: false, message: 'Số điện thoại không hợp lệ (phải có 10 số và bắt đầu bằng 0)' };
    }

    return { isValid: true, message: '' };
}

function validateAddress(address) {
    if (!address || address.trim() === '') {
        return { isValid: false, message: 'Vui lòng nhập địa chỉ' };
    }

    if (address.length < 5 || address.length > 200) {
        return { isValid: false, message: 'Địa chỉ phải từ 5 đến 200 ký tự' };
    }

    return { isValid: true, message: '' };
}

function validateEmail(email) {
    if (!email || email.trim() === '') {
        return { isValid: true, message: '' };
    }

    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailRegex.test(email)) {
        return { isValid: false, message: 'Email không hợp lệ' };
    }

    return { isValid: true, message: '' };
}

function showError(inputElement, errorMessage) {
    const errorElement = $(inputElement).next('.text-danger');
    if (errorElement.length) {
        errorElement.text(errorMessage);
        errorElement.css('display', 'block');
    } else {
        const newErrorElement = $(`<p class="text-danger">${errorMessage}</p>`);
        $(inputElement).after(newErrorElement);
    }
}

function hideError(inputElement) {
    const errorElement = $(inputElement).next('.text-danger');
    if (errorElement.length) {
        errorElement.css('display', 'none');
    }
}

function attachValidators() {
    $('#customerName').on('input blur', function () {
        const result = validateName($(this).val());
        if (!result.isValid) {
            showError(this, result.message);
        } else {
            hideError(this);
        }
    });

    $('#customerPhoneNumber').on('input blur', function () {
        $(this).val($(this).val().replace(/[^0-9]/g, ''));

        const result = validatePhoneNumber($(this).val());
        if (!result.isValid) {
            showError(this, result.message);
        } else {
            hideError(this);
        }
    });

    $('#customerAddress').on('input blur', function () {
        const result = validateAddress($(this).val());
        if (!result.isValid) {
            showError(this, result.message);
        } else {
            hideError(this);
        }
    });

    $('#customerEmail').on('input blur', function () {
        const result = validateEmail($(this).val());
        if (!result.isValid) {
            showError(this, result.message);
        } else {
            hideError(this);
        }
    });

    $('#receiverName').on('input blur', function () {
        const result = validateName($(this).val());
        if (!result.isValid) {
            showError(this, result.message);
        } else {
            hideError(this);
        }
    });

    $('#receiverPhone').on('input blur', function () {
        $(this).val($(this).val().replace(/[^0-9]/g, ''));

        const result = validatePhoneNumber($(this).val());
        if (!result.isValid) {
            showError(this, result.message);
        } else {
            hideError(this);
        }
    });

    $('#receiverAddress').on('input blur', function () {
        const result = validateAddress($(this).val());
        if (!result.isValid) {
            showError(this, result.message);
        } else {
            hideError(this);
        }
    });
}

function validateAllFields() {
    let isValid = true;

    const nameResult = validateName($('#customerName').val());
    if (!nameResult.isValid) {
        showError('#customerName', nameResult.message);
        isValid = false;
    } else {
        hideError('#customerName');
    }

    const phoneResult = validatePhoneNumber($('#customerPhoneNumber').val());
    if (!phoneResult.isValid) {
        showError('#customerPhoneNumber', phoneResult.message);
        isValid = false;
    } else {
        hideError('#customerPhoneNumber');
    }

    const addressResult = validateAddress($('#customerAddress').val());
    if (!addressResult.isValid) {
        showError('#customerAddress', addressResult.message);
        isValid = false;
    } else {
        hideError('#customerAddress');
    }

    const emailResult = validateEmail($('#customerEmail').val());
    if (!emailResult.isValid) {
        showError('#customerEmail', emailResult.message);
        isValid = false;
    } else {
        hideError('#customerEmail');
    }

    if ($('#isSameAsCustomerAddress').is(':checked') === false && $('#shippingLocation').val() === "1") {
        const receiverNameResult = validateName($('#receiverName').val());
        if (!receiverNameResult.isValid) {
            showError('#receiverName', receiverNameResult.message);
            isValid = false;
        } else {
            hideError('#receiverName');
        }

        const receiverPhoneResult = validatePhoneNumber($('#receiverPhone').val());
        if (!receiverPhoneResult.isValid) {
            showError('#receiverPhone', receiverPhoneResult.message);
            isValid = false;
        } else {
            hideError('#receiverPhone');
        }

        const receiverAddressResult = validateAddress($('#receiverAddress').val());
        if (!receiverAddressResult.isValid) {
            showError('#receiverAddress', receiverAddressResult.message);
            isValid = false;
        } else {
            hideError('#receiverAddress');
        }
    }

    const items = $('#orderItemContainer table tbody tr').length;
    if (items === 0) {
        toastr.error("Giỏ hàng đang không có sản phẩm nào!");
        isValid = false;
    }

    return isValid;
}

$(document).ready(function () {
    attachValidators();

    window.verify = validateAllFields;
});