var IdsDeleted = [];

$('.remove-item').click(function () {
    var element = this.id;
    var id = element.replace("remove-item", "");
    //ids to remove
    var itemId = $('#PRItemId' + id).val();
    if (itemId != null) {
        ids.push(itemId);
    }
    //values of line items removed
    var digits = 2;
    var removedTotal = $('#Total_' + id).val();
    var removedSalesTax = $('#TaxAmount_' + id).val();
    var removedLine = $('#LineTotal_' + id).val();
    //Values of page level amounts
    var total = $('#Total').val();
    var salesTax = $('#TotalTaxAmount').val();
    var grand = $('#GrandTotal').val();
    // values after removing line item
    var pageTotal = Number(total - removedTotal).toFixed(digits);
    var pageSalesTax = Number(salesTax - removedSalesTax).toFixed(digits);
    var pageGrand = Number(grand - removedLine).toFixed(digits);
    //final cvalues after removing
    $('#Total').val(pageTotal);
    $('#TotalTaxAmount').val(pageSalesTax);
    $('#GrandTotal').val(pageGrand);


    $('#IdsDeleted').text(ids);
    $("#purchase-Order" + id).remove();
});

function calculateLineTotal(id) {
    debugger;
    var qty = $('#Qty_' + id + '').val();
    var rate = $('#Rate_' + id + '').val();
    $('#Total_' + id).val((qty * rate).toFixed(2));
    //After selecting tax slab calculating other values
    $('#LineTotal_' + id).val($('#Total_' + id).val());
    //
    $('#Total_' + id).val((qty * rate).toFixed(2));
    //
    $('#LineTotal_' + id).val((qty * rate).toFixed(2));
    calculateGrandTotal();
    calculateTax(id);
}
var slabVal = 0.00;
//function calculateTax(id) {
//    //tax slab defined
//    debugger;
//    var taxId = $('#TaxSlab_' + id).val();
//    //define tax values
//    $.ajax({
//        type: 'GET',
//        async: false,
//        url: '/AR/Api/GetTaxValues?id=' + taxId
//    }).done(function (data) {
//        if (data != "NotFound") {
//            slabVal = data.salesTaxPercentage.toFixed(2);
//        }
//        else {
//            slabVal = 0.00;
//        }
//    });   
//    var total = $('#Total_' + id).val();
//    var taxAmount = ((slabVal / 100) * total).toFixed(2);
//    var lineAfter = (Number(total) + Number(taxAmount)).toFixed(2)
//    $('#NetTotal' + id).val(lineAfter);
//    //
//    var tax = $('#TotalTaxAmount').val();
//    var newTax = tax - $('#TaxAmount_' + id).val();
//    $('#TaxAmount_' + id).val(taxAmount);
//    $('#TotalTaxAmount').val((Number(taxAmount) + Number(newTax)).toFixed(2));
//    //
//    calculateGrandTotal();
//    var exchangeRate = $('#CurrencyExchangeRate').val(); 
//    $('#FCValue' + id).val(lineAfter);
//    $('#PKRValue' + id).val((Number(lineAfter) * Number(exchangeRate)).toFixed(2));
    
//}

function calculateTax2(id) {
    //tax slab defined
    debugger;
    var taxId = $('#TaxSlab_' + id).val();
    //define tax values
    $.ajax({
        type: 'GET',
        async: false,
        url: '/AR/Api/GetTaxValues?id=' + taxId
    }).done(function (data) {
        if (data != "NotFound") {
            slabVal = data.salesTaxPercentage.toFixed(2);
        }
        else
        {
            slabVal = 0.00;
        }
    });
    var total = $('#Total_' + id).val();
    var taxAmount = ((slabVal / 100) * total).toFixed(2);
    var lineAfter = (Number(total) + Number(taxAmount)).toFixed(2)
    $('#NetTotal' + id).val(lineAfter);
    //
    var tax = $('#TotalTaxAmount').val();
    var newTax = tax - $('#TaxAmount_' + id).val();
    $('#TaxAmount_' + id).val(taxAmount);
    $('#TotalTaxAmount').val((Number(taxAmount) + Number(newTax)).toFixed(2));
    //
    calculateGrandTotal();
    var exchangeRate = $('#CurrencyExchangeRate').val();
    $('#FCValue' + id).val(lineAfter);
    $('#PKRValue' + id).val((Number(lineAfter) * Number(exchangeRate)).toFixed(2));
    var currency = $('#Currency').val();
    if (currency == "PKR") {

        $('#PKRValue' + id).val(lineAfter);
        $('#FCValue' + id).val(0);
    }

}
function getItemDetails(val) {
    var element = val.replace("ItemId", "");
    var id = $('#' + val).val();
    $.ajax({
        url: '/AP/PurchaseOrder/GetItemDetails?id=' + id,
        type: 'GET'
    }).done(function (data) {
        $('.UOM' + element).val(data[0].id);
        $('#UOM' + element).html(data[0].uom);
    });
    if ($('#ItemId' + element).val() == null) {
        $('.dependent').prop('readonly', 'readonly');
    }
    else {
        $('.dependent').removeAttr('readonly', 'readonly');
        $('select[id=TaxSlab_' + element + ']').removeAttr("disabled", "disabled");

    }
}
function lockOutInputs(counter) {

    if ($('#ItemId' + counter).val() == null) {
        $('.dependent').prop('readonly', 'readonly');
        //$('.dependent select').attr('disabled', 'disabled');
        $('select[id=TaxSlab_' + counter + ']').prop("disabled", "disabled");
    }
    else {
        $('.dependent').removeAttr('readonly', 'readonly');
        $('select[id=TaxSlab_' + counter + ']').removeAttr("disabled", "disabled");
    }
}

//Calculating total of all the amounts at present page
//function calculateGrandTotal() {
//    var total = 0;
//    var taxAmount = 0;
//    var grandTotal = 0;
//    $("input[name='Total_']").each(function () {
//        total = (Number(total) + Number($(this).val())).toFixed(2);
//        $('#Total').val(total);
//    });
//    $("input[name='TaxAmount']").each(function () {
//        taxAmount = (Number(taxAmount) + Number($(this).val())).toFixed(2);
//        $('#TotalTaxAmount').val(taxAmount);
//    });
//    $("input[name='LineTotal']").each(function () {
//        grandTotal = (Number(grandTotal) + Number($(this).val())).toFixed(2);
//        $('#GrandTotal').val(grandTotal);
//    });
//}
function calculateGrandTotal() {
    var total = 0;
    var taxAmount = 0;
    var grandTotal = 0;
    var exchangeRate = $('#CurrencyExchangeRate').val();
    $("input[name='Total_']").each(function () {
        total = (Number(total) + Number($(this).val())).toFixed(2);
        $('#Total').val(total * exchangeRate);
    });
    $("input[name='TaxAmount']").each(function () {
        taxAmount = (Number(taxAmount) + Number($(this).val())).toFixed(2);
        $('#TotalTaxAmount').val(taxAmount * Number(exchangeRate));
    });
    //$("input[name='LineTotal']").each(function () {
    // grandTotal = (Number(grandTotal) + Number($(this).val())).toFixed(2);
    // $('#GrandTotal').val(grandTotal);
    //});
    var totalTax = $('#TotalTaxAmount').val();
    var totalAmount = $('#Total').val();
    var freight = $('#Freight').val();
    grandTotal = (Number(totalTax) + Number(totalAmount) + Number(freight)).toFixed(2);
    $('#GrandTotal').val(grandTotal);

}


