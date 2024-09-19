$(document).ready(function () {
    $('.chosen-select').chosen();
    LoadCostRate();
    
    $('#NoofInstallment').prop('disabled', true);
    LoadData();
    
    var project = $('#ProjectName').val();
    if (project == "") {
        $('#SaveOrder').prop('disabled', true);
    } else {
        $('#SaveOrder').prop('disabled', false); 
    }
    //LoadClientBasicInfoByIds();

});
var ItemArray = new Array();
var autoRowId = 1;
var currentItemSerial = 0;
var DeleveryCurrentId = 0;
var invCountCurrent = 0;
var RcciveIdGlobal = 0;
var obj = {};
var paymentEdit = "";

$("#PloiSize, #LandPrice").keyup(function () {
    var PloiSize = $('#PloiSize').val();
    var LandPrice = $('#LandPrice').val();
    var Landvalue = PloiSize * LandPrice;
    $('#LandValue').val(Landvalue);
});
$("#Discount").keyup(function () {
    var LandValue = $('#LandValue').val() * 1;
    var Discount = $('#Discount').val() * 1;
    var DiscountAmount = (LandValue * Discount) / 100;
    var AfterDiscountValue = LandValue - DiscountAmount;
    //var grndtotal = (LandValue - DiscountAmount)*1;
    $('#DiscountValue').val(DiscountAmount);
    $('#AfterDiscountValue').val(DiscountAmount);
    //$('#GrandTotal').val(grndtotal);
});

function totalcost() {
    var DiscountValue = $('#DiscountValue').val() * 1;
    var AfterDiscountValue = $('#AfterDiscountValue').val() * 1;
    var LandValue = $('#LandValue').val() * 1;
    var GrandTotal = LandValue - DiscountValue;
    var AdditionalCost = $('#AdditionalCost').val() * 1;
    var UtilityCost = $('#UtilityCost').val() * 1;
    var GrandTotalval = GrandTotal + AdditionalCost + UtilityCost;
    $('#GrandTotal').val(GrandTotalval) * 1;
}
$("#AdditionalCost, #UtilityCost").keyup(function () {
    totalcost();
});

$("#BookingMoney").keyup(function () {
    var BookingMoney = $('#BookingMoney').val() * 1;
    var GrandTotal = $('#GrandTotal').val() * 1;
    var GrandTotalval = (GrandTotal - BookingMoney) * 1;
    $('#RestOfAmount').val(GrandTotalval) * 1;
});

$("#NoofInstallment").keyup(function () {
    var RestOfAmount = $('#RestOfAmount').val() * 1;
    var NoofInstallment = $('#NoofInstallment').val() * 1;
    var Installment = (RestOfAmount / NoofInstallment) * 1;
    $('#InstallmentAmount').val(Installment) * 1;
});

$(document).on("change", "input[type='checkbox']", function () {
    if ($('input[type="checkbox"]:checked').length == 1) {


        $('#NoofInstallment').prop('disabled', false);
    }
    else if ($('input[type="checkbox"]:checked').length == 0) {

        $('#NoofInstallment').prop('disabled', true);

    }
});
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#clientImage')
                .attr('src', e.target.result)
                .width(125)
                .height(90);
        };

        reader.readAsDataURL(input.files[0]);
    }
}
function readURL1(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#nomineeImage')
                .attr('src', e.target.result)
                .width(125)
                .height(90);
        };

        reader.readAsDataURL(input.files[0]);
    }
}

