$(document).ready(function () {
    buttonVisibility();
});

var sum = 0;

$('#add').click(function () {
    createRowForStock();
    buttonVisibility();
});

$(document).keypress(function (e) {
    if (e.which === 13) {
        addItemToGrid();
    }
});

function addItemToGrid() {
    createRowForStock();
    buttonVisibility();
}


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
    if ($('#product').val() === "0" || ($('#product').val() === "Select")) {
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
        //Removing QuantitySum of Quantity Cell
        $(".trAmount").remove();

        var selectedItems = getSelectedItems();
        var index = $('#itemDetails').children("tr").length;
        var sl = index;

        var indexCell = "<td style='display:none'> <input type='hidden' id='Index " + index + "' name='StoreDetails.Index' value='" + index + "' /> </td>";
        var serialCell = "<td>" + ++sl + "</td>";
        var removeCell = "<td><input type='button' id='removeItem' class='remove' value='x'/> </td>";
        var prodectNameCell = "<td><input type='hidden' id='ProductId" + index + "' name='StoreDetails[" + index + "].ProductId' value='" + selectedItems.ProductId + "' />" + selectedItems.ProductName + " </td>";
        var qtyCell = "<td><input type='hidden' class='quantityCell' id='Qty" + index + "' name='StoreDetails[" + index + "].Qty' value='" + selectedItems.Qty + "' />" + selectedItems.Qty + " </td>";
        var priceCell = "<td><input type='hidden' id='UnitPrice" + index + "' name='StoreDetails[" + index + "].UnitPrice' value='" + selectedItems.UnitPrice + "' />" + selectedItems.UnitPrice + " </td>";
        var amountCell = "<td><input class='totalPrice' type='hidden' id='Amount" + index + "' name='StoreDetails[" + index + "].Amount' value='" + selectedItems.Amount + "' />" + selectedItems.Amount + " </td>";
        var avgPriceCell = "<td hidden><input type='hidden' id='AvgParchaseRate" + index + "' name='StoreDetails[" + index + "].AvgParchaseRate' value='" + selectedItems.AvgParchaseRate + "' />" + +" </td>";
        var createNewRow = "<tr id='" + (++sl) + "'>" + indexCell + serialCell + prodectNameCell + qtyCell + priceCell + amountCell + avgPriceCell + removeCell + " </tr>";
        $('#itemDetails').append(createNewRow);

        sum += selectedItems.Qty * selectedItems.UnitPrice;
        $('#tamount').val(sum);
        $('#grandtotal').val(sum);

        $('#product').val('');
        $('#qty,#unitPrice,#amount').val('');
        $('#orderItemError').empty();

        //Adding Sum of Amount Cell
        calculateQuantitySum();

    }
}

$("body").on('click', '.remove', function () {
    var rid = $(this).closest('tr').attr('id');
    if (confirm("Are you sure to remove this ?")) {
        $("#" + rid).remove();
        $(".trAmount").remove();
    }
    calculateQuantitySum();
    buttonVisibility();
});

function calculateQuantitySum() {
    var sumQty = 0;
    var sumAmount = 0;
    $(".quantityCell").each(function () {
        var value = $(this).val();
        if (!isNaN(value) && value.length !== 0) {
            sumQty += parseFloat(value);
        }
    });

    $(".totalPrice").each(function () {
        var value = $(this).val();
        console.log(value);
        if (!isNaN(value) && value.length !== 0) {
            sumAmount += parseFloat(value);
        }
    });

    var sumRow = "<tr class='trAmount' style='background-color: #89CFF0' ><td colspan='2'><b>Total</b></td><td colspan='2'><b>" + parseFloat(sumQty).toFixed(2) + "</b></td><td colspan='2'><b>" + parseFloat(sumAmount).toFixed(2) + "</b></td></tr>";
    $('#itemDetails').append(sumRow);
}


function getSelectedItems() {
    var productId = $('#hfProductId').val();
    var productName = $('#product').val();
    var qty = $('#qty').val();
    var unitPrice = $('#unitPrice').val();
    var amount = $('#amount').val();
    var avgPrice = $('#avgPrice').val();
    var item = {
        "ProductId": productId,
        "ProductName": productName,
        "Qty": qty,
        "UnitPrice": unitPrice,
        "Amount": amount,
        "AvgParchaseRate": avgPrice
    };
    return item;
}



var amount = 0;
$("#unitPrice").keyup(function () {
    var unitPrice = ($(this).val());
    var qty = $('#qty').val();
    amount = parseFloat(unitPrice * qty).toFixed(2);
    $('#amount').val(parseFloat(amount).toFixed(2));
});

$("#qty").keyup(function () {
    var qty = $(this).val();
    var unitPrice = $('#unitPrice').val();
    $('#amount').val(parseFloat(unitPrice * qty).toFixed(2));
});

//$("#qty").keyup(function () {
//    var qty = $(this).val();
//    if (isNaN(qty)) {
//        alert("Plese select valid number");
//        $('#qty').val('');
//    }
//    var unitPrice = $('#unitPrice').val();
//    $('#amount').val(parseFloat(unitPrice * qty).toFixed(2));
//});




