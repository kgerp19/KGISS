function StartLoader() {
    // $('#kgLoader').removeClass('hide');
    $('#kgLoader').addClass('kgloading');
}
function EndLoader() {

    setTimeout(function () {
        $('#kgLoader').removeClass('kgloading');
    }, 100);

    // $('#kgLoader').addClass('hide');


}
function isnull(x, y) {
    if (x == null || x == 'undefined' || x == NaN) {
        return y;
    }
    else { return x; }
}