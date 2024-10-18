class SiteController {
    constructor() {
        this.initialize = function () {
            regsiterEvents();
            loadCart();
        };

        function loadCart() {
            const culture = $('#hidCulture').val();
            $.ajax({
                type: "GET",
                url: "/" + culture + '/Cart/GetListItems',
                success: function (res) {
                    $('#lbl_number_items_header').text(res.length);
                }
            });
        }

        function regsiterEvents() {
            $('body').on('click', '.btn-add-cart', function (e) {
                e.preventDefault();
                const culture = $('#hidCulture').val();
                const id = $(this).data('id');
                console.log(id);

                $.ajax({
                    type: "POST",
                    url: "/" + culture + '/Cart/AddToCart',
                    data: {
                        id: id,
                        languageId: culture
                    },
                    success: function (res) {
                        $('#lbl_number_items_header').text(res.length);
                    },
                    error: function (err) {
                        console.log(err);
                    }
                });
            });
        }
    }
}

function numberWithCommas(number) {
    return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

// display number as currency
let x = document.querySelectorAll("#currencyDisplay");
for (let i = 0, len = x.length; i < len; i++) {
  let num = Number(x[i].innerHTML).toLocaleString('en');
  x[i].innerHTML = num;
  x[i].classList.add("currSign");
}
