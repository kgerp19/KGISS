$(document).ready(function () {
    buttonVisibility();
});

var sum = 0;

$('#add').click(function () {
    createRowForStock();
    buttonVisibility();
});

function buttonVisibility() {
    var index = $('#itemDetails').children("tr").length;
    if (index === 0 || index === 1) {
        $("#submit").attr("disabled", true);
    }
    else {
        $("#submit").attr("disabled", false);
    }
}
function createRowForStock() {
    var isAllValid = true;
    //if ($('#txtLcNo').val().trim() === '') {
    //    isAllValid = false;
    //    $('#txtLcNo').siblings('span.error').css('visibility', 'visible');
    //}
    //else {
    //    $('#txtLcNo').siblings('span.error').css('visibility', 'hidden');
    //}

    //if ($('#ddlProduct').val() === "0" || $('#ddlProduct').val() === "Select") {
    //    isAllValid = false;
    //    $('#ddlProduct').siblings('span.error').css('visibility', 'visible');
    //}
    //else {
    //    $('#ddlProduct').siblings('span.error').css('visibility', 'hidden');
    //}
    if (isAllValid) {

        var selectedItems = getSelectedItems();
        var index = $('#itemDetails').children("tr").length;
        var sl = index;
        var indexCell = "<td style='display:none'> <input type='hidden' id='Index " + index + "' name='OrderDetails.Index' value='" + index + "' /> </td>";
        var serialCell = "<td>" + ++sl + "</td>";
        var removeCell = "<td><input type='button' id='removeItem' class='remove' value='x'/> </td>";
        var engineNoCell = "<td><input type='hidden' id='EngineNo" + index + "' name='ProductDetails[" + index + "].EngineNo' value='" + selectedItems.EngineNo + "' />" + selectedItems.EngineNo + " </td>";
        var chassisNoCell = "<td><input type='hidden'  id='ChassisNO" + index + "' name='ProductDetails[" + index + "].ChassissNO' value='" + selectedItems.ChassisNO + "' />" + selectedItems.ChassisNO + " </td>";
        var betteryNoCell = "<td><input type='hidden'  id='BetteryNo" + index + "' name='ProductDetails[" + index + "].BetteryNo' value='" + selectedItems.BetteryNo + "' />" + selectedItems.BetteryNo + " </td>";
        var fuelPumpCell = "<td><input type='hidden'  id='FuelPumpSlNo" + index + "' name='ProductDetails[" + index + "].FuelPumpSlNo' value='" + selectedItems.FuelPumpSlNo + "' />" + selectedItems.FuelPumpSlNo + " </td>";
        var tyreLhCell = "<td><input type='hidden'  id='RearTyreLh" + index + "' name='ProductDetails[" + index + "].RearTyreLh' value='" + selectedItems.RearTyreLh + "' />" + selectedItems.RearTyreLh + " </td>";
        var tyreRhCell = "<td><input type='hidden'  id='RearTyreRh" + index + "' name='ProductDetails[" + index + "].RearTyreRh' value='" + selectedItems.RearTyreRh + "' />" + selectedItems.RearTyreRh + " </td>";
        var lcNo = "<td hidden><input type='hidden'  id='LcNo" + index + "' name='ProductDetails[" + index + "].LcNo' value='" + selectedItems.LcNo + "' />" + selectedItems.LcNo + " </td>";
        var productId = "<td hidden><input type='hidden'  id='ProductId" + index + "' name='ProductDetails[" + index + "].ProductId' value='" + selectedItems.ProductId + "' />" + selectedItems.ProductId + " </td>";

        var createNewRow = "<tr id='" + (++sl) + "'>" + indexCell + serialCell + engineNoCell + chassisNoCell + betteryNoCell + fuelPumpCell + tyreLhCell + tyreRhCell + lcNo + productId + removeCell + " </tr>";
        $('#itemDetails').append(createNewRow);      
        
        $('#engineNo,#chassisNo,#betteryNo,#fuelPump,#rearTyreLh,#rearTyreRh').val(''); 
        $('#orderItemError').empty();
    }
}
function getSelectedItems() {
    var engineNo = $('#engineNo').val();
    var chassisNo = $('#chassisNo').val();
    var betteryNo = $('#betteryNo').val();
    var fuelPump = $('#fuelPump').val();
    var rearTyreLh = $('#rearTyreLh').val();
    var rearTyreRh = $('#rearTyreRh').val();
    var lcNo = $('#txtlcNo').val();
    var productId = $('#ddlProduct').val();
    var item = {
        "EngineNo": engineNo,
        "ChassisNO": chassisNo,
        "BetteryNo": betteryNo,
        "FuelPumpSlNo": fuelPump,
        "RearTyreLh": rearTyreLh,
        "RearTyreRh": rearTyreRh,
        "LcNo": lcNo,
        "ProductId": productId
    };
    return item;
}




$("body").on('click', '.remove', function () {

    var rid = $(this).closest('tr').attr('id');
    

    if (confirm("Are you sure to remove this ?")) {
        $("#" + rid).remove();
 
    }   
    buttonVisibility();
});














