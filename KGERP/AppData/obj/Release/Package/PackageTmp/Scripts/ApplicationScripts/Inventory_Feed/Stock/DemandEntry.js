$(document).ready(function () {
    buttonVisibility();
});

var sum = 0;
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
        //Removing Sum of Amount Cell
        $(".trSumQty").remove();

        var selectedItems = getSelectedItems();
        var index = $('#itemDetails').children("tr").length;
        var sl = index;

        var indexCell = "<td style='display:none'> <input type='hidden' id='Index " + index + "' name='DemandItems.Index' value='" + index + "' /> </td>";
        var serialCell = "<td>" + (++sl) + "</td>";
        var removeCell = "<td><input type='button' id='removeItem' class='remove' value='x'/> </td>";
        var prodectNameCell = "<td><input type='hidden' id='ProductId" + index + "' name='DemandItems[" + index + "].ProductId' value='" + selectedItems.ProductId + "' />" + selectedItems.ProductName + " </td>";
        var qtyCell = "<td><input type='hidden' class='cellQty' id='Qty"  + index + "' name='DemandItems[" + index + "].Qty' value='" + selectedItems.Qty + "' />" + selectedItems.Qty + " </td>";
        var createNewRow = "<tr id='" + (++sl) + "'>" + indexCell + serialCell + prodectNameCell + qtyCell + removeCell + " </tr>";
        $('#itemDetails').append(createNewRow);

        $('#product').val('');
        $('#qty').val('');
        $('#orderItemError').empty();

        //Adding Sum of Amount Cell
        calculateSum();

    }
}
function calculateSum() {
    var sum = 0;
    $(".cellQty").each(function () {
        var value = $(this).val();
        console.log(value);
        if (!isNaN(value) && value.length !== 0) {
            sum += parseFloat(value);
        }
    });
    var sumRow = "<tr class='trSumQty' ><td colspan='2'><b>Total</b></td><td colspan='2'><b>" + parseFloat(sum).toFixed(0) + "</b></td></tr>";
    $('#itemDetails').append(sumRow);
}

$("body").on('click', '.remove', function () {
    var rid = $(this).closest('tr').attr('id');
    if (confirm("Are you sure to remove this ?")) {
        $("#" + rid).remove();
        $(".trAmount").remove();
    }
    $(".trSumQty").remove();
    var rowCount = $('#orderdetailsItems tr').length;
    if (rowCount > 2) {
        calculateSum();
    }

    buttonVisibility();
});

function getSelectedItems() {
    var productId = $('#hfProductId').val();
    var productName = $('#product').val();
    var qty = $('#qty').val();

    var item = {
        "ProductId": productId,
        "ProductName": productName,
        "Qty": qty
    };
    return item;
}



$("#qty").keyup(function () {
    var qty = $(this).val();
    if (isNaN(qty)) {
        alert("Plese select valid number");
        $('#qty').val('');
    }
});