function AddItem() {
    var pp = $("#clientImage").attr('src');
    var pp1 = $("#nomineeImage").attr('src');


    if ($("#addButton").text() == "Update") {
        UpdatePurchase();
    } else {

        var lastRow = "", rowNumber = 0;
        $("#ClientInfoTable tbody tr").each(function () {
            lastRow = $(this).find("td.SL").text();

        });
        if (lastRow == "") lastRow = "0";
        rowNumber = parseInt(lastRow) + 1;


        var rowCount = rowNumber;
        var fileUploadChallan = $("#customerItemImage").get(0);

        var tableData = "";
        tableData += "<tr class='DeleveryDetailListRow' id=dsi_" + autoRowId + ">";
        tableData += "<td class='SL'>" + rowCount + "</td>";
        tableData += "<td class='DeleveryDetailId hidden' >" +1 + "</td>";
        tableData += "<td class='ApplicantName' >" + $("#ApplicantName").val() + "</td>";
        tableData += "<td class='FathersName'>" + $("#FathersName").val() + "</td>";
        tableData += "<td class='MothersName hidden'>" + $("#MothersName").val() + "</td>";
        tableData += "<td class='SpousesName hidden'>" + $("#SpousesName").val() + "</td>";
        tableData += "<td class='PresentAddress'>" + $("#PresentAddress").val() + "</td>";
        tableData += "<td class='PermanentAddress hidden'>" + $("#PermanentAddress").val() + "</td>";
        tableData += "<td class='Nationality hidden'>" + $("#Nationality").val() + "</td>";
        tableData += "<td class='NationalId hidden'>" + $("#NationalId").val() + "</td>";
        tableData += "<td class='BirthDate hidden' >" + $("#BirthDate").val() + "</td>";
        tableData += "<td class='TinNo hidden'>" + $("#TinNo").val() + "</td>";
        tableData += "<td class='TelOffice hidden'>" + $("#TelOffice").val() + "</td>";
        tableData += "<td class='MobileNo hidden'>" + $("#MobileNo").val() + "</td>";
        tableData += "<td class='TelNoRes hidden'>" + $("#TelNoRes").val() + "</td>";

        tableData += "<td class='Email hidden'>" + $("#Email").val() + "</td>";
        tableData += "<td class='Fax hidden'>" + $("#Fax").val() + "</td>";
        tableData += "<td class='PassportNo hidden'>" + $("#PassportNo").val() + "</td>";
        tableData += "<td class='Profession hidden'>" + $("#Profession").val() + "</td>";
        tableData += "<td class='Designation hidden' >" + $("#Designation").val() + "</td>";
        tableData += "<td class='BankDrafPayOrdchq hidden'>" + $("#BankDrafPayOrdchq").val() + "</td>";
        tableData += "<td class='OfficeAddress hidden'>" + $("#OfficeAddress").val() + "</td>";
        tableData += "<td class='Representative hidden'>" + $("#Representative").val() + "</td>";

        tableData += "<td class='nomFullName'>" + $("#nomFullName").val() + "</td>";
        tableData += "<td class='nomFathers'>" + $("#nomFathers").val() + "</td>";
        tableData += "<td class='nomMothers hidden'>" + $("#nomMothers").val() + "</td>";
        tableData += "<td class='nomAddress'>" + $("#nomAddress").val() + "</td>";
        tableData += "<td class='nomRelation'>" + $("#nomRelation").val() + "</td>";
        tableData += "<td class='nomNationality hidden' >" + $("#nomNationality").val() + "</td>";
        tableData += "<td class='nomNationalId hidden'>" + $("#nomNationalId").val() + "</td>";
        tableData += "<td class='nomTleMobNo hidden'>" + $("#nomTleMobNo").val() + "</td>";
        tableData += "<td class='nomEmail hidden'>" + $("#nomEmail").val() + "</td>";
        tableData += "<td class='Cliimg' style='padding-left:5px;'>" + '<img id="tableImage' + autoRowId + '" src="' + pp + '" style="height: 90px; width: 125px;">' + "</td>";
        tableData += "<td class='Nomimg' style='padding-left:5px;'>" + '<img id="tableImage1' + autoRowId + '" src="' + pp1 + '" style="height: 90px; width: 125px;">' + "</td>";


        tableData += "<td><button type='button'  class='btn btn-danger btn-sm ButtonLeftPadding' onclick='DeleteAProduct(" + autoRowId + ")' >Delete</button></td>";
        tableData += "</tr>";
        autoRowId++;

        $("#ClientInfoTableBody").append(tableData);
        //}



    }

}
function LoadData() {
  
    var full_url = document.URL; // Get current url
    var url_array = full_url.split('?id=') // Split the string into an array with / as separator
    var id = url_array[url_array.length - 1];
 
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: "/ClientBooking/LoadTableDataByids", 
        dataType: 'json',
        data: JSON.stringify({id:id}), 
        async: false, 
        success: function (data) {
            $("#ProjectId").val(data[0].ProjectId);
            $("#ClientAutoId").val(data[0].ClientId);
            if (data[0].Nationality != null) {
                $("#BirthDate").val(getDate('', data[0].DateofBirth));
            } 
            $("#ApplicantName").val(data[0].FullName);
            
                $("#MobileNo").val(data[0].MobileNo);
            
            
                //$("#Email").val(data[0].Email);
            
            //else {
            //    $("#Email").val(0);
            //}
            //$("#Designation").val(data[0].Designation);
            //$("#NationalId").val(data[0].NID);
            $("#ProjectName").val(data[0].Project);
            ////$("#ProjectName").val(data.d[0].ProjectName).prop('selected', true);
            //if (data[0].Nationality != null) {
            //    $("#Nationality").val(data[0].Nationality);
            //}
            //else {
            //    $("#Nationality").val(0);
            //}
           
            $("#PresentAddress").val(data[0].PresentAddress);        

            //$("#PloiSize").val(data.d[0].PlotSize);
            //$("#LandPrice").val(data.d[0].LandPricePerKatha);
            //$("#LandValue").val(data.d[0].LandValue);
            //$("#Discount").val(data.d[0].Discount);
            //$("#DiscountValue").val(data.d[0].LandValueAfterDiscount);
            //$("#AdditionalCost").val(data.d[0].AdditionalCost);
            //$("#UtilityCost").val(data.d[0].UtilityCost);
            //$("#GrandTotal").val(data.d[0].GrandTotal);
            //$("#BookingMoney").val(data.d[0].BokkingMoney);
            //$("#RestOfAmount").val(data.d[0].RestOfAmount);
            //$("#NoofInstallment").val(data.d[0].NoOfInstallment);
            //$("#InstallmentAmount").val(data.d[0].InstallMentAmount);
            //paymentEdit = 0;
            LodeploatByid(id);
            //LodeprojectByProidandCliAutoid(id);
            //LoadTableDataByid(id);
            //LodeprojectByProidandCliAutoid(id);
        },

    });

}

