$(document).ready(function () {
    //LoadCompaney();
    BindGridView();
});


$('#FabricsProcName').keypress(function (e) {
    if (e.which == 13) // Enter key = keycode 13
    {
        $('#Description').focus();  //Use whatever selector necessary to focus the 'next' input
        return false;
    }
});


$('#Description').keypress(function (e) {
    if (e.which == 13) // Enter key = keycode 13
    {
        $('#FabricsProcSubmit').focus();  //Use whatever selector necessary to focus the 'next' input
        return false;
    } else if (e.which == 2404) {
        $(this).val($(this).val() + '.');
        return false;
    }
});

$("#FabricsProcSubmit").click(function () {
    var FabricsProcName = $("#FabricsProcName").val();
    var Company = $("#Company").val();
    var Rate = $("#Rate").val();
    var Description = $("#Description").val();
    var FabricsProcId = $("#FabricsProcId").val();
    if (FabricsProcName == "" && $.trim(FabricsProcName) == "") {
        msg = "FabricsProcName is Required";
        $("#FabricsProcName").after("<p class='errorMessage'>" + msg + "</p>");
        return false;
    }
    if (FabricsProcId != "") {
        FabricsProcId = $("#FabricsProcId").val();

    } else {

        FabricsProcId = 0;
    }

    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: "/KgreCostSetup/SaveFabricsProcTable",
        dataType: 'json',
        data: JSON.stringify({ Company: Company, Rate: Rate, FabricsProcName: FabricsProcName, Description: Description, FabricsProcId: FabricsProcId }),
        async: false,
        success: function (data) {
            alert("Save Successfull.");
            BindGridView();
            $("#FabricsProcName").val("");
            $("#Company").val("");
            $("#Rate").val("");
            $("#Description").val(0);
            $('#FabricsProcName').focus();

            Clear();

        },
        error: function () {
        }

    });
    return false;
});
function Clear() {

    $("#FabricsProcId").val("");
    $("#FabricsProcName").val("");
    $("#Company").val("");
    $("#Rate").val("");


}

function Delete(comanyId) {
    var reply = confirm("Ary you sure you want to delete this?");
    var msg = "";
    if (reply) {
        $.ajax({
            type: "POST",
            url: "/KgreCostSetup/DeleteRecord",
            data: "{'comanyId':'" + comanyId + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                BindGridView();
            },

        });
    }
}



function BindGridView() {

    $.ajax({
        type: "POST",
        url: "/KgreCostSetup/LodeGrvLists",
        contentType: "application/json;charset=utf-8",
        data: {},
        dataType: "json",
        success: function (data) {

            $("#FabricsProcTableBody").empty();
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var table = "<tr style='text-align:center;'>" +
                        "<td>" + (i + 1) + "</td>" +
                        "<td>" + data[i].Company + "</td>" +
                        "<td>" + data[i].NameofCost + "</td>" +
                        "<td>" + data[i].Rate + "</td>";
                    //"<td>" + data.d[i].Description + "</td>";


                    table += "<td>"
                        + "<button type='button' onclick=\"Edit('" + data[i].FabricsProcId + "')\" class='btn btn-warning'><span class='glyphicon glyphicon-edit'></span></button>" +
                        //+ "<button type='button' onclick=\"Delete('" + data.d[i].Id + "')\" class='btn btn-danger'><span class='glyphicon glyphicon-trash'></span></button>" +
                        "</td></tr>";

                    $("#FabricsProcTableBody").append(table);
                }
            }
        },
        error: function (result) {
            //alert("Error login");

        }
    });
}
function Edit(FabricsProcId) {

    $.ajax({
        type: "POST",
        url: "/KgreCostSetup/LoadFabricsProcByIds",
        data: "{'FabricsProcId':'" + FabricsProcId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#FabricsProcId").val(data.FabricsProcId);
            $("#FabricsProcName").val(data.NameofCost);
            $("#Company").val(data.Company);
            $("#Rate").val(data.Rate);




        }
    });

}
function LoadCompaney() {
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: "/KgreCostSetup/LoadCompaney",
        dataType: 'json',
        async: false,
        success: function (data) {
            var x = 1;
            var p = x;
            $("#Company").empty();
            $("#Company").append('<option value="' + 0 + '" selected>--Select One--</option>');
            $.each(data.d, function (i, value) {
                $("#Company").append('<option value="' + value.BuyerCode + '">' + value.CompanyName + '</option>');
            });
        },
        error: function () {
        }
    });
}
//$("#AjentName,#AjentAddress,#MobailNo").change(function () {
//    $(".errorMessage").remove();
//    hideMsg();
//});
//,#BankAddress,#ContectPerson,#MobailNo,