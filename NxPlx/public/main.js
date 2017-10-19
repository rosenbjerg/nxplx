

$(".left-pane-menu .collapser").click(function () {
    $(".left-pane-menu").toggleClass("collapsed");
    $(this).toggleClass("flipped");
});