function ConvertDateFromDB(jsonDate) {
    var dateT = (new Date(parseInt(jsonDate.substr(6))));
    var dd = dateT.getDate();
    var x = dd.toString().length;
    if (x == 1) {
        dd = "0" + dd;
    }
    var mm = dateT.getUTCMonth() + 1;
    x = mm.toString().length;
    if (x== 1) {
        mm = "0" + mm;
    }
    var yy = dateT.getFullYear();
    return mm + "/" +dd + "/" + yy;
};
function ConvertDateFromDBText(jsonDate) {
    var dateT = (new Date(parseInt(jsonDate.substr(6)))).toDateString();
    return dateT;
};

function formatDate(fromFormat, toFormat, date) {
    if (date.length == '10') {
        var fromSeparator = fromFormat.substr(2, 1);
        if (fromSeparator == "d" || fromSeparator == "m" || fromSeparator == "y")
            fromSeparator = fromFormat.substr(4, 1);

        var toSeparator = toFormat.substr(2, 1);
        if (toSeparator == "d" || toSeparator == "m" || toSeparator == "y")
            toSeparator = toFormat.substr(4, 1);

        var resDate = "";
        var day, month, year;
        fromFormat = fromFormat.toLowerCase();
        toFormat = toFormat.toLowerCase();
        day = date.substr(fromFormat.indexOf('d'), 2);
        month = date.substr(fromFormat.indexOf('m'), 2);
        year = date.substr(fromFormat.indexOf('y'), 4);

        resDate = toFormat.replace(fromSeparator, toSeparator);
        resDate = resDate.replace("dd", day);
        resDate = resDate.replace("mm", month);
        resDate = resDate.replace("yyyy", year);
        return resDate;

    } else {
        return date;
    }
}