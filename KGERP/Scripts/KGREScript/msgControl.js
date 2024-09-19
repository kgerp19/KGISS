
function showMsg(msg, msgType) {
    msgType = msgType.toLowerCase();

    if (msgType == "error") {
        $(".msgBox").addClass("error");
        $(".msgBox").removeClass("success");
        $(".msgBox").removeClass("warning");
    } else if (msgType == "warning") {
        $(".msgBox").addClass("warning");
        $(".msgBox").removeClass("success");
        $(".msgBox").removeClass("error");
    } else if (msgType == "success") {
        $(".msgBox").addClass("success");
        $(".msgBox").removeClass("error");
        $(".msgBox").removeClass("warning");
    }

    $(".msgBox").text(msg);

    $(".msgBox").show(200);
}

function hideMsg() {
    $(".msgBox").hide(200);
}


$(document).ready(function() {

    $(".disabled").attr('disabled', true);
});