function LoadTableDataByid(id) {
    $.ajax({
        type: "POST",
        url: "ClientBooking.aspx/LoadTableDataByid",
        data: "{'id':'" + id + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var lastRow = "", rowNumber = 0;
            $("#ClientInfoTable tbody tr").each(function () {
                lastRow = $(this).find("td.SL").text();

            });
            if (lastRow == "") lastRow = "0";
            rowNumber = parseInt(lastRow) + 1;


            var rowCount = rowNumber;
            var fileUploadChallan = $("#customerItemImage").get(0);

            if (response.d.length > 0) {
                var stockRecord = response.d;
                var table = "";
                var count = 1;
                $("#ClientInfoTableBody").empty();
                $.each(stockRecord, function (key, value) {
                    table += "<tr id=dsi_" + count + ">" +
                        "<td class='SL'>" + count + "</td>" +
                        "<td class='DeleveryDetailId hidden' >" + 2 + "</td>" +
                        "<td class='ApplicantName' >" + value.NameOfApplicant + "</td>" +
                        "<td class='FathersName'>" + value.FathersName + "</td>" +
                        "<td class='MothersName hidden'>" + value.MothersName + "</td>" +
                        "<td class='SpousesName hidden'>" + value.SpousesName + "</td>" +
                        "<td class='PresentAddress'>" + value.PresentAddress + "</td>" +
                        "<td class='PermanentAddress hidden'>" + value.PermanentAddress + "</td>" +
                        "<td class='Nationality hidden'>" + value.Nationality + "</td>" +
                        "<td class='NationalId hidden'>" + value.NationalIdNo + "</td>" +
                        "<td class='BirthDate hidden' >" + ConvertDateFromDB(value.DateOfBirrh) + "</td>" +
                        "<td class='TinNo hidden'>" + value.TinNo + "</td>" +
                        "<td class='TelOffice hidden'>" + value.TelOffice + "</td>" +
                        "<td class='MobileNo hidden'>" + value.MobileNo + "</td>" +
                        "<td class='TelNoRes hidden'>" + value.TelephoneNoRes + "</td>" +
                        "<td class='Email hidden'>" + value.EmailAdderss + "</td>" +
                        "<td class='Fax hidden'>" + value.Fax + "</td>" +
                        "<td class='PassportNo hidden'>" + value.PassportNo + "</td>" +
                        "<td class='Profession hidden'>" + value.Profession + "</td>" +
                        "<td class='Designation hidden' >" + value.Designation + "</td>" +
                        "<td class='BankDrafPayOrdchq hidden'>" + value.BankDraftPayOrdChq + "</td>" +
                        "<td class='OfficeAddress hidden'>" + value.OfficialAddress + "</td>" +
                        "<td class='Representative hidden'>" + value.RepresentativeName + "</td>" +
                        "<td class='nomFullName'>" + value.Nominee_FullName + "</td>" +
                        "<td class='nomFathers'>" + value.Nominee_FathersName + "</td>" +
                        "<td class='nomMothers hidden'>" + value.Nominee_MothersName + "</td>" +
                        "<td class='nomAddress'>" + value.Nominee_perAdderss + "</td>" +
                        "<td class='nomRelation'>" + value.ReletionwithApplicant + "</td>" +
                        "<td class='nomNationality hidden' >" + value.Nationlaty + "</td>" +
                        "<td class='nomNationalId hidden'>" + value.Natioal_IdNo + "</td>" +
                        "<td class='nomTleMobNo hidden'>" + value.TleOrMobileNo + "</td>" +
                        "<td class='nomEmail hidden'>" + value.Email + "</td>" +
                        "<td class='Cliimg' style='padding-left:5px+'>" + '<img id="tableImage' + 0 + '" src="' + 0 + '" style="height: 90px+ width: 125px+">' + "</td>" +
                        "<td class='Nomimg' style='padding-left:5px+'>" + '<img id="tableImage1' + 0 + '" src="' + 0 + '" style="height: 90px+ width: 125px+">' + "</td>" +


                        "<td style='width:15%'>";
                    if (1) {

                        table +=
                            "<button type='button'  class='btn btn-danger btn-sm ButtonLeftPadding' onclick='DeleteAProduct(" + count + ")' >Delete</button>";
                    }
                    table += "</td>" +
                        "</tr>";


                    count++;



                })
                $("#ClientInfoTableBody").append(table);




            }
        }

    });

}

