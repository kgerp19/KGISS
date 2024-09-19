////const { Modal } = require("bootstrap");
////const { modes } = require("codemirror");

$(document).ready(function () {
  hideShowDiv();
    $('#VendorId').siblings('span.error').css('visibility', 'hidden');
    $('#SharePercentage').siblings('span.error').css('visibility', 'hidden');
    $('#itemEdit').css('visibility', 'hidden');
    $('#itemClose').css('visibility', 'hidden');
    $('#SharePercentage').keyup(function () {
        var sum2 = calculatePercentage();
        var Percentage = $('#SharePercentage').val();
        var total = parseFloat(sum2) + parseFloat(Percentage);
        //console.log(total);
        if (total>100) {
            $("#add").attr("disabled", true);
        }
        else {
            $("#add").attr("disabled", false);
        }       
    })

    buttonVisibility();
});

var sum = 0;
function addItemToGrid() {
    var res = createRowForStock();
    if (res) {
        getSelectedItems();
        PopulateTableFromArray();
    }
    buttonVisibility();
}

function hideShowDiv() {
    var showdata = $('#ddlCommonProductFk').val();
    if (showdata === 0 || showdata === "") {
        $('#hideDive').css('visibility', 'hidden');
    } else {
        $('#hideDive').css('visibility', 'visible');
    }
}


function CloseItemToGrid() {
    $('#Customer').val('');
    $('#SharePercentage').val('');
    $('#itemEdit').css('visibility', 'hidden');
    $('#itemClose').css('visibility', 'hidden');
    $('#add').css('visibility', 'visible');
}

$(document).keypress(function (e) {
    if (e.which === 13) {
        addItemToGrid();

    }
});


function buttonVisibility() {
    var totalsum = calculatePercentage();

    var index = customersList.length;
    if (index === 0) {
        $("#submit").attr("disabled", true);
    }
    else if (totalsum!==100) {
        $("#submit").attr("disabled", true);
    }
    else {
        $("#submit").attr("disabled", false);
    }
}



function createRowForStock() {

    var isAllValid = true;


    if (!($('#VendorId').val().trim() !== '')) {
        isAllValid = false;
        $('#VendorId').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#VendorId').siblings('span.error').css('visibility', 'hidden');
    }

    if (!($('#SharePercentage').val().trim() !== '')) {
        isAllValid = false;
        $('#SharePercentage').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#SharePercentage').siblings('span.error').css('visibility', 'hidden');
    }

    return isAllValid;
}

function PopulateTableFromArray() {
  
    $("#itemDetails").empty();
    if (customersList.length > 0) {
        //Customers[3].Amount
        var html = '';
        $.each(customersList, function (key, item) {
           // console.log(item);
            html += '<tr id="' + key + '">';
            html += '<td> ' + (key+1) + '</td>';
            html += '<td><input type="hidden" id="' + key + item.VendorId + '" name="Cutomers[' + key + '].VendorId" value="' + item.VendorId + '"/>' + item.VendorName + '</td>';
            html += '<td><input type="hidden" id="' + key + item.SharePercentage + '" name="Cutomers[' + key + '].SharePercentage" value="' + item.SharePercentage + '"/>' + item.SharePercentage +"%"+ '</td>';
            html += '<td><input type="hidden" id="' + key + item.Customerlandvalue + '"name="Cutomers[' + key + '].Customerlandvalue" value="' + item.Customerlandvalue + '"/>' + item.Customerlandvalue + '</td>';
            html += '<td> <a onclick="removecell(' + item.VendorId + ',' + key + ')" class=" btn-outline-danger btn-flat btn-sm sm"> <i class="fa fa-trash"></i></a> <a onclick="Editcell(' + item.VendorId + ',' + key + ')" class=" btn-outline-danger btn-flat btn-sm sm"> <i class="fa fa-pencil-square-o"></i></a></td>';
            html += '</tr>';
        });
   $('#itemDetails').html(html);
        calculateSum();  
    }
}
function calculateSum() {
    var total = 0;
    for (var i = 0; i < customersList.length; i++) {
        total += customersList[i].SharePercentage << 0;
    }
    var sumRow = "<tr class='trSumQty' ><td colspan='2'><b>Total</b></td><td colspan='3'><b>" + parseFloat(total).toFixed(0) + "%" + "</b></td></tr>";
    $('#itemDetails').append(sumRow);
}


