// validate

// 

toastr.options.timeOut = 3000;

function createCategory() {
    const payload = {
        name: $('#categoryName').val().trim(),
        status: $('#categoryStatus').is(':checked') ? 0 : 1
    }

    if(payload.name === '') {
        $('#validate-message').css({'display': 'block'})
        return;
    }

    fetch(CREATE_CATEGORY, {
        headers: {
            'Content-Type': 'application/json'
        },
        method: 'POST',
        body: JSON.stringify(payload)
    })
    .then(res => res.json())
    .then(data => {
        $('#categoryTable').html('');
        $('#categoryTable').html(data.table);
        $('#modalContent').html(data.modal);
        $('#categoryModal').modal('hide');
        toastr.success("Thêm thành công");
    }).catch(_ => {
        $('#validate-message').html('Tên đã bị trùng');
        $('#validate-message').css({'display': 'block'});
    });
}

function updateCategory(id) {
    const payload = {
        name: $('#categoryName').val().trim(),
        status: $('#categoryStatus').is(':checked') ? 0 : 1
    }

    if(payload.name === '') {
        $('#validate-message').css({'display': 'block'})
        return;
    }

    fetch(`${CREATE_CATEGORY}/${id}`, {
        headers: {
            'Content-Type': 'application/json'
        },
        method: 'PATCH',
        body: JSON.stringify(payload)
    })
    .then(res => res.json())
    .then(data => {
        $('#categoryTable').html('');
        $('#categoryTable').html(data.table);
        $('#modalContent').html(data.modal);
        toastr.success("Cập nhật thành công");
        $('#categoryModal').modal('hide');
    }).catch(_ => {
        $('#validate-message').html('Tên đã bị trùng');
        $('#validate-message').css({'display': 'block'});
    });
}

function showDeatail(id) {
    fetch(GET_CATEGORY_DETAIL(id))
        .then(res => res.json())
        .then(data => {
            $('#modalContent').html('');
            $('#modalContent').html(data.modal);
            $('#categoryModal').modal('show');
        });
}

function search() {
    const keyword = $('#categorySearch').val();
    fetch(`${CREATE_CATEGORY}/search?name=${keyword}`)
        .then(res => res.json())
        .then(data => {
            $('#categoryTable').html('');
            $('#categoryTable').html(data.table);
        });
}

function reset() {
    $('#categorySearch').val('');
    search();
}

function clear() {
    $('#categoryName').val('');
    $('#categoryStatus').checked();
}