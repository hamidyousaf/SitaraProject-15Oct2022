var ids = [];
$('.remove-item').click(function () {
    var elementId = this.id;
    console.log(elementId);
    var id = elementId.split('_');
    id = id[1];
    //ids to remove from database
    var itemId = $('#PurchaseItemId' + id).val();
    if (itemId != null && itemId > 0) {
        ids.push(itemId);
    }
    $('#IdsDeleted').text(ids);
    var digits = 2;
    var itemTotal = $('#Total_' + id).val();
    var lineDisc = $("#DiscountAmount_" + id).val();
    var lineSales = $("#SalesTaxAmount_" + id).val();
    var lineTotal = $("#LineTotal_" + id).val();
    //
    var pageTotal = ($('#Total').val() - itemTotal).toFixed(digits);
    var pageDiscount = ($('#discountAmount').val() - lineDisc).toFixed(digits);
    var pageSales = ($('#salesTaxAmount').val() - lineSales).toFixed(digits);
    var pageGrand = ($('#GrandTotal').val() - lineTotal).toFixed(digits);

    $('#Total').val(pageTotal);
    $('#discountAmount').val(pageDiscount);
    $('#salesTaxAmount').val(pageSales);
    $('#GrandTotal').val(pageGrand);

    $("#purchase-items_" + id).remove();
    //calculateTax(elementId);

});

function getItemDetails(val) {
    debugger;
    var element = val.replace("ServiceAccountId", "");
    if ($('#ServiceAccountId' + element).val() == null) {
        $('.dependent').html(0);
    }
    else {
        $('.dependent').removeAttr('readonly', 'readonly');
    }
}

function calculateTax(elementId) {
    var id = elementId.split('_');
    id = id[1];
    //tax slab defined
    var taxId = slabVal = $('#TaxSlab_' + id).val();
    //define tax values
    $.ajax({
        type: 'GET',
        async: false,
        url: '/AR/Api/GetTaxValues?id=' + taxId
    }).done(function (data) {
        if (data != "NotFound") {
            $('#SalesTaxPercentage_' + id).val((data.salesTaxPercentage).toFixed(2));
        }
        else {
            $('#SalesTaxPercentage_' + id).val(0.00);
        }
        calculateLineTotal(id)
    });
}
function calculateLineTotal(lineId) {
    debugger
    var digits = 2; //up to two digit will be dynamic later.
    var taxPercentage = 0;
    var taxPercentage = 0;
    var lineTotal = 0;
    var total = $("#Total_" + lineId).val();
    total = (total == "" ? 0 : total);
    var discountPercentage = $('#DiscountPercentage_' + lineId).val();
    discountPercentage = (discountPercentage == "" ? 0 : discountPercentage);
    var discountAmount = ((discountPercentage / 100) * total).toFixed(digits);
    total = total - discountAmount;
    var taxPercentage = $('#SalesTaxPercentage_' + lineId).val();
    taxPercentage = (taxPercentage == "" ? 0 : taxPercentage);
    var taxAmount = ((taxPercentage / 100) * total).toFixed(digits);
    var lineTotal = (parseFloat(total) + parseFloat(taxAmount)).toFixed(digits);

    $("#DiscountAmount_" + lineId).val(discountAmount);
    $("#SalesTaxAmount_" + lineId).val(taxAmount);
    $("#LineTotal_" + lineId).val(lineTotal);
    calculateGrandTotal();
}
function calculateGrandTotal() {
    //Calculating total of all the amounts at present page
    var totalAmount = 0;
    var netValue = 0;
    var discountAmount = 0;
    var salesTaxAmount = 0;
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
}
