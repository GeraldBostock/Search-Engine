$(document).ready(function () {

    var canvas = document.getElementById("doodle");
    var width = canvas.width;
    var height = canvas.height;
    var ctx = canvas.getContext("2d");

    var cellSize = 10;
    var grid = initGameOfLife();
    var ageArray = getZeroGrid();

    var myVar = setInterval(gameOfLife, 500);

    function draw() {
        ctx.clearRect(0, 0, width, height);

        for (i = 0; i < width / cellSize; i++) {
            for (j = 0; j < height / cellSize; j++) {
                ctx.beginPath();
                ctx.lineWidth = "1";
                ctx.strokeStyle = "black";
                ctx.fillStyle = "rgb( " + ageArray[i][j] * 10 + ", 0, 0)";

                if (grid[i][j] == 1) ctx.fillRect(i * cellSize, j * cellSize, cellSize, cellSize);
                else ctx.rect(i * cellSize, j * cellSize, cellSize, cellSize);
                ctx.stroke();
            }
        }
    }

    function gameOfLife() {

        var nextGrid = get2DArray();

        for (var i = 0; i < width / cellSize; i++) {
            for (var j = 0; j < height / cellSize; j++) {
                if (grid[i][j] == 1 && getNeighbors(i, j) < 2) nextGrid[i][j] = 0;
                else if (grid[i][j] == 1 && getNeighbors(i, j) > 3) nextGrid[i][j] = 0;
                else if (grid[i][j] == 0 && getNeighbors(i, j) == 3) nextGrid[i][j] = 1;
                else nextGrid[i][j] = grid[i][j];
            }
        }

        grid = nextGrid;

        for (var i = 0; i < width / cellSize; i++) {
            for (var j = 0; j < height / cellSize; j++) {
                if (grid[i][j] == 1) ageArray[i][j] += 1;
                else ageArray[i][j] = 0;
            }
        }

        draw();
    }

    function getNeighbors(x , y) {
        var neighbors = 0;

        for (var i = -1; i < 2; i++) {
            for (var j = -1; j < 2; j++) {
                if (grid[(x + i + 80) % 80][(y + j + 80) % 80] == 1) neighbors += 1;
            }
        }

        neighbors -= grid[x][y];

        return neighbors;
    }

    function initGameOfLife() {
        var grid = get2DArray();

        for (var i = 0; i < width / cellSize; i++) {
            for (var j = 0; j < height / cellSize; j++) {
                grid[i][j] = Math.round(Math.random());
            }
        }

        return grid;
    }

    function getZeroGrid() {

        var array = new Array(width / cellSize);

        for (var i = 0; i < width / cellSize; i++) {
            array[i] = new Array(height / cellSize);
        }

        for (var i = 0; i < width / cellSize; i++) {
            for (var j = 0; j < height / cellSize; j++) {
                array[i][j] = 0;
            }
        }

        return array;
    }

    function get2DArray() {

        var array = new Array(width / cellSize);

        for (var i = 0; i < width / cellSize; i++) {
            array[i] = new Array(height / cellSize);
        }

        return array;
    }
});