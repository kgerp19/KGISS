$(document).ready(function () { 
    LoadPlotInfo();
    LoadProjectName();
    LoadPlotStatus();
});

function LoadProjectName() {
    $.ajax({
        type: "POST",
        url: "/PlotInfo/LoadProjectName", 
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var option = "";
            option = "<option value='0'>--Select One--</option>";
            $.each(data, function (key, value) {
                option += "<option value='" + value.ProjectId + "'>" + value.ProjectName + "</option>";
            });
            $("#ProjectName").empty();
            $("#ProjectName").append(option);
        },
        error: function (ex) {
            alert(ex.message);
        }
    });
}

function LoadPlotStatus() {
    $.ajax({
        type: "POST",
        url: "/PlotInfo/LoadPlotStatus",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var option = "";
            option = "<option value='0'>--Select One--</option>";
            $.each(data, function (key, value) {
                option += "<option value='" + value.DropDownItemId + "'>" + value.Name + "</option>";
            });
            $("#PlotStatus").empty();
            $("#PlotStatus").append(option);
        },
        error: function (ex) {
            alert(ex.message);
        }
    });
}
var ItemArray = new Array();
var autoRowId = 1;
var currentItemSerial = 0;
var ReciveCurrentId = 0;
var invCountCurrent = 0;
var RcciveIdGlobal = 0;
function LoadPlotInfo() {
    $.ajax({
        type: "POST",
        url: "/PlotInfo/LoadPlotInfo",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.length > 0) {
                var stockRecord = response;
                var table = "";
                var count = 1;
                $("#plottblBody").empty();
                $.each(stockRecord, function (key, value) {
                    id = value.ClientsAutoId;
                    table += "<tr >" +
                        "<td style='padding-left:5px;'>" + count + "</td>" +
                        "<td style='padding-left:5px;' hidden>" + value.PlotId + "</td>" +
                        "<td style='padding-left:5px;'>" + value.ProjectName + "</td>" +
                        "<td style='padding-left:5px;' hidden>" + value.ProjectId + "</td>" +
                        "<td style='padding-left:5px;'>" + value.PlotNo + "</td>" +
                        "<td style='padding-left:5px;'>" + value.PlotFace + "</td>" +
                        "<td style='padding-left:5px;'>" + value.PlotSize + "</td>" +
                        "<td style='padding-left:5px;'>" + value.BlockNo + "</td>" +
                        "<td style='padding-left:5px;'>" + value.Name + "</td>" +
                        "<td style='padding-left:5px;' hidden>" + value.DropDownItemId + "</td>" +
                        "<td style='width:15%'>";
                    if (1) {

                        table +=
                            "<button style='height: 30px; width: 30px;margin:2px' type='button'  onclick=\"LoadData('" + value.PlotId + "')\" class='btn btn-info'><span class='glyphicon glyphicon-pencil'></span></button>";
                    }
                    table += "</td>" +
                        "</tr>";
                    count++;
                })

                $("#plottblBody").append(table);
                $('#plottblTableMain').DataTable({
                    "scrollY": "500px",
                    "scrollCollapse": true,
                    "paging": true,  
                    "bInfo": false
                });
            }
        }
    });
}

function SavePlotInfo() {
    var plotId, IdBasicInfo,plotStatusId;
    plotId = $("#PlotId").val();
    plotStatusId = $("#PlotStatus").val();
    if (plotId != "") {
        IdBasicInfo = $("#PlotId").val();
    } else {
        IdBasicInfo = 0;
    }

    var projectId = $("#ProjectName").val(); 
    var PlotNo = $("#PlotNo").val();
    var Face = $("#Face").val();
    var Size = $("#Size").val();
    var Block = $("#Block").val();
    var PlotStatus = $("#PlotStatus").val();

    if (projectId == null || $.trim(projectId) == "0") {
        msg = "Project Name is Required";
        $("#ProjectName").after("<p class='errorMessage'>" + msg + "</p>");
        return false;
    }
    if (PlotNo == "") {
        msg = "Plot No is Required";
        $("#PlotNo").after("<p class='errorMessage'>" + msg + "</p>");
        return false;
    }
    if (Block == "") {
        msg = "Block No is Required";
        $("#Block").after("<p class='errorMessage'>" + msg + "</p>");
        return false;
    }

    var BasicInfo = {
        PlotId: IdBasicInfo,
        ProjectId: projectId,
        PlotNo: PlotNo,
        PlotFace: Face,
        PlotSize: Size,
        BlockNo: Block,
        PlotStatus: PlotStatus
    }
    $.ajax({
        url: "/PlotInfo/SaveProjectInfo",
        type: "POST",
        data: JSON.stringify({ BasicInfo: BasicInfo }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data.d == 1) {
                alert("Plot no is already exist.");
                LoadPlotInfo();
                AddClear();
            } else {
                alert("Plot Saved Successfully");
                LoadPlotInfo();
                AddClear();
            }
        },
        error: function (ex) {
            //alert("Hoy nai");
        }
    });
}


function LoadData(id) {
    $.ajax({
        type: "POST",
        url: "/PlotInfo/LoadData",
        data: JSON.stringify({ id: id }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) { 
            $("#PlotId").val(data[0].PlotId);
            $("#ProjectName").val(data[0].ProjectId).prop('selected', true);            
            $("#PlotNo").val(data[0].PlotNo);
            $("#Face").val(data[0].PlotFace);
            $("#Size").val(data[0].PlotSize);
            $("#Block").val(data[0].BlockNo);    
            $("#PlotStatus option:selected").text(data[0].Name);  
        }
    });
}


function AddClear() {    
    $("#PlotId").val("");
    $("#ProjectName").val("");
    $("#PlotNo").val("");
    $("#Face").val("");
    $("#Size").val("");
    $("#Block").val("");
    $("#PlotStatus").val("");
}


$("#ProjectName,#PloatNo,#Block").change(function () {
    $(".errorMessage").remove();
    hide();
});