var ids = [];
$('.remove-item').click(function () {
    //var element = this.id;
    //var id = element.replace("remove-item", "");
    ////ids to remove
    //var itemId = $('#SRItemId' + id).val();
    //if (itemId != null) {
    //    ids.push(itemId);
    //}
    ////amounts of netValues removed
    //var removedNetValue = $('#Total_' + id).val();
    //if (removedNetValue != null) {
    //    calculateTotalNetValue(removedNetValue);
    //}
    ////amounts of sales taxes removed
    //var removedSalesTax = $('#SalesTaxAmount_' + id).val();
    //if (removedSalesTax != null) {
    //    calculateTotalSalesTax(removedSalesTax);
    //}
    ////amounts of Discounts removed
    //var removedDiscount = $('#DiscountAmount_' + id).val();
    //if (removedDiscount != null) {
    //    calculateTotalDiscountAmount(removedDiscount);
    //}
    ////amount of LineTotal_ remove
    //var removedAmount = $('#LineTotal_' + id).val();
    //if (removedAmount != null) {
    //    calculateTotalAmount(removedAmount);
    //}
    ////amounts of items removed
    //$('#IdsDeleted').text(ids);
    //$("#sale-return" + id).remove();

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
    var removedExciseTax = 0; //$('#ExciseTaxAmount_' + id).val();

    var pageTotal = $('#Total').val();
    var pageDsicount = $('#discountAmount').val();
    var pageSalesTax = $('#salesTaxAmount').val();
    var pageExciseAmount = $('#exciseTaxAmount').val();
    var pageGrand = $('#GrandTotal').val();
    //Assign values
    $('#Total').val(Number(pageTotal - removedTotalValue).toFixed(digits));
    $('#discountAmount').val(Number(pageDsicount - removedDiscount).toFixed(digits));
    $('#salesTaxAmount').val(Number(pageSalesTax - removedSalesTax).toFixed(digits));
    $('#exciseTaxAmount').val(Number(pageExciseAmount - removedExciseTax).toFixed(digits));
    $('#GrandTotal').val(Number(pageGrand - removedAmount).toFixed(digits));
    // console.log(totalAmount);
    $('#IdsDeleted').text(ids);
    $("#sale-return" + id).remove();
});

