 
function getAccounts1() {
    debugger;
    var sttDate = $('#StartDate').val();
    var enDate = $('#EndDate').val();
    var level1 = $('#Level1').val();
    var level2 = $('#Level2').val();
    var level3 = $('#Level3').val();
    var level4 = $('#Level4').val().toString();

    var Level1Chk = $("#Level1").is(":checked");
    var Level2Chk = $("#Level2").is(":checked");
    var Level3Chk = $("#Level3").is(":checked");
    var Level4Chk = $("#Level4").is(":checked");
    $.ajax({
        type: 'GET',
        async: false,
       // traditional: true,
        url: '/GL/api/GetAllAccounts',
        contentType: 'JSON',
       // data: { startDate: sttDate, endDate: enDate}
        data: { startDate: sttDate, endDate: enDate, lev1: level1, lev2: level2, lev3: level3, lev4: level4 },
       
    }).done(function (data) {
        debugger;
        $('#tblAccounts tbody tr').remove();
        $('#tblLedger tbody tr').remove();
        $('#tblVoucher tbody tr').remove();
        var item = jQuery.parseJSON(JSON.stringify(data));
        var rows = "";
        var codecheck1="";
        var codecheck2="";

        var codecheck3="";
        var codecheck4="";
        $.each(item, function (key, value) {
            debugger;
            var totalOpening =0;
            var totalDebit = 0;
            var totalCredit = 0;
            var totalClosing = 0;
            var code1 = value.code1.toString();
            var code2 = value.code2.toString();
            var code3 = value.code3.toString();
            var code4 = value.code4.toString();


              totalOpening +=  Number(value.opening);
              totalDebit +=   Number(value.debit);
            totalCredit += Number(value.credit);
            totalClosing += (Number(value.opening) + Number(value.debit) - Number(value.credit)).toFixed(2);

            if (Level1Chk) {
                if (codecheck1 != code1) {
                    codecheck1 = code1;

                    rows += '<tr class="text-danger">' +
                        '<td class="text-left" style="font-size:10px">' + value.code1 + '</td>' +
                        '<td  style="font-size:10px">' + value.name1 + '</td>' +
                        '<td class="text-right" style="font-size:10px">' + totalOpening + '</td>' +
                        '<td class="text-right" style="font-size:10px">' + totalDebit + '</td>' +
                        '<td class="text-right" style="font-size:10px">' + totalCredit + '</td>' +
                        '<td class="text-right" style="font-size:10px">' + totalClosing + '</td>' +
                        '<td class="text-right" style="font-size:10px"></td>' +
                        //'<td style="font-size:10px"><a class="btn btn-sm btn-secondary m-t-n-xs" onclick="GetLedger(' + "'" + code1 + "'" + ');"><i class="fa fa-eye"></i>View</a></td>' +
                        //'<td><input type="checkbox" id="checkbox" name="checkbox" onclick="abc(this)" >Final</td>' +
                        '</tr>';
                }
            }
            if (Level2Chk) {
                if (codecheck2 != code2) {
                    codecheck2 = code2;

                    rows += '<tr class="text-success">' +
                        '<td style="font-size:10px">' + value.code2 + '</td>' +
                        '<td style="font-size:10px">' + value.name2 + '</td>' +
                        '<td class="text-right" style="font-size:10px">' + totalOpening + '</td>' +
                        '<td class="text-right" style="font-size:10px">' + totalDebit + '</td>' +
                        '<td class="text-right" style="font-size:10px">' + totalCredit + '</td>' +
                        '<td class="text-right" style="font-size:10px">' + totalClosing + '</td>' +
                        '<td class="text-right" style="font-size:10px"></td>' +
                        //'<td style="font-size:10px"><a class="btn btn-sm btn-secondary m-t-n-xs" onclick="GetLedger(' + "'" + code2 + "'" + ');"><i class="fa fa-eye"></i>View</a></td>' +
                        //'<td><input type="checkbox" id="checkbox" name="checkbox" onclick="abc(this)" >Final</td>' +
                        '</tr>';
                }
            }
            if (Level3Chk) {
                if (codecheck3 != code3) {
                    codecheck3 = code3;

                    rows += '<tr class="text-info">' +
                        '<td style="font-size:10px">' + value.code3 + '</td>' +
                        '<td style="font-size:10px">' + value.name3 + '</td>' +
                        '<td class="text-right" style="font-size:10px">' + totalOpening + '</td>' +
                        '<td class="text-right" style="font-size:10px">' + totalDebit + '</td>' +
                        '<td class="text-right" style="font-size:10px">' + totalCredit + '</td>' +
                        '<td class="text-right" style="font-size:10px">' + totalClosing + '</td>' +
                        '<td class="text-right" style="font-size:10px"></td>' +
                        //'<td class="text-center" style="font-size:10px"><a class="btn btn-sm btn-secondary m-t-n-xs" onclick="GetLedger(' + "'" + code3 + "'" + ');"><i class="fa fa-eye"></i>View</a></td>' +
                        //'<td><input type="checkbox" id="checkbox" name="checkbox" onclick="abc(this)" >Final</td>' +
                        '</tr>';
                }
            }
            if (Level4Chk) {
                rows += '<tr>' +
                    '<td style="font-size:10px">' + value.code4 + '</td>' +
                    '<td style="font-size:10px">' + value.name4 + '</td>' +
                    '<td class="text-right" style="font-size:10px">' + totalOpening + '</td>' +
                    '<td class="text-right" style="font-size:10px">' + totalDebit + '</td>' +
                    '<td class="text-right" style="font-size:10px">' + totalCredit + '</td>' +
                    '<td class="text-right" style="font-size:10px">' + totalClosing + '</td>' +
                    '<td><a class="btn btn-sm btn-secondary m-t-n-xs" onclick="GetLedger(' + "'" + code4 + "'" + ');"><i class="fa fa-eye"></i>View</a></td>' +
                    //'<td><input type="checkbox" id="checkbox" name="checkbox" onclick="abc(this)" >Final</td>' +
                    '</tr>';
            }
              totalOpening = 0;
              totalDebit = 0;
              totalCredit = 0;
              totalClosing = 0;
        });
        $('#tblAccounts tbody').append(rows);
       
    });
}

 

