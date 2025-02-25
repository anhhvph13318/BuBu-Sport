$(document).ready(function () {
    $("#accountSearchBtn").click(function () {
        var searchValue = $("#accountSearch").val().trim();
        searchAccounts(searchValue);
    });

    $("#resetSearchBtn").click(function () {
        $("#accountSearch").val("");
        searchAccounts("");
    });

function searchAccounts(keyword) {
    $.ajax({
        url: "/accounts/search",
        type: "GET",
        data: { keyword: keyword },
        success: function (response) {
            $("#accountTable tbody").html(response);
        },
        error: function () {
            toastr.error("Lỗi khi tìm kiếm tài khoản!");
        }
    });
}
