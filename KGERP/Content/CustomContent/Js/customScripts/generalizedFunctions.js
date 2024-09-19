
var datePickerOptions = {
    dateFormat: 'dd-M-yy',
    firstDay: 1,
    changeMonth: true,
    changeYear: true,
    // ...
};
function GetDayDifference(_dateFrom, _dateTo,isInclusive) {
   
    let dateFrom = new Date(_dateFrom);
    let dateTo = new Date(_dateTo);
    if (dateTo >= dateFrom) {
        // To calculate the time difference of two dates 
        let differenceInTime = dateTo.getTime() - dateFrom.getTime();
        // To calculate the no. of days between two dates 
        let differenceInDays = differenceInTime / (1000 * 3600 * 24);
        if (isInclusive) {
            differenceInDays += 1;
        }
        return differenceInDays;
    } else {
        return 0;
    }
}
function RtnRoundNumber(num) {
    return Math.round(num) || 0;
}
function RtnNumber(num) {
    return parseInt(num) || 0;
}
function RtnBool(num) {

    var rtn = null;
    if (!num) {
        return rtn;
    } else {
        rtn = RtnNumber(num) == 1 ? true : false;
    }
    return rtn;

}
function RtnBoolToLower(value) {

    return value == "True" ? true : false;

}
function RtnFloat(value) {
    value = value == undefined ? 0 : value;
    value = value == "" ? 0 : value;
    return parseFloat(value);
}
function RtnFloatUpToPrecision(value, precision) {

    if (precision == "") {
        precision = 2;
    }

    value = RtnFloat(value).toFixed(precision);

    return parseFloat(value);
}
function CurrencyReturn(curr, rate) {

    var amount = curr * rate;
    return amount;
};
function IsChecked(chkBoxId) {
    var rtn = false;
    if ($('#' + chkBoxId).is(':checked')) {
        rtn = true;
    }
    return rtn;
}
function checkNumeric(event) {
   
    var key = window.event ? event.keyCode : event.which;
    if (event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 46
        || event.keyCode == 37 || event.keyCode == 39) {
        return true;
    }
    if (event.which === 13) {
        $.next().focus();
    }
    else if (key < 48 || key > 57) {
        return false;
    }
    else return true;
}
function checkDecimal(event) {
    

    var key = window.event ? event.keyCode : event.which;//|| event.keyCode == 46
    if (event.keyCode == 8 || event.keyCode == 9
        || event.keyCode == 37 || event.keyCode == 39) {
        return true;
    }
    if (event.which === 13) {
        $(this).next().focus();
    }
    else if (key == 46) {
        var element = event.target.id;
        var findDecimal = $('#' + element).val();
        var isExist = ".";
        if (findDecimal.indexOf(isExist) != -1) {
            return false;
        } else {
            return true;
        }

    }
    else if (key != 46 && (key < 48 || key > 57)) {
        return false;
    }
    else return true;
}