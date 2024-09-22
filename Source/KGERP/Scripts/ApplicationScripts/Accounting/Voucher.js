$(document).ready(function () {
    $("#submit").attr("disabled", true);
});

$('#add').click(function () {    
    if ($("#accountName").val().length == 0) {
        $('#accountName').css('border-color', 'red');
        alert("Please select an account first.");
        return false;
    }
    else {
        $('#accountName').css('border-color', '');
    }
    createRow();
});

function createRow() {
    var isAllValid = true;
    if ($('#txtDebitAmount').val() === '' && ($('#txtCreditAmount').val() === '')) {
        isAllValid = false;
        $('#txtDebitAmount').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#txtDebitAmount').siblings('span.error').css('visibility', 'hidden');
    }

    if (!($('#accountName').val().trim() !== '')) {
        isAllValid = false;
        $('#accountName').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#accountName').siblings('span.error').css('visibility', 'hidden');
    }

    if (isAllValid) {
        //Removing QuantitySum of Quantity Cell
        $(".trAmount").remove();

        var selectedItems = getSelectedItems();
        var index = $('#voucherItems').children("tr").length;
        var sl = index;

        var indexCell = "<td style='display:none'> <input type='hidden' id='Index " + index + "' name='VoucherDetails.Index' value='" + index + "' /> </td>";
        var serialCell = "<td>" + ++sl + "</td>";
        var accName = "<td><input type='hidden' id='AccountHeadId" + index + "' name='VoucherDetails[" + index + "].AccountHeadId' value='" + selectedItems.AccountHeadId + "' />" + selectedItems.AccCode + " </td>";
        var accCode = "<td><input type='hidden' id='AccCode" + index + "' name='VoucherDetails[" + index + "].AccCode' value='" + selectedItems.AccCode + "' />" + selectedItems.AccName + " </td>";
        var particular = "<td><input type='hidden'  id='Particular" + index + "' name='VoucherDetails[" + index + "].Particular' value='" + selectedItems.Particular + "' />" + selectedItems.Particular + " </td>";
        var debit = "<td><input  class='debitCell' type='hidden' id='DebitAmount" + index + "' name='VoucherDetails[" + index + "].DebitAmount' value='" + selectedItems.DebitAmount + "' />" + selectedItems.DebitAmount + " </td>";
        var credit = "<td><input class='creditCell' type='hidden' id='CreditAmount" + index + "' name='VoucherDetails[" + index + "].CreditAmount' value='" + selectedItems.CreditAmount + "' />" + selectedItems.CreditAmount + " </td>";
        var removeCell = "<td><input type='button' id='removeItem' class='remove btn btn-danger btn-xs' value='Remove'/> </td>";
        var createNewRow = "<tr id='" + (++sl) + "'>" + indexCell + accCode + accName + particular + debit + credit + removeCell + " </tr>";

        $('#voucherItems').append(createNewRow);
        $('#accountName').val('');
        $('#txtDebitAmount,#txtCreditAmount').val('');
        $('#orderItemError').empty();

        //Adding Sum of Amount Cell
        calculateQuantitySum();

    }
}

function calculateQuantitySum() {
    var debitAmount = 0;
    var creditAmount = 0;
    $(".debitCell").each(function () {
        var value = $(this).val();

        if (!isNaN(value) && value.length !== 0) {
            debitAmount += parseFloat(value);
        }
    });

    $(".creditCell").each(function () {
        var value = $(this).val();

        if (!isNaN(value) && value.length !== 0) {
            creditAmount += parseFloat(value);
        }
       
    });

    var sumRow = "<tr class='trAmount' style='background-color: #add8e6' ><td colspan='2'><b>Total</b></td><td></td><td><b>" + parseFloat(debitAmount).toFixed(2) + "</b></td><td colspan='2'><b>" + parseFloat(creditAmount).toFixed(2) + "</b></td></tr>";
    $('#voucherItems').append(sumRow);
    debitAmount = Number(debitAmount).toFixed(2);
    creditAmount = Number(creditAmount).toFixed(2);


    if (debitAmount === creditAmount) {
        $("#submit").attr("disabled", false);
    }
    else {
        $("#submit").attr("disabled", true);
    }
}


$("body").on('click', '.remove', function () {
    var rid = $(this).closest('tr').attr('id');
    if (confirm("Are you sure you want to remove this ?")) {
        $("#" + rid).remove();
        $(".trAmount").remove();

        if ($('#divVoucherGrid tr').length > 1) {
            calculateQuantitySum();
        }
    }
});

function getSelectedItems() {

    var accId = $('#accountHeadId').val();
    var string = $('#accountName').val();
    var Name = string.split("]")[0];
    var accName = Name.substring(1);
    var accCode = string.split("]")[1];
    var particular = $('#particular').val();
    var debitAmount = parseFloat($('#txtDebitAmount').val());
    if (isNaN(debitAmount)) {
        debitAmount = 0.00;
    }

    var creditAmount = parseFloat($('#txtCreditAmount').val());

    if (isNaN(creditAmount)) {
        creditAmount = 0.00;
    }
    var item = {
        "AccountHeadId": accId,
        "AccName": accName,
        "AccCode": accCode,
        "Particular": particular,
        "DebitAmount": debitAmount,
        "CreditAmount": creditAmount
    };
    return item;
}

























