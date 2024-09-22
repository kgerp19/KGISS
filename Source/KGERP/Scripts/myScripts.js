var Categories = [];
var sum = 0;
//fetch categories from database
function LoadCategory(element) {
    if (Categories.length == 0) {
        //ajax function for fetch data
        $.ajax({
            type: "GET",
            url: '/OrderMaster/GetProductCategories',
            success: function (data) {
                Categories = $.parseJSON(data);
                //render catagory
                renderCategory(element);
            }
        });
    }
    else {
        //render catagory to the element
        renderCategory(element);
    }
}
function renderCategory(element) {
    var $ele = $(element);
    $ele.empty();
    $ele.append($('<option/>').val('0').text('Select'));
    $.each(Categories, function (i, val) {
        $ele.append($('<option/>').val(val.Value).text(val.Text));
    });
}

//fetch product sub categories
function LoadProductSubCategory(elementProductCategory) {
    $.ajax({
        type: "GET",
        url: "/OrderMaster/GetProductSubCategories",
        data: { 'productCategoryId': $(elementProductCategory).val() },
        success: function (data) {
            var data = $.parseJSON(data);
            //render products to appropriate dropdown
            renderProductSubCategory($(elementProductCategory).parents('.mycontainer').find('select.psc'), data);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function renderProductSubCategory(element, data) {
    //render product
    var $ele = $(element);
    $ele.empty();
    $ele.append($('<option/>').val('0').text('Select'));
    $.each(data, function (i, val) {
        $ele.append($('<option/>').val(val.Value).text(val.Text));
    });
}


//fetch products
function LoadProduct(elementProductSubCategory) {
    $.ajax({
        type: "GET",
        url: "/OrderMaster/GetProducts",
        data: { 'productSubCategoryId': $(elementProductSubCategory).val() },
        success: function (data) {
            var data = $.parseJSON(data);
            //render products to appropriate dropdown
            renderProduct($(elementProductSubCategory).parents('.mycontainer').find('select.product'), data);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function renderProduct(element, data) {
    //render product
    var $ele = $(element);
    $ele.empty();
    $ele.append($('<option/>').val('0').text('Select'));
    $.each(data, function (i, val) {
        $ele.append($('<option/>').val(val.Value).text(val.Text));
    });
}

$(document).ready(function () {
    //Add button click event
    $('#add').click(function () {
        //validation and add order items

        var isAllValid = true;
        if ($('#productCategory').val() == "0") {
            isAllValid = false;
            $('#productCategory').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#productCategory').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#productSubCategory').val() == "0") {
            isAllValid = false;
            $('#productSubCategory').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#productSubCategory').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#product').val() == "0") {
            isAllValid = false;
            $('#product').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#product').siblings('span.error').css('visibility', 'hidden');
        }

        if (!($('#qty').val().trim() != '' && (parseInt($('#qty').val()) || 0))) {
            isAllValid = false;
            $('#qty').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#qty').siblings('span.error').css('visibility', 'hidden');
        }

        if (!($('#unitPrice').val().trim() != '' && !isNaN($('#unitPrice').val().trim()))) {
            isAllValid = false;
            $('#unitPrice').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#unitPrice').siblings('span.error').css('visibility', 'hidden');
        }

        if (isAllValid) {
         
            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('.pc', $newRow).val($('#productCategory').val());
            $('.psc', $newRow).val($('#productSubCategory').val());
            $('.product', $newRow).val($('#product').val());

            //Replace add button with remove button
            $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');

            //remove id attribute from new clone row
            $('#productCategory,#productSubCategory,#product,#qty,#unitPrice,#amount,#add', $newRow).removeAttr('id');
            $('span.error', $newRow).remove();
            //append clone row
            $('#orderdetailsItems').append($newRow);

            var t = 0;
            var amount = $('#amount').val();
            t = t + amount;         
            $('#tamount').val(t);
            //clear select data
            $('#productCategory,#productSubCategory,#product').val('0');
            $('#qty,#unitPrice,#amount').val('');
            $('#orderItemError').empty();
        }

    });

    //remove button click event
    $('#orderdetailsItems').on('click', '.remove', function () {
        $(this).parents('tr').remove();
    });

    $('#submit').click(function () {
        var isAllValid = true;
        //validate order items
        $('#orderItemError').text('');
        var list = [];
        var errorItemCount = 0;
        $('#orderdetailsItems tr').each(function (index, ele) {
            if ($('select.product', this).val() == "0" || (parseInt($('.qty', this).val()) || 0) == 0 || $('.unitPrice', this).val() == "" || isNaN($('.unitPrice', this).val())) {
                errorItemCount++;
                $(this).addClass('error');
            }
            else {
                var orderItem = {
                    ProductCategoryId: $('select.pc', this).val(),
                    ProductSubCategoryId: $('select.psc', this).val(),
                    ProductId: $('select.product', this).val(),
                    Qty: parseInt($('.qty', this).val()),
                    UnitPrice: parseFloat($('.unitPrice', this).val()),
                    Amount: parseFloat($('.amount', this).val())
                };
                list.push(orderItem);
            }
        });

        if (errorItemCount > 0) {
            $('#orderItemError').text(errorItemCount + " invalid entry in order item list.");
            isAllValid = false;
        }

        if (list.length == 0) {
            $('#orderItemError').text('At least 1 order item required.');
            isAllValid = false;
        }

        if ($('#orderNo').val().trim() == '') {
            $('#orderNo').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#orderNo').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#orderDate').val().trim() == '') {
            $('#orderDate').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#orderDate').siblings('span.error').css('visibility', 'hidden');
        }

        if (isAllValid) {
            var data = {
                CustomerId: $('#customerId').val().trim(),
                OrderNo: $('#orderNo').val().trim(),
                orderDateString: $('#orderDate').val().trim(),
                Remarks: $('#remarks').val().trim(),
                OrderDetails: list
            };
            $(this).val('Please wait...');

            $.ajax({
                type: 'POST',
                url: '/OrderMaster/SaveOrder',
                data: JSON.stringify(data),
                contentType: 'application/json',
                success: function (data) {
                    $('#submit').text('Save');
                    if (data.status) {
                        alert('Successfully saved');
                        //here we will clear the form
                        list = [];
                        $('#orderNo,#orderDate,#remarks').val('');
                        $('#customerId').val('');
                        $('#orderdetailsItems').empty();
                        window.location = '/OrderMaster/Index';
                    }
                    else {
                        alert('Error');
                    }
                },
                error: function (error) {
                    alert("Something error has happened");
                    $('#submit').text('Save');
                }
            });
        }

    });

});

LoadCategory($('#productCategory'));


var Customers = [];
LoadCustomer($('#customerId'));
//fetch categories from database
function LoadCustomer(element) {
    if (Customers.length == 0) {
        $.ajax({
            type: "GET",
            url: '/OrderMaster/GetCustomers',
            success: function (data) {
                Customers = $.parseJSON(data);
                //render customer
                renderCustomer(element);
            }
        });
    }
    else {
        //render catagory to the element
        renderCustomer(element);
    }
}

function renderCustomer(element) {
    var $ele = $(element);
    $ele.empty();
    $ele.append($('<option/>').val('0').text('Select'));
    $.each(Customers, function (i, val) {
        $ele.append($('<option/>').val(val.Value).text(val.Text));
    });
}

