var ids = [];

$('.remove-item').click(function () {
    var element = this.id;
    var id = element.replace("remove-item", "");
    //ids to remove
    var itemId = $('#PRItemId' + id).val();
    if (itemId != null) {
        ids.push(itemId);
    }
    var digits = 0;
    var removedTotal = $('#Total_' + id).val();

    var removedSalesTax = $('#TaxAmount_' + id).val();

    var removedLine = $('#LineTotal_' + id).val();


   var pageTotal = $('#Total').val();
    var pageTax = $('#TotalTaxAmount').val();
    var pageGrand = $('#GrandTotal').val();

    $('#Total').val(pageTotal - removedTotal);
    $('#TotalTaxAmount').val(pageTax - removedSalesTax);
    $('#GrandTotal').val(pageGrand - removedLine);


    $('#IdsDeleted').text(ids);
    $("#sales-Order" + id).remove();
});

function calculateLineTotal(id) {

    var qty = $('#Qty_' + id + '').val();
    var rate = $('#Rate_' + id + '').val();
    $('#Total_' + id).val((qty * rate).toFixed(2));
    //After selecting tax slab calculating other values
    $('#LineTotal_' + id).val($('#Total_' + id).val());
    //calculateTax(elementId);
    //
    $('#Total_' + id).val((qty * rate).toFixed(2));    //
    $('#LineTotal_' + id).val((qty * rate).toFixed(2));

    calculateTax(id);
}
var slabVal = 0.00;
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
            slabVal = data.salesTaxPercentage.toFixed(2);
        }
        else {
            slabVal = 0.00;
        }
    });   
    var total = $('#Total_' + id).val();
    var taxAmount = ((slabVal / 100) * total).toFixed(2);
    var lineTotal = (Number(total) + Number(taxAmount)).toFixed(2);
    $('#LineTotal_' + id).val(lineTotal);
    //
    $('#TaxAmount_' + id).val(taxAmount);
    //
    calculateGrandTotal();
}

function getItemDetails(val) {
    var element = val.replace("ItemId", "");
    var id = $('#' + val).val();
    $.ajax({
        url: '/AR/SaleOrder/GetItemDetails?id=' + id,
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
function calculateGrandTotal(){
    var total = 0;
    var taxAmount = 0;
    var grandTotal = 0;
    $("input[name='Total_']").each(function () {
        total = (Number(total) + Number($(this).val())).toFixed(2);
        $('#Total').val(total);
    });
    $("input[name='TaxAmount']").each(function () {
        taxAmount = (Number(taxAmount) + Number($(this).val())).toFixed(2);
        $('#TotalTaxAmount').val(taxAmount);
    });
    $("input[name='LineTotal']").each(function () {
        grandTotal = (Number(grandTotal) + Number($(this).val())).toFixed(2);
        $('#GrandTotal').val(grandTotal);
    });
}