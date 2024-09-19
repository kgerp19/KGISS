$(document).ready(function () {
    LoadClientInfo();
});

function LoadClientInfo() {
    $.ajax({
        type: "POST",
        url: "/ClientBookingList/LoadBookingName",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data.length > 0) {
                var stockRecord = data;
                var table = "";
                var count = 1;
                $("#ClientListTableBody").empty();
                $.each(stockRecord, function (key, value) {
                    table += "<tr>" +
                        "<td style='padding-left:5px;'>" + count + "</td>" +
                        "<td style='padding-left:5px;'>" + value.ClientId + "</td>" +
                        "<td style='padding-left:5px;'>" + value.FullName + "</td>" +
                        "<td style='padding-left:5px;'>" + value.Project + "</td>" +
                        "<td style='padding-left:5px;'>" + value.PresentAddress + "</td>" +
                        "<td style='padding-left:5px;'>" + value.Nationality + "</td>" +
                        "<td style='padding-left:5px;'>" + value.MobileNo + "</td>" +
                        "<td style='padding-left:5px;'>" + value.PlotStatus + "</td>" +
                        "<td style='width:10%'>";
                    if (1) {
                        table += " <a href='/KGRECRM/CreateOrEdit?id=" + value.ClientId + ",companyId=" + value.CompanyId > 0 ? value.CompanyId : 0 + "' style='height: 30px; width: 70px;' class='btn btn-info'>Booking</span></a>";
                    }
                    table += "</td>" +
                        "</tr>";
                    count++;
                })

                $("#ClientListTableBody").append(table);

                $('#ClientListTableMain').DataTable({
                    "scrollY": "500px",
                    "scrollCollapse": true,
                    "paging": true,
                    "bInfo": true
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
            url: "ClientBasicInfoList.aspx/DeleteRecord",
            dataType: 'json',
            data: JSON.stringify({ id: id }),
            async: false,
            success: function (data) {
                LoadClientInfo();
            },

        });


    }

}