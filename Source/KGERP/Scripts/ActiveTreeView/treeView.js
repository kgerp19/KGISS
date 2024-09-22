
$(document).ready(function () {

    $(".collapsible").on("click", function (e) {
        e.preventDefault();

        var this1 = $(this); // Get Click item 
        var data = {
            pid: $(this).attr('pid')
        };

        var isLoaded = $(this1).attr('data-loaded'); // Check data already loaded or not
        if (isLoaded === "false") {
            $(this1).addClass("loadingP");   // Show loading panel
            $(this1).removeClass("collapse");

            // Now Load Data Here 
            $.ajax({
                url: "/AccountHeads/GetSubMenu",
                type: "GET",
                data: data,
                dataType: "json",
                success: function (d) {
                    $(this1).removeClass("loadingP");

                    if (d.length > 0) {

                        var $ul = $("<ul></ul>");
                        $.each(d, function (i, ele) {
                            $ul.append(
                                $("<li></li>").append(
                                    "<span class='collapse collapsible' data-loaded='false' pid='" + ele.AccountHeadId + "'>&nbsp;</span>" +
                                    "<span><a href='#'>" + ele.AccName + "</a></span>"
                                )
                            );
                        });

                        $(this1).parent().append($ul);
                        $(this1).addClass('collapse');
                        $(this1).toggleClass('collapse expand');
                        $(this1).closest('li').children('ul').slideDown();
                    }
                    else {
                        // no sub menu
                        $(this1).css({ 'dispaly': 'inline-block', 'width': '15px' });
                    }

                    $(this1).attr('data-loaded', true);
                },
                error: function () {
                    alert("Error!");
                }
            });
        }
        else {
            // if already data loaded
            $(this1).toggleClass("collapse expand");
            $(this1).closest('li').children('ul').slideToggle();
        }

    });
});