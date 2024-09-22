var finalEnlishToBanglaNumber =
{ '0': '০', '1': '১', '2': '২', '3': '৩', '4': '৪', '5': '৫', '6': '৬', '7': '৭', '8': '৮', '9': '৯', '.': '.' };

var finalBanglaToEnlishNumber =
{ '০': '0', '১': '1', '২': '2', '৩': '3', '৪': '4', '৫': '5', '৬': '6', '৭': '7', '৮': '8', '৯': '9', '.': '.', '।': '.' };

function convertEngToBanNum(str) {
    if (str == undefined) {
        str = "";
    }
    var mystr = str.toString();
    var outj;    // javascript escaped hex
    var outj1;
    outj1 = "";
    for (var i = 0; i < mystr.length; i++) {
        var ch = mystr.substr(i, 1);
        outj = finalEnlishToBanglaNumber[ch];
        outj1 += outj != undefined && outj != "" ? outj : ch;
    }
    return outj1;
}
function convertBanToEngNum(str) {
    if (str == undefined) {
        str = "";
    }
    var mystr = str.toString();
    var outj;    // javascript escaped hex
    var outj1;
    outj1 = "";
    for (var i = 0; i < mystr.length; i++) {
        var ch = mystr.substr(i, 1);
        outj = finalBanglaToEnlishNumber[ch];
        outj1 += outj != undefined && outj != "" ? outj : ch;
    }
    return outj1;
}
function convertOnlyEngToBanNum(str) {
    if (str == undefined) {
        str = "";
        return str;
    }
    var mystr = str.toString();
    var outj;    // javascript escaped hex
    var outj1;
    outj1 = "";
    for (var i = 0; i < mystr.length; i++) {
        var ch = mystr.substr(i, 1);
        outj = finalEnlishToBanglaNumber[ch];
        if (outj == undefined) {
            return outj;
            break;
        } else {
            outj1 += outj;
        }
    }
    return outj1;
}
function convertOnlyBanToEngNum(str) {
    if (str == undefined) {
        str = "";
        return str;
    }
    var mystr = str.toString();
    var outj;    // javascript escaped hex
    var outj1;
    outj1 = "";
    for (var i = 0; i < mystr.length; i++) {
        var ch = mystr.substr(i, 1);
        outj = finalBanglaToEnlishNumber[ch];
        if (outj == undefined) {
            return outj;
            break;
        } else {
            outj1 += outj;
        }
    }
    return outj1;
}

