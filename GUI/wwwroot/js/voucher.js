function voucherUnitChange(event) {
    if (event.target.value == "0") {
        $("#maxDiscount").css({ "display": "block" });
    } else {
        $("#maxDiscount").css({ "display": "none" });
    }
}