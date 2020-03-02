$('#calculateBtn').click(function() {
    var postData = {
        airport1: $('#Airport1 option:selected').text(),
        airport2: $('#Airport2 option:selected').text()
    };

    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: $('#getDistanceUrl').val(),
        data: postData,
        success: function(distance) {
            $("#result").html("Result is: " + distance);
            $("#result").show();
        }
    });
});