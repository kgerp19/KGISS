$(document).ready(function () {
    //$('#NoofInstallment').prop('disabled', true);
    LoadData();
    $('#BankName').prop('disabled', true);
    $('#ChaqueNo').prop('disabled', true);
    //LoadClientBasicInfoByIds();
   
});
var ItemArray = new Array();
var autoRowId = 1;
var currentItemSerial = 0;
var DeleveryCurrentId = 0;
var invCountCurrent = 0;
var RcciveIdGlobal = 0;

function LoadData() {
    var full_url = document.URL; // Get current url
    var url_array = full_url.split('?id=') // Split the string into an array with / as separator
    var id = url_array[url_array.length - 1];
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: "/KGREPaymentInfo/LoadClientPayInfoById",
        dataType: 'json',
        data: JSON.stringify({ id: id }),
        async: false,
        success: function (data) {
            $("#ProjectName").val(data[0].ProjectName);
            $("#ClientAutoId").val(data[0].ClientsAutoId);
            $("#BlockNo").val(data[0].Cli_BlockNo);
            $("#PloatNo").val(data[0].Cli_PlotNo);
            $("#PloatSize").val(data[0].Cli_PlotSize);
            $("#Facing").val(data[0].Cli_Facing);
            LoadPayInfo(id);
        },
    });
}

function LoadPayInfo(id) {
        $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: "/KGREPaymentInfo/LoadPayInfoById",
        dataType: 'json',
        data: JSON.stringify({ Autoid: id }),
        async: false,
        success: function (data) {
            $("#InstallMentAmount").val(data.InstallMentAmount);
            $("#NoOfInstallment").val(data.NoOfInstallment);
            $("#dates").val(getDate('',data.Booking_Date));
            $("#LastPayAmount").val(data.BokkingMoney);
            $("#TotalPayAmount").val(data.GrandTotal);
            $("#DueAmount").val(data.RestOfAmount);
        },
    });
}


function SaveInfo() {
    var date =$("#PayDate").val();
    var ClientAutoId = $("#ClientAutoId").val();
    var DueAmount = $("#DueAmount").val();
    var PayType = $("#PayType").val();
    var BankName = $("#BankName").val();
    var ChaqueNo = $("#ChaqueNo").val();
    var GrandTotal = $("#GrandTotal").val();
   
    var InstallMentAmount = $("#InstallMentAmount").val();
    var NoOfInstallment = $("#NoOfInstallment").val();
   

    var PaymentInfo = {
        ClientAutoId:ClientAutoId,
        RestOfAmount:DueAmount,
        PayType:PayType,
        BankName:BankName,
        ChaqueNo:ChaqueNo,
        BokkingMoney:GrandTotal,       
        InstallMentAmount:InstallMentAmount,
        NoOfInstallment:NoOfInstallment      
    }

    $.ajax({
        url: "/KGREPaymentInfo/PaymentInfos",
        type: "POST",
        data: JSON.stringify({ PaymentInfo: PaymentInfo, date: date}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
       
        success: function (data) {
            alert("Data Saved Successfully");
            window.location.href = '/KGREClientInstallmentList/Index';
        },
        error: function (ex) {
        }
    });
}
function UpdateClear() {
    window.location.href = '../UI/ClientsUI.aspx';
}

$("#PayType").change(function () {
    var PayType = $('#PayType').val();
    if (PayType =="Bank") {
        $('#BankName').prop('disabled', false);
        $('#ChaqueNo').prop('disabled', false);
      
    } else {
        $('#BankName').prop('disabled', true);
        $('#ChaqueNo').prop('disabled', true);
    }
});