var countEmail = 0;

var dt = new Date();
var ndt = moment(dt).format("YYYY-MM-DD");
 
var timeIsBeing936 = new Date(" 28-Mar-2022 12:21:00 PM").getTime()
    , currentTime = new Date().getTime()
    , subtractMilliSecondsValue = timeIsBeing936 - currentTime;
setTimeout(sendEmail, subtractMilliSecondsValue);

$(document).ready(function () {
    /*$('#ctl00_sidebar').delay(2000).fadeIn();*/
    
    setInterval(blinker, 1000);
    //setInterval(GetBellNotification, 1000);
     
    setInterval(function () {
        $("#here").load(window.location.href + " #here");
    }, 3000);

   // setInterval(GetBellNotification, 3000);
    //setInterval(sendEmail(), 3000);
    var dt = new Date();
    var ndt = moment(dt).format("YYYY-MM-DD");
    var timeIsBeing936 = new Date("" + ndt + " 06:00:00 PM").getTime()
        , currentTime = new Date().getTime()
        , subtractMilliSecondsValue = timeIsBeing936 - currentTime;
    setTimeout(sendEmail, subtractMilliSecondsValue);

    //var timeIsBeing936 = new Date(" 28-Mar-2022 12:14:00 PM").getTime()
    //    , currentTime = new Date().getTime()
    //    , subtractMilliSecondsValue = timeIsBeing936 - currentTime;
    //setTimeout(sendEmail, subtractMilliSecondsValue);

});

function GetBellNotification() {
    
    var dt = new Date();
    var d = new Date();
    var time = d.getHours() + ":" + d.getMinutes();
    if (time == "12:00") {

        $.ajax({
            type: 'GET',
            async: false,
            url: '/Notification/SaleOrderExpiration',
            contentType: 'JSON'
        });
    }
}



function sendEmail() {
    
    var dt = new Date();
    var d = new Date();
     
        var time = d.getHours() + ":" + d.getMinutes();
    if (time == "06:00") {

            
            $.ajax({
                type: 'POST',
                async: false,
                url: '/EmailSender/SendEmaiL',
                contentType: 'JSON'
            });
        }

    
  
}

function blinker() {
    $('#blink').fadeOut(500);
    $('#blink').fadeIn(500);
}

function GetFollowUp() {
    
    $.ajax({
        type: 'GET',
        async: false,
        url: '/AusPak/Notification/Follow',
        contentType: 'JSON'
    }).done(function (data) {

        //$('#history').remove();
        //rows.remove();
        document.getElementById("history").innerHTML = "";
        var item = jQuery.parseJSON(JSON.stringify(data));
        //JSON.parse(data);

        for (var i = 0; i < item.length; i++) {
            
            var Followup = '';

            var rows = '<div class="to_sho_w_da_te_and_tim_e">' +
                '<h4>' +
                '<i class="fa fa-clock-o" aria-hidden="true"></i>' +
                item[i].nextFDate +
                '</h4>' +
                '<h5>' +
                item[i].followUpTime +
                '</h5>' +
                '<h3>' +
                '<i class="fa fa-envelope-open to_sty_le" aria - hidden="true" ></i>' +
                '</h3>' +
                '<div class="to_assi_gn_wid_th" >' +
                '<h6>' +
                '<i class="fa fa-user" aria-hidden="true" ></i>' +
                item[i].followType +
                '</h6> ' +
                '</div >' +
                '<div class="to_assi_gn_wid_th">' +
                '<h6>' +
                '<i class="fa fa-clock-o" aria-hidden="true"></i>' +
                item[i].description +
                '</h6> ' +
                '</div>' +

                '</div>';
            $('#history').append(rows);
        }
    });
}

