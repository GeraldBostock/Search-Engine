$(document).ready(function () {

    var canvas = document.getElementById("doodle");
    var ctx = canvas.getContext("2d");
    var font;
    var n = 0;
    var c = 2;

    var myVar = setInterval(draw, 250);

    $("#searchButton").click(function () {

        var keywords = $('#keywords').val();
        var url = $('#url').val();

        console.log(keywords);

        var data = {
            keywords: keywords,
            url: url
        };

        $.ajax({
            url: '/Home/Keyword',
            dataType: "json",
            type: 'POST',
            contentType: 'application/json',
            cache: false,
            data: JSON.stringify(data),
            success: function (data) {
                append(data);
            },
            error: function (xhr, status, error) {
                alert(xhr.responseText);
            }
        });
    });

    function append(data) {
        $("#result").empty();
        for (i = 0, len = data.length; i < len; ++i) {
            var keywordID = i + 1;
            $("#result").append("<div><b>Keyword #" + keywordID + ": " + data[i].keyword + ", Total count: " + data[i].keywordTotal + "</b></div>");
        }
        $("#result").show();
    }

    function draw() {
        var a = n * 137.5;
        var r = c * Math.sqrt(n);

        var x = r * Math.cos(a) + 400;
        var y = r * Math.sin(a) + 125;

        ctx.beginPath();
        ctx.arc(x, y, 5, 0, 2 * Math.PI, false);
        ctx.fillStyle = 'green';
        ctx.fill();
        ctx.lineWidth = 2;
        ctx.strokeStyle = '#003300';
        ctx.stroke();

        n++;
    }

});