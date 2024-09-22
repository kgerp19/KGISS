$(document).ready(function () {
    LoadProjectName();
    LoadClientBasicInfoById();
});
var ItemArray = new Array();
var autoRowId = 1;
var currentItemSerial = 0;
var ReciveCurrentId = 0;
var invCountCurrent = 0;
var RcciveIdGlobal = 0;

function LoadProjectName() {
    $.ajax({
        url: "PlotInformation.aspx/LoadProjectName",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var option = "";
            option = "<option value='0'>--Select One--</option>";
            $.each(data.d, function (key, value) {
                option += "<option value='" + value.pro_id + "'>" + value.ProName + "</option>";
            });
            $("#ProjectName").empty();
            $("#ProjectName").append(option);
        },
        error: function (ex) {
            alert(ex.message);
        }
    });
}


function SaveClientBasicInfo() {
    var IdBasic, IdBasicInfo;
    IdBasic = $("#IdBasicInfo").val();
    if (IdBasic != "0") {
        IdBasicInfo = $("#IdBasicInfo").val();
    } else {
        IdBasicInfo = IdBasic;
    }
    var date = $("#date").val();
    var ClientName = $("#ClientName").val();
    var FathersName = $("#FathersName").val();
    var MobileNo = $("#MobileNo").val();
    var Email = $("#Email").val();
    var ProjectName = $("#ProjectName").val();
    var Profession = $("#Profession").val();
    var PresentAddress = $("#MainContent_PresentAddress").val();


    if (ClientName == "") {
        msg = "ClientName is Required";
        $("#ClientName").after("<p class='errorMessage'>" + msg + "</p>");
        return false;
    }
    if (MobileNo == "") {
        msg = "MobileNo is Required";
        $("#MobileNo").after("<p class='errorMessage'>" + msg + "</p>");
        return false;
    }
    if (Email =="") {
        msg = "Email is Required";
        $("#Email").after("<p class='errorMessage'>" + msg + "</p>");
        return false;
    }
    if (IsEmail(Email) == false) {
        msg = "Invalid Email";
        $("#Email").after("<p class='errorMessage'>" + msg + "</p>");
        return false;
    }

    var BasicInfo = {
        Basic_Info_id: IdBasicInfo,
        Name: ClientName,
        FathersName: FathersName,
        PresentAddress: PresentAddress,
        EntryDate: formatDate("dd/mm/yyyy", "mm-dd-yyyy", $("#date").val()),
        MobileNo: MobileNo,
        Email: Email,
        Profession: Profession,
        ProjectName: ProjectName

    }
    $.ajax({
        url: "ClientsBasicUI.aspx/ClientsInfoFinalSave",
        type: "POST",
        data: JSON.stringify({ BasicInfo: BasicInfo }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            alert("Saved");
            AddClear();
            
            window.location.href = '../UI/ClientBasicInfoList.aspx';
           
        },
        error: function (ex) {
            //alert("Hoy nai");
        }
    });




}

function IsEmail(email) {
    var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (!regex.test(email)) {
        return false;
    } else {
        return true;
    }
}
function LoadClientBasicInfoById() {
    $.ajax({
        type: "POST",
        url: "ClientsBasicUI.aspx/LoadClientBasicInfoById",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            //$("#IdBasicInfo").val(data.d[0].Basic_Info_id);
            $("#date").val(getDate('', data.d[0].EntryDate));
            $("#ClientName").val(data.d[0].Name);
            $("#FathersName").val(data.d[0].FathersName);
            $("#MobileNo").val(data.d[0].MobileNo);
            $("#Email").val(data.d[0].Email);
            $("#ProjectName").val(data.d[0].ProjectName);
            $("#Profession").val(data.d[0].Profession);
            $("#MainContent_PresentAddress").val(data.d[0].PresentAddress);
        }

    });

}









function AddClear() {
    $("#IdBasicInfo").val("");
    $("#ClientName").val("");
    $("#FathersName").val("");
    $("#MobileNo").val("");
    $("#Email").val("");
    $("#ProjectName").val("");
    $("#Profession").val("");
    $("#MainContent_PresentAddress").val("");


}


$("#ClientName,#MobileNo,#Email").change(function () {
    $(".errorMessage").remove();
    hideMsg();

});

