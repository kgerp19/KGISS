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
    if (index <= 1) {
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

    if (!($('#qty').val().trim() !== '' && (parseFloat($('#qty').val()) || 0.0))) {
        isAllValid = false;
        $('#qty').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#qty').siblings('span.error').css('visibility', 'hidden');
    }

    if (!($('#rate').val().trim() !== '' && (parseFloat($('#rate').val()) || 0.0))) {
        isAllValid = false;
        $('#rate').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#rate').siblings('span.error').css('visibility', 'hidden');
    }



    if (isAllValid) {
        //Removing QuantitySum of Quantity Cell
        $(".trAmount").remove();

        var selectedItems = getSelectedItems();
        var index = $('#itemDetails').children("tr").length;
        var sl = index;

        var indexCell = "<td style='display:none'> <input type='hidden' id='Index " + index + "' name='RentProductionDetails.Index' value='" + index + "' /> </td>";
        var serialCell = "<td>" + ++sl + "</td>";
        var removeCell = "<td><input type='button' id='removeItem' class='remove' value='x'/> </td>";
        var prodectNameCell = "<td><input type='hidden' id='ProductId" + index + "' name='RentProductionDetails[" + index + "].ProductId' value='" + selectedItems.ProductId + "' />" + selectedItems.ProductName + " </td>";
        var dieSizeCell = "<td><input type='hidden' class='dieSizeCell' id='dieSize" + index + "' name='RentProductionDetails[" + index + "].DieSize' value='" + selectedItems.DieSize + "' />" + selectedItems.DieSize + " </td>";
        var qtyCell = "<td><input type='hidden' class='quantityCell' id='Qty" + index + "' name='RentProductionDetails[" + index + "].Qty' value='" + selectedItems.Qty + "' />" + selectedItems.Qty + " </td>";
        var rateCell = "<td><input type='hidden' id='rate" + index + "' name='RentProductionDetails[" + index + "].Rate' value='" + selectedItems.Rate + "' />" + selectedItems.Rate + " </td>";
        var amountCell = "<td><input class='totalPrice' type='hidden' id='Amount" + index + "' name='RentProductionDetails[" + index + "].Amount' value='" + selectedItems.Amount + "' />" + selectedItems.Amount + " </td>";
        var createNewRow = "<tr id='" + (++sl) + "'>" + indexCell + serialCell + prodectNameCell + dieSizeCell + qtyCell + rateCell + amountCell + removeCell + " </tr>";
        $('#itemDetails').append(createNewRow);

        sum += selectedItems.Qty * selectedItems.Rate;
        $('#tamount').val(sum);
        $('#grandtotal').val(sum);

        $('#product').val('');
        $('#qty,#rate,#amount').val('');
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

    var sumRow = "<tr class='trAmount' style='background-color: #FFFF00' ><td colspan='3'><b>Total</b></td><td colspan='2'><b>" + parseFloat(sumQty).toFixed(2) + "</b></td><td colspan='2'><b>" + parseFloat(sumAmount).toFixed(2) + "</b></td></tr>";
    $('#itemDetails').append(sumRow);
}


function getSelectedItems() {
    var productId = $('#hfProductId').val();
    var productName = $('#product').val();
    var dieSize = $('#dieSize').val();
    var qty = $('#qty').val();
    var rate = $('#rate').val();
    var amount = $('#amount').val();
    var item = {
        "ProductId": productId,
        "ProductName": productName,
        "DieSize": dieSize,
        "Qty": qty,
        "Rate": rate,
        "Amount": amount,
    };
    return item;
}



var amount = 0;
$("#qty").keyup(function () {
    var qty = $(this).val();
    var rate = $('#rate').val();
    $('#amount').val(parseFloat(rate * qty).toFixed(2));
});

$("#rate").keyup(function () {
    var rate = ($(this).val());
    var qty = $('#qty').val();
    amount = parseFloat(rate * qty).toFixed(2);
    $('#amount').val(parseFloat(amount).toFixed(2));
});






