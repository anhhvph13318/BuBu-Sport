
    function openNav() {
        document.getElementById("myNav").classList.toggle("menu_width")
        document.querySelector(".custom_menu-btn").classList.toggle("menu_btn-style")
    }


    function Login() {
        $.ajax({
            type: 'GET',
            url: `ClearSession`,
            contentType: "application/json; charset=utf-8",
            contentType: 'text/plain',
            crossDomain: false,
            async: true,
            success: function (response) {
                window.location.href="/Login";
            }
        });

    }
