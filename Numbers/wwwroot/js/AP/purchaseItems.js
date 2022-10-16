var ids = [];
var grnItemQty = 0.0000;
$('.remove-item').click(function () {
    var elementId = this.id;
    var id = elementId.replace("remove-item", "");
    //ids to remove
    var itemId = $('#PurchaseItemId' + id).val();

    if (itemId != null) {
        ids.push(itemId);
    }
    //Values of line items being removed
    var digits = 2;
    var lineTotal = $('#Total_' + id).val();
    var lineDiscount = $('#DiscountAmount_' + id).val();
    var lineSalesTax = $('#SalesTaxAmount_' + id).val();
    var lineExciseTax = $('#ExciseTaxAmount_' + id).val();
    var lineLineTotal = $('#LineTotal_' + id).val();
    //Page Level Total
    var total = $('#Total').val();
    var discountAmount = $('#discountAmount').val();
    var salesTaxAmount = $('#salesTaxAmount').val();
    var exciseTaxAmount = $('#exciseTaxAmount').val();
    var grandTotal = $('#GrandTotal').val();
    
    //After calculating removed Values
    var pageTotal = Number(total - lineTotal).toFixed(digits);
    var pageDiscount = Number(discountAmount - lineDiscount).toFixed(digits);
    var pageSalesTax = Number(salesTaxAmount - lineSalesTax).toFixed(digits);
    var pageExciseTax = Number(exciseTaxAmount - lineExciseTax).toFixed(digits);
    var pageGrandTotal = Number(grandTotal - lineLineTotal).toFixed(digits);
    // Assign Valus after calculations
    $('#Total').val(pageTotal);
    $('#discountAmount').val(pageDiscount);
    $('#salesTaxAmount').val(pageSalesTax);
    $('#exciseTaxAmount').val(pageExciseTax);
    $('#GrandTotal').val(pageGrandTotal);

    $('#IdsDeleted').text(ids);
    $("#purchase-items" + id).remove();
});
function getItemDetails(val) {
    var element = val.replace("ItemId", "");
    var id = $('#' + val).val();//$('#ItemId0')
    $.ajax({
        url: '/AP/Purchase/GetItemDetails?id=' + id,
        type: 'GET'
    }).done(function (data) {
        $('.UOM' + element).val(data[0].id);
        $('#UOM' + element).html(data[0].uom);
    });
    if ($('#ItemId' + element).val() == null) {
        $('.dependent').prop('readonly', 'readonly');
        $('.dependent').html(0);

    }
    else {
        $('.dependent').removeAttr('readonly', 'readonly');
        $('select[id=TaxSlab_' + element + ']').removeAttr("disabled", "disabled");

    }
}
function lockOutInputs(counter) {

    if ($('#ItemId' + counter).val() == null) {
        $('.dependent').prop('readonly', 'readonly');
        $('select[id=TaxSlab_' + counter + ']').prop("disabled", "disabled");
    }
    else {
        $('.dependent').removeAttr('readonly', 'readonly');
        $('select[id=TaxSlab_' + counter + ']').removeAttr("disabled", "disabled");
    }
}
function calculateLineTotal(id) {
    debugger
    var digits = 2;
    var qty = $('#Qty_' + id).val();
    var rate = $('#Rate_' + id).val();
    var itemTotal = $('#Total_' + id).val();
    $('#Total_' + id).val((qty * rate).toFixed(digits));
    //After selecting tax slab calculating other values
    var discountPercentage = $('#DiscountPercentage_' + id).val();
    var discountAmount = (discountPercentage / 100) * itemTotal;
    if (discountPercentage == 0) { $('#DiscountAmount_' + id).val(0.00); }
    $('#DiscountAmount_' + id).val(discountAmount.toFixed(2));
    calculateTax(id);
}
function calculateTax(id) {
    var taxId = slabVal = $('#TaxSlab_' + id).val();
    //defined tax values
    $.ajax({
        type: 'GET',
        async: false,
        url: '/AR/Api/GetTaxValues?id=' + taxId
    }).done(function (data) {
        //console.log(data);
        if (data != "NotFound") {
            $('#SalesTaxPercentage_' + id).val((data.salesTaxPercentage).toFixed(2));
            $('#ExciseTaxPercentage_' + id).val((data.exciseTaxPercentage).toFixed(2));
        }
        else {
            $('#SalesTaxPercentage_' + id).val(0.00);
            $('#ExciseTaxPercentage_' + id).val(0.00);
        }
    });
    //calculate sales tax
    var salesTaxPercentage = $('#SalesTaxPercentage_' + id).val();
    var salesTaxAmount = (salesTaxPercentage / 100) * $('#Total_' + id).val();
    $('#SalesTaxAmount_' + id).val(salesTaxAmount.toFixed(2));
    //calculate excise tax
    var exciseTaxPercentage = $('#ExciseTaxPercentage_' + id).val();
    var exciseTaxAmount = (exciseTaxPercentage / 100) * $('#Total_' + id).val();
    $('#ExciseTaxAmount_' + id).val(exciseTaxAmount.toFixed(2));
    //Calculate total remaining amount after taxes and discount
    var disc = parseFloat($('#DiscountAmount_' + id).val() == null ? 0 : $('#DiscountAmount_' + id).val());
    var saleTax = parseFloat($('#SalesTaxAmount_' + id).val());
    var exciseTax = parseFloat($('#ExciseTaxAmount_' + id).val());
    var lessAmount = parseFloat((saleTax + exciseTax) - disc);
    var netValue = parseFloat($('#Total_' + id).val());
         $('#LineTotal_' + id).val(Number(netValue + lessAmount).toFixed(2));
    calculateGrandTotal();
}
function calculateGrandTotal() {
    var totalAmount = 0;
    var netValue = 0;
    var discountAmount = 0;
    var salesTaxAmount = 0;
    var exciseTaxAmount = 0;
    $("input[name='LineTotal']").each(function () {
        totalAmount = (Number(totalAmount) + Number($(this).val())).toFixed(2);
        $('#GrandTotal').val(totalAmount);
    });
    $("input[name='Total_']").each(function () {
        netValue = (Number(netValue) + Number($(this).val())).toFixed(2);
        $('#Total').val(netValue);
    });
    $("input[name='DiscountAmount']").each(function () {
        discountAmount = (Number(discountAmount) + Number($(this).val())).toFixed(2);
        $('#discountAmount').val(discountAmount);
    });
    $("input[name='SalesTaxAmount']").each(function () {
        salesTaxAmount = (Number(salesTaxAmount) + Number($(this).val())).toFixed(2);
        $('#salesTaxAmount').val(salesTaxAmount);
    });
    $("input[name='ExciseTaxAmount']").each(function () {
        exciseTaxAmount = (Number(exciseTaxAmount) + Number($(this).val())).toFixed(2);
        $('#exciseTaxAmount').val(exciseTaxAmount);
    });
}
