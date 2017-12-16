$(document).ready(function () {

    var canvas = document.getElementById("doodle");
    var ctx = canvas.getContext("2d");
    var font;
    var inputUrls = [];
    var keywordsToSend = [];
    var urlCount = 1;

    ctx.font = '100pt Calibri';
    ctx.lineWidth = 5;
    ctx.strokeStyle = 'white';
    ctx.strokeText('Hello World!', 55, 155);

    $("#urlButton").click(function () {
        var urlString = $('#url').val();
        inputUrls.push(urlString);
        appendToUrls(urlString);
        $('#url').val('');
    });

    $("#submitKeywords").click(function () {
        var keywords = $('#keywords').val();

        var data = {
            keywords: keywords,
            urls: inputUrls
        };

        $.ajax({
            url: '/Home/getSynonyms',
            dataType: "json",
            type: 'POST',
            contentType: 'application/json',
            cache: false,
            data: JSON.stringify(data),
            success: function (synonyms) {
                appendToSynonyms(synonyms);
            }
        });
    });

    $("#submit").click(function () {
        var keywords = $('#keywords').val();

        var data = {
            keywords: keywords,
            urls: inputUrls,
            synonyms: keywordsToSend
        };

        $.ajax({
            url: '/Home/Semantic',
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

    function appendToSynonyms(synonyms) {
        $("#synonyms").empty();
        for (i = 0; i < synonyms.arraySize; i++)
        {
            keywordsToSend.push(synonyms.synonyms[i]);
            $("#synonyms").append('<div><b>' + synonyms.keywords[i] + ': ' + synonyms.synonyms[i] + '</b></div>');
        }
        $("#synonyms").show();
    }

    function appendToUrls(data) {
        $("#urls").append('<div><a href="' + data + '"><b>' + urlCount + ') ' + data + '</b></a></div>');
        urlCount++;
        $("#urls").show();
    }

});