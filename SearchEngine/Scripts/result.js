$(document).ready(function () {
    var canvas = document.getElementById("doodle");
    var ctx = canvas.getContext("2d");
    var font;

    ctx.font = '100pt Calibri';
    ctx.lineWidth = 5;
    ctx.strokeStyle = 'white';
    ctx.strokeText('Hello World!', 55, 155);
});