$(document).ready(function () {

    var canvas = document.getElementById("doodle");
    var ctx = canvas.getContext("2d");
    var font;
    var inputUrls = [];
    var urlCount = 1;

    ctx.font = '100pt Calibri';
    ctx.lineWidth = 5;
    ctx.strokeStyle = 'white';
    ctx.strokeText('Hello World!', 55, 155);

    $("#urlButton").click(function () {
        var urlString = $('#url').val();
        inputUrls.push(urlString);
        append(urlString);
        $('#url').val('');
    });

    $("#submit").click(function () {
        var keywords = $('#keywords').val();

        var data = {
            keywords: keywords,
            urls: inputUrls
        };

        $.ajax({
            url: '/Home/Site',
            dataType: "json",
            type: 'POST',
            contentType: 'application/json',
            cache: false,
            data: JSON.stringify(data),
            success: function (data) {
                window.location.href = '/Home/Result'
            }
        });
    });

    function append(data) {
        $("#urls").append('<div><a href="' + data + '"><b>' + urlCount + ') ' + data + '</b></a></div>');
        urlCount++;
        $("#urls").show();
    }

});