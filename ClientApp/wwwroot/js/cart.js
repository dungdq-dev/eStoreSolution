﻿class CartController {
  constructor() {
    const BASE_ADDRESS = "https://localhost:8080/";

    this.initialize = function () {
      loadData();

      registerEvents();
    };

    function registerEvents() {
      $('body').on('click', '.btn-plus', function (e) {
        e.preventDefault();
        const id = $(this).data('id');
        const quantity = parseInt($('#txt_quantity_' + id).val()) + 1;
        updateCart(id, quantity);
      });

      $('body').on('click', '.btn-minus', function (e) {
        e.preventDefault();
        const id = $(this).data('id');
        const quantity = parseInt($('#txt_quantity_' + id).val()) - 1;
        updateCart(id, quantity);
      });
      $('body').on('click', '.btn-remove', function (e) {
        e.preventDefault();
        const id = $(this).data('id');
        updateCart(id, 0);
      });
    }

    function updateCart(id, quantity) {
      const culture = $('#hidCulture').val();
      $.ajax({
        type: "POST",
        url: "/" + culture + '/Cart/UpdateCart',
        data: {
          id: id,
          quantity: quantity
        },
        success: function (res) {
          $('#lbl_number_items_header').text(res.length);
          loadData();
        },
        error: function (err) {
          console.log(err);
        }
      });
    }

    function loadData() {
      const culture = $('#hidCulture').val();
      $.ajax({
        type: "GET",
        url: "/" + culture + '/Cart/GetListItems',
        success: function (res) {
          if (res.length === 0) {
            $('#tbl_cart').hide();
          }
          let html = '';
          let total = 0;

          $.each(res, function (i, item) {
            let amount = item.price * item.quantity;
            html += "<tr>"
              + "<td> <img width=\"60\" src=\"" + BASE_ADDRESS + item.image + "\" alt=\"\" /></td>"
              + "<td>" + item.name + "</td>"
              + "<td>"
              + "<div class=\"input-append\"><input disabled class=\"span1\" style=\"max-width: 34px\" placeholder=\"1\" id=\"txt_quantity_" + item.productId + "\" value=\"" + item.quantity + "\" size=\"16\" type=\"text\">"
              + "<button id=\"btnMinus\" class=\"btn btn-minus\" data-id=\"" + item.productId + "\" type =\"button\"> <i class=\"icon-minus\"></i></button>"
              + "<button id=\"btnPlus\" class=\"btn btn-plus\" type=\"button\" data-id=\"" + item.productId + "\"><i class=\"icon-plus\"></i></button>"
              + "<button class=\"btn btn-danger btn-remove\" type=\"button\" data-id=\"" + item.productId + "\"><i class=\"icon-remove icon-white\"></i></button>"
              + "</div>"
              + "<p>Còn lại: " + item.productStock + " sản phẩm</p>"
              + "</td>"
              + "<td>" + numberWithCommas(item.price) + "</td>"
              + "<td>" + numberWithCommas(amount) + "</td>"
              + "</tr>";
            total += amount;
          });

          $('#cart_body').html(html);
          $('#lbl_number_of_items').text(res.length);
          $('#lbl_total').text(numberWithCommas(total));
        }
      });
    }
  }
}