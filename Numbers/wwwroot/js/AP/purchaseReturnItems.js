var ids = [];

$('.remove-item').click(function () {
    var element = this.id;
    var id = element.replace("remove-item", "");
    //ids to remove
    var itemId = $('#PRItemId' + id).val();
    if (itemId != null) {
        ids.push(itemId);
    }
    //amounts of items removed
    $('#IdsDeleted').text(ids);
    $("#purchase-return" + id).remove();

    calculateGrandTotal();
});
function getItemDetails(val) {
    var element = val.replace("ItemId", "");
    var id = $('#' + val).val();
    $.ajax({
        url: '/AP/PurchaseReturn/GetItemDetails?id=' + id,
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
        $('select[id=TaxSlab_' + counter + ']').prop("disabled", "disabled");
    }
    else {
        $('.dependent').removeAttr('readonly', 'readonly');
        $('select[id=TaxSlab_' + counter + ']').removeAttr("disabled", "disabled");
    }
}





function calculateLineTotal(id) {
    var qty = $('#Qty_' + id).val();
    var rate = $('#Rate_' + id).val();
    $('#Total_' + id).val((qty * rate).toFixed(2));
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
    var disc = 0; //parseFloat($('#DiscountAmount_' + id).val() == null ? 0 : $('#DiscountAmount_' + id).val());
    var saleTax = parseFloat($('#SalesTaxAmount_' + id).val());
    var exciseTax = 0; //parseFloat($('#ExciseTaxAmount_' + id).val());
    var lessAmount = parseFloat((saleTax + exciseTax) - disc);
    var netValue = parseFloat($('#Total_' + id).val());

    $('#LineTotal_' + id).val(Number(netValue + lessAmount).toFixed(2));
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
        $('#TotalSaleTaxAmount').val(salesTaxAmount == null ? 0 : salesTaxAmount);
    });
    $("input[name='ExciseTaxAmount']").each(function () {
        exciseTaxAmount = (Number(exciseTaxAmount) + Number($(this).val())).toFixed(2);
        $('#exciseTaxAmount').val(exciseTaxAmount == null ? 0 : exciseTaxAmount);
    });
}