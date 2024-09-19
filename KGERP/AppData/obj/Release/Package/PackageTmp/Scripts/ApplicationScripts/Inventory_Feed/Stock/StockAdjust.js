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
    if ($('#product').val() === "") {
        isAllValid = false;
        $('#product').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#product').siblings('span.error').css('visibility', 'hidden');
    }

    //if (!($('#excessQty').val().trim() !== '' && (parseFloat($('#excessQty').val())< 0))) {
    //    isAllValid = false;
    //    $('#excessQty').siblings('span.error').css('visibility', 'visible');
    //}
    //else {
    //    $('#excessQty').siblings('span.error').css('visibility', 'hidden');
    //}

    //if (!($('#lessQty').val().trim() !== '' && (parseFloat($('#lessQty').val()) < 0))) {
    //    isAllValid = false;
    //    $('#lessQty').siblings('span.error').css('visibility', 'visible');
    //}
    //else {
    //    $('#lessQty').siblings('span.error').css('visibility', 'hidden');
    //}

    if ($('#unitPrice').val().trim() == '' || parseFloat($('#unitPrice').val()) <= 0) {
        isAllValid = false;
        $('#unitPrice').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#unitPrice').siblings('span.error').css('visibility', 'hidden');
    }


    if (isAllValid) {
        //Removing QuantitySum of Quantity Cell
        $(".trAmount").remove();

        var selectedItems = getSelectedItems();
        console.log(selectedItems);
        var index = $('#itemDetails').children("tr").length;
        var sl = index;

        var indexCell = "<td style='display:none'> <input type='hidden' id='Index " + index + "' name='StockAdjustDetails.Index' value='" + index + "' /> </td>";
        var serialCell = "<td>" + ++sl + "</td>";
        var removeCell = "<td><input type='button' id='removeItem' class='remove' value='x'/> </td>";
        var prodectNameCell = "<td><input type='hidden' id='ProductId" + index + "' name='StockAdjustDetails[" + index + "].ProductId' value='" + selectedItems.ProductId + "' />" + selectedItems.ProductName + " </td>";
        var excessQtyCell = "<td><input type='hidden' class='excessQtyCell' id='excessQty" + index + "' name='StockAdjustDetails[" + index + "].ExcessQty' value='" + selectedItems.ExcessQty + "' />" + selectedItems.ExcessQty + " </td>";
        var lessQtyCell = "<td><input type='hidden' class='lessQtyCell' id='lessQty" + index + "' name='StockAdjustDetails[" + index + "].LessQty' value='" + selectedItems.LessQty + "' />" + selectedItems.LessQty + " </td>";
        var priceCell = "<td><input type='hidden' id='UnitPrice" + index + "' name='StockAdjustDetails[" + index + "].UnitPrice' value='" + selectedItems.UnitPrice + "' />" + selectedItems.UnitPrice + " </td>";
        var createNewRow = "<tr id='" + (++sl) + "'>" + indexCell + serialCell + prodectNameCell + excessQtyCell + lessQtyCell + priceCell  + removeCell + " </tr>";
        $('#itemDetails').append(createNewRow);

        $('#product').val('');
        $('#excessQty,#lessQty,#unitPrice').val(0);
        $('#orderItemError').empty();
        //Adding Sum of Amount Cell
        calculateQuantitySum();

    }
}

$("body").on('click', '.remove', function () {

    var rid = $(this).closest('tr').attr('id');
    if (confirm("Are you sure to remove this ?")) {
        $("#" + rid).remove();
    }
    $(".trAmount").remove();
    calculateQuantitySum()
    buttonVisibility();
});

function calculateQuantitySum() {
    var sumQty = 0;
    var sumExcessQty = 0;
    var sumLessQty = 0;
    $(".excessQtyCell").each(function () {
        var value = $(this).val();
        console.log(value);
        if (!isNaN(value) && value.length !== 0) {
            sumExcessQty += parseFloat(value);
        }
    });
    $(".lessQtyCell").each(function () {
        var value = $(this).val();
        console.log(value);
        if (!isNaN(value) && value.length !== 0) {
            sumLessQty += parseFloat(value);
        }
    });

    $(".totalPrice").each(function () {
        var value = $(this).val();
        console.log(value);
        if (!isNaN(value) && value.length !== 0) {
            sumAmount += parseFloat(value);
        }
    });

    var sumRow = "<tr class='trAmount' style='background-color: #89CFF0' ><td colspan='2'><b>Total</b></td><td><b>" + parseFloat(sumExcessQty).toFixed(2) + "</b></td><td colspan='3'>"+ parseFloat(sumLessQty).toFixed(2) +"</td></tr>";
    $('#itemDetails').append(sumRow);
}

function getSelectedItems() {
    var productId = $('#hfProductId').val();
    var productName = $('#product').val();
    var excessQty = $('#excessQty').val();
    var lessQty = $('#lessQty').val();
    var unitPrice = $('#unitPrice').val();

   
    var item = {
        "ProductId": productId,
        "ProductName": productName,
        "ExcessQty": excessQty,
        "LessQty": lessQty,
        "UnitPrice": unitPrice
    };

    return item;
}









