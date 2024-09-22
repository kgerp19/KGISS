function GetMessage(msgType, msg) {
    var div = "<div class='alert alert-" + msgType + " alert-dismissible' role='alert'>" +
        "<button type='button' class='close' data-dismiss='alert' aria-label='Close'>" +
        "<span aria-hidden='true'>&times;</span>" +
        "</button>" + msg +
        "</div>";
    return div;
}