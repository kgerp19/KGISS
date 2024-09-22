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

    if (!($('#usdPrice').val().trim() !== '')) {
        isAllValid = false;
        $('#usdPrice').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#usdPrice').siblings('span.error').css('visibility', 'hidden');
    }

    if (isAllValid) {
        var createNewRow;
        var companyId = parseInt($("#companyId").val());
        
        var selectedItems = getSelectedItems();
        var index = $('#itemDetails').children("tr").length;
        var sl = index;

        var indexCell = "<td style='display:none'> <input type='hidden' id='Index " + index + "' name='StoreDetails.Index' value='" + index + "' /> </td>";
        var serialCell = "<td>" + (++sl) + "</td>";
        var removeCell = "<td><input type='button' id='removeItem' class='remove' value='x'/> </td>";
        var prodectNameCell = "<td><input type='hidden' id='ProductId" + index + "' name='StoreDetails[" + index + "].ProductId' value='" + selectedItems.ProductId + "' />" + selectedItems.ProductName + " </td>";
        var usdPriceCell = "<td><input type='hidden' id='USDPrice" + index + "' name='StoreDetails[" + index + "].USDPrice' value='" + selectedItems.USDPrice + "' />" + selectedItems.USDPrice + " </td>";
        var bdtPrice = "<td><input type='hidden' id='BDTPrice" + index + "' name='StoreDetails[" + index + "].BDTPrice' value='" + selectedItems.BDTPrice + "' />" + selectedItems.BDTPrice + " </td>";
        var invoiceValue = "<td><input type='hidden' id='InvoiceValue" + index + "' name='StoreDetails[" + index + "].InvoiceValue' value='" + selectedItems.InvoiceValue + "' />" + selectedItems.InvoiceValue + " </td>";
        var landedCost = "<td><input type='hidden' id='LandedCost" + index + "' name='StoreDetails[" + index + "].LandedCost' value='" + selectedItems.LandedCost + "' />" + selectedItems.LandedCost + " </td>";
        var totalCogs = "<td><input type='hidden' id='TotalCOGS" + index + "' name='StoreDetails[" + index + "].TotalCOGS' value='" + selectedItems.TotalCOGS + "' />" + selectedItems.TotalCOGS + " </td>";
        var unitPrice = "<td hidden><input type='hidden' id='UnitPrice" + index + "' name='StoreDetails[" + index + "].UnitPrice' value='" + selectedItems.CogsPerUnit + "' />" + selectedItems.CogsPerUnit + " </td>";
        var cogsPerUnit = "<td><input type='hidden' id='CogsPerUnit" + index + "' name='StoreDetails[" + index + "].CogsPerUnit' value='" + selectedItems.CogsPerUnit + "' />" + selectedItems.CogsPerUnit + " </td>";
        var qtyCell = "<td><input type='hidden' id='Qty" + index + "' name='StoreDetails[" + index + "].Qty' value='" + selectedItems.Qty + "' />" + selectedItems.Qty + " </td>";
       
        //var unitPriceCell = "<td><input type='hidden' id='UnitPrice" + index + "' name='StoreDetails[" + index + "].UnitPrice' value='" + selectedItems.ProductionRate + "' />" + selectedItems.ProductionRate + " </td>";
        
        createNewRow = "<tr id='" + (++sl) + "'>" + indexCell + serialCell + prodectNameCell + usdPriceCell + bdtPrice + qtyCell + invoiceValue + landedCost + totalCogs + cogsPerUnit + unitPrice+ removeCell + " </tr>";
           
       
     
        $('#itemDetails').append(createNewRow);
        

        //sum += selectedItems.Qty * selectedItems.UnitPrice;
        //$('#tamount').val(sum);

        $('#product').val('');
        $('#qty').val('');
        $('#usdPrice').val('');
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
    var insurencePremiumCharge = $("#insurencePremiumCharge").val() === '' ? 0 : $("#insurencePremiumCharge").val();
    var bankCharge = $("#bankCharge").val() === '' ? 0 : $("#bankCharge").val();
    var customDutyCharge = $("#customDutyCharge").val() === '' ? 0 : $("#customDutyCharge").val();
    var otherCharge = $("#otherCharge").val() === '' ? 0 : $("#otherCharge").val();
    var lcValueBdt = $("#lcValueBdt").val();
    var convRateBdt = $("#convRateBdt").val();
   
    var otherCost = parseInt(insurencePremiumCharge) + parseInt(bankCharge) + parseInt(customDutyCharge) + parseInt(otherCharge);
    var otherCostPerUnit = otherCost / lcValueBdt;
    var productId = $('#hfProductId').val();
    var productName = $('#product').val();
    var qty = $('#qty').val();
    var usdPrice = $('#usdPrice').val();


    var bdtPrice = parseInt(usdPrice * convRateBdt);
    var invoiceValue = parseInt(qty * bdtPrice);
    var landedCost = parseInt(invoiceValue * otherCostPerUnit);
    var totalCOGS = parseInt(invoiceValue) + parseFloat(landedCost);
    var cogsPerUnit = parseInt(totalCOGS / qty);

    var item = {      
        "ProductId": productId,
        "ProductName": productName,
        "Qty": qty,
        "USDPrice": usdPrice,
        "BDTPrice": bdtPrice,
        "InvoiceValue": invoiceValue,
        "LandedCost": landedCost,
        "TotalCOGS": totalCOGS,
        "CogsPerUnit": cogsPerUnit    
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

function totalLcValue() {

    var lcValue = $("#lcValue").val();
   
    var convRateBdt = $("#convRateBdt").val();
    
    var totalLcValue = lcValue * convRateBdt;
    
    $("#lcValueBdt").val(totalLcValue);
}


$("#convRateBdt").keyup(function () {

    totalLcValue();
});


$("#lcValue").keyup(function () {

    totalLcValue();
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




var amount = 0;
$("#unitPrice").keyup(function () {


    var unitPrice = ($(this).val());
    var qty = $('#qty').val();
    amount = unitPrice * qty;
    $('#amount').val(amount);
});

$("#qty").keyup(function () {
    var qty = $(this).val();
    if (isNaN(qty)) {
        alert("Plese select valid number");
        $('#qty').val('');
    }

    //var unitPrice = $('#unitPrice').val();

    //$('#amount').val(unitPrice * qty);
});

$("#usdPrice").keyup(function () {
    var qty = $(this).val();
    if (isNaN(qty)) {
        alert("Plese select valid number");
        $('#usdPrice').val('');
    }
});

$("#lcQty").keyup(function () {
    var qty = $(this).val();
    if (isNaN(qty)) {
        alert("Plese select valid number");
        $('#lcQty').val('');
    }
});

$("#lcValue").keyup(function () {
    var qty = $(this).val();
    if (isNaN(qty)) {
        alert("Plese select valid number");
        $('#lcValue').val('');
    }
});

$("#convRateBdt").keyup(function () {
    var qty = $(this).val();
    if (isNaN(qty)) {
        alert("Plese select valid number");
        $('#convRateBdt').val('');
    }
});

$("#insurencePremiumCharge").keyup(function () {
    var qty = $(this).val();
    if (isNaN(qty)) {
        alert("Plese select valid number");
        $('#insurencePremiumCharge').val('');
    }
});

$("#bankCharge").keyup(function () {
    var qty = $(this).val();
    if (isNaN(qty)) {
        alert("Plese select valid number");
        $('#bankCharge').val('');
    }
});

$("#customDutyCharge").keyup(function () {
    var qty = $(this).val();
    if (isNaN(qty)) {
        alert("Plese select valid number");
        $('#customDutyCharge').val('');
    }
});

$("#otherCharge").keyup(function () {
    var qty = $(this).val();
    if (isNaN(qty)) {
        alert("Plese select valid number");
        $('#otherCharge').val('');
    }
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