function DeleteAProduct(id) {
    var con = confirm("Are you sure, you want to delete");
    if (con == true) {
        var itemCode = $("#dsi_" + id).find("td.itemCode").text();
        $("#dsi_" + id).remove();

    }

}

function SaveClientFinal() {
    var EditClientAutoId = "";
    var ClientArray = new Array();
    var dates = $("#dates").val();
    var ProjectId = $("#ProjectId").val();
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
    var WorkProcess = $("#WorkProcessText").val();
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

    var PaymentInfo = {
        paymentEdit: paymentEdit,
        PlotSize: ploiSize,
        LandPricePerKatha: LandPrice,
        LandValue: LandValue,
        Discount: Discount,
        LandValueAfterDiscount: DiscountValue,
        AdditionalCost: AdditionalCost,
        OtharCostName: WorkProcess,
        UtilityCost: UtilityCost,
        GrandTotal: GrandTotal,
        BokkingMoney: BookingMoney,
        RestOfAmount: RestOfAmount,
        OneTime: OneTime,
        InstallMent: Installment,
        NoOfInstallment: NoofInstallment,
        InstallMentAmount: InstallmentAmount
    }

    var ClientBasicInfo = {
        Cli_Date: dates,
        Cli_ProjectName: ProjectId,
        ClientsAutoId: ClientAutoId,
        Cli_BlockNo: BlockNo,
        Cli_PlotNo: PloatNo,
        Cli_PlotSize: PloatSize,
        Cli_Facing: Facing

    }

    $("#ClientInfoTable tbody tr").each(function () {
        if (EditClientAutoId == "") {
            EditClientAutoId = $(this).find("td.DeleveryDetailId").text();
        }

        //filesf = filesf.replace('data:image/png;base64,', '');

        var ClientsDetailObj = {

            ApplicantName: $(this).find("td.ApplicantName").text(),
            FathersName: $(this).find("td.FathersName").text(),
            MothersName: $(this).find("td.MothersName").text(),
            SpousesName: $(this).find("td.SpousesName").text(),
            PresentAddress: $(this).find("td.PresentAddress").text(),
            PermanentAddress: $(this).find("td.PermanentAddress").text(),
            Nationality: $(this).find("td.Nationality").text(),
            NationalId: $(this).find("td.NationalId").text(),
            BirthDate: $(this).find("td.BirthDate").text(),
            TinNo: $(this).find("td.TinNo").text(),
            TelOffice: $(this).find("td.TelOffice ").text(),
            MobileNo: $(this).find("td.MobileNo").text(),
            TelNoRes: $(this).find("td.TelNoRes").text(),
            Email: $(this).find("td.Email").text(),
            Fax: $(this).find("td.Fax").text(),
            PassportNo: $(this).find("td.PassportNo").text(),
            Profession: $(this).find("td.Profession").text(),
            Designation: $(this).find("td.Designation").text(),
            BankDrafPayOrdchq: $(this).find("td.BankDrafPayOrdchq").text(),
            OfficeAddress: $(this).find("td.OfficeAddress").text(),
            Representative: $(this).find("td.Representative").text(),
            nomFullName: $(this).find("td.nomFullName").text(),
            nomFathers: $(this).find("td.nomFathers").text(),
            nomMothers: $(this).find("td.nomMothers").text(),
            nomAddress: $(this).find("td.nomAddress").text(),
            nomRelation: $(this).find("td.nomRelation").text(),
            nomNationality: $(this).find("td.nomNationality").text(),
            nomNationalId: $(this).find("td.nomNationalId").text(),
            nomTleMobNo: $(this).find("td.nomTleMobNo").text(),
            nomEmail: $(this).find("td.nomEmail").text()
            //CliImgName:'',
            //nomImgName: $(this).find("td.Nomimg img").attr('src')


        }
        ClientArray.push(ClientsDetailObj);
    });
    $.ajax({
        url: "/ClientBooking/SaveClientFinal",
        type: "POST",
        data: JSON.stringify({ PaymentInfo: PaymentInfo, ClientBasicInfo: ClientBasicInfo, ClientArray: ClientArray, EditClientAutoId: EditClientAutoId, obj: obj }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (data) {

            alert("Saved");
            window.location.href = '/ClientBooking/booking';

        },
        error: function (ex) {
        }
    });



}
function UpdateClear() {
    window.location.href = '../UI/ClientBooking.aspx';
}


function LodeploatByid(id) {
    //var id = $("#ProjectId").val();
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: "/ClientBooking/LodeploatbyId",
        dataType: 'json',
        data: JSON.stringify({ id: id }),
        async: false,
        success: function (data) {
            var option = "";

            option = "<option value=0>--Select One--</option>";
            $.each(data, function (key, value) {
                option += "<option value='" + data[key].BlockNo + "'>" + data[key].BlockNo + "</option>";
            });
            $("#BlockNo").empty();
            $("#BlockNo").append(option);

         



        },

    });
}

