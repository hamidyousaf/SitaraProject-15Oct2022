

var ids = [];
$('.remove-item').click(function () {
    var element = this.id;
    var id = element.replace("remove-item", "");
    //ids to remove
    var itemId = $('#PaymentItemId' + id).val();
    if (itemId != null) {
        ids.push(itemId);
    }
    var digits = 2;
    //amounts of paidAmount removed
    var removedNetValue = $('#PaymentAmount_' + id).val().replace(/,/g, "");
    var removedIncomeTax = $('#TaxAmount_' + id).val().replace(/,/g, "");
    var removedAmount = $('#LineTotal_' + id).val().replace(/,/g, "");
    //
    var pageTotal = $('#GrandTotal').val().replace(/,/g, "");
    var pagePaidAmount = $('#TotalPaidAmount').val().replace(/,/g, "");
    var pageTaxAmount = $('#TotalTaxAmount').val().replace(/,/g, "");

    $('#GrandTotal').val(Number(pageTotal - removedAmount).toFixed(digits));
    $('#TotalPaidAmount').val(Number(pagePaidAmount - removedNetValue).toFixed(digits));
    $('#TotalTaxAmount').val(Number(pageTaxAmount - removedIncomeTax).toFixed(digits));
    //to remove ids
    $('#IdsDeleted').text(ids);
    $("#supplier-payment" + id).remove();
    $('input.add-comma').commaTextbox();
});

function lockOutInputs(counter) {

    if ($('#PaymentAmount_' + counter).val() == null) {
        $('.dependent').prop('readonly', 'readonly');
        $('select[id=TaxSlab_' + counter + ']').prop("disabled", "disabled");
    }
    else {
        $('.dependent').removeAttr('readonly', 'readonly');
        //$('select[id=TaxSlab_' + counter + ']').removeAttr("disabled", "disabled");
    }
}
var slabVal = 0.00;
function calculateTax(id) {
    debugger;
    //tax slab defined
    var taxId = $('#TaxSlab_' + id).val();
    //define tax values
    $.ajax({
        type: 'GET',
        async: false,
        url: '/AR/Api/GetTaxValues?id=' + taxId
    }).done(function (data) {
        console.log(data);
        if (data != "NotFound") {
            slabVal = data.incomeTaxPercentage.toFixed(2);
            //console.log("taxslab= ", slabVal);
        }
        else {
            slabVal = 0.00;
        }
    });
    //calculate Income tax Amount
    var incomeTaxPercentage = slabVal;
    var incomeTaxAmount = (incomeTaxPercentage / 100) * $('#PaymentAmount_' + id).val().replace(/,/g, "");
    $('#TaxAmount_' + id).val(incomeTaxAmount.toFixed(2));
    //Calculate total remaining amount after incometaxes
    var incomeTax = parseFloat($('#TaxAmount_' + id).val().replace(/,/g, ""));
    var lessAmount = parseFloat(incomeTax);
    var netValue = parseFloat($('#PaymentAmount_' + id).val().replace(/,/g, ""));
    //Cumulative LineTotal
    //Add PaymentAmount(netValue) + incomeTaxAmount(lessAmount)
    $('#LineTotal_' + id).val(Number(netValue - lessAmount).toFixed(2));
    calculateGrandTotal();

}

//Calculating total of all the amounts at present page
function calculateGrandTotal() {
    var totalAmount = 0;
    var netValue = 0;
    var incomeTaxAmount = 0;
    $("input[name='LineTotal']").each(function () {
        totalAmount = (Number(totalAmount) + Number($(this).val().replace(/,/g, ""))).toFixed(2);
        $('#GrandTotal').val(totalAmount);
    });
    $("input[name='PaymentAmount']").each(function () {
        netValue = (Number(netValue) + Number($(this).val().replace(/,/g, ""))).toFixed(2);
        $('#TotalPaidAmount').val(netValue);
    });
    $("input[name='TaxAmount']").each(function () {
        incomeTaxAmount = (Number(incomeTaxAmount) + Number($(this).val().replace(/,/g, ""))).toFixed(2);
        $('#TotalTaxAmount').val(incomeTaxAmount);
    });
    $('input.add-comma').commaTextbox();
}

// PaymentAmount > 0 --validation
function PaymentForm() {
    var i = true;
    $('input[name="PaymentAmount"]').each(function () {
        var split = (this.id).split('_');
        var id = split[1];
        var receiptAmount = parseFloat($('#PaymentAmount_' + id).val().replace(/,/g, ""));
        if (receiptAmount < 1) {
            alert("'Payment Amount' can't be Zero (0).");
            i = false;
            return false;
        }
    });
    $.each($('.add-comma'), function () {
        $(this).val(parseFloat($(this).val().replace(/,/g, "")));
    });
    if (i) $("#FormId").submit();
}

