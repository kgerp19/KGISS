function ConvertDateFromClient(value) {
    if (value == "")
        return value;

    var day = value.substring(0, 2); //14/12/2016
    var month = value.substring(3, 5);
    var year = value.substring(6, 10);
    value = month + '/' + day + '/' + year;
    return value;
}
function getDate(id, value) {
    var todaydate = "";
    if (value != "") {
        var dateString = value.substr(6);
        todaydate = new Date(parseInt(dateString));
    } else {
        todaydate = new Date();
    }
    var day = ("0" + todaydate.getDate()).slice(-2);
    var month = ("0" + (todaydate.getMonth() + 1)).slice(-2);
    var year = todaydate.getFullYear();
    var datestring = day + "/" + month + "/" + year;
    if (id == "")
        return datestring;
    else {
        document.getElementById(id).value = datestring;
    }
}

function getDateHypen(id, value) {
    var todaydate = "";
    if (value != "") {
        var dateString = value.substr(6);
        todaydate = new Date(parseInt(dateString));
    } else {
        todaydate = new Date();
    }
    var day = ("0" + todaydate.getDate()).slice(-2);
    var month = ("0" + (todaydate.getMonth() + 1)).slice(-2);
    var year = todaydate.getFullYear();
    var datestring = year + "-" + month + "-" + day ;
    if (id == "")
        return datestring;
    else {
        document.getElementById(id).value = datestring;
    }
}

function ConvertDateFromDB(jsonDate) {
    return (new Date(parseInt(jsonDate.substr(6)))).format("dd/mm/yyyy");
};
