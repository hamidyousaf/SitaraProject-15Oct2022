var ids = [];
$('.remove-item').click(function () {
    var element = this.id;
    var id = element.replace("remove-item", "");
    //ids to remove
    var itemId = $('#ReceiptItemId' + id).val();
    if (itemId != null) {
        ids.push(itemId);
    }
    var digits = 2;
    var dt = 0;
    var removedTotal = $('#ReceiptAmount_' + id).val();
    var removedIncomeTax = $('#TaxAmount_' + id).val();
    var removedGrandTotal = $('#LineTotal_' + id).val();

    var pageTotal = $('#TotalReceivedAmount').val();
    var pageTax = $('#TotalTaxAmount').val();
    var pageGrand = $('#GrandTotal').val();

    $('#TotalReceivedAmount').val(Number(pageTotal - removedTotal).toFixed(digits));
    $('#TotalTaxAmount').val(Number(pageTax - removedIncomeTax).toFixed(digits));
    $('#GrandTotal').val(Number(pageGrand-removedGrandTotal).toFixed(digits));

    $('#IdsDeleted').text(ids);
    $("#receipt-invoice" + id).remove();
});

var slabVal = 0.00;
function calculateTax(id) {
    debugger;
    var paymentamount = Number($('#ReceiptAmount_' + id).val());
    var balance = Number($('#Balance_' + id).val());
    if (balance >= paymentamount) {
        //tax slab defined
        var taxId = $('#TaxSlab_' + id).val();//Value of select
        //define tax values
        $.ajax({
            type: 'GET',
            async: false,
            url: '/AR/Api/GetTaxValues?id=' + taxId
        }).done(function (data) {
            if (data != "NotFound") {
                slabVal = data.incomeTaxPercentage.toFixed(2); // 7.00
            }
            else {
                slabVal = 0.00;
            }
        });
        var payment = $('#ReceiptAmount_' + id).val();
        var taxAmount = ((slabVal / 100) * payment).toFixed(2);
        var lineAfter = (Number(payment) + Number(taxAmount)).toFixed(2)
        $('#LineTotal_' + id).val(lineAfter);
        var tax = $('#TotalTaxAmount').val();
        var newTax = tax - $('#TaxAmount_' + id).val();
        $('#TaxAmount_' + id).val(taxAmount);
        $('#TotalTaxAmount').val((Number(taxAmount) + Number(newTax)).toFixed(2));
        //calculateGrandTotal();
        //total paid amt

    } else {
        swal("", "Received Amount Must be equal to Inv Balance..!", "info");
        $('#ReceiptAmount_' + id).val(0);
    }

}

function calculateGrandTotal() {
    debugger
    //Calculating total of all the amounts at present page
    var total = 0;
    var taxAmount = 0;
    var grandTotal = 0;
    $('#GrandTotal').val(total);
    $("input[name='ReceiptAmount']").each(function () {
        debugger
        var amount = parseFloat($(this).val());
        if (amount.toString() == 'NaN') {
            //$(this).val(0);
            amount = 0;
        }
        total = (parseFloat(total) + parseFloat(amount)).toFixed(2);
        $('#GrandTotal').val(total);
    });

}
function lockOutInputs(counter) {

    if ($('#InvoiceId' + counter).val() == null) {
        $('.dependent').prop('readonly', 'readonly');
        $('select[id=TaxSlab_' + counter + ']').prop("disabled", "disabled");
    }
    else {
        $('.dependent').removeAttr('readonly', 'readonly');
        $('select[id=TaxSlab_' + counter + ']').removeAttr("disabled", "disabled");
    }
}


// receiptAmount > 0 --validation
function ReceiptForm() {
    var i = true;
    $('input[name="ReceiptAmount"]').each(function () {
        var split = (this.id).split('_');
        var id = split[1];
        var receiptAmount = parseFloat($('#ReceiptAmount').val());
        if (receiptAmount < 1) {
            alert("'Receipt Amount' can't be Zero (0).");
            i = false;
            return false;
        }
    });
    if (i) $("#FormId").submit();
}