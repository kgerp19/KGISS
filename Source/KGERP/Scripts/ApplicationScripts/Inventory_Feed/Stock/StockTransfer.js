$(document).ready(function () {
    buttonVisibility();
});

function addItemToGrid() {
    createRowForStock();
    buttonVisibility();
}

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


    if ($('#product').val() === "0" || $('#product').val() === "Select") {
        isAllValid = false;
        $('#product').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#product').siblings('span.error').css('visibility', 'hidden');
    }

    if (!($('#qty').val().trim() !== '')) {
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

        var indexCell = "<td style='display:none'> <input type='hidden' id='Index " + index + "' name='StockTransferDetail.Index' value='" + index + "' /> </td>";
        var serialCell = "<td>" + (++sl) + "</td>";
        var removeCell = "<td><input type='button' id='removeItem' class='remove' value='x'/> </td>";
        var prodectNameCell = "<td><input type='hidden' id='ProductId" + index + "' name='StockTransferDetail[" + index + "].ProductId' value='" + selectedItems.ProductId + "' />" + selectedItems.ProductName + " </td>";
        var qtyCell = "<td><input type='hidden' id='TransferQty' class='quantityCell'" + index + "' name='StockTransferDetail[" + index + "].TransferQty' value='" + selectedItems.TransferQty + "' />" + selectedItems.TransferQty + " </td>";

        var createNewRow = "<tr id='" + (++sl) + "'>" + indexCell + serialCell + prodectNameCell + qtyCell + removeCell + " </tr>";
        $('#itemDetails').append(createNewRow);


        $('#product').val('');
        $('#qty').val('');
        $('#stockAvailableQty').val('');
        $('#orderItemError').empty();

        //Adding Sum of Amount Cell
        calculateQuantitySum();
    }
    function getSelectedItems() {
        var productId = $('#hfProductId').val();
        var productName = $('#product').val();
        var qty = $('#qty').val();

        var item = {
            "ProductId": productId,
            "ProductName": productName,
            "TransferQty": qty
        };
        return item;
    }
}

function calculateQuantitySum() {
    var sumQty = 0;
    var sumAmount = 0;
    $(".quantityCell").each(function () {
        var value = $(this).val();
        console.log(value);
        if (!isNaN(value) && value.length !== 0) {
            sumQty += parseFloat(value);
        }
    });

    var sumRow = "<tr class='trAmount' style='background-color: #89CFF0' ><td colspan='2'><b>Total</b></td><td colspan='2'><b>" + parseFloat(sumQty).toFixed(2) + "</b></td></tr>";
    $('#itemDetails').append(sumRow);
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










