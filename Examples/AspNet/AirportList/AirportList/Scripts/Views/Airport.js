$(document).ready(function() {
    var columns = [
        {},
        { "searchable": false},
        { "searchable": false},
        {},
        { "searchable": false},
        {},
        { "searchable": false},
        { "searchable": false},
        { "searchable": false}
    ];

    $('#AirportInfoTable').show();

    $('#AirportInfoTable').DataTable({
        "pageLength": 15,
        "dom": "ftip",
        "pagingType": "simple",
        "paging": true,
        "searching": true,
        "sorting": true,
        "stateSave": true,
        "fixedHeader": true,
        "responsive": true,
        "order": [[0, "asc"]],
        "columns": columns
    });
});

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