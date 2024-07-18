$(document).ready(function () {
    setTimeout(() => {
        var total = document.getElementById("finalAmount");
        $("#finalAmount").text(Number(total.innerText));
        var listTotal = document.querySelectorAll('.total-list');
        for (let i = 0; i < listTotal.length; i++) {
            listTotal[i].innerHTML = Number(listTotal[i].innerHTML).toString();
        }
    }, 100);


});

function IncreaseQuantity(proId, quantity, price) {
    var quantityVal = document.getElementById(proId);
    $.ajax({
        type: 'POST',
        url: `IncreaseQuantity/${proId}/${quantityVal.value}`,
        contentType: 'text/plain',
        crossDomain: false,
        async: true,
        success: function (response) {
            console.log(response);
            if (response=="Success") {
                var quantityVal = document.getElementById(proId);
                var total = document.getElementById("finalAmount");
                quantityVal.value = Number(quantityVal.value) + 1;
                $("#finalAmount").text(Number(total.innerText) + Number(price));
            }
            else {
                var quantityVal = document.getElementById(proId);
                var total = document.getElementById("finalAmount");
                quantityVal.value = 1;
               // $("#finalAmount").text(Number(total.innerText) + Number(price));
            }

        }
    });


}

function OnBlurQuantity(proId) {

    var quantityVal = document.getElementById(proId).value;
    if (quantityVal != "") {

        $.ajax({
            type: 'POST',
            url: `OnKeyUpQuantity/${proId}/${quantityVal}`,
            contentType: 'text/plain',
            crossDomain: false,
            async: true,
            success: function (response) {

                if (response != '0' && !response.includes('Sản phẩm')) {
                    $("#finalAmount").text(Number(response));
                }
                else {
                        var quantityVal = document.getElementById(proId);
                        var total = document.getElementById("finalAmount");
                        quantityVal.value = 1;
                        // $("#finalAmount").text(Number(total.innerText) + Number(price));
                    }
                }



        });

    }
}

function DecreaseQuantity(proId, quantity, price) {

    $.ajax({
        type: 'POST',
        url: `DecreaseQuantity/${proId}/${quantity}`,
        contentType: 'text/plain',
        crossDomain: false,
        async: true,
        success: function (response) {
            console.log(response)
            var quantityVal = document.getElementById(proId);
            var total = document.getElementById("finalAmount");
            quantityVal.value = Number(quantityVal.value) - 1;
            $("#finalAmount").text(Number(total.innerText) - Number(price));


        }
    });

}


function Payment() {
    var fullNameVal = document.getElementById("FullName").value;
    var phoneNumberVal = document.getElementById("PhoneNumber").value;
    var addressVal = document.getElementById("Address").value;
    var codeVal = document.getElementById("Code").value;
    var shippingDateVal = document.getElementById("ShippingDate").value;
    var noteVal = document.getElementById("Note").value;
    var storedDataSend = {
        "fullName": fullNameVal,
        "phoneNumber": phoneNumberVal,
        "address": addressVal,
        "code": codeVal,
        "shippingDate": shippingDateVal,
        "note": noteVal
    }

    $.ajax({
        type: 'POST',
        url: `Payment`,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(storedDataSend),
        crossDomain: false,
        async: true,
        success: function (response) {

alert("Payment successfully");
            window.location.reload();

        }
    });
}

$(document).ready(function () { //DISABLED PAST DATES IN APPOINTMENT DATE
    var dateToday = new Date();
    var month = dateToday.getMonth() + 1;
    var day = dateToday.getDate();
    var year = dateToday.getFullYear();

    if (month < 10)
        month = '0' + month.toString();
    if (day < 10)
        day = '0' + day.toString();

    var maxDate = year + '-' + month + '-' + day;
    $('#ShippingDate').attr('min', maxDate);
});

function ShowDetailOrder(code) {

    $.ajax({
        type: 'POST',
        url: `Cart/${code}`,
        contentType: 'text/plain',
        crossDomain: false,
        async: true,
        success: function (response) {

        }
    });

}
