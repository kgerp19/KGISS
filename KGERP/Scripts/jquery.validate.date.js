$(function () {
    $.validator.addMethod('date',
        function (value, element) {
            if (this.optional(element)) {
                return true;
            }
            var valid = true;
            try {
                $.datepicker.parseDate('dd/mm/yy', value);
            }
            catch (err) {
                valid = false;
            }
            return valid;
        });


    $('.datepicker').datepicker({
        dateFormat: "dd/mm/yy",
        showOn: 'both',
        buttonText: "<i class='fa fa-calendar'></i>",
        changeMonth: true,
        changeYear: true,
        yearRange: "1901:+50"
    });
});