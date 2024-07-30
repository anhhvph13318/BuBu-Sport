

    var init = "No items yet!";
    var counter = 0;
    function openNav() {
        document.getElementById("myNav").classList.toggle("menu_width")
        document.querySelector(".custom_menu-btn").classList.toggle("menu_btn-style")
    }
    // Initial Cart
    $(".counter").html(init);

    // Add Items To Basket
    function addToBasket() {
        counter++;
        $(".counter").html(counter).animate({
            'opacity' : '0'
        },300, function() {
            $(".counter").delay(300).animate({
                'opacity' : '1'
            })
        })
    }

    // Add To Basket Animation
    $(".btn-add-to-cart").on("click", function() {
        addToBasket(); $(this).parent().parent().find(".product_overlay").css({
            'transform': ' translateY(0px)',
            'opacity': '1',
            'transition': 'all ease-in-out .45s'
        }).delay(1500).queue(function() {
            $(this).css({
                'transform': 'translateY(-500px)',
                'opacity': '0',
                'transition': 'all ease-in-out .45s'
            }).dequeue();
        });
    });

    function  AddProductToCart(id){
        $.ajax({

            type: 'POST',
            url: `api/v1/addToCart/`+id,
            contentType: 'text/plain',
            crossDomain: false,
            async:true,
            success:function(response) {
alert("Add to basket successfully");

            }
        });
    }