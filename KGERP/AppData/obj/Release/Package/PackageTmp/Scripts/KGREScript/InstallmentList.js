$(document).ready(function () {
    var full_url = document.URL; // Get current url
    var url_array = full_url.split('?id=') // Split the string into an array with / as separator
     id = url_array[url_array.length - 1];
    LoadInstallmentBookingInfo(id);
    LoadInstallmentBookingInfo1(id);
    
});
var id;
var AutorowId = 1;
var retval = new Array();
var PaymentInfo = new Array();
function LoadInstallmentBookingInfo(id) {

    $.ajax({
        type: "POST",
        url: "/KGREPaymentInfo/CalculateNextPayDateFrom1",
        data: JSON.stringify({ id: id }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {

            var stockRecord = response;
            var table = "";
            var count = 1;
            //$("#InstallmentListTableBody").empty();
            $.each(stockRecord, function (key, value) {
                //table += "<tr>" +                       
                //    "<td style='padding-left:5px;'>" + count + "</td>" +
                //    "<td style='padding-left:5px;'>" + getDate('',)+ "</td>" +                        
                //    "</tr>";
                retval.push(value);

                //count++;
            })

            //$("#InstallmentListTableBody").append(table);

            //$('#InstallmentListTableMain').DataTable({
            //    "scrollY": "500px",
            //    "scrollCollapse": true,
            //    "paging": false,
            //    "bInfo": false
            //});


        }

    });

}

var DueAmount1 = 0;

function LoadInstallmentBookingInfo1(id) {
   
 
    $.ajax({
        type: "POST",
        url: "/KGREPaymentInfo/ClientPaymentHistory",
        data: JSON.stringify({ id: id }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {

            var stockRecord = response;
            $.each(stockRecord, function (key, value) {
                PaymentInfo.push(value);
            })

            //DueAmount1 = (PaymentInfo[0].InstallMentAmount - PaymentInfo[0].BokkingMoney).toFixed(3);

            if (response.length > 0) {

                var table = "";
                var count = 1;

                $("#InstallmentListTableBody1").empty();
                var status;
                var DueAmount = 0;
               
                
                $.each(retval, function (key, value) {                 


                    if (PaymentInfo.length > 0 && PaymentInfo[0].GrandTotal==null) {
                        DueAmount = (PaymentInfo[0].InstallMentAmount - PaymentInfo[0].BokkingMoney).toFixed(3);
                        if (PaymentInfo[0].BokkingMoney == PaymentInfo[0].InstallMentAmount) {
                            status = ("Payment");
                            result = status.fontcolor("green");
                        } else if (PaymentInfo[0].BokkingMoney < PaymentInfo[0].InstallMentAmount) {
                            status = "Partial payment"
                            result = status.fontcolor("red");
                        } else if (PaymentInfo[0].BokkingMoney > PaymentInfo[0].InstallMentAmount) {
                            status = "advance payment"
                            result = status.fontcolor("yellow");
                        }
                        table += "<tr>" +

                            "<td style='padding-left:5px;'>" + count + "</td>" +
                            "<td style='padding-left:5px;' class='BokkingMoney'>" + PaymentInfo[0].BokkingMoney + "</td>" +
                            "<td style='padding-left:5px;' class='InstallMentAmount'>" + PaymentInfo[0].InstallMentAmount + "</td>" +
                            "<td style='padding-left:5px;' class='Booking_Date'>" + getDate('', PaymentInfo[0].Booking_Date) + "</td>" +
                            "<td style='padding-left:5px;' class='DueAmount'>" + DueAmount + "</td>" +
                            "<td style='width:10%' class='PayDueAmount'>";
                        if (status == "Payment") {

                            table +=
                                '<input  class="form-control  input-sm" type="text" disabled>';
                        } else {
                            table +=
                               '<input id="PayValue_' + PaymentInfo[0].booking_id + '" attr-id="' + PaymentInfo[0].booking_id + '"  class="form-control  input-sm" type="text">';
                        }
                        table += "</td>" +
                           
                            "<td style='padding-left:5px;' class='NoOfInstallment'>" + PaymentInfo[0].NoOfInstallment + "</td>" +
                            "<td style='padding-left:5px;' class='InstallmentDate'>" + getDate('', value) + "</td>" +
                            "<td style='padding-left:5px;' class='Status'>" + result + "</td>" +
                            "<td style='width:10%'>";
                        if (1) {

                            table +=
                                "<button style='height: 30px; width: 30px;margin:2px' type='button'  onclick=\"Edit('" + PaymentInfo[0].booking_id + "')\" class='btn btn-info'><span class='glyphicon glyphicon-pencil'></span></button>";
                        }
                        table += "</td>" +
                            "</tr>";
                        count++; 
                    } else {
                     
                        table += "<tr>" +
                            "<td style='padding-left:5px;'>" + count + "</td>" +
                            "<td style='padding-left:5px;'>" + 0 + "</td>" +
                            "<td style='padding-left:5px;'>" + 0 + "</td>" +
                            "<td style='padding-left:5px;'>" + 0 + "</td>" +
                            "<td style='padding-left:5px;'>" + 0 + "</td>" +
                            "<td style='padding-left:5px;'>" + 0 + "</td>" +
                            "<td style='padding-left:5px;'>" + 0 + "</td>" +
                            "<td style='padding-left:5px;'>" + getDate('', value) + "</td>" +
                            "<td style='padding-left:5px;'>" + 0 + "</td>" +
                            "<td style='padding-left:5px;'>" + 0 + "</td>" +
                            
                        
                            "</tr>";
                        count++;


                    }


                    
                    PaymentInfo.splice(0, 1);
                    AutorowId++;
                })
                $("#InstallmentListTableBody1").append(table);
                $('#InstallmentListTableMain1').DataTable({
                    "scrollY": "500px",
                    "scrollCollapse": true,
                    "paging": false,
                    "bInfo": false
                });
            }
        }

    });

}


function Edit(id) {
    var PayDueAmount = parseInt($("#PayValue_" + id).val());
    var Id = parseInt(id);
   
    $.ajax({
        type: "POST",
        url: "/KGREPaymentInfo/Edits",
        data: JSON.stringify({PayDueAmount:PayDueAmount,Id:Id }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            alert("Update Success fully.")
            location.reload();

        },
        error: function (ex) {
        }
    });



}

function CleareAItem() {
    window.location.href = '../UI/ClientBookingInfoList.aspx';
}