function GetLedger(code) {
    debugger;
    var sttDate = $('#StartDate').val();
    var enDate = $('#EndDate').val();
    var level1 = $('#Level1').val();
    var level2 = $('#Level2').val();
    var level3 = $('#Level3').val();
    var level4 = $('#Level4').val();
    $.ajax({
        type: 'GET',
        async: false,
        // traditional: true,
        url: '/GL/api/GetLedgers',
        contentType: 'JSON',
        // data: { startDate: sttDate, endDate: enDate}
        data: { startDate: sttDate, endDate: enDate, StartCode: code, EndCode: code},

    }).done(function (data) {
        debugger;
        $('#tblLedger tbody tr').remove();
        var item = jQuery.parseJSON(JSON.stringify(data));

        var rows = "";
        $.each(item, function (key, value) {
            debugger;

          
            var totalDebit = 0;
            var totalCredit = 0;
          

            totalDebit += Number(value.debit);
            totalCredit += Number(value.credit);

            rows += '<tr>' +
                '<td>' + value.voucherDate + '</td>' +
                '<td>' + value.voucherType + '</td>' +
                '<td class="text-center">' + value.voucherNo + '</td>' +
                '<td>' + value.reference + '</td>' +
                '<td class="text-right">' + totalDebit + '</td>' +
                '<td class="text-right">' + totalCredit + '</td>' +
                '<td class="text-right">' + 0 + '</td>' +
                '<td><a class="btn btn-sm btn-secondary m-t-n-xs" onclick="GetVoucher(' + value.voucherId + ');"><i class="fa fa-eye"></i>View</a><a class="btn btn-sm btn-secondary m-t-n-xs" onclick="Track(' + value.voucherId + ');"><i class="fa fa-eye"></i>Track</a></td>' +
                 
                '</tr>';
        });
        $('#tblLedger tbody').append(rows);

    });

}

function GetVoucher(id) {
    debugger;
    var sttDate = $('#StartDate').val();
    var enDate = $('#EndDate').val();

    $.ajax({
        type: 'GET',
        async: false,
        // traditional: true,
        url: '/GL/api/GetVouchers',
        contentType: 'JSON',
        data: { startDate: sttDate, endDate: enDate, voucherId: id }
        //data: { startDate: sttDate, endDate: enDate, lev1: level1, lev2: level2, lev3: level3, lev4: level4 },

    }).done(function (data) {
        debugger;
        $('#tblVoucher tbody tr').remove();
        var item = jQuery.parseJSON(JSON.stringify(data));

        var rows = "";
        $.each(item, function (key, value) {
            debugger;
            
            var totalDebit = 0;
            var totalCredit = 0;
           
            totalDebit += Number(value.debit);
            totalCredit += Number(value.credit);
 

            rows += '<tr>' +
                '<td>' + value.accountCode + '</td>' +
                '<td>' + value.accountName + '</td>' +
                '<td>' + value.reference + '</td>' +
                '<td>' + value.voucherDetailDescription + '</td>' +
                '<td class="text-right">' + value.debit + '</td>' +
                '<td class="text-right">' + value.credit + '</td>' +
                '<td ><a class="btn btn-sm btn-secondary m-t-n-xs" onclick="GetVoucher(' + value.id + ');"><i class="fa fa-eye"></i>View</a></td>' +

                '</tr>';

            totalDebit = 0;
            totalCredit = 0;
            
        });
        $('#tblVoucher tbody').append(rows);

    });
}

function Track(id) {

    debugger;
    var link = window.origin + "/GL/Voucher/Create/" + id;
    //if (vtype == "JV") {
    //    link = window.origin + "/GL/Voucher/Create/" + id;
    //} else {
    //      link = window.origin + "/GL/PaymentVoucher/Create/" + id;
    //}
        window.open(link, "_blank");
    //href = "~/GL/Voucher/Create/" + id + "";
}
