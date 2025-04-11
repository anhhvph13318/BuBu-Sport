(function ($) {
	"use strict"

	let discount = 0;
	let maxDiscount = 0;
	let shipping = 0;

	function ValidateEmail(email) {
		return String(email)
			.toLowerCase()
			.match(
				/(?:(?:\r\n)?[ \t])*(?:(?:(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*))*@(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*))*|(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*)*\<(?:(?:\r\n)?[ \t])*(?:@(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*))*(?:,@(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*))*)*:(?:(?:\r\n)?[ \t])*)?(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*))*@(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*))*\>(?:(?:\r\n)?[ \t])*)|(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*)*:(?:(?:\r\n)?[ \t])*(?:(?:(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*))*@(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*))*|(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*)*\<(?:(?:\r\n)?[ \t])*(?:@(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*))*(?:,@(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*))*)*:(?:(?:\r\n)?[ \t])*)?(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*))*@(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*))*\>(?:(?:\r\n)?[ \t])*)(?:,\s*(?:(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*))*@(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*))*|(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*)*\<(?:(?:\r\n)?[ \t])*(?:@(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*))*(?:,@(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*))*)*:(?:(?:\r\n)?[ \t])*)?(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|"(?:[^\"\r\\]|\\.|(?:(?:\r\n)?[ \t]))*"(?:(?:\r\n)?[ \t])*))*@(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*)(?:\.(?:(?:\r\n)?[ \t])*(?:[^()<>@,;:\\".\[\] \000-\031]+(?:(?:(?:\r\n)?[ \t])+|\Z|(?=[\["()<>@,;:\\".\[\]]))|\[([^\[\]\r\\]|\\.)*\](?:(?:\r\n)?[ \t])*))*\>(?:(?:\r\n)?[ \t])*))*)?;\s*)/
			);
	}

	function ValidatePhone(phone) {
		return String(phone)
			.toLowerCase()
			.match(
				/^(0|84)(2(0[3-9]|1[0-689]|2[0-25-9]|3[2-9]|4[0-9]|5[124-9]|6[0369]|7[0-7]|8[0-9]|9[012346789])|3[2-9]|5[25689]|7[06-9]|8[0-9]|9[012346789])([0-9]{7})$/
			);
	}

	function convertVND(value) {
		return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
	}

	function insertParam(key, value) {
		key = encodeURIComponent(key);
		value = encodeURIComponent(value);

		// kvp looks like ['key1=value1', 'key2=value2', ...]
		var kvp = document.location.search.substr(1).split('&');
		let i = 0;

		for (; i < kvp.length; i++) {
			if (kvp[i].startsWith(key + '=')) {
				let pair = kvp[i].split('=');
				pair[1] = value;
				kvp[i] = pair.join('=');
				break;
			}
		}

		if (i >= kvp.length) {
			kvp[kvp.length] = [key, value].join('=');
		}

		// can return this or...
		let params = kvp.join('&');

		// reload page with new params
		document.location.search = params;
	}

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
	$('.products-slick').each(function () {
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
	$('.products-widget-slick').each(function () {
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

	let min = parseInt($("#price-min").attr("data-value"));
	let minInput = parseInt($("#price-min").val());
	let max = parseInt($("#price-max").attr("data-value"));
	let maxInput = parseInt($("#price-max").val());
	let step = (max - min) / 100;

	$('.input-number').each(function () {
		var $this = $(this),
			$input = $this.find('input[type="number"]'),
			up = $this.find('.qty-up'),
			down = $this.find('.qty-down');

		down.on('click', function () {
			var value = parseInt($input.val()) - step;
			value = value < min ? min : value;
			$input.val(value);
			$input.change();
			updatePriceSlider($this, value)
		})

		up.on('click', function () {
			var value = parseInt($input.val()) + step;
			$input.val(value);
			$input.change();
			updatePriceSlider($this, value)
		})
	});

	var priceInputMax = document.getElementById('price-max'),
		priceInputMin = document.getElementById('price-min');

	if (priceInputMax) {
		priceInputMax.addEventListener('change', function () {
			updatePriceSlider($(this).parent(), this.value)
		});
		priceInputMax.value = maxInput;
	}

	if (priceInputMin) {
		priceInputMin.addEventListener('change', function () {
			updatePriceSlider($(this).parent(), this.value)
		});
		priceInputMin.value = minInput;
	}

	function updatePriceSlider(elem, value) {
		if (elem.hasClass('price-min')) {
			priceSlider.noUiSlider.set([value, null]);
		} else if (elem.hasClass('price-max')) {
			priceSlider.noUiSlider.set([null, value]);
		}
	}

	// Price Slider
	var priceSlider = document.getElementById('price-slider');
	if (priceSlider) {
		noUiSlider.create(priceSlider, {
			start: [min, max],
			connect: true,
			step: step,
			range: {
				'min': min,
				'max': max
			}
		});

		priceSlider.noUiSlider.on('update', function (values, handle) {
			var value = values[handle];
			handle ? priceInputMax.value = value : priceInputMin.value = value
		});
		priceSlider.noUiSlider.set([minInput, maxInput]);
	}


	$(function () {
		//let path = window.location.pathname.slice(0, 6);
		//if (path == "/store") {
		//	$("#nav-bar li").removeClass("active");
		//	$("#store").addClass("active");
		//}
	});

	$('.addCart').on('click', function () {
		let customerId = getCookie("user-id");
		$.post("/AddCart", { prId: $(this).attr('data-prId'), userId: customerId, quantity: $(this).attr('data-quantity') }, function (data) {
			if (!customerId) {
				setCookie("user-id", data.userId, 90)
			}
			window.location = "/cart";
		});
	});

	$('.buyNow').on('click', function () {
		let customerId = getCookie("user-id");
		$.post("/BuyNow", { prId: $(this).attr('data-prId'), userId: customerId, quantity: $(this).attr('data-quantity') }, function (data) {
			if (!customerId) {
				setCookie("user-id", data.userId, 90)
			}
			window.location = "/checkout";
		});
	});

	$('.remove-item').on('click', function () {
		let id = $(this).parent().parent().attr('data-itemId')
		$.post("/DeleteItem", { id: id }, function () {
			$(`#check-${id}`).prop("checked", false);
			window.location.reload();
		});
	});

	$('#order-submit').on('click', function (e) {
		e.preventDefault();

		let name = $('#order-name').val().trim();
		let phone = $('#order-phone').val().trim();
		let address = $('#order-address').val().trim();
		let district = $('#order-district').val().trim();
		let city = $('#order-city').val().trim();
		let email = $('#order-email').val().trim();

		let ids = [];
		let items = $('.order-item');
		$.each(items, function (i, obj) {
			let id = $(obj).attr('data-itemId');
			ids.push(id);
		});

		let isVNP = $('#payment-2').is(":checked");

		let isValid = true;
		let errors = [];

		if (!name) {
			errors.push("Họ tên không được để trống.");
			isValid = false;
		} else if (name.length < 2 || name.length > 50) {
			errors.push("Họ tên phải từ 2 đến 50 ký tự.");
			isValid = false;
		} else if (!/^[a-zA-ZÀ-ỹ\s'-]+$/.test(name)) {
			errors.push("Họ tên chỉ được chứa chữ cái, dấu cách, dấu gạch ngang hoặc dấu nháy đơn.");
			isValid = false;
		}

		if (!phone) {
			errors.push("Số điện thoại không được để trống.");
			isValid = false;
		} else if (!ValidatePhone(phone)) {
			errors.push("Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại 10 chữ số bắt đầu bằng 0 hoặc +84.");
			isValid = false;
		}

		if (email && !ValidateEmail(email)) {
			errors.push("Email không hợp lệ.");
			isValid = false;
		} else if (email.length > 100) {
			errors.push("Email không được vượt quá 100 ký tự.");
			isValid = false;
		}

		if (!address) {
			errors.push("Số nhà/tên đường không được để trống.");
			isValid = false;
		} else if (address.length < 5 || address.length > 100) {
			errors.push("Số nhà/tên đường phải từ 5 đến 100 ký tự.");
			isValid = false;
		}

		if (!district) {
			errors.push("Quận/huyện không được để trống.");
			isValid = false;
		} else if (district.length < 2 || district.length > 50) {
			errors.push("Quận/huyện phải từ 2 đến 50 ký tự.");
			isValid = false;
		} else if (!/^[a-zA-Z0-9\sÀ-ỹ]+$/.test(district)) {
			errors.push("Quận/huyện chỉ được chứa chữ cái, số và dấu cách.");
			isValid = false;
		}

		if (!city) {
			errors.push("Thành phố không được để trống.");
			isValid = false;
		} else if (city.length < 2 || city.length > 50) {
			errors.push("Thành phố phải từ 2 đến 50 ký tự.");
			isValid = false;
		} else if (!/^[a-zA-Z0-9\sÀ-ỹ]+$/.test(city)) {
			errors.push("Thành phố chỉ được chứa chữ cái, số và dấu cách.");
			isValid = false;
		}

		if (!ids.length) {
			errors.push("Không có sản phẩm nào được chọn để đặt hàng.");
			isValid = false;
		}

		if (!isValid) {
			Swal.fire({
				icon: 'error',
				title: 'Lỗi nhập liệu',
				html: errors.join('<br>'),
				confirmButtonText: 'OK',
				width: '600px',
				padding: '2em',
				customClass: {
					popup: 'larger-swal'
				}
			});
			return false;
		}

		Swal.fire({
			title: 'Xác nhận đặt hàng',
			text: 'Bạn có chắc chắn muốn đặt đơn hàng này?',
			icon: 'question',
			showCancelButton: true,
			confirmButtonColor: '#3085d6',
			cancelButtonColor: '#d33',
			confirmButtonText: 'Đồng ý',
			cancelButtonText: 'Hủy',
			width: '600px',
			padding: '2em',
			customClass: {
				popup: 'larger-swal'
			}
		}).then((result) => {
			if (result.isConfirmed) {
				$.ajax({
					url: "/Buy",
					type: "POST",
					contentType: "application/json",
					data: JSON.stringify({
						name: name,
						phone: phone,
						address: address,
						district: district,
						city: city,
						email: email,
						ids: ids,
						getatstore: false,
						isVNP: isVNP
					}),
					success: function (data) {
						if (data.success) {
							if (data.redirect) {
								window.location.href = data.url;
							} else {
								Swal.fire({
									icon: 'success',
									title: 'Đặt hàng thành công',
									text: `Mã đơn hàng: ${data.orderId}`,
									confirmButtonText: 'OK',
									width: '600px',
									padding: '2em',
									customClass: {
										popup: 'larger-swal'
									}
								}).then(() => {
									window.location.href = `/success?vnp_TxnRef=${data.orderId}`;
								});
							}
						} else {
							Swal.fire({
								icon: 'error',
								title: 'Lỗi',
								text: 'Đã có lỗi xảy ra khi đặt hàng.',
								confirmButtonText: 'OK',
								width: '600px',
								padding: '2em',
								customClass: {
									popup: 'larger-swal'
								}
							});
						}
					},
					error: function () {
						Swal.fire({
							icon: 'error',
							title: 'Lỗi',
							text: 'Không thể kết nối đến server. Vui lòng thử lại.',
							confirmButtonText: 'OK',
							width: '600px',
							padding: '2em',
							customClass: {
								popup: 'larger-swal'
							}
						});
					}
				});
			}
		});
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

		}, 500)
	});

	$('.quant-down').on('click', function () {
		let el = $(this);
		let value = -1;
		let id = $(this).attr('data-itemId');

		let price = $(`#item-${id}`).attr('data-price');
		let quant = parseFloat($(`#quant-${id}`).val()) + value;
		$(`#quant-${id}`).val(quant);
		$(`#sub-${id}`).text(convertVND(parseFloat(price) * parseFloat(quant)));

		updateQuantity(value, id, true, el);
	});
	$('.quant-up').on('click', function () {
		let el = $(this);
		let value = 1;
		let id = $(this).attr('data-itemId');

		let price = $(`#item-${id}`).attr('data-price');
		let quant = parseFloat($(`#quant-${id}`).val()) + value;

		if (updateQuantity(value, id, true, el)) {
			$(`#quant-${id}`).val(quant);
			$(`#sub-${id}`).text(convertVND(parseFloat(price) * quant));
		}
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
				el.attr('data-value', value);
				return true;
			} else {
				el.val(el.attr('data-value'));
				return false;
			}
		});
	}

	const UpdatePrice = function () {
		let subTotals = $('.sub-total');
		let total = 0;
		$.each(subTotals, function (i, obj) {
			var el = obj.id;
			let number = parseFloat($("#" + el).attr("data-value"));
			if (number) {
				total += number;
			}
		});

		if (shipping) {
			total += shipping;
		}

		if (total > 0 && discount > 0) {
			let discountAmount = 0;
			if (maxDiscount > 0) {
				discountAmount = total / 100 * discount;
				discountAmount = discountAmount > maxDiscount ? maxDiscount : discountAmount;
			} else {
				discountAmount = discount;
			}
			let discountString = convertVND(discountAmount);
			$("#discount-value").text(discountString);
			total -= discountAmount;
		}
		let totalString = convertVND(total);
		$('.order-total').text(totalString);
	}
	const updateQuantityCart = function (quantity, id, incre, el) {
		if ((quantity > 0 && !incre) || incre) {
			$.post("/ChangeQuantity", {
				cartDetaiID: id,
				quantity: quantity,
				isIncrement: incre
			}, function (data) {
				if (data && data.success) {
					let subTotal = parseFloat(data.data.quantity) * parseFloat(data.data.price);
					if (incre) {
						let price = $(`#item-${id}`).attr('data-price');
						let quant = parseFloat($(`#quant-${id}`).val()) + quantity;
						quantity = quant < 1 ? 1 : quant;
						$(`#quant-${id}`).val(quantity);
						let val = parseFloat(price) * quantity;
					}
					$(`#sub-${id}`).text(convertVND(subTotal));
					$(`#sub-${id}`).attr("data-value", subTotal);
					el.attr('data-value', quantity);
					updateCartTotal();
				} else {
					el.val(parseInt(el.attr('data-value')) < 1 ? 1 : el.attr('data-value'));
				}
			});
		} else {
			el.val(parseInt(el.attr('data-value')) < 1 ? 1 : el.attr('data-value'));
		}
	}
	$('.cart-input-quant').on('keyup', function () {
		let value = $(this).val();
		let id = $(this).parent().parent().attr('data-itemId');
		let el = $(this);
		if (isNaN(value)) {
			$(this).val($(this).attr('data-value'));
			return;
		}

		if (changeQuant) {
			clearTimeout(changeQuant);
		}
		changeQuant = setTimeout(function () {
			updateQuantity(value, id, false, el);

		}, 350)
	});

	$('.cart-quant-down').on('click', function () {
		let value = -1;
		let id = $(this).attr('data-itemId');
		let el = $(`#quant-${id}`);

		//let price = $(`#item-${id}`).attr('data-price');
		//let quant = parseFloat($(`#quant-${id}`).val()) + value;
		//$(`#quant-${id}`).val(quant);
		//$(`#sub-${id}`).text(parseFloat(price) * parseFloat(quant));

		updateQuantityCart(value, id, true, el);
	});
	$('.cart-quant-up').on('click', function () {
		let value = 1;
		let id = $(this).attr('data-itemId');
		let el = $(`#quant-${id}`);

		updateQuantityCart(value, id, true, el);
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
				let val = parseFloat($("#sub-" + id).attr("data-value"));
				let number = parseFloat(val);
				if (number) {
					total += number;
				}
			}
		});



		if (total > 0) {
			$("#cart-submit").show();
		} else {
			$("#cart-submit").hide();
		}

		$('.cart-total').text(convertVND(total));
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
		$.post("/ConfirmCart", { ids: buyingItems }, (data) => {
			if (data.success) {
				location.href = "/Checkout";
			}
		});
	});

	function setCookie(cname, cvalue, exdays) {
		const d = new Date();
		d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
		let expires = "expires=" + d.toUTCString();
		document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
	}

	function getCookie(cname) {
		let name = cname + "=";
		let ca = document.cookie.split(';');
		for (let i = 0; i < ca.length; i++) {
			let c = ca[i];
			while (c.charAt(0) == ' ') {
				c = c.substring(1);
			}
			if (c.indexOf(name) == 0) {
				return c.substring(name.length, c.length);
			}
		}
		return "";
	}

	function eraseCookie(name) {
		document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
	}

	$("input[type=radio][name=shipping]").on('change', function (e) {
		let shippingFee = 0;
		if ($(this).val() == 1) {
			$(".shipping-details").addClass("active");
			shippingFee = 0;
		} else {
			$(".shipping-details").removeClass("active");
		}
		var shippingString = convertVND(shippingFee);
		$("#shipping-fee").text(shippingString);
		shipping = shippingFee;
		UpdatePrice();
	});

	$("#page-display").on("change", function () {
		insertParam("p", 0);
		insertParam("t", $(this).val());
	});

	$(".pagination-btn").on("click", function (e) {
		e.preventDefault();
		if (!$(this).hasClass("active")) {
			insertParam("p", $(this).attr("data-page"));
		}
	})

	let cName = getCookie("cName");
	if (cName) {
		$('#customer-name').text(cName);
		$(".anonymous").hide();
		$(".signed").show();
	}

	$('#sign-out').on("click", function (e) {
		e.preventDefault();
		eraseCookie("cName");
		eraseCookie("cPhone");
		eraseCookie("aId");
		eraseCookie("user-id");
		$.post("/LogOutStorefront", {}, () => {
			window.location = "/store";
		});
	});


	$("#customer-submit").on("click", function () {
		let name = $("#customer-fullname").val();
		if (!name) {
			alert("Bắt buộc phải nhập tên");
			return false;
		}
		let address = $("#customer-address").val();
		let email = $("#customer-email").val();
		if (email && !ValidateEmail(email)) {
			alert("Email không đúng định dạng");
			return false;
		}
		let sex = null;
		let selectedSex = $("input[type=radio][name=sex]:checked");
		if (selectedSex && selectedSex.length > 0) {
			sex = selectedSex.val();
		}
		$.post("/UpdateCustomerInfo", {
			name,
			address,
			sex,
			email
		}, (data) => {
			if (data.success) {
				alert("Thành công");
				setCookie("cName", name)
				location.reload();
			} else {
				alert("Thất bại");
			}
		});
	});

	$("#password-submit").on("click", function () {
		let oldPassword = $("#customer-oldpassword").val();
		let password = $("#customer-newpassword").val();
		let cfpassword = $("#customer-cfnewpassword").val();
		if (password != cfpassword) {
			alert("Mật khẩu và mật khẩu xác nhận không khớp");
			return false;
		}
		if (password == oldPassword) {
			alert("Mật khẩu cũ và mật khẩu mới không được giống nhau");
			return false;
		}
		$.post("/UpdatePassword", {
			oldPassword,
			password,
		}, (data) => {
			if (data.success) {

				alert("Thành công! Vui lòng đăng nhập lại");
				eraseCookie("cName");
				eraseCookie("cPhone");
				eraseCookie("aId");
				eraseCookie("user-id");
				$.post("/LogOutStorefront", {}, () => {
					window.location = "/signin";
				});
			} else {
				if (data.wrong) {
					alert("Mật khẩu cũ chưa chính xác");
				}
				else { alert("Thất bại"); }
			}
		});
	});

})(jQuery);
