$(document).ready(function () {

    LoadClientBookingInfo();


});


function LoadClientBookingInfo() {
    var id;
    $.ajax({
        type: "POST",
        url: "/KGREClientInstallmentList/ClientsPaymentList",
        //data: "{'ClientBookingOrClientBooking':'" + types + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.length > 0) {
                var stockRecord = response;
                var table = "";
                var count = 1;
                $("#ClientBookingListTableBody").empty();
                $.each(stockRecord, function (key, value) {
                    id = value.ClientsAutoId;
                    table += "<tr>" +                       
                        "<td style='padding-left:5px;'>" + count + "</td>" +
                        "<td style='padding-left:5px;'>" + value.ClientsAutoId + "</td>" +
                        "<td style='padding-left:5px;'>" + value.ClientName + "</td>" +
                        "<td style='padding-left:5px;'>" + value.PresentAddress + "</td>" +
                        "<td style='padding-left:5px;'>" + value.Cli_ProjectName + "</td>" +
                            "<td style='padding-left:5px;'>" + value.Cli_BlockNo + "</td>" +
                            "<td style='padding-left:5px;'>" + value.Cli_PlotNo + "</td>" +
                            "<td style='padding-left:5px;'>" + value.Cli_PlotSize + "</td>" +
                            "<td style='padding-left:5px;'>" + value.Cli_Facing + "</td>" +
                            "<td style='padding-left:5px;'>" + 0 + "</td>" +
                        "<td style='width:15%'>";
                    if (1) {

                        table += " <a href='/KGREPaymentInfo/Index?id=" + id + "' style='height: 30px; width: 35px;' class='btn btn-success'><span class='glyphicon glyphicon-pencil'></span></a>" +
                            " <a href='/KGREPaymentInfo/Index?id=" + id + "' style='height: 30px; width: 70px;' class='btn btn-info'>Payment</span></a>";
                            //"<button style='height: 30px; width: 30px;margin:2px' type='button'  onclick=\"LoadData('" + value.id + "')\" class='btn btn-danger'><span class='glyphicon glyphicon-trash'></span></button>";
                            //"<button style='height: 30px; width: 70px;margin:2px' type='button'  onclick=\"LoadCliInfo('" + value.id + "')\" class='btn btn-info'>booking</span></button>";
                    }
                    table += "</td>" +
                        "</tr>";


                    count++;
                })
               
                $("#ClientBookingListTableBody").append(table);
               
                $('#ClientBookingListTableMain').DataTable({
                    "scrollY": "500px",
                    "scrollCollapse": true,
                    "paging": true,
                    "bInfo": false
                });
                
               
            }
        }

    });

}







function Delete(id) {
    var reply = confirm("Ary you sure you want to delete this?");
    if (reply) {
        $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            url: "ClientBookingBasicInfoList.aspx/DeleteRecord",
            dataType: 'json',
            data: JSON.stringify({ id: id }),
            async: false,
            success: function (data) {
                LoadClientBookingInfo();
            },

        });


    }

}