function calculatePercentage() {
    var sum = 0;
    for (var i = 0; i < customersList.length; i++) {
        sum += customersList[i].SharePercentage << 0;
    }
    return sum;
}


function removecell(id, key) {
    var mainClient = $("#BookingBy").val();
    alert("232", mainClient);
    if (Number(mainClient)===id) {
        alert("this customer can't be deleted. you can re-distribute his/her percentage")
    }
    else {
        if (confirm("Are you sure to remove this ?")) {
            customersList.splice(customersList.findIndex(x => x.VendorId == id), 1);
            //console.log(customersList);
            $("#" + key).remove();
            calculateSum();
            PopulateTableFromArray();
            $('#Customer').val('');
            $('#SharePercentage').val('');
            $('#itemEdit').css('visibility', 'hidden');
            $('#itemClose').css('visibility', 'hidden');
            $('#add').css('visibility', 'visible');
            //console.log(item);
        }
        buttonVisibility();
    }
    
}
function Editcell(id, key) {
    console.log(item);
    if (confirm("Are you sure to remove this ?")) {
        $('#itemEdit').css('visibility', 'visible');
        $('#itemClose').css('visibility', 'visible');
        $('#add').css('visibility', 'hidden');
        var item=customersList.find(x => x.VendorId == id);
        $('#VendorId').val(item.VendorId);
         $('#Customer').val(item.VendorName);
        $('#SharePercentage').val(item.SharePercentage);
        //calculateSum();
        //PopulateTableFromArray();
    }
    buttonVisibility();
}


function getSelectedItems() {
    var vendorId = $('#VendorId').val();
    var vendorName = $('#Customer').val();
    var qty = $('#SharePercentage').val();
    var psize = $('#txtPlotSize').val();
    var customerlandvalue = ((psize / 100) * qty).toFixed(4);

    var item = {
        "VendorId": vendorId,
        "VendorName": vendorName,
        "SharePercentage": qty,
        "Customerlandvalue": customerlandvalue
    };
    customersList.push(item);
    $('#Customer').val('');
    $('#SharePercentage').val('');
    //console.log(item);
    //console.log(customersList);
    return item;
}

function DefultCustomer() {
    var vendorId = $('#ClientId').val();
    var vendorName = $('#CustomerGroupNameId').val(); 
    var psize = $('#txtPlotSize').val();
    var customerlandvalue = ((psize / 100) * 100).toFixed(4);

    var item = {
        "VendorId": vendorId,
        "VendorName": vendorName,
        "SharePercentage": 100,
        "Customerlandvalue": customerlandvalue
    };
    customersList.push(item);
    $('#Customer').val('');
    $('#SharePercentage').val('');
    return item;
}


function EditItemToGrid() {

    var res = createRowForStock();
    if (res) {
        var vendorId = $('#VendorId').val();
        var vendorName = $('#Customer').val();
        var qty = $('#SharePercentage').val();
        var psize = $('#txtPlotSize').val();
        var customerlandvalue = ((psize / 100) * qty).toFixed(4);
        var index = customersList.findIndex(x => x.VendorId == vendorId);
        //customersList.splice(customersList.findIndex(x => x.VendorId == vendorId), 1);
        //console.log(index);
        var item = {
            "VendorId": vendorId,
            "VendorName": vendorName,
            "SharePercentage": qty,
            "Customerlandvalue": customerlandvalue
        };

        customersList[index] = item;
        $('#Customer').val('');
        $('#SharePercentage').val('');
        PopulateTableFromArray();
        $('#itemEdit').css('visibility', 'hidden');
        $('#itemClose').css('visibility', 'hidden');
        $('#add').css('visibility', 'visible');
    }
    else {
        alert("Required Fild")
    }
}

function RecalculatCustomerlandvalue() {
    let newcustomersList = new Array();
    var psize = $('#txtPlotSize').val();
    customersList.forEach(function (item) {
        var customerlandvalue = ((psize / 100) * item.SharePercentage).toFixed(4);
        item.Customerlandvalue = customerlandvalue;
        newcustomersList.push(item);
    });
    //console.log('appen', newcustomersList);
    customersList = newcustomersList;
    PopulateTableFromArray();
}