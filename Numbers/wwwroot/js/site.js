

$(function () {
    scrollCollapsebutton();
    $('#collapse-button').css({ 'display': 'block' });
    $(body).click(function () {

    })
    //implement date picker

    $(".monthpicker").datepicker({
        startView: "months",
        minViewMode: "months",
        autoclose: true,
        forceParse: false,
        format: "M-yy"
    });
    //implement data table
    $('.dataTables-example').DataTable({
        pageLength: 100,
        responsive: true,
        dom: '<"html5buttons"B>lTfgitp',
        order: [0, "desc"],
        buttons: [
            { extend: 'copy' },
            { extend: 'csv' },
            { extend: 'excel', title: 'ExampleFile' },
            { extend: 'pdf', title: 'ExampleFile' },
            {
                extend: 'print',
                customize: function (win) {
                    $(win.document.body).addClass('white-bg');
                    $(win.document.body).css('font-size', '10px');

                    $(win.document.body).find('table')
                        .addClass('compact')
                        .css('font-size', 'inherit');
                }
            }
        ]

    });

});

$('.monthpicker').datepicker({
    todayBtn: "linked",
    keyboardNavigation: false,
    forceParse: false,
    calendarWeeks: true,
    autoclose: true,
    format: 'M-yy',
    todayHighlight: true
});

$('.custom-date-picker').datepicker({
    todayBtn: "linked",
    keyboardNavigation: false,
    forceParse: false,
    calendarWeeks: true,
    autoclose: true,
    format: 'dd-M-yyyy',
    todayHighlight: true
});

$(window).scroll(function () {
    scrollCollapsebutton();
});
function scrollCollapsebutton() {
    var winScrollTop = $(window).scrollTop();
    var winHeight = $(window).height();
    var floaterHeight = $('#collapse-button').outerHeight(true);
    var top = Math.max(0, ((winHeight - floaterHeight) / 2) + winScrollTop) + "px"
    $('#collapse-button').css({ 'top': top });
}
function toasterMessage(isError = false, title, message) {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "progressBar": true,
        "preventDuplicates": false,
        "positionClass": "toast-top-right",
        "onclick": null,
        "showDuration": "400",
        "hideDuration": "1000",
        "timeOut": "7000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut",
        onclick: function () {
            $('#right-sidebar').toggleClass('sidebar-open');
        }
    };
    if (isError)
        toastr.error(message, title);
    else
        toastr.success(message, title);
}
function formatApprovalStatus(status) {
    var returnVal = status; //default
    if (status == "Created" || status == "Booking")
        returnVal = '<span class="label">' + status + '</span>';
    else if (status == "Approved")
        returnVal = '<span class="label label-success">' + status + '</span>';
    return returnVal;
}
function dateFormat(date) {
    const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var current_datetime = new Date(date);
    var formatted_date = current_datetime.getDate() + "-" + months[current_datetime.getMonth()] + "-" + current_datetime.getFullYear();
    return formatted_date;
}
function toggleLoader(start = true) {
    if (start) {
        //start loader
        $('#p').addClass('loader');
        $('#p').after('<h3 id="loaderText" style="margin-left:42%;margin-top:15px;">Loading...</h3>');
    }
    else {
        //stop loader
        $('#p').removeClass('loader');
        $('#loaderText').remove();
    }
}
function generateReport() {
   // debugger;
    $.post('/GL/Api/Create',
        $("#report").serialize()).done(function (data) {
         //   debugger
            //var reportURL = "@ViewBag.ReportPath"
            //var reportURL = "http://localhost:53525/ReportViewer";
            //reportURL += "?Id=";
            var reportURL = data;
            //debugger;
            window.open(reportURL, "_blank");
        }).fail(function (data) {
            //alert(data.responseText)
            toasterMessage(true, "Report Error", data.responseText);
        }
    );
}
function generateApArReport(id, name, companyName, companyId) {
    var parameters = { ReportTitle: name, id: id, CompanyName: companyName, CompanyId: companyId };
    $.post('/GL/Api/Create', parameters).done(function (data) {
        var reportURL = data;
        window.open(reportURL, "_blank");
    }).fail(function (data) {
        //alert(data.responseText)
        toasterMessage(true, "Report Error", data.responseText);
    });
}
//function generateVoucher(voucherId) {
//    $.post('/GL/Api/Create',
//        $("#report").serialize()).done(function (data) {
//            //var reportURL = "@ViewBag.ReportPath"
//            //var reportURL = "http://localhost:53525/ReportViewer";
//            //reportURL += "?Id=";
//            reportURL = data;
//            window.open(reportURL, "_blank");
//        });;
//}
function getStock(itemId, wareHouseId, stockDate) {
    //debugger;
    return $.ajax({
        type: 'POST',
        data: { itemId: itemId, warehouseId: wareHouseId, stockDate: stockDate },
        url: '/Inventory/Api/GetStockByItemWarehouse',
        method: 'POST',

        error: function (data) {
            console.log(data);
            toasterMessage(true, "GetStock Function Error", data.statusText);
        }
        //success: function (data) {
        //    console.log("stock---", data);
        //    result = data;                     
        //} 
    });
}

function setCookie(cname, cvalue) {
    var d = new Date();
    var exdays = 365;
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#preview-img').attr('src', e.target.result);
        }

        reader.readAsDataURL(input.files[0]);
    }
}
$("#preview-file").change(function () {
    readURL(this);
});

function bindSelect2(select2Id, listUrl, urlById, id = 0) {
    $(select2Id).select2({
        placeholder: "Please select",
        ajax: {
            url: listUrl,
            dataType: 'json',
            delay: 250,
            //  placeholder: 'Search for an item',
            processResults: function (data, params) {
                debugger;
                return {
                    results: data
                };
            },
            cache: true
        }
    });
    //if (type != "") {
    //    $.ajax({
    //        type: 'GET',
    //        placeholder: "Please select",
    //        url: urlById + id
    //    }).then(function (data) {
    //        var option = new Option(data.text, data.id, true, true);
    //        select2Id.append(option).trigger('change');
    //        select2Id.trigger({
    //            type: 'select2:select',
    //            params: {
    //                data: data
    //            }
    //        });
    //    });
    /*} else {*/
    $.ajax({
        type: 'GET',
        placeholder: "Please select",
        url: urlById + id
    }).then(function (data) {
        debugger;
        var option = new Option(data.text, data.id, true, true);
        select2Id.append(option).trigger('change');
        select2Id.trigger({
            type: 'select2:select',
            params: {
                data: data
            }
        });
    });
    /*}*/




}

function bindbankSelect2(select2Id, listUrl, urlById, id = 0) {
    debugger
    $(select2Id).select2({
        placeholder: "Please select",
        ajax: {
            url: listUrl,
            dataType: 'json',
            delay: 250,
            //  placeholder: 'Search for an item',
            processResults: function (data, params) {
                debugger;
                return {
                    results: data
                };
            },
            cache: true
        }
    });

    $.ajax({
        type: 'GET',
        placeholder: "Please select",
        url: urlById + id
    }).then(function (data) {
        var option = new Option(data.text, data.id, true, true);
        select2Id.append(option).trigger('change');
        select2Id.trigger({
            type: 'select2:select',
            params: {
                data: data
            }
        });
    });
}