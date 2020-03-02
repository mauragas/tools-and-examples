$(document).ready(function() {
    var callColumns = [
        {}, // Id
        {}, // Msisdn
        {}, // Duration
        {}  // Date
    ];
    var smsColumns = [
        {}, // Id
        {}, // Msisdn
        {}  // Date
    ];
    
    $('#PhoneCallStatisticsTable').show();
    $('#PhoneSmsStatisticsTable').show();
    
    $('#PhoneCallStatisticsTable').DataTable({
        "pageLength": 5,
        "dom": "ftip",
        "pagingType": "simple",
        "paging": true,
        "searching": true,
        "sorting": true,
        "stateSave": true,
        "fixedHeader": true,
        "responsive": true,
        "order": [[2, "dec"]],
        "columns": callColumns
    });

    $('#PhoneSmsStatisticsTable').DataTable({
        "pageLength": 5,
        "dom": "ftip",
        "pagingType": "simple",
        "paging": true,
        "searching": true,
        "sorting": true,
        "stateSave": true,
        "fixedHeader": true,
        "responsive": true,
        "order": [[2, "dec"]],
        "columns": smsColumns
    });
});