function getItemDetails(val) { 
    var element = val.replace("ItemId", "");
    var id = $('#' + val).val();
    $.ajax({
        url: '/AR/SalesReturn/GetItemDetails?id=' + id,
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
        var returnDate = $('#ReturnDate').val();
        var stock = getStock(id, wareHouseId, returnDate);
        stock.done(function (data) {
            //console.log(data);
            $('#Stock_' + element).val(data.toFixed(2));
        });
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

$('.calculate-total').on('keyup', function () {
    debugger;
    var elementId = this.id;
    var id = elementId.split('_');
    id = id[1];

    var qty = $('#Qty_' + id + '').val();
    var rate = $('#Rate_' + id + '').val();
    var netValueBefore = $('#Total_' + id).val();
    $('#Total_' + id).val((qty * rate).toFixed(2));
    var netValueAfter = $('#Total_' + id).val();
    TotalNetValue(netValueBefore, netValueAfter);
    //After selecting tax slab calculating other values
    var discountPercentage = $('#DiscountPercentage_' + id).val();
    var beforeDiscount = $('#DiscountAmount_' + id).val();
    var discountAmount = (discountPercentage / 100) * $('#Total_' + id).val();
    var afterDiscount = discountAmount;
    calculateDiscount(beforeDiscount, afterDiscount);
    if (discountPercentage == 0) { $('#DiscountAmount_' + id).val(0); }
    $('#DiscountAmount_' + id).val(discountAmount.toFixed(2));
    //items.avgRate x qty
    var avgRate = $('#IssueRate_' + id).val();
    $('#CostofSales_' + id).val((avgRate * qty).toFixed(2));
    //calculateTax(elementId);
});

//calculate Total
function TotalNetValue(before, after) {
    //    New Net Value
    var totalNetValue = $('#Total').val();
    var remainingNetValue = totalNetValue - before;

    var newNetValue = (Number(remainingNetValue) + Number(after)).toFixed(2);
    $('#Total').val(newNetValue);
}
//Calculate Discount
function calculateDiscount(before, after) {
    var totalDiscount = $('#TotalDiscountAmount').val();
    var remainingDiscount = totalDiscount - before;

    var newDiscount = (Number(remainingDiscount) + Number(after)).toFixed(2);
    $('#TotalDiscountAmount').val(newDiscount);
}
//Calculate total Sales Tax
function totalSalesTaxAmount(before, after) {
    var totalSalesTaxAmount = $('#TotalSaleTaxAmount').val();
    var remainingSalesTaxAmount = totalSalesTaxAmount - before;

    var newSalesTaxAmount = (Number(remainingSalesTaxAmount) + Number(after)).toFixed(2);
    $('#TotalSaleTaxAmount').val(newSalesTaxAmount);
}
var slabVal = 0.00;


////Calculating total of all the amounts at present page
//var totalAmount = 0;
//var netValue = 0;
//var discountAmount = 0;
//var salesTaxAmount = 0;
//var exciseTaxAmount = 0;
//$("input[name='LineTotal']").each(function () {
//    totalAmount = (Number(totalAmount) + Number($(this).val())).toFixed(2);
//    $('#GrandTotal').val(totalAmount);
//});
//$("input[name='Total_']").each(function () {
//    netValue = (Number(netValue) + Number($(this).val())).toFixed(2);
//    $('#Total').val(netValue);
//});
//$("input[name='DiscountAmount']").each(function () {
//    discountAmount = (Number(discountAmount) + Number($(this).val())).toFixed(2);
//    $('#TotalDiscountAmount').val(discountAmount);
//});
//$("input[name='SalesTaxAmount']").each(function () {
//    salesTaxAmount = (Number(salesTaxAmount) + Number($(this).val())).toFixed(2);
//    $('#TotalSaleTaxAmount').val(salesTaxAmount);
//});

////functions to calculate page level total against different amounts
//function calculateTotalAmount(lessAmount) {
//    var totalAmount = $('#GrandTotal').val();
//    var remainingAmount = totalAmount - lessAmount;
//    $('#GrandTotal').val(remainingAmount);
//}
//function calculateTotalNetValue(lessAmount) {
//    var netValue = $('#Total').val();
//    var remainingAmount = netValue - lessAmount;
//    $('#Total').val(remainingAmount);
//}
//function calculateTotalDiscountAmount(lessAmount) {
//    var discountAmount = $('#TotalDiscountAmount').val();
//    var remainingAmount = discountAmount - lessAmount;
//    $('#TotalDiscountAmount').val(remainingAmount);
//}
//function calculateTotalSalesTax(lessAmount) {
//    var salesTaxAmount = $('#TotalSaleTaxAmount').val();
//    var remainingAmount = salesTaxAmount - lessAmount;
//    $('#TotalSaleTaxAmount').val(remainingAmount);
//}



function calculateLineTotal(id) {
    var qty = $('#Qty_' + id).val();
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


function calculateTax(id) {
    //tax slab defined
    var taxId = $('#TaxSlab_' + id).val();
    //define tax values
    $.ajax({
        type: 'GET',
        async: false,
        url: '/AR/Api/GetTaxValues?id=' + taxId
    }).done(function (data) {
        if (data != "NotFound") {
            $('#SalesTaxPercentage_' + id).val((data.salesTaxPercentage).toFixed(2));
            //$('#ExciseTaxPercentage_' + id).val((data.exciseTaxPercentage).toFixed(2));
        }
        else {
            $('#SalesTaxPercentage_' + id).val(0.00);
            //$('#ExciseTaxPercentage_' + id).val(0.00);
        }
    });
    //calculate sales tax
    var salesTaxPercentage = $('#SalesTaxPercentage_' + id).val();
    var salesTaxAmount = (salesTaxPercentage / 100) * $('#Total_' + id).val();
    $('#SalesTaxAmount_' + id).val(salesTaxAmount.toFixed(2));
    //calculate excise tax
    //var exciseTaxPercentage = $('#ExciseTaxPercentage_' + id).val();
    //var exciseTaxAmount = (exciseTaxPercentage / 100) * $('#Total_' + id).val();
    //$('#ExciseTaxAmount_' + id).val(exciseTaxAmount.toFixed(2));
    //Calculate total remaining amount after taxes and iscount
    var disc = parseFloat($('#DiscountAmount_' + id).val() == null ? 0 : $('#DiscountAmount_' + id).val());
    var saleTax = parseFloat($('#SalesTaxAmount_' + id).val());
    var exciseTax = 0; //parseFloat($('#ExciseTaxAmount_' + id).val());
    var lessAmount = parseFloat((saleTax + exciseTax) - disc);
    var netValue = parseFloat($('#Total_' + id).val());

    $('#LineTotal_' + id).val(Number(netValue - lessAmount).toFixed(2));
    calculateGrandTotal();
}

//Calculating total of all the amounts at present page
function calculateGrandTotal() {
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
        $('#TotalSalesTaxAmount').val(salesTaxAmount == null ? 0 : salesTaxAmount);
    });
    $("input[name='ExciseTaxAmount']").each(function () {
        exciseTaxAmount = (Number(exciseTaxAmount) + Number($(this).val())).toFixed(2);
        $('#exciseTaxAmount').val(exciseTaxAmount == null ? 0 : exciseTaxAmount);
    });
}