$("#BlockNo").change(function () {
    var ProjectId = $("#ProjectId").val();
    var BlockNo = $("#BlockNo").val();
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: "/ClientBooking/LodeploatbyId1",
        dataType: 'json',
        data: JSON.stringify({ ProjectId: ProjectId, BlockNo: BlockNo }),
        async: false,
        success: function (data) {
            var option = "";

            option = "<option value=0>--Select One--</option>";
            $.each(data, function (key, value) {
                option += "<option value='" + data[key].PloatNo + "'>" + data[key].PloatNo + "</option>";
            });
            $("#PloatNo").empty();
            $("#PloatNo").append(option);

            $("#PloatSize").empty();
            $("#Facing").empty();

         
           



        },

    });
});
$("#PloatNo").change(function () {
    var ProjectId = $("#ProjectId").val();
    var BlockNo = $("#BlockNo").val();
    var PloatNo = $("#PloatNo").val();
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: "/ClientBooking/LodeploatbyId2",
        dataType: 'json',
        data: JSON.stringify({ ProjectId: ProjectId, BlockNo: BlockNo, PloatNo: PloatNo }),
        async: false,
        success: function (data) {
            var option = "";

            option = "<option value=0>--Select One--</option>";
            $.each(data, function (key, value) {
                option += "<option value='" + data[key].PlotSize + "'>" + data[key].PlotSize + "</option>";
            });
            $("#PloatSize").empty();
            $("#PloatSize").append(option);

            $("#Facing").empty();



        },

    });
});
$("#PloatSize").change(function () {
    var ProjectId = $("#ProjectId").val();
    var BlockNo = $("#BlockNo").val();
    var PloatNo = $("#PloatNo").val();
    var PloatSize = $("#PloatSize").val();
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: "/ClientBooking/LodeploatbyId3",
        dataType: 'json',
        data: JSON.stringify({ ProjectId: ProjectId, BlockNo: BlockNo, PloatNo: PloatNo, PloatSize: PloatSize }),
        async: false,
        success: function (data) {
            var option = "";

            option = "<option value=0>--Select One--</option>";
            $.each(data, function (key, value) {
                option += "<option value='" + data[key].PlotFace + "'>" + data[key].PlotFace + "</option>";
            });
            $("#Facing").empty();
            $("#Facing").append(option);





        },

    });
});

