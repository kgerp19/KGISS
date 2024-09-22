var Categories = [];
var sum = 0;


$(document).ready(function () {
    $('input[type=datetime]').datepicker({
        dateFormat: "dd/M/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "2001:+50"
    });
    buttonVisibility();

});


//fetch categories from database
function LoadCategory() {
 
        
        $.ajax({
            type: "GET",
            url: '/OrderMaster/GetProductCategories',
            success: function (data) {
                Categories = $.parseJSON(data);
                
                renderCategory(element);
            }
        });
 
}


function LoadCategory() {
    $.ajax({
        url: '/OrderMaster/GetProductCategories',
        type: "Get",
        dataType: "Json",
        contentType: "application/json"
    }).done(function (data) {
        //console.log(data);
        $("#ddlcat").empty().append("<option value=" + '0'+ ">Select catagory</option>")
        for (var i = 0; i < data.length; i++) {

            $("#ddlcat").append("<option value=" + data[i].CAT_CODE + ">" + data[i].CAT_DESCR + "</option>")
        }

    })
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
            var dataa = $.parseJSON(data);
            //render products to appropriate dropdown
            renderProductSubCategory($(elementProductCategory).parents('.mycontainer').find('select.psc'), dataa);
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
            var dataa = $.parseJSON(data);
            //render products to appropriate dropdown
            renderProduct($(elementProductSubCategory).parents('.mycontainer').find('select.product'), dataa);
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
LoadCategory($('#productCategory'));




$('#add').click(function () {

    createRowForStock();
    buttonVisibility();
});

function buttonVisibility() {
    var index = $('#itemDetails').children("tr").length;
    if (index === 0) {
        $("#submit").attr("disabled", true);
    }
    else {
        $("#submit").attr("disabled", false);
    }
}
function createRowForStock() {

    var isAllValid = true;


    if ($('#product').val() === "0" || $('#product').val() === "Select") {
        isAllValid = false;
        $('#product').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#product').siblings('span.error').css('visibility', 'hidden');
    }

    if (!($('#qty').val().trim() !== '' && (parseInt($('#qty').val()) || 0))) {
        isAllValid = false;
        $('#qty').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#qty').siblings('span.error').css('visibility', 'hidden');
    }

    if (isAllValid) {

        var selectedItems = getSelectedItems();
        var index = $('#itemDetails').children("tr").length;
        var sl = index;

        var indexCell = "<td style='display:none'> <input type='hidden' id='Index " + index + "' name='StoreDetails.Index' value='" + index + "' /> </td>";
        var serialCell = "<td>" + (++sl) + "</td>";
        var removeCell = "<td><input type='button' id='removeItem' class='remove' value='x'/> </td>";
        var prodectNameCell = "<td><input type='hidden' id='ProductId" + index + "' name='StoreDetails[" + index + "].ProductId' value='" + selectedItems.ProductId + "' />" + selectedItems.ProductName + " </td>";
        var qtyCell = "<td><input type='hidden' id='Qty" + index + "' name='StoreDetails[" + index + "].Qty' value='" + selectedItems.Qty + "' />" + selectedItems.Qty + " </td>";
        //var priceCell = "<td><input type='hidden' id='UnitPrice" + index + "' name='StoreDetails[" + index + "].UnitPrice' value='" + selectedItems.UnitPrice + "' />" + selectedItems.UnitPrice + " </td>";
        var categoryCell = "<td><input type='hidden' id='CategoryId" + index + "' name='StoreDetails[" + index + "].ProductCategoryId' value='" + selectedItems.CategoryId + "' />" + selectedItems.CategoryName + " </td>";
        var subCategoryCell = "<td><input type='hidden' id='SubCategoryId" + index + "' name='StoreDetails[" + index + "].ProductSubCategoryId' value='" + selectedItems.SubCategoryId + "' />" + selectedItems.SubCategoryName + " </td>";
        //var amountCell = "<td><input type='hidden' id='Amount" + index + "' name='StoreDetails[" + index + "].TotalAmount' value='" + selectedItems.Amount + "' />" + selectedItems.Amount + " </td>";
        var createNewRow = "<tr id='" + (++sl) + "'>" + indexCell + serialCell + categoryCell + subCategoryCell + prodectNameCell + qtyCell + removeCell + " </tr>";
        $('#itemDetails').append(createNewRow);

        //sum += selectedItems.Qty * selectedItems.UnitPrice;
        //$('#tamount').val(sum);

        $('#productCategory,#productSubCategory,#product').val(2);
        $('#qty').val('');
        $('#orderItemError').empty();
    }
}

$("body").on('click', '.remove', function () {

    var rid = $(this).closest('tr').attr('id');
    if (confirm("Are you sure to remove this ?")) {
        $("#" + rid).remove();
    }
    buttonVisibility();
});

function getSelectedItems() {

    var catId = $('#productCategory option:selected').val();
    var catName = $('#productCategory option:selected').text();
    var subCatId = $('#productSubCategory option:selected').val();
    var subCatName = $('#productSubCategory option:selected').text();
    var productId = $('#product option:selected').val();
    var productName = $('#product option:selected').text();
    var qty = $('#qty').val();
    //var unitPrice = $('#unitPrice').val();
    //var amount = $('#amount').val();
    var item = {
        "CategoryId": catId,
        "CategoryName": catName,
        "SubCategoryId": subCatId,
        "SubCategoryName": subCatName,
        "ProductId": productId,
        "ProductName": productName,
        "Qty": qty
        //"UnitPrice": unitPrice,
        //"Amount": amount
    };
    return item;
}

var disamount = 0;
$("#disrate").keyup(function () {

    var disrate = ($(this).val());
    var totalamount = $("#tamount").val();
    disamount = (totalamount * disrate) / 100
    $("#disamount").val(disamount);

    grandTotal();
});

$("#disamount").keyup(function () {

    grandTotal();
});


function grandTotal() {
    var grandTotal = 0;
    if ($("#disamount").val() >= 0) {
        var total = $("#tamount").val();
        var disAmount = $("#disamount").val();
        grandTotal = total - disAmount;
    }

    $("#grandtotal").val(grandTotal);
}

$('#ddlProductCategory').change(function () {
    $.ajax({
        type: "post",
        url: "/ProductSubCategories/GetProductSubCategorySelectModelsByProductCategory",
        data: { productCategoryId: $('#ddlProductCategory').val() },
        datatype: "json",
        traditional: true,
        success: function (data) {
            var ProductSubCategory = "<select>";
            ProductSubCategory = ProductSubCategory + '<option value="">--Select--</option>';
            for (var i = 0; i < data.length; i++) {
                ProductSubCategory = ProductSubCategory + '<option value=' + data[i].Value + '>' + data[i].Text + '</option>';
            }
            ProductSubCategory = ProductSubCategory + '</select>';
            $('#ddlProductSubCategory').html(ProductSubCategory);
        }
    });
});
$('#ddlProductSubCategory').change(function () {
    $.ajax({
        type: "post",
        url: "/Products/GetProductSelectModelsByProductSubCategory",
        data: { productSubCategoryId: $('#ddlProductSubCategory').val() },
        datatype: "json",
        traditional: true,
        success: function (data) {
            var Product = "<select>";
            Product = Product + '<option value="">--Select--</option>';
            for (var i = 0; i < data.length; i++) {
                Product = Product + '<option value=' + data[i].Value + '>' + data[i].Text + '</option>';
            }
            Product = Product + '</select>';
            $('#ddlProduct').html(Product);
        }
    });
});

var amount = 0;
$("#unitPrice").keyup(function () {


    var unitPrice = ($(this).val());
    var qty = $('#qty').val();
    amount = unitPrice * qty;
    $('#amount').val(amount);
});

$("#qty").keyup(function () {
    var qty = ($(this).val());
    var unitPrice = $('#unitPrice').val();

    $('#amount').val(unitPrice * qty);
});

$('#customerId').change(function () {

    var cusId = $(this).val();

    $.ajax({
        type: "GET",
        url: "/OrderMaster/GetCustomerInfo",
        data: { 'id': cusId },
        success: function (data) {
            console.log(data);
            $('#CompanyName').val(data.Name);
            $('#Address').val(data.Address);


        },
        error: function (error) {
            console.log(error);
        }
    });

});