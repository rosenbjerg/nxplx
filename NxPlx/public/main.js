var templates = {
    transcoding: $("#transcoding-template")
};
var ui = {
    content: $("#view-content")
};

var _onViewLeft;
function showViewContent(viewTemplate, onViewLeft) {
    if (_onViewLeft)
        _onViewLeft();
    ui.content.empty().append(viewTemplate.html());
    if (onViewLeft)
        _onViewLeft = onViewLeft;
    else
        _onViewLeft = null;
}

$(".left-pane-menu .collapser").click(function () {
    $(".left-pane-menu").toggleClass("collapsed");
    $(this).toggleClass("flipped");
});

$("#open-transcoding").click(function () {
    showViewContent(templates.transcoding);
    var $status = ui.content.find(".status");
    var $progressBar = ui.content.find(".inner-progress-bar");
    var ws = new WebSocket("ws://localhost:5000/transcoder/statusupdates");
    ws.onmessage = function (msg) {
        msg = JSON.parse(msg.data);
        console.log(msg);
        if (msg.Status){
            var type = msg.StatusCode === 1 ? "information" : msg.StatusCode === 2 ? "success" : "error";
            $.toast({
                text: msg.Status,
                heading: "File: " + msg.File,
                icon: type,
                showHideTransition: 'fade',
                allowToastClose: true,
                hideAfter: 3000,
                stack: 5,
                position: 'bottom-right',
                textAlign: 'left',
                loader: false,
            });
        }
        else {
            $status.text("Transcoding " + msg.InputFile);
            $progressBar.width(msg.Percentage + "%")
            $progressBar.text(msg.Percentage + "%")
        }
    };
    _onViewLeft = function () {
        ws.close();
    }
});