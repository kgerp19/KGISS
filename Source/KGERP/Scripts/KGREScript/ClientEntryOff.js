$(document).ready(function () {
    $('#NoofInstallment').prop('disabled', true);
    LoadClientBookingInfoById();
    ////LoadClientBasicInfoByIds();
    //LoadData();
});
var ItemArray = new Array();
var autoRowId = 1;
var currentItemSerial = 0;
var DeleveryCurrentId = 0;
var invCountCurrent = 0;
var RcciveIdGlobal = 0;

$("#PloiSize, #LandPrice").keyup(function () {
    var PloiSize = $('#PloiSize').val();
    var LandPrice = $('#LandPrice').val();
    var Landvalue = PloiSize * LandPrice;
    $('#LandValue').val(Landvalue).toFixed(3);
});
$("#Discount").keyup(function () {
    var LandValue = $('#LandValue').val() * 1;
    var Discount = $('#Discount').val() * 1;
    var DiscountAmount = (LandValue * Discount) / 100;
    //var grndtotal = (LandValue - DiscountAmount)*1;
    $('#DiscountValue').val(DiscountAmount).toFixed(3);
    //$('#GrandTotal').val(grndtotal);
});

$("#AdditionalCost, #UtilityCost").keyup(function () {
    var DiscountValue = $('#DiscountValue').val() * 1;
    var LandValue = $('#LandValue').val() * 1;
    var GrandTotal = LandValue - DiscountValue;
    var AdditionalCost = $('#AdditionalCost').val() * 1;
    var UtilityCost = $('#UtilityCost').val() * 1;
    var GrandTotalval = GrandTotal + AdditionalCost + UtilityCost;
    $('#GrandTotal').val(GrandTotalval).toFixed(3);
});
$("#BookingMoney").keyup(function () {
    var BookingMoney = $('#BookingMoney').val() * 1;
    var GrandTotal = $('#GrandTotal').val() * 1;
    var GrandTotalval = (GrandTotal - BookingMoney) * 1;
    $('#RestOfAmount').val(GrandTotalval).toFixed(3);
});

$("#NoofInstallment").keyup(function () {
    var RestOfAmount = $('#RestOfAmount').val() * 1;
    var NoofInstallment = $('#NoofInstallment').val() * 1;
    var Installment = (RestOfAmount / NoofInstallment) * 1;
    $('#InstallmentAmount').val(Installment).toFixed(3);
});

$(document).on("change", "input[type='checkbox']", function () {
    if ($('input[type="checkbox"]:checked').length == 1) {


        $('#NoofInstallment').prop('disabled', false);
    }
    else if ($('input[type="checkbox"]:checked').length == 0) {

        $('#NoofInstallment').prop('disabled', true);

    }
});

function LoadClientBookingInfoById() {
    $.ajax({
        type: "POST",
        url: "ClientInfoEntOff.aspx/LoadClientBookingInfoById",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            $("#ProjectName").val(data.d[0].Cli_ProjectName);
            $("#ClientAutoId").val(data.d[0].ClientsAutoId);
            $("#BlockNo").val(data.d[0].Cli_BlockNo);
            $("#PloatNo").val(data.d[0].Cli_PlotNo);
            $("#PloatSize").val(data.d[0].Cli_PlotSize);
            $("#Facing").val(data.d[0].Cli_Facing);

        }

    });

}

function SaveOfficeInfo() {
    var ClientAutoId = $("#ClientAutoId").val();
    var ProjectName = $("#ProjectName").val();
    var BlockNo = $("#BlockNo").val();
    var PloatNo = $("#PloatNo").val();
    var PloatSize = $("#PloatSize").val();
    var Facing = $("#Facing").val();

    var ploiSize = $("#PloiSize").val();
    var LandPrice = $("#LandPrice").val();
    var LandValue = $("#LandValue").val();
    var Discount = $("#Discount").val();
    var DiscountValue = $("#DiscountValue").val();
    var AdditionalCost = $("#AdditionalCost").val();
    var UtilityCost = $("#UtilityCost").val();
    var GrandTotal = $("#GrandTotal").val();
    var BookingMoney = $("#BookingMoney").val();
    var RestOfAmount = $("#RestOfAmount").val();

    var OneTime = $("#OneTime").is(":checked");
    var Installment = $("#Installment").is(":checked");
    var NoofInstallment = $("#NoofInstallment").val();
    var InstallmentAmount = $("#InstallmentAmount").val();

    if (OneTime == false) {
        OneTime = 0;
    } else {
        OneTime = 1;
    }
    if (Installment == false) {
        Installment = 0;
    } else {
        Installment = 1;
    }


    var ClientOffInfo = {
        ClientAutoId: ClientAutoId,
        BlockNo: BlockNo,
        PloatNo: PloatNo,
        PloatSize: PloatSize,
        Facing: Facing,
        PlotSize: ploiSize,
        LandPricePerKatha: LandPrice,
        LandValue: LandValue,
        Discount: Discount,
        LandValueAfterDiscount: DiscountValue,
        AdditionalCost: AdditionalCost,
        UtilityCost: UtilityCost,
        GrandTotal: GrandTotal,
        BokkingMoney: BookingMoney,
        RestOfAmount: RestOfAmount,
        OneTime: OneTime,
        InstallMent: Installment,
        NoOfInstallment: NoofInstallment,
        InstallMentAmount: InstallmentAmount
    }
    $.ajax({
        url: "ClientInfoEntOff.aspx/SaveOfficeInfo",
        type: "POST",
        data: JSON.stringify({ ClientOffInfo: ClientOffInfo }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (data) {

            alert("Saved");
            window.location.href = '../UI/ClientInfoEntOff.aspx';

        },
        error: function (ex) {
        }
    });



}
function UpdateClear() {
    window.location.href = '../UI/ClientsUI.aspx';
}










