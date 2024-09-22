$(document).ready(function () {
    LoadProjectInfo();
   
});
var ItemArray = new Array();
var autoRowId = 1;
var currentItemSerial = 0;
var ReciveCurrentId = 0;
var invCountCurrent = 0;
var RcciveIdGlobal = 0;
function LoadProjectInfo() {
    $.ajax({
        type: "POST",
        url: "ProjectInfoSetup.aspx/LoadProjectInfo",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.d.length > 0) {
                var stockRecord = response.d;
                var table = "";
                var count = 1;
                $("#projecttblBody").empty();
                $.each(stockRecord, function (key, value) {
                    id = value.ClientsAutoId;
                    table += "<tr>" +
                        "<td style='padding-left:5px;'>" + count + "</td>" +
                        "<td style='padding-left:5px;' hidden>" + value.pro_id + "</td>" +
                        "<td style='padding-left:5px;'>" + value.ProName + "</td>" +
                        "<td style='padding-left:5px;'>" + value.ProAddress + "</td>" +
                        "<td style='padding-left:5px;'>" + value.TotalPloatFlat + "</td>" +
                        "<td style='padding-left:5px;'>" + value.Remarks + "</td>" +
                        "<td style='width:15%'>";
                    if (1) {

                        table +=
                            "<button style='height: 30px; width: 30px;margin:2px' type='button'  onclick=\"LoadData('" + value.pro_id + "')\" class='btn btn-info'><span class='glyphicon glyphicon-pencil'></span></button>";
                    }
                    table += "</td>" +
                        "</tr>";


                    count++;
                })

                $("#projecttblBody").append(table);
                $('#projecttblTableMain').DataTable({
                    "scrollY": "500px",
                    "scrollCollapse": true,
                    "paging": true,
                    "bInfo": false
                });



            }
        }

    });

}


function SaveProjectInfo() {
    var ProjectId, IdBasicInfo;
    ProjectId = $("#ProjectId").val();
    if (ProjectId != "") {
        IdBasicInfo = $("#ProjectId").val();
    } else {
        IdBasicInfo = 0;
    }

    var ProjectName = $("#ProjectName").val();
    var Address = $("#MainContent_Address").val();
    var TotalPlot = $("#TotalPlot").val();
    var Remarks = $("#MainContent_Remarks").val();

    //if (ProjectName == "") {
    //    msg = "ProjectName is Required";
    //    $("#ProjectName").after("<p class='errorMessage'>" + msg + "</p>");
    //    return false;
    //}
    //if (Address == "") {
    //    msg = "Address is Required";
    //    $("#Address").after("<p class='errorMessage'>" + msg + "</p>");
    //    return false;
    //}

    var BasicInfo = {
        pro_id: IdBasicInfo,
        ProName: ProjectName,
        ProAddress: Address,
        TotalPloatFlat: TotalPlot,
        Remarks: Remarks

    }
    $.ajax({
        url: "ProjectInfoSetup.aspx/SaveProjectInfo",
        type: "POST",
        data: JSON.stringify({ BasicInfo: BasicInfo }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            alert("Saved");
            AddClear();
            LoadProjectInfo();
           


        },
        error: function (ex) {
            //alert("Hoy nai");
        }
    });

}



function LoadData(id) {
    $.ajax({
        type: "POST",
        url: "ProjectInfoSetup.aspx/LoadData",
        data: JSON.stringify({ id: id }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            $("#ProjectId").val(data.d[0].pro_id);
            $("#ProjectName").val(data.d[0].ProName);
            $("#MainContent_Address").val(data.d[0].ProAddress);
            $("#TotalPlot").val(data.d[0].TotalPloatFlat);
            $("#MainContent_Remarks").val(data.d[0].Remarks);
        }

    });

}









function AddClear() {
    $("#ProjectId").val("");
    $("#ProjectName").val("");
    $("#MainContent_Address").val("");
    $("#TotalPlot").val("");
    $("#MainContent_Remarks").val("");


}


$("#challan,#BuyerName,#order,#CompanyName,#Address").change(function () {
    $(".errorMessage").remove();
    hideMsg();

});

