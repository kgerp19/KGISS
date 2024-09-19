
function LoadRmProducts() {
    $.ajax({
        type: "GET",
        url: "/Issue/GetRmProducts",
        data: { 'productId': $(hfProductId).val(), 'qty': $(qty).val() },
        dataType: "Json",
        contentType: "application/json"
    }).done(function (data) {
        $("#orderdetailsItems tbody").empty();
        $.each(data, function (i, v) {
            //console.log(v.ProductName);
            var index = $('#orderdetailsItems tbody').children("tr").length;
            var sl = index;
            var serialCell = "<td>" + (++sl) + "</td>";
            var name = "<td><input type='hidden' id='RProductId" + index + "' name='IssueDetailInfoes[" + index + "].RProductId' value='" + v.RProductId + "' />" + v.ProductName + " </td>";
            var qty = "<td><input type='hidden' id='RMQ" + index + "' name='IssueDetailInfoes[" + index + "].RMQ' value='" + v.RMQ + "' />" + v.RMQ + " </td>";
            //$("#orderdetailsItems tbody").append(
            //    "<tr><td>" + v.ProductName + "</td><td>" + v.RMQ + "</td><td hidden>" + v.RProductId + "</td><tr>"
            //);

            var createNewRow = "<tr>" + serialCell+ name + qty  + " </tr>";
            $('#orderdetailsItems tbody').append(createNewRow);

        });
       
       
    });
}

function renderProduct(element, data) {
    //render product
    var $ele = $(element);
    $ele.empty();
    $ele.append($('<option/>').val('0').text('Select'));
    $.each(data, function (i, val) {
        $ele.append($('<option/>').val(val.Value).text(val.Text));
    });
}




