var isLogin;
function checkLogin() {
    var userId = $("#uname").val();
    var pass = $("#psw").val();

    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: "LoginUI.aspx/CheckUserLogin",
        dataType: 'json',
        data: JSON.stringify({ userId:userId, pass:pass }),
        async: false,
        success: function (response) {
            //alert(response.d.Id);
            if (response.d.Id != null) {
                isLogin = true;
            } else {
                isLogin = false;
            }
        },
        error: function () {
            alert("prb");
        }
    });
    return isLogin;
}

$("#loginButton").on("click", function () {

    var isLogin = checkLogin();
    if (isLogin == true) {
        window.location.href = '../UI/HomeUI.aspx';
    } else {
        window.location.reload();
    }
});
$("#uname").keypress(function (e) {

    if (e.which == 13) {
        $("#psw").focus();
    }
});

$("#psw").keypress(function (e) {

    if (e.which == 13) {
        var isLogin = checkLogin();
        if (isLogin == true) {
            window.location.href = '../UI/HomeUI.aspx';
        } else {
            window.location.reload();
        }
    }
});