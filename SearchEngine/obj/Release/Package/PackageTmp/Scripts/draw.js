$(document).ready(function () {
    var canvas = document.getElementById("doodle");
    var ctx = canvas.getContext("2d");

    var n = 0;
    var c = 2;

    while (true)
    {
        var a = n * 137.5;
        var r = c * Math.sqrt(n);

        var x = r * Math.cos(a) + 400;
        var y = r * Math.sin(a) + 125;

        ctx.beginPath();
        ctx.arc(x, y, 8, 0, 2 * Math.PI, false);
        ctx.fillStyle = 'green';
        ctx.fill();
        ctx.lineWidth = 5;
        ctx.strokeStyle = '#003300';
        ctx.stroke();

        n++;
    }
});