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