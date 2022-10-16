var ids = [];
//var GrandTotal = 0.00;
$('.remove-item').click(function () {
    var element = this.id;
    var id = element.replace("remove-item", "");
    //ids to remove
    var itemId = $('#InvoiceItemId' + id).val();
    if (itemId != null) {
        ids.push(itemId);
    }
    var digits = 0;
    //amounts of netValues removed
    var removedTotalValue = $('#Total_' + id).val();

    //amounts of Discounts removed
    var removedDiscount = $('#DiscountAmount_' + id).val();

    //amounts of sales taxes removed
    var removedSalesTax = $('#SalesTaxAmount_' + id).val();

    //amounts of items removed
    var removedAmount = $('#LineTotal_' + id).val();

    //amounts of Excise Tax removed from the page
    var removedExciseTax = $('#ExciseTaxAmount_' + id).val();

    var pageTotal = $('#Total').val();
    var pageDsicount = $('#discountAmount').val();
    var pageSalesTax = $('#salesTaxAmount').val();
    var pageExciseAmount = $('#exciseTaxAmount').val();
    var pageGrand = $('#GrandTotal').val();
    //Assign values
    $('#Total').val(Number(pageTotal - removedTotalValue).toFixed(digits));
    $('#discountAmount').val(Number(pageDsicount - removedDiscount).toFixed(digits));
    $('#totalExcvAmt').val(Number(pageTotal - pageDsicount).toFixed(digits));
    $('#salesTaxAmount').val(Number(pageSalesTax - removedSalesTax).toFixed(digits));
    $('#exciseTaxAmount').val(Number(pageExciseAmount - removedExciseTax).toFixed(digits));
    $('#GrandTotal').val(Number(pageGrand - removedAmount).toFixed(digits));
    // console.log(totalAmount);
    $('#IdsDeleted').text(ids);
    $("#sale-invoice" + id).remove();
 
});
function getUOM(val) {
    var element = val.replace("ItemId", "");
    var id = $('#' + val).val();//$('#ItemId0')
    $.ajax({
        url: '/AR/Invoice/GetUOM?id=' + id,
        type: 'GET'
    }).done(function (data) {
        $('.UOM' + element).val(data[0].id);
        $('#UOM' + element).html(data[0].uom);
        $('#IssueRate_' + element).val(data[0].avgRate);
    });
    if ($('#ItemId' + element).val() == null) {
        $('.dependent').prop('readonly', 'readonly');
    }
    else {
        $('.dependent').removeAttr('readonly', 'readonly');
        $('select[id=TaxSlab_' + element + ']').removeAttr("disabled", "disabled");
        var wareHouseId = $('#WareHouseId').val();
        var invoiceDate = $('#InvoiceDate').val();
        var stock = getStock(id, wareHouseId, invoiceDate);
        stock.done(function (data) {
            $('#Stock_' + element).val(data.toFixed(2));
        });
    } 
}

function calculateLineTotal(id) {

    var qty = $('#Qty_'+id).val();
    var rate = $('#Rate_' + id).val();
    $('#Total_' + id).val((qty * rate).toFixed(2));
    //After selecting tax slab calculating other values
    var discountPercentage = $('#DiscountPercentage_' + id).val();
    var discountAmount = (discountPercentage / 100) * $('#Total_' + id).val();
    if (discountPercentage == 0) { $('#DiscountAmount_' + id).val(0); }
    $('#DiscountAmount_' + id).val(discountAmount.toFixed(2));
    //items.avgRate x qty
    var avgRate = $('#IssueRate_' + id).val();
    $('#CostofSales_' + id).val((avgRate * qty).toFixed(2));
    calculateTax(id);    
}
function calculatlineTotal(id) {

    var qty = $('#Qty_' + id).val();
    var rate = $('#Rate_' + id).val();
    $('#Total_' + id).val((qty * rate).toFixed(2));
    /*//After selecting tax slab calculating other values
    var discountPercentage = $('#DiscountPercentage_' + id).val();
    var discountAmount = (discountPercentage / 100) * $('#Total_' + id).val();
    if (discountPercentage == 0) { $('#DiscountAmount_' + id).val(0); }
    $('#DiscountAmount_' + id).val(discountAmount.toFixed(2));*/
    //items.avgRate x qty
    var avgRate = $('#IssueRate_' + id).val();
    $('#CostofSales_' + id).val((avgRate * qty).toFixed(2));
    calculateTax(id);
}
function calculateTax(id) {
     //tax slab defined
    var taxId = slabVal = $('#TaxSlab_' + id).val();
    //define tax values
    $.ajax({
        type: 'GET',
        async:false,
        url: '/AR/Api/GetTaxValues?id=' + taxId
    }).done(function (data) {
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
    var exciseTaxPercentage=$('#ExciseTaxPercentage_' + id).val();
    var exciseTaxAmount = (exciseTaxPercentage / 100) * $('#Total_' + id).val();
    $('#ExciseTaxAmount_' + id).val(exciseTaxAmount.toFixed(2));
    //Calculate total remaining amount after taxes and iscount
    var disc = parseFloat($('#DiscountAmount_' + id).val() == null ? 0 : $('#DiscountAmount_' + id).val());
    var saleTax = parseFloat($('#SalesTaxAmount_' + id).val());
    var exciseTax = parseFloat($('#ExciseTaxAmount_' + id).val());
    var lessAmount = parseFloat((saleTax + exciseTax)-disc);
    var netValue = parseFloat($('#Total_' + id).val());

    $('#LineTotal_' + id).val(Number(netValue + lessAmount).toFixed(2));
    calculateGrandTotal();
}

//Calculating total of all the amounts at present page
function calculateGrandTotal() {
    debugger
    var totalAmount = 0;
    var netValue = 0;
    var discountAmount = 0;
    var salesTaxAmount = 0;
    var exciseTaxAmount = 0;
    $("input[name='LineTotal']").each(function () {
        totalAmount = (Number(totalAmount) + Number($(this).val())).toFixed(2);
        $('#GrandTotal').val(totalAmount == null ? 0 : totalAmount);
    });
    $("input[name='Total_']").each(function () {
        netValue = (Number(netValue) + Number($(this).val())).toFixed(2);
        $('#Total').val(netValue == null ? 0 : netValue);
    });
    $("input[name='DiscountAmount']").each(function () {
        discountAmount = (Number(discountAmount) + Number($(this).val())).toFixed(2);
        $('#discountAmount').val(discountAmount == null ? 0 : discountAmount);
    });
    $("input[name='SalesTaxAmount']").each(function () {
        salesTaxAmount = (Number(salesTaxAmount) + Number($(this).val())).toFixed(2);
        $('#salesTaxAmount').val(salesTaxAmount == null ? 0 : salesTaxAmount);
    });
    $("input[name='ExciseTaxAmount']").each(function () {
        exciseTaxAmount = (Number(exciseTaxAmount) + Number($(this).val())).toFixed(2);
        $('#exciseTaxAmount').val(exciseTaxAmount == null ? 0 : exciseTaxAmount);
    });
    debugger
    var pageTotal = parseFloat(Number($('#Total').val()));
    var pageDsicount = parseFloat(Number($('#discountAmount').val()));
    $('#totalExcvAmt').val(Number(pageTotal - pageDsicount).toFixed(2));
    var exvAMt = parseFloat(Number($('#totalExcvAmt').val()));
    var toAleTax = parseFloat(Number($('#salesTaxAmount').val()));
    $('#GrandTotal').val(Number(exvAMt + toAleTax).toFixed(2));
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
$(document).ready(function () {
    calculateGrandTotal();
});