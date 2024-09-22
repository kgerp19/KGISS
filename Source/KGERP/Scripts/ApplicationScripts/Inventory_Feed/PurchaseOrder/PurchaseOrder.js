$(document).ready(function () {
    buttonVisibility();
});

$('#btnAdd').click(function () {
    createRow();
    buttonVisibility();
});

function buttonVisibility() {
    var index = $('#itemDetails').children("tr").length;
    if (index === 0) {
        $("#btnSubmit").attr("disabled", true);
    }
    else {
        $("#btnSubmit").attr("disabled", false);
    }
}
function createRow() {
    var result = validateInput();

    if (!result) {
        return !result;
    }
    var selectedItems = getSelectedItems();
    var index = $('#itemDetails').children("tr").length;
    var sl = index;

    var indexCell = "<td style='display:none'> <input type='hidden' id='Index " + index + "' name='PurchaseOrder.PurchaseOrderDetails.Index' value='" + index + "' /> </td>";

    var prodectNameCell = "<td><input type='hidden' id='ProductId" + index + "' name='PurchaseOrder.PurchaseOrderDetails[" + index + "].ProductId' value='" + selectedItems.ProductId + "' />" + selectedItems.ProductName + " </td>";
    var unitNameCell = "<td><input type='hidden' id='UnitId" + index + "' name='PurchaseOrder.PurchaseOrderDetails[" + index + "].UnitId' value='" + selectedItems.UnitId + "' />" + selectedItems.UnitName + " </td>";
    var presentStockCell = "<td><input type='hidden' id='PresentStock" + index + "' name='PurchaseOrder.PurchaseOrderDetails[" + index + "].PresentStock' value='" + selectedItems.PresentStock + "' />" + selectedItems.PresentStock + " </td>";
    var requiredQtyCell = "<td><input type='hidden' id='RequiredQty" + index + "' name='PurchaseOrder.PurchaseOrderDetails[" + index + "].RequiredQty' value='" + selectedItems.RequiredQty + "' />" + selectedItems.RequiredQty + " </td>";
    var purchasedQtyCell = "<td><input type='hidden' id='PurchasedQty" + index + "' name='PurchaseOrder.PurchaseOrderDetails[" + index + "].PurchasedQty' value='" + selectedItems.PurchasedQty + "' />" + selectedItems.PurchasedQty + " </td>";
    var dueAmountCell = "<td><input type='hidden' id='DueAmount" + index + "' name='PurchaseOrder.PurchaseOrderDetails[" + index + "].DueAmount' value='" + selectedItems.DueAmount + "' />" + selectedItems.DueAmount + " </td>";
    var demandRateCell = "<td><input type='hidden' id='DemandRate" + index + "' name='PurchaseOrder.PurchaseOrderDetails[" + index + "].DemandRate' value='" + selectedItems.DemandRate + "' />" + selectedItems.DemandRate + " </td>";
    var purchaseQtyCell = "<td><input type='hidden' id='PurchaseQty" + index + "' name='PurchaseOrder.PurchaseOrderDetails[" + index + "].PurchaseQty' value='" + selectedItems.PurchaseQty + "' />" + selectedItems.PurchaseQty + " </td>";
    var purchaseRateCell = "<td><input type='hidden' id='PurchaseRate" + index + "' name='PurchaseOrder.PurchaseOrderDetails[" + index + "].PurchaseRate' value='" + selectedItems.PurchaseRate + "' />" + selectedItems.PurchaseRate + " </td>";
    var amountCell = "<td><input type='hidden' id='Amount" + index + "' name='PurchaseOrder.PurchaseOrderDetails[" + index + "].Amount' value='" + selectedItems.Amount + "' />" + selectedItems.Amount + " </td>";
    var packSizeCell = "<td><input type='hidden' id='PackSize" + index + "' name='PurchaseOrder.PurchaseOrderDetails[" + index + "].PackSize' value='" + selectedItems.PackSize + "' />" + selectedItems.PackSize + " </td>";

    var removeCell = "<td><input type='button' id='removeItem' class='remove btn btn-danger btn-xs' value='Remove'/> </td>";

    var createNewRow = "<tr id='" + (++sl) + "'>" + indexCell + prodectNameCell + unitNameCell + presentStockCell + requiredQtyCell + purchasedQtyCell + dueAmountCell + demandRateCell + purchaseQtyCell + purchaseRateCell + amountCell + packSizeCell + removeCell + " </tr>";
    $('#itemDetails').append(createNewRow);
    $('#txtUnitName,#txtPresentStock,#txtRequiredQty,#txtPurchasedQty,#txtDueAmount,#txtDemandRate,#txtPurchaseQty,#txtPurchaseRate,#txtAmount,#txtPackSize').val('');
}
function validateInput() {
    if ($("#ddlRawMaterial option:selected").val() == undefined || $("#ddlRawMaterial option:selected").val() == "") {
        alert("Please select a Raw Material");
        return false;
    }
    else if ($("#txtPurchaseQty").val() == 0 || $("#txtPurchaseQty").val() == "") {
        alert("Please enter Purchase Qty");
        return false;
    }

    else if ($("#txtPurchaseRate").val() == 0 || $("#txtPurchaseQty").val() == "") {
        alert("Please enter Purchase Rate");
        return false;
    }
    else {
        return true;
    }
}

$("body").on('click', '.remove', function () {
    var rid = $(this).closest('tr').attr('id');
    $("#" + rid).remove();
    buttonVisibility();
});

function getSelectedItems() {
    var productId = $('#ddlRawMaterial option:selected').val();
    var unitId = $('#hfUnitId').val();
    var productName = $('#ddlRawMaterial option:selected').text();
    var unitName = $('#txtUnitName').val();
    var presentStock = $('#txtPresentStock').val();
    var requiredQty = $('#txtRequiredQty').val();
    var purchasedQty = $('#txtPurchasedQty').val();
    var dueAmount = $('#txtDueAmount').val();
    var demandRate = $('#txtDemandRate').val();
    var purchaseQty = $('#txtPurchaseQty').val();
    var purchaseRate = $('#txtPurchaseRate').val();
    var amount = $('#txtAmount').val();
    var packSize = $('#txtPackSize').val();
    var item = {
        "ProductId": productId,
        "ProductName": productName,
        "UnitId": unitId,
        "UnitName": unitName,
        "PresentStock": presentStock,
        "RequiredQty": requiredQty,
        "PurchasedQty": purchasedQty,
        "DueAmount": dueAmount,
        "DemandRate": demandRate,
        "PurchaseQty": purchaseQty,
        "PurchaseRate": purchaseRate,
        "Amount": amount,
        "PackSize": packSize,
    };
    return item;
}






















