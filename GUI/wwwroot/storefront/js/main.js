(function($) {
	"use strict"

	// Mobile Nav toggle
	$('.menu-toggle > a').on('click', function (e) {
		e.preventDefault();
		$('#responsive-nav').toggleClass('active');
	})

	// Fix cart dropdown from closing
	$('.cart-dropdown').on('click', function (e) {
		e.stopPropagation();
	});

	/////////////////////////////////////////

	// Products Slick
	$('.products-slick').each(function() {
		var $this = $(this),
				$nav = $this.attr('data-nav');

		$this.slick({
			slidesToShow: 4,
			slidesToScroll: 1,
			autoplay: true,
			infinite: true,
			speed: 300,
			dots: false,
			arrows: true,
			appendArrows: $nav ? $nav : false,
			responsive: [{
	        breakpoint: 991,
	        settings: {
	          slidesToShow: 2,
	          slidesToScroll: 1,
	        }
	      },
	      {
	        breakpoint: 480,
	        settings: {
	          slidesToShow: 1,
	          slidesToScroll: 1,
	        }
	      },
	    ]
		});
	});

	// Products Widget Slick
	$('.products-widget-slick').each(function() {
		var $this = $(this),
				$nav = $this.attr('data-nav');

		$this.slick({
			infinite: true,
			autoplay: true,
			speed: 300,
			dots: false,
			arrows: true,
			appendArrows: $nav ? $nav : false,
		});
	});

	/////////////////////////////////////////

	// Product Main img Slick
	$('#product-main-img').slick({
    infinite: true,
    speed: 300,
    dots: false,
    arrows: true,
    fade: true,
    asNavFor: '#product-imgs',
  });

	// Product imgs Slick
  $('#product-imgs').slick({
    slidesToShow: 3,
    slidesToScroll: 1,
    arrows: true,
    centerMode: true,
    focusOnSelect: true,
		centerPadding: 0,
		vertical: true,
    asNavFor: '#product-main-img',
		responsive: [{
        breakpoint: 991,
        settings: {
					vertical: false,
					arrows: false,
					dots: true,
        }
      },
    ]
  });

	// Product img zoom
	var zoomMainProduct = document.getElementById('product-main-img');
	if (zoomMainProduct) {
		$('#product-main-img .product-preview').zoom();
	}

	/////////////////////////////////////////

	// Input number
	$('.input-number').each(function() {
		var $this = $(this),
		$input = $this.find('input[type="number"]'),
		up = $this.find('.qty-up'),
		down = $this.find('.qty-down');

		down.on('click', function () {
			var value = parseInt($input.val()) - 1;
			value = value < 1 ? 1 : value;
			$input.val(value);
			$input.change();
			updatePriceSlider($this , value)
		})

		up.on('click', function () {
			var value = parseInt($input.val()) + 1;
			$input.val(value);
			$input.change();
			updatePriceSlider($this , value)
		})
	});

	var priceInputMax = document.getElementById('price-max'),
			priceInputMin = document.getElementById('price-min');

    if (priceInputMax) {
		priceInputMax.addEventListener('change', function () {
			updatePriceSlider($(this).parent(), this.value)
		});
    }

    if (priceInputMin) {
		priceInputMin.addEventListener('change', function () {
			updatePriceSlider($(this).parent(), this.value)
		});
    }

	function updatePriceSlider(elem , value) {
		if ( elem.hasClass('price-min') ) {
			console.log('min')
			priceSlider.noUiSlider.set([value, null]);
		} else if ( elem.hasClass('price-max')) {
			console.log('max')
			priceSlider.noUiSlider.set([null, value]);
		}
	}

	// Price Slider
	var priceSlider = document.getElementById('price-slider');
	if (priceSlider) {
		noUiSlider.create(priceSlider, {
			start: [1, 999],
			connect: true,
			step: 1,
			range: {
				'min': 1,
				'max': 999
			}
		});

		priceSlider.noUiSlider.on('update', function( values, handle ) {
			var value = values[handle];
			handle ? priceInputMax.value = value : priceInputMin.value = value
		});
	}

	$(function () {
		let path = window.location.pathname.slice(0, 6);
		if (path == "/store") {
			$("#nav-bar li").removeClass("active");
			$("#store").addClass("active");
		}
	});

	$('.add-to-cart-btn').on('click',function () {
		$.post("/AddCart", { prId: $(this).attr('data-prId') }, function () {
			window.location = "/checkout";
		});
	});

	$('.remove-item').on('click', function () {
		$.post("/DeleteItem", { id: $(this).parent().parent().attr('data-itemId') }, function () {
			window.location.reload();
		});
	});

	$('#order-submit').on('click', function (e) {
		e.preventDefault();
		let name = $('#order-name').val();
		let phone = $('#order-phone').val();
		let address = $('#order-address').val();
		let district = $('#order-district').val();
		let city = $('#order-city').val();
		let code = $('#order-code').val();
		let note = $('#order-note').val();
		let ids = [];
		let items = $('.order-item');
		$.each(items, function (i, obj) {
			let id = $(obj).attr('data-itemId');
			ids.push(id);
		});
		if (name && phone && address && district && city && code && ids.length) {
			$.post("/Buy", {
				name: name,
				phone: phone,
				address: address,
				district: district,
				city: city,
				zipCode: code,
				note: note, 
				ids: ids
			}, function (data) {
				if (data.success) {
					window.location.href = "/store";
				} else {
                    alert("FAILED");
				}
			});
		} else {
            alert("fill all fields");
			return false;
		}
	});

	let changeQuant;
	$('.input-quant').on('change', function () {
		let el = $(this);
		let value = $(this).val();
		let id = $(this).parent().parent().attr('data-itemId');
        if (isNaN(value)) {
			$(this).val($(this).attr('data-value'));
			return;
		}

		if (changeQuant) {
			clearTimeout(changeQuant);
        }
		changeQuant = setTimeout(function () {
			updateQuantity(value, id, false, el);
			el.attr('data-value', value);
			
		}, 500)
	});

	$('.quant-down').on('click', function () {
		let el = $(this);
		let value = -1;
		let id = $(this).attr('data-itemId');

		let price = $(`#item-${id}`).attr('data-price');
		let quant = parseFloat($(`#quant-${id}`).val()) + value;
		$(`#quant-${id}`).val(quant);
		$(`#sub-${id}`).text(parseFloat(price) * parseFloat(quant));

			updateQuantity(value, id, true, el);
			el.attr('data-value', value);
	});
	$('.quant-up').on('click', function () {
		let el = $(this);
		let value = 1;
		let id = $(this).attr('data-itemId');

		let price = $(`#item-${id}`).attr('data-price');
		let quant = parseFloat($(`#quant-${id}`).val()) + value;
		$(`#quant-${id}`).val(quant);
		$(`#sub-${id}`).text(parseFloat(price) * quant);

		updateQuantity(value, id, true, el);
		el.attr('data-value', value);
	});

	const updateQuantity = function (quantity, id, incre, el) {
		$.post("/ChangeQuantity", {
			cartDetaiID: id,
			quantity: quantity,
			isIncrement: incre
		}, function (data) {
            if (data && data.success) {
				let subTotal = parseFloat(data.data.quantity) * parseFloat(data.data.price);
				if (!incre) {
					$(`#sub-${id}`).text(subTotal);
                }
				UpdatePrice();
			} else {
				el.val(el.attr('data-value'));
            }
		});
	}

	const UpdatePrice = function () {
		let subTotals = $('.sub-total');
		let total = 0;
		$.each(subTotals ,function (i, obj) {
			let number = parseFloat(obj.innerText);
            if (number) {
				total += number;
            }
		});

		$('.order-total').text(total);
	}
	const updateQuantityCart = function (quantity, id, incre, el) {
		$.post("/ChangeQuantity", {
			cartDetaiID: id,
			quantity: quantity,
			isIncrement: incre
		}, function (data) {
			if (data && data.success) {
				let subTotal = parseFloat(data.data.quantity) * parseFloat(data.data.price);
				if (!incre) {
					$(`#sub-${id}`).text(subTotal);
				}
				updateCartTotal();
			} else {
				el.val(el.attr('data-value'));
			}
		});
	}
	$('.cart-input-quant').on('change', function () {
		let el = $(this);
		let value = $(this).val();
		let id = $(this).parent().parent().attr('data-itemId');
		if (isNaN(value)) {
			$(this).val($(this).attr('data-value'));
			return;
		}

		if (changeQuant) {
			clearTimeout(changeQuant);
		}
		changeQuant = setTimeout(function () {
			updateQuantity(value, id, false, el);
			el.attr('data-value', value);

		}, 500)
	});

	$('.cart-quant-down').on('click', function () {
		let el = $(this);
		let value = -1;
		let id = $(this).attr('data-itemId');

		let price = $(`#item-${id}`).attr('data-price');
		let quant = parseFloat($(`#quant-${id}`).val()) + value;
		$(`#quant-${id}`).val(quant);
		$(`#sub-${id}`).text(parseFloat(price) * parseFloat(quant));

		updateQuantityCart(value, id, true, el);
		el.attr('data-value', value);
	});
	$('.cart-quant-up').on('click', function () {
		let el = $(this);
		let value = 1;
		let id = $(this).attr('data-itemId');

		let price = $(`#item-${id}`).attr('data-price');
		let quant = parseFloat($(`#quant-${id}`).val()) + value;
		$(`#quant-${id}`).val(quant);
		$(`#sub-${id}`).text(parseFloat(price) * quant);

		updateQuantityCart(value, id, true, el);
		el.attr('data-value', value);
	});

	$('.chk-select-item').on("change", function () {
		let id = $(this).parent().parent().attr('data-itemId');
		updateCartTotal();
	});

	//const updateCartTotal = function (id) {
	//	let sub = parseFloat($(`#sub-${id}`).text());
	//	let total = parseFloat($('.cart-total').text());
	//	if ($(`#check-${id}`).is(':checked')) {
	//		total += sub;
	//	} else {
	//		total -= sub;
	//	}
	//	$('.cart-total').text(total);
	//}

	const updateCartTotal = function () {
		let subTotals = $('.sub-total');
		let total = 0;
		$.each(subTotals, function (i, obj) {
			let id = $(obj).parent().attr('data-itemId');
			if ($(`#check-${id}`).is(':checked')) {
				let number = parseFloat(obj.innerText);
				if (number) {
					total += number;
				}
			}
		});

		$('.cart-total').text(total);
	}

	$('#cart-submit').on('click', function (e) {
		e.preventDefault();
		let items = $('.cart-item');
		let buyingItems = [];
		$.each(items, function (i, obj) {
			if ($(obj).find('.chk-select-item').is(':checked')) {
				let id = $(obj).attr('data-itemId');
				buyingItems.push(id);
			}
		});
		$.post("/ConfirmCart", { ids : buyingItems}, (data) => {
            if (data.success) {
				location.href = "/Checkout";
            }
		});
	});
})(jQuery);