function LodeprojectByProidandCliAutoid(id) {
    var ids = $("#ProjectId").val();
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: "/ClientBooking/LodeprojectByProidandCliAutoid",
        dataType: 'json',
        data: JSON.stringify({id:id }),
        async: false,
        success: function (data) {
            obj = {
                Cli_ProjectName: ids,
                Cli_BlockNo: data[0].BlockNo,
                Cli_PlotNo: data[0].PloatNo,
                Cli_PlotSize: data[0].PlotSize,
                Cli_Facing: data[0].PlotFace

            }

            var option = "";
            var option1 = "";
            var option2 = "";
            var option3 = "";

            option += "<option value='" + data[0].BlockNo + "'>" + data[0].BlockNo + "</option>";

            $("#BlockNo").append(option);
            var theSelect = document.getElementById('BlockNo');
            var purchaseInvoiceNo = theSelect.options[theSelect.options.length - 1].value;
            $("#BlockNo").val(purchaseInvoiceNo);

            option1 += "<option value='" + data[0].PloatNo + "'>" + data[0].PloatNo + "</option>";

            $("#PloatNo").append(option1);
            var theSelect = document.getElementById('PloatNo');
            var purchaseInvoiceNo = theSelect.options[theSelect.options.length - 1].value;
            $("#PloatNo").val(purchaseInvoiceNo);

            option2 += "<option value='" + data[0].PlotSize + "'>" + data[0].PlotSize + "</option>";

            $("#PloatSize").append(option2);

            var theSelect = document.getElementById('PloatSize');
            var purchaseInvoiceNo = theSelect.options[theSelect.options.length - 1].value;
            $("#PloatSize").val(purchaseInvoiceNo);

            option3 += "<option value='" + data[0].PlotFace + "'>" + data[0].PlotFace + "</option>";

            $("#Facing").append(option3);
            var theSelect = document.getElementById('Facing');
            var purchaseInvoiceNo = theSelect.options[theSelect.options.length - 1].value;
            $("#Facing").val(purchaseInvoiceNo);



        },

    });
}

//choon All fuction
function LoadCostRate() {
    var invoice = $("#CompanyName").val();

    $.ajax({
        url: "/ClientBooking/LoadCostsRate",
        type: "POST",
        data: JSON.stringify({ invoice: invoice }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            var x = 1;
            var p = x;
            $("#WorkProcess").empty().trigger("chosen:updated");
            $("#WorkProcess").append('<option value="' + 0 + '">--Select One--</option>').trigger("chosen:updated");
            $.each(data, function (i, value) {
                $("#WorkProcess").append('<option value="' + value.Rate + '">' + value.NameofCost + '</option>').trigger("chosen:updated");
            });
           
        }

    });
}


var worklist = "";
function workList() {
    var results = "";
    $('#WorkProcess option:selected').each(function (index, sel) {
        results += $(sel).text() + '+';
    });
    worklist = results;
}

function SumRate() {
    var sum =0;
    var quantity = $("#WorkProcess").val();
    //rate += +quantity;
    //rate = parseFloat($("#rate").val()*1);
    //var ss = rate + qty;
    $.each(quantity, function (idx, num) {
        sum += parseFloat(num)
    })

    $("#UtilityCost").val(sum);
    totalcost();
    workList();
    $("#WorkProcessText").val(worklist);
}

$(".chosen-select").chosen().on("change", function (event, params) {
    if (params.selected) {
        $("#status").text('The option:' + params.selected + 'was selected.');
    }
    if (params.deselected) {
        $("#status").text('The option:' + params.deselected + 'was deselected.');
    }
})

//$(